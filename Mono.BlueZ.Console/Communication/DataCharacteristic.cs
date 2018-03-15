using System;
using System.Collections.Generic;

using DBus;
using Mono.BlueZ.Console.Test;

namespace Mono.BlueZ.Console.Communication
{
    public class DataCharacteristic : Characteristic
    {
        private const string dataUUID = "12345678-1234-5678-0003-56789abcdef1";
        private static readonly string[] flags = { "read" };
        private byte[] dataToDownload;
        private int lastIndex = 0;
        private MtuValue mtuValue;

        public DataCharacteristic(Bus bus, int index, ObjectPath service, MtuValue mtuValue) : base(bus, index, dataUUID, flags, service)
        {
            this.mtuValue = mtuValue;
        }

        internal void SetData(byte[] dataToDownload)
        {
            this.dataToDownload = dataToDownload;
        }

        public override byte[] ReadValue(IDictionary<string, object> options)
        {
            var valueToReturn = new byte[mtuValue.Mtu];

            var nextIndexAfterDownload = lastIndex + mtuValue.Mtu;

            if ((nextIndexAfterDownload) >= dataToDownload.Length)
            {
                var numberOfBytesRemaining = dataToDownload.Length - lastIndex;
                valueToReturn = new byte[numberOfBytesRemaining];
            }

            Buffer.BlockCopy(dataToDownload, lastIndex, valueToReturn, 0, valueToReturn.Length);

            lastIndex += mtuValue.Mtu;

            return dataToDownload;
        }
    }
}
