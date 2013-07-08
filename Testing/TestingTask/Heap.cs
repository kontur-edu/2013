using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace TestingTask
{
	// Задача: разаботать систему тестов для этого класса с использованием NUnit
	

	//http://en.wikipedia.org/wiki/Heap_(data_structure)
	public class Heap<T> : IEnumerable<T>
	{
		private readonly Func<T, int> score;
		private readonly List<T> data = new List<T>(10);

		public Heap(Func<T, int> score)
		{
			this.score = score;
		}

        //find max/min
		public T PeekBest()
		{
			return data[0];
		}

        //heap size
		public int Count { get { return data.Count; } }

        // return - delete best
		public T ExtractBest()
		{
			T res = data[0];
			data[0] = data[data.Count - 1];
			data.RemoveAt(data.Count - 1);
			Down(0);
			CheckConsistency();
			return res;
		}

		public void Add(T item)
		{
			data.Add(item);
			Up(data.Count - 1);
			CheckConsistency();
		}

		private void Up(int i)
		{
			if (i == 0) return;
            var parent = (i - 1) / 2;
            if (score(data[parent]) < score(data[i]))
			{
				Swap(parent, i);
				Up(parent);
			}
		}

		[Conditional("HEAP_CLASS_TESTING")]
		private void CheckConsistency()
		{
			// Это метод проверки инварианта структуры данных "Куча". Его можно использовать для 
            // тестирования этого класса.
			// Атрибут Conditional дает инструкцию компилятору игнорировать все вызовы этого метода,
            // если только не определен символ HEAP_CLASS_TESTING.
			// Определить символ можно с помощью инструкции #define HEAP_CLASS_TESTING
            // (см начало этого файла)
			for (int i = 0; i < data.Count; i++)
			{
                var c1 = i * 2 + 1;
                var c2 = i * 2 + 2;
				if (c1 < data.Count && score(data[i]) < score(data[c1]) ||
				    c2 < data.Count && score(data[i]) < score(data[c2])
					)
				{
					Console.WriteLine(this);
					throw new Exception("Inconsistent heap");
				}
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return data.GetEnumerator();
		}

		public override string ToString()
		{
			return string.Join(" ", data.Select(item => item.ToString()));
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private void Swap(int j, int i)
		{
			var t = data[j];
			data[j] = data[i];
			data[i] = t;
		}

        //=?
		private void Down(int i)
		{
            var c1 = i * 2 + 1;
            var c2 = i * 2 + 2;
			if (c1 >= data.Count) return; // уже на дне!
			var c = (c2 < data.Count && score(data[c2]) > score(data[c1])) ? c2 : c1;
			if (score(data[i]) < score(data[c]))
			{
				Swap(i, c);
				Down(c);
			}
		}
	}
}