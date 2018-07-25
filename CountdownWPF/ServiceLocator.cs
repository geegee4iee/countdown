using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CountdownWPF
{
    public class Dependency
    {
        public object Instance { get; set; }
        public RegisteredTypeConfig TypeConfig { get; set; }
        public Tuple<Type, Type> RegisteredType { get; set; }
    }

    public enum RegisteredTypeConfig
    {
        SingleTon
    }

    public class ServiceLocator
    {

        static public IDictionary<string, Dependency> _dependencies = new Dictionary<string, Dependency>();

        private static ServiceLocator _serviceLocator = new ServiceLocator();

        public static T GetInstance<T>() where T : class
        {
            return GetInstance<T>(null);
        }

        public static T GetInstance<T>(string name) where T : class
        {
            var typeOfT = typeof(T);
            var typeFullName = typeOfT.FullName;

            if (!string.IsNullOrEmpty(name))
            {
                typeFullName += "_" + name;
            }

            if (!_dependencies.TryGetValue(typeFullName, out Dependency dependency)) return default(T);
            if (dependency.Instance != null) return (T)dependency.Instance;

            return (T)Instantiate(typeFullName);
        }

        private static object Instantiate(string fullName)
        {
            var dependency = _dependencies[fullName];

            if (dependency.Instance != null) return dependency.Instance;

            var derivedType = dependency.RegisteredType.Item2;

            var constructors = derivedType.GetConstructors();
            ConstructorInfo selectedConstructor = null;

            var emptyParamConstructor = constructors.FirstOrDefault(c => c.GetParameters().Length == 0);

            if (emptyParamConstructor != null)
            {
                var instance = Activator.CreateInstance(derivedType);
                _dependencies[fullName].Instance = instance;
                return instance;
            } else
            {
                foreach(var c in constructors)
                {
                    var @params = c.GetParameters();
                    int validParams = 0;
                    foreach(var p in @params)
                    {
                        var paramType = p.ParameterType;
                        foreach(var d in _dependencies)
                        {
                            if (d.Key != fullName && d.Key == paramType.FullName)
                            {
                                validParams++;
                            }
                        }
                    }

                    if (validParams == @params.Length)
                    {
                        selectedConstructor = c;
                        break;
                    }
                }

                var selectedParams = selectedConstructor.GetParameters();
                object[] arguments = new object[selectedParams.Length];
                for (int i = 0; i < selectedParams.Length; i++)
                {
                    arguments[i] = Instantiate(selectedParams[i].ParameterType.FullName);
                }

                var instance = Activator.CreateInstance(derivedType, arguments);
                _dependencies[fullName].Instance = instance;
                return instance;
            }
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

            static private object obj = new object();
            private Tuple<Type, Type> _registeredType;

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
                            RegisterAssembly(typeOfT, t);
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

            private void RegisterAssembly(Type @base, Type derived)
            {
                if (@base.IsAssignableFrom(derived))
                {
                    _registeredType = new Tuple<Type, Type>(@base, derived);
                }
            }

            public ServiceLocatorSetup RegisterAssemblyForType<T, D>() where T : class where D : T
            {
                lock (obj)
                {
                    try
                    {
                        var typeOfT = typeof(T);
                        var typeOfD = typeof(D);

                        RegisterAssembly(typeOfT, typeOfD);

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
                if (_registeredType != null)
                {
                    var fullname = name == null ? _registeredType.Item1.FullName : _registeredType.Item1.FullName + "_" + name;

                    _dependencies.Add(fullname, new Dependency
                    {
                        RegisteredType = _registeredType,
                        TypeConfig = RegisteredTypeConfig.SingleTon
                    });
                }
            }

            public void AsSingleTon()
            {
                AsSingleTon(null);
            }
        }

    }
}
