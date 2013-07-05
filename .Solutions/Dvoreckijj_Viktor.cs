using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/*
 * ID на тимусе: 4902351
 * Реализованы все усложнения (не факт, что работают)
 */

namespace Training
{
    class Chunk : IEquatable<Chunk>
    {
        public int Width { get; private set; }
        public int Depth { get; private set; }
        public int Height { get; private set; }
        public string Colors { get; private set; }
        private char[] rotation;
        public bool IsCorner
        {
            get
            {
                return colorGroupCount == 3;
            }
        }
        public bool IsEdge
        {
            get
            {
                return colorGroupCount >= 2;
            }
        }
        private int colorGroupCount;
        private static readonly int[] rotationsX = new[] { 0, 1, 4, 5, 3, 2 };
        private static readonly int[] rotationsY = new[] { 2, 3, 1, 0, 4, 5 };
        private static readonly int[] rotationsZ = new[] { 4, 5, 2, 3, 1, 0 };
        private int ColorCount()
        {
            int count = 0;
            for (int i = 0; i < 6; i += 2)
                count += (Colors[i] != '.' || Colors[i + 1] != '.') ? 1 : 0;
            return count;
        }
        public int EdgeLength(char color1, char color2, bool strict = false)
        {
            int index1 = Colors.IndexOf(color1), index2 = Colors.IndexOf(color2);
            if ((index1 >= 0 && index1 < 2) && (index2 >= 2 && index2 < 4))
                return Height;
            if ((index1 >= 4 && index1 < 6) && (index2 >= 0 && index2 < 2))
                return Depth;
            if ((index1 >= 2 && index1 < 4) && (index2 >= 4 && index2 < 6))
                return Width;
            if (strict)
                return -1;
            return EdgeLength(color2, color1, true);
        }

