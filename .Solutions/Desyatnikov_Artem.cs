using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParallelepipedCutting.Enums;
using ParallelepipedCutting.Dimensions;
using ParallelepipedCutting.Collections;
using ParallelepipedCutting.Collections.ColorCollections;
using ParallelepipedCutting.Collections.SideCollections;
using ParallelepipedCutting.Formatters;
using ParallelepipedCutting.Parsers;

namespace ParallelepipedCutting
{
	/// <summary>
	/// Содержит методы для работы с хэш-таблицей с множественными
	/// значениями
	/// </summary>
	static class DictionaryUtils
	{
		public static void MultiInsert<Key, Value>(
				this Dictionary<Key, LinkedList<Value>> dict,
				Key key,
				Value val)
		{
			LinkedList<Value> list;
			if (dict.TryGetValue(key, out list))
			{
				list.AddLast(val);
			}
			else
			{
				var newList = new LinkedList<Value>();
				newList.AddLast(val);
				dict[key] = newList;
			}
		}

		public static Dictionary<K2, V> TakeInsertedDictionary<K, K2, V>(
			this Dictionary<K,Dictionary<K2,V>> bigDict,
			K key
			)
		{
			Dictionary<K2, V> dict;
			if (!bigDict.TryGetValue(key, out dict))
			{
				dict = bigDict[key] = new Dictionary<K2, V>();
			}
			return dict;
		}

		public static Value PopFromMultiDictionary<Key, Value>(this Dictionary<Key, LinkedList<Value>> dict, Key key)
		{
			var list = dict[key];
			Value value = list.First.Value;
			list.RemoveFirst();
			if (list.Count == 0)
				dict.Remove(key);
			return value;
		}
	}
}

namespace ParallelepipedCutting
{

	
	/// <summary>
	/// Представление любого параллелепипеда, раскрашенного в некоторые
	/// цвета и ориентированного в пространстве
	/// </summary>
	class Parallelepiped
	{
		/*
		 * Используется массив цветов вместо Dictionary для ускорения доступа к грани
		 * (это придется делать часто)
		 */
		private Color[] _sidesColors;
		private Side[] _coloredSides;
		private UVector3D _size;
		private Side[] _orientation;


		/// <summary>
		/// Возвращает цвет данной стороны
		/// </summary>
		/// <param name="side"></param>
		/// <returns></returns>
		public Color GetSideColor(Side side)
		{
			return _sidesColors[(int)side];
		}

		public Parallelepiped(UVector3D size, Color[] sidesColors)
		{
			_size = size;
			_sidesColors = sidesColors;
			_coloredSides = Side.None.MakeArray(6);
			_orientation = new Side[6];
			for ( int i = 0; i < 6; i++)
			{
				Color color = sidesColors[i];
				if ((int)color >= 0)
					_coloredSides[(int)color] = (Side)i;
				_orientation[i] = (Side)i; // Изначально ориентация совпадает с индексом
			}
		}

		/// <summary>
		/// Вектор исходных размеров (не меняется при повороте)
		/// </summary>
		public UVector3D Size
		{
			get { return _size; }
		}

		/// <summary>
		/// Возвращает цвета сторон, образующих данное ребро
		/// </summary>
		/// <param name="edge"></param>
		/// <returns></returns>
		public EdgeColors GetEdgeColors(Edge edge)
		{
			return new EdgeColors(_sidesColors[(int)edge.Side1], _sidesColors[(int)edge.Side2]);
		}

		/// <summary>
		/// Возвращает цвета сторон, образующих данный угол
		/// </summary>
		/// <param name="corner"></param>
		/// <returns></returns>
		public CornerColors GetCornerColors(Corner corner)
		{
			return new CornerColors(
				_sidesColors[(int)corner.Side1], _sidesColors[(int)corner.Side2], _sidesColors[(int)corner.Side3]
				);
		}

		/// <summary>
		/// Проверяет, есть ли в параллелепипеде цветной угол, если нет - возвращает null, иначе - любой подходящий угол
		/// </summary>
		/// <returns></returns>
		public Corner AnyColoredCorner()
		{
			foreach (var corner in ParallelepipedUtils.Corners)
			{
				if (!GetCornerColors(corner).Contains(Color.None))
					return corner;
			}
			return null;
		}

		/// <summary>
		/// Проверяет, есть ли в параллелепипеде цветное ребро, если нет - возвращает null, иначе - любое подходящее ребро
		/// </summary>
		/// <returns></returns>
		public Edge AnyColoredEdge()
		{
			foreach (var edge in ParallelepipedUtils.Edges)
			{
				if (!GetEdgeColors(edge).Contains(Color.None))
					return edge;
			}
			return null;
		}

		/// <summary>
		/// Возвращает любую цветную сторону
		/// </summary>
		/// <returns></returns>
		public Side AnyColoredSide()
		{
			for (int i=0; i<6; i++)
			{
				if (_sidesColors[i] != Color.None)
				{
					return (Side)i;
				}
			}
			return Side.None;
		}

		/// <summary>
		/// Возвращает длину данного исходного ребра
		/// </summary>
		/// <param name="edge"></param>
		/// <returns></returns>
		private uint GetEdgeLength(Edge edge)
		{
			if (edge.IsParallelToOx())
				return _size.X;
			if (edge.IsParallelToOy())
				return _size.Y;
			return _size.Z;
		}

		/// <summary>
		/// Возвращает размеры данного ребра
		/// </summary>
		/// <param name="edge"></param>
		/// <returns></returns>
		public OneValueDimensions GetEdgeDimensions(Edge edge)
		{
			return new OneValueDimensions(GetEdgeLength(edge));
		}

		/// <summary>
		/// Возвращает размеры данной грани
		/// </summary>
		/// <param name="side"></param>
		/// <returns></returns>
		public TwoValuesDimensions GetSideDimensions(Side side)
		{
			switch (side)
			{
				case Side.Up:
				case Side.Down:
					return new TwoValuesDimensions(_size.X, _size.Y);
				case Side.Left:
				case Side.Right:
					return new TwoValuesDimensions(_size.Z, _size.Y);
				case Side.Front:
				case Side.Back:
					return new TwoValuesDimensions(_size.Z, _size.X);
				default:
					return null;
			}
		}

