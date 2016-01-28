using System.Reflection;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization;
using OFD.Properties;

namespace OFD.Reflection
{
    /// <summary>
    /// This class converts A Model instances' class name and properties into meaningful Oracle parameters and tokens.
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
        /// <param name="instance">A Model with some scalar type properties.</param>
        public static string GetTableName(ref Model instance)
        {
            return Hasher.Hash(instance.GetType().Name);
        }

        /// <summary>
        /// Returns a dictionary of column names, mapped to corresponding PL-SQL data types, for building DDL statements. 
        /// </summary>
        /// <param name="instance">A Model with some scalar type properties.</param>
        public static Dictionary<string, string> GetColumnMap(ref Model instance)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> index in instance.Cache.IdentityCache)
            {
                Type type = GetPropertyType(ref instance, index.Value);

                dic.Add(index.Key, TypeMap[type]);
            }

            return dic;
        }

        /// <summary>
        /// Returns a dictionary of column names, mapped to corresponding values, for building CRUD/DML statements. 
        /// </summary>
        /// <param name="instance">A Model with some scalar type properties.</param>
        public static Dictionary<string, string> GetPersistenceMap(ref Model instance)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> index in instance.Cache.IdentityCache)
            {
                object p = GetProperty(ref instance, index.Value);

                if (p != null && !string.IsNullOrWhiteSpace(p.ToString()))
                {
                    if (p.GetType().Equals(typeof(String)))
                    {
                        dic.Add(index.Key, "'" + p.ToString() + "'");
                    }
                    else
                    {
                        dic.Add(index.Key, p.ToString());
                    }
                }
            }

            return dic;
        }

        /// <summary>
        /// Returns a dictionary that maps property names to their hashed PL-SQL identity counterparts.
        /// </summary>
        /// <param name="instance">A Model with some scalar type properties.</param>
        public static Dictionary<string, string> GetIdentityMap(Type type)
        {
            Model instance = (Model)FormatterServices.GetUninitializedObject(type);

            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (var p in GetWritableProperties(ref instance))
            {
                dic.Add(Hasher.Hash(p.Name), p.Name);
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
        /// <param name="instance">A Model with some scalar type properties.</param>
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
        /// Returns a property of the supplied instance.
        /// </summary>
        /// <param name="instance">A Model with some scalar type properties.</param>
        public static object GetProperty(ref Model instance, string name)
        {
            return instance.GetType().GetProperty(name).GetValue(instance, null);
        }

        /// <summary>
        /// Gets the type of a property.
        /// </summary>
        /// <param name="instance">A Model the owns the property.</param>
        /// <param name="name">The property name.</param>
        /// <returns></returns>
        public static Type GetPropertyType(ref Model instance, string name)
        {
            return instance.GetType().GetProperty(name).PropertyType;
        }

        /// <summary>
        /// Sets a property of the specified Model.
        /// </summary>
        /// <param name="instance">A Model with some scalar type properties.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value to set.</param>
        public static void SetProperty(ref Model instance, string name, object value)
        {
            try
            {
                if (value.GetType().Equals(typeof(DBNull)))
                {
                    return;
                }

                var property = instance.GetType().GetProperty(name);

                if (value.GetType().Equals(typeof(long)) && (long)value < int.MaxValue)
                {
                    property.SetValue(instance, unchecked((int)(long)value), null);
                }
                else
                {
                    property.SetValue(instance, value, null);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.NoSetProperty, name, instance.GetType().ToString()), ex);
            }
        }

        /// <summary>
        /// Returns an uninitialized object for reflection purposes. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetUninitializedObject<T>() where T : Model, new()
        {
            T child = (T)FormatterServices.GetUninitializedObject(typeof(T));

            child.Cache = new Caching.Cache();
            child.Cache.IdentityCache = GetIdentityMap(typeof(T));

            return child;
        }
    }
}
