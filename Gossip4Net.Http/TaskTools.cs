using System.Reflection;

namespace Gossip4Net.Http
{
    internal static class TaskTools
    {
        private static readonly MethodInfo genericConverType = typeof(TaskTools).GetMethods().Where(m => m.Name.Equals(nameof(TaskTools.ConvertType))).Single();

        public async static Task<TTarget> ConvertType<TOriginal, TTarget>(this Task<TOriginal> task) where TTarget : TOriginal
        {
            var result = await task;
            return (TTarget) result;
        }

        public static Task WithType<T>(this Task<T> task, Type expectedType)
        {
            var specific = genericConverType.MakeGenericMethod(typeof(T), expectedType);
            return (Task)specific.Invoke(null, new object[] { task });
        }

        public static bool IsTask(this Type t)
        {
            return t.IsAssignableTo(typeof(Task));
        }

        public static bool IsVoidTask(this Type t)
        {
            return t.IsTask() && (!t.IsGenericType || t.GenericTypeArguments[0].Equals(typeof(void)));
        }

        public static Type? TaskResultType(this Type t)
        {
            if (!t.IsAssignableTo(typeof(Task)) || !t.IsGenericType) {
                return null;
            }
            
            Type taskArg = t.GenericTypeArguments[0];
            return taskArg;
        }
    }
}
