using System;
using System.Collections.Generic;

using DBus;

namespace Mono.BlueZ.Console.Communication
{
   public class DataCharacteristic : Characteristic
   {
      private const string dataUUID = "12345678-1234-5678-0003-56789abcdef1";
      private static readonly string[] flags = { "read" };

      private byte[] dataToDownload;
      private const int mtuSize = 512;
      private int lastIndex = 0;

      public DataCharacteristic(Bus bus, int index, ObjectPath service) : base(bus, index, dataUUID, flags, service)
      {

      }

      internal void SetData(byte[] dataToDownload)
      {
         this.dataToDownload = dataToDownload;
      }

      public override byte[] ReadValue(IDictionary<string, object> options)
      {
         var valueToReturn = new byte[mtuSize];

         var nextIndexAfterDownload = lastIndex + mtuSize;

         if ((nextIndexAfterDownload) >= dataToDownload.Length)
         {
            var numberOfBytesRemaining = dataToDownload.Length - lastIndex;
            valueToReturn = new byte[numberOfBytesRemaining];
         }

         Buffer.BlockCopy(dataToDownload, lastIndex, valueToReturn, 0, valueToReturn.Length);

         lastIndex += mtuSize;

         return valueToReturn;
      }
   }
}