		/// <summary>
		/// Возвращает размеры данного параллелепипеда
		/// </summary>
		/// <returns></returns>
		public ThreeValuesDimensions GetDimensions()
		{
			return new ThreeValuesDimensions(_size.X, _size.Y, _size.Z);
		}

		/// <summary>
		/// Возвращает сторону, имеющую данный цвет
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public Side GetSideByColor(Color color)
		{
			return _coloredSides[(int)color];
		}

		/// <summary>
		/// Задает новую ориентацию массивом
		/// (если, например, _orientation[Side.Bottom] == Side.Left, это значит, что при такой ориентации
		/// снизу будет находиться изначально левая сторона)
		/// </summary>
		/// <param name="orientation"></param>
		public void SetOrientation(Side[] orientation)
		{
			_orientation = orientation;
		}

		/// <summary>
		/// Задает новую ориентацию информацией о новом положении двух смежных сторон
		/// </summary>
		/// <param name="s1"> Желаемое место первой стороны </param>
		/// <param name="is1"> Сама первая сторона </param>
		/// <param name="s2"> Желаемое место второй стороны (вторая смежна первой) </param>
		/// <param name="is2"> Вторая сторона </param>
		public void SetOrientation(Side s1, Side is1, Side s2, Side is2)
		{
			SetOrientation(ParallelepipedUtils.MakeOrientation(s1, is1, s2, is2));
		}

		/// <summary>
		/// Задает новую ориентацию по двум новым сторонам и функции, говорящей по
		/// новой стороне ту исходную, которая должна стоять на ее месте
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <param name="func"></param>
		public void SetOrientation(Side s1, Side s2, Func<Side, Side> func)
		{
			SetOrientation(s1, func(s1), s2, func(s2));
		}

		/// <summary>
		/// Ориентирует так, чтобы цвета двух новых сторон совпали с цветами этих сторон у данного
		/// параллелепипеда
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <param name="p"></param>
		public void SetOrientation(Side s1, Side s2, Parallelepiped p)
		{
			SetOrientation(s1, s2, side => GetSideByColor(p.GetSideColor(side)));
		}

		/// <summary>
		/// Возвращает изначальную сторону, стоящую на данном месте при текущей ориентации
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public Side GetInitialSideOn(Side s)
		{
			return _orientation[(int)s];
		}

		/// <summary>
		/// Считает свои цвета в счетчике, прибавляя к соответствующему элементу
		/// счетчика количество цветов (может быть не более одной стороны некоторого цвета)
		/// </summary>
		/// <param name="counter"></param>
		public void Count(uint[] counter)
		{
			foreach (var color in _sidesColors)
			{
				if ((int)color >= 0)
					counter[(int)color]++;
			}
		}

		/// <summary>
		/// Возвращает расстояние между левой и правой гранями с учетом ориентации
		/// </summary>
		/// <returns></returns>
		public uint GetCurrentX()
		{
			return GetEdgeLength(new Edge(GetInitialSideOn(Side.Down), GetInitialSideOn(Side.Front)));
		}

		/// <summary>
		/// Возвращает расстояние между передней и задней гранями с учетом ориентации
		/// </summary>
		/// <returns></returns>
		public uint GetCurrentY()
		{
			var edge = new Edge(GetInitialSideOn(Side.Down), GetInitialSideOn(Side.Left));
			return GetEdgeLength(edge);
		}

		/// <summary>
		/// Возвращает расстояние между нижней и верхней гранями с учетом ориентации
		/// </summary>
		/// <returns></returns>
		public uint GetCurrentZ()
		{
			return GetEdgeLength(new Edge(GetInitialSideOn(Side.Left), GetInitialSideOn(Side.Front)));
		}
	}

}   

namespace ParallelepipedCutting
{
	using EdgeDatas = Dictionary<EdgeColors, Dictionary<OneValueDimensions, LinkedList<Parallelepiped>>>;
	using CornerDatas = List<Parallelepiped>;
	using UncoloredDatas = Dictionary<ThreeValuesDimensions, LinkedList<Parallelepiped>>;
	using SideDatas = Dictionary<Color, Dictionary<TwoValuesDimensions, LinkedList<Parallelepiped>>>;
	using ParallelepipedCutting.Collections.ColorCollections;

	/// <summary>
	/// Разделяет параллелепипеды на лежащие на гранях, сторонах, углах, внутри и предоставляет
	/// доступ к соответствующим коллекциям
	/// </summary>
	class ParallelepipedDistributor
	{
		SideDatas _sideDatas;
		EdgeDatas _edgeDatas;
		CornerDatas _cornerDatas;
		UncoloredDatas _uncoloredDatas;

		public ParallelepipedDistributor()
		{
			_sideDatas = new SideDatas();
			_edgeDatas = new EdgeDatas();
			_cornerDatas = new CornerDatas();
			_uncoloredDatas = new UncoloredDatas(); 
		}

		public EdgeDatas EdgeDatas
		{
			get { return _edgeDatas; }
		}

		public SideDatas SideDatas
		{
			get { return _sideDatas; }
		}

		public CornerDatas CornerDatas
		{
			get { return _cornerDatas; }
		}

		public UncoloredDatas UncoloredDatas
		{
			get { return _uncoloredDatas; }
		}

		/// <summary>
		/// Распределяет данный параллелепипед
		/// </summary>
		/// <param name="p"></param>
		public void Handle(Parallelepiped p)
		{
			var side = p.AnyColoredSide();
			if (side == Side.None)
			{
				// Внутренний
				_uncoloredDatas.MultiInsert(p.GetDimensions(), p);
			}
			else
			{
				var corner = p.AnyColoredCorner();
				if (corner == null)
				{
					var edge = p.AnyColoredEdge();
					if (edge == null)
					{
						// На грани
						Color sideColor = p.GetSideColor(side);
						var dict = _sideDatas.TakeInsertedDictionary(sideColor);
						dict.MultiInsert(p.GetSideDimensions(side), p);
					}
					else
					{
						// На ребре
						EdgeColors edgeColors = p.GetEdgeColors(edge);
						var dict = _edgeDatas.TakeInsertedDictionary(edgeColors);
						dict.MultiInsert(p.GetEdgeDimensions(edge), p);
					}
				}
				else
				{
					// Угловой
					_cornerDatas.Add(p);
				}
			}
		}
	}
}

