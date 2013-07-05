#region Описание решения
/*   ID попытки на тимусе: 4900479
 * 
 *   Реализованно усложнение: 
 * Количесто частей n может быть 100000 и более.
 * 
 *   Суть алгоритма решения:
 *   
 * Пусть исходный параллелепипед находится в пространстве со следующей
 * системой координат:
 *  ось X - вправо вдоль нижнего ребра передней грани,
 *  ось Y - вверх вдоль левого ребра передней грани,
 *  ось Z - от наблюдателя вдоль левого ребра нижней грани, 
 *  начало координат совпадает с передней левой нижней вершиной,
 * тогда можно получить наборы чисел описывающих расстояния между разрезами, 
 * перпендикулярными каждой из осей. Эти расстояния можно получить из таких
 * частей, ребра которых лежат на описанных осях. Длины ребер, расположенных 
 * на осях, и будут расстоянием между соседними разрезами. 
 * 
 * Часть исходного параллелепипеда лежит на оси X, если ее грани содержат цвета 
 * его передней и нижней граней. Важно выделить первую и последнюю части на оси, 
 * они содержат также цвета левой или правой граней соответственно, все остальные
 * части могут рассматриваться в произвольном порядке. В список расстояний между 
 * разрезами перпендикулярными оси X заносятся длины ребер выбранных частей между
 * указанными гранями. При этом в начале списка всегда будет длина ребра начальной
 * части, в конце - последней. Аналогично заполняются списки для осей Y и Z.
 * 
 * Имея такие списки расстояний, можно получить все места частей в параллелепипеде,
 * последовательно перебирая расстояния из спиков. Для каждого места можно
 * сформировать описание части, которая должна его занять, то есть определить 
 * ее размеры и цвета граней. Для каждого места нужно подобрать соответствующую 
 * ему часть. Для определения соответствия части месту достаточно выполнение 
 * следующих условий:
 *  - место части еще не определено
 *  - размеры части совпадают с размерами места для нее
 *  - множество цветов части состоит из тех же элементов что и множество цветов,
 *    описанное местом.
 * 
 * Далее следует перебирая все части определить подходящую.
 * Часть, удовлетворяющую этим условием следует повернуть таким образом, чтобы
 * ее цвета совпали с цветами, описанными местом, в  точности том же порядке, а 
 * ее каждый из ее размеров Ширина, Высота и Глубина совпал с таким же размером 
 * места. Затем ей следует назначить координаты этого места и отметить что ее 
 * место определено, исключив из рассмотрения для других мест. Таким образом для
 * каждого места будет найдена подходящая часть, а каждая часть повернута нужным
 * образом и имеет координаты. 
*/
#endregion

using System;
using System.Collections.Generic;

// Структура, описывающая прямоугольный параллелепипед
internal struct Cuboid 
{
    // Значение, показывающее найдено ли место для части
    public bool IsPlaceFound { get; set; }

    // Координаты и размеры параллелепипеда
    private int x, y, z;
    private int width, depth, hight;
    // Грани описываются двумя массивами - положений и цветов
    // Массивы по очереди описывают переднюю, заднюю, нижнюю, верхнюю, левую и 
    // правую грани
    private char[] faces;
    private char[] colors;

    // Свойство, определяющее, содержит ли часть ребро, лежащее на какой-либо
    // из осей. Если содержит - показывает, является ли часть первой или последней
    // вдоль оси, либо находится где-то в середине.
    public char SpecialPosition
    {
        get
        {
            if (colors.Contains(Program.OriginalColors[0]) &&
                colors.Contains(Program.OriginalColors[2]))
            {
                if (colors.Contains(Program.OriginalColors[4]))
                    return 'F';
                return colors.Contains(Program.OriginalColors[5]) ? 'R' : 'X';
            }
            if (colors.Contains(Program.OriginalColors[2]) &&
                colors.Contains(Program.OriginalColors[4]))
            {
                return colors.Contains(Program.OriginalColors[1]) ? 'B' : 'Y';
            }
            if (colors.Contains(Program.OriginalColors[0]) &&
                colors.Contains(Program.OriginalColors[4]))
            {
                return colors.Contains(Program.OriginalColors[3]) ? 'U' : 'Z';
            }
            return 'N';
        }
    }

    public Cuboid(int width, int depth, int hight, char[] colors) : this()
    {
        this.width = width;
        this.depth = depth;
        this.hight = hight;
        this.colors = colors;
    }

    public void Init()
    {
        string[] s = Console.ReadLine().Split(Program.Separator);
        width = s[0].ToInt();
        depth = s[1].ToInt();
        hight = s[2].ToInt();
        colors = s[3].ToCharArray();
        faces = new char[] { 'F', 'B', 'D', 'U', 'L', 'R' };
    }

    public void SetCoords(int newX, int newY, int newZ)
    {
        x = newX;
        y = newY;
        z = newZ;
    }

    public void WriteAnswer()
    {
        Console.WriteLine("{0} {1} {2} {3} {4}", faces[0], faces[2], y, z, x);
    }

    private static void Swap2(ref int a, ref int b)
    {
        var tmp = a;
        a = b;
        b = tmp;
    }

