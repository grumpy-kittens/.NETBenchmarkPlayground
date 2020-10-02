using BenchmarkDotNet.Attributes;

namespace BenchmarkNET
{
    [MemoryDiagnoser]
    public class ClassVsStructs
    {
        private const int ArraySize = 10_000;

        [Benchmark]
        public void Class()
        {
            var array = new PointClass[ArraySize];
            for (var i = 0; i < ArraySize; i++)
            {
                array[i] = new PointClass(1, 1);
            }
        }

        [Benchmark]
        public void FinalizedClass()
        {
            var array = new PointFinalizedClass[ArraySize];
            for (var i = 0; i < ArraySize; i++)
            {
                array[i] = new PointFinalizedClass(1, 1);
            }
        }

        [Benchmark]
        public void Struct()
        {
            var array = new PointStruct[ArraySize];
            for (var i = 0; i < ArraySize; i++)
            {
                array[i] = new PointStruct(1, 1);
            }
        }


        private class PointClass
        {
            public PointClass(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }
        }

        private class PointFinalizedClass: PointClass
        {
            public PointFinalizedClass(int x, int y) : base(x, y)
            {
            }

            ~PointFinalizedClass()
            {
            }
        }
        
        private struct PointStruct
        {
            public PointStruct(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}