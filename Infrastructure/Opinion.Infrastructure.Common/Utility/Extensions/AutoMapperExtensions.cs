namespace Opinion.Infrastructure.Common.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    // TODO: Implementar la estrategia Queryable Extensions de https://github.com/AutoMapper/AutoMapper/wiki/Queryable-Extensions\
    //       para poder generalizar las propiedades de los catalogos automaticamente cuando se genera el DTO.

    public static class AutoMapperExtensions
    {
        public static void CreateMapsForSourceTypes(this IConfiguration configuration, Func<Type, bool> filter, Func<Type, Type> destinationType, Action<IMappingExpression, Type, Type> mappingConfiguration)
        {
            var typesInThisAssembly = typeof(AutoMapperExtensions).Assembly.GetExportedTypes();
            CreateMapsForSourceTypes(configuration, typesInThisAssembly.Where(filter), destinationType, mappingConfiguration);
        }

        public static void CreateMapsForSourceTypes(this IConfiguration configuration, IEnumerable<Type> typeSource, Func<Type, Type> destinationType, Action<IMappingExpression, Type, Type> mappingConfiguration)
        {
            foreach (var type in typeSource)
            {
                
                var destType = destinationType(type);                 
                if (destType == null) continue;
                var mappingExpression = configuration.CreateMap(type, destType);
                mappingConfiguration(mappingExpression, type, destType);
            }
        }

        public static IMappingExpression MapDisplayValues(this IMappingExpression expression, Type sourceType, Type destinationType)
        {
            //List<string> prefixes = new List<string> { "GENCAT_", "PERCAT_", "INSCAT_" };
            const string DISPLAY_SUFFIX = "Display";
            const string DISPLAY_PROPERTY= ".Descripcion";

            var entityProperties = destinationType.GetProperties().Where(p => p.PropertyType == typeof(string) && p.Name.EndsWith(DISPLAY_SUFFIX));
            foreach (var prop in entityProperties)
            {
                var destPropertyName = prop.Name;
                var sourcePropertyName = prop.Name.Replace(DISPLAY_SUFFIX, string.Empty).ToUpper() + DISPLAY_PROPERTY;
                //var destPropertyName = prefixes.GENCAT + prop.Name.ToUpper() ;
                expression.ForMember(destPropertyName, map => map.MapFrom(sourcePropertyName));
            }
            return expression;
        }
    }
}
