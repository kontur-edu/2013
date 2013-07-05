using System;
using System.Collections.Generic;


namespace SKB_Test
{
    class Program
    {
        // Класс "Куб"
        public class Cube: Block
        {
            // Коллекция "кусков", из которых состоит куб
            public List<Block> Blocks { get; set; }

            public Cube(string info): base(info)
            {
                Blocks = new List<Block>();
            }

            public void Solve()
            {
                // Ориентирование кусочков
                foreach (var t in Blocks)
                    t.DoOrient(this);
                if (Blocks.Count > 1)
                {
                    // Нахождение кусков, содержащих точки (0,0,0) и (w,d,h)
                    int startCornerIndex = 0, endCornerIndex = 0;
                    for (var i = 0; i < Blocks.Count; i++)
                    {
                        if ((Blocks[i].Front.Color == Front.Color) && (Blocks[i].Left.Color == Left.Color) &&
                            (Blocks[i].Down.Color == Down.Color))
                            startCornerIndex = i;
                        if ((Blocks[i].Back.Color == Back.Color) && (Blocks[i].Right.Color == Right.Color) &&
                            (Blocks[i].Up.Color == Up.Color))
                            endCornerIndex = i;
                    }
                    bool down = false, front = false;
                    var nextVal = 0;
                    // Определение, в какой плоскости были сделаны разрезы
                    if (Blocks[startCornerIndex].Up.Color == '.')
                    {
                        // Куб разрезан параллельно плоскости ZX
                        down = true;
                        nextVal = Blocks[startCornerIndex].Length;
                    }
                    else if (Blocks[startCornerIndex].Back.Color == '.')
                    {
                        // Куб разрезан параллельно плоскости ZY
                        front = true;
                        nextVal = Blocks[startCornerIndex].Width;
                    }
                    else if (Blocks[startCornerIndex].Right.Color == '.')
                    {
                        // Куб разрезан параллельно плоскости YX
                        nextVal = Blocks[startCornerIndex].Height;
                    }
                    // Определение координат левой нижней точки для всех кусков, кроме первого и последнего
                    for (int i = 0; i < Blocks.Count; i++)
                    {
                        if ((i == startCornerIndex) || (i == endCornerIndex)) continue;
                        var t = Blocks[i];
                        if (down)
                        {
                            t.Point.Y = nextVal;
                            nextVal += t.Length;
                        }
                        else if (front)
                        {
                            t.Point.X = nextVal;
                            nextVal += t.Width;
                        }
                        else
                        {
                            t.Point.Z = nextVal;
                            nextVal += t.Height;
                        }
                    }
                    // Заполнение координаты для последнего куска
                    if (down)
                        Blocks[endCornerIndex].Point.Y = nextVal;
                    else if (front)
                        Blocks[endCornerIndex].Point.X = nextVal;
                    else
                        Blocks[endCornerIndex].Point.Z = nextVal;
                }
                // Вывод результатов
                foreach (var t in Blocks)
                {
                    Console.WriteLine("{0} {1} {2} {3} {4}", t.Front.Side, t.Down.Side, t.Point.X, t.Point.Y, t.Point.Z);
                }

            }
        }

        // Класс "Точка в пространстве"
        public class CornerPoint
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
        }

        // Класс "Грань параллелограмма"
        public class Face
        {
            public char Color { get; private set; }
            public char Side { get; private set; }

            public Face(char aColor, char aSide)
            {
                Color = aColor;
                Side = aSide;
            }
        }

        // Класс "Паралеллограмм"
        public class Block
        {
            public int Width { get; set; }
            public int Length { get; set; }
            public int Height { get; set; }
            public CornerPoint Point { get; set; }
            public Face Front { get; private set; }
            public Face Back { get; private set; }
            public Face Down { get; private set; }
            public Face Up { get; private set; }
            public Face Left { get; private set; }
            public Face Right { get; private set; }

