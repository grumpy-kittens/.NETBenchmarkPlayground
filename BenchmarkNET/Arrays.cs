using BenchmarkDotNet.Attributes;

namespace BenchmarkNET
{
    [MemoryDiagnoser]
    public class Arrays
    {
        private const int ArraySize = 1000;

        [Benchmark]
        public void ClassicThreeDimensionalArray()
        {
            var source = new int[ArraySize, ArraySize, ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                for (int j = 0; j < ArraySize; j++)
                {
                    for (int k = 0; k < ArraySize; k++)
                    {
                        source[i, j, k]++;
                    }
                }
            }
        }

        [Benchmark]
        public void SingleThreeDimensionalArray_CalculatedIndex()
        {
            var source = new int[ArraySize * ArraySize * ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                for (int j = 0; j < ArraySize; j++)
                {
                    for (int k = 0; k < ArraySize; k++)
                    {
                        var index = k + ArraySize * (j + ArraySize * i);
                        source[index]++;
                    }
                }
            }
        }

        [Benchmark]
        public void SingleThreeDimensionalArray_FixedIndex()
        {
            var source = new int[ArraySize * ArraySize * ArraySize];
            var index = 0;
            for (int i = 0; i < ArraySize; i++)
            {
                for (int j = 0; j < ArraySize; j++)
                {
                    for (int k = 0; k < ArraySize; k++)
                    {
                        source[index]++;
                        index++;
                    }
                }
            }
        }
    }
}