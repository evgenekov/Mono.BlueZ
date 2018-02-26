using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DBus;
using Mono.BlueZ.DBus;
using org.freedesktop.DBus;

namespace Mono.BlueZ.Console
{
    public class GattServer
    {
        private Bus system;
        public Exception _startupException { get; private set; }
        private const string SERVICE = "org.bluez";
        private ObjectPath connectedDevicePath;
        private ObjectPath appObjectPath;
        private ObjectPath advertisingManagerPath;
        private ObjectPath adapterFound;
        private LEAdvertisingManager1 advertisementManager;

        public void Run()
        {
            GattManager1 gattManager = null;

            try
            {
                System.Console.WriteLine("Fetching objects");

                // 1. Find adapter
                var getBusSystemTask = Task.Run(() => system = Bus.System);
                    
                var mainTask = getBusSystemTask.ContinueWith((antecedent) => 
                {
                    if (antecedent.IsFaulted)
                    {
                        System.Console.WriteLine(antecedent.Exception);
                        return;
                    }

                    adapterFound = FindAdapter();

                    if (adapterFound == null)
                    {
                        System.Console.WriteLine("Couldn't find adapter that supports LE");
                        return;
                    }

                    gattManager = GetObject<GattManager1>(SERVICE, adapterFound);
                    advertisementManager = GetObject<LEAdvertisingManager1>(SERVICE, adapterFound);
                                      
                    // 2. Start advertising
                    StartAdvertising();

                    // 4. Start application
                    StartApplication(gattManager);

                    while (true)
                    {
                        system.Iterate();
                    }
                });

                mainTask.Wait();
            }
            catch (Exception exception)
            {
                System.Console.WriteLine(exception);
            }
            finally
            {
                if (gattManager != null)
                {
                    gattManager.UnregisterApplication(appObjectPath);    
                }

                StopAdvertising();
            }
        }

        private void StartAdvertising()
        {
            var adapter_properties = GetObject<Adapter1>(SERVICE, adapterFound);
            adapter_properties.Powered = true;

            var advertisement = new Advertisement(system, 0, "peripheral");
            advertisingManagerPath = advertisement.GetPath();
            var options = new Dictionary<string, object>();

            advertisementManager.RegisterAdvertisement(advertisingManagerPath, options);
        }

        private void StartApplication(GattManager1 gattManager)
        {
            if (gattManager == null)
            {
                System.Console.WriteLine("Couldn't find Gatt manager.");
                return;
            }
           
            var application = new Application(system);
            application.AddService(new Test.TestService(system, 0));

            appObjectPath = application.GetPath();
            var options = new Dictionary<string, object>();
            gattManager.RegisterApplication(appObjectPath, options);
        }

        private void StopAdvertising()
        {
            if (advertisementManager != null)
            {
                advertisementManager.UnregisterAdvertisement(advertisingManagerPath);    
            }
        }

        private ObjectManager GetCopyObjectManager()
        {
            //get a copy of the object manager so we can browse the "tree" of bluetooth items
            ObjectManager manager = null;

            manager = GetObject<ObjectManager>(SERVICE, ObjectPath.Root);

            if (manager != null)
            {
                System.Console.WriteLine("Registering interface events");

                //register these events so we can tell when things are added/removed (eg: discovery)
                manager.InterfacesAdded += (p, i) =>
                {
                    connectedDevicePath = p;

                    DeviceConnected();

                    System.Console.WriteLine(p + " Discovered");
                };
                manager.InterfacesRemoved += (p, i) =>
                {
                    System.Console.WriteLine(p + " Lost");
                    DeviceDisconnected(p);
                };

                System.Console.WriteLine("Done");
            }

            return manager;
        }

        private ObjectPath FindAdapter()
        {
            var manager = GetCopyObjectManager();
            //get the bluetooth object tree
            var managedObjects = manager.GetManagedObjects();
            //find our adapter
            ObjectPath adapterPath = null;
            foreach (var obj in managedObjects.Keys)
            {
                System.Console.WriteLine("Checking " + obj);
                if (managedObjects[obj].ContainsKey(typeof(LEAdvertisingManager1).DBusInterfaceName()))
                {
                    System.Console.WriteLine("Adapter found at" + obj + " that supports LE");
                    adapterPath = obj;
                    break;
                }
            }

            return adapterPath;
        }

        private T GetObject<T>(string busName, ObjectPath path)
        {
            if (system != null)
            {
                var obj = system.GetObject<T>(busName, path);

                if (obj == null)
                {
                    System.Console.WriteLine("Object is NULL");
                }

                return obj;
            }

            System.Console.WriteLine("System is NULL");
            return default(T);
        }

        private void DeviceDisconnected(ObjectPath disconnectedDevicePath)
        {
            // Start Advertising again.
            StartAdvertising();
        }

        private void DeviceConnected()
        {
            StopAdvertising();
        }
    }
}
