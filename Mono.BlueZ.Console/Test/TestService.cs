using DBus;

using Mono.BlueZ.Console.Communication;

namespace Mono.BlueZ.Console.Test
{
    public class TestService : Service
    {
        private const bool primary = true;
        private const string testServiceUUID = "12345678-1234-5678-1234-56789abcdef0";
        private const int dataSize = 3000;
        private byte[] dataToDownload;
        private MtuValue mtuValue;

        private CommandCharacteristic commandCharacteristic;
        private LengthCharacteristic lengthCharacteristic;
        private DataCharacteristic dataCharacteristic;
        private MtuCharacteristic mtuCharacteristic;

        public TestService(Bus bus, int index) : base(bus, index, testServiceUUID, primary)
        {
            mtuValue = new MtuValue();
            SetData();
            AddCommandCharacteristic();
            AddLengthCharacteristic();
            AddDataCharacteristic();
            AddMtuCharacteristic();
        }

        private void AddCommandCharacteristic()
        {
            commandCharacteristic = new CommandCharacteristic(Bus, 1, GetPath());

            AddCharacteristic(commandCharacteristic);
        }

        private void AddLengthCharacteristic()
        {
            lengthCharacteristic = new LengthCharacteristic(Bus, 2, GetPath());
            lengthCharacteristic.SetLength((uint)dataToDownload.Length);

            AddCharacteristic(lengthCharacteristic);
        }

        private void AddDataCharacteristic()
        {
            dataCharacteristic = new DataCharacteristic(Bus, 3, GetPath(), mtuValue);
            dataCharacteristic.SetData(dataToDownload);

            AddCharacteristic(dataCharacteristic);
        }

        private void AddMtuCharacteristic()
        {
            mtuCharacteristic = new MtuCharacteristic(Bus, 4, GetPath(), mtuValue);

            AddCharacteristic(mtuCharacteristic); ;
        }

        private void SetData()
        {
            dataToDownload = new byte[dataSize];
        }
    }

    public class MtuValue
    {
        public MtuValue()
        {
            Mtu = 20;
        }

        public int Mtu { get; set; }
    }
}