    private static void Swap4(ref char a, ref char b, ref char c, ref char d)
    {
        var tmp = a;
        a = b;
        b = c;
        c = d;
        d = tmp;
    }

    // Вовзвращает длину ребра между двумя гранями указанных цветов
    // Принимаемые значения - индексы цветов в массиве цветов оригинала
    public int GetDimension(int index1, int index2)
    {
        int i = Array.IndexOf(colors, Program.OriginalColors[index1]);
        int j = Array.IndexOf(colors, Program.OriginalColors[index2]);
        // Выбирается размер, который нужно вернуть, для этого
        // служит матрица, описывающая параллельность ребер осям
        switch (Program.AxisBetweenTwoFaces[i, j])
        {
            case 'x':
                return hight;
            case 'y':
                return width;
            case 'z':
                return depth;
            default:
                throw new Exception("There's no edge between the specified faces");
        }
    }

    // Поворот по часовой стрелке при неизменной передней грани
    private void RotateFrontIsFixed()
    {
        Swap4(ref colors[4], ref colors[2], ref colors[5], ref colors[3]);
        Swap4(ref faces[4], ref faces[2], ref faces[5], ref faces[3]);
        Swap2(ref hight, ref depth);
    }

    // Поворот по часовой стрелке при неизменной нижней грани
    private void RotateDownIsFixed()
    {
        Swap4(ref colors[0], ref colors[5], ref colors[1], ref colors[4]);
        Swap4(ref faces[0], ref faces[5], ref faces[1], ref faces[4]);
        Swap2(ref width, ref hight);
    }

    // Проверяет, содержат ли множества цветов части и указанной другой части
    // одинаковые наборы элементов
    public bool CompareColors(Cuboid target)
    {
        char[] targetColors = new char[6];
        target.colors.CopyTo(targetColors, 0);
        for (int i = 0; i < 6; i++)
        {
            if (!targetColors.FindAndReplace(colors[i], ' '))
            {
                return false;
            }
        }
        return true;
    }

    // Проверяет, совпадают ли три размера части с тремя размерами другой части
    public bool CompareDimensions(Cuboid target)
    {
        return
            target.depth == depth ?
                target.width == width ?
                    target.hight == hight :
                    target.width == hight ?
                        target.hight == width : false :
            target.depth == width ?
                target.width == depth ?
                    target.hight == hight :
                    target.width == hight ?
                        target.hight == depth : false :
            target.depth == hight ?
                target.width == depth ?
                    target.hight == width :
                    target.width == width ?
                        target.hight == depth : false :
            false;
    }

    // Поворачивает часть до полного совпадения с указанным описанием
    public void Rotate(Cuboid target)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (IsAppropriate(target))
                {
                    return;
                }
                RotateFrontIsFixed();
            }
            RotateDownIsFixed();
        }
        RotateFrontIsFixed();
        RotateDownIsFixed();
        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                if (IsAppropriate(target))
                {
                    return;
                }
                RotateFrontIsFixed();
            }
            RotateDownIsFixed();
            RotateDownIsFixed();
        }
        throw new Exception("An unexpected error occurred");
    }

    // Проверяет, совпадает ли часть с указанным описанием, т.е. повернута ли
    // она как нужно. Поскольку множества цветов и размеров заведомо совпадают
    // с таковыми у описания, то достаточно проверить равенства 5 цветов из 6
    // и 2 размеров из 3.
    private bool IsAppropriate(Cuboid target)
    {
        return
            colors[0] == target.colors[0] &&
            colors[1] == target.colors[1] &&
            colors[2] == target.colors[2] &&
            colors[3] == target.colors[3] &&
            colors[4] == target.colors[4] &&
            depth == target.depth &&
            hight == target.hight;
    }
}


internal static class Program
{
    // Список цветов граней исходного параллелепипеда
    public static char[] OriginalColors { get; private set; }
    public static readonly char[] Separator = new char[] { ' ' };
    // Матрица описывающая сонаправленность ребра между двумя гранями с какой-либо из осей
    public static readonly char[,] AxisBetweenTwoFaces = new char[,] {
        {'-','-','x','x','z','z'},
        {'-','-','x','x','z','z'},
        {'x','x','-','-','y','y'},
        {'x','x','-','-','y','y'},
        {'z','z','y','y','-','-'},
        {'z','z','y','y','-','-'} 
    };

    private static int originalWidth, originalDepth, originalHight, n;
    // Массив структур описывающих части параллелеипеда
    private static Cuboid[] pieces;
    // Списки, определяющие расстояния между разрезами перпендикулярными трем осям
    private static readonly LinkedList<int> cutX = new LinkedList<int>();
    private static readonly LinkedList<int> cutY = new LinkedList<int>();
    private static readonly LinkedList<int> cutZ = new LinkedList<int>();

