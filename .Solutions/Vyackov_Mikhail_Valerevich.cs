using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//Asymptotics is O(n). So it should work pretty fast with great amount of data
//On developer's machine it takes 2 seconds to process 10^5 pieces

namespace KonturItern
{
    internal enum Side
    {
        Front, Back, Bottom, Top, Left, Right
    }

    internal enum Axis
    {
        FrontBack, TopBottom, LeftRight
    }

    internal static class PieceExtentions
    {
        private static readonly int[][] rotationMatrix = new[] { 
                new[] {0, 1, 4, 5, 3, 2}, //FrontBack
                new[] {4, 5, 2, 3, 1, 0}, //TopBottom
                new[] {3, 2, 0, 1, 4, 5}  //LeftRight
            };

        private static readonly Side[] rotationBuffer = new Side[6];

        public static bool HasColorsOnSides(this Piece piece, IEnumerable<Side> sides)
        {
            return sides.All(e => piece.Coloring[(int)piece.Rotation[(int)e]] != '.');
        }

        public static bool HasNoColorsOnSides(this Piece piece, IEnumerable<Side> sides)
        {
            return sides.All(e => piece.Coloring[(int)piece.Rotation[(int)e]] == '.');
        }

        public static bool MatchBasePiece(this Piece piece, Piece basePiece)
        {
            return Enumerable.Range(0, 6).All(e =>
                piece.Coloring[(int)piece.Rotation[e]] == '.' || piece.Coloring[(int)piece.Rotation[e]] == basePiece.Coloring[e]);
        }

        public static void ApplyTranformation(this Piece piece, Axis axis)
        {
            for (int i = 0; i < 6; i++)
                rotationBuffer[i] = piece.Rotation[i];
            for (int i = 0; i < 6; i++)
                piece.Rotation[i] = rotationBuffer[rotationMatrix[(int)axis][i]];
        }
    }

    internal struct Size
    {
        public int Length { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public override bool Equals(object obj)
        {
            return Equals((Size)obj);
        }

        public bool Equals(Size other)
        {
            return Length == other.Length && Height == other.Height && Width == other.Width;
        }

        public override int GetHashCode()
        {
            return Length ^ Height ^ Width;
        }

        public Size GetUnordered()
        {
            var size = new Size { Length = Math.Max(Length, Math.Max(Height, Width)), Width = Math.Min(Length, Math.Min(Height, Width)) };
            size.Height = Length + Height + Width - size.Length - size.Width;
            return size;
        }
    }

    internal struct PieceIdentity
    {
        public Size UnorderedSize { 
            get { return unorderedSize; }
            set { unorderedSize = value.GetUnordered(); if (coloring != null) hash = UnorderedSize.GetHashCode() ^ Coloring.GetHashCode(); } 
        }
        public string Coloring { 
            get { return coloring; }
            set { coloring = new string(value.Where(e => e != '.').OrderBy(e => e).ToArray()); hash = UnorderedSize.GetHashCode() ^ Coloring.GetHashCode(); } 
        }

        private Size unorderedSize;
        private string coloring;
        private int hash;

        public override int GetHashCode()
        {
            return hash;
        }

        public override bool Equals(object obj)
        {
            return Equals((PieceIdentity)obj);
        }

        public bool Equals(PieceIdentity other)
        {
            return UnorderedSize.Equals(other.UnorderedSize) && Coloring.Equals(other.Coloring);
        }
    }

    internal class Piece
    {
        private static readonly string[] sideToStringMapping = new[] {
            "F", "B", "D", "U", "L", "R"
        };

        private int currentRotation = 0;

        public Size Size { get; set; }
        public string Coloring { get; set; }
        public Side[] Rotation { get; set; }
        public PieceIdentity Identity { get; set; }

        public Piece(Size size, string coloring)
        {
            Rotation = new[] { Side.Front, Side.Back, Side.Bottom, Side.Top, Side.Left, Side.Right };
            Size = size;
            Coloring = coloring;
            Identity = new PieceIdentity { UnorderedSize = Size, Coloring = Coloring };
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Piece);
        }

        public bool Equals(Piece other)
        {
            return Identity.Equals(other.Identity);
        }

        public override int GetHashCode()
        {
            return Identity.GetHashCode();
        }