namespace ParallelepipedCutting
{
	/// <summary>
	/// Содержит функции и методы расширения, знающие о логике расположения граней,
	/// сторон и углов в параллелепипедах
	/// </summary>
	static class ParallelepipedUtils
	{
		private static readonly Side[][] _adjacent = new Side[][]
		{
			new Side[] {Side.Down, Side.Up, Side.Left, Side.Right }, // Смежные с Side.Front и Side.Back
			new Side[] {Side.Front, Side.Back, Side.Left, Side.Right }, // Смежные с Side.Down и  Side.Up
			new Side[] {Side.Front, Side.Back, Side.Down, Side.Up }, // Смежные с Side.Left и  Side.Right
		};

		/// <summary>
		/// Три стороны, соответствующие координатным плоскостям
		/// </summary>
		public static readonly Side[] MainSides = new[] { Side.Down, Side.Front, Side.Left };
		
		/// <summary>
		/// Все ребра
		/// </summary>
		public static readonly Edge[] Edges = new Edge[]
		{
			new Edge(Side.Front, Side.Down), new Edge(Side.Front, Side.Up),
			new Edge(Side.Front, Side.Left), new Edge(Side.Front, Side.Right),
			new Edge(Side.Back, Side.Down), new Edge(Side.Back, Side.Up),
			new Edge(Side.Back, Side.Left), new Edge(Side.Back, Side.Right),
			new Edge(Side.Left, Side.Down), new Edge(Side.Up, Side.Left),
			new Edge(Side.Right, Side.Up), new Edge(Side.Down, Side.Right),
		};

		/// <summary>
		/// Все углы
		/// </summary>
		public static readonly Corner[] Corners = new Corner[]
		{
			new Corner(Side.Up, Side.Left, Side.Front), new Corner(Side.Up, Side.Right, Side.Front),
			new Corner(Side.Down, Side.Left, Side.Front), new Corner(Side.Down, Side.Right, Side.Front),
			new Corner(Side.Up, Side.Left, Side.Back), new Corner(Side.Up, Side.Right, Side.Back),
			new Corner(Side.Down, Side.Left, Side.Back), new Corner(Side.Down, Side.Right, Side.Back),
		};

		private static readonly Edge[] _oxEdges = new[] { Edges[0], Edges[1], Edges[4], Edges[5] };
		private static readonly Edge[] _oyEdges = new[] { Edges[8], Edges[9], Edges[10], Edges[11] };
		private static readonly Edge[] _ozEdges = new[] { Edges[2], Edges[3], Edges[6], Edges[7] };

		private static readonly Vector3D<int>[] _sideToVector;
		private static readonly Dictionary<Vector3D<int>, Side> _vectorToSide;

		/// <summary>
		/// Возвращает массив сторон, смежных данной
		/// </summary>
		/// <param name="side"></param>
		/// <returns></returns>
		public static Side[] GetAdjacent(this Side side)
		{
			return _adjacent[(int)side / 2];
		}

		/// <summary>
		/// Говорит, смежны ли две стороны
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <returns></returns>
		static public bool IsAdjacent(this Side s1, Side s2)
		{
			/*
			 * Стороны перечислены так, что пары несмежных сторон идут
			 * друг за другом, а все остальные смежны
			 */
			if (s1 == Side.None)
				return false;
			int val1 = (int)s1;
			int val2 = (int)s2;
			if (val1 == val2)
				return true;
			return (val1 * 2) != (val2 * 2);
		}

		/// <summary>
		/// Возвращает противоположную сторону
		/// </summary>
		/// <param name="side"></param>
		/// <returns></returns>
		public static Side GetOpposite(this Side side)
		{
			int sideNum = (int)side;
			if (sideNum % 2 == 1)
				return (Side)(sideNum - 1);
			else
				return (Side)(sideNum + 1);
		}

		/// <summary>
		/// Устанавливает соответствие между сторонами и их нормальными векторами
		/// </summary>
		static ParallelepipedUtils()
		{
			_vectorToSide = new Dictionary<Vector3D<int>, Side>();
			_sideToVector = new Vector3D<int>[6];
			AddToVectors(Side.Front, new Vector3D<int>(0, -1, 0));
			AddToVectors(Side.Back, new Vector3D<int>(0, 1, 0));
			AddToVectors(Side.Down, new Vector3D<int>(0, 0, -1));
			AddToVectors(Side.Up, new Vector3D<int>(0, 0, 1));
			AddToVectors(Side.Left, new Vector3D<int>(-1, 0, 0));
			AddToVectors(Side.Right, new Vector3D<int>(1, 0, 0));
		}

		/// <summary>
		/// Добавляет соответствие стороны и нормального вектора
		/// </summary>
		/// <param name="s"></param>
		/// <param name="v"></param>
		private static void AddToVectors(Side s, Vector3D<int> v)
		{
			_sideToVector[(int)s] = v;
			_vectorToSide[v] = s;
		}

		/// <summary>
		/// Возвращает сторону, нормальный вектор которой составляет правую тройку
		/// с нормалями двух данный сторон
		/// (это нужно для задания ориентации)
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <returns></returns>
		private static Side GetRightTriple(this Side s1, Side s2)
		{
			Vector3D<int> v1 = _sideToVector[(int)s1];
			Vector3D<int> v2 = _sideToVector[(int)s2];
			return _vectorToSide[new Vector3D<int>(
				v1.Y * v2.Z - v2.Y * v1.Z,
				-v1.X * v2.Z + v1.Z * v2.X,
				v1.X * v2.Y - v1.Y * v2.X
				)];
		}

