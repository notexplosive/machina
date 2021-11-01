namespace Machina.ThirdParty
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Adapted from stackoverflow
    /// https://stackoverflow.com/questions/5411694/get-all-inherited-classes-of-an-abstract-class/6944605
    /// </summary>
    public static class ReflectiveEnumerator
    {
        static ReflectiveEnumerator() { }

        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
        {
            List<T> result = new List<T>();
            foreach (Type type in 
                Assembly.GetAssembly(typeof(T)).GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                result.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            return result;
        }
    }
}