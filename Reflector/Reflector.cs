using System.Reflection;
using System.Collections.Generic;
using System;

namespace OFD
{
    /// <summary>
    /// This class converts an object instances' class name and properties into meaningful Oracle parameters and tokens.
    /// </summary>
    public static class Reflector
    {
        public static string GetClassName(ref object instance)
        {
            return instance.GetType().Name.ToLowerInvariant();
        }

        private static Dictionary<string, string> ResolveColumns(ref object instance)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            var propertyInfos = instance.GetType().GetProperties(
                BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Static
                | BindingFlags.Instance
                | BindingFlags.FlattenHierarchy
                );

            foreach (var p in propertyInfos)
            {
                dic.Add(p.Name.ToLowerInvariant(), GetOracleType(p.PropertyType));
            }

            return dic;
        }

        private static string GetOracleType(Type type)
        { 

            return"";
        }
    }
}
