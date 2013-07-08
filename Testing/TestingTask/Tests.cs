using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TestingTask
{
    class Tests
    {
        [NUnit.Framework.TestFixture]
        public class Heap_Test
        {
            [Test]
            public void EmptyHeapTest()
            {
                var h = new Heap<int>(i => i);
                Assert.Throws<ArgumentOutOfRangeException>(() => h.PeekBest());
            }
            
            [Test]
            public void InitializationTest()
            {
                
                var h = new Heap<int>(i => i);
                h.Add(1);
                Assert.AreEqual(1, h.PeekBest());

            }

            [TestCase(2, 5, "5 2")]
            [TestCase(5, 2, "5 2")]
            public void HeapOneChildTest(int a, int b, String expectedString)
            {
                var h = new Heap<int>(i => i);
                h.Add(a);
                h.Add(b);
                Assert.AreEqual(expectedString, h.ToString());
            }

            [TestCase(new[] { 9, 2, 4, 1, 1 })] // bug #1 found
            [TestCase(new[] { 1, 3, 5, 2, 6 })]
            [TestCase(new[] { 1, 1, 1 })]
            [TestCase(new[] { 1, 2, 1 })]
            [TestCase(new[] { 2, 1, 2, 1 })]
            public void HeapArrayTest(int[] data)
            {
                var h = new Heap<int>(i => i);
                foreach (var i in data)
                {
                    h.Add(i);
                }
                var count = data.Count();
                Array.Sort(data, (a, b) => -1 * a.CompareTo(b));
                for (int i = 0; i < count; i++)
                {
                    Assert.AreEqual(data[i], h.ExtractBest());
                }
            }

            [Test]
            [Timeout(10000)]
            public void PerformanceTest()
            {
                const int count = 1000000;
                int seed = 42;
                var rand = new Random(seed);
                var h = new Heap<int>(i => i);
                for (int i = 0; i < count; i++)
                {
                    h.Add(i);
                }
                for (int i = 0; i < count; i++)
                {
                    h.ExtractBest();
                }
                
            }

            [Test]
            public void PermutationsTest()
            {
                const int count = 7;
                var values = new List<int>(count);
                for (int i = 0; i < count; i++)
                {
                    values.Add(i);
                }
                foreach (var value in Permutations(values.ToArray()))
                {
                    HeapArrayTest(value);
                }
            }

            private static IEnumerable<T[]> Permutations<T>(T[] values, int fromInd = 0)
            {
                if (fromInd + 1 == values.Length)
                    yield return values;
                else
                {
                    foreach (var v in Permutations(values, fromInd + 1))
                        yield return v;

                    for (var i = fromInd + 1; i < values.Length; i++)
                    {
                        SwapValues(values, fromInd, i);
                        foreach (var v in Permutations(values, fromInd + 1))
                            yield return v;
                        SwapValues(values, fromInd, i);
                    }
                }
            }

            private static void SwapValues<T>(T[] values, int pos1, int pos2)
            {
                if (pos1 != pos2)
                {
                    T tmp = values[pos1];
                    values[pos1] = values[pos2];
                    values[pos2] = tmp;
                }
            }
        }
    }
}
