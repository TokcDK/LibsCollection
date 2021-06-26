using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
        /// <typeparam name="T">type of subclass from which to search</typeparam>
        /// <returns>List of subclasses inherited from abstract class T</returns>
        public static List<T> GetListOfinheritedSubClasses<T>()
        {
            return GetListOfinheritedSubClasses<T>(null);
        }

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// non linq a variant of https://stackoverflow.com/a/5411981
        /// </summary>
        /// <typeparam name="T">type of subclass from which to search</typeparam>
        /// <param name="args">parameters required for subclass(set null if not need)</param>
        /// <returns>List of subclasses inherited from abstract class T</returns>
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
        /// Get all interface implementations from project where from it was called
        /// </summary>
        /// <typeparam name="T">Interface type from which to search</typeparam>
        /// <returns>List of interface implementations</returns>
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

        /// <summary>
        /// Get list of interface implementations from some folder files
        /// </summary>
        /// <typeparam name="T">Interface type from which to search</typeparam>
        /// <param name="pluginsDirPath">Folder with files</param>
        /// <param name="extension">Extension of files. Defauld is .dll</param>
        /// <returns>List of interface implementations</returns>
        public static List<T> GetListOfInterfaceImplimentations<T>(string pluginsDirPath, string extension = ".dll")
        {
            var ListOfSubClasses = new List<T>();
            var type = typeof(T);

            foreach (var dll in Directory.GetFiles(pluginsDirPath, "*"+ extension, SearchOption.AllDirectories))
            {
                foreach (var ClassType in Assembly.LoadFrom(dll).GetTypes())
                {
                    if (ClassType.IsClass && !ClassType.IsInterface && !ClassType.IsAbstract && type.IsAssignableFrom(ClassType))
                    {
                        ListOfSubClasses.Add((T)Activator.CreateInstance(ClassType));
                    }
                }
            }

            return ListOfSubClasses;
        }
    }
}
