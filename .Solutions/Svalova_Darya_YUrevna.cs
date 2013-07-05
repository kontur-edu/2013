using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace testTask {


	public class Reader {
		System.IO.TextReader reader;
		string currentString;
		Regex regex4Num;
		Regex regex4Colors;
		Match numbersMatch;
		Match colorsMatch;

		public Reader(string fileName) {
			reader = new System.IO.StreamReader(fileName);
			RegExp4ArgsInit();
		}

		public Reader(System.IO.TextReader input) {
			reader = input;
			RegExp4ArgsInit();
		}

		private void RegExp4ArgsInit() {
			string pattern4Num = @"(\d+)";
			string pattern4Colors = @"([ROYGBV\.])";
			regex4Colors = new Regex(pattern4Colors);
			regex4Num = new Regex(pattern4Num);
		}

		public int GetNextInt() {
			int result = int.Parse(numbersMatch.Value);
			numbersMatch = numbersMatch.NextMatch();
			return result;
		}

		public char GetNextColor() {
			char result = char.Parse(colorsMatch.Value);
			colorsMatch = colorsMatch.NextMatch();
			return result;
		}

		public void ReadLine() {
			currentString = reader.ReadLine();
			numbersMatch = regex4Num.Match(currentString);
			colorsMatch = regex4Colors.Match(currentString);
		}

		public void Close() {
			reader.Close();
		}

	}

	public class Writer {
		System.IO.TextWriter writer;

		public Writer(string fileName) {
			writer = new System.IO.StreamWriter(fileName);
		}

		public Writer(System.IO.TextWriter output) {
			writer = output;
		}

		public void Close() {
			writer.Close();
		}

		public void WriteLine(string line) {
			writer.WriteLine(line);
		}
	}

	class Comparer<T> : IEqualityComparer<T> { 
		private readonly Func<T, T, bool> _comparer; 

		public Comparer(Func<T, T, bool> comparer) {
			_comparer = comparer; 
		} 

		public bool Equals(T x, T y) { 
			return _comparer(x, y); 
		} 

		public int GetHashCode(T obj) {
			return obj.ToString().ToLower().GetHashCode(); 
		}
	}

	public class Fragment {
		public static char[] planeName = { 'F', 'B', 'D', 'U', 'L', 'R' };
		public static List<Dictionary<int, int>> allPermutations;
		public static int numberOfDementios = 3;
		public List<int> dementions;
		public static int numberOfColors = 6;
		public List<char> colors;
		public List<int> mapping;
		public string answer;


		public Fragment(Fragment donor) {
			dementions = new List<int>();
			mapping = new List<int>();
			colors = new List<char>();
			for (int i = 0; i < donor.dementions.Count(); i++) {
				dementions.Add(donor.dementions[i]);
				mapping.Add(donor.mapping[i]);
			}
			for (int i = 0; i < donor.colors.Count(); i++)
				colors.Add(donor.colors[i]);
			answer = "";
		}

		public Fragment(Reader reader) {
			reader.ReadLine();
			dementions = new List<int>();
			mapping = new List<int>();
			for (int i = 0; i < numberOfDementios; i++) {
				dementions.Add(reader.GetNextInt());
				mapping.Add(0);
			}
			colors = new List<char>();
			for (int i = 0; i < numberOfColors; i++) {
				colors.Add(reader.GetNextColor());
			}
			answer = "";
		}

		public static void InitAllPermutations() {
			int[] currentPermutation = { 0, 1, 2, 3, 4, 5 };
			HashSet<int[]> reverseAllPermutations = new HashSet<int[]>();
			Func<int[], int[], bool> cmp = (a, b) => a.SequenceEqual(b);
			Comparer<int[]> comparer = new Comparer<int[]>(cmp);
			for (int i = 0; i < 4; i++) {
				TurnForward(ref currentPermutation);
				for (int j = 0; j < 4; j++) {
					TurnLeft(ref currentPermutation);
					for (int k = 0; k < 4; k++) {
						AddPermutation(currentPermutation, reverseAllPermutations, comparer);
						TurnRight(ref currentPermutation);
					}
				}
			}
			allPermutations = new List<Dictionary<int, int>>();
			foreach (int[] permutation in reverseAllPermutations) {
				allPermutations.Add(new Dictionary<int, int>());
				for (int j = 0; j < 6; j++)
					allPermutations[allPermutations.Count - 1].Add(permutation[j], j);
			}
		}

		private static void AddPermutation(int[] permutation, HashSet<int[]> permutations, Comparer<int[]> comparer) {
			int[] clone = (int[])permutation.Clone();
			if (!permutations.Contains(clone, comparer))
				permutations.Add(clone);
		}

		private static void TurnForward(ref int[] block) {
			int[] turnForwardPermutation = { 3, 2, 0, 1, 4, 5 };
			ApplyPermutation(ref block, turnForwardPermutation);
		}

		private static void TurnLeft(ref int[] block) {
			int[] dropLeftPermutation = { 0, 1, 4, 5, 3, 2 };
			ApplyPermutation(ref block, dropLeftPermutation);
		}

		private static void TurnRight(ref int[] block) {
			int[] turnRightPermutation = { 4, 5, 2, 3, 1, 0 };
			ApplyPermutation(ref block, turnRightPermutation);
		}

		private static void ApplyPermutation(ref int[] array, int[] permutation) {
			int[] clone = (int[])array.Clone();
			for (int i = 0; i < numberOfColors; i++)
				array[i] = clone[permutation[i]];
		}

		public void AddAnswer(int[] coordinates, Fragment sample) {
			for (int i = 0; i < 24; i++) {
				for (int j = 0; j < 3; j++) {
					int color1 = allPermutations[i][j * 2];
					int color2 = allPermutations[i][j * 2 + 1];
					if (sample.colors[j * 2] != this.colors[color1] || 
								sample.colors[j * 2 + 1] != this.colors[color2])
						break;
					if ((sample.dementions[j] != this.dementions[color1 / 2])
								&& (sample.dementions[j] != 0))
						break;
					if (j == 2) {
						answer += planeName[allPermutations[i][0]] + " " + planeName[allPermutations[i][2]] + " ";
						i+=24;
					}
				}
			}
			for (int i = 0; i < 3; i++)
				answer += coordinates[i] + " ";
		}

		public bool Equals(Fragment sample) {
			bool[] used = { false, false, false };
			for (int i = 0; i < numberOfDementios; i++) {
				int planeIndex = FindPlane(sample.colors[i * 2], sample.colors[i * 2 + 1], sample.dementions[i], used);
				if (planeIndex < 0)
					return false;
				used[planeIndex] = true;
				mapping[i] = planeIndex;
			}
			return true;
		}

		int FindPlane(char color1, char color2, int demention, bool[] used) {
			for (int j = 0; j < numberOfColors ; j++) {
				int pairIndex = j / 2;
				int secondColor = FindPair(j);
				if ((used[pairIndex] || color1 != this.colors[j] || color2 != this.colors[secondColor])
							|| (demention != this.dementions[pairIndex] && demention != 0))
					continue;
				return pairIndex;
			}
			return -1;
		}

		int FindPair(int planeIndex) {
			return planeIndex ^ 1;
		}

	}

	public class Program {

		static List<Fragment> fragments;
		static int n;
		static Fragment parallelepiped;
		static int basePlane;  // пара граней вдоль которой резали
		static string inputFileName = "input.txt";
		static string outputFileName = "output.txt";

		public static void Main(string[] args) {
			#if(ONLINE_JUDGE)
				Reader argsReader = new Reader(Console.In);
			#else
				Reader argsReader = new Reader(inputFileName);
			#endif
			parallelepiped = new Fragment(argsReader);

			argsReader.ReadLine();
			n = argsReader.GetNextInt();
			ReadFragments(argsReader);
			argsReader.Close();

			basePlane = FindBasePlane(parallelepiped);

			Fragment.InitAllPermutations();
			if (basePlane < 0) {
				int[] NullCoordinates = { 0, 0, 0 };
				fragments[0].AddAnswer(NullCoordinates, parallelepiped);
			}
			else {
				RecoverParallelepiped(parallelepiped);
			}

			PrintAnswer();
		}

		static int FindEqFragment(Fragment sample) {
			for (int i = 0; i < fragments.Count(); i++) {
				if (fragments[i].answer != "")
					continue;
				if (fragments[i].Equals(sample))
					return i;
			}
			return -1;
		}

		static int FindBasePlane(Fragment sample) {
			int basePlane = -1;
			for (int i = 0; i < Fragment.numberOfDementios && basePlane < 0; i++) {
				Fragment tmp = new Fragment(sample);
				tmp.dementions[i] = 0;
				tmp.colors[i * 2] = '.';
				for (int j = 0; j < n && basePlane < 0; j++) {
					if (fragments[j].Equals(tmp))
						basePlane = i;
				}
			}
			return basePlane;
		}

		static void RecoverParallelepiped(Fragment parallelepiped) {
			Fragment sample = new Fragment(parallelepiped);
			int[] currentCoordinates = { 0, 0, 0 };
			int recoveredPartLength = 0;
			sample.colors[basePlane * 2 + 1] = '.';
			sample.dementions[basePlane] = 0;
			while (recoveredPartLength != parallelepiped.dementions[basePlane]) {
				int next = FindEqFragment(sample);
				if (next < 0) {
					sample.colors[basePlane * 2 + 1] = parallelepiped.colors[basePlane * 2 + 1];
					continue;
				}
				fragments[next].AddAnswer(currentCoordinates, sample);
				Fragment current = new Fragment(fragments[next]);
				recoveredPartLength += current.dementions[current.mapping[basePlane]];
				currentCoordinates[basePlane] = recoveredPartLength;
				sample.colors[basePlane * 2] = '.';
			}
		}

		static void ReadFragments(Reader reader) {
			fragments = new List<Fragment>();
			for (int i = 0; i < n; i++)
				fragments.Add(new Fragment(reader));
		}

		static void PrintAnswer() {
			#if(ONLINE_JUDGE)
				Writer answerWriter = new Writer(Console.Out);
			#else
				Writer answerWriter = new Writer(outputFileName);
			#endif
			for (int i = 0; i < fragments.Count(); i++)
				answerWriter.WriteLine(fragments[i].answer);
			answerWriter.Close();
		}
	}
}