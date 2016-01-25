using System.Reflection;
using System.Collections.Generic;
using System;
using System.IO;
using OFD.Properties;

namespace OFD.Data
{
    /// <summary>
    /// This class converts an Model instances' class name and properties into meaningful Oracle parameters and tokens.
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

        /// <summary>
        /// Returns the lowercase invariant name of the supplied instance type.
        /// </summary>
        /// <param name="instance">An Model with some scalar type properties.</param>
        public static string GetClassName(ref Model instance)
        {
            return instance.GetType().Name.ToLowerInvariant();
        }

        /// <summary>
        /// Returns a dictionary of column names, mapped to corresponding PL-SQL data types, for building DDL statements. 
        /// </summary>
        /// <param name="instance">An Model with some scalar type properties.</param>
        public static Dictionary<string, string> ResolveColumns(ref Model instance)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (var p in GetWritableProperties(ref instance))
            {
                dic.Add(p.Name.ToLowerInvariant(), TypeMap[p.PropertyType]);
            }

            return dic;
        }

        /// <summary>
        /// Returns a dictionary of column names, mapped to corresponding values, for building CRUD/DML statements. 
        /// </summary>
        /// <param name="instance">An Model with some scalar type properties.</param>
        public static Dictionary<string, string> ResolvePersistenceMappings(ref Model instance)
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

        /// <summary>
        /// Returns an embedded text file resource located in the OFD.Data.Scripts namespace.
        /// </summary>
        /// <param name="name">The name of an embedded text file resource.</param>
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

        /// <summary>
        /// Returns a list of properties of the supplied Model that are simple, scalar, types and in the Reflector.TypeMap.
        /// </summary>
        /// <param name="instance">An Model with some scalar type properties.</param>
        public static List<PropertyInfo> GetWritableProperties(ref Model instance)
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

        /// <summary>
        /// Returns the Model's ID
        /// </summary>
        /// <param name="instance">An Model with some scalar type properties.</param>
        public static int GetID(ref Model instance)
        {
            return (int)instance.GetType().GetProperty("ID").GetValue(instance, null);
        }

        /// <summary>
        /// Sets a property of the specified Model.
        /// </summary>
        /// <param name="instance">An Model with some scalar type properties.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value to set.</param>
        public static void SetProperty(ref Model instance, string name, object value)
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
