using System;
using System.Collections.Generic;

using DBus;

namespace Mono.BlueZ.DBus
{
    public class Advertisement : LEAdvertisement1
    {
        private const string leAdvertisement1 = "org.bluez.LEAdvertisement1";
        private string PATH_BASE = "/org/bluez/example/advertisement";
        private readonly string path;
        private List<string> includes;

        public Advertisement(Bus bus, int index, string advertisingType)
        {
            Type = advertisingType;
            path = PATH_BASE + index;

            LocalName = "TestAdvertisement";

            ServiceUUIDs = new string[] { };

            ManufacturerData = new Dictionary<string, object>();

            ServiceData = new Dictionary<string, object>();

            SolicitUUIDs = new string[] { };

            includes = new List<string>();
            Includes = includes.ToArray();

            Timeout = 60;

            bus.Register(new ObjectPath(path), this);
            Console.WriteLine("Advertisement registred.");
        }

        public string Type { get; set; }
        public string[] ServiceUUIDs { get; set; }
        public IDictionary<string, object> ManufacturerData { get; set; }
        public string[] SolicitUUIDs { get; set; }
        public IDictionary<string, object> ServiceData { get; set; }
        public string[] Includes { get; set; }
        public ushort Appearance { get; set; }
        public ushort Duration { get; set; }
        public ushort Timeout { get; set; }
        public string LocalName { get; set; }

        public ObjectPath GetPath()
        {
            return new ObjectPath(path);
        }

        public void Release()
        {
            Console.WriteLine("Advertisement released.");
        }

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
    }
}