            public Block(string info)
            {
                var par = info.Split(' ');
                Width = Int32.Parse(par[0]);
                Length = Int32.Parse(par[1]);
                Height = Int32.Parse(par[2]);
                Front = new Face(par[3][0], 'F');
                Back = new Face(par[3][1], 'B');
                Down = new Face(par[3][2], 'D');
                Up = new Face(par[3][3], 'U');
                Left = new Face(par[3][4], 'L');
                Right = new Face(par[3][5], 'R');
                Point = new CornerPoint();
            }

            private bool IsOrientedFB(Block original)
            {
                // Проверка на то, что передняя и задняя грани находятся на своих местах
                return ((Front.Color == original.Front.Color) || (Front.Color == '.')) &&
                       ((Back.Color == original.Back.Color) || (Back.Color == '.')) &&
                       (((Left.Color != original.Front.Color) && (Right.Color != original.Front.Color) &&
                         (Up.Color != original.Front.Color) && (Down.Color != original.Front.Color)));
            }

            private bool IsOrientedLR(Block original)
            {
                // Проверка на то, что левая и правая грани находятся на своих местах
                return ((Left.Color == original.Left.Color) || (Left.Color == '.')) &&
                       ((Right.Color == original.Right.Color) || (Right.Color == '.')) &&
                       (((Front.Color != original.Left.Color) && (Back.Color != original.Left.Color) &&
                         (Up.Color != original.Left.Color) && (Down.Color != original.Left.Color)));
            }

            private bool IsOrientedUD(Block original)
            {
                // Проверка на то, что верхняя и нижняя грани находятся на своих местах
                return ((Up.Color == original.Up.Color) || (Up.Color == '.')) &&
                       ((Down.Color == original.Down.Color) || (Down.Color == '.')) &&
                       (((Front.Color != original.Up.Color) && (Back.Color != original.Up.Color) &&
                         (Left.Color != original.Up.Color) && (Right.Color != original.Up.Color)));
            }

            private bool IsOriented(Block original)
            {
                return IsOrientedFB(original) && IsOrientedLR(original) && IsOrientedUD(original);
            }

            public void DoOrient(Block original)
            {
                while (!IsOriented(original))
                {
                    // Вращение куска до тех пор, пока грани не встанут на свои места
                    for (var i = 0; (i < 4) && !IsOrientedFB(original); i++)
                        DoRotateUp();
                    for (var i = 0; (i < 4) && !IsOrientedFB(original); i++)
                        DoRotateLeft();
                    for (var i = 0; (i < 4) && !IsOrientedLR(original); i++)
                        DoRotateClockwise();
                    if (!IsOrientedUD(original))
                    {
                        // Замена местами верх и низ
                        DoRotateClockwise();
                        DoRotateClockwise();
                        DoRotateLeft();
                    }
                }
            }

            public void DoRotateUp()
            {
                // Вращение "вверх"
                var tmp = Front;
                Front = Down;
                Down = Back;
                Back = Up;
                Up = tmp;
                var w = Width;
                Width = Length;
                Length = w;
            }

            public void DoRotateLeft()
            {
                // Вращение "влево"
                var tmp = Front;
                Front = Right;
                Right = Back;
                Back = Left;
                Left = tmp;
                var w = Width;
                Width = Height;
                Height = w;
            }

            public void DoRotateClockwise()
            {
                // Вращение "по часовой стрелке"
                var tmp = Up;
                Up = Left;
                Left = Down;
                Down = Right;
                Right = tmp;
                var h = Height;
                Height = Length;
                Length = h;
            }
        }

        static void Main()
        {
            // Считывание
            var cube = new Cube(Console.ReadLine());
            var n = Int32.Parse(Console.ReadLine());
            for (var i = 0; i < n; i++)
            {
                var block = new Block(Console.ReadLine());
                cube.Blocks.Insert(i, block);
            }
            // Сборка
            cube.Solve();
        }
    }
}