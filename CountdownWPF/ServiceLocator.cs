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

        static IDictionary<Type, object> _singleTonInstances = new Dictionary<Type, object>();
        static IDictionary<Type, RegisterType> _typeConfigs = new Dictionary<Type, RegisterType>();

        private static ServiceLocator _serviceLocator = new ServiceLocator();

        public static T GetInstance<T>() where T : class
        {
            var typeOfT = typeof(T);

            if (!_typeConfigs.ContainsKey(typeOfT)) return default(T);

            var registerType = _typeConfigs[typeOfT];

            switch (registerType)
            {
                case RegisterType.SingleTon:
                    if (_singleTonInstances.ContainsKey(typeOfT))
                    {
                        return (T)_singleTonInstances[typeOfT];
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
            private RegisterType _registerType = RegisterType.SingleTon;
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

            public void AsSingleTon()
            {
                if (_instance != null && _type != null)
                {
                    _typeConfigs.Add(this._type, RegisterType.SingleTon);
                    _singleTonInstances.Add(_type, _instance);
                }
            }
        }

    }
}