    private static void Main()
    {
        string[] s = Console.ReadLine().Split(Separator);
        originalWidth = s[0].ToInt();
        originalDepth = s[1].ToInt();
        originalHight = s[2].ToInt();
        OriginalColors = s[3].ToCharArray();
        n = Console.ReadLine().ToInt();
        pieces = new Cuboid[n];

        // Заполняем список частей и списки разрезов
        GetPiecesAndCuts();

        // x, y, z - координаты места для которого подбирается часть
        // Перебираем расстояния между соседними разрезами перпендикулярными осям
        int x = 0;
        for (var xNode = cutX.First; xNode != null; x += xNode.Value, xNode = xNode.Next)
        {
            int y = 0;
            for (var yNode = cutY.First; yNode != null; y += yNode.Value, yNode = yNode.Next)
            {
                int z = 0;
                for (var zNode = cutZ.First; zNode != null; z += zNode.Value, zNode = zNode.Next)
                {
                    // Создаем структуру "параллелепипед", описывающую характеристики
                    // части в месте с координатами x, y, z
                    var nextPiece = GetNextPiece(x, y, z, yNode.Value, zNode.Value, xNode.Value);
                    // Находим подходящую часть, поворачиваем и назначаем координаты
                    int index = Search(nextPiece);
                    pieces[index].Rotate(nextPiece);
                    pieces[index].SetCoords(x, y, z);
                    pieces[index].IsPlaceFound = true;
                }
            }
        }

        for (int i = 0; i < pieces.Length; i++)
            pieces[i].WriteAnswer();
    }

    // Функция для инициализации всех частей, заполнения списка частей,
    // определения расстояний между разрезами и занесением их в списки
    private static void GetPiecesAndCuts()
    {
        int lastX = -1, lastY = -1, lastZ = -1;
        for (int i = 0; i < n; i++)
        {
            Cuboid piece = new Cuboid();
            piece.Init();
            // Проверяем положение, которое занимает новая часть
            switch (piece.SpecialPosition)
            {
                // Часть не содержит никакую ось
                case 'N':
                    break;

                // Часть содержит начало координат
                // Добавляем соответсвующие ее размеры в начало списков,
                // описывающих расстояния между разрезами
                case 'F':
                    cutX.AddFirst(piece.GetDimension(0, 2));
                    cutY.AddFirst(piece.GetDimension(4, 2));
                    cutZ.AddFirst(piece.GetDimension(0, 4));
                    break;

                // Часть содержит ось X
                // Добавляем в список, описывающий расстояния между разрезами,
                // перпендикулярными оси X длину ребра, лежащего на этой оси
                case 'X':
                    cutX.AddLast(piece.GetDimension(0, 2));
                    break;
                case 'Y':
                    cutY.AddLast(piece.GetDimension(4, 2));
                    break;
                case 'Z':
                    cutZ.AddLast(piece.GetDimension(0, 4));
                    break;

                // Часть является последней по оси X, т.е. самой правой
                case 'R':
                    lastX = i;
                    break;
                case 'B':
                    lastY = i;
                    break;
                case 'U':
                    lastZ = i;
                    break;
            }
            pieces[i] = piece;
        }
        if (lastX != -1)
            cutX.AddLast(pieces[lastX].GetDimension(0, 2));
        if (lastY != -1)
            cutY.AddLast(pieces[lastY].GetDimension(4, 2));
        if (lastZ != -1)
            cutZ.AddLast(pieces[lastZ].GetDimension(0, 4));
    }

    // Определяет парметры части на очередном месте, т.е. цвета и размеры
    private static Cuboid GetNextPiece(int x, int y, int z, int w, int d, int h)
    {
        char[] colors = new char[6] { '.', '.', '.', '.', '.', '.' };
        if (y == 0) colors[0] = Program.OriginalColors[0];
        if (z == 0) colors[2] = Program.OriginalColors[2];
        if (x == 0) colors[4] = Program.OriginalColors[4];
        if (y + w == originalWidth) colors[1] = Program.OriginalColors[1];
        if (z + d == originalDepth) colors[3] = Program.OriginalColors[3];
        if (x + h == originalHight) colors[5] = Program.OriginalColors[5];
        return new Cuboid(w, d, h, colors);
    }

    private static int Search(Cuboid target)
    {
        for (int i = 0; i < pieces.Length; i++)
            if (!pieces[i].IsPlaceFound &&
                pieces[i].CompareDimensions(target) &&
                pieces[i].CompareColors(target))
            {
                return i;
            }
        throw new Exception("Applicable piece not found");
    }

    // Метод-расширение для конвертации строки в число без выполнения 
    // дополнительных проверок как в методе Int32.Parse(), поскольку гарантируется
    // корректность исходных данных.
    public static int ToInt(this string s)
    {
        int result = 0;
        for (int i = 0; i < s.Length; i++)
        {
            result = unchecked(result * 10 + s[i] - '0');
        }
        return result;
    }

    // Метод-расширение, определяющий принадлежность элемента массиву
    public static bool Contains(this char[] array, char item)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == item)
                return true;
        }
        return false;
    }

    // Метод-расширение, заменяющий элемент массива и возвращающий успех или неудачу.
    public static bool FindAndReplace(this char[] array, char item1, char item2)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == item1)
            {
                array[i] = item2;
                return true;
            }
        }
        return false;
    }
}