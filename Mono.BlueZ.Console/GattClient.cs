﻿using System;
using System.Collections.Generic;
using System.Threading;

using DBus;
using Mono.BlueZ.DBus;
using org.freedesktop.DBus;

namespace Mono.BlueZ.Console
{
    public class GattClient
    {
        private Bus system;
        private ManualResetEvent _started = new ManualResetEvent(false);
        public Exception _startupException { get; private set; }
        private const string SERVICE = "org.bluez";

        public void Run()
        {
            StartMessageLoopDBus();

            try
            {
                System.Console.WriteLine("Fetching objects");

                // 2. Find adapter
                var objectManager = GetCopyObjectManager();

                var managedObjects = objectManager.GetManagedObjects();

                if (objectManager == null)
                {
                    System.Console.WriteLine("Couldn't find Gatt manager.");
                    return;
                }

                while (true)
                {
                    // Gatt client is running. Do nothing here.
                }
            }
            catch (Exception exception)
            {
                System.Console.WriteLine(exception);
            }
            finally
            {
                //gattManager.UnregisterApplication(appObjectPath);
            }
        }

        private void StartMessageLoopDBus()
        {
            // Run a message loop for DBus on a new thread.
            var t = new Thread(DBusLoop)
            {
                IsBackground = true
            };
            t.Start();
            _started.WaitOne(15 * 1000);
            _started.Close();
            if (_startupException != null)
            {
                throw _startupException;
            }
            else
            {
                System.Console.WriteLine("Bus connected at " + system.UniqueName);
            }
        }

        private void DBusLoop()
        {
            try
            {
                system = Bus.System;
            }
            catch (Exception ex)
            {
                _startupException = ex;
                return;
            }
            finally
            {
                _started.Set();
            }

            while (true)
            {
                system.Iterate();
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
                    System.Console.WriteLine(p + " Discovered");
                };
                manager.InterfacesRemoved += (p, i) =>
                {
                    System.Console.WriteLine(p + " Lost");
                };

                System.Console.WriteLine("Done");
            }

            return manager;
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
    }
}
