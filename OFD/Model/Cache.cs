using System;
using System.Collections.Generic;
using OFD.Reflection;

namespace OFD.Caching
{
    public static class Cache
    {
        private static Dictionary<Type, ModelCache> MasterCache { get; set; }

        /// <summary>
        /// Gets the cache associated with a type derived from the abstract class Model.
        /// If the cache hasn't been accessed yet, this creates one for the type.
        /// </summary>
        public static ModelCache Get(Model instance)
        {
            Type type = instance.GetType();

            SetMasterCache();

            if (!MasterCache.ContainsKey(type))
            {
                MasterCache.Add(type, new ModelCache(type));
            }

            return MasterCache[type];
        }

        /// <summary>
        /// Sets the MasterCache if it's null.
        /// </summary>
        private static void SetMasterCache()
        {
            if (MasterCache == null)
            {
                MasterCache = new Dictionary<Type, ModelCache>();
            }
        }
    }

    /// <summary>
    /// This class contains caches to be coupled with types derived from the abstract class Model.
    /// </summary>
    public class ModelCache
    {
        /// <summary>
        /// This dictionary maps concrete class properties to it's PL-SQL column name counterparts.
        /// </summary>
        public Dictionary<string, string> IdentityCache { get; }

        public ModelCache(Type type)
        {
            IdentityCache = Reflector.GetIdentityDictionary(type);
        }
    }
}
