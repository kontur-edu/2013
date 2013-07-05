using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _1843
{
    class Program
    {
        static void Main(string[] args)
        {
            //Усложнения не реализованы

            BasicCuboid basicCuboid;        //Параллелепипед Эрнеста
            List<Piece> pieces;             //Куски параллелепипеда Эрнеста
            string[] tokens;

#if !ONLINE_JUDGE
            Console.SetIn(new StreamReader(@"..\..\input.txt"));
            Console.SetOut(new StreamWriter(@"..\..\output.txt"));
#endif
            
            //Input

            tokens = Console.ReadLine().Split(' ');
            basicCuboid = new BasicCuboid(
                Convert.ToInt32(tokens[0]), 
                Convert.ToInt32(tokens[1]), 
                Convert.ToInt32(tokens[2]), 
                tokens[3]);

            pieces = new List<Piece>(Convert.ToInt32(Console.ReadLine()));
            for (int i = 0; i < pieces.Capacity; i++)
            {
                tokens = Console.ReadLine().Split(' ');

                pieces.Add(new Piece(
                    Convert.ToInt32(tokens[0]),
                    Convert.ToInt32(tokens[1]),
                    Convert.ToInt32(tokens[2]),
                    tokens[3]));
            }

            //Processing

            basicCuboid.Resolve(pieces);

            //Output

            foreach (Piece piece in pieces)
            {
                piece.Print(Console.Out);
            }

            Console.Out.Flush();
        }

        public abstract class Cuboid
        {
            public char[] faceColors { get; set; } //Соответствие номеров граней цветам

            public enum Axis
            {
                X,      //Ось X проходит от передней к задней грани
                Y,      //Ось Y проходит от верхней к нижней грани
                Z,      //Ось Z проходит от левой к правой грани
            }

            public Dictionary<Axis, int> Measure = new Dictionary<Axis, int>(3);    //Width, Depth, Height

            public Cuboid(int width, int depth, int height)
            {
                Measure[Axis.X] = width;
                Measure[Axis.Y] = depth;
                Measure[Axis.Z] = height;
            }
        }

        public class BasicCuboid : Cuboid
        {
            #region Fields

            private Dictionary<Axis, List<int>> cutsPositions;      //Линии разрезов
            private Dictionary<Axis, List<int>> cutsLengths;        //Расстояния между разрезами

            private bool[, ,] Taken;        //Занятые блоки в сетке

            #endregion

            #region Methods

            public BasicCuboid(int width, int depth, int height, string faceColorsString)
                : base(width, depth, height)
            {
                faceColors = new char[6];
                for (int i = 0; i < 6; i++)
                    faceColors[i] = faceColorsString[i];

                cutsPositions = new Dictionary<Axis, List<int>>(3);
                cutsPositions[Axis.X] = new List<int> { 0 };
                cutsPositions[Axis.Y] = new List<int> { 0 };
                cutsPositions[Axis.Z] = new List<int> { 0 };

                cutsLengths = new Dictionary<Axis, List<int>>(3);
                cutsLengths[Axis.X] = new List<int>();
                cutsLengths[Axis.Y] = new List<int>();
                cutsLengths[Axis.Z] = new List<int>();
            }

            public int AddAxisLabel(Axis cutAxis, int length)   
            {
                //Добавляет новый разрез. Возвращает его левую координату
                //Отрицательная длина откладывается справа и для нее не добавляется разрез, чтобы не нарушать возрастающий порядок разрезов
                //При этом разрез не теряется

                if (length > 0)
                {
                    int result = cutsPositions[cutAxis].Last();
                    cutsPositions[cutAxis].Add(cutsPositions[cutAxis].Last() + length);
                    return result;
                }
                else
                {
                    return Measure[cutAxis] + length;
                }
            }

            public void CreateGrid()
            {
                //Создает сетку для параллелепипеда

                foreach (Axis axis in Enum.GetValues(typeof(Axis)))
                {
                    //Добавляет измерение по оси в качестве последнего разреза
                    //Необходимо для корректного вычисления длины последнего отрезка                
                    cutsPositions[axis].Add(Measure[axis]);

                    //Вычисляет длины отрезков между разрезами
                    for (int i = 1; i < cutsPositions[axis].Count; i++)
                    {
                        cutsLengths[axis].Add(cutsPositions[axis][i] - cutsPositions[axis][i - 1]);
                    }
                }

                //Инициализирует массив занятых блоков. Блоки, стоящие вдоль осей, помечаются как занятые.
                Taken = new bool[cutsLengths[Axis.X].Count, cutsLengths[Axis.Y].Count, cutsLengths[Axis.Z].Count];

                for (int i = 0; i < cutsLengths[Axis.X].Count; i++)
                {
                    Taken[i, 0, 0] = true;
                }
                for (int i = 0; i < cutsLengths[Axis.Y].Count; i++)
                {
                    Taken[0, i, 0] = true;
                }
                for (int i = 0; i < cutsLengths[Axis.Z].Count; i++)
                {
                    Taken[0, 0, i] = true;
                }
            }

            public int[] FindPlace(Piece piece)
            {
                //Выполняет поиск места для блока и помечает это место как занятое
                //Возвращает координаты, оговоренные условием

                Dictionary<Axis, int> StartIndices = new Dictionary<Axis, int>(3);
                Dictionary<Axis, int> StopIndices = new Dictionary<Axis, int>(3);

                foreach (Axis axis in Enum.GetValues(typeof(Axis)))
                {
                    if (piece.AxisMask(axis, ".C"))
                    {
                        //Если блок должен прилегать к грани, удаленной от оси,
                        //начальный индекс поиска его места приравнивается к максимальному индексу
                        StartIndices[axis] = cutsLengths[axis].Count - 1;
                    }
                    else
                    {
                        StartIndices[axis] = 0;
                    }

                    if (piece.AxisMask(axis, "C."))
                    {
                        //Если блок должен прилегать к грани, проходящей через ось,
                        //конечный индекс поиска его места приравнивается к минимальному индексу плюс один
                        StopIndices[axis] = 1;
                    }
                    else
                    {
                        StopIndices[axis] = cutsLengths[axis].Count;
                    }
                }

                //Поиск места для блока полным перебором с промежуточными условиями,
                //Отражающими, подходит ли блок для данной плоскости(ряда) блоков
                for (int i = StartIndices[Axis.X]; i < StopIndices[Axis.X]; i++)
                {
                    if (cutsLengths[Axis.X][i] == piece.Measure[Axis.X])
                    {
                        for (int j = StartIndices[Axis.Y]; j < StopIndices[Axis.Y]; j++)
                        {
                            if (cutsLengths[Axis.Y][j] == piece.Measure[Axis.Y])
                            {
                                for (int k = StartIndices[Axis.Z]; k < StopIndices[Axis.Z]; k++)
                                {
                                    if (cutsLengths[Axis.Z][k] == piece.Measure[Axis.Z])
                                    {
                                        if (!Taken[i, j, k])
                                        {
                                            //Если блок подходит по размерам и это место еще не занято, 
                                            //Оно занимается и возвращаются координаты, оговоренные условием
                                            Taken[i, j, k] = true;
                                            return new int[] { cutsPositions[Axis.X][i], cutsPositions[Axis.Y][j], cutsPositions[Axis.Z][k] };
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //Если подходящий блок не найден, возвращаются нулевые координаты.
                return new int[] { 0, 0, 0 };
            }

            public void Resolve(List<Piece> pieces)    
            {
                //Вызывает выполнение всех действий, необходимых для решения задачи

                //Ориентирование кусков
                foreach (Piece piece in pieces)
                {
                    piece.Orient(this);
                }

                //Расставляет куски, лежащие на осях, в порядке, соответствующем типам кусков
                for (int i = 0; i < 10; i++)
                {
                    foreach (Piece piece in pieces)
                    {
                        if (piece.isAxisPiece && (int)piece.type == i)
                            piece.PlaceAxePiece(this);
                    }
                }

                //Создается сетку для последующего размещения кусков
                this.CreateGrid();

                //Расставляет куски, не лежащие на осях, в порядке, соответствующем типам кусков
                for (int i = 0; i < 10; i++)
                {
                    foreach (Piece piece in pieces)
                    {
                        if (!piece.isAxisPiece && (int)piece.type == i)
                            piece.Place(this);
                    }
                }
            }

            #endregion
        }

        public class Piece : Cuboid
        {
            #region Fields

            public Type type;    //Тип куска
            public bool isAxisPiece = false; //Лежит ли кусок на оси
            public int[] coordinates = new int[3];  //Координаты, оговоренные условием

            private int oppositeColorPairs, cutCount; //Количество пар противоположных цветов, срезов

            //Позиции, на которых изначально были грани
            //Индекс - где грань была в начале
            //Значение - где грань сейчас
            //Необходимо для определения, какая грань должна стать передней, а какая нижней
            private int[] startPositions;           

            //Оси, по которым разрешено поворачивать куб
            //При ориентации после фиксации первой грани можно продолжать крутить только по одной оси
            private Dictionary<Axis, bool> allowedAxes = new Dictionary<Axis, bool>(3);
            
            public enum Type
            {
                // Через пробел указано:
                // Количество парных цветов, количество измерений среза, количество срезанных граней
                Full,       // 3 0 0        // Полный параллелепипед
                //Одномерный случай
                Half,       // 2 1 1        // Параллелепипед, обрезанные с одной стороны, "пополам"
                Segment,    // 2 1 2        // Параллелепипед, обрезанный с двух сторон параллельно
                //Двумерный случай
                Angle,      // 1 2 2        // Угол
                Slice,      // 1 2 3        // Сторона
                Tube,       // 1 2 4        // Середина
                //Трехмерный случай
                Vertex,     // 0 3 3        // Вершина
                Edge,       // 0 3 4        // Ребро
                Face,       // 0 3 5        // Грань
                Internal    // 0 3 6        // Внутренний параллелепипед
            }

            private static Dictionary<Axis, List<byte>> rotationDictionary = new Dictionary<Axis, List<byte>>
            {
                //Перечисление граней в порядке, совпадающем с направлением вращения по правилу правого буравчика
                { Axis.X, new List<byte> { 3, 5, 2, 4 } },
                { Axis.Y, new List<byte> { 0, 5, 1, 4 } },
                { Axis.Z, new List<byte> { 0, 2, 1, 3 } }
            };

            //Соответствие номеров граней их положению в пространстве
            //Используется только в выводе
            private string Faces = "FBDULR";  

            #endregion

            #region Public Methods

            public Piece(int width, int depth, int height, string faceColorsString)
                : base(width, depth, height)
            {
                //Запись цветов в массив
                faceColors = new char[6];
                startPositions = new int[6];
                cutCount = 0;
                for (int i = 0; i < 6; i++)
                {
                    faceColors[i] = faceColorsString[i];
                    if (faceColors[i] == '.')
                        cutCount++;
                    startPositions[i] = i;
                }

                //Определение типа куска
                oppositeColorPairs = AxisMask("CC");

                switch (cutCount)
                {
                    case 0: type = (Type)0; break;
                    case 1: type = (Type)1; break;
                    case 2: type = oppositeColorPairs == 1 ? (Type)3 : (Type)2; break;
                    case 3: type = oppositeColorPairs == 1 ? (Type)4 : (Type)6; break;
                    case 4: type = oppositeColorPairs == 1 ? (Type)5 : (Type)7; break;
                    case 5: type = (Type)8; break;
                    case 6: type = (Type)9; break;
                }
            }

            public void Orient(BasicCuboid basicCuboid)
            {                
                //Инициализация всех направлений вращения как допустимых
                foreach (Axis axe in Enum.GetValues(typeof(Axis)))
                {
                    allowedAxes.Add(axe, true);
                }

                //Первая ориентированная грань.
                //Значение -2 соответствует состоянию, когда ни одна грань не была повернута
                //Использование -1 затрудняет использование куска несколько ниже
                // if (faceColors[i] != '.' && i / 2 != firstOrientedFace / 2)
                int firstOrientedFace = -2;

                //Определение, сколько граней возможно зафиксировать
                int possibleFixedFaces;
                if (cutCount == 6)
                {
                    possibleFixedFaces = 0;
                }
                else if (cutCount == 5 || type == Type.Tube)
                {
                    possibleFixedFaces = 1;
                }
                else
                {
                    possibleFixedFaces = 2;
                }
                
                int fixedFaces = 0; //Число уже зафиксированных граней
                byte i = 0; //Итератор
                while (fixedFaces != possibleFixedFaces)
                {
                    if (faceColors[i] != '.' && i / 2 != firstOrientedFace / 2)
                    {
                        //Если (данная грань цветная) И (данная грань и противоположная еще не были зафиксированы)
                        for (byte j = 0; j < 6; j++)
                        {
                            if (faceColors[i] == basicCuboid.faceColors[j])
                            {
                                //Если найдено, на каком месте должен находиться данный цвет
                                if (i == j || Rotate(i, j)) //Повернуть, если он еще не на месте
                                {
                                    firstOrientedFace = j;
                                    fixedFaces++;

                                    //Запретить вращение по всем граням
                                    foreach (Axis axe in Enum.GetValues(typeof(Axis)))
                                        allowedAxes[axe] = false;

                                    //Кроме той, которая не повлияет на положение данной грани
                                    //X для 0, 1
                                    //Y для 2, 3
                                    //Z для 4, 5
                                    allowedAxes[(Axis)(j / 2)] = true;
                                }
                                break;
                            }
                        }
                    }
                    i++;
                    i %= 6;  //Начать просмотр граней с начала. В ходе процесса они могут меняться местами
                }

                //Если куоск соседствует с двумя гранями, лежащими на оси, он лежит на оси
                if (AxisMask("C.") >= 2)
                {
                    isAxisPiece = true;
                }
            }

            public void PlaceAxePiece(BasicCuboid basicCuboid)
            {
                foreach (Axis axe in Enum.GetValues(typeof(Axis)))
                {
                    if (AxisMask("C.") - (AxisMask(axe, "C.") ? 1 : 0) == 2)
                    {
                        if (AxisMask(axe, ".C"))
                        {
                            coordinates[(int)axe] = basicCuboid.AddAxisLabel(axe, -Measure[axe]);
                        }
                        else
                        {
                            coordinates[(int)axe] = basicCuboid.AddAxisLabel(axe, Measure[axe]);
                        }
                    }
                }
            }

            public void Place(BasicCuboid basicCuboid)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            //Рассмотрим все возможные положения данного блока, начиная с нулевого
                            if (allowedAxes[Axis.X]) Rotate(Axis.X, i);
                            if (allowedAxes[Axis.Y]) Rotate(Axis.Y, j);
                            if (allowedAxes[Axis.Z]) Rotate(Axis.Z, k);

                            //Когда найдется подходящее место, завершаем поиск
                            //0, 0, 0 никогда не является верным ответом. Угловые куски не попадают в эту ветвь кода.
                            coordinates = basicCuboid.FindPlace(this);
                            if (coordinates[0] != 0 || coordinates[1] != 0 || coordinates[2] != 0)
                            {
                                return;
                            }
                        }
                    }
                }
            }

            #endregion

            #region Rotation

            private void Rotate(Axis rotationAxis, int count)
            {
                List<byte> Faces = rotationDictionary[rotationAxis];
                char tmpChar;
                int tmpInt;
                while (count < 0) count += 4; count %= 4; //Приведение в диапазон [0, 3]

                for (int i = 0; i < count; i++)
                {
                    //Производится обмен значениями по кругу для цветов граней
                    tmpChar = faceColors[Faces[3]];
                    for (int j = 3; j > 0; j--)
                    {
                        faceColors[Faces[j]] = faceColors[Faces[j - 1]];
                    }
                    faceColors[Faces[0]] = tmpChar;

                    //И для массива startPositions
                    tmpInt = startPositions[Faces[3]];
                    for (int j = 3; j > 0; j--)
                    {
                        startPositions[Faces[j]] = startPositions[Faces[j - 1]];
                    }
                    startPositions[Faces[0]] = tmpInt;

                    //Производится соответствующий обмен значениями габаритов
                    switch (rotationAxis)
                    {
                        case Axis.X: SwapMeasure(Axis.Y, Axis.Z); break;
                        case Axis.Y: SwapMeasure(Axis.X, Axis.Z); break;
                        case Axis.Z: SwapMeasure(Axis.X, Axis.Y); break;
                    }
                }
            }

            private bool Rotate(byte from, byte to)
            {
                //Перемещает грань с позиции from на позицию to

                int fromIndex, toIndex; //Индексы from и to в rotationDictionary
                foreach (Axis axe in Enum.GetValues(typeof(Axis)))
                {
                    if (allowedAxes[axe])
                    {
                        if (rotationDictionary[axe].Contains(from) && rotationDictionary[axe].Contains(to))
                        {
                            fromIndex = rotationDictionary[axe].Select((value, index) => new { value, index })
                                                       .Where(pair => pair.value == from)
                                                       .Select(pair => pair.index)
                                                       .First();
                            toIndex = rotationDictionary[axe].Select((value, index) => new { value, index })
                                                       .Where(pair => pair.value == to)
                                                       .Select(pair => pair.index)
                                                       .First();

                            //Вызывает метод вращения на нужное число оборотов
                            Rotate(axe, toIndex - fromIndex);
                            return true;
                        }
                    }
                }

                return false;
            }

            private void SwapMeasure(Axis A, Axis B)
            {
                //Своп габаритов по осям A, B
                Measure[A] = Measure[A] + Measure[B];
                Measure[B] = Measure[A] - Measure[B];
                Measure[A] = Measure[A] - Measure[B];
            }

            #endregion

            public bool Mask(string mask)
            {
                //Маска вида "C..C.." для удобной классификации кусков
                //В этой маске точка означает любое содержимое, а С - содержимое-цвет
                for (int i = 0; i < 6; i++)
                {
                    if (faceColors[i] == '.' && mask[i] != '.')
                    {
                        return false;
                    }
                }
                return true;
            }

            public bool AxisMask(Axis axis, string axisMask)
            {
                //Маска вида "C." для удобной классификации кусков
                //В этой маске точка означает любое содержимое, а С - содержимое-цвет
                //Применяется только по одной оси
                for (int i = 0; i < 2; i++)
                {
                    if (faceColors[(int)axis * 2 + i] == '.' && axisMask[i] != '.')
                    {
                        return false;
                    }
                }
                return true;
            }

            public int AxisMask(string axisMask)
            {
                //Маска вида "C." для удобной классификации кусков
                //В этой маске точка означает любое содержимое, а С - содержимое-цвет
                //Считается количество осей, грани по которым удовлетворяют маске
                int result = 0;
                foreach (Axis axis in Enum.GetValues(typeof(Axis)))
                    if (AxisMask(axis, axisMask))
                        result++;
                return result;
            }

            public void Print(TextWriter textWriter)
            {
                //Вывод данных

#if DEBUG
                //Для отладки
                textWriter.Write("{0} {1} {2} ", this.Measure[Axis.X], this.Measure[Axis.Y], this.Measure[Axis.Z]);
                foreach (char c in this.faceColors)
                    textWriter.Write(c);
                textWriter.Write(" {0} {1} \t", (int)this.type, this.isAxisPiece);
#endif
                
                //По условию
                textWriter.WriteLine("{0} {1} {2} {3} {4}", 
                    Faces[startPositions[0]], Faces[startPositions[2]],
                    this.coordinates[0], this.coordinates[1], this.coordinates[2]);
            }
        }
    }
}