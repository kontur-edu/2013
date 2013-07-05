using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssemblingBoxes
{
    /*Передняя, нижняя и левая грани в контексте комментариев называются "лицевыми". Кусочки - деталями. 
     * Исходный параллелепипед - основным. Идея заключается в разбиении области на ячейки, размеры 
     * которых соответствуют расстоянию между распилами.
     Из усложнений задача работает для 100000 кучков (пробовал запускать на собственном тесте с распилами по 
     * двум сторонам и без поворотов для (401)*(301) кусочков), но к сожалению не за 0.5 секунды*/


    /// <summary>
    /// набор именновых констант отождествляемый с передней,нижней и левой гранями.
    /// В некоторых случаях также отождествяется с номером координаты, так ось ОХ параллельна ребру, 
    ///  соединяющему переднюю и заднюю грань, OY - нижнюю и верхнюю, OZ - левую и правую
    ///  </summary>
    enum Faces { Front, Down, Left, NUMB_FACES };

    /// <summary>
    /// Основной класс решающий задачу.
    /// </summary>
    class CurrentTask
    {
        public static char[] COLOR = { 'R', 'O', 'Y', 'G', 'B', 'V' };         // массив с буквами обозначаюзими цвета 
        public static char[] LETTERS_FOR_FACE = { 'F', 'B', 'D', 'U', 'L', 'R' };  // буквы для граней параллелепипеда
        public const int NUMBERS_OF_FACES = 6;                      // число граней

        public readonly int N;      // число деталий
        public ColorBox mainBox;    // основной параллелепипед
        /// <summary>
        /// конструктор класса. Читает информацию о основном параллелепипеде и создает его. А также запоминает число деталей. 
        /// </summary>
        public CurrentTask()
        {
            int[] dimensions;
            char[] faceColors;

            read(out dimensions, out faceColors);
            mainBox = new ColorBox(dimensions, faceColors);

            N = Int32.Parse(Console.ReadLine());
        }
        /// <summary>
        /// меняет местами значения аргументов
        /// </summary>
        /// <typeparam name="T">универсальный тип</typeparam>
        /// <param name="a">первый аргумент</param>
        /// <param name="b">второй аргумент</param>
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp;
            temp = a;
            a = b;
            b = temp;
        }
        /// <summary>
        /// Читает информацию с экрана. и записывает в массивы
        /// </summary>
        /// <param name="dimensions">Массив с предполагаемыми размерами</param>
        /// <param name="faceColors">Массив с предполагаемыми цветами граней</param>
        public void read(out int[] dimensions, out char[] faceColors)
        {
            // читаем строку
            string[] tokens = Console.ReadLine().Split(' ');

            //инцилизируем массив с размерами параллелепипеда и заполняем его
            dimensions = new int[tokens.Length - 1];
            for (int i = 0; i < dimensions.Length; i++)
            {
                dimensions[i] = Int32.Parse(tokens[i]);
            }

            //инцилизируем массив с цветами граней параллелепипеда и заполняем его
            faceColors = new char[NUMBERS_OF_FACES];
            for (int i = 0; i < NUMBERS_OF_FACES; i++)
            {
                faceColors[i] = tokens[tokens.Length - 1][i];
            }
        }

        static void Main(string[] args)
        {
            // создаем объект основного класса
            CurrentTask A = new CurrentTask();
            // инциализируем сетку
            Grid myGrid = new Grid(A.mainBox);
            // инициализируем массив с деталями
            Boxes[] output = new Boxes[A.N];
            // цикл по номерам деталей
            for (int i = 0; i < A.N; i++)
            {
                int[] dimensions;
                char[] faceColors;
                // читаем информацию
                A.read(out dimensions, out faceColors);
                // записываем в массив
                output[i] = new Boxes(dimensions, faceColors, i);
                // определяем колличество цветных непараллельных граней и (если возможно) лицевые грани
                output[i].setFacialFaces(A.mainBox);
                // цветных не меньше двух
                if (output[i].numberOfPaintedFaces >= 2)
                {
                    // корректируем массив с размерами согласно располажению лицевых
                    output[i].correctDimensionAfterRotate();

                    if (output[i].numberOfPaintedFaces == 3)
                    {
                        // для трех цветов (угловая деталь) можем определить её расположение
                        myGrid.itsTricolorBox(output[i]);
                    }
                    else
                    {
                        // для двух цветов добавляем отрезок для нашей сетки
                        myGrid.addInterval(output[i]);
                    }
                }
            }

            // задаем сетку
            myGrid.meshing();
            // идем по массиву с деталями
            for (int i = 0; i < output.Length; i++)
            {
                // деталь не готова, находим ей место
                if (output[i].isAlready == false)
                    myGrid.determineCoordinates(output[i]);
                // выводим информацию
                output[i].write();
            }
        }
    };


    /// <summary>
    /// Класс цветных параллелепипедов. Обладает полями хранящими размеры и цвета граней.
    /// </summary>
    class ColorBox
    {

        public int[] dimensions;        // поле хранящее размеры
        public char[] faceColors;       // поле хранящее цвета граней

        /// <summary>
        /// Конструктор класса. Создающий "цветной параллелепипед" по размерам и цветам граней  
        /// </summary>
        /// <param name="dimensions">массив с размерами параллелепипеда</param>
        /// <param name="faceColors">массив с цветами граней</param>
        public ColorBox(int[] dimensions, char[] faceColors)
        {
            this.dimensions = dimensions;
            this.faceColors = faceColors;
        }

        /// <summary>
        /// По номеру грани возвращает номер ей параллельной.
        /// </summary>
        /// <param name="face">Номер грани</param>
        /// <returns>номер грани параллельной грани с номером аргумента</returns>
        public int parallelFace(int face)
        {
            return face - 2 * (face % 2) + 1;
        }
        /// <summary>
        /// По номерам двух "лицевых" непараллельных граней, возвращает номер третьей "лицевой" непараллельной им обеим
        /// </summary>
        /// <param name="firstFace">Первая грань</param>
        /// <param name="secondFace">Вторая грань</param>
        /// <returns>Грань непараллельная аргументам</returns>
        public static int thirdFace(int firstFace, int secondFace)
        {
            return 3 - (firstFace + secondFace);
        }

    };

    /// <summary>
    /// Класс параллелепипедов,отождествляемый с деталями. Кроме полей суперкласса "ColorBox",
    /// обладает полями хранящими позицию в файле, координаты. А также номера граней, которые станут 
    /// "лицевыми" при правильном расположении детали в основном параллелепипеде
    /// </summary>
    class Boxes : ColorBox
    {

        #region Поля класса

        private readonly int positionInFile;    // позиция в исходном файле
        public int[] coordinates;       // координаты после расположения в основном параллелепипеде
        public int[] facialFaces;     //  Номера граней, которые станут передней,нижней и левой при правильном расположении 

        #endregion

        #region Свойства класса

        public int numberOfPaintedFaces     // число непараллельных окрашенных граней
        {
            get;
            private set;
        }
        public bool isAlready       // объект готов, если у него определенны лицевые грани и координаты в исходном параллелепипеед
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Конструктор класса. Для задания размеров и цветов 
        /// граней используется конструктор суперкласса.
        /// </summary>
        /// <param name="dimensions">Размеры</param>
        /// <param name="faceColors">Цвета граней</param>
        /// <param name="positionInFile">Позиция в выходном файле</param>
        public Boxes(int[] dimensions, char[] faceColors, int positionInFile)
            : base(dimensions, faceColors)
        {
            //задаем позицию
            this.positionInFile = positionInFile;

            // инциализируем остальные поля и свойства некорректными значениями для упрощения их проверки на "пустоту"
            numberOfPaintedFaces = -1;
            coordinates = new int[(int)Faces.NUMB_FACES];
            facialFaces = new int[(int)Faces.NUMB_FACES];
            for (Faces j = Faces.Front; j < Faces.NUMB_FACES; j++)
            {
                coordinates[(int)j] = -1;
                facialFaces[(int)j] = -1;
            }
            isAlready = false;
        }

        #region Методы с помощью которых задаются коорднаты

        /// <summary>
        /// Метод позволяющий определить координаты (все или лишь некоторые) по цветам граней детали 
        /// </summary>
        /// <param name="mainBox">Основной параллелепипед</param>
        public void setCoordinatesByColor(ColorBox mainBox)
        {
            for (Faces i = Faces.Front; i < Faces.NUMB_FACES; i++)  // Для каждой из "лицевых" граней
            {
                if (getColorFacialFaces(i) == mainBox.faceColors[2 * (int)i])     /* смотрим совпадает ли цвет грани детали определенной как 
                                                                              "лицевая" с цветом лицевой стороной основного параллелепипеда*/
                {
                    // если да, то координата по этой оси равна 0
                    coordinates[(int)i] = 0;
                    continue;
                }
                if (getColorFacialParallelFaces(i) == mainBox.faceColors[2 * (int)i + 1])  /*совпадает цвет грани параллельной лицевой с 
                                                                                            такой же в основном параллелепипеде*/
                {
                    /* координата равна размерности основного параллелепипеда по                                                                    
                     соответствующей оси минус размерность детали*/
                    coordinates[(int)i] = mainBox.dimensions[(int)i] - dimensions[(int)i];
                }
            }
        }
        #endregion

        #region Определение "лицевых" (передней,нижней и левой) граней детали. А также её правильный поворот.
        /// <summary>
        /// Определение "лицевых" граней по цветам стенок детали
        /// (Если цветных граней неменьше двух, то поворот детали можно однозначно определить) 
        /// А также подсчет числа цветных непараллельных граней детали.
        /// </summary>
        /// <param name="mainBox">Основной параллелепипед</param>
        public void setFacialFaces(ColorBox mainBox)
        {
            numberOfPaintedFaces = 0;
            for (int i = 0; i < faceColors.Length; i++) // проходим по всем граням детали
            {
                for (Faces j = Faces.Front; j < Faces.NUMB_FACES; j++)  // и по названиям всех лицевых
                {
                    if (facialFaces[(int)j] != -1)   // проверяем не определена ли уже лицевая грань с таким названием
                        continue;

                    if (faceColors[i] == mainBox.faceColors[2 * (int)j]
                                 || faceColors[i] == mainBox.faceColors[2 * (int)j + 1])    /* цвет грани в детали совпадает с цветом лицевой 
                                                                                      грани основного параллелепипеда или ей параллельной*/
                    {
                        /*Если с лицевой основного значит это лицевая и её номер записывается в поле класса, 
                         * если нет, значит в поле записывается ей противоположная*/
                        facialFaces[(int)j] = (faceColors[i] == mainBox.faceColors[2 * (int)j]) ? i : parallelFace(i);
                        numberOfPaintedFaces++;         // число цветных увеличивается
                        i = 2 * (i / 2);                // ей параллельную в детали уже можно не смотреть 
                        break;                          // совпадает ли цвет с остальными можно не смотреть
                    }
                }
            }
            // число цветных непараллельных цветных граней только две, значит определим третью другим способом
            if (numberOfPaintedFaces == 2)
            {
                unknownFace();
            }
        }

        /// <summary>
        /// поворот вокруг оси OX
        /// </summary>
        /// <param name="faces">Массив содержащий номера граней</param>
        private void rotateOX(int[] faces)
        {
            int temp = faces[2];
            faces[2] = faces[5];
            faces[5] = faces[3];
            faces[3] = faces[4];
            faces[4] = temp;
        }
        private void rotateOY(int[] faces)
        {
            int temp = faces[0];
            faces[0] = faces[5];
            faces[5] = faces[1];
            faces[1] = faces[4];
            faces[4] = temp;
        }
        private void rotateOZ(int[] faces)
        {
            int temp = faces[0];
            faces[0] = faces[3];
            faces[3] = faces[1];
            faces[1] = faces[2];
            faces[2] = temp;
        }

        /// <summary>
        /// Определяет по лицевой грани вокруг какой оси следует сделать поворот и делает его.
        /// Так для Передней грани это будет ось OX, нижней - OY ,левой - OZ
        /// </summary>
        /// <param name="axisOfRotation">грань, которая остается на месте</param>
        /// <param name="faces">массив содержащий номера граней</param>
        private void whatRotate(Faces axisOfRotation, int[] faces)
        {
            switch (axisOfRotation)
            {
                case Faces.Front:
                    rotateOX(faces);
                    break;
                case Faces.Down:
                    rotateOY(faces);
                    break;
                case Faces.Left:
                    rotateOZ(faces);
                    break;
            }
        }
        /// <summary>
        /// Определяет какая грань детали является недостающей лицевой, когда две известны
        /// Для этого производится комбинация поворотов, до тех пор пока известные  лицевые грани
        /// не встанут на свои места. Тогда неизвестная грань также необходимо должна встать на свое место
        /// </summary>
        public void unknownFace()
        {
            // массив с номерами граней детали (0,1 - передняя, задняя; 2,3 - нижняя, верхняя; 4,5 - левая, правая)
            int[] faces = new int[] { 0, 1, 2, 3, 4, 5 };

            //определяем известные грани
            Faces firstKnownFace = (facialFaces[(int)Faces.Front] != -1) ? Faces.Front : Faces.Down;
            Faces secondKnownFace = (facialFaces[(int)Faces.Left] != -1) ? Faces.Left : Faces.Down;

            // до тех пор пока номер первой известной лицевой грани не окажется на своем месте
            // (передней на 0, нижней на 2, левой на 4ом)
            while (faces[2 * (int)firstKnownFace] != facialFaces[(int)firstKnownFace])
            {
                // если первая известная грань не стоит на месте себе параллельной
                if (facialFaces[(int)firstKnownFace] / 2 != (int)firstKnownFace)
                {
                    // то вращаем вокруг оси не совпадающей ни с её местом в детали, ни с её местом в большом прямоугольнике
                    whatRotate((Faces)thirdFace((int)firstKnownFace, facialFaces[(int)firstKnownFace] / 2), faces);
                }
                else
                {
                    // первая известная грань стоит на месте параллельной ей грани
                    // вращаем вокруг второй известной
                    whatRotate(secondKnownFace, faces);
                }
            }
            // пока номер второй известной лицевой грани не окажется на своем месте
            while (faces[2 * (int)secondKnownFace] != facialFaces[(int)secondKnownFace])
            {
                // вращаем вокруг первой известной уже вставшей на свое место
                whatRotate(firstKnownFace, faces);
            }
            int third = thirdFace((int)firstKnownFace, (int)secondKnownFace);   // определяем какая неизвестная
            facialFaces[third] = faces[2 * third];    // записываем её номер в детали до поворота
        }
        /// <summary>
        /// Корректирует размеры детали после её поворота,когда были определены все лицевые грани.
        /// </summary>
        public void correctDimensionAfterRotate()
        {
                dimensions = new int[] { dimensions[facialFaces[0] / 2], dimensions[facialFaces[1] / 2], dimensions[facialFaces[2] / 2] };
        }
        /// <summary>
        /// меняет местами размеры детали и лицевые грани
        /// </summary>
        /// <param name="firstCoordinate">перая координата</param>
        /// <param name="secondCoordinate">вторая координата</param>
        public void rotate(int firstCoordinate, int secondCoordinate)
        {
            // меняем местами размеры
            CurrentTask.Swap(ref dimensions[firstCoordinate], ref dimensions[secondCoordinate]);
            // меняем местами лицевые грани
            CurrentTask.Swap(ref facialFaces[firstCoordinate], ref facialFaces[secondCoordinate]);
            // корректируем одну по двум другим
            facialFaces[secondCoordinate] = -1;
            unknownFace();
        }
        #endregion

        #region методы помогающие получить по названию лицевой грани цвета граней детали
        /// <summary>
        /// Определяет цвет лицевой стороны по её названию. 
        /// </summary>
        /// <param name="face">название лицевой стороны</param>
        /// <returns>Возвращает цвет лицевой стороны. Если лицевая сторона не известна, 
        /// то возвращается символ ' ' </returns>
        public char getColorFacialFaces(Faces face)
        {
            if (facialFaces[(int)face]==-1)
                return ' ';
            return faceColors[facialFaces[(int)face]];
        }
        /// <summary>
        /// По названию лицевой грани определяет цвет стороны, ей паралелльной. 
        /// </summary>
        /// <param name="face">название лицевой стороны</param>
        /// <returns>Возвращает цвет стороны. Если лицевая сторона не известна, 
        /// то возвращается символ ' ' </returns>
        public char getColorFacialParallelFaces(Faces face)
        {
            if (facialFaces[(int)face]==-1)
                return ' ';
            return faceColors[parallelFace(facialFaces[(int)face])];
        }

        #endregion

        /// <summary>
        /// Метод определяющий в каком углу основного параллепипеда находится деталь. И корректирующий число распилов
        /// </summary>
        /// <returns>возвращает количество координат равных нулю</returns>
        public int whereThisBox(int[] numberOfCut)
        {
            int numbZeroCoord = 0;  // число нулевых координат
            int nonzeroCoord = -1; // ненулевая координата
            for (int i = 0; i < coordinates.Length; i++)
            {
                if (coordinates[i] == 0)
                    numbZeroCoord++;
                else
                    nonzeroCoord = i;
            }
            // если нулевых координат ровно две, значит есть угловая деталь 
            // отличная от содержащей начало координат
            if (numbZeroCoord == 2)
            {
                numberOfCut[nonzeroCoord]++;    // корректируем число распилов
            }
            return numbZeroCoord;
        }
        /// <summary>
        /// Вывод посчитанных значений для детали
        /// </summary>
        public void write()
        {
            // выводим буквы граней
            for (int i = 0; i < 2; i++)
            {
                Console.Write(CurrentTask.LETTERS_FOR_FACE[facialFaces[i]] + " ");
            }
            // координаты детали
            for (int i = 0; i < coordinates.Length - 1; i++)
            {
                Console.Write(coordinates[i] + " ");
            }
            Console.WriteLine(coordinates[coordinates.Length - 1]);
        }
    };

    /// <summary>
    /// Класс интервалов. Хранящих длину и значение левого края. Реализует интерфейс IComparable
    /// </summary>
    
    class Interval : IComparable<Interval>
    {
        // свойство хранящее длину
        public int lenght 
        {
            get;
            private set;
        }
        // свойство хранящее значение левого края
        public int leftEdge
        {
            get;
            set;
        }  

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="lenght">длина</param>
        public Interval(int lenght)
        {
            this.lenght = lenght;
        }
        /// <summary>
        /// Метод интерфейса IComparable. 
        /// </summary>
        /// <param name="other">Другой интервал с которым сравниваем</param>
        /// <returns>Возвращает положительное число, если объект должен быть больше аргумента,
        /// отрицательно, если меньше, и 0 если должны быть равны.</returns>
        public int CompareTo(Interval other)
        {   
            // сравнивем по значениям длины
            return lenght - other.lenght;
        }
    };

    /// <summary>
    /// Наследник класса LinkedList. Узлы хранятся по возрастанию значений и могут повторяться.
    /// </summary>
    /// <typeparam name="T">универсальный тип должен реализовывать интерфейс IComparable</typeparam>
    class SortLinkedList<T> : LinkedList<T>
        where T : IComparable<T>
    {
        /// <summary>
        /// метод добавляющий узел по значению, не нарушаяя 
        /// порядок расположения узлов списка по возрастанию значений
        /// </summary>
        /// <param name="elementValue">Елемент который хотим добавить</param>
        /// <returns>Ссылка на новый узел</returns>
        public LinkedListNode<T> Add(T elementValue)
        {
            // начинаем с первого элемента
            LinkedListNode<T> current = this.First;
            // список пуст
            if (current == null)
            {
                // просто добавляем элемент на первое место
                return AddFirst(elementValue);
            }
            // пока есть следующий элемент и он меньше чем добавляемый
            while (current.Next != null && current.Next.Value.CompareTo(elementValue) < 0)
            {
                // переходим на следующий
                current = current.Next;
            }
            // текущий меньше добавляемого
            if (current.Value.CompareTo(elementValue) < 0)
                // добавляем после текущего
                return AddAfter(current, elementValue);
            else
                // иначе, перед текущим
                return AddBefore(current, elementValue);
        }
    };

    /// <summary>
    /// Класс "диапазон индексов" хранящий нижний и верхнюю границу для индексов. 
    /// </summary>
    class IndexRange
    {
        // нижняя граница
        public int lower
        {
            get;
            private set;
        }
        // верхняя граница
        public int upper
        {
            get;
            private set;
        }
        //конструктор
        public IndexRange(int value)
        {
            lower = (upper = value);
        }
        // сдвиг каждой границы вправо на 1
        public void shiftRight()
        {
            lower++;
            upper++;
        }
        /// <summary>
        /// Функция расширяющая диапозон индексов. Так что все элемнты массива
        /// с индексами от lower до upper равны второму аргументы (равны согласно 
        /// функции CompareTo реализации интерфейса IComparable) 
        /// </summary>
        /// <typeparam name="T">универсальный тип, должен реализовывать интерфейс IComparable</typeparam>
        /// <param name="array">массив</param>
        /// <param name="Object">объект с которым сравниваются элементы массива</param>
        public void expand<T>(T[] array, T Object) where T : IComparable<T>
        {
            // пока предыдущий находится в пределах массива и подходит по размеру идем налево
            while (lower - 1 >= 0 && array[lower - 1].CompareTo(Object) == 0)
            {
                lower--;
            }
            // пока следующий находится в пределах массива и подходит по размеру идем направо
            while (upper + 1 < array.Length && array[upper + 1].CompareTo(Object) == 0)
            {
                upper++;
            }
        }
    };

    /// <summary>
    /// Класс "Ячейки". Задает трехмерный массив ячеек. 
    /// Каждая ячейка может быть пустой или не пустой
    /// </summary>
    class Cells
    {
        private bool[, ,] gridCells;        // массив ячеек
        /// <summary>
        /// Конструктор класса. Создает массив ячеек по его размерам
        /// </summary>
        /// <param name="firstSize">Размер по первой координате</param>
        /// <param name="secondSize">Размер по второй координате</param>
        /// <param name="thirdSize">Размер по третьей координате</param>
        public Cells(int firstSize, int secondSize, int thirdSize)
        {
            gridCells = new bool[firstSize, secondSize, thirdSize];
        }
        /// <summary>
        /// Ищет пустую ячейку в диапазоне заданном массивом "Диапазона индексов"
        /// </summary>
        /// <param name="indexes">Массив диапазона индексов</param>
        /// <returns>возвращает массив с индексами ячейки. Либо null, 
        /// если пустая так и не была найдена</returns>
        public int[] searchEmptyCell(IndexRange[] indexes)
        {
            // Идем по индексам, каждый индекс меняется в своем диапазоне, заданном соответствующим 
            // элементом в массиве indexes 
            for (int firstInd = indexes[0].lower; firstInd <= indexes[0].upper; firstInd++)
            {
                for (int secondInd = indexes[1].lower; secondInd <= indexes[1].upper; secondInd++)
                {
                    for (int thirdInd = indexes[2].lower; thirdInd <= indexes[2].upper; thirdInd++)
                    {
                        // если ячейка пуста, возвращаем её индексы
                        if (gridCells[firstInd, secondInd, thirdInd] == false)
                        {
                            gridCells[firstInd, secondInd, thirdInd] = true;
                            return new int[] { firstInd, secondInd, thirdInd };
                        }
                    }
                }
            }
            // иначе null
            return null;
        }

    };

    /// <summary>
    /// класс описывающий сетку, разбивающую внутреннюю область основного параллелепипеда
    /// А также методы заполняющие её(присваювающие координаты деталям)
    /// </summary> 
    class Grid
    {
        #region Поля класса
        private ColorBox mainBox;   // поля хранящее ссылку на основной параллелепипед
        private Boxes zeroBox;      // ссылка на деталь содержащую начало координат

        // массив отсортированных связных списков в которых будут хранится интервалы разбиения
        private SortLinkedList<Interval>[] tempAxis;
        
        // массив массивов хранящих разбиение (то же что и предыдущее поле, но массивы, а не связные списки)
        private Interval[][] axis;
        
        // массив с количеством разрезов по каждой координате
        private int[] numberOfCut;

        // Ячейки на которые разбивается наша область
        private Cells gridCells;

        #endregion

        /// <summary>
        /// Конструктор класса. Задается поле хранящее основной параллелепипед.
        /// Инциализируются поля-массивы
        /// </summary>
        /// <param name="mainBox">Основной параллелепипед</param>
        public Grid(ColorBox mainBox)
        {
            this.mainBox = mainBox;
            zeroBox = null;

            axis = new Interval[3][];
            tempAxis = new SortLinkedList<Interval>[3];
            for (int i = 0; i < tempAxis.Length; i++)
            {
                tempAxis[i] = new SortLinkedList<Interval>();
            }
            numberOfCut = new int[(int)Faces.NUMB_FACES];
        }

        #region Построение сетки
        /// <summary>
        /// Добавляет интервал на нужную ось. Так, если у детали цвета передней и нижней грани 
        /// совпадают с цветами передней и нижней основного параллелепипеда, то на ось OZ добавляется интервал
        /// </summary>
        /// <param name="detail">Деталь</param>
        public void addInterval(Boxes detail)
        {
            /* массив для лицевых граней детали, 
             цвета которых совпадают с цветами лицевых граней основного*/
            Faces[] facesWithLikeColor = new Faces[(int)Faces.NUMB_FACES - 1];
            // индекс для массива facialFace
            int indexForFacesWithLikeColor = 0;                   
            for (Faces i = Faces.Front; i < Faces.NUMB_FACES; i++)  // цикл по названиям лицевых
            {
                if (detail.getColorFacialFaces(i) == mainBox.faceColors[2 * (int)i])  // проверяем совпадают ли цвета м гранями основного
                {
                    // записываем название грани с совпадающим цветом в массив
                    facesWithLikeColor[indexForFacesWithLikeColor++] = i;
                }
            }
            if (indexForFacesWithLikeColor != (int)Faces.NUMB_FACES - 1)     // не нашлось двух граней с цветами такими же как 
                return;                                          //у лицевых основного - выходим

            int third = ColorBox.thirdFace((int)facesWithLikeColor[0], (int)facesWithLikeColor[1]);   // считаем третью
            tempAxis[third].Add(new Interval(detail.dimensions[third]));  // добавляем на ось третьей еще один отрезок
            numberOfCut[third]++;  // увеличиваем число разрезов по этой оси
        }
        /// <summary>
        /// Финальный этап для списка хранящего разбиение оси.
        /// Когда все интервалы найденны, и следовательно список окончательно отсортирован
        /// для каждого интервала задается левый край
        /// </summary>
        /// <param name="numberOfAxis">номер оси</param>
        private void tempAxisFinalize(int numberOfAxis)
        {
            // левый край первого отрезка равен размеру нулевой детали по соответствующей оси
            int leftEdge = zeroBox.dimensions[numberOfAxis];
            foreach (Interval at in tempAxis[numberOfAxis])
            {
                at.leftEdge = leftEdge;       // для кажого отрезка записываем его левый край
                leftEdge += at.lenght;        // левый край следующего будет равен левый край текущего+его длина
            }
        }
        /// <summary>
        /// Преобразовывает разбиения по осям из сортированных связных списков в массивы
        /// </summary>
        private void convertAxis()
        {
            for (int i = 0; i < tempAxis.Length; i++)
            {
                tempAxisFinalize(i);            // финальная обработка разбиения
                axis[i] = new Interval[tempAxis[i].Count];      // инциализация массива
                tempAxis[i].CopyTo(axis[i], 0);          // заполнение
                tempAxis[i] = null;
            }
            tempAxis = null;      // ссылка на массив временных списков разбиения зануляется
        }
        /// <summary>
        /// Задание сетки. Конвертирование разбиений по осям из списков в массивы.
        /// Создание ячеек сетки
        /// </summary>
        public void meshing()
        {
            convertAxis();          // конвертируем разбиение из списков в массивы
             // инциализация ячеек сетки
            gridCells = new Cells(numberOfCut[0] + 1, numberOfCut[1] + 1, numberOfCut[2] + 1);
        }
        #endregion

        /// <summary>
        /// Выбирает для детали нужный метод определения координат в зависимости от количества
        /// цветных непараллельных граней
        /// </summary>
        /// <param name="detail">Деталь</param>
        public void determineCoordinates(Boxes detail)
        {
            switch (detail.numberOfPaintedFaces)
            {
                case 2:
                    itsBicolorBox(detail);
                    break;
                case 1:
                    itsOneColoreBox(detail);
                    break;
                case 0:
                    itsColorlessBox(detail);
                    break;
            }
        }

        #region Методы определяющие координаты в зависимости от количества цветных граней
        /// <summary>
        /// У детали три непараллельные грани имеют цвета. (угловая деталь)
        /// </summary>
        /// <param name="detail">Деталь</param>
        public void itsTricolorBox(Boxes detail)
        {
            // задаем координаты
            detail.setCoordinatesByColor(mainBox);
            // отмечаем, что деталь обработана
            detail.isAlready = true;
            // смотрим в каком углу она находится. (whereThisBox корректирует количество распилов,и 
            // считает число нулевых координат)
            if (detail.whereThisBox(numberOfCut) == 3)
            {
                // все три координаты равны нулю - деталь содержит начало координат
                zeroBox = detail;
            }
        }
        /// <summary>
        /// Определяет координаты для двуцветной детали
        /// </summary>
        /// <param name="detail">Деталь</param>
        private void itsBicolorBox(Boxes detail)
        {
            detail.setCoordinatesByColor(mainBox);      // задаем те координаты которые мы можем определить по цвету
            searchCoordinatesByCell(detail);              //ищем пустую ячейку и по ней задаем координаты
        }
        /// <summary>
        /// Определяет координаты для одноцветной детали
        /// </summary>
        /// <param name="detail">деталь</param>
        private void itsOneColoreBox(Boxes detail)
        {
            int knownFace = -1;   // здесь будет хранится номер известной лицевой грани (ранее определенной) 
            int[] unknownFace = new int[2]; // здесь будут хранится номера еще неизвестных координат
            int indUnknown = 0;             // индекс массива с номерами неизвестных координат
 
            for (int i = 0; i < detail.facialFaces.Length; i++)
            {
                if (detail.facialFaces[i] != -1)
                {
                    knownFace = i;
                }
                else
                {
                    unknownFace[indUnknown++] = i;
                }
            }
            // задаем неизвестные лицевые грани (должны отличаться от известной и ей параллельной)
            detail.facialFaces[unknownFace[0]] = (detail.facialFaces[knownFace] / 2 != unknownFace[0]) ? 2 * unknownFace[0] : 2 * unknownFace[1];
            detail.facialFaces[unknownFace[1]] = -1;
            detail.unknownFace();                   // третью находим по первым двум
            detail.correctDimensionAfterRotate();   // корректируем размеры согласно лицевым граням
            detail.setCoordinatesByColor(mainBox);      // задаем те координаты которые мы можем определить по цвету
            int numberOfSwap = 0;                   // количество поворотов

            while (numberOfSwap <= 1)               // не больше одного (поменять местами размеры по неизвестным координатам )
            {
                if (searchCoordinatesByCell(detail) == false)         // ищем координаты по ячейке.
                {
                    numberOfSwap++;                                 // если не удалось, то поворачиваем
                    detail.rotate(unknownFace[0], unknownFace[1]);
                }
                else
                    break;
            }
        }
        /// <summary>
        /// определяет координаты для безцветной детали
        /// </summary>
        /// <param name="detail">Деталь</param>
        private void itsColorlessBox(Boxes detail)
        {
            //задаем лицевые грани
            for (int i = 0; i < detail.facialFaces.Length - 1; i++)
            {
                detail.facialFaces[i] = 2 * i;
            }
            // корректируем третью по двум
            detail.facialFaces[2] = -1;
            detail.unknownFace();
            detail.correctDimensionAfterRotate();// корректируем размеры согласно выбранным граням
            for (int i = 0; i < 3; i++)             // можем выполнить шесть поворотов
                for (int j = 1; j >= 0; j--)
                {
                    if (searchCoordinatesByCell(detail) == false) // если найти координаты по ячейке не удалось, то поворачиваем 
                    {
                        detail.rotate( j + 1, j);
                    }
                    else
                        break;
                }
        }
        #endregion
        
        /// <summary>
        /// Ищет подходящую пустую ячейку(вызывая другой метод) и Определяет координаты по ней
        /// </summary>
        /// <param name="detail">Деталь</param>
        /// <returns>true если удалось определить координаты,false - иначе</returns>
        private bool searchCoordinatesByCell(Boxes detail)
        {
            // диапазон в котором могут меняться индексы ячеек
            IndexRange[] indexRangeSuitableCells = searchRangeCells(detail);
            if (indexRangeSuitableCells == null) // неудалось определить
            {
                return false;
            }
            int[] indexEmptyCell = gridCells.searchEmptyCell(indexRangeSuitableCells);  // ищем пустую ячейку
            if (indexEmptyCell != null)     // удалось найти
            {
                for (int i = 0; i < detail.coordinates.Length; i++) // записываем соответствующие значения координат
                {
                    if (detail.coordinates[i] == -1)
                        detail.coordinates[i] = axis[i][indexEmptyCell[i] - 1].leftEdge;
                }
                // помечаем как готовый
                detail.isAlready = true;
            }
            return detail.isAlready;
        }
        /// <summary>
        /// Метод ищущий диапазон индексов ячеек по размеру детали
        /// </summary>
        /// <param name="detail">Деталь</param>
        /// <returns>Массив с диапазоном индексов. Либо null, если найти не удалось</returns>
        private IndexRange[] searchRangeCells(Boxes detail)
        {
            // инициализируем массив
            IndexRange[] result = new IndexRange[detail.coordinates.Length];
            for (int i = 0; i < detail.coordinates.Length; i++)
            {
                // если какая то координата уже определенна, значит лежит на границ
                if (detail.coordinates[i] >= 0)
                {
                    int indexValue = (detail.coordinates[i] == 0) ? 0 : numberOfCut[i];
                    result[i] = new IndexRange(indexValue);
                }
                else
                {
                    // иначе ищем отрезки подходящего размера, тем самым задаем диапазон индексов
                    result[i] = searchIndexRangeForDimensionOnAxis(detail.dimensions[i], i);
                    if (result[i] == null)
                        return null;
                }
            }
            return result;
        }
        /// <summary>
        /// Ищем нижний и верхний индекс среди подходящих по размеру отрезков на данной оси
        /// </summary>
        /// <param name="dimensionDetail">Необходимая длина отрезка</param>
        /// <param name="numberAxis">Номер оси на которой ищем подходящую длину отрезка</param>
        /// <returns>нижний и верхний индекс диапозона совпадающих отрезков. Если таких нет, то null</returns>
        private IndexRange searchIndexRangeForDimensionOnAxis(int dimensionDetail, int numberAxis)
        {
            // создаем интервал необходимого размера.
            Interval intervalDetail = new Interval(dimensionDetail);

            // ищем отрезок нужного размера
            int index = Array.BinarySearch<Interval>(axis[numberAxis], intervalDetail);
            if (index < 0)
            {
                // не нашлось
                return null;
            }
            // инциализируем объект класса "Диапазон индексов"
            IndexRange result = new IndexRange(index);
            // находи границы диапазона
            result.expand<Interval>(axis[numberAxis], intervalDetail);
            // делаем сдвиг (поскольку угловые не входят в список отрезков)
            result.shiftRight();
            return result;
        }
        
    };
}