        public string ToString(Size offset)
        {
            return String.Format("{0} {1} {2} {3} {4}",
                sideToStringMapping[(int)Rotation[(int)Side.Front]],
                sideToStringMapping[(int)Rotation[(int)Side.Bottom]],
                offset.Length, offset.Height, offset.Width);
        }

        public void CommitNextRotation()
        {
            Roll(Axis.FrontBack);
            if (currentRotation % 4 == 3)
                Roll((currentRotation / 4) % 2 != 0 ? Axis.LeftRight : Axis.TopBottom);
            ++currentRotation;
        }

        public void Roll(Axis axis)
        {
            this.ApplyTranformation(axis);
            if (axis == Axis.FrontBack)
                Size = new Size { Length = Size.Length, Height = Size.Width, Width = Size.Height };
            if (axis == Axis.TopBottom)
                Size = new Size { Length = Size.Width, Height = Size.Height, Width = Size.Length };
            if (axis == Axis.LeftRight)
                Size = new Size { Length = Size.Height, Height = Size.Length, Width = Size.Width };
        }
    }

    internal class ProblemSolver
    {
        private Piece basePiece;
        private readonly Dictionary<PieceIdentity, Queue<int>> storage = new Dictionary<PieceIdentity, Queue<int>>();
        private readonly List<Piece> pieces = new List<Piece>();
        private readonly bool[] isSlicingFinished = new[] { false, false, false };
        private readonly List<int>[] slices = Enumerable.Range(0, 3).Select(e => new List<int>()).ToArray();
        private readonly Side[][] edges = new[] {
                new[] { Side.Front, Side.Back },
                new[] { Side.Bottom, Side.Top },
                new[] { Side.Left, Side.Right } };
        private List<string> answer;

        public void AddInitialData(int length, int height, int width, string coloring)
        {
            basePiece = new Piece(new Size { Length = length, Height = height, Width = width }, coloring);
        }

        public static Piece GetDefiningPiece(int length, int height, int width, string coloring, Piece basePiece)
        {
            Piece newPiece = new Piece(new Size { Length = length, Height = height, Width = width }, coloring);
            while (!newPiece.MatchBasePiece(basePiece))
                newPiece.CommitNextRotation();
            return newPiece;
        }

        public void AddPiece(int length, int height, int width, string coloring)
        {
            var newPiece = GetDefiningPiece(length, height, width, coloring, basePiece);
            pieces.Add(newPiece);
            if (!storage.ContainsKey(newPiece.Identity))
                storage[newPiece.Identity] = new Queue<int>();
            storage[newPiece.Identity].Enqueue(pieces.Count - 1);
        }

        public static string FormColoring(Piece basePiece, params bool[] coloring)
        {
            return new String(Enumerable.Range(0, 6).Select(e => coloring[e] ? basePiece.Coloring[e] : '.').ToArray());
        }

        private void ProcessPieceByPlace(Size requredSize, string coloring, Size offset)
        {
            PieceIdentity identityToFind = new PieceIdentity { UnorderedSize = requredSize, Coloring = coloring };
            int index = storage[identityToFind].Dequeue();
            var piece = pieces[index];
            while (!(piece.MatchBasePiece(basePiece) && piece.Size.Equals(requredSize)))
                piece.CommitNextRotation();
            answer[index] = pieces[index].ToString(offset);
        }

        private void PlacePieces()
        {
            answer = Enumerable.Repeat<string>(null, pieces.Count).ToList();
            for (int i = 0, lengthOffset = 0; i < slices[(int)Axis.FrontBack].Count; lengthOffset += slices[(int)Axis.FrontBack][i++])
                for (int j = 0, heightOffset = 0; j < slices[(int)Axis.TopBottom].Count; heightOffset += slices[(int)Axis.TopBottom][j++])
                    for (int k = 0, widthOffset = 0; k < slices[(int)Axis.LeftRight].Count; widthOffset += slices[(int)Axis.LeftRight][k++])
                    {
                        ProcessPieceByPlace(
                            new Size { Length = slices[(int)Axis.FrontBack][i], Height = slices[(int)Axis.TopBottom][j], Width = slices[(int)Axis.LeftRight][k] },
                            FormColoring(basePiece,
                                i == 0, i == slices[(int)Axis.FrontBack].Count - 1,
                                j == 0, j == slices[(int)Axis.TopBottom].Count - 1,
                                k == 0, k == slices[(int)Axis.LeftRight].Count - 1),
                            new Size { Length = lengthOffset, Height = heightOffset, Width = widthOffset });
                    }
        }

