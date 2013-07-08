using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace IntroToFunctionalProgramming
{
    public static class FoldExamples
    {
        public static U Fold<T, U>(Func<U, T, U> step, U initialAcc, IEnumerable<T> collection)
        {
            //to avoid multiple enumeration
            var coll = collection as T[] ?? collection.ToArray();

            if (!coll.Any()) 
                return initialAcc;
            return Fold(step, step(initialAcc, coll.First()), coll.Skip(1));
        }

        public static int StringToInt(string s)
        {
            var sWithIndices = s.Reverse().Zip(Enumerable.Range(0, s.Length), (ch, idx) => new Tuple<char, int>(ch, idx));
            return
                Fold(
                    (acc, elem) => acc + (int)(Char.GetNumericValue(elem.Item1) * Math.Pow(10, elem.Item2)), 0,
                    sWithIndices);
        }

        //TODO: now, make StringToInt using Aggregate from LINQ
        public static int StringToIntAggregate(string s)
        {
            throw new NotImplementedException();
        }
        
        //TODO:
        public static IEnumerable<T> Flatten<T>(IEnumerable<IEnumerable<T>> deepList)
        {
            throw new NotImplementedException();
        }

        //TODO:
        public static IEnumerable<IEnumerable<T>> PartitionBy<T>(Func<T, T, bool> criterion, IEnumerable<T> collection)
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    internal class FoldExamplesTests
    {
        [Test]
        public void FoldTest()
        {
            Assert.AreEqual(6, FoldExamples.Fold((acc, elem) => acc + elem, 0, Enumerable.Range(1, 3)));
            Assert.AreEqual(256, FoldExamples.StringToInt("256"));
            //or, you can use Aggregate
            Assert.AreEqual(6, Enumerable.Range(1, 3).Aggregate(0, (acc, elem) => acc + elem));
        }

        [Test]
        public void StringToIntAggregateTest()
        {
            Assert.AreEqual(256, FoldExamples.StringToIntAggregate("256"));
        }

        [Test]
        public void FlattenTest()
        {
            Assert.AreEqual(new[] {3, 4, 1, 2, 2, 5},
                            FoldExamples.Flatten(new[] {new[] {3, 4}, new[] {1}, new[] {2, 2, 5}}));
        }

        [Test]
        public void PartitionByTest()
        {
            Assert.AreEqual(new[] {new[] {1, 2}, new[] {0, 9}, new[] {5, 6, 10}, new[] {1}},
                            FoldExamples.PartitionBy((x, y) => x < y, new[] {1, 2, 0, 9, 5, 6, 10, 1}));
        }
    }
}
