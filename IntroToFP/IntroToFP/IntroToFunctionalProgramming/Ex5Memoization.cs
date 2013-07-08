using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace IntroToFunctionalProgramming
{
    delegate Func<A,R> Recursive<A, R>(Recursive<A,R> r);
    
    public static class MemoizationExamples
    {
        public static Func<T, U> Memoize<T, U>(Func<T, U> f)
        {
            var cache = new Dictionary<T, U>();

            return arg =>
                {
                    if (!cache.ContainsKey(arg))
                    {
                        Console.WriteLine("cache miss for {0}", arg);
                        cache[arg] = f(arg);
                    }
                    Console.WriteLine("cache contains for {0}", arg);
                    return cache[arg];
                };
        }

        public static int Fib(int n)
        {
            return n > 1 ? Fib(n - 1) + Fib(n - 2) : n;
        } 

        public static Func<int, int> GetMemoFib()
        {
            return Memoize<int, int>(Fib);
        }

        //fixed-point combinator
        public static Func<A, R> Y<A, R>(Func<Func<A, R>, Func<A, R>> f)
        {
            Recursive<A, R> rec = r => a => f(r(r))(a);
            return rec(rec);
        }

        public static Func<int, int> GetDeepMemoFib()
        {
            Func<Func<int, int>, Func<int, int>> f = 
                g => 
                    n => 
                        n > 1 ? g(n - 1) + g(n - 2) : n;
            return Y(Memoize(f));
        } 
    }

    [TestFixture]
    internal class MemoizationExamplesTests
    {
        [Test]
        public void ShallowFibTest()
        {
            var fib = MemoizationExamples.GetMemoFib();
            Assert.AreEqual(8, fib(6));
        }

        [Test]
        public void DeepFibTest()
        {
            var fib = MemoizationExamples.GetDeepMemoFib();
            Assert.AreEqual(8, fib(6));
        }
    }
}
