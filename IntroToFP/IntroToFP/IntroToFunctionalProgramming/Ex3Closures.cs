using System;
using NUnit.Framework;

namespace IntroToFunctionalProgramming
{
    public static class ClosuresExample
    {
        public static Tuple<Func<string>, Action<int>> MakeAccount(int initialSum)
        {
            var accountTotal = initialSum;
            return new Tuple<Func<string>, Action<int>>(
                () => String.Format("${0}", accountTotal),
                sum => accountTotal -= sum);
        }
    }

    [TestFixture]
    internal class ClosuresExampleTests
    {
        [Test]
        public void ClosuresExampleTest()
        {
            var fs = ClosuresExample.MakeAccount(10000);
            var show = fs.Item1;
            var withdraw = fs.Item2;
            Assert.AreEqual("$10000", show());
            withdraw(2000);
            Assert.AreEqual("$8000", show());
            withdraw(8000);
            Assert.AreEqual("$0", show());
        }
    }
}
