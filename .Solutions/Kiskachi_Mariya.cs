using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tim1843
{
    internal static class Reader
    {
        private static string TempStr;
        private static string[] TempArgs;

        public static Figure ReadFigure()
        {
            TempStr = Console.ReadLine();
            TempArgs = TempStr.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            return
                new Figure(new Dot(int.Parse(TempArgs[0]), int.Parse(TempArgs[1]), int.Parse(TempArgs[2])), TempArgs[3]);
        }

        public static List<Figure> ReadFigureList()
        {
            int count = int.Parse(Console.ReadLine());
            List<Figure> figures = new List<Figure>();
            for (int i = 0; i < count; i++)
            {
                figures.Add(Reader.ReadFigure());
            }
            return figures;
        }
    }

    internal class Dot
    {
        public int X;
        public int Y;
        public int Z;
        public Dictionary<char, int> DotDict { get; private set; }

        public Dot(int x = -1, int y = -1, int z = -1)
        {
            Set(x, y, z);
            DotDict = new Dictionary<char, int>();
            DotDict['X'] = x;
            DotDict['Y'] = y;
            DotDict['Z'] = z;
        }

        public void Set(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
       
        public new string ToString()
        {
            return "" + X + Y + Z;
        }

        public bool Compare(int x, int y, int z)
        {
            if ((x == X) && (y == Y) && (z == Z))
                return true;
            return false;
        }
    }

    internal class Orientator
    {
        public Dictionary<string, Tuple<string, string>> Orientation { get; private set; }

        public Orientator(Figure paral)
        {
            var Orient = new Dictionary<string, string>();
            string R = paral.Color[0].ToString(), O = paral.Color[1].ToString(), Y = paral.Color[2].ToString(), G = paral.Color[3].ToString(), B = paral.Color[4].ToString(), V = paral.Color[5].ToString();

            foreach (string str in new List<string> { (R + O + Y + G + B + V), (R + O + G + Y + V + B), (O + R + Y + G + V + B), (O + R + G + Y + B + V)}) 
                Orient[str] = "XYZ";
            foreach (String str in new List<string> { (R + O + V + B + Y + G), (R + O + B + V + G + Y), (O + R + B + V + Y + G), (O + R + V + B + G + Y)})
                Orient[str] = "XZY";
            foreach (String str in new List<string> { (Y + G + O + R + B + V), (Y + G + R + O + V + B), (G + Y + O + R + V + B), (G + Y + R + O + B + V)})
                Orient[str] = "YXZ";
            foreach (String str in new List<string> { (Y + G + V + B + O + R), (Y + G + B + V + R + O), (G + Y + B + V + O + R), (G + Y + V + B + R + O)})
                Orient[str] = "ZXY";
            foreach (String str in new List<string> { (B + V + Y + G + O + R), (B + V + G + Y + R + O), (V + B + Y + G + R + O), (V + B + G + Y + O + R)})
                Orient[str] = "ZYX";
            foreach (String str in new List<string> { (B + V + R + O + Y + G), (B + V + O + R + G + Y), (V + B + O + R + Y + G), (V + B + R + O + G + Y)})
                Orient[str] = "YZX";
            
            Orientation = new Dictionary<string, Tuple<string, string>>();
            string pattern = "FBDULR";
            foreach (string key in Orient.Keys)
            {
                char [] sides = new char[2];
                for (int i = 0; i < 6; i++)
                {
                    if (key[i] == paral.Color[0])
                        sides[0] = pattern[i];
                    if (key[i] == paral.Color[2])
                        sides[1] = pattern[i];
                }

                Orientation[key] = new Tuple<string, string>(Orient[key], sides[0].ToString() + sides[1].ToString());

                StringBuilder skips = new StringBuilder(key); 
                for (int i = 0; i < skips.Length; i++)
                {
                    skips[i] = '.';
                    for (int j = i; j < skips.Length; j++)
                    {
                        skips[j] = '.';
                        Orientation[skips.ToString()] = new Tuple<string, string>(Orient[key],
                                                                       sides[0].ToString() + sides[1].ToString());
                        if(i != j)
                            skips[j] = key[j];
                    }
                    skips[i] = key[i];
                }
            }
        }
    }

    internal class Figure
    {
        public Dot Size { get; private set; }
        public string Color { get; private set; }
        public Dot Location;
        public List<char> InsideColor { get; private set; }
        public Dot CorrectOrientation { get; private set; }
        public String Sides { get; private set; }

        public Figure(Dot size, string color)
        {
            Size = size;
            Color = color;
            Location = new Dot();
            CorrectOrientation = new Dot(0,0,0);
            
            InsideColor = new List<char>();
            foreach (char i in Color)
                if (i != '.')
                    InsideColor.Add(i);
        }

        public void FindOrientation(Orientator dict)
        {
            CorrectOrientation = new Dot(Size.DotDict[dict.Orientation[Color].Item1[0]],
                                         Size.DotDict[dict.Orientation[Color].Item1[1]],
                                         Size.DotDict[dict.Orientation[Color].Item1[2]]);
            Sides = dict.Orientation[Color].Item2;
        }

       

        public void TryPack(Figure paral)
        {
            if (InsideColor.Contains(paral.Color[0])) //FrontAngles
            {
                if (InsideColor.Contains(paral.Color[2]))
                {
                    if (InsideColor.Contains(paral.Color[4]))
                        Location.Set(0, 0, 0);
                    if (InsideColor.Contains(paral.Color[5]))
                        Location.Set(0, 0, paral.Size.Z - CorrectOrientation.Z);
                }
                if (InsideColor.Contains(paral.Color[3]))
                {
                    if (InsideColor.Contains(paral.Color[4]))
                        Location.Set(0, paral.Size.Y - CorrectOrientation.Y, 0);
                    if (InsideColor.Contains(paral.Color[5]))
                        Location.Set(0, paral.Size.Y - CorrectOrientation.Y, paral.Size.Z - CorrectOrientation.Z);
                }
            }

            if (InsideColor.Contains(paral.Color[1])) //BackAngles
            {
                if (InsideColor.Contains(paral.Color[2]))
                {
                    if (InsideColor.Contains(paral.Color[4]))
                        Location.Set(paral.Size.X - CorrectOrientation.X, 0, 0);
                    if (InsideColor.Contains(paral.Color[5]))
                        Location.Set(paral.Size.X - CorrectOrientation.X, 0, paral.Size.X - CorrectOrientation.X);
                }
                if (InsideColor.Contains(paral.Color[3]))
                {
                    if (InsideColor.Contains(paral.Color[4]))
                        Location.Set(paral.Size.X - CorrectOrientation.X, paral.Size.Y - CorrectOrientation.Y, 0);
                    if (InsideColor.Contains(paral.Color[5]))
                        Location.Set(paral.Size.X - CorrectOrientation.X, paral.Size.Y - CorrectOrientation.Y,
                                     paral.Size.Z - CorrectOrientation.Z);
                }
            }
        }

        public void PackFront(List<Figure> figures, Figure paral)
        {
            List<Figure> frontFigure = new List<Figure>();
            Dot nextDot = new Dot(0, 0, 0);
            foreach (Figure f in figures)
            {
                if (f.InsideColor.Contains(paral.Color[0]))
                {
                    if (f.Location.Compare(0, 0, 0))
                    {
                        if ((paral.Size.X == f.CorrectOrientation.X) && (paral.Size.Y == f.CorrectOrientation.Y))
                            nextDot = new Dot(0, 0, f.CorrectOrientation.Z);
                        if ((paral.Size.X == f.CorrectOrientation.X) && (paral.Size.Z == f.CorrectOrientation.Z))
                            nextDot = new Dot(0, f.CorrectOrientation.Y, 0);
                        if ((paral.Size.Y == f.CorrectOrientation.Y) && (paral.Size.Z == f.CorrectOrientation.Z))
                        {
                            nextDot = new Dot(f.CorrectOrientation.X, 0, 0);
                            PackSide(figures, nextDot);
                            return;
                        }
                    }
                    else
                        if(f.Location.Compare(-1,-1,-1))
                            frontFigure.Add(f);
                }
            }
            foreach (Figure f in frontFigure)
            {
                if (nextDot.Z != 0)
                {
                    f.Location.Set(nextDot.X, nextDot.Y, nextDot.Z);
                    nextDot.Z += f.CorrectOrientation.Z;
                }
                if (nextDot.Y != 0)
                {
                    f.Location.Set(nextDot.X, nextDot.Y, nextDot.Z);
                    nextDot.Y += f.CorrectOrientation.Y;
                }
            }

        }

        private void PackSide(List<Figure> figures, Dot nextDot)//Запихнуть сюда все координаты.
        {
            foreach (Figure f in figures)
            {
                if (f.Location.Compare(-1,-1,-1))
                {
                    f.Location.Set(nextDot.X, nextDot.Y, nextDot.Z);
                    nextDot.X += f.CorrectOrientation.X;
                }

            }
        }
    }
        class Program
        {
            private static void Main(string[] args)
            {

                Figure Parall = Reader.ReadFigure();
                List<Figure> Figures = Reader.ReadFigureList();

                Orientator dict = new Orientator(Parall);

                foreach (Figure f in Figures)
                {
                    f.FindOrientation(dict);
                    f.TryPack(Parall);
                }

                Figures[0].PackFront(Figures, Parall);

                foreach(Figure f in Figures)
                      Console.WriteLine(f.Sides[0] + " " + f.Sides[1] + " " + f.Location.X + " " + f.Location.Y + " " + f.Location.Z);
            }
        }

}