using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KonturTask
{
   static class Program
   {
      static void Main()
      {
         var output = new StreamWriter(Console.OpenStandardOutput());

         string firstLine = Console.ReadLine();
         Builder.Whole = new Parallelepiped(firstLine, true);

         string secondLine = Console.ReadLine();
         var count = int.Parse(secondLine);

         Builder.ReadAndCreatePieces(count);
         Builder.ProcessPieces();
         Builder.OutputAllPieces(output);
         output.Flush();
      }
   }

   /// <summary>
   /// Представляет собой одну из трех плоскостей куска
   /// </summary>
   class Dimension
   {
      public char[] Colors { get; private set; }
      public int Coordinate { get; set; }
      public char[] Sides { get; private set; }
      public int Size { get; private set; }

      public Dimension(char[] colors, char[] sides, int size)
      {
         Colors = colors;
         Sides = sides;
         Size = size;
         Coordinate = -1;
      }

      /// <summary>
      /// Меняет местами стороны одной плоскости
      /// </summary>
      public void SwapSides()
      {
         Utilities.Swap(ref Colors[0], ref Colors[1]);
         Utilities.Swap(ref Sides[0], ref Sides[1]);
      }
   }

   /// <summary>
   /// Класс с вспомогательнами методами
   /// </summary>
   static class Utilities
   {
      /// <summary>
      /// Генерирует следующую перестановку
      /// </summary>
      /// <param name="ar">Предыдущая перестановка</param>
      /// <param name="xcount">Максимум первого элемента</param>
      /// <param name="ycount">Максимум второго элемента</param>
      /// <param name="zcount">Максимум третьего элемента</param>
      /// <returns></returns>
      public static int[] NextPermutation(int[] ar, int xcount, int ycount, int zcount)
      {
         ar[2]++;
         for (var i = ar[0]; i < xcount - 1; i++)
         {
            for (var j = ar[1]; j < ycount - 1; j++)
            {
               int k;
               for (k = ar[2]; k < zcount - 1; )
                  return new[] { i, j, k };
               ar[2] = 0;
            }
            ar[1] = 0;
         }
         return new[] { 0, 0, 0 };
      }

      public static void Swap<T>(IList<T> list, int index1, int index2)
      {
         T tmp = list[index1];
         list[index1] = list[index2];
         list[index2] = tmp;
      }

      public static void Swap<T>(ref T first, ref T second) where T : struct
      {
         T temp = first;
         first = second;
         second = temp;
      }
   }

   /// <summary>
   /// Представляет собой кусок в форме параллелепипеда
   /// </summary>
   class Parallelepiped
   {
      char[] colors;

      public char[] Colors
      {
         get
         {
            colors[0] = Dimensions[0].Colors[0];
            colors[1] = Dimensions[0].Colors[1];
            colors[2] = Dimensions[1].Colors[0];
            colors[3] = Dimensions[1].Colors[1];
            colors[4] = Dimensions[2].Colors[0];
            colors[5] = Dimensions[2].Colors[1];
            return colors;
         }
      }
      int Depth { get { return Dimensions[0].Size; } }
      int Height { get { return Dimensions[1].Size; } }
      int Width { get { return Dimensions[2].Size; } }
      public int X { get { return Dimensions[0].Coordinate; } }
      public int Y { get { return Dimensions[1].Coordinate; } }
      public int Z { get { return Dimensions[2].Coordinate; } }

      public Dimension[] Dimensions { get; private set; }
      public int BlackSidesCount { get; private set; }
      public bool IsCorner { get; private set; }
      public List<int> UndefinedDimensions { get; private set; }

      /// <summary>
      /// Строит экземпляр класса
      /// </summary>
      /// <param name="line">Строка ввода</param>
      /// <param name="isWhole">Это прототип целого параллелепипеда?</param>
      public Parallelepiped(string line, bool isWhole = false)
      {
         Initialize(line);
         if (BlackSidesCount != 6 && !isWhole)
            Normalize();
         if (BlackSidesCount > 1)
            UndefinedDimensions = FindUndefinedDimensions();
      }

      void Initialize(string line)
      {
         string[] initArray = line.Split(' ');
         int depth = int.Parse(initArray[0]);
         int height = int.Parse(initArray[1]);
         int width = int.Parse(initArray[2]);
         colors = initArray[3].ToCharArray();

         Dimensions = new Dimension[3];
         Dimensions[0] = new Dimension(new[] { colors[0], colors[1] }, new[] { 'F', 'B' }, depth);
         Dimensions[1] = new Dimension(new[] { colors[2], colors[3] }, new[] { 'D', 'U' }, height);
         Dimensions[2] = new Dimension(new[] { colors[4], colors[5] }, new[] { 'L', 'R' }, width);

         BlackSidesCount = Colors.Count(c => c == '.');
         IsCorner = true;
      }

      /// <summary>
      /// Находит список номеров измерений с неопределенным положением
      /// </summary>
      /// <returns>Список номеров измерений</returns>
      List<int> FindUndefinedDimensions()
      {
         var result = new List<int>();
         for (int i = 0; i < 3; i++)
            if (Dimensions[i].Colors[0] == '.' && Dimensions[i].Colors[1] == '.')
            {
               result.Add(i);
               IsCorner = false;
            }
         return result;
      }

      /// <summary>
      /// Приводит кусок в правильное состояние
      /// </summary>
      void Normalize()
      {
         int indexInWhole = 0, countOfTurn = 0;
         while (indexInWhole < 6 && countOfTurn < 2)
         {
            char color = Builder.Whole.Colors[indexInWhole];
            int index = Array.IndexOf(Colors, color);
            int dimensionNumber = (index >> 1);
            int dimensionNumberInWhole = (indexInWhole >> 1);

            if (index != -1 && index != indexInWhole)
            {
               if (dimensionNumber == dimensionNumberInWhole)
                  SwapOppositeSides(dimensionNumber);
               else if (Math.Abs(indexInWhole - index) % 2 == 0)
                  Turn(dimensionNumberInWhole, dimensionNumber, dimensionNumberInWhole);
               else
                  Turn(dimensionNumber, dimensionNumber, dimensionNumberInWhole);
               indexInWhole = 0;
               countOfTurn++;
               continue;
            }
            indexInWhole++;
         }
      }

      /// <summary>
      /// Применяет к куску новое состояние и координаты
      /// </summary>
      /// <param name="statesCount">Количество всевозможных состояний куска</param>
      /// <param name="state">Новое состояние</param>
      /// <param name="newCoordinates">Новые координаты</param>
      public void ApplyNewStateAndCoordinates(int statesCount, int[] state, int[] newCoordinates)
      {
         if (!state.SequenceEqual(new[] { 0, 1, 2 }))
         {
            if (statesCount == 2)
               for (int i = 0; i < 3; i++)
                  if (i != state[i])
                  {
                     Dimensions[i].SwapSides();
                     break;
                  }
            Dimensions = new[]
            {
               Dimensions[state[0]],
               Dimensions[state[1]],
               Dimensions[state[2]]
            };
         }
         for (int k = 0; k < 3; k++)
            Dimensions[k].Coordinate = newCoordinates[k];
      }

      /// <summary>
      /// Генерирует всевозможные состояния куска
      /// </summary>
      /// <returns></returns>
      public List<int[]> GetStates()
      {
         var result = new List<int[]> { new[] { 0, 1, 2 } };

         int[] sizes = UndefinedDimensions.Select(d => Dimensions[d].Size).ToArray();

         switch (sizes.Length)
         {
            case 2:
               if (sizes[0] != sizes[1])
                  AddNewState(0, 1, result);
               break;

            case 3:
               if (Depth != Height && Depth != Width && Width != Height)
               {
                  result = new List<int[]>
                     {
                        new[] {0, 1, 2},
                        new[] {0, 2, 1},
                        new[] {1, 0, 2},
                        new[] {1, 2, 0},
                        new[] {2, 0, 1},
                        new[] {2, 1, 0}
                     };
               }
               else
               {
                  for (int i = 0; i < 2; i++)
                     for (int j = 1; j < 3; j++)
                        if (sizes[i] != sizes[j])
                           AddNewState(i, j, result);
               }
               break;
         }

         return result;
      }

      void AddNewState(int firstDimNumber, int secondDimNumber, List<int[]> result)
      {
         var ar = new[] { 0, 1, 2 };
         int temp = ar[UndefinedDimensions[firstDimNumber]];
         ar[UndefinedDimensions[firstDimNumber]] = ar[UndefinedDimensions[secondDimNumber]];
         ar[UndefinedDimensions[secondDimNumber]] = temp;
         result.Add(ar);
      }

      /// <summary>
      /// Находит координаты куска с однозначным положением
      /// </summary>
      /// <param name="cuts">Список разрезов по плоскостям</param>
      public void FindDefinedCoordinates(List<LinkedList<int>> cuts)
      {
         for (int i = 0; i < 6; i++)
         {
            if (Colors[i] != '.')
            {
               int dimNumber = (i / 2);
               if (i % 2 != 0)
               {
                  int k = (Builder.Whole.Dimensions[dimNumber].Size - Dimensions[dimNumber].Size);
                  if (!TryDefineCoordinate(cuts[dimNumber].Last, k, dimNumber, cuts[dimNumber].AddLast))
                     return;
               }
               else
               {
                  if (!TryDefineCoordinate(cuts[dimNumber].First, 0, dimNumber, cuts[dimNumber].AddFirst))
                     return;
                  i++;
               }
            }
         }
      }

      /// <summary>
      /// Пробует определить координату
      /// </summary>
      /// <param name="token">Разделитель списка разрезов</param>
      /// <param name="coordinate">Новая координата</param>
      /// <param name="dimNumber">Номер измерения</param>
      /// <param name="addFunction">Указатель на функцию, определяющий куда добавить размер куска</param>
      /// <returns></returns>
      bool TryDefineCoordinate(LinkedListNode<int> token, int coordinate, int dimNumber, Func<int, LinkedListNode<int>> addFunction)
      {
         if (token.Value == 0)
         {
            Dimensions[dimNumber].Coordinate = coordinate;
            addFunction(Dimensions[dimNumber].Size);
         }
         else if (token.Value == Dimensions[dimNumber].Size)
            Dimensions[dimNumber].Coordinate = coordinate;
         else
            return false;
         return true;
      }

      /// <summary>
      /// Находит координату с неоднозначным положением
      /// </summary>
      /// <param name="cuts">Список разрезов по плоскостям</param>
      public void FindUndefinedCoordinates(List<LinkedList<int>> cuts)
      {
         int dimNumber = UndefinedDimensions[0];
         LinkedListNode<int> current = cuts[dimNumber].First.Next;
         int currentCoordinate = cuts[dimNumber].First.Value;

         while (current != null && current.Next != null)
         {
            if (current.Value == Dimensions[dimNumber].Size)
            {
               Dimensions[dimNumber].Coordinate = currentCoordinate;
               if (!Builder.IsOccupiedPlace(new[] { X, Y, Z }))
                  return;
            }
            currentCoordinate += current.Value;
            current = current.Next;
         }

         Dimensions[dimNumber].Coordinate = currentCoordinate;
         cuts[dimNumber].AddBefore(cuts[dimNumber].Last, Dimensions[dimNumber].Size);
      }

      /// <summary>
      /// Поворачивает кусок, меняя противоположные стороны
      /// </summary>
      /// <param name="dimToTurn">Номер измерения, стороны которого нужно поменять</param>
      void SwapOppositeSides(int dimToTurn)
      {
         Dimensions[dimToTurn].SwapSides();
         for (int i = 0; i < 3; i++)
            if (i != dimToTurn
               && Dimensions[i].Colors[0] != Builder.Whole.Colors[0]
               && Dimensions[i].Colors[1] != Builder.Whole.Colors[1])
            {
               Dimensions[i].SwapSides();
               break;
            }
      }

      /// <summary>
      /// Поворачивает кусок
      /// </summary>
      /// <param name="dimToTurn">Номер измерения, у которого нужно поменять стороны местами</param>
      /// <param name="dimNum">Номер измерения в куске</param>
      /// <param name="dimNumInWhole">Номер измерения в целом параллелепипеде</param>
      void Turn(int dimToTurn, int dimNum, int dimNumInWhole)
      {
         Dimensions[dimToTurn].SwapSides();
         Utilities.Swap(Dimensions, dimNum, dimNumInWhole);
      }

      public override string ToString()
      {
         string result = String.Empty;
         if (X >= 0 && Y >= 0 && Z >= 0)
            result = String.Format("{0} {1} {2} {3} {4}", Dimensions[0].Sides[0], Dimensions[1].Sides[0], X, Y, Z);
         return result;
      }
   }

   /// <summary>
   /// Класс, собирающий целый параллелепипед из кусочков
   /// </summary>
   static class Builder
   {
      /// <summary>
      /// Хранит угловые куски
      /// </summary>
      static Dictionary<string, List<Parallelepiped>> _corners;
      /// <summary>
      /// Хранит разрезы
      /// </summary>
      static readonly List<LinkedList<int>> Cuts;
      /// <summary>
      /// Хранит куски на ребрах
      /// </summary>
      static Dictionary<string, List<Parallelepiped>> _edges;
      static Dictionary<string, int> _edgesSizes;
      /// <summary>
      /// Хранит все свободные места в параллелепипеде
      /// </summary>
      static List<Dictionary<int, List<int>>> _places;
      /// <summary>
      /// Хранит угловые куски
      /// </summary>
      static readonly Dictionary<int, Dictionary<int, HashSet<int>>> OccupiedCoords;
      /// <summary>
      /// Хранит внутренние куски
      /// </summary>
      static List<Parallelepiped> _blacks;
      /// <summary>
      /// Хранит куски, находящиеся внутри граней
      /// </summary>
      static List<Parallelepiped> _middles;
      /// <summary>
      /// Прототип целого параллелепипеда
      /// </summary>
      public static Parallelepiped Whole { get; set; }
      /// <summary>
      /// Хранит куски в порядке ввода
      /// </summary>
      static Parallelepiped[] _piecesInInputOrder;
      const int WrongCoordinate = -1;

      static Builder()
      {
         OccupiedCoords = new Dictionary<int, Dictionary<int, HashSet<int>>>();
         Cuts = new List<LinkedList<int>> { new LinkedList<int>(), new LinkedList<int>(), new LinkedList<int>() };
         for (int i = 0; i < 3; i++)
            Cuts[i].AddFirst(0);
      }

      /// <summary>
      /// Считывает новые куски
      /// </summary>
      /// <param name="count"></param>
      public static void ReadAndCreatePieces(int count)
      {
         _middles = new List<Parallelepiped>();
         _blacks = new List<Parallelepiped>();
         _corners = new Dictionary<string, List<Parallelepiped>>();
         _edges = new Dictionary<string, List<Parallelepiped>>();
         _edgesSizes = new Dictionary<string, int>();
         _piecesInInputOrder = new Parallelepiped[count];

         for (int i = 0; i < count; i++)
         {
            var newPiece = new Parallelepiped(Console.ReadLine());
            AddNewPiece(newPiece, i);
         }
      }

      /// <summary>
      /// Добавляет новый кусок в определенный контейнер
      /// </summary>
      /// <param name="piece">Новый кусок</param>
      /// <param name="index">Номер куска</param>
      static void AddNewPiece(Parallelepiped piece, int index)
      {
         _piecesInInputOrder[index] = piece;

         if (piece.IsCorner)
            AddNewCorner(piece);
         else if (piece.UndefinedDimensions.Count == 1)
            AddNewEdge(piece);
         else if (piece.UndefinedDimensions.Count == 2)
            _middles.Add(piece);
         else if (piece.BlackSidesCount == 6)
            _blacks.Add(piece);
      }

      /// <summary>
      /// Добавляет новый угловой кусок
      /// </summary>
      /// <param name="piece">Новый кусок</param>
      static void AddNewCorner(Parallelepiped piece)
      {
         var key = new string(piece.Colors);
         if (!_corners.ContainsKey(key))
            _corners.Add(key, new List<Parallelepiped>());
         _corners[key].Add(piece);
      }

      /// <summary>
      /// Добавляет новый кусок на ребре
      /// </summary>
      /// <param name="piece">Новый кусок</param>
      static void AddNewEdge(Parallelepiped piece)
      {
         var key = new string(piece.Colors);
         if (!_edges.ContainsKey(key))
         {
            _edges.Add(key, new List<Parallelepiped>());
            _edgesSizes.Add(key, 0);
         }
         _edges[key].Add(piece);
         _edgesSizes[key] += piece.Dimensions[piece.UndefinedDimensions[0]].Size;
      }

      /// <summary>
      /// Расставляет все куски
      /// </summary>
      public static void ProcessPieces()
      {
         ProcessCorners();
         if (_edges.Count > 0)
         {
            ProcessEdges();
            if (_middles.Count > 0)
               ProcessMiddlesAndBlacks();
         }
      }

      /// <summary>
      /// Выводит все куски в выходной поток
      /// </summary>
      /// <param name="output"></param>
      public static void OutputAllPieces(StreamWriter output)
      {
         foreach (var piece in _piecesInInputOrder)
            output.WriteLine(piece.ToString());
      }

      /// <summary>
      /// Обрабатывает углы
      /// </summary>
      static void ProcessCorners()
      {
         List<Parallelepiped> duplicates = null;

         foreach (string key in _corners.Keys)
         {
            if (_corners[key].Count == 1)
            {
               _corners[key][0].FindDefinedCoordinates(Cuts);
               PutInWhole(_corners[key][0]);
            }
            else
               duplicates = _corners[key];
         }

         if (duplicates != null)
            FindAndDeleteExcessCorner(duplicates);
      }

      /// <summary>
      /// Находит лишний угловой кусок из двух подходящих
      /// </summary>
      /// <param name="duplicates">Куски-дубликаты</param>
      static void FindAndDeleteExcessCorner(List<Parallelepiped> duplicates)
      {
         Parallelepiped excess = null;
         foreach (Parallelepiped piece in duplicates)
         {
            piece.FindDefinedCoordinates(Cuts);
            if (!IsOccupiedPlace(new[] { piece.X, piece.Y, piece.Z }))
               PutInWhole(piece);
            else
               excess = piece;
         }
         if (excess != null)
         {
            excess.Dimensions[0].Coordinate = WrongCoordinate;
            duplicates.Remove(excess);
         }
      }

      /// <summary>
      /// Обрабатывает куски на ребрах
      /// </summary>
      static void ProcessEdges()
      {
         foreach (var cut in Cuts)
            cut.Remove(0);
         foreach (string key in _edges.Keys)
            _edges[key].ForEach(e => e.FindDefinedCoordinates(Cuts));
         foreach (string key in _edges.Keys)
         {
            if (TryFindAndDeleteExcessEdge(key))
               break;
         }
         foreach (string key in _edges.Keys)
            _edges[key].ForEach(piece =>
            {
               piece.FindUndefinedCoordinates(Cuts);
               PutInWhole(piece);
            });
      }

      /// <summary>
      /// Пробует найти лишний кусок на ребре
      /// </summary>
      /// <param name="key">Вид ребра</param>
      /// <returns>Найден ли лишний кусок?</returns>
      static bool TryFindAndDeleteExcessEdge(string key)
      {
         int undefDimNumber = _edges[key][0].UndefinedDimensions[0];
         int leftCornerSize = Cuts[undefDimNumber].First.Value;
         int rightCornerSize = Cuts[undefDimNumber].Last.Value;
         int wholeSize = Whole.Dimensions[undefDimNumber].Size;
         int sizeBetweenCorners = wholeSize - leftCornerSize - rightCornerSize;
         int filledSize = _edgesSizes[key];
         if (sizeBetweenCorners < filledSize) //overflow
         {
            DeleteExcessPiece(key, undefDimNumber, filledSize - sizeBetweenCorners);
            return true;
         }
         return false;
      }

      static void DeleteExcessPiece(string key, int undefDimNumber, int emptySpaceSize)
      {
         Parallelepiped excess = _edges[key].Find(piece =>
         {
            for (int i = 0; i < 2; i++)
               if (i != undefDimNumber && piece.Dimensions[i].Coordinate == WrongCoordinate)
                  return true;
            return false;
         });

         excess = excess ?? _edges[key].First(p => p.Dimensions[undefDimNumber].Size == emptySpaceSize);
         excess.Dimensions[0].Coordinate = WrongCoordinate;
         _edges[key].Remove(excess);
      }

      /// <summary>
      /// Обрабатывает черные куски и кускиЮ находящиеся в середине граней
      /// </summary>
      static void ProcessMiddlesAndBlacks()
      {
         _places = CreateFreePlacesDictionary();
         foreach (var piece in _middles)
         {
            piece.FindDefinedCoordinates(Cuts);
            CheckNextFreePlace(piece);
            PutInWhole(piece);
         }
         if (_blacks.Count > 0)
         {
            foreach (var piece in _blacks)
            {
               CheckNextFreePlace(piece);
               PutInWhole(piece);
            }
         }
      }

      /// <summary>
      /// Создает список всевозможных пустых мест в каждом измерении
      /// </summary>
      /// <returns>Список пустых мест</returns>
      static List<Dictionary<int, List<int>>> CreateFreePlacesDictionary()
      {
         var places = new List<Dictionary<int, List<int>>>
            {
               new Dictionary<int, List<int>>(),
               new Dictionary<int, List<int>>(),
               new Dictionary<int, List<int>>()
            };
         for (int i = 0; i < 3; i++)
            CheckDimension(places, i);
         return places;
      }

      /// <summary>
      /// Проверяет найдется ли место для куска
      /// </summary>
      /// <param name="places">Все места</param>
      /// <param name="dimNumber">Номер измерения</param>
      static void CheckDimension(List<Dictionary<int, List<int>>> places, int dimNumber)
      {
         int coordinate = Cuts[dimNumber].First.Value;
         if (Cuts[dimNumber].Count > 1)
         {
            LinkedListNode<int> current = Cuts[dimNumber].First.Next;
            while (current != null && current.Next != null)
            {
               List<int> ar;
               if (!places[dimNumber].TryGetValue(current.Value, out ar))
                  places[dimNumber].Add(current.Value, new List<int> { 0, coordinate });
               else
                  ar.Add(coordinate);

               coordinate += current.Value;
               current = current.Next;
            }
         }
         else
         {
            if (!places[dimNumber].ContainsKey(Cuts[dimNumber].First.Value))
               places[dimNumber].Add(Cuts[dimNumber].First.Value, new List<int> { 0, coordinate });
         }
      }

      /// <summary>
      /// Проверяет правильное ли это состояние куска
      /// </summary>
      /// <param name="piece">Кусок</param>
      /// <param name="state">Состояние</param>
      /// <returns>Это правильное состоние?</returns>
      static bool IsRightState(Parallelepiped piece, IList<int> state)
      {
         for (int i = 0; i < 3; i++)
            if (!IsRightDimension(piece.Dimensions[state[i]], i))
               return false;
         return true;
      }

      /// <summary>
      /// Проверяет правильно ли повернуто измерение
      /// </summary>
      /// <param name="dimension">Измерение</param>
      /// <param name="dimNumber">Номер измерения</param>
      /// <returns>Правильно ли повернуто измерение?</returns>
      static bool IsRightDimension(Dimension dimension, int dimNumber)
      {
         if (dimension.Coordinate == WrongCoordinate)
            if (!_places[dimNumber].ContainsKey(dimension.Size))
               return false;
         return true;
      }

      /// <summary>
      /// Возвращает список всех мест, которые может занять кусок в данном состоянии
      /// </summary>
      /// <param name="piece">Кусок</param>
      /// <param name="state">Состояние</param>
      /// <returns>Список мест</returns>
      static List<List<int>> GetAllPossiblePlaces(Parallelepiped piece, IList<int> state)
      {
         var allPossiblePlaces = new List<List<int>>();

         for (int i = 0; i < 3; i++)
         {
            Dimension currentDimension = piece.Dimensions[state[i]];
            if (currentDimension.Coordinate == WrongCoordinate)
            {
               List<int> sizes = _places[i][currentDimension.Size];
               allPossiblePlaces.Add(sizes);
            }
            else
               allPossiblePlaces.Add(new List<int> { 0, 0 });
         }

         return allPossiblePlaces;
      }

      /// <summary>
      /// Проверяет следующее свободное место
      /// </summary>
      /// <param name="piece">Кусок</param>
      static void CheckNextFreePlace(Parallelepiped piece)
      {
         List<int[]> states = piece.GetStates();
         foreach (var state in states)
         {
            if (IsRightState(piece, state))
            {
               List<List<int>> p = GetAllPossiblePlaces(piece, state);
               var coordSet = new[] { p[0][0], p[1][0], p[2][0] };
               var rem = new int[3];
               Array.Copy(coordSet, rem, 3);
               do
               {
                  var newCoodrs = GetNewCoordinates(piece, state, coordSet);
                  if (!IsOccupiedPlace(newCoodrs))
                  {
                     piece.ApplyNewStateAndCoordinates(states.Count, state, newCoodrs);
                     coordSet = Utilities.NextPermutation(coordSet, p[0].Count, p[1].Count, p[2].Count);
                     p[0][0] = coordSet[0];
                     p[1][0] = coordSet[1];
                     p[2][0] = coordSet[2];
                     return;
                  }
                  coordSet = Utilities.NextPermutation(coordSet, p[0].Count, p[1].Count, p[2].Count);
               } while (!coordSet.SequenceEqual(rem));
            }
         }
      }

      /// <summary>
      /// Генерирует новые координаты для проверки
      /// </summary>
      /// <param name="piece">Кусок</param>
      /// <param name="state">Состояние куска</param>
      /// <param name="coordinates">Последние координаты</param>
      /// <returns>Новые координаты</returns>
      static int[] GetNewCoordinates(Parallelepiped piece, IList<int> state, IList<int> coordinates)
      {
         var newCoordinates = new int[3];
         for (int j = 0; j < 3; j++)
         {
            Dimension currentDimension = piece.Dimensions[state[j]];
            if (piece.Dimensions[state[j]].Coordinate == WrongCoordinate)
               newCoordinates[j] = _places[j][currentDimension.Size][coordinates[j] + 1];
            else
               newCoordinates[j] = currentDimension.Coordinate;
         }
         return newCoordinates;
      }

      /// <summary>
      /// Проверяет занято ли место с данными координатами
      /// </summary>
      /// <param name="place">Координаты места</param>
      /// <returns>Место занято?</returns>
      public static bool IsOccupiedPlace(IList<int> place)
      {
         if (!OccupiedCoords.ContainsKey(place[0]))
            return false;
         if (!OccupiedCoords[place[0]].ContainsKey(place[1]))
            return false;
         return OccupiedCoords[place[0]][place[1]].Contains(place[2]);
      }

      /// <summary>
      /// Присваивает пустое место куску
      /// </summary>
      /// <param name="piece">Кусок</param>
      static void PutInWhole(Parallelepiped piece)
      {
         if (!OccupiedCoords.ContainsKey(piece.X))
            OccupiedCoords.Add(piece.X, new Dictionary<int, HashSet<int>>());
         if (!OccupiedCoords[piece.X].ContainsKey(piece.Y))
            OccupiedCoords[piece.X].Add(piece.Y, new HashSet<int>());
         OccupiedCoords[piece.X][piece.Y].Add(piece.Z);
      }
   }
}