using System.Reflection;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization;
using OFD.Properties;
using OFD.Caching;

namespace OFD.Reflect
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
        public static string GetTableName(Type type)
        {
            return Hasher.Hash(type.Name);
        }

        /// <summary>
        /// Returns a dictionary of column names, mapped to corresponding PL-SQL data types, for building DDL statements. 
        /// </summary>
        /// <param name="instance">A Model with some scalar type properties.</param>
        public static Dictionary<string, string> GetColumnDictionary(Type type)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> index in GetIdentityDictionary(type))
            {
                Type pType = GetPropertyType(type, index.Value);

                dic.Add(index.Key, TypeMap[pType]);
            }

            return dic;
        }

        /// <summary>
        /// Returns a dictionary of column names, mapped to corresponding values, for building CRUD/DML statements. 
        /// </summary>
        /// <param name="instance">A Model with some scalar type properties.</param>
        public static Dictionary<string, string> GetPersistenceDictionary(ref Model instance)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> index in Cache.Get(instance).IdentityCache)
            {
                object p = GetPropertyInstance(ref instance, index.Value);

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
        public static Dictionary<string, string> GetIdentityDictionary(Type type)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (var p in GetWritableProperties(type))
            {
                dic.Add(Hasher.Hash(p.Name), p.Name);
            }

            return dic;
        }

        /// <summary>
        /// Returns an embedded text file resource located in the OFD.SQLizer.Scripts namespace.
        /// </summary>
        /// <param name="name">The name of an embedded text file resource.</param>
        public static string GetEmbeddedResource(string name)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                // OFD.SQLizer.Scripts.DropTable.sql
                StreamReader reader = new StreamReader(assembly.GetManifestResourceStream("OFD.SQLizer.Scripts." + name + ".sql"));

                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.NoResource, name), ex);
            }
        }

        /// <summary>
        /// Returns a list of properties of the supplied Model that are simple, scalar types, and in the Reflector.TypeMap.
        /// </summary>
        /// <param name="instance">A Model with some scalar type properties.</param>
        public static List<PropertyInfo> GetWritableProperties(Type type)
        {
            List<PropertyInfo> writable = new List<PropertyInfo>();

            var propertyInfos = type.GetProperties(
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
        public static object GetPropertyInstance(ref Model instance, string name)
        {
            return instance.GetType().GetProperty(name).GetValue(instance, null);
        }

        /// <summary>
        /// Gets the type of a property.
        /// </summary>
        /// <param name="instance">A Model the owns the property.</param>
        /// <param name="name">The property name.</param>
        /// <returns></returns>
        public static Type GetPropertyType(Type type, string name)
        {
            return type.GetProperty(name).PropertyType;
        }

        /// <summary>
        /// Sets a property of the specified Model.
        /// </summary>
        /// <param name="instance">A Model with some scalar type properties.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value to set.</param>
        public static void SetPropertyValue(ref Model instance, string name, object value)
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

            return child;
        }
    }
}
