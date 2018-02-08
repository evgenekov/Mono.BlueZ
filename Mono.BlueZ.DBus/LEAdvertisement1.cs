using System.Collections.Generic;
using DBus;

namespace Mono.BlueZ.DBus
{
	// exposed by client application to advertise
	[Interface("org.bluez.LEAdvertisement1")]
	public interface LEAdvertisement1
	{
		void Release();

        string Type { get; set; }
        string[] ServiceUUIDs { get; set; }
        IDictionary<string, object> ManufacturerData { get; set; }
        string[] SolicitUUIDs { get; set; }
        IDictionary<string, object> ServiceData { get; set; }
        string[] Includes { get; set; }
        string LocalName { get; set; }
        int Appearance { get; set; }
        uint Duration { get; set; }
        uint Timeout { get; set; }
	}
}
