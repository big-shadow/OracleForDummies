using System.Reflection;
using System.Collections.Generic;
using System;
using System.IO;

namespace OFD.Data
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

        public static Dictionary<string, string> ResolveColumns(ref object instance)
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

        public static string GetEmbeddedResource(string name)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                StreamReader reader = new StreamReader(assembly.GetManifestResourceStream("OFD.Data." + name));

                return reader.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }
    }
}
