using System.Collections.Generic;

using DBus;

namespace Mono.BlueZ.Console.Communication
{
   public class CommandCharacteristic : Characteristic
   {
      private const string commandUUID = "12345678-1234-5678-0001-56789abcdef1";

      private static readonly string[] flags = { "write" };

      public CommandCharacteristic(Bus bus, int index, ObjectPath service) : base(bus, index, commandUUID, flags, service)
      {

      }

      public override void WriteValue(byte[] value, IDictionary<string, object> options)
      {
         Value = value;
      }

      public override byte[] Value { get => base.Value; protected set => base.Value = value; }
   }
}
