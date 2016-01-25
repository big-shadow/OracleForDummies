using System.Reflection;
using System.Collections.Generic;
using System;
using System.IO;
using OFD.Properties;

namespace OFD.Data
{
    /// <summary>
    /// This class converts an object instances' class name and properties into meaningful Oracle parameters and tokens.
    /// </summary>
    public static class Reflector
    {
        private static Dictionary<Type, string> TypeMap
        {
            get
            {
                Dictionary<Type, string> map = new Dictionary<Type, string>();
                map.Add(typeof(bool), "NUMBER(1)");
                map.Add(typeof(string), "VARCHAR2(4000)");
                map.Add(typeof(short), "NUMBER(5)");
                map.Add(typeof(int), "NUMBER(10)");
                map.Add(typeof(long), "NUMBER(19)");
                map.Add(typeof(DateTime), "DATE");

                return map;
            }
        }

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
                dic.Add(p.Name.ToLowerInvariant(), TypeMap[p.PropertyType]);
            }

            return dic;
        }

        public static Dictionary<string, string> ResolvePersistenceMappings(ref object instance)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (var p in GetWritableProperties(ref instance))
            {
                object val = p.GetValue(instance, null);

                if (p.PropertyType.Equals(typeof(String)))
                {
                    dic.Add(p.Name.ToLowerInvariant(), "'" + val.ToString() + "'");
                }
                else
                {
                    dic.Add(p.Name.ToLowerInvariant(), val.ToString());
                }
            }

            return dic;
        }

        public static string GetEmbeddedResource(string name)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                StreamReader reader = new StreamReader(assembly.GetManifestResourceStream("OFD.Data.Scripts." + name + ".txt"));

                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.NoResource, name), ex);
            }
        }

        public static List<PropertyInfo> GetWritableProperties(ref object instance)
        {
            List<PropertyInfo> writable = new List<PropertyInfo>();

            var propertyInfos = instance.GetType().GetProperties(
                BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Static
                | BindingFlags.Instance
                | BindingFlags.FlattenHierarchy
                );

            foreach (var p in propertyInfos)
            {
                if (TypeMap.ContainsKey(p.PropertyType))
                {
                    writable.Add(p);
                }
            }

            return writable;
        }

        public static int GetID(ref object instance)
        {
            return (int)instance.GetType().GetProperty("ID").GetValue(instance, null);
        }

        public static void SetProperty(ref object instance, string name, object value)
        {
            var property = instance.GetType().GetProperty(name);

            if (value.GetType().Equals(typeof(long)))
            {
                property.SetValue(instance, unchecked((int)(long)value), null);
            }
            else
            {
                property.SetValue(instance, value, null);
            }
        }
    }
}
