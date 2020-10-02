using System.Text;
using BenchmarkDotNet.Attributes;

namespace BenchmarkNET
{
    [MemoryDiagnoser]
    public class StringOperations
    {
        private const int StringLength = 1000;

        [Benchmark]
        public string StringConcat()
        {
            var result = "";
            for (var i = 0; i < StringLength; i++)
            {
                result += "+";
            }

            return result;
        }

        [Benchmark]
        public string StringBuilder()
        {
            var result = new StringBuilder();
            for (var i = 0; i < StringLength; i++)
            {
                result.Append("+");
            }

            return result.ToString();
        }

        [Benchmark]
        public unsafe string ConcatWithPointers()
        {
            var result = new char[StringLength];
            fixed (char* fixedPointer = result)
            {
                var pointer = fixedPointer;
                for (var i = 0; i < StringLength; i++)
                {
                    *pointer++ = '+';
                }
            }

            return new string(result);
        }

        [Benchmark]
        public string ConcatFromCharArray()
        {
            var result = new char[StringLength];
            for (var i = 0; i < StringLength; i++)
            {
                result[i] = '+';
            }

            return new string(result);
        }
    }
}