        public void DisplaySolution(TextWriter outputStream)
        {
            foreach (var item in answer)
            {
                outputStream.WriteLine(item);
            }
        }

        private static int GetSliceScale(Axis axis, Piece piece)
        {
            return axis == Axis.FrontBack ? piece.Size.Length : axis == Axis.TopBottom ? piece.Size.Height : piece.Size.Width;
        }

        private void AddSlice(Axis axis, Piece piece)
        {
            if (piece.HasColorsOnSides(new[] { edges[(int)axis][0] }))
                slices[(int)axis].Insert(0, GetSliceScale(axis, piece));
            else if (piece.HasNoColorsOnSides(new[] { edges[(int)axis][1] }))
            {
                if (slices[(int)axis].Count == 0)
                    slices[(int)axis].Add(GetSliceScale(axis, piece));
                else
                    slices[(int)axis].Insert(slices[(int)axis].Count - (isSlicingFinished[(int)axis] ? 1 : 0), GetSliceScale(axis, piece));
            }
            else
            {
                slices[(int)axis].Add(GetSliceScale(axis, piece));
                isSlicingFinished[(int)axis] = true;
            }
        }

        private void MakeSlices()
        {
            foreach (var piece in pieces)
            {
                if (piece.HasColorsOnSides(new[] { Side.Front, Side.Bottom }))
                    AddSlice(Axis.LeftRight, piece);
                if (piece.HasColorsOnSides(new[] { Side.Front, Side.Left }))
                    AddSlice(Axis.TopBottom, piece);
                if (piece.HasColorsOnSides(new[] { Side.Left, Side.Bottom }))
                    AddSlice(Axis.FrontBack, piece);
            }
        }

        public void ProcessGivenData()
        {
            MakeSlices();
            PlacePieces();
        }
    }

    public class StreamTokenizer
    {
        private IEnumerator<string> tokensEnumerator;
        private const int blockSize = 1024;

        public StreamTokenizer(TextReader inputStream)
        {
            tokensEnumerator = ReadStream(inputStream).GetEnumerator();
        }

        public string NextWord()
        {
            return tokensEnumerator.MoveNext() ? tokensEnumerator.Current : null;
        }

        public int NextInt()
        {
            return int.Parse(NextWord());
        }

        public void ProcessQueryCollection(Action<StreamTokenizer> processor)
        {
            var count = NextInt();
            for (int i = 0; i < count; ++i)
            {
                processor(this);
            }
        }

        public void ProcessSingleQuery(Action<StreamTokenizer> processor)
        {
            processor(this);
        }

        private IEnumerable<string> ReadStream(TextReader reader)
        {
            StringBuilder currentToken = new StringBuilder();
            char[] buffer = new char[blockSize];
            int readCount;
            while ((readCount = reader.Read(buffer, 0, blockSize)) != 0)
            {
                for (int i = 0; i < readCount; ++i)
                    if (!Char.IsWhiteSpace(buffer[i]))
                    {
                        currentToken.Append(buffer[i]);
                    }
                    else if (currentToken.Length != 0)
                    {
                        yield return currentToken.ToString();
                        currentToken.Clear();
                    }
            }
            if (currentToken.Length != 0)
                yield return currentToken.ToString();
        }
    }

    public static partial class Program
    {
        public static void Main(string[] args)
        {
            StreamTokenizer tokenizer = new StreamTokenizer(Console.In);
            ProblemSolver solver = new ProblemSolver();
            tokenizer.ProcessSingleQuery(r => solver.AddInitialData(
                length: r.NextInt(), height: r.NextInt(), width: r.NextInt(), coloring: r.NextWord()));
            tokenizer.ProcessQueryCollection(r => solver.AddPiece(
                length: r.NextInt(), height: r.NextInt(), width: r.NextInt(), coloring: r.NextWord()));
            solver.ProcessGivenData();
            solver.DisplaySolution(Console.Out);
        }
    }
}