		/// <summary>
		/// Создает новую ориентацию по новому положению двух смежных сторон
		/// </summary>
		/// <param name="on1">На этом месте...</param>
		/// <param name="is1">...стоит эта сторона</param>
		/// <param name="on2">А на том...</param>
		/// <param name="is2">... - та</param>
		/// <returns></returns>
		public static Side[] MakeOrientation(Side on1, Side is1, Side on2, Side is2)
		{
			// На стороне on1 будет находиться изначальная сторона is1
			var orientation = new Side[6];
			orientation[(int)on1] = is1;
			orientation[(int)on2] = is2;

			orientation[(int)on1.GetOpposite()] = is1.GetOpposite();
			orientation[(int)on2.GetOpposite()] = is2.GetOpposite();

			Side rightTripleOn = on1.GetRightTriple(on2);
			Side rightTripleIs = is1.GetRightTriple(is2);

			orientation[(int)rightTripleOn] = rightTripleIs;
			orientation[(int)rightTripleOn.GetOpposite()] = rightTripleIs.GetOpposite();

			return orientation;
		}

		/// <summary>
		/// Говорит, параллельна ли данное ребро Ox
		/// </summary>
		public static bool IsParallelToOx(this Edge edge)
		{
			return !edge.Contains(Side.Left) && !edge.Contains(Side.Right);
		}

		/// <summary>
		/// Говорит, параллельна ли данное ребро Oy
		/// </summary>
		public static bool IsParallelToOy(this Edge edge)
		{
			return !edge.Contains(Side.Front) && !edge.Contains(Side.Back);
		}

		/// <summary>
		/// Говорит, параллельна ли данное ребро Oz
		/// </summary>
		public static bool IsParallelToOz(this Edge edge)
		{
			return !edge.Contains(Side.Up) && !edge.Contains(Side.Down);
		}

		/// <summary>
		/// Возвращает массив ребер, параллельных данному
		/// </summary>
		public static Edge[] GetParallel(this Edge edge)
		{
			if (edge.IsParallelToOx())
			{
				return _oxEdges;
			}
			if (edge.IsParallelToOy())
			{
				return _oyEdges;
			}
			if (edge.IsParallelToOz())
			{
				return _ozEdges;
			}
			return new Edge[0];
		}
	}
}

namespace ParallelepipedCutting
{
	class Program
	{

		public static void Main()
		{
			var model = ParallelepipedParser.Parse(Console.ReadLine());
			var pieces = new Parallelepiped[int.Parse(Console.ReadLine())];
			var pd = new ParallelepipedDistributor();
			uint[] counter = new uint[6];

			for (int i = 0; i < pieces.Length; i++)
			{
				pieces[i] = ParallelepipedParser.Parse(Console.ReadLine());
				pd.Handle(pieces[i]);
				pieces[i].Count(counter);
			}
			
			var rubick = new RubicksParallelepiped(pd, counter, model);
			rubick.PlacePieces();
			var formatter = new RubicksParallelepipedFormatter(rubick);
			for (int i = 0; i < pieces.Length; i++)
			{
				Console.WriteLine(formatter.FormatInformation(pieces[i]));
			}

		}
	} 
}

namespace ParallelepipedCutting
{
	/// <summary>
	/// Реализует логику расстановки параллелепипедов по местам
	/// </summary>
	class RubicksParallelepiped
	{
		private Parallelepiped _model;
		
		/* Нужно быстро уметь отвечать на запросы о том, где находится
		 * заданный маленький параллелепипед в большом, и о том, что находится
		 * на позиции (i,j,k) в сетке разрезов исходного параллелепипеда
		 */
		private Dictionary<Parallelepiped, UVector3D> _indexesDictionary;
		private Parallelepiped[,,] _parallelepipedArray;
		private ParallelepipedDistributor _distributor;
		private uint[] _oxCuts;
		private uint[] _oyCuts;
		private uint[] _ozCuts;

		/// <summary>
		/// Вычисляет размеры массива частей и создает его
		/// </summary>
		/// <param name="colorCounter"> Счетчик цветов </param>
		private void FormArrayFromCounter(uint[] colorCounter)
		{
			/* Вычислить длины граней по площадям сторон
			 * (x,y,z) - длины сторон, (a,b,c) - площади
			 * xy = a
			 * xz = b
			 * yz = c
			 * xyz = sqrt abc
			 * x = sqrt abc / c
			 * y = sqrt abc / b
			 * z = sqrt abc / a
			 */

			uint a = colorCounter[(int)_model.GetSideColor(Side.Down)],
				b = colorCounter[(int)_model.GetSideColor(Side.Front)],
				c = colorCounter[(int)_model.GetSideColor(Side.Left)];
			uint xyz = (uint)Math.Sqrt(a * b * c);
			uint x = xyz / c,
				y = xyz / b,
				z = xyz / a;
			_parallelepipedArray = new Parallelepiped[x, y, z];
		}

		public RubicksParallelepiped(ParallelepipedDistributor pd, uint[] colorCounter,
			Parallelepiped model)
		{
			_distributor = pd;
			_model = model;
			_indexesDictionary = new Dictionary<Parallelepiped,UVector3D>();
			FormArrayFromCounter(colorCounter);
		}

		/// <summary>
		/// Складывает данный параллелепипед по данному индексу
		/// </summary>
		/// <param name="cp"></param>
		/// <param name="position"></param>
		private void PutParallelepiped(Parallelepiped cp, UVector3D position)
		{
			_indexesDictionary[cp] = position;
			_parallelepipedArray[position.X, position.Y, position.Z] = cp;
		}

		/// <summary>
		/// Получает координаты данного параллелепипеда (если их нет, выбрасывается исключение)
		/// </summary>
		/// <param name="cp"></param>
		/// <returns></returns>
		private UVector3D GetGridCoordinates(Parallelepiped cp)
		{
			return _indexesDictionary[cp];
		}

		/// <summary>
		/// Получает параллелепипед по координатам или null, если там ничего нет
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		private Parallelepiped GetParallelepiped(UVector3D pos)
		{
			return _parallelepipedArray[pos.X, pos.Y, pos.Z];
		}

