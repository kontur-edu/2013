using System;
using NUnit.Framework;

namespace IntroToFunctionalProgramming
{
    public static class MaybeMonadExtension
    {
        //bind
        public static Nullable<T> SelectMany<S, T>(this Nullable<S> x, Func<S, Nullable<T>> f)  
            where S : struct 
            where T : struct
        {
            return x.HasValue ? f(x.Value) : null;
        }

        public static Nullable<S> Return<S>(S x) where S : struct
        {
            return x;
        }

        //fmap, liftM
        public static Nullable<T> Select<S, T>(this Nullable<S> x, Func<S, T> f)
            where S : struct
            where T : struct
        {
            return x.SelectMany(y => Return(f(y)));
        }

        //sugar herlper
        public static Nullable<T> SelectMany<S, T>(this Nullable<S> x, Func<S, Nullable<T>> f, Func<S, T, Nullable<T>> id)
            where S : struct
            where T : struct
        {
            return x.SelectMany(f);
        }
    }

    [TestFixture]
    public class MaybeMonadTests
    {
        [Test]
        public void SelectTest()
        {
            int? x1 = 5;
            int? x2 = null;

            var y1 =
                from y in
                    from x in x1
                    select (x + 5)
                select (y - 10);
            var y2 =
                from y in
                    from x in x2
                    select (x + 5)
                select (y - 10);

            Assert.AreEqual(0, y1);
            Assert.AreEqual(null, y2);
        }

        protected double? MySqrt(double x)
        {
            if (x >= 0)
                return Math.Sqrt(x);
            return null;
        }

        [Test]
        public void SelectManyTest()
        {
            var y1 =
                from x in (double?)null
                from y in MySqrt(x)
                select y;

            var y11 = ((double?)null).SelectMany(MySqrt);
            
            Assert.AreEqual(null, y1);
            Assert.AreEqual(null, y11);

            var y2 = ((double?) 12).SelectMany(MySqrt).SelectMany(MySqrt);

            Assert.AreEqual(1.86, y2, 0.01);
        }
    }
}
