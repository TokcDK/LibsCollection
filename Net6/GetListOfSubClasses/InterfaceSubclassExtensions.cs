using System.Reflection;

namespace GetListOfSubClasses
{
    /// <summary>
    /// inherited subclasses/interface implementations extensions
    /// </summary>
    public static class InterfaceSubclassExtensions
    {
        /// <summary>
        /// Enumerate all types, inherited in <paramref name="baseType"/>
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns>Return all class types which is not abstract and not an interface and is subclasses of the <paramref name="baseType"/></returns>
        public static IEnumerable<Type> EnumerateInheritedTypes(this Type baseType)
        {
            foreach (var selectedType in baseType.Assembly.GetTypes())
            {
                if (!selectedType.IsClass) continue;
                if (selectedType.IsInterface) continue;
                if (selectedType.IsAbstract) continue;
                if (!selectedType.IsSubclassOf(baseType)) continue;

                yield return selectedType;
            }
        }

        /// <summary>
        /// Enumerate all inherited classes of an abstract class
        /// Improved non linq a variant of https://stackoverflow.com/a/5411981
        /// </summary>
        /// <typeparam name="T">type of subclass from which to search</typeparam>
        /// <param name="assembly">Specified assembly where to search</param>
        /// <param name="args">parameters required for subclass(set null if not need)</param>
        /// <returns>List of subclasses inherited from abstract class T</returns>
        public static IEnumerable<T> EnumerateInheritedSubClasses<T>(params object[] args)
        {
            foreach (var selectedType in EnumerateInheritedSubClasses<T>(typeof(T).Assembly, args))
            {
                yield return selectedType;
            }
        }

        /// <summary>
        /// Enumerate all inherited classes of an abstract class
        /// Improved non linq a variant of https://stackoverflow.com/a/5411981
        /// </summary>
        /// <typeparam name="T">type of subclass from which to search</typeparam>
        /// <param name="assembly">Specified assembly where to search</param>
        /// <param name="args">parameters required for subclass(set null if not need)</param>
        /// <returns>List of subclasses inherited from abstract class T</returns>
        public static IEnumerable<T> EnumerateInheritedSubClasses<T>(Assembly assembly, object[] args)
        {
            var BaseType = typeof(T);
            bool noargs = args == null || args.Length == 0;
            foreach (var selectedType in assembly.GetTypes())
            {
                if (!selectedType.IsClass) continue;
                if (selectedType.IsInterface) continue;
                if (selectedType.IsAbstract) continue;
                if (!selectedType.IsSubclassOf(BaseType)) continue;

                yield return noargs ? (T)Activator.CreateInstance(selectedType)! 
                    : (T)Activator.CreateInstance(selectedType, args)!;
            }
        }

        /// <summary>
        /// Enumerate all interface implementations from project where from it was called
        /// </summary>
        /// <typeparam name="T">Interface type from which to search</typeparam>
        /// <returns>List of interface implementations</returns>
        public static IEnumerable<T> EnumerateInterfaceImplimentations<T>(Assembly? assembly = null)
        {
            var baseType = typeof(T);
            assembly ??= baseType.Assembly;
            foreach (var selectedType in assembly.GetTypes())
            {
                if (!selectedType.IsClass) continue;
                if (selectedType.IsInterface) continue;
                if (selectedType.IsAbstract) continue;
                if (!baseType.IsAssignableFrom(selectedType)) continue;

                yield return (T)Activator.CreateInstance(selectedType)!;
            }
        }
    }
}
