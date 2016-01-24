using System.Reflection;

namespace OFD
{
    public static class Reflector
    {
        public static string GetClassName(object instance)
        {
            return instance.GetType().Name;
        }
    }
}
