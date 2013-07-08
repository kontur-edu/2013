using NUnit.Framework;

namespace IntroToFunctionalProgramming
{
    public static class Addable
    {
        public static int Add(this int x, int y)
        {
            return x + y;
        }

        public static Vector Add(this Vector v1, Vector v2)
        {
            return new Vector {X = v1.X + v2.X, Y = v1.Y + v2.Y};
        }
    }

    public static class AddableUser
    {
        public static T Add<T>(T x, T y)
        {
            dynamic dx = x;
            dynamic dy = y;
            return Addable.Add(dx, dy);
        }
    }

    [TestFixture]
    internal class Ex7Tests
    {
        [Test]
        public void InstancesTest()
        {
            Assert.AreEqual(3, AddableUser.Add(1, 2));
            Assert.AreEqual(new Vector {X = 0, Y = 1}, AddableUser.Add(new Vector {X = 5, Y = 6}, new Vector{X = -5, Y = -5}));
        }
    }
}
