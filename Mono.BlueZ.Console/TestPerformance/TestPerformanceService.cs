using System;

using DBus;

namespace Mono.BlueZ.Console.TestPerformance
{
    public class TestPerformanceService : Service
    {
        private const bool primary = true;
        private const string uuid = "a0591000-6839-4bae-ab29-e781163c52c9";

        public TestPerformanceService(Bus bus) : base (bus, 0, uuid, primary)
        {
            AddCharacteristic(new TestPerformanceCharacteristic(bus, GetPath()));
        }
    }
}
