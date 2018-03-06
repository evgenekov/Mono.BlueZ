using System;
using System.Collections.Generic;

using DBus;

namespace Mono.BlueZ.Console.Communication
{
   public class LengthCharacteristic : Characteristic
   {
      private const string testCharacteristicUUID = "12345678-1234-5678-0002-56789abcdef1";
      private static readonly string[] flags = { "read" };
      private uint length;

      public LengthCharacteristic(Bus bus, int index, ObjectPath service) : base(bus, index, testCharacteristicUUID, flags, service)
      {

      }

      public void SetLength(uint length)
      {
         this.length = length;
      }

      public override byte[] ReadValue(IDictionary<string, object> options)
      {
         return BitConverter.GetBytes(length);
      }
   }
}
