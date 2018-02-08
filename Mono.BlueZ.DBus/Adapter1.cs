using System;
using System.Collections.Generic;
using DBus;

namespace Mono.BlueZ.DBus
{
	[Interface("org.bluez.Adapter1")]
	public interface Adapter1
	{
		void StartDiscovery();
		void StopDiscovery();
        void RemoveDevice(ObjectPath device);
        void SetDiscoveryFilter(IDictionary<string, object> properties);
        string[] GetDiscoveryFilters();

		string Address { get; }
        string AddressType { get; }
        string Name { get; }
        string Alias { get; set; }
        uint Class { get; }
        bool Powered { get; set; }
        bool Discoverable { get; set; }
        bool Pairable { get; set; }
        uint PairableTimeout { get; set; }
        uint DiscoverableTimeout { get; set; }
        bool Discovering { get; }
        IList<string> UUIDs { get; }
		string Modalias { get; }
	}
}
