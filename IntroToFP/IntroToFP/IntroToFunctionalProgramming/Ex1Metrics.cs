using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace IntroToFunctionalProgramming
{
    public class Datum {
        public double X;
        public double Y;
    }

    public class Data {
        public Datum D1;
        public Datum D2;
    }

    public static class Metrics {
        private static readonly Dictionary<string, Func<Data, double>> Items = 
            new Dictionary<string, Func<Data, double>> {
                {"Only X", data => Math.Abs(data.D1.X - data.D2.X)},
                {"Only Y", data => Math.Abs(data.D1.Y - data.D2.Y)},
                {"Manhattan", data => Math.Abs(data.D1.X - data.D2.X) + Math.Abs(data.D1.Y - data.D2.Y)}
            };

        public static string[] KnownMetrics = Items.Keys.ToArray();
        public static double GetDistance(string metric, Data data)
        {
            if (!KnownMetrics.Contains(metric))
                throw new ArgumentException(String.Format("Don't know such metric [{0}]", metric), "metric");
            return Items[metric](data);
        }
    }

    [TestFixture]
    internal class MetricsTests
    {
        [Test]
        public void OnlyXTest()
        {
            Assert.AreEqual(1,
                            Metrics.GetDistance("Only X",
                                                new Data
                                                    {D1 = new Datum {X = 0, Y = 0}, D2 = new Datum {X = 1, Y = 2.5}}));
            Assert.AreEqual(0,
                            Metrics.GetDistance("Only X",
                                                new Data
                                                    {D1 = new Datum {X = 1, Y = 0}, D2 = new Datum {X = 1, Y = 2.5}}));
        }

        [Test]
        public void OnlyYTest()
        {
            Assert.AreEqual(2.5,
                            Metrics.GetDistance("Only Y",
                                                new Data
                                                    {D1 = new Datum {X = 0, Y = 0}, D2 = new Datum {X = 1, Y = 2.5}}));
            Assert.AreEqual(0,
                            Metrics.GetDistance("Only Y",
                                                new Data
                                                    {D1 = new Datum {X = 0, Y = 2.5}, D2 = new Datum {X = 1, Y = 2.5}}));
        }

        [Test]
        public void ManhattanTest()
        {
            Assert.AreEqual(3.5,
                            Metrics.GetDistance("Manhattan",
                                                new Data
                                                    {D1 = new Datum {X = 0, Y = 0}, D2 = new Datum {X = 1, Y = 2.5}}));
            Assert.AreEqual(0,
                            Metrics.GetDistance("Manhattan",
                                                new Data
                                                    {D1 = new Datum {X = 1, Y = 2.5}, D2 = new Datum {X = 1, Y = 2.5}}));
        }

        [Test]
        public void EuclideanTest()
        {
            Assert.AreEqual(2.69258,
                            Metrics.GetDistance("Euclidean",
                                                new Data
                                                    {D1 = new Datum {X = 0, Y = 0}, D2 = new Datum {X = 1, Y = 2.5}}),
                            0.00001);
            Assert.AreEqual(0,
                            Metrics.GetDistance("Euclidean",
                                                new Data
                                                    {D1 = new Datum {X = 1, Y = 2.5}, D2 = new Datum {X = 1, Y = 2.5}}),
                            0.00001);
        }
    }
}
