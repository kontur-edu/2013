using NUnit.Framework;

namespace NUnit_demo
{
	[TestFixture]
	public class Ratio_Test
	{
		[Test]
		[TestCase(2, 4, "1/2")]
		[TestCase(2, 6, "1/3")]
		[TestCase(6, 4, "3/2")]
		[TestCase(-6, 4, "-3/2")]
		[TestCase(6, -4, "-3/2")]
		[TestCase(0, -4, "0/1")]
		public void Cancelation(int a, int b, string ratio)
		{
			Assert.AreEqual(ratio, new Ratio(a, b).ToString());
		}
	}
}