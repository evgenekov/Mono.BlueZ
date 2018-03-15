using System;
using System.Collections.Generic;

using DBus;
using Mono.BlueZ.Console.Test;

namespace Mono.BlueZ.Console.Communication
{
    public class MtuCharacteristic : Characteristic
    {
        private const string mtuUUID = "12345678-1234-5678-0005-56789abcdef1";
        private static readonly string[] flags = { "read", "write" };
        private MtuValue mtuValue;

        public MtuCharacteristic(Bus bus, int index, ObjectPath service, MtuValue mtuValue) : base(bus, index, mtuUUID, flags, service)
        {
            this.mtuValue = mtuValue;
            base.Value = BitConverter.GetBytes(mtuValue.Mtu);
        }

        public override byte[] ReadValue(IDictionary<string, object> options)
        {
            return BitConverter.GetBytes(mtuValue.Mtu);
        }

        public override void WriteValue(byte[] value, IDictionary<string, object> options)
        {
            Value = value;
        }

        public override byte[] Value 
        { 
            get 
            {
                System.Console.WriteLine("GetValue: " + mtuValue.Mtu);
                return base.Value;   
            } 
            protected set 
            {
                mtuValue.Mtu = BitConverter.ToInt32(value, 0);
                System.Console.WriteLine("SetValue: " + mtuValue.Mtu);
                base.Value = value;   
            } 
        }
    }
}
