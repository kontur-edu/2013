//Rozhin Sergey. URFU, IRIT-RTF. rammenfluss@gmail.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParPiped
{
    public enum Color : byte
    {
        No = 0,
        R,
        O,
        Y,
        G,
        B,
        V
    }
    public enum Side : byte
    {
        //символ ЂFї, задней Ч ЂBї, нижней Ч ЂDї, верхней Ч ЂUї, левой Ч ЂLї, правой Ч ЂRї
        F = 0,
        B,
        D,
        U,
        L,
        R
    }

    public class Verges
    {

        public readonly Color[] Vrgs = new Color[6];
        public Verges(string colours)
        {
            if (colours.Length != 6)
            {
                throw new ArgumentOutOfRangeException();
            }
            for (int i = 0; i < 6; i++)
                Vrgs[i] = ColorEnumFromChar(colours[i]);
        }
        public int GetIndexOfColor(Color c)
        {
            for (int i = 0; i < Vrgs.Length; i++)
                if (Vrgs[i] == c)
                    return i;
            for (int i = 0; i < Vrgs.Length; i++)
                if (Vrgs[i] == Color.No)
                    return i;
            throw new ArgumentOutOfRangeException("No such color");
        }

        private static Color ColorEnumFromChar(char color)
        {
            Color newColor;
            if (Color.TryParse(color.ToString(), true, out newColor))
            {
                return newColor;
            }
            return Color.No;
        }
    }

    public class Placement
    {
        public Side Forward;
        public Side Down;
        public int ForwardDownLeftX = 0;
        public int ForwardDownLeftY = 0;
        public int ForwardDownLeftZ = 0;
        public Placement(int x, int y, int z, Side forward, Side down)
        {
            ForwardDownLeftX = x;
            ForwardDownLeftY = y;
            ForwardDownLeftZ = z;
            Forward = forward;
            Down = down;
        }
        public Placement(int x, int y, int z)
        {
            ForwardDownLeftX = x;
            ForwardDownLeftY = y;
            ForwardDownLeftZ = z;
        }
        public new string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4}", Forward, Down, ForwardDownLeftX, ForwardDownLeftY,
                                 ForwardDownLeftZ);
        }
    }

    public class Piece
    {
        public short W;//F - B distance
        public short D;//D - U distance
        public short H;//L - R distance
        public Verges Verges;
        public Placement Placement;
        public Piece(short w, short d, short h, Verges verges)
        {
            W = w;
            D = d;
            H = h;
            Verges = verges;
            Placement = new Placement(0, 0, 0);
        }

        protected Piece()
        {
            W = 0;
            D = 0;
            H = 0;
            Verges = new Verges("ROYGBV");
        }

        //return distance between two(one) non-coloured sides
        public int GetStep()
        {
            int index = Verges.GetIndexOfColor(Color.No);
            if (index < 2)
            {
                return W;
            }
            if (index < 4)
            {
                return D;
            }
            return H;
        }
        public void SetForward(Color c)
        {
            Placement.Forward = (Side)Verges.GetIndexOfColor(c);
        }
        public void SetDown(Color c)
        {
            Placement.Down = (Side)Verges.GetIndexOfColor(c);
        }
        public bool IsEqualindexes(Color c, Side s)
        {
            return Verges.GetIndexOfColor(c) == (int)s;
        }
    }

    public class BaseParPiped : Piece
    {
        public BaseParPiped(short w, short d, short h, Verges verges)
        {
            W = w;
            D = d;
            H = h;
            Verges = verges;
            Placement = new Placement(0, 0, 0);
        }

        public Color GetColor(Side s)
        {
            return Verges.Vrgs[(int)s];
        }
    }

    public class PiecesContainer
    {
        private List<Piece> _pieces;
        private BaseParPiped _basePP;
        public PiecesContainer(int piecesCount, BaseParPiped basePp)
        {
            _basePP = basePp;
            _pieces = new List<Piece>(piecesCount);
        }

        private void Sort()
        {
            var tmpPieces = new List<Piece>(_pieces);
            var p = (from piece in tmpPieces
                     where piece.Verges.Vrgs.Count(color => color == Color.No) == 1
                     select piece).First();
            //making decision about direction of slices
            int index = p.Verges.GetIndexOfColor(Color.No);
            index = _basePP.Verges.GetIndexOfColor(p.Verges.Vrgs[index ^ 1]);
            if (index < 2)
            {
                SortX(tmpPieces);
                return;
            }
            if (index < 4)
            {
                SortY(tmpPieces);
                return;
            }
            SortZ(tmpPieces);
        }
        private void SortX(List<Piece> tmpPieces)
        {
            var pieces = (from piece in tmpPieces
                          where piece.Verges.Vrgs.Count(color => color == Color.No) == 1
                          select piece).ToArray();
            Piece pFront = pieces.First(c => c.Verges.Vrgs.Contains(_basePP.GetColor(Side.F)));
            Piece pBack = pieces.First(c => c.Verges.Vrgs.Contains(_basePP.GetColor(Side.B)));

            pFront.Placement = new Placement(0, 0, 0);
            pBack.Placement = new Placement(_basePP.W - pBack.GetStep(), 0, 0);
            foreach (var p in pieces)
            {
                tmpPieces.Remove(p);
                p.SetForward(_basePP.GetColor(Side.F));
                p.SetDown(_basePP.GetColor(Side.D));
            }
            int currentX = pFront.GetStep();
            foreach (Piece t in tmpPieces)
            {
                t.Placement = new Placement(currentX, 0, 0);
                int nonColoredIndex = t.Verges.GetIndexOfColor(Color.No);
                t.SetDown(_basePP.GetColor(Side.D));
                currentX += t.GetStep();
                //choosing right side to forward
                if (nonColoredIndex < 2)
                {
                    if ((t.IsEqualindexes(_basePP.GetColor(Side.U), Side.U) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.L), Side.L)) ||
                        (t.IsEqualindexes(_basePP.GetColor(Side.U), Side.D) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.L), Side.R)))
                    {
                        t.Placement.Forward = Side.F;
                        continue;
                    }
                    if ((t.IsEqualindexes(_basePP.GetColor(Side.D), Side.L) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.L), Side.U)) ||
                        (t.IsEqualindexes(_basePP.GetColor(Side.D), Side.R) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.R), Side.U)))
                    {
                        t.Placement.Forward = Side.F;
                        continue;
                    }
                    t.Placement.Forward = Side.B;
                    continue;
                }
                if ((nonColoredIndex < 4) && ((nonColoredIndex > 1)))
                {
                    if ((t.IsEqualindexes(_basePP.GetColor(Side.L), Side.F) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.U), Side.L)) ||
                        (t.IsEqualindexes(_basePP.GetColor(Side.L), Side.B) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.U), Side.R)))
                    {
                        t.Placement.Forward = Side.U;
                        continue;
                    }
                    if ((t.IsEqualindexes(_basePP.GetColor(Side.U), Side.F) &&
                              t.IsEqualindexes(_basePP.GetColor(Side.L), Side.R)) ||
                             (t.IsEqualindexes(_basePP.GetColor(Side.U), Side.B) &&
                              t.IsEqualindexes(_basePP.GetColor(Side.L), Side.L)))
                    {
                        t.Placement.Forward = Side.U;
                        continue;
                    }
                    t.Placement.Forward = Side.D;
                    continue;
                }
                if (nonColoredIndex > 3)
                {
                    if ((t.IsEqualindexes(_basePP.GetColor(Side.U), Side.U) &&
                              t.IsEqualindexes(_basePP.GetColor(Side.L), Side.B)) ||
                             (t.IsEqualindexes(_basePP.GetColor(Side.U), Side.D) &&
                              t.IsEqualindexes(_basePP.GetColor(Side.L), Side.F)))
                    {
                        t.Placement.Forward = Side.L;
                        continue;
                    }
                    if ((t.IsEqualindexes(_basePP.GetColor(Side.U), Side.F) &&
                              t.IsEqualindexes(_basePP.GetColor(Side.L), Side.U)) ||
                             (t.IsEqualindexes(_basePP.GetColor(Side.U), Side.B) &&
                              t.IsEqualindexes(_basePP.GetColor(Side.L), Side.D)))
                    {
                        t.Placement.Forward = Side.L;
                        continue;
                    }
                    t.Placement.Forward = Side.R;
                }
            }
        }

        private void SortY(List<Piece> tmpPieces)
        {
            var pieces = (from piece in tmpPieces
                          where piece.Verges.Vrgs.Count(color => color == Color.No) == 1
                          select piece).ToArray();
            //subdivide by color 
            Piece pDown = pieces.First(c => c.Verges.Vrgs.Contains(_basePP.GetColor(Side.D)));
            Piece pUp = pieces.First(c => c.Verges.Vrgs.Contains(_basePP.GetColor(Side.U)));
            pDown.Placement = new Placement(0, 0, 0);
            pUp.Placement = new Placement(0, _basePP.D - pUp.GetStep(), 0);
            foreach (var p in pieces)
            {
                tmpPieces.Remove(p);
                p.SetForward(_basePP.GetColor(Side.F));
                p.SetDown(_basePP.GetColor(Side.D));
            }
            int currentY = pDown.GetStep();
            foreach (Piece t in tmpPieces)
            {
                t.Placement = new Placement(0, currentY, 0);
                t.SetForward(_basePP.GetColor(Side.F));
                currentY += t.GetStep();
                int nonColoredIndex = t.Verges.GetIndexOfColor(Color.No);
                //choosing right side to down
                if (nonColoredIndex < 2)
                {
                    if ((t.IsEqualindexes(_basePP.GetColor(Side.L), Side.L) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.F), Side.D)) ||
                        (t.IsEqualindexes(_basePP.GetColor(Side.L), Side.R) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.F), Side.U)))
                    {
                        t.Placement.Down = Side.B;
                        continue;
                    }
                    if ((t.IsEqualindexes(_basePP.GetColor(Side.L), Side.U) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.F), Side.L)) ||
                        (t.IsEqualindexes(_basePP.GetColor(Side.L), Side.D) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.F), Side.R)))
                    {
                        t.Placement.Down = Side.B;
                        continue;
                    }
                    t.Placement.Down = Side.F;
                    continue;
                }
                if ((nonColoredIndex < 4) && ((nonColoredIndex > 1)))
                {
                    if ((t.IsEqualindexes(_basePP.GetColor(Side.L), Side.L) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.F), Side.F)) ||
                        (t.IsEqualindexes(_basePP.GetColor(Side.L), Side.R) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.F), Side.B)))
                    {
                        t.Placement.Down = Side.D;
                        continue;
                    }
                    if ((t.IsEqualindexes(_basePP.GetColor(Side.L), Side.F) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.F), Side.R)) ||
                        (t.IsEqualindexes(_basePP.GetColor(Side.L), Side.B) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.F), Side.L)))
                    {
                        t.Placement.Down = Side.D;
                        continue;
                    }
                    t.Placement.Down = Side.U;
                    continue;
                }
                if ((nonColoredIndex > 3))
                {
                    if ((t.IsEqualindexes(_basePP.GetColor(Side.L), Side.F) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.B), Side.U)) ||
                        (t.IsEqualindexes(_basePP.GetColor(Side.F), Side.F) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.L), Side.U)))
                    {
                        t.Placement.Down = Side.L;
                        continue;
                    }
                    if ((t.IsEqualindexes(_basePP.GetColor(Side.R), Side.F) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.F), Side.U)) ||
                        (t.IsEqualindexes(_basePP.GetColor(Side.B), Side.F) &&
                         t.IsEqualindexes(_basePP.GetColor(Side.R), Side.U)))
                    {
                        t.Placement.Down = Side.L;
                        continue;
                    }
                    t.Placement.Down = Side.R;
                }
            }
        }

        private void SortZ(List<Piece> tmpPieces)
        {
            var pieces = (from piece in tmpPieces
                          where piece.Verges.Vrgs.Count(color => color == Color.No) == 1
                          select piece).ToArray();
            //subdivide by color 
            Piece pLeft = pieces.First(c => c.Verges.Vrgs.Contains(_basePP.GetColor(Side.L)));
            Piece pRight = pieces.First(c => c.Verges.Vrgs.Contains(_basePP.GetColor(Side.R)));
            pLeft.Placement = new Placement(0, 0, 0);
            pRight.Placement = new Placement(0, 0, _basePP.H - pRight.GetStep());


            foreach (var p in pieces)
            {
                tmpPieces.Remove(p);
                p.SetForward(_basePP.GetColor(Side.F));
                p.SetDown(_basePP.GetColor(Side.D));
            }
            int currentZ = pLeft.GetStep();
            foreach (Piece t in tmpPieces)
            {
                t.Placement = new Placement(0, 0, currentZ);
                t.SetForward(_basePP.GetColor(Side.F));
                t.SetDown(_basePP.GetColor(Side.D));
                currentZ += t.GetStep();
            }

        }
        public void AddPiece(string inString)
        {
            string[] tokens = inString.Split(' ');
            _pieces.Add(new Piece(short.Parse(tokens[0]), short.Parse(tokens[1]), short.Parse(tokens[2]), new Verges(tokens[3])));
        }

        public string Answere()
        {
            if (_pieces.Count == 1)
                return PLacementForSingle();
            Sort();
            StringBuilder stringBuilder = new StringBuilder("");
            foreach (var piece in _pieces)
            {
                stringBuilder.Append(piece.Placement.ToString() + "\n");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }
        private string PLacementForSingle()
        {
            _pieces[0].Placement = new Placement(0, 0, 0);
            _pieces[0].SetForward(_basePP.Verges.Vrgs[(int)Side.F]);
            _pieces[0].SetDown(_basePP.Verges.Vrgs[(int)Side.D]);
            return _pieces[0].Placement.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string line = Console.ReadLine();
            if (String.IsNullOrEmpty(line))
            {
                throw new ArgumentOutOfRangeException("No input!");
            }
            string[] tokens = line.Split(' ');
            BaseParPiped a = new BaseParPiped(short.Parse(tokens[0]), short.Parse(tokens[1]), short.Parse(tokens[2]), new Verges(tokens[3]));
            line = Console.ReadLine();
            int piecesCount = int.Parse(line);
            PiecesContainer PK = new PiecesContainer(piecesCount, a);
            if (piecesCount == 0)
            {
                return;
            }
            for (int i = 0; i < piecesCount; i++)
            {
                PK.AddPiece(Console.ReadLine());
            }
            Console.Write(PK.Answere());
        }
    }

}
