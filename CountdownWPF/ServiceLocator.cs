using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CountdownWPF
{

    public class ServiceLocator
    {
        enum RegisterType
        {
            SingleTon
        }

        static IDictionary<string, object> _singleTonInstances = new Dictionary<string, object>();
        static IDictionary<string, RegisterType> _typeConfigs = new Dictionary<string, RegisterType>();

        private static ServiceLocator _serviceLocator = new ServiceLocator();

        public static T GetInstance<T>() where T : class
        {
            return GetInstance<T>(null);
        }

        public static T GetInstance<T>(string name) where T: class
        {
            var typeOfT = typeof(T);
            var typeFullName = typeOfT.FullName;

            if (!string.IsNullOrEmpty(name))
            {
                typeFullName += "_" + name;
            }


            if (!_typeConfigs.ContainsKey(typeFullName)) return default(T);

            var registerType = _typeConfigs[typeFullName];

            switch (registerType)
            {
                case RegisterType.SingleTon:
                    if (_singleTonInstances.ContainsKey(typeFullName))
                    {
                        return (T)_singleTonInstances[typeFullName];
                    }
                    break;
            }

            return default(T);
        }

        public static ServiceLocatorSetup Setup
        {
            get
            {
                return new ServiceLocatorSetup();
            }
        }

        public class ServiceLocatorSetup
        {
            private Type _type;

            // Default register type would be SingleTon
            private object _instance;
            static private object obj = new object();

            public ServiceLocatorSetup RegisterAssemblyForType<T>(string assemblyName) where T : class
            {
                lock (obj)
                {
                    try
                    {
                        var typeOfT = typeof(T);
                        var assembly = Assembly.Load(assemblyName);

                        var t = assembly.GetExportedTypes().Where(c => c.GetInterfaces().Any(i => i == typeOfT)).FirstOrDefault();
                        if (t != null)
                        {
                            var instance = Activator.CreateInstance(t);
                            _instance = instance;
                            _type = typeOfT;
                        }

                        return this;
                    }
                    catch (Exception ex)
                    {
                        // Currently let errors fail silently.
                        Debug.WriteLine(ex.Message);
                        return this;
                    }
                }
            }

            public ServiceLocatorSetup RegisterAssemblyForType<T, D>() where T: class where D: T
            {
                lock (obj)
                {
                    try
                    {
                        var typeOfT = typeof(T);
                        var typeOfD = typeof(D);

                        if (typeOfT.IsAssignableFrom(typeOfD))
                        {
                            var instance = Activator.CreateInstance(typeOfD);
                            _instance = instance;
                            _type = typeOfT;
                        }

                        return this;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return this;
                    }
                }
            }

            public void AsSingleTon(string name)
            {
                if (_instance != null && _type != null)
                {
                    var fullname = name == null ? this._type.FullName : this._type.FullName + "_" + name;
                    _typeConfigs.Add(fullname, RegisterType.SingleTon);
                    _singleTonInstances.Add(fullname, _instance);
                }
            }

            public void AsSingleTon()
            {
                AsSingleTon(null);
            }
        }

    }
}
