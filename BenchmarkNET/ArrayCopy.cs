using System;
using BenchmarkDotNet.Attributes;

namespace BenchmarkNET
{
    [MemoryDiagnoser]
    public class ArrayCopy
    {
        private const int ArraySize = 1_000_000;

        private readonly int[] _source = new int[ArraySize];

        public ArrayCopy()
        {
            var random = new Random();
            for (var i = 0; i < ArraySize; i++)
            {
                _source[i] = random.Next();
            }
        }

        [Benchmark]
        public int[] LoopCopy()
        {
            var target = new int[ArraySize];
            for (var i = 0; i < ArraySize; i++)
            {
                target[i] = _source[i];
            }

            return target;
        }

        [Benchmark]
        public unsafe int[] PointerCopy()
        {
            var target = new int[ArraySize];
            fixed (int* sourceFixed = &_source[0])
            fixed (int* targetFixed = &target[0])
            {
                var srcPoint = sourceFixed;
                var destPoint = targetFixed;
                for (var i = 0; i < ArraySize; i++)
                {
                    *(destPoint++) = *(srcPoint++);
                }
            }
            return target;
        }

        [Benchmark]
        public int[] NativeCopy()
        {
            var target = new int[ArraySize];
            _source.CopyTo(target, 0);
            return target;
        }
    }
}