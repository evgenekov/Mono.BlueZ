using DBus;

using Mono.BlueZ.Console.Communication;

namespace Mono.BlueZ.Console.Test
{
   public class TestService : Service
   {
      private const bool primary = true;
      private const string testServiceUUID = "12345678-1234-5678-1234-56789abcdef0";
      private const int dataSize = 1024;
      private byte[] dataToDownload;

      private CommandCharacteristic commandCharacteristic;
      private LengthCharacteristic lengthCharacteristic;
      private DataCharacteristic dataCharacteristic;

      public TestService(Bus bus, int index) : base(bus, index, testServiceUUID, primary)
      {
         SetData();
         AddCommandCharacteristic();
         AddLengthCharacteristic();
         AddDataCharacteristic();
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
         dataCharacteristic = new DataCharacteristic(Bus, 3, GetPath());
         dataCharacteristic.SetData(dataToDownload);

         AddCharacteristic(dataCharacteristic);
      }

      private void SetData()
      {
         dataToDownload = new byte[dataSize];
      }
   }
}
