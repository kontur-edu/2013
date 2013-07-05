using System;

namespace TestingTask
{
	class Program
	{
		static void Main(string[] args)
		{
			var heap = new Heap<int>(i => i);
			heap.Add(1);
			heap.Add(2);
			heap.Add(3);
			heap.Add(2);
			Console.WriteLine(heap.ExtractBest());
			Console.WriteLine(heap.ExtractBest());
			Console.WriteLine(heap.ExtractBest());
			heap.Add(5);
			Console.WriteLine(heap.ExtractBest());
			// outputs 3 2 2 5 → everything is OK!
		}
	}
}
