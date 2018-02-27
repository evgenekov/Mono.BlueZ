using System;
using System.Collections.Generic;

using DBus;

namespace Mono.BlueZ.Console.Test
{
    public class TestCharacteristic : Characteristic
    {
        private const string testCharacteristicUUID = "12345678-1234-5678-1234-56789abcdef1";
        private static readonly string[] flags = { "read", "write", "writable-auxiliaries", "notify" };
        private IDictionary<string, object> changed_properties;
        private IDictionary<string, object> current_properties;
        private List<string> invalidated_properties;

        private byte[] data;
        private const int dataSize = 20;
        private Random randomData = new Random();
        private bool notifying;
        private int byteArrayIndex;


        private System.Threading.Thread updateNotifyThread;

        public TestCharacteristic(Bus bus, int index, ObjectPath service) : base(bus, index, testCharacteristicUUID, flags, service)
        {
            AddDescriptor(new TestDescriptor(bus, 0, GetPath()));
            changed_properties = new Dictionary<string, object>();
            current_properties = new Dictionary<string, object>();
            invalidated_properties = new List<string>();
            data = new byte[dataSize];
        }

        public override byte[] ReadValue(IDictionary<string, object> options)
        {
            return Value;
        }

        public override void WriteValue(byte[] value, IDictionary<string, object> options)
        {
            Value = value;
        }

        public override void StartNotify()
        {
            if (notifying)
            {
                System.Console.WriteLine("Already notifying, nothing to do.");
                return;
            }

            notifying = true;
            UpdateNotifying();
        }

        public override void StopNotify()
        {
            if (!notifying)
            {
                System.Console.WriteLine("Not notifying, nothing to do.");
                return;
            }

            notifying = false;
            UpdateNotifying();
        }

        private void UpdateNotifying()
        {
            if (!notifying)
            {
                if (updateNotifyThread != null && updateNotifyThread.IsAlive)
                {
                    updateNotifyThread.Abort();
                }
                return;
            }

            updateNotifyThread = new System.Threading.Thread(UpdateValue);
            updateNotifyThread.Start();
        }

        private void UpdateValue()
        {
            while (true)
            { 
                UpdateIncrementValue();
                AddPropertyChange(gattCharacteristic, new string[] { nameof(Value) });
                System.Threading.Thread.Sleep(100);
            }
        }

        private void UpdateIncrementValue()
        {
            if (data[byteArrayIndex] == 0xFF)
            {
                if (byteArrayIndex == (data.Length - 1))
                {
                    ResetDataArray();
                    byteArrayIndex = 0;
                }

                byteArrayIndex++;
            }

            data[byteArrayIndex] += 1;
        }

        private void ResetDataArray()
        {
            for (int index = 0; index < dataSize; index++)
            {
                data[index] = 0x00;
            }
        }

        public override byte[] Value
        {
            get
            {
                return data;   
            }
            protected set
            {
                data = value;
            }
        }

        public override object Get(string @interface, string propname)
        {
            if (@interface == gattCharacteristic)
            {
                switch (propname)
                {
                    case nameof(Value):
                        var newValue= new byte[Value.Length];
                        Array.Copy(Value, newValue, Value.Length);
                        return newValue;
                    default:
                        System.Console.WriteLine("Error while getting property name");
                        return null;
                }
            }

            return null;
        }

        private void AddPropertyChange(string interface_name, IEnumerable<string> property_names)
        {
            lock (changed_properties)
            {
                foreach (string prop_name in property_names)
                {
                    object current_value = null;
                    current_properties.TryGetValue(prop_name, out current_value);
                    var new_value = Get(interface_name, prop_name);
                    if ((current_value == null) || !(current_value.Equals(new_value)))
                    {
                        changed_properties[prop_name] = new_value;
                        current_properties[prop_name] = new_value;
                    }
                }
                if (changed_properties.Count > 0)
                {
                    HandlePropertiesChange(interface_name);
                }
            }
        }

        private void HandlePropertiesChange(string interface_name)
        {
            lock (changed_properties)
            {
                try
                {
                    OnPropertiesChanged(interface_name, changed_properties, invalidated_properties.ToArray());
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Error occured HandlePropertiesChanged " + e);
                }
                changed_properties.Clear();
                invalidated_properties.Clear();
            }
        }

        protected override void OnPropertiesChanged(string @interface, IDictionary<string, object> changed, string[] invalidated)
        {
            base.OnPropertiesChanged(@interface, changed, invalidated);
        }
    }
}