using System.Reflection;

namespace TGFramework
{
    /// <summary>
    /// Simple attribute-based dependency injector that assigns provided dependencies
    /// to fields and properties marked with <see cref="InjectAttribute"/> on a target object.
    /// </summary>
    public static class DependencyInjector
    {
        /// <summary>
        /// Injects the given dependencies into all non-public instance fields and properties
        /// of the target object that are marked with <see cref="InjectAttribute"/>.
        /// </summary>
        /// <param name="target">The object whose members will receive the dependencies.</param>
        /// <param name="dependencies">The dependency instances to inject.</param>
        public static void InjectDependencies(object target, params object[] dependencies)
        {
            var targetType = target.GetType();
            var injectFields = targetType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            var injectProperties = targetType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var dependency in dependencies)
            {
                foreach (var field in injectFields)
                {
                    if (field.GetCustomAttribute<InjectAttribute>() != null && field.FieldType.IsInstanceOfType(dependency))
                    {
                        field.SetValue(target, dependency);
                    }
                }

                foreach (var property in injectProperties)
                {
                    if (property.GetCustomAttribute<InjectAttribute>() != null && property.PropertyType.IsInstanceOfType(dependency))
                    {
                        property.SetValue(target, dependency);
                    }
                }
            }
        }
    }
}