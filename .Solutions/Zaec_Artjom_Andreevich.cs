// ID посылки на timus'е - 4901712

using System;
using System.Collections.Generic;
using System.Linq;

namespace Parallelepiped
{
    public class Program
    {
        private static void Main()
        {
            (new Solver()).Solve();
        }
    }

    public class Solver
    {
        private WholeParallelepiped theCube;
        private Grid grid = new Grid();

        private readonly InputReader reader;

        public void Solve()
        {
            reader.ReadAll();
            PlaceCorners(GetPartsOfKind(PartKind.Corner));
            PlaceEdges(GetPartsOfKind(PartKind.Egde));
            PlaceFaces(GetPartsOfKind(PartKind.Face));
            WriteResult();
        }

        /// <summary>
        /// "Пристыковывает" кусочек к указанной стороне
        /// </summary>
        private void PlaceToSide(ParallelepipedPart part, Side side)
        {
            var axis = side.ToAxis();
            part.Place(axis, side.IsFirstInAxis() ? 0 : theCube.GetLength(axis) - part.GetLength(axis));
        }

        private void PlaceCorners(IEnumerable<ParallelepipedPart> corners)
        {
            foreach (var corner in corners)
            {
                var from = new Side[3];
                from[0] = Tools.IterateSide().Where(corner.IsColoured).First();
                from[1] = from[0].AdjacentSides().Where(corner.IsColoured).First();
                from[2] = from[0].AdjacentSides().Intersect(from[1].AdjacentSides()).Where(corner.IsColoured).First();
                // Это мы взяли три покрашенных стороны, попарно смежных между собой
                var to = from.Select(side => theCube.GetSideByColour(corner.GetColour(side))).ToArray();
                corner.Rotate(from[0], from[1], to[0], to[1]);
                foreach (var side in to)
                {
                    PlaceToSide(corner, side);
                }
            }

            // Поставить начальные разрезы
            var firstSides = Tools.IterateSide().Where(Tools.IsFirstInAxis);
            foreach (var corner in corners.Where(corner => firstSides.All(corner.IsColoured)))
            {
                foreach (var axis in Tools.IterateAxis())
                {
                    grid.AddNextCut(axis, corner.GetLength(axis));
                }
                break;
            }
        }

        private void RotateEdges(IEnumerable<ParallelepipedPart> edges)
        {
            foreach (var edge in edges)
            {
                var from = new Side[2];
                from[0] = Tools.IterateSide().Where(edge.IsColoured).First();
                from[1] = from[0].AdjacentSides().Where(edge.IsColoured).First();
                Side[] to = from.Select(side => theCube.GetSideByColour(edge.GetColour(side))).ToArray();
                edge.Rotate(from[0], from[1], to[0], to[1]);
            }
        }

        private static void SortEdgesBySides(Axis axis, IEnumerable<ParallelepipedPart> edges, out List<Side> sides,
                                             out StackDictionary<int, ParallelepipedPart>[] hashes,
                                             out List<ParallelepipedPart> mainEdges)
        {
            sides = axis.ToAnySide().AdjacentSides(ordered: true).ToList();
            sides.Add(sides[0]);

            hashes = new StackDictionary<int, ParallelepipedPart>[4];
            for (int i = 0; i < 4; i++)
            {
                hashes[i] = new StackDictionary<int, ParallelepipedPart>();
            }

            mainEdges = new List<ParallelepipedPart>();
            foreach (var edge in edges)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (edge.IsColoured(new[] {sides[i], sides[i + 1]}))
                    {
                        if (i == 0) mainEdges.Add(edge);
                        hashes[i].Add(edge.GetLengthOnBorder(sides[i], sides[i + 1]), edge);
                        break;
                    }
                }
            }
        }

        private void PlaceEdgesOnAxis(Axis axis, IEnumerable<ParallelepipedPart> edges)
        {
            if (!edges.Any()) return;

            List<Side> sides;
            StackDictionary<int, ParallelepipedPart>[] edgeHashes;
            List<ParallelepipedPart> mainEdges;
            SortEdgesBySides(axis, edges, out sides, out edgeHashes, out mainEdges);

            foreach (var edge in mainEdges)
            {
                var currLen = grid.LastCutCoord(axis);
                var len = edge.GetLength(axis);

                PlaceToSide(edge, sides[0]);
                PlaceToSide(edge, sides[1]);
                edge.Place(axis, currLen);

                for (int i = 1; i < 4; i++)
                {
                    var othEdge = edgeHashes[i][len];
                    if (othEdge == null) continue;
                    edgeHashes[i].Remove(len);

                    PlaceToSide(othEdge, sides[i]);
                    PlaceToSide(othEdge, sides[i + 1]);
                    othEdge.Place(axis, currLen);
                }
                grid.AddNextCut(axis, len);
            }
        }

        private void PlaceEdges(IEnumerable<ParallelepipedPart> edges) //TODO
        {
            if (!edges.Any()) return;
            RotateEdges(edges);
            foreach (var axis in Tools.IterateAxis())
            {
                PlaceEdgesOnAxis(axis,
                                 edges.Where(
                                     edge =>
                                     !edge.IsColoured(axis.ToAnySide()) && !edge.IsColoured(axis.ToAnySide().Opposite())));
            }
        }

        private IEnumerable<ParallelepipedPart> GetPartsOfKind(PartKind kind)
        {
            return theCube.Parts.Where(part => part.Kind == kind).ToList();
        }

        private void RotateFaces(IEnumerable<ParallelepipedPart> faces)
        {
            foreach (var face in faces)
            {
                var from = Tools.IterateSide().Where(face.IsColoured).First();
                var to = theCube.GetSideByColour(face.GetColour(from));
                face.Rotate(from, to);
            }
        }

        private static int[] MakeFaceKey(Side colouredSide, ParallelepipedPart face)
        {
            return Tools.IterateAxis().Where(axis => axis != colouredSide.ToAxis()).Select(face.GetLength).ToArray();
        }

        private class PartKeyComparer : IEqualityComparer<int[]>
        {
            public bool Equals(int[] x, int[] y)
            {
                if (x.Length != y.Length) return false;
                var a = new List<int>(x);
                var b = new List<int>(y);
                a.Sort();
                b.Sort();
                return !a.Where((t, i) => t != b[i]).Any();
            }

            public int GetHashCode(int[] obj)
            {
                return obj.Sum();
            }
        }

        private void PlaceAFace(Side side, IEnumerable<ParallelepipedPart> faces)
        {
            if (!faces.Any()) return;

            var hash = new StackDictionary<int[], ParallelepipedPart>(new PartKeyComparer());
            foreach (var face in faces)
            {
                hash.Add(MakeFaceKey(side, face), face);
            }

            var axises = Tools.IterateAxis().Where(axis => axis != side.ToAxis()).ToArray();
            for (int i = 1; i < grid.NumberOfCuts(axises[0]); i++)
                for (int j = 1; j < grid.NumberOfCuts(axises[1]); j++)
                {
                    var key = new[] {grid.GetSliceLength(axises[0], i), grid.GetSliceLength(axises[1], j)};
                    var part = hash[key];
                    hash.Remove(key);

                    if (part.GetLength(axises[0]) != key[0])
                        part.Rotate(side, axises[0].ToAnySide(), side, axises[1].ToAnySide());

                    PlaceToSide(part, side);
                    part.Place(axises[0], grid.GetCutCoord(axises[0], i));
                    part.Place(axises[1], grid.GetCutCoord(axises[1], j));
                }
        }

        private void PlaceFaces(IEnumerable<ParallelepipedPart> faces)
        {
            if (!faces.Any()) return;

            RotateFaces(faces);

            var facesOnSide = new Dictionary<Side, List<ParallelepipedPart>>(6);
            foreach (var face in faces)
            {
                var side = Tools.IterateSide().Where(face.IsColoured).First();
                if (!facesOnSide.ContainsKey(side))
                    facesOnSide[side] = new List<ParallelepipedPart>();
                facesOnSide[side].Add(face);
            }

            foreach (var side in Tools.IterateSide())
            {
                if (facesOnSide.ContainsKey(side))
                    PlaceAFace(side, facesOnSide[side]);
            }
        }

        public Solver()
        {
            reader = new InputReader(this);
        }

        private void WriteResult()
        {
            var sideSymbols = new[] {'F', 'B', 'D', 'U', 'L', 'R'};
            foreach (var part in theCube.Parts)
            {
                Console.WriteLine("{0} {1} {2} {3} {4}", sideSymbols[(int) part.ToCurrentSide(Side.Front)],
                                  sideSymbols[(int) part.ToCurrentSide(Side.Bottom)], part.GetLocation(Axis.FrontBack),
                                  part.GetLocation(Axis.BottomTop), part.GetLocation(Axis.LeftRight));
            }
        }

        private class InputReader
        {
            private readonly TokenReader r = new TokenReader();
            private readonly Solver parent;

            public void ReadAll()
            {
                int[] lengthes;
                string colors;
                ReadCube(out lengthes, out colors);
                int numOfParts = r.ReadInt();
                parent.theCube = new WholeParallelepiped(lengthes, colors, numOfParts);
                for (int i = 0; i < numOfParts; i++)
                {
                    ReadCube(out lengthes, out colors);
                    parent.theCube.Parts[i] = new ParallelepipedPart(lengthes, colors);
                }
            }

            private void ReadCube(out int[] lengthes, out string colors)
            {
                lengthes = new int[3];
                for (int i = 0; i < lengthes.Length; i++)
                    lengthes[i] = r.ReadInt();
                colors = r.ReadString();
            }

            public InputReader(Solver solver)
            {
                parent = solver;
            }
        }
    }

    /// <summary>
    /// Разрезы создают сетку, она однозначно определяется расставленными углами и рёбрами
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// Здесь хранятся длины между соседними разрезами
        /// </summary>
        private readonly Dictionary<Axis, List<int>> sliceLengthes = new Dictionary<Axis, List<int>>();

        /// <summary>
        /// Хранит координаты разрезов
        /// </summary>
        private readonly Dictionary<Axis, List<int>> cutCoords = new Dictionary<Axis, List<int>>();

        public int GetSliceLength(Axis axis, int index)
        {
            return sliceLengthes[axis][index];
        }

        public int GetCutCoord(Axis axis, int index)
        {
            return cutCoords[axis][index];
        }

        public int NumberOfCuts(Axis axis)
        {
            return sliceLengthes[axis].Count;
        }

        public int LastCutCoord(Axis axis)
        {
            return cutCoords[axis].Last();
        }

        /// <summary>
        /// Добавить следующий разрез по оси
        /// </summary>
        /// <param name="axis">Ось, по которой добавить разрез</param>
        /// <param name="length">Расстояние до предыдущего разреза</param>
        /// <returns></returns>
        public void AddNextCut(Axis axis, int length)
        {
            sliceLengthes[axis].Add(length);
            cutCoords[axis].Add(cutCoords[axis].LastOrDefault() + length);
        }

        public Grid()
        {
            foreach (var axis in Tools.IterateAxis())
            {
                sliceLengthes[axis] = new List<int>();
                cutCoords[axis] = new List<int> {0};
            }
        }
    }

    public class StackDictionary<TKey, TValue> where TValue : class
    {
        private readonly Dictionary<TKey, Stack<TValue>> dictionary;

        public void Add(TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
                dictionary[key] = new Stack<TValue>();
            dictionary[key].Push(value);
        }

        public bool Remove(TKey key)
        {
            if (!dictionary.ContainsKey(key))
                return false;
            if (dictionary[key].Count == 1)
                dictionary.Remove(key);
            else
                dictionary[key].Pop();
            return true;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!dictionary.ContainsKey(key))
                    return null;
                var value = dictionary[key];
                return value.Peek();
            }
        }

        public StackDictionary(int capacity)
        {
            dictionary = new Dictionary<TKey, Stack<TValue>>(capacity);
        }

        public StackDictionary()
        {
            dictionary = new Dictionary<TKey, Stack<TValue>>();
        }

        public StackDictionary(IEqualityComparer<TKey> comparer)
        {
            dictionary = new Dictionary<TKey, Stack<TValue>>(comparer);
        }
    }

    public enum Side
    {
        Front = 0,
        Back,
        Bottom,
        Top,
        Left,
        Right
    }

    public enum Axis
    {
        FrontBack = 0,
        BottomTop,
        LeftRight
    }

    public static class Tools
    {
        /// <summary>
        /// Возвращает противоположную сторону
        /// </summary>
        public static Side Opposite(this Side side)
        {
            if (side.IsFirstInAxis())
                return side + 1;
            return side - 1;
        }

        public static IEnumerable<Side> IterateSide()
        {
            for (int i = 0; i <= 5; i++)
                yield return (Side) i;
        }

        public static IEnumerable<Axis> IterateAxis()
        {
            for (int i = 0; i <= 2; i++)
                yield return (Axis) i;
        }

        /// <summary>
        /// Итерирует смежные грани
        /// </summary>
        /// <param name="side"></param>
        /// <param name="ordered">Если true, то в результирующей коллекции каждая следующая сторона будет смежна к предыдущей и последняя смежна с первой</param>
        public static IEnumerable<Side> AdjacentSides(this Side side, bool ordered = false)
        {
            if (ordered)
            {
                var sides = AdjacentSides(side).ToList();
                var t = sides[1];
                sides[1] = sides[2];
                sides[2] = t;
                return sides;
            }
            return IterateSide().Where(sd => side.ToAxis() != sd.ToAxis());
        }

        public static Axis ToAxis(this Side side)
        {
            return (Axis) ((int) side/2);
        }

        public static Side ToAnySide(this Axis axis)
        {
            return (Side) ((int) axis*2);
        }

        /// <summary>
        /// Проверяет, находится ли данная сторона первой в своей оси
        /// </summary>
        public static bool IsFirstInAxis(this Side side)
        {
            return (int) side%2 == 0;
        }

        /// <summary>
        /// По двум смежным сторонам возвращает третью, смежную с ними, при этом все три стороны будут образовывать правую верхнюю тройку
        /// </summary>
        public static Side ThirdAdjacent(Side first, Side second)
        {
            int f = (int) first, s = (int) second;
            var ans = new[,]
                          {
                              {0, 2, 4}, {0, 3, 5}, {0, 4, 3}, {0, 5, 2},
                              {1, 2, 5}, {1, 3, 4}, {1, 4, 2}, {1, 5, 3},
                              {2, 4, 0}, {2, 5, 1},
                              {3, 4, 1}, {3, 5, 0},
                          };
            for (int i = 0; i < ans.Length; i++)
            {
                if (f == ans[i, 0] && s == ans[i, 1]) return (Side) ans[i, 2];
                if (s == ans[i, 0] && f == ans[i, 1]) return ((Side) ans[i, 2]).Opposite();
            }
            throw new AlgorithmException();
        }
    }

    public abstract class Parallelepiped
    {
        protected readonly char[] Colours = new char[6];
        protected readonly int[] Lengthes = new int[3];

        /// <summary>
        /// Возвращает длину по оси.
        /// </summary>
        public int GetLength(Axis axis)
        {
            return Lengthes[(int) axis];
        }

        /// <summary>
        /// Возвращает длину ребра на стыке двух сторон
        /// </summary>
        public int GetLengthOnBorder(Side side1, Side side2)
        {
            foreach (var axis in Tools.IterateAxis())
            {
                if (axis != side1.ToAxis() && axis != side2.ToAxis())
                {
                    return GetLength(axis);
                }
            }
            throw new AlgorithmException();
        }

        public int GetLengthOnBorder(Side[] sides)
        {
            if (sides.Length != 2) throw new ArgumentException();
            return GetLengthOnBorder(sides[0], sides[1]);
        }

        /// <summary>
        /// Цвет соответствующей стороны
        /// </summary>
        public char GetColour(Side side)
        {
            return Colours[(int) side];
        }

        public bool IsColoured(Side side)
        {
            return GetColour(side) != '.';
        }

        public bool IsColoured(IEnumerable<Side> sides)
        {
            return sides.All(IsColoured);
        }

        /// <summary>
        /// Проверяет, есть ли эти цвета на данном кубике
        /// </summary>
        public bool HasColours(IEnumerable<char> colours)
        {
            return colours.Intersect(Colours).Count() == colours.Count();
        }

        public Side GetSideByColour(char colour)
        {
            foreach (var sd in Tools.IterateSide().Where(sd => colour == GetColour(sd)))
            {
                return sd;
            }
            throw new ArgumentException();
        }

        protected Parallelepiped(int[] lengthes, string colours)
        {
            if (colours.Length != 6 || lengthes.Length != 3) throw new ArgumentException();
            Lengthes = lengthes;
            Colours = colours.ToCharArray();
        }
    }

    public class WholeParallelepiped : Parallelepiped
    {
        public readonly ParallelepipedPart[] Parts;

        public WholeParallelepiped(int[] lengthes, string colours, int numberOfParts)
            : base(lengthes, colours)
        {
            Parts = new ParallelepipedPart[numberOfParts];
        }
    }

    /// <summary>
    /// Тип кусочка параллелепипеда
    /// </summary>
    public enum PartKind
    {
        Corner,
        Egde,
        Face,
        Inner,
        /*// Частные случаи
        FlatCorner,
        FlatEdge,
        FlatFace,
        LineEnd,
        LineInner,
        Single*/
    }

    public class ParallelepipedPart : Parallelepiped
    {
        public PartKind Kind { get; private set; }

        public ParallelepipedPart(int[] lengthes, string colours)
            : base(lengthes, colours)
        {
            DetermineType();
            currentSides = new Side[6];
            currentLocation = new int[3];
            for (int i = 0; i <= 5; i++)
                currentSides[i] = (Side) i;
        }

        private readonly int[] currentLocation;

        public int GetLocation(Axis axis)
        {
            return currentLocation[(int) axis];
        }

        /// <summary>
        /// Перестановкой сторон задаётся поворот
        /// </summary>
        private readonly Side[] currentSides;

        /// <summary>
        /// Преобразует сторону на входе в сторону, которая фактически находится с этой стороны (т.е. учесть поворот кубика)
        /// </summary>
        public Side ToCurrentSide(Side side)
        {
            return currentSides[(int) side];
        }

        /// <summary>
        /// Поворачивает кусочек так, чтобы первые 2 стороны перешли во вторые 2 соответственно. И первые, и вторые стороны должны быть смежны. Предполагается, что кусочек не был повернут.
        /// </summary>
        public void Rotate(Side from1, Side from2, Side to1, Side to2)
        {
            if (from1 == from2.Opposite()) throw new AlgorithmException();
            var from = new[] {from1, from2, Tools.ThirdAdjacent(from1, from2)};
            var to = new[] {to1, to2, Tools.ThirdAdjacent(to1, to2)};
            var sideBuf = new Side[6];
            var lenBuf = new int[3];
            var colBuf = new char[6];
            for (int i = 0; i < from.Length; i++)
            {
                sideBuf[(int) to[i]] = currentSides[(int) from[i]];
                sideBuf[(int) to[i].Opposite()] = currentSides[(int) from[i].Opposite()];
                lenBuf[(int) to[i].ToAxis()] = Lengthes[(int) from[i].ToAxis()];
                colBuf[(int) to[i]] = Colours[(int) from[i]];
                colBuf[(int) to[i].Opposite()] = Colours[(int) from[i].Opposite()];
            }
            for (int i = 0; i < 6; i++)
            {
                currentSides[i] = sideBuf[i];
                Lengthes[i/2] = lenBuf[i/2];
                Colours[i] = colBuf[i];
            }
        }

        public void Rotate(Side from, Side to)
        {
            Rotate(from, from.AdjacentSides().First(), to, to.AdjacentSides().First());
        }

        /// <summary>
        /// Размещает кубик по данной оси на данное расстояние от начала оси
        /// </summary>
        public void Place(Axis axis, int value)
        {
            currentLocation[(int) axis] = value;
        }

        private void DetermineType()
        {
            int colourNumber = Colours.Count(ch => ch != '.');
            var colouredSides = Tools.IterateSide().Where(IsColoured);
            switch (colourNumber)
            {
                case 0:
                    Kind = PartKind.Inner;
                    break;
                case 1:
                    Kind = PartKind.Face;
                    break;
                case 2:
                    foreach (var side in colouredSides)
                    {
                        Kind = IsColoured(side.Opposite()) ? PartKind.Face : PartKind.Egde;
                        break;
                    }
                    break;
                case 3:
                    foreach (var side in colouredSides)
                    {
                        if (IsColoured(side.Opposite()))
                        {
                            Kind = PartKind.Egde;
                            goto l1;
                        }
                    }
                    Kind = PartKind.Corner;
                    l1:
                    break;
                case 4:
                    foreach (var side in colouredSides)
                    {
                        if (!IsColoured(side.Opposite()))
                        {
                            Kind = PartKind.Corner;
                            goto l2;
                        }
                    }
                    Kind = PartKind.Egde;
                    l2:
                    break;
                case 5:
                    Kind = PartKind.Corner;
                    break;
                case 6:
                    Kind = PartKind.Corner;
                    break;
            }
        }
    }

    public class TokenReader
    {
        private readonly LinkedList<String> tokenQueue = new LinkedList<string>();

        private String NextToken()
        {
            if (!tokenQueue.Any())
            {
                string line = Console.ReadLine() ?? "";
                var tokens = line.Split(new[] {' ', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var token in tokens)
                {
                    tokenQueue.AddLast(token);
                }
            }
            String res = tokenQueue.First.Value;
            tokenQueue.RemoveFirst();
            return res;
        }

        public int ReadInt()
        {
            return int.Parse(NextToken());
        }

        public string ReadString()
        {
            return NextToken();
        }
    }

    public class AlgorithmException : Exception
    {
    }
}