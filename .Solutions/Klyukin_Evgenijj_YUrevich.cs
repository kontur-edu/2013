using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paralepiped
{
    class Paralepiped
    {
        private int w, d, h;
        private char[] color = new char[6];
        private char[] F = new char[6];
        private int x, y, z;
        private int N;

        public Paralepiped() { }
        public Paralepiped(int w1, int d1, int h1, char[] color1, int N1)
        {
            w = w1;
            d = d1;
            h = h1;
            for (int i = 0; i < 6; i++)
            {
                color[i] = color1[i];
            }
            N = N1;
            F[0] = 'F';
            F[1] = 'B';
            F[2] = 'D';
            F[3] = 'U';
            F[4] = 'L';
            F[5] = 'R';
        }
        public override bool Equals(object obj)
        {
            Paralepiped P = (Paralepiped)obj;
            if (N == P.N)
                return true;
            return false;
        }
        public override int GetHashCode()
        {
            return N;
        }
        public string Print()
        {
            return F[0].ToString() + ' ' + F[2].ToString() + ' ' + x.ToString() + ' ' + y.ToString() + ' ' + z.ToString();
        }
        public int Ncolor(char c)
        {
            //проверяет есть ли этот цвет в паралепипеде
            for (int i = 0; i < 6; i++)
                if (c == color[i]) return i + 1;
            return 0;
        }
        public bool TestSize(int w1, int d1, int h1)
        {
            //проверяет удвлетворяет ли размерам паралепипед
            if (w1 == w && d1 == d && h1 == h) return true;
            if (w1 == w && d1 == h && h1 == d) return true;
            if (w1 == d && d1 == w && h1 == h) return true;
            if (w1 == d && d1 == h && h1 == w) return true;
            if (w1 == h && d1 == w && h1 == d) return true;
            if (w1 == h && d1 == d && h1 == w) return true;
            return false;
        }
        public bool TestColor(int n, char[] c)
        {
            //проверяет есть ли цвета в паралепипеде, n - колличество цветов
            for (int i = 0; i < n; i++)
                if (Ncolor(c[i]) == 0) return false;
            return true;
        }
        public char[] GetColor()
        {
            return color;
        }
        public void SetXYZ(int x1, int y1, int z1)
        {
            x = x1;
            y = y1;
            z = z1;
        }
        public int GetW()
        {
            return w;
        }
        public int GetD()
        {
            return d;
        }
        public int GetH()
        {
            return h;
        }
        public int GetN()
        {
            return N;
        }
        public void TurnVertical()
        {
            //поворачивает паралепипед вокруг оси Z
            int k = h;
            h = w;
            w = k;
            char c = color[0];
            color[0] = color[5];
            color[5] = color[1];
            color[1] = color[4];
            color[4] = c;
            c = F[0];
            F[0] = F[5];
            F[5] = F[1];
            F[1] = F[4];
            F[4] = c;
        }
        public void TurnHorisontal()
        {
            //поворачивает паралепипед вокруг оси X
            int k = h;
            h = d;
            d = k;
            char c = color[5];
            color[5] = color[3];
            color[3] = color[4];
            color[4] = color[2];
            color[2] = c;
            c = F[5];
            F[5] = F[3];
            F[3] = F[4];
            F[4] = F[2];
            F[2] = c;
        }
        public void TurnHorisontal1()
        {
            //поворачивает паралепипед вокруг оси Y
            int k = w;
            w = d;
            d = k;
            char c = color[0];
            color[0] = color[3];
            color[3] = color[1];
            color[1] = color[2];
            color[2] = c;
            c = F[0];
            F[0] = F[3];
            F[3] = F[1];
            F[1] = F[2];
            F[2] = c;
        }

    }
    class Program
    {
        static int ReadConsole(ref Paralepiped FulP, ISet<Paralepiped> SetP)
        {
            int N;
            string str = Console.ReadLine();
            string[] parse = str.Split(new char[] { ' ' });
            FulP = new Paralepiped(int.Parse(parse[0]), int.Parse(parse[1]), int.Parse(parse[2]), parse[3].ToCharArray(0, 6), 0);
            str = Console.ReadLine();
            N = int.Parse(str);
            for (int i = 0; i < N; i++)
            {
                str = Console.ReadLine();
                parse = str.Split(new char[] { ' ' });
                SetP.Add(new Paralepiped(int.Parse(parse[0]), int.Parse(parse[1]), int.Parse(parse[2]), parse[3].ToCharArray(0, 6), i + 1));
            }
            return N;
        }
        static void WriteConsole(ISet<Paralepiped> Set, int N)
        {
            for (int i = 0; i < N; i++)
                foreach (Paralepiped P in Set)
                    if (P.GetN() == i + 1)
                        Console.WriteLine(P.Print());
        }
        static bool Turn3(int i, int j, int k, Paralepiped P, char[] color)
        {
            //проверяет на наличие 3 цветов и поворачивает относительно их
            if (P.TestColor(3, new char[] { color[i], color[j], color[k] }) == true)
            {
                if (P.GetColor()[3] == color[i] || P.GetColor()[2] == color[i]) P.TurnHorisontal();
                while (P.GetColor()[i] != color[i])
                    P.TurnVertical();
                while (P.GetColor()[j] != color[j])
                    P.TurnHorisontal();

                return true;
            }
            return false;
        }
        static bool Turn2(int i, int j, Paralepiped P, char[] color)
        {
            //проверяет на наличие 2 цветов и поворачивает относительно их
            if (P.TestColor(2, new char[] { color[i], color[j] }) == true)
                if (i == 0 || i == 1)
                {
                    if (P.GetColor()[3] == color[i] || P.GetColor()[2] == color[i])
                        P.TurnHorisontal();
                    while (P.GetColor()[i] != color[i])
                        P.TurnVertical();
                    while (P.GetColor()[j] != color[j])
                        P.TurnHorisontal();
                    return true;
                }
                else
                {
                    if (P.GetColor()[0] == color[i] || P.GetColor()[1] == color[i])
                        P.TurnVertical();
                    while (P.GetColor()[i] != color[i])
                        P.TurnHorisontal();
                    while (P.GetColor()[j] != color[j])
                        P.TurnVertical();
                    return true;
                }
            return false;
        }
        static bool Turn1(int i, Paralepiped P, char[] color)
        {
            //проверяет на наличие 1 цвета и поворачивает относительно его
            if (P.TestColor(1, new char[] { color[i] }) == true)
                if (i == 0 || i == 1)
                {
                    if (P.GetColor()[3] == color[i] || P.GetColor()[2] == color[i])
                        P.TurnHorisontal();
                    while (P.GetColor()[i] != color[i])
                        P.TurnVertical();
                    return true;
                }
                else
                {
                    if (P.GetColor()[0] == color[i] || P.GetColor()[1] == color[i])
                        P.TurnVertical();
                    while (P.GetColor()[i] != color[i])
                        P.TurnHorisontal();

                    return true;
                }
            return false;
        }
        static void U2(int[] w_mas, int[] d_mas, int[] h_mas, ISet<Paralepiped> SetP, ISet<Paralepiped> SetF, char[] color)
        {

            //Устанавливает все паралепипеды с 2 цветами
            for (int i = 1; i < w_mas.Length - 1; i++)
                foreach (Paralepiped P in SetP)
                    if (Turn2(2, 4, P, color) == true)
                    {
                        SetP.Remove(P);
                        P.SetXYZ(w_mas[i - 1], 0, 0);
                        w_mas[i] = P.GetW() + w_mas[i - 1];
                        SetF.Add(P);
                        break;
                    }
            for (int i = 1; i < d_mas.Length - 1; i++)
                foreach (Paralepiped P in SetP)
                    if (Turn2(0, 4, P, color) == true)
                    {
                        SetP.Remove(P);
                        P.SetXYZ(0, d_mas[i - 1], 0);
                        d_mas[i] = P.GetD() + d_mas[i - 1];
                        SetF.Add(P);
                        break;
                    }
            for (int i = 1; i < h_mas.Length - 1; i++)
                foreach (Paralepiped P in SetP)
                    if (Turn2(0, 2, P, color) == true)
                    {
                        SetP.Remove(P);
                        P.SetXYZ(0, 0, h_mas[i - 1]);
                        h_mas[i] = P.GetH() + h_mas[i - 1];
                        SetF.Add(P);
                        break;
                    }
            //--------------------------------------------------------------------------------------
            for (int i = 1; i < w_mas.Length - 1; i++)
                foreach (Paralepiped P in SetP)
                    if (Turn2(2, 5, P, color) == true && P.GetW() == w_mas[i] - w_mas[i - 1])
                    {
                        SetP.Remove(P);
                        P.SetXYZ(w_mas[i - 1], 0, (h_mas.Length < 2) ? 0 : h_mas[h_mas.Length - 2]);
                        SetF.Add(P);
                        break;
                    }
            for (int i = 1; i < w_mas.Length - 1; i++)
                foreach (Paralepiped P in SetP)
                    if (Turn2(3, 4, P, color) == true && P.GetW() == w_mas[i] - w_mas[i - 1])
                    {
                        SetP.Remove(P);
                        P.SetXYZ(w_mas[i - 1], (d_mas.Length < 2) ? 0 : d_mas[d_mas.Length - 2], 0);
                        SetF.Add(P);
                        break;
                    }
            for (int i = 1; i < w_mas.Length - 1; i++)
                foreach (Paralepiped P in SetP)
                    if (Turn2(3, 5, P, color) == true && P.GetW() == w_mas[i] - w_mas[i - 1])
                    {
                        SetP.Remove(P);
                        P.SetXYZ(w_mas[i - 1], (d_mas.Length < 2) ? 0 : d_mas[d_mas.Length - 2], (h_mas.Length < 2) ? 0 : h_mas[h_mas.Length - 2]);
                        SetF.Add(P);
                        break;
                    }
            //--------------------------------------------------------------------------------------------------------------------
            for (int i = 1; i < d_mas.Length - 1; i++)
                foreach (Paralepiped P in SetP)
                    if (Turn2(0, 5, P, color) == true && P.GetD() == d_mas[i] - d_mas[i - 1])
                    {
                        SetP.Remove(P);
                        P.SetXYZ(0, d_mas[i - 1], (h_mas.Length < 2) ? 0 : h_mas[h_mas.Length - 2]);
                        SetF.Add(P);
                        break;
                    }
            for (int i = 1; i < d_mas.Length - 1; i++)
                foreach (Paralepiped P in SetP)
                    if (Turn2(1, 4, P, color) == true && P.GetD() == d_mas[i] - d_mas[i - 1])
                    {
                        SetP.Remove(P);
                        P.SetXYZ((w_mas.Length < 2) ? 0 : w_mas[w_mas.Length - 2], d_mas[i - 1], 0);
                        SetF.Add(P);
                        break;
                    }
            for (int i = 1; i < d_mas.Length - 1; i++)
                foreach (Paralepiped P in SetP)
                    if (Turn2(1, 5, P, color) == true && P.GetD() == d_mas[i] - d_mas[i - 1])
                    {
                        SetP.Remove(P);
                        P.SetXYZ((w_mas.Length < 2) ? 0 : w_mas[w_mas.Length - 2], d_mas[i - 1], (h_mas.Length < 2) ? 0 : h_mas[h_mas.Length - 2]);
                        SetF.Add(P);
                        break;
                    }
            //----------------------------------------------------------------------------------------------------------------------
            for (int i = 1; i < h_mas.Length - 1; i++)
                foreach (Paralepiped P in SetP)
                    if (Turn2(0, 3, P, color) == true && P.GetH() == h_mas[i] - h_mas[i - 1])
                    {
                        SetP.Remove(P);
                        P.SetXYZ(0, (d_mas.Length < 2) ? 0 : d_mas[d_mas.Length - 2], h_mas[i - 1]);
                        SetF.Add(P);
                        break;
                    }
            for (int i = 1; i < h_mas.Length - 1; i++)
                foreach (Paralepiped P in SetP)
                    if (Turn2(1, 2, P, color) == true && P.GetH() == h_mas[i] - h_mas[i - 1])
                    {
                        SetP.Remove(P);
                        P.SetXYZ((w_mas.Length < 2) ? 0 : w_mas[w_mas.Length - 2], 0, h_mas[i - 1]);
                        SetF.Add(P);
                        break;
                    }
            for (int i = 1; i < h_mas.Length - 1; i++)
                foreach (Paralepiped P in SetP)
                    if (Turn2(1, 3, P, color) == true && P.GetH() == h_mas[i] - h_mas[i - 1])
                    {
                        SetP.Remove(P);
                        P.SetXYZ((w_mas.Length < 2) ? 0 : w_mas[w_mas.Length - 2], (d_mas.Length < 2) ? 0 : d_mas[d_mas.Length - 2], h_mas[i - 1]);
                        SetF.Add(P);
                        break;
                    }
        }
        static void U1(int[] w_mas, int[] d_mas, int[] h_mas, ISet<Paralepiped> SetP, ISet<Paralepiped> SetF, char[] color)
        {
            for (int k = 0; k < 2; k++)
                for (int i = 1; i < h_mas.Length - 1; i++)
                    for (int j = 1; j < d_mas.Length - 1; j++)
                        foreach (Paralepiped P in SetP)
                            if (Turn1(k, P, color) == true && ((P.GetH() == h_mas[i] - h_mas[i - 1] && P.GetD() == d_mas[j] - d_mas[j - 1]) || P.GetD() == h_mas[i] - h_mas[i - 1] && P.GetH() == d_mas[j] - d_mas[j - 1]))
                            {

                                SetP.Remove(P);
                                if (d_mas[j] - d_mas[j - 1] != P.GetD())
                                    P.TurnHorisontal();
                                P.SetXYZ((k == 0 || w_mas.Length < 2) ? 0 : w_mas[w_mas.Length - 2], d_mas[j - 1], h_mas[i - 1]);
                                SetF.Add(P);
                                break;
                            }
            for (int k = 2; k < 4; k++)
                for (int i = 1; i < h_mas.Length - 1; i++)
                    for (int j = 1; j < w_mas.Length - 1; j++)
                        foreach (Paralepiped P in SetP)
                            if (Turn1(k, P, color) == true && ((P.GetH() == h_mas[i] - h_mas[i - 1] && P.GetW() == w_mas[j] - w_mas[j - 1]) || P.GetW() == h_mas[i] - h_mas[i - 1] && P.GetH() == w_mas[j] - w_mas[j - 1]))
                            {
                                SetP.Remove(P);
                                if (w_mas[j] - w_mas[j - 1] != P.GetW())
                                    P.TurnVertical();
                                P.SetXYZ(w_mas[j - 1], (k == 2 || d_mas.Length < 2) ? 0 : d_mas[d_mas.Length - 2], h_mas[i - 1]);
                                SetF.Add(P);
                                break;
                            }
            for (int k = 4; k < 6; k++)
                for (int i = 1; i < w_mas.Length - 1; i++)
                    for (int j = 1; j < d_mas.Length - 1; j++)
                        foreach (Paralepiped P in SetP)
                            if (Turn1(k, P, color) == true && ((P.GetW() == w_mas[i] - w_mas[i - 1] && P.GetD() == d_mas[j] - d_mas[j - 1]) || P.GetD() == w_mas[i] - w_mas[i - 1] && P.GetW() == d_mas[j] - d_mas[j - 1]))
                            {
                                SetP.Remove(P);
                                if (d_mas[j] - d_mas[j - 1] != P.GetD()) P.TurnHorisontal1();
                                P.SetXYZ(w_mas[i - 1], d_mas[j - 1], (k == 4 || h_mas.Length < 2) ? 0 : h_mas[h_mas.Length - 2]);
                                SetF.Add(P);
                                break;
                            }
        }
        static void Algorithm(ISet<Paralepiped> SetP, ISet<Paralepiped> SetF, Paralepiped FulP)
        {
            char[] color = FulP.GetColor();
            //подсчитываем колличество паралепипедов на стыках сторон
            int n_d = 0, n_w = 0, n_h = 0;
            foreach (Paralepiped P in SetP)
            {
                if (P.TestColor(2, new char[] { color[0], color[2] }) == true)
                    n_h++;
                if (P.TestColor(2, new char[] { color[2], color[4] }) == true)
                    n_w++;
                if (P.TestColor(2, new char[] { color[0], color[4] }) == true)
                    n_d++;
            }
            int[] h_mas = new int[n_h];
            int[] d_mas = new int[n_d];
            int[] w_mas = new int[n_w];

            //-------------------------------------------------------------------------------------------------
            //устанавливаем паралепипеды с 3 цветами
            for (int i = 0; i < 2; i++)
                for (int j = 2; j < 4; j++)
                    for (int k = 4; k < 6; k++)
                        foreach (Paralepiped P in SetP)
                            if (Turn3(i, j, k, P, color) == true)
                            {
                                SetP.Remove(P);
                                P.SetXYZ((i == 0) ? 0 : FulP.GetW() - P.GetW(), (j == 2) ? 0 : FulP.GetD() - P.GetD(), (k == 4) ? 0 : FulP.GetH() - P.GetH());
                                w_mas[(i == 0) ? 0 : w_mas.Length - 1] = (i == 0) ? P.GetW() : FulP.GetW();
                                d_mas[(j == 2) ? 0 : d_mas.Length - 1] = (j == 2) ? P.GetD() : FulP.GetD();
                                h_mas[(k == 4) ? 0 : h_mas.Length - 1] = (k == 4) ? P.GetH() : FulP.GetH();
                                SetF.Add(P);
                                break;
                            }
            //------------------------------------------------------------------------------------------------
            U2(w_mas, d_mas, h_mas, SetP, SetF, color);
            U1(w_mas, d_mas, h_mas, SetP, SetF, color);
            //устанавливаем паралепипеды без цветов
            for (int i = 1; i < w_mas.Length - 1; i++)
                for (int j = 1; j < d_mas.Length - 1; j++)
                    for (int k = 1; k < h_mas.Length - 1; k++)
                        foreach (Paralepiped P in SetP)
                            if (P.TestSize(w_mas[i] - w_mas[i - 1], d_mas[j] - d_mas[j - 1], h_mas[k] - h_mas[k - 1]) == true)
                            {
                                SetP.Remove(P);
                                if (P.GetW() != w_mas[i] - w_mas[i - 1])
                                    if (P.GetH() == w_mas[i] - w_mas[i - 1])
                                        P.TurnVertical();
                                    else
                                        P.TurnHorisontal1();
                                if (P.GetH() != h_mas[k] - h_mas[k - 1])
                                    P.TurnHorisontal();
                                P.SetXYZ(w_mas[i - 1], d_mas[j - 1], h_mas[k - 1]);
                                SetF.Add(P);
                                break;
                            }
        }
        static void Main(string[] args)
        {
            Paralepiped FulP = new Paralepiped();
            ISet<Paralepiped> SetP = new HashSet<Paralepiped>();
            ISet<Paralepiped> SetF = new HashSet<Paralepiped>();
            int N = ReadConsole(ref FulP, SetP);
            Algorithm(SetP, SetF, FulP);
            WriteConsole(SetF, N);
            Console.ReadLine();
        }
    }
}