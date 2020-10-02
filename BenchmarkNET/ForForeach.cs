using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace BenchmarkNET
{
    public class ForForeach
    {
        private const int CollectionSize = 1_000_000;

        private readonly List<int> _list = new List<int>(CollectionSize);
        private readonly int[] _array = new int[CollectionSize];

        public ForForeach()
        {
            var random = new Random();
            for (int i = 0; i < CollectionSize; i++)
            {
                var val = random.Next();
                _list.Add(val);
                _array[i] = val;
            }
        }

        [Benchmark]
        public void ForeachLoopOverList()
        {
            foreach (var el in _list)
            {
                // do stuff
            }
        }

        [Benchmark]
        public void ForLoopOverList()
        {
            for (var i = 0; i < _list.Count; i++)
            {
                var el = _list[i];
                // do stuff
            }
        }
        
        [Benchmark]
        public void ForeachLoopOverArray()
        {
            foreach (var el in _array)
            {
                // do stuff
            }
        }

        [Benchmark]
        public void ForLoopOverArray()
        {
            for (var i = 0; i < _array.Length; i++)
            {
                var el = _array[i];
                // do stuff
            }
        }
    }
}