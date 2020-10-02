using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace BenchmarkNET
{
    public class ExceptionThrowingTest
    {
        private const int ListSize = 10000;
        private const int NumberSize = 5;

        private readonly char[] _digitsArray = {'1', '2', '3', '4', '5', '6', '7', '8', '9', 'A'};
        private readonly List<string> _numbers = new List<string>();

        public ExceptionThrowingTest()
        {
            var random = new Random();
            for (var i = 0; i < ListSize; i++)
            {
                var stringBuilder = new StringBuilder();
                for (var j = 0; j < NumberSize; j++)
                {
                    var index = random.Next(_digitsArray.Length);
                    stringBuilder.Append(_digitsArray[index]);
                }

                _numbers.Add(stringBuilder.ToString());
            }
        }

        [Benchmark]
        public void TryParseCatch()
        {
            for (var i = 0; i < ListSize; i++)
            {
                try
                {
                    var i1 = int.Parse(_numbers[i]);
                }
                catch (FormatException)
                {
                }
            }
        }

        [Benchmark]
        public void TryParse()
        {
            for (var i = 0; i < ListSize; i++)
            {
                var success = int.TryParse(_numbers[i], out var res);
            }
        }
    }
}