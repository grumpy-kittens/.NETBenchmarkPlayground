using System;
using System.Reflection;
using System.Reflection.Emit;
using BenchmarkDotNet.Attributes;

namespace BenchmarkNET
{
    public class DynamicGettersAndSetters
    {
        private readonly DynamicMethods.PropertyGetter _dynamicGetter = DynamicMethods.GetPropertyGetter(
            typeof(Foo),
            typeof(Foo).GetProperty(nameof(Foo.Bar))
        );

        private readonly DynamicMethods.PropertySetter _dynamicSetter = DynamicMethods.GetPropertySetter(
            typeof(Foo),
            typeof(Foo).GetProperty(nameof(Foo.Bar))
        );

        private readonly MethodInfo _getter = typeof(Foo).GetProperty(nameof(Foo.Bar))?.GetGetMethod();
        private readonly MethodInfo _setter = typeof(Foo).GetProperty(nameof(Foo.Bar))?.GetSetMethod();

        [Benchmark]
        public void Getter()
        {
            var foo = new Foo {Bar = 5};
            var value = foo.Bar;
        }

        [Benchmark]
        public void Setter()
        {
            var foo = new Foo {Bar = 5};
            foo.Bar = 1000;
        }

        [Benchmark]
        public void ReflectionGetter()
        {
            var foo = new Foo {Bar = 5};
            var value = (int) _getter.Invoke(foo, new object[0]);
        }

        [Benchmark]
        public void ReflectionSetter()
        {
            var foo = new Foo {Bar = 5};
            _setter.Invoke(foo, new object[] {1000});
        }

        [Benchmark]
        public void DynamicReflectionGetter()
        {
            var foo = new Foo {Bar = 5};
            var value = (int) _dynamicGetter(foo);
        }

        [Benchmark]
        public void DynamicReflectionSetter()
        {
            var foo = new Foo {Bar = 5};
            _dynamicSetter.Invoke(foo, 1000);
        }


        private class Foo
        {
            public int Bar { get; set; }
        }

        private class DynamicMethods
        {
            public delegate object PropertyGetter(object target);

            public delegate void PropertySetter(object target, object value);

            public static PropertyGetter GetPropertyGetter(Type type, PropertyInfo propertyInfo)
            {
                var propertyGetter = propertyInfo.GetGetMethod();

                if (propertyGetter is null)
                    throw new Exception($"Property getter for '{propertyInfo.Name}' could not be found.");

                var newMethod = new DynamicMethod(
                    "dynamic_get" + propertyInfo.Name,
                    typeof(object),
                    new[] {typeof(object)},
                    type,
                    true
                );

                var ilGen = newMethod.GetILGenerator();

                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Call, propertyGetter);

                if (propertyGetter.ReturnType.GetTypeInfo().IsValueType)
                {
                    ilGen.Emit(OpCodes.Box, propertyGetter.ReturnType);
                }

                ilGen.Emit(OpCodes.Ret);

                return newMethod.CreateDelegate(typeof(PropertyGetter)) as PropertyGetter;
            }

            public static PropertySetter GetPropertySetter(Type type, PropertyInfo propertyInfo)
            {
                var propertySetter = propertyInfo.GetSetMethod(true);

                if (propertySetter is null)
                    throw new Exception($"Property setter for '{propertyInfo.Name}' could not be found.");

                var newMethod = new DynamicMethod(
                    "dynamic_set" + propertyInfo.Name,
                    typeof(void),
                    new[] {typeof(object), typeof(object)},
                    type,
                    true
                );

                var ilGen = newMethod.GetILGenerator();

                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldarg_1);

                var parameterType = propertySetter.GetParameters()[0].ParameterType;
                if (parameterType.GetTypeInfo().IsValueType)
                {
                    ilGen.Emit(OpCodes.Unbox_Any, parameterType);
                }

                ilGen.Emit(OpCodes.Call, propertySetter);
                ilGen.Emit(OpCodes.Ret);

                return newMethod.CreateDelegate(typeof(PropertySetter)) as PropertySetter;
            }
        }
    }
}