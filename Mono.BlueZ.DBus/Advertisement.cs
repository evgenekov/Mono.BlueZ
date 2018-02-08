using System;
using System.Collections.Generic;

using DBus;
using org.freedesktop.DBus;

namespace Mono.BlueZ.DBus
{
    public class Advertisement : LEAdvertisement1, Properties
    {
        protected const string leAdvertisement1 = "org.bluez.LEAdvertisement1";
        private string PATH_BASE = "/org/bluez/example/advertisement";
        private readonly string path;
        private List<string> includes;

        public event PropertiesChangedHandler PropertiesChanged;

        public Advertisement(Bus bus, int index, string advertisingType)
        {
            Type = advertisingType;
            path = PATH_BASE + index;

            LocalName = "TestAdvertisement";

            ServiceUUIDs = new string[] { "180D", "180F" };

            ManufacturerData = new Dictionary<string, object>();
            ManufacturerData.Add("0xFFFF", new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04 });

            ServiceData = new Dictionary<string, object>();
            ServiceData.Add("9999", new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04 });

            includes = new List<string>();
            includes.Add("local-name");
            includes.Add("tx-power");
            Includes = includes.ToArray();

            bus.Register(new ObjectPath(path), this);
        }

        public string Type { get; set; }
        public string[] ServiceUUIDs { get; set; }
        public IDictionary<string, object> ManufacturerData { get; set; }
        public string[] SolicitUUIDs { get; set; }
        public IDictionary<string, object> ServiceData { get; set; }
        public string[] Includes { get; set; }
        public int Appearance { get; set; }
        public uint Duration { get; set; }
        public uint Timeout { get; set; }
        public string LocalName { get; set; }

        public ObjectPath GetPath()
        {
            return new ObjectPath(path);
        }

        public void Release()
        {
            Console.WriteLine("Advertisement release!");
        }

        [return: Argument("props")]
        public IDictionary<string, object> GetAll(string @interface)
        {
            if (@interface != leAdvertisement1)
            {
                Console.WriteLine("Advertisement: Not the good interface for org.bluez.LEAdvertisement1. " + @interface);
                throw new ArgumentException();
            }

            Console.WriteLine("Advertisement: GetAll");

            var dict = new Dictionary<string, object>
            {
                { nameof(Type), Type },
                { nameof(ServiceUUIDs), ServiceUUIDs },
                { nameof(ManufacturerData), ManufacturerData },
                { nameof(ServiceData), ServiceData },
                { nameof(Includes), Includes },
                { nameof(LocalName), LocalName }
            };

            return dict;
        }

        [return: Argument("value")]
        public object Get(string @interface, string propname)
        {
            throw new NotImplementedException();
        }

        public void Set(string @interface, string propname, object value)
        {
            throw new NotImplementedException();
        }
    }
}
