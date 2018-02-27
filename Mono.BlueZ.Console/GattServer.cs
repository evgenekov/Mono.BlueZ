using System;
using System.Collections.Generic;

using DBus;
using Mono.BlueZ.DBus;
using org.freedesktop.DBus;

namespace Mono.BlueZ.Console
{
    public class GattServer
    {
        private const string SERVICE = "org.bluez";
        private ObjectPath applicationPath;
        private ObjectPath advertisementPath;
        private ObjectPath adapterFoundPath;
        private LEAdvertisingManager1 advertisementManager;
        private LEAdvertisement1 advertisement;
        private DBusConnection busConnection;
        private string deviceConnected;

        public GattServer()
        {
            busConnection = new DBusConnection();
        }

        public void Run()
        {
            try
            {
                busConnection.Startup();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                return;
            }

            GattManager1 gattManager = null;

            try
            {
                System.Console.WriteLine("Fetching objects");
                // Find adapter
                adapterFoundPath = FindAdapter();

                if (adapterFoundPath == null)
                {
                    System.Console.WriteLine("Couldn't find adapter that supports LE");
                    return;
                }

                gattManager = GetObject<GattManager1>(SERVICE, adapterFoundPath);
                advertisementManager = GetObject<LEAdvertisingManager1>(SERVICE, adapterFoundPath);
                                  
                // Start advertising
                StartAdvertising();

                // Start application
                StartApplication(gattManager);

                busConnection.Wait();
            }
            catch (Exception exception)
            {
                System.Console.WriteLine(exception);
            }
            finally
            {
                if (gattManager != null)
                {
                    gattManager.UnregisterApplication(applicationPath);    
                }

                StopAdvertising();
            }
        }

        private void StartAdvertising()
        {
            var adapter_properties = GetObject<Adapter1>(SERVICE, adapterFoundPath);
            adapter_properties.Powered = true;

            advertisement = new Advertisement(busConnection.System, 0, "peripheral");
            advertisementPath = (advertisement as Advertisement).GetPath();
            var options = new Dictionary<string, object>();

            advertisementManager.RegisterAdvertisement(advertisementPath, options);
        }

        private void StartApplication(GattManager1 gattManager)
        {
            if (gattManager == null)
            {
                System.Console.WriteLine("Couldn't find Gatt manager.");
                return;
            }
           
            var application = new Application(busConnection.System);
            application.AddService(new Test.TestService(busConnection.System, 0));

            applicationPath = application.GetPath();
            var options = new Dictionary<string, object>();
            gattManager.RegisterApplication(applicationPath, options);
        }

        private void StopAdvertising()
        {
            if (advertisement != null)
            {
                advertisement.Release();
            }

            if (advertisementPath != null)
            {
                advertisementManager.UnregisterAdvertisement(advertisementPath);
                advertisementPath = null;
                advertisement = null;
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
                    if (string.IsNullOrEmpty(deviceConnected))
                    {
                        if (p.ToString().Contains(adapterFoundPath.ToString()))
                        {
                            deviceConnected = p.ToString();

                            StopAdvertising();
                            System.Console.WriteLine("Device connected: " + deviceConnected);
                        }
                    }

                    System.Console.WriteLine(p + " Discovered");
                };
                manager.InterfacesRemoved += (p, i) =>
                {
                    if (!string.IsNullOrEmpty(deviceConnected))
                    {
                        if (deviceConnected.Equals(p.ToString()))
                        {
                            deviceConnected = string.Empty;
                            StopAdvertising();
                            StartAdvertising();
                            System.Console.WriteLine("Device disconnected");
                        }
                    }

                    System.Console.WriteLine(p + " Lost");
                };

                System.Console.WriteLine("Done");
            }

            return manager;
        }

        private ObjectPath FindAdapter()
        {
            var manager = GetCopyObjectManager();
            //get the bluetooth object tree
            System.Console.WriteLine("GetManagedObjects");
            var managedObjects = manager.GetManagedObjects();
            //find our adapter
            ObjectPath adapterPath = null;
            foreach (var obj in managedObjects.Keys)
            {
                System.Console.WriteLine("Checking " + obj);
                if (managedObjects[obj].ContainsKey(typeof(LEAdvertisingManager1).DBusInterfaceName()))
                {
                    System.Console.WriteLine("Adapter found at " + obj + " that supports LE");
                    adapterPath = obj;
                    break;
                }
            }

            return adapterPath;
        }

        private T GetObject<T>(string busName, ObjectPath path)
        {
            if (busConnection.System != null)
            {
                var obj = busConnection.System.GetObject<T>(busName, path);

                if (obj == null)
                {
                    System.Console.WriteLine("Object is NULL");
                }

                return obj;
            }

            System.Console.WriteLine("System is NULL");
            return default(T);
        }
    }
}
