using System;
using DBus;

namespace Mono.BlueZ.Console.TestPerformance
{
    public class TestPerformanceCharacteristic : Characteristic
    {
        private const string uuid = "a0591100-6839-4bae-ab29-e781163c52c9";
        private static readonly string[] flags = { "notify" };
        private byte[] data;

        public TestPerformanceCharacteristic(Bus bus, ObjectPath service) : base(bus, 0, uuid, flags, service)
        {
            data = new byte[0];
        }

    }
}
