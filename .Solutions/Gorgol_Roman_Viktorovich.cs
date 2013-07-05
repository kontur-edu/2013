// Пока только решение для первых 18 тестов
// JUDGE_ID:89826VK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            StreamReader sr = new StreamReader(Console.OpenStandardInput());

            string[] input = sr.ReadLine().Split(' ');
            int w = int.Parse(input[0]);
            int d = int.Parse(input[1]);
            int h = int.Parse(input[2]);
            int[] sizes = new[]{w,d,h};
            string colors = input[3];

            input = sr.ReadLine().Split(' ');
            int n = int.Parse(input[0]);

            List<Piece> list = new List<Piece>();
            List<Piece>[] list2 = new List<Piece>[4];
            for (int i = 0; i < list2.Length; i++ )
            {
                list2[i] = new List<Piece>();
            }

            //считывание данных о кубиках
            for (int i = 0; i < n; i++)
            {
                input = sr.ReadLine().Split(' ');
                int w1 = int.Parse(input[0]);
                int d1 = int.Parse(input[1]);
                int h1 = int.Parse(input[2]);

                Piece piece = new Piece(input[3],w1,d1,h1);
                list.Add(piece);
                list2[piece.count].Add(piece);

                int check=0;
                //ориентирование считанного кубика
                for (int j = 0; j < 6; j++)
                {
                    if (piece.colors[j] == '.') continue;
                    check++;
                    //j - текущая позиция, k - требуемая позиция
                    int k = 0;
                    while (k < 6 && (colors[k] != piece.colors[j]))
                    {
                        k++;
                    }
                    int pos = k;
                    if (k < 6 && j != pos)
                    {
                        //проверяем, нужно ли развернуть на 180 градусов
                        if (j == Piece.Reverse(pos))
                        {
                            //выбираем соседнюю, неправильно ориентированную грань
                            pos = (Math.Max(j, pos) + 1) % 6;
                            if (piece.colors[pos] == colors[pos]) pos = (Math.Min(k, j) - 1 + 6) % 6;
                            piece.Turn(j, pos);
                        }
                        piece.Turn(j, pos);
                    }
                }
            }
            //
            // обработка одномерного случая
            //
            int mainSize = 0;
            Piece start = list2[3][0];//начало с крайней грани
            //выбор оси
            for (int i = 0; i < 3; i++)
            {
                if (start.sizes[i] != sizes[i])
                {
                    mainSize = i;
                }
            }
            int[] begin = new[]{0,0,0};
            //ориентация кубиков, у которых не меньше 3 граней с цветом
            foreach (var piece in list2[3])
            {
                for (int i = 0; i < 3; i++)
                {
                    if (colors[2*i + 1] == piece.colors[2*i + 1])
                    {
                        piece.coordinates[i] = sizes[i] - piece.sizes[i];
                    }
                }
                //выбираем левый, нижний кубик
                if (piece.coordinates[mainSize] == 0) start = piece;
            }
            begin[mainSize] += start.sizes[mainSize];
            //ориентация кубиков, у которых 2 грани с цветом
            foreach (var piece in list2[2])
            {
                piece.coordinates[mainSize] = begin[mainSize];
                begin[mainSize] += piece.sizes[mainSize];
            }
            foreach (var piece in list)
            {
                Console.WriteLine("{0} {1} {2}", piece.faces[0], piece.faces[2],String.Join(" ",piece.coordinates));   
            }
        }

        public static bool equal(string s1,string s2)
        {
            int k = 0;
            while(k<s1.Length && (s1[k]==s2[k] || s1[k] == '.'))
            {
                k++;
            }
            return (k == s1.Length);
        }
        
    }
    public class Piece
    {
        public string colors;
        public string faces = "FBDULR";
        public int count = 0;
        public int[] coordinates = new[] {0, 0, 0};

        public int[] sizes;

        public Piece(string s,int w,int d, int h)
        {
            colors = s;
            sizes = new[] {w, d, h};
            for (int i = 0; i < s.Length; i+=2 )
            {
                if (s[i] != '.' || s[i+1]!='.') count++;
            }
            count = Math.Min(count, 3);
        }
        /// <summary>
        /// перемещение символа с i на j
        /// </summary>
        /// <param name="s"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public void Turn(int i,int j)
        {
            int size = sizes[i/2];
            sizes[i/2] = sizes[j/2];
            sizes[j/2] = size;
            colors = Turn(colors, i, j);
            faces = Turn(faces, i, j);
        }
        private static string Turn(string s, int i, int j)
        {
            char[] temp = s.ToCharArray();
            temp[j] = s[i];
            temp[Reverse(i)] = s[j];
            temp[Reverse(j)] = s[Reverse(i)];
            temp[i] = s[Reverse(j)];
            return String.Join("", temp);
        }
        /// <summary>
        /// возвращает номер противоположенной грани
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int Reverse(int i)
        {
            return i % 2 == 0 ? i + 1 : i - 1;
        }
    }
}
