using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace IntroToFunctionalProgramming
{
    public struct Vector
    {
        public double X;
        public double Y;
    }

    public static class TypeClassesExamples
    {
        private static Dictionary<Type, Func<object, string>> Printable =
            new Dictionary<Type, Func<object, string>>
                {
                    {typeof (int), x => String.Format("I'm an int with value {0}", x)},
                    {typeof (double), x => String.Format("I'm a double with value {0}", x)},
                    {
                        typeof (Vector), x =>
                                             {
                                                 var vector = (Vector) x;
                                                 return String.Format(
                                                     "I'm a vector with value ({0}, {1})",
                                                     vector.X,
                                                     vector.Y);
                                             }
                    }
                };
        
        public static string Show(object x)
        {
            var type = x.GetType();
            if (Printable.ContainsKey(type))
                return Printable[type](x);
            throw new ArgumentException(String.Format("There aren't any instances of Printable for type {0}", type));
        }

        public static void RegisterInstanceOfShow<T>(Func<object, string> printer)
        {
            var type = typeof (T);
            if (Printable.ContainsKey(type))
                throw new ArgumentException(String.Format("An instance of Show for type {0} has been already registered.", type));
            Printable.Add(type, printer);
        }

        public static void UnregisterInstanceOfShow<T>()
        {
            var type = typeof (T);
            if (!Printable.ContainsKey(type))
                throw new ArgumentException(String.Format("There were no instances of Show for type {0}", type));
            Printable.Remove(type);
        }

        //TODO:
        public static T Add<T>(T x, T y)
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    internal class TypeClassesExamplesTests
    {
        [Test]
        public void PrintableTest()
        {
            Assert.AreEqual("I'm an int with value -666", TypeClassesExamples.Show(-666));
            Assert.AreEqual("I'm a double with value 0", TypeClassesExamples.Show(0.0));
            Assert.AreEqual("I'm a vector with value (5, -7.5)", TypeClassesExamples.Show(new Vector {X = 5, Y = -7.5}));
            Assert.Throws<ArgumentException>(() => TypeClassesExamples.Show("foo"));
        }

        [Test]
        public void ShowRegister()
        {
            Assert.Throws<ArgumentException>(() => TypeClassesExamples.Show("foo"));
            TypeClassesExamples.RegisterInstanceOfShow<string>(x => String.Format("I'm a string with value [{0}]", x));
            Assert.AreEqual("I'm a string with value [foo]", TypeClassesExamples.Show("foo"));
            TypeClassesExamples.UnregisterInstanceOfShow<string>();
            Assert.Throws<ArgumentException>(() => TypeClassesExamples.Show("foo"));
        }

        [Test]
        public void AddableTest()
        {
            Assert.AreEqual(3, TypeClassesExamples.Add(-7, 10));
            Assert.AreEqual(-12.3, TypeClassesExamples.Add(-15, 2.7));
            Assert.AreEqual(new Vector {X = 0, Y = 0}, TypeClassesExamples.Add(new Vector{X = 5, Y = 6}, new Vector{X = -5, Y = -6}));
        }
    }
}
