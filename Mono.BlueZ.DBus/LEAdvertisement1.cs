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
        ushort Appearance { get; set; }
        ushort Duration { get; set; }
        ushort Timeout { get; set; }
	}
}
