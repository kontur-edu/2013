using System;

namespace NUnit_demo
{
	public class Ratio
	{
		private readonly int numerator;
		private readonly int denominator;

		public Ratio(int numerator, int denominator)
		{
			var absNum = Math.Abs(numerator);
			var absDen = Math.Abs(denominator);

			var gcd = Gcd(absNum, absDen);
			var sign = Math.Sign(numerator) * Math.Sign(denominator);
			this.numerator = sign * absNum / gcd;
			this.denominator = absDen / gcd;
		}

		public static int Gcd(int a, int b)
		{
			return b == 0 ? a : Gcd(b, a % b);
		}

		public override string ToString()
		{
			return string.Format("{0}/{1}", numerator, denominator);
		}
	}
}