		/// <summary>
		/// Расставляет угловые куски
		/// </summary>
		private void PlaceCorners()
		{
			foreach (var corner in ParallelepipedUtils.Corners)
			{
				var position = GetCornerPosition(corner);
				if (GetParallelepiped(position) == null)
				{
					var cornerColors = _model.GetCornerColors(corner);
					var cornerPiece = _distributor.CornerDatas.Where(
						p => p.GetSideByColor(cornerColors.Color1) != Side.None &&
							p.GetSideByColor(cornerColors.Color2) != Side.None &&
							p.GetSideByColor(cornerColors.Color3) != Side.None).First();
					cornerPiece.SetOrientation(corner.Side1, corner.Side2, _model);
					PutParallelepiped(cornerPiece, position);
				}
			}
			_distributor.CornerDatas.Clear();
		}

		/// <summary>
		/// Расставляет куски на ребрах
		/// </summary>
		private void PlaceEdges()
		{
			foreach (var edge in ParallelepipedUtils.Edges)
			{
				// Все кубики с данного ребра
				var edgeColors = _model.GetEdgeColors(edge);
				if (!_distributor.EdgeDatas.ContainsKey(edgeColors))
					continue;
				
				var edgePieces = _distributor.EdgeDatas[edgeColors];
				for (uint layer = 1; edgePieces.Count != 0; layer++)
				{
					var position = GetEdgePosition(edge, layer);

					if (GetParallelepiped(position) == null)
					{
						// Выбираем любой кубик на ребре и удаляем его из списка
						var singleEdgeDimension = edgePieces.Keys.First();
						var singleEdgePiece = edgePieces.PopFromMultiDictionary(singleEdgeDimension);

						// Ориентируем
						singleEdgePiece.SetOrientation(edge.Side1, edge.Side2, _model);

						// Складываем его на место
						PutParallelepiped(singleEdgePiece, position);

						// Во всех остальных ребрах, параллельных этому						
						foreach (var othEdge in edge.GetParallel())
						{
							var othPosition = GetEdgePosition(othEdge, layer);
							if (GetParallelepiped(othPosition) == null)
							{
								var othEdgeColors = _model.GetEdgeColors(othEdge);
								if (!_distributor.EdgeDatas.ContainsKey(othEdgeColors))
									continue;

								var othEdgePieces = _distributor.EdgeDatas[_model.GetEdgeColors(othEdge)];

								if (!othEdgePieces.ContainsKey(singleEdgeDimension))
									continue;
								// Находим такой же по размеру
								var othEdgePiece = othEdgePieces.PopFromMultiDictionary(singleEdgeDimension);

								// Его тоже ориентируем и складываем на место (в том же слое)
								othEdgePiece.SetOrientation(othEdge.Side1, othEdge.Side2, _model);
								PutParallelepiped(othEdgePiece, othPosition);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Расставляет все куски
		/// </summary>
		public void PlacePieces()
		{
			PlaceCorners();
			PlaceEdges();
			PlaceSides();
			PlaceUncolored();
			DetermineCutsCooordinates();
		}

		/// <summary>
		/// Расставляет куски на гранях
		/// </summary>
		private void PlaceSides()
		{
			for (uint i = 0; i < _parallelepipedArray.GetLength(0); i++)
 				for (uint j = 0; j < _parallelepipedArray.GetLength(1); j++)
					for (uint k = 0; k < _parallelepipedArray.GetLength(2); k++)
					{
						var position = new UVector3D(i, j, k);
						if (GetParallelepiped(position) == null)
						{
							var sidesToSearch = GetSides(position);
							if (sidesToSearch.Count != 0)		// Если на грани
								PlaceSidePiece(position, sidesToSearch);
						}
					}

		}

		/// <summary>
		/// Расставляет внутренние куски
		/// </summary>
		private void PlaceUncolored()
		{
			for (uint i = 1; i < _parallelepipedArray.GetLength(0) - 1; i++)
 				for (uint j = 1; j < _parallelepipedArray.GetLength(1) - 1; j++)
					for (uint k = 1; k < _parallelepipedArray.GetLength(2) - 1; k++)
					{
						var position = new UVector3D(i, j, k);
						if (GetParallelepiped(position) == null)
						{
							PlaceUncoloredPiece(position);
						}
					}
		}

		/// <summary>
		/// Подбирает и ставит внутренний кусок на указанную позицию
		/// </summary>
		/// <param name="position"></param>
		private void PlaceUncoloredPiece(UVector3D position)
		{
			Side s1, s2;
			TwoValuesDimensions dim1, dim2;
			ThreeValuesDimensions dim;
			GetUncoloredRestrictions(position, out s1, out dim1, out s2, out dim2, out dim);

			var piece = _distributor.UncoloredDatas.PopFromMultiDictionary(dim);

			var is1 = ParallelepipedUtils.MainSides.Where(
						s => piece.GetSideDimensions(s).Equals(dim1)).First();
			var is2 = is1.GetAdjacent().Where(
						s => piece.GetSideDimensions(s).Equals(dim2)).First();
			piece.SetOrientation(s1, is1, s2, is2);
			PutParallelepiped(piece, position);
		}

		/// <summary>
		/// Возвращает требования ко внутреннему куску, находящемуся на
		/// данной позиции
		/// </summary>
		/// <param name="position"></param>
		/// <param name="s1"> Первая сторона </param>
		/// <param name="dim1"> Размеры первой стороны </param>
		/// <param name="s2"> Вторая сторона </param>
		/// <param name="dim2"> Размеры второй стороны </param>
		/// <param name="threeDim"> Общие размеры куска </param>
		private void GetUncoloredRestrictions(
			UVector3D position, out Side s1, out TwoValuesDimensions dim1,
			out Side s2, out TwoValuesDimensions dim2, out ThreeValuesDimensions threeDim)
		{
			var xPiece = GetParallelepiped(new UVector3D(0, position.Y, position.Z));
			var zPiece = GetParallelepiped(new UVector3D(position.X, position.Y, 0));
			uint x = zPiece.GetCurrentX(), y = xPiece.GetCurrentY(), z = xPiece.GetCurrentZ();
			threeDim = new ThreeValuesDimensions(x, y, z);
			s1 = Side.Front;
			s2 = Side.Down;
			dim1 = new TwoValuesDimensions(x, z);
			dim2 = new TwoValuesDimensions(x, y);
		}

		/// <summary>
		/// Подбирает кусок на грани из данной коллекции сторон и складывает его
		/// на данную позицию
		/// </summary>
		/// <param name="position"></param>
		/// <param name="sidesToSearch"></param>
		private void PlaceSidePiece(UVector3D position, IEnumerable<Side> sidesToSearch)
		{
			foreach (var side in sidesToSearch)
			{
				var color = _model.GetSideColor(side);
				TwoValuesDimensions dimensions, adjDim;
				Side adjSide;
				GetSidePieceRestrictions(position, side, out dimensions, out adjSide, out adjDim);
				if (_distributor.SideDatas.ContainsKey(color) &&
					_distributor.SideDatas[color].ContainsKey(dimensions))
				{ 
					var piece = _distributor.SideDatas[color].PopFromMultiDictionary(dimensions);
					var pieceSide = piece.GetSideByColor(color);
					var pieceAdjSide = pieceSide.GetAdjacent().Where(
						s => piece.GetSideDimensions(s).Equals(adjDim)).First();
					piece.SetOrientation(side, pieceSide, adjSide, pieceAdjSide);
					PutParallelepiped(piece, position);
					return;
				}
			}

		}

		/// <summary>
		/// Выдает требования к куску на грани
		/// </summary>
		/// <param name="position"> Позиция на грани </param>
		/// <param name="side"> Сама грань </param>
		/// <param name="dim"> Размерность стороны куска, лежащей на грани </param>
		/// <param name="adjSide"> Смежная грань </param>
		/// <param name="adjDim"> Размерность смежной грани у куска </param>
		private void GetSidePieceRestrictions(
			UVector3D position, Side side, out TwoValuesDimensions dim,
			out Side adjSide, out TwoValuesDimensions adjDim
			)
		{
			// "Координатные" куски на осях
			var xPiece = GetParallelepiped(new UVector3D(0, position.Y, position.Z));
			var yPiece = GetParallelepiped(new UVector3D(position.X, 0, position.Z));
			var zPiece = GetParallelepiped(new UVector3D(position.X, position.Y, 0));

			if (side != Side.Up && side != Side.Down)
			{
				adjSide = Side.Up;
				adjDim = zPiece.GetSideDimensions(zPiece.GetInitialSideOn(Side.Up));
			}
			else
			{
				adjSide = Side.Front;
				adjDim = yPiece.GetSideDimensions(yPiece.GetInitialSideOn(Side.Front));
			}

			if (side == Side.Left || side == Side.Right)
				dim = new TwoValuesDimensions(yPiece.GetCurrentZ(), zPiece.GetCurrentY());
			else if (side == Side.Front || side == Side.Back)
				dim = new TwoValuesDimensions(xPiece.GetCurrentZ(), zPiece.GetCurrentX());
			else
				dim = new TwoValuesDimensions(xPiece.GetCurrentY(), yPiece.GetCurrentX());
		}

		/// <summary>
		/// Возвращает список сторон, на которых должен лежать кубик с данной позицией
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		private List<Side> GetSides(UVector3D position)
		{
			var list = new List<Side>();
			if (position.X == 0)
				list.Add(Side.Left);
			if (position.X == _parallelepipedArray.GetLength(0) - 1)
				list.Add(Side.Right);
			if (position.Y == 0)
				list.Add(Side.Front);
			if (position.Y == _parallelepipedArray.GetLength(1) - 1)
				list.Add(Side.Back);
			if (position.Z == 0)
				list.Add(Side.Down);
			if (position.Z == _parallelepipedArray.GetLength(2) - 1)
				list.Add(Side.Up);
			return list;
		}

		/// <summary>
		/// Возвращает X-координату кусков на данной стороне, если
		/// для всех кусков на этой стороне она (координата) постоянна,
		/// иначе возвращает -1
		/// </summary>
		/// <param name="side"></param>
		/// <returns></returns>
		private int GetX(Side side)
		{
			switch (side)
			{
				case Side.Left:
					return 0;
				case Side.Right:
					return _parallelepipedArray.GetLength(0) - 1;
				default:
					return -1;
			}
		}

		/// <summary>
		/// Возвращает Y-координату кусков на данной стороне, если
		/// для всех кусков на этой стороне она (координата) постоянна,
		/// иначе возвращает -1
		/// </summary>
		/// <param name="side"></param>
		/// <returns></returns>
		private int GetY(Side side)
		{
			switch (side)
			{
				case Side.Front:
					return 0;
				case Side.Back:
					return _parallelepipedArray.GetLength(1) - 1;
				default:
					return -1;
			}
		}

		/// <summary>
		/// Возвращает Z-координату кусков на данной стороне, если
		/// для всех кусков на этой стороне она (координата) постоянна,
		/// иначе возвращает -1
		/// </summary>
		/// <param name="side"></param>
		/// <returns></returns>
		private int GetZ(Side side)
		{
			switch (side)
			{
				case Side.Up:
					return _parallelepipedArray.GetLength(2) - 1;
				case Side.Down:
					return 0;
				default:
					return -1;
			}
		}

		/// <summary>
		/// Возвращает координаты ячейки на данном ребре с данным отступом
		/// от его начала (от 0)
		/// </summary>
		/// <param name="edge"></param>
		/// <param name="layer"></param>
		/// <returns></returns>
		private UVector3D GetEdgePosition(Edge edge, uint layer)
		{
			int x = Math.Max(GetX(edge.Side1), GetX(edge.Side2));
			int y = Math.Max(GetY(edge.Side1), GetY(edge.Side2));
			int z = Math.Max(GetZ(edge.Side1), GetZ(edge.Side2));

			return new UVector3D(
				x == -1 ? layer : (uint)x,
				y == -1 ? layer : (uint)y,
				z == -1 ? layer : (uint)z
				);
		}

		/// <summary>
		/// Возвращает позицию угловой ячейки
		/// </summary>
		/// <param name="corner"></param>
		/// <returns></returns>
		private UVector3D GetCornerPosition(Corner corner)
		{
			return new UVector3D(
				corner.Contains(Side.Right) ? (uint)_parallelepipedArray.GetLength(0) - 1 : 0,
				corner.Contains(Side.Back) ? (uint)_parallelepipedArray.GetLength(1) - 1 : 0,
				corner.Contains(Side.Up) ? (uint)_parallelepipedArray.GetLength(2) - 1 : 0
				);
		}

		/// <summary>
		/// Вычисляет координаты разрезов и записывает их
		/// </summary>
		private void DetermineCutsCooordinates()
		{
			_oxCuts = new uint[_parallelepipedArray.GetLength(0)];
			_oyCuts = new uint[_parallelepipedArray.GetLength(1)];
			_ozCuts = new uint[_parallelepipedArray.GetLength(2)];
			_oxCuts[0] = _oyCuts[0] = _ozCuts[0] = 0;
			for (uint i = 1; i < _oxCuts.Length; i++)
				_oxCuts[i] = _oxCuts[i - 1] + GetParallelepiped(new UVector3D(i - 1, 0, 0)).GetCurrentX();
			for (uint i = 1; i < _oyCuts.Length; i++)
				_oyCuts[i] = _oyCuts[i - 1] + GetParallelepiped(new UVector3D(0, i - 1, 0)).GetCurrentY();
			for (uint i = 1; i < _ozCuts.Length; i++)
				_ozCuts[i] = _ozCuts[i - 1] + GetParallelepiped(new UVector3D(0, 0, i - 1)).GetCurrentZ();
		}

		/// <summary>
		/// Пытается выдать координаты в пространстве, если координаты разрезов
		/// еще не вычеслены, возвращает ложь
		/// </summary>
		/// <param name="about"></param>
		/// <param name="coordinates"></param>
		/// <returns></returns>
		public bool TryQueryRealCoordinates(Parallelepiped about, out UVector3D coordinates)
		{
			if (_oxCuts != null) {
				UVector3D pos = GetGridCoordinates(about);
				coordinates = new UVector3D(_oxCuts[pos.X], _oyCuts[pos.Y], _ozCuts[pos.Z]);
				return true;
			}
			else
			{
				coordinates = null;
				return false;
			}
		}
	}
}

namespace ParallelepipedCutting.Collections
{
	/// <summary>
	/// Реализация неизменяемого множества. Используется для построения
	///	неупорядоченных пар и троек (длин сторон)
	/// </summary>
	/// <typeparam name="T"> Хранимый тип </typeparam>
	class StaticSortedSet<T> where T : IComparable
	{
		protected readonly T[] _values;

		public StaticSortedSet(T[] values)
		{
			Array.Sort(values);
			_values = values;
		}

		public override bool Equals(System.Object obj)
		{
			if (obj == null)
			{
				return false;
			}
			var p = obj as StaticSortedSet<T>;
			if ((System.Object)p == null)
			{
				return false;
			}
			return _values.SequenceEqual(p._values);
		}

		public override int GetHashCode()
		{
			return _values.Length != 0 ? _values[0].GetHashCode() : 0;
		}

		public bool Contains(T elem)
		{
			return _values.Contains(elem);
		}

		public override string ToString()
		{
			return "{" + String.Join(", ", _values) + "}";
		}
	}
}

namespace ParallelepipedCutting.Collections
{
	/// <summary>
	/// Упорядоченный неизменяемый набор
	/// </summary>
	/// <typeparam name="T"> Тип элементов в наборе</typeparam>
	class StaticVector<T>
	{
		protected readonly T[] _values;

		public StaticVector(T[] values)
		{
			_values = values;
		}

		public bool Contains(T value)
		{
			return _values.Contains(value);
		}

		public override bool Equals(System.Object obj)
		{
			if (obj == null)
			{
				return false;
			}
			var p = obj as StaticVector<T>;
			if ((System.Object)p == null)
			{
				return false;
			}
			return _values.SequenceEqual(p._values);
		}

		public override int GetHashCode()
		{
			return _values.Length != 0 ? _values[0].GetHashCode() : 0;
		}


		public override string ToString()
		{
			return "(" + String.Join(", ", _values) + ")";
		}
	}
}

namespace ParallelepipedCutting.Collections
{
	/// <summary>
	/// Набор из трех положительных чисел (представляет
	/// размеры, индексы в трехмерном масиве и т.п.)
	/// </summary>
	class UVector3D : Vector3D<uint>
	{
		public UVector3D(uint x, uint y, uint z) :
			base(x, y, z)
		{
		}
	}
}

namespace ParallelepipedCutting.Collections
{
	/// <summary>
	/// Упорядоченный набор из трех однотипных элементов
	/// </summary>
	/// <typeparam name="T"> Тип элементов </typeparam>
	class Vector3D<T> : StaticVector<T>
	{
		public T X
		{
			get { return _values[0]; }
		}


		public T Y
		{
			get { return _values[1]; }
		}


		public T Z
		{
			get { return _values[2]; }
		}


		public Vector3D(T x, T y, T z) :
			base(new[] { x, y, z })
		{
		}
	}
}

namespace ParallelepipedCutting.Collections.ColorCollections
{
	/// <summary>
	/// Цвета граней, образующих угол
	/// </summary>
	class CornerColors : StaticSortedSet<Color>
	{
		public CornerColors(Color c1, Color c2, Color c3) :
			base(new Color[] { c1, c2, c3 })
		{
		}
		public Color Color1 { get { return _values[0]; } }
		public Color Color2 { get { return _values[1]; } }
		public Color Color3 { get { return _values[2]; } }
	}

}

namespace ParallelepipedCutting.Collections.ColorCollections
{
	/// <summary>
	/// Цвета граней, образующих ребро
	/// </summary>
	class EdgeColors : StaticSortedSet<Color>
	{
		public EdgeColors(Color c1, Color c2) :
			base(new Color[] { c1, c2 })
		{
		}
		public Color Color1 { get { return _values[0]; } }
		public Color Color2 { get { return _values[1]; } }
	}

}

namespace ParallelepipedCutting.Dimensions
{
	/// <summary>
	/// Хранит неупорядоченные длины сторон.
	/// Служит базовым классом для ключей для доступа к частям
	/// параллелепипеда, заменяет Equals() и GetHashCode()
	/// </summary>
	class ArrayDimensions : StaticSortedSet<uint>
	{
		public ArrayDimensions(uint[] dims) :
			base(dims)
		{
		}

		public override int GetHashCode()
		{
			int x = 0, k = 0;
			foreach (int elem in _values)
			{
				x ^= elem << k;
				k = (k + 3) & 63;
			}
			return x;
		}

		/// <summary>
		/// Для удобства отладки перегружен и он
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "{" + String.Join(", ", _values) + "}";
		}
	}
}

namespace ParallelepipedCutting.Dimensions
{
	/// <summary>
	/// Размер одного ребра (используется для хранения ребер)
	/// </summary>
	class OneValueDimensions : ArrayDimensions
	{
		public OneValueDimensions(uint val1)
			: base(new uint[] { val1 }) { }
	}
}

namespace ParallelepipedCutting.Dimensions
{
	/// <summary>
	/// Размеры трех ребер (используется для хранения внутренних частей)
	/// </summary>
	class ThreeValuesDimensions : ArrayDimensions
	{
		public ThreeValuesDimensions(uint val1, uint val2, uint val3)
			: base(new uint[] { val1, val2, val3 }) { }
	}
}

namespace ParallelepipedCutting.Dimensions
{
	/// <summary>
	/// Размеры двух ребер (используется для хранения граней)
	/// </summary>
	class TwoValuesDimensions : ArrayDimensions
	{
		public TwoValuesDimensions(uint val1, uint val2)
			: base(new uint[] { val1, val2 }) { }
	}
}

namespace ParallelepipedCutting.Collections.SideCollections
{

	/// <summary>
	/// Содержит набор граней, образующих угол
	/// </summary>
	class Corner : StaticSortedSet<Side>
	{
		public Corner(Side s1, Side s2, Side s3) :
			base(new Side[] { s1, s2, s3 })
		{
			if (!s1.IsAdjacent(s2) || !s2.IsAdjacent(s3) || !s3.IsAdjacent(s1))
				throw new Exception("Not adjacent sides in Corner constructor");
		}
		public Side Side1
		{
			get { return _values[0]; }
		}
		public Side Side2
		{
			get { return _values[1]; }
		}
		public Side Side3
		{
			get { return _values[2]; }
		}
	}
}

namespace ParallelepipedCutting.Collections.SideCollections
{
	/// <summary>
	/// Содержит набор граней, образующих ребро
	/// </summary>
	class Edge : StaticSortedSet<Side>
	{
		public Edge(Side s1, Side s2) :
			base(new Side[] { s1, s2 })
		{
			if (!s1.IsAdjacent(s2))
				throw new Exception("Not adjacent sides in Edge constructor");
		}
		public Side Side1
		{
			get { return _values[0]; }
		}
		public Side Side2
		{
			get { return _values[1]; }
		}

	}
}

namespace ParallelepipedCutting.Enums
{
	/// <summary>
	/// Цвет (бывает никаким)
	/// </summary>
	enum Color
	{
		None = -1, Red, Olive, Yellow, Green, Blue, Violet
	}
}

namespace ParallelepipedCutting.Enums
{
	/// <summary>
	/// Сторона (бывает никакой)
	/// </summary>
	enum Side
	{
		None = -1, Front, Back, Down, Up, Left, Right
	}
}

namespace ParallelepipedCutting.Enums
{
	/// <summary>
	/// Расширение Side
	/// </summary>
	static class SideExtension
	{
		/// <summary>
		/// Возвращает массив указанной длины, заполненный данной
		/// стороной (работает быстрее Enumerator'ов)
		/// </summary>
		/// <param name="side"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static Side[] MakeArray(this Side side, int n)
		{
			var arr = new Side[n];
			for (int i = 0; i < n; i++)
			{
				arr[i] = side;
			}
			return arr;
		}
	}
}

namespace ParallelepipedCutting.Formatters
{
	class RubicksParallelepipedFormatter
	{
		private RubicksParallelepiped _rp;
		public RubicksParallelepipedFormatter(RubicksParallelepiped rp)
		{
			_rp = rp;
		}

		public string FormatInformation(Parallelepiped about)
		{
			UVector3D coords;
			if (_rp.TryQueryRealCoordinates(about, out coords))
			{
				return SideFormatter.Format(about.GetInitialSideOn(Side.Front))
					+ " " + SideFormatter.Format(about.GetInitialSideOn(Side.Down))
					+ " " + string.Join(" ", new uint[] {coords.Y, coords.Z, coords.X});
			}
			else
			{
				throw new Exception("Can't get information from Rubick");
			}
		}
	}
}

namespace ParallelepipedCutting.Formatters
{
	static class SideFormatter
	{
		public static string Format(Side s)
		{
			switch (s)
			{
				case Side.Back:
					return "B";
				case Side.Front:
					return "F";
				case Side.Down:
					return "D";
				case Side.Up:
					return "U";
				case Side.Left:
					return "L";
				case Side.Right:
					return "R";
				default:
					throw new FormatException();
			}
		}
	}
}

namespace ParallelepipedCutting
{
	/// <summary>
	/// Распознает цвет по символу
	/// </summary>
	static class ColorParser
	{
		public static Color Parse(char c)
		{
			switch (c)
			{
				case 'R':
					return Color.Red;
				case 'O':
					return Color.Olive;
				case 'Y':
					return Color.Yellow;
				case 'V':
					return Color.Violet;
				case 'G':
					return Color.Green;
				case 'B':
					return Color.Blue;
				default:
					return Color.None;
			}
		}
	}
}

namespace ParallelepipedCutting.Parsers
{
	/// <summary>
	/// Распознает параллелепипед по строке
	/// </summary>
	static class ParallelepipedParser
	{
		public static Parallelepiped Parse(string s)
		{
			string[] tokens = s.Split(' ');
			return new Parallelepiped(
				size : new Collections.UVector3D(uint.Parse(tokens[2]), uint.Parse(tokens[0]), uint.Parse(tokens[1])),
				sidesColors : tokens[3].Select<char,Color>(c => ColorParser.Parse(c)).ToArray() 
				);
		}
	}
}
