using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace BenchmarkNET
{
    public class DynamicInstanceConstructors
    {
        private static readonly ConstructorInfo StringBuilderConstructor =
            typeof(StringBuilder).GetConstructor(new Type[0]);

        private static readonly ConstructorDelegate<StringBuilder> StringBuilderFancyConstructor =
            GetConstructor<StringBuilder>();

        [Benchmark]
        public StringBuilder DefaultConstructor()
        {
            var result = new StringBuilder();

            if (result.GetType() != typeof(StringBuilder))
                throw new InvalidOperationException($"Object is not a {nameof(StringBuilder)}");

            return result;
        }

        [Benchmark]
        public StringBuilder ActivatorCreateInstance()
        {
            var stringBuilderType = typeof(StringBuilder);
            var result = Activator.CreateInstance(stringBuilderType) as StringBuilder;

            if (result?.GetType() != stringBuilderType)
                throw new InvalidOperationException($"Object is not a {nameof(StringBuilder)}");

            return result;
        }

        [Benchmark]
        public StringBuilder SimpleReflection()
        {
            var result = StringBuilderConstructor.Invoke(new object[0]) as StringBuilder;

            if (result?.GetType() != typeof(StringBuilder))
                throw new InvalidOperationException($"Object is not a {nameof(StringBuilder)}");

            return result;
        }

        [Benchmark]
        public StringBuilder DynamicMethod()
        {
            var result = StringBuilderFancyConstructor();

            if (result?.GetType() != typeof(StringBuilder))
                throw new InvalidOperationException($"Object is not a {nameof(StringBuilder)}");

            return result;
        }

        private delegate T ConstructorDelegate<out T>();

        private static ConstructorDelegate<T> GetConstructor<T>()
        {
            var type = typeof(T);
            var defaultConstructor = type.GetConstructor(new Type[0]);

            if (defaultConstructor is null)
                throw new Exception($"Can't find default constructor for '{type.Name}'.");

            var methodName = type.Name + "dynamic_ctor";
            var method = new DynamicMethod(methodName, type, new Type[0], typeof(Activator));
            var ilGen = method.GetILGenerator();
            ilGen.Emit(OpCodes.Newobj, defaultConstructor);
            ilGen.Emit(OpCodes.Ret);

            var result = (ConstructorDelegate<T>) method.CreateDelegate(typeof(ConstructorDelegate<T>));
            return result;
        }
    }
}