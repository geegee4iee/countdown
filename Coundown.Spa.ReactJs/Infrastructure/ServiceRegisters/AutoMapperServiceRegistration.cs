using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Infrastructure.ServiceRegisters
{
    public static class AutoMapperServiceRegistration
    {
        // Currently, It just loads classes which inherit from Mapper.Profile base class.
        private static void AddAutoMapperClasses(IServiceCollection services, IEnumerable<Assembly> assembliesToScan)
        {
            assembliesToScan = assembliesToScan as Assembly[] ?? assembliesToScan.ToArray();

            var allTypes = assembliesToScan.SelectMany(a => a.ExportedTypes).ToArray();

            var profiles = allTypes
                .Where(t => typeof(Profile).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
                .Where(t => !t.GetTypeInfo().IsAbstract);


            Mapper.Initialize(cfg =>
            {
                foreach (var pf in profiles)
                {
                    cfg.AddProfile(pf);
                }
            });
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(DependencyContext.Default);
        }

        public static void AddAutoMapper(this IServiceCollection services, DependencyContext dependencyContext)
        {
            AddAutoMapperClasses(services, dependencyContext.RuntimeLibraries.SelectMany(lib => lib.GetDefaultAssemblyNames(dependencyContext).Select(n => Assembly.Load(n))));
        }
    }
}
