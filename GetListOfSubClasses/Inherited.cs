using System;
using System.Collections.Generic;

namespace GetListOfSubClasses
{
    /// <summary>
    /// inherited subclasses functions
    /// </summary>
    public class Inherited
    {
        /// <summary>
        /// Get all inherited classes of an abstract class
        /// non linq a variant of https://stackoverflow.com/a/5411981
        /// </summary>
        /// <typeparam name="T">type of subclasses</typeparam>
        /// <param name="parameter">parameter required for subclass(remove if not need)</param>
        /// <returns>List of subclasses of abstract class</returns>
        public static List<T> GetListOfinheritedSubClasses<T>()
        {
            return GetListOfinheritedSubClasses<T>(null);
        }

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// non linq a variant of https://stackoverflow.com/a/5411981
        /// </summary>
        /// <typeparam name="T">type of subclasses</typeparam>
        /// <param name="parameter">parameter required for subclass(remove if not need)</param>
        /// <returns>List of subclasses of abstract class</returns>
        public static List<T> GetListOfinheritedSubClasses<T>(params object[] args)
        {
            var ListOfSubClasses = new List<T>();
            var type = typeof(T);
            bool noargs = args == null || args.Length == 0;
            foreach (var ClassType in type.Assembly.GetTypes())
            {
                if (
                    !ClassType.IsClass
                    || ClassType.IsInterface
                    || ClassType.IsAbstract
                    || !ClassType.IsSubclassOf(type)
                    )
                {
                    continue;
                }

                T Instance;

                if (noargs)
                {
                    Instance = (T)Activator.CreateInstance(ClassType);
                }
                else
                {
                    Instance = (T)Activator.CreateInstance(ClassType, args);
                }

                ListOfSubClasses.Add(Instance);
            }

            return ListOfSubClasses;
        }

        /// <summary>
        /// Get all interface implementstions
        /// non linq version of https://stackoverflow.com/a/5411981
        /// </summary>
        /// <typeparam name="T">type of subclasses</typeparam>
        /// <param name="parameter">parameter required for subclass(remove if not need)</param>
        /// <returns>List of subclasses of abstract class</returns>
        public static List<T> GetListOfInterfaceImplimentations<T>()
        {
            var ListOfSubClasses = new List<T>();
            var type = typeof(T);
            foreach (var ClassType in type.Assembly.GetTypes())
            {
                if (ClassType.IsClass && !ClassType.IsInterface && !ClassType.IsAbstract && type.IsAssignableFrom(ClassType))
                {
                    ListOfSubClasses.Add((T)Activator.CreateInstance(ClassType));
                }
            }

            return ListOfSubClasses;
        }
    }
}
