using System;
using System.Collections.Generic;
using OFD.Reflect;

namespace OFD.Caching
{
    public static class Cache
    {
        private static Dictionary<Type, ModelCache> MasterCache { get; set; }
        private static Dictionary<string, string> EmbeddedResources { get; set; }

        /// <summary>
        /// Gets the cache associated with a type derived from the abstract class Model.
        /// If the cache hasn't been accessed yet, this creates one for the type.
        /// </summary>
        public static ModelCache Get(Model instance)
        {
            Type type = instance.GetType();

            return Get(type);
        }

        /// <summary>
        /// Gets the cache associated with a type derived from the abstract class Model.
        /// If the cache hasn't been accessed yet, this creates one for the type.
        /// </summary>
        public static ModelCache Get(Type type)
        {
            if (MasterCache == null)
            {
                MasterCache = new Dictionary<Type, ModelCache>();
            }

            if (!MasterCache.ContainsKey(type))
            {
                MasterCache.Add(type, new ModelCache(type));
            }

            return MasterCache[type];
        }

        public static string GetResource(string key)
        {
            if (EmbeddedResources == null)
            {
                EmbeddedResources = new Dictionary<string, string>();
            }

            if (!EmbeddedResources.ContainsKey(key))
            {
                EmbeddedResources.Add(key, Reflector.GetEmbeddedResource(key));
            }

            return EmbeddedResources[key];
        }
    }

    /// <summary>
    /// This class contains caches to be coupled with types derived from the abstract class Model.
    /// </summary>
    public class ModelCache
    {
        public String TableName { get; }
        public Dictionary<string, string> IdentityCache { get; }
        public Dictionary<string, string> ColumnDictionary { get; }
        public List<System.Reflection.PropertyInfo> WritableProperties { get; }

        public ModelCache(Type type)
        {
            TableName = Reflector.GetTableName(type);
            IdentityCache = Reflector.GetIdentityDictionary(type);
            ColumnDictionary = Reflector.GetColumnDictionary(type);
            WritableProperties = Reflector.GetWritableProperties(type);
        }
    }
}