        public Chunk(int w, int d, int h, string colors)
        {
            Width = w;
            Depth = d; 
            Height = h;
            Colors = colors;
            colorGroupCount = ColorCount();
        }
        public IEnumerable<Chunk> GetRotations()
        {
            Chunk template = this.MemberwiseClone() as Chunk;
            template.rotation = "FBDULR".ToCharArray();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        yield return template.MemberwiseClone() as Chunk;
                        Rotate(template, rotationsZ);
                    }
                    Rotate(template, rotationsY);
                }
                Rotate(template, rotationsX);
            }
        }
        private void Rotate(Chunk chunk, int[] rotations)
        {
            char[] oldFaces = chunk.rotation.Clone() as char[];
            char[] oldColors = chunk.Colors.ToCharArray();
            char[] newColors = new char[oldColors.Length];
            for (int i = 0; i < rotations.Length; i++)
            {
                chunk.rotation[i] = oldFaces[rotations[i]];
                newColors[i] = oldColors[rotations[i]];
            }
            chunk.Colors = new string(newColors);
            if (rotations[0] == 0)
            {
                chunk.Height ^= chunk.Depth;
                chunk.Depth ^= chunk.Height;
                chunk.Height ^= chunk.Depth;
            }
            else if (rotations[2] == 2)
            {
                chunk.Height ^= chunk.Width;
                chunk.Width ^= chunk.Height;
                chunk.Height ^= chunk.Width;
            }
            else if (rotations[4] == 4)
            {
                chunk.Width ^= chunk.Depth;
                chunk.Depth ^= chunk.Width;
                chunk.Width ^= chunk.Depth;
            }
        }
        public string Format(Location location)
        {
            if (location == null || rotation == null)
                return "";
            return string.Join(" ", rotation[0], rotation[2], location.X, location.Z, location.Y);
        }

        public bool IsBaseCorner(string colors)
        {
            return GetRotations().Any(e => e.Colors[0] == colors[0] && e.Colors[2] == colors[2] && e.Colors[4] == colors[4]);
        }
        public override bool Equals(object obj)
        {
            return obj is Chunk && Equals(obj as Chunk);
        }
        public override int GetHashCode()
        {
            return Width * 7 + Height * 19 + Depth * 167 + Colors.GetHashCode() * 5;
        }

        public bool Equals(Chunk other)
        {
            return Width == other.Width && Height == other.Height && Depth == other.Depth && Colors == other.Colors;
        }
    }
    class Location
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }
        public Location(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    class Grid
    {
        private class Edge
        {
            private int length;
            private int currentLength;
            private char color1, color2;
            private bool forward;
            private Chunk last;
            public LinkedList<int> Segments { get; private set; }
            public Edge(Chunk first, Chunk last, int length, char color1, char color2)
            {
                Segments = new LinkedList<int>();
                this.length = length;
                this.color1 = color1;
                this.color2 = color2;
                this.last = last;
                forward = first != null;
                TryAddChunk(forward ? first : last);
            }
            public bool TryAddChunk(Chunk chunk)
            {
                int chunkLength = chunk.EdgeLength(color1, color2);
                if (chunkLength <= 0)
                    return false;
                AddSegment(chunkLength);
                currentLength += chunkLength;
                return true;
            }
            public void Complete()
            {
                if (forward && last != null)
                {
                    int chunkLength = last.EdgeLength(color1, color2);
                    if (currentLength + chunkLength != length)
                    {
                        int lengthDelta = length - currentLength - chunkLength;
                        AddSegment(lengthDelta);
                        currentLength = length - chunkLength;
                    }
                    TryAddChunk(last);
                }
                if (currentLength != length)
                    AddSegment(length - currentLength);
            }
            private void AddSegment(int length)
            {
                if (forward)
                    Segments.AddLast(length);
                else
                    Segments.AddFirst(length);
            }
        }
        private Edge XEdge, YEdge, ZEdge;
        private CubeInfo info;
        private Dictionary<Chunk, LinkedList<Location>> chunksCells = new Dictionary<Chunk, LinkedList<Location>>();
        public Grid(CubeInfo info, Chunk baseCorner, IEnumerable<Chunk> corners, IEnumerable<Chunk> edgeChunks)
        {
            this.info = info;
            XEdge = new Edge(baseCorner, corners.FirstOrDefault(e => e.EdgeLength(info.Colors[2], info.Colors[4]) > 0), 
                info.Width, info.Colors[2], info.Colors[4]);
            YEdge = new Edge(baseCorner, corners.FirstOrDefault(e => e.EdgeLength(info.Colors[0], info.Colors[2]) > 0),
                info.Height, info.Colors[0], info.Colors[2]);
            ZEdge = new Edge(baseCorner, corners.FirstOrDefault(e => e.EdgeLength(info.Colors[0], info.Colors[4]) > 0),
                info.Depth, info.Colors[0], info.Colors[4]);

            Build(edgeChunks);

            XEdge.Complete();
            YEdge.Complete();
            ZEdge.Complete();

            BuildCells();
        }
        private void BuildCells()
        {
            int x = 0;
            foreach (int dx in XEdge.Segments)
            {
                int y = 0;
                foreach (int dy in YEdge.Segments)
                {
                    int z = 0;
                    foreach (int dz in ZEdge.Segments)
                    {
                        Location location = new Location(x, y, z);
                        Chunk chunk = new Chunk(dx, dz, dy, "" +
                            (x == 0 ? info.Colors[0] : '.') + (x + dx == info.Width ? info.Colors[1] : '.') +
                            (z == 0 ? info.Colors[2] : '.') + (z + dz == info.Depth ? info.Colors[3] : '.') +
                            (y == 0 ? info.Colors[4] : '.') + (y + dy == info.Height ? info.Colors[5] : '.'));
                        if (!chunksCells.ContainsKey(chunk))
                            chunksCells[chunk] = new LinkedList<Location>();
                        chunksCells[chunk].AddLast(location);
                        z += dz;
                    }
                    y += dy;
                }
                x += dx;
            }
        }
        private void Build(IEnumerable<Chunk> edgeChunks)
        {
            foreach (Chunk chunk in edgeChunks)
            {
                if (!XEdge.TryAddChunk(chunk))
                    if (!YEdge.TryAddChunk(chunk))
                        ZEdge.TryAddChunk(chunk);
            }
        }
        public Location Fit(Chunk chunk)
        {
            if (!chunksCells.ContainsKey(chunk) || chunksCells[chunk].Count == 0)
                return null;
            Location location = chunksCells[chunk].Last.Value;
            chunksCells[chunk].RemoveLast();
            return location;
        }
    }

    class Tokenizer
    {
        private TextReader reader;
        private string[] tokens;
        private int position = 0;
        public Tokenizer(TextReader reader)
        {
            this.reader = reader;
            tokens = reader.ReadToEnd().Split("\r\n ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }
        public string NextString()
        {
            return tokens[position++];
        }
        public int NextInt()
        {
            return int.Parse(NextString());
        }
        public Chunk NextChunk()
        {
            return new Chunk(NextInt(), NextInt(), NextInt(), NextString());
        }
        public CubeInfo NextCubeInfo()
        {
            return new CubeInfo(NextInt(), NextInt(), NextInt(), NextString());
        }
    }
    class CubeInfo
    {
        public int Width { get; private set; }
        public int Depth { get; private set; }
        public int Height { get; private set; }
        public string Colors { get; private set; }
        public CubeInfo(int width, int depth, int height, string colors)
        {
            Width = width; Height = height; Depth = depth; Colors = colors;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Console.SetIn(new StreamReader("..\\..\\input.txt"));
            //Console.SetOut(new StreamWriter("..\\..\\output.txt"));

            var tokenizer = new Tokenizer(Console.In);
            CubeInfo info = tokenizer.NextCubeInfo();
            int count = tokenizer.NextInt();

            List<Chunk> chunks = new List<Chunk>(count), corners = new List<Chunk>();
            Chunk baseCorner = null;
            foreach (Chunk chunk in ReadChunks(tokenizer, count))
            {
                chunks.Add(chunk);
                if (baseCorner == null && chunk.IsBaseCorner(info.Colors))
                    baseCorner = chunk;
                else if (chunk.IsCorner)
                    corners.Add(chunk);
            }

            Grid grid = new Grid(info, baseCorner, corners,  chunks.Where(e => e.IsEdge && !e.IsCorner));

            foreach (Chunk chunk in chunks)
            {
                var fit = chunk.GetRotations().Select(e => new { Location = grid.Fit(e), Chunk = e })
                    .FirstOrDefault(e => e.Location != null);
                Console.WriteLine(fit == null ? "" : fit.Chunk.Format(fit.Location));
            }

            Console.Out.Close();
        }
        static IEnumerable<Chunk> ReadChunks(Tokenizer tokenizer, int count)
        {
            for (int i = 0; i < count; i++)
                yield return tokenizer.NextChunk();
        }
    }
}
