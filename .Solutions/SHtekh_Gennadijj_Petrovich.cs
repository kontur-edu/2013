using System;
using System.Collections.Generic;
using System.Linq;

namespace ConturInternat
{
    

    #region 
    //Спецкласс для вывода ответа на вопрос о поворотах. Повторяет все повороты исходного куба
    public static class ForAnwerSideRotator


    {
        private static Dictionary<Directions, Char> side2Char;

        private static void Reset()
        {
            side2Char = new Dictionary<Directions, char>();
            var chars = new char[] {'F', 'B', 'D', 'U', 'L', 'R'};
            for (int i = 0; i < 6; i++)
            {
                side2Char.Add(CubeUtils.Order[i], chars[i]);
            }
        }

        public static void Rotate(Rotates rotateTo)
        {
            for (var j = 0; j < 4; j++)
            {
                SwapChars(CubeUtils.RotateRings[rotateTo][0], CubeUtils.RotateRings[rotateTo][j]);
            }
        }

        private static void SwapChars(Directions from, Directions to)
        {
            var temp = side2Char[from];
            side2Char[from] = side2Char[to];
            side2Char[to] = temp;
        }

        public static Dictionary<Directions, Char> StateAfterRotates(List<Rotates> rotates)
        {
            Reset();
            foreach (var rotate in rotates)
            {
                Rotate(rotate);
            }
            return side2Char;
        }

    }
    #endregion


    public enum Colores
    {
        Red = 'R',
        Orange = 'O',
        Yelow = 'Y',
        Green = 'G',
        Blue = 'B',
        Violence = 'V',
        Uncolored = '.'
    }

    //change 'V'

    public enum Rotates
    {
        Forward,
        Side,
        Around
    }

    public enum Directions
    {
        Front = 0,
        Back = 1,
        Bottom = 2,
        Top = 3,
        Left = 4,
        Right = 5
    }
    
    public struct Vector  //Для фиксации состояний, связанных с координатами
    {
        public int SizeByDirect(Directions direct)
        {
            if (direct.Equals(Directions.Front))
            {
                return width;
            }
            if (direct.Equals(Directions.Left))
            {
                return hight;
            }
            if (direct.Equals(Directions.Bottom))
            {
                return depth;
            }
            throw new Exception("Не должно такого быть");
        }

        public Vector(int[] sizes)
        {
            width = sizes[0];
            depth = sizes[1];
            hight = sizes[2];
        }

        public int width;
        public int depth;
        public int hight;
    }

    public static class CubeUtils  //Наборы знаний о порядке сторон, поворотах, определяющих свойствах фигур и тд.
    {
        public static Directions[] SideRing = GetSideRing();
        public static Directions[] ForwardRing = GetForwardRing();
        public static Directions[] AroundRing = GetAroundRing();
        public static Directions[] Order = GetOrder();
        public static Directions[] ZeroSides = GetZeroSides();

        public static Dictionary<Rotates, Directions[]> RotateRings = GetRotateRings();

        public static Rotates[] RotatesSet = GetRotates();

        public static Directions ReverseDirection(Directions direction)
        {
            var counter = 0;
            for (int i = 0; i < 6; i++)
            {
                if (Order[i].Equals(direction))
                {
                    counter = i + 1;
                    break;
                }
            }
            if (counter%2 == 0)
            {
                return Order[counter - 2];
            }
            else
            {
                return Order[counter];
            }
        }

        public static Directions[] Rotates2Ring(Rotates rotateTo)
        {
            if (rotateTo.Equals(Rotates.Around))
            {
                return AroundRing;
            }

            if (rotateTo.Equals(Rotates.Side))
            {
                return SideRing;
            }

            if (rotateTo.Equals(Rotates.Forward))
            {
                return ForwardRing;
            }
            return null;
        }

        public static Directions Rotates2InvariantDirection(Rotates rotate)
        {
            if (rotate.Equals(Rotates.Around))
            {
                return Directions.Top;
            }

            if (rotate.Equals(Rotates.Side))
            {
                return Directions.Back;
            }

            if (rotate.Equals(Rotates.Forward))
            {
                return Directions.Right;
            }
            return Directions.Top;
        }

        public static Rotates Direction2InvariantRotate(Directions direct)
        {
            if (direct.Equals(Directions.Front) || direct.Equals(Directions.Back))
            {
                return Rotates.Side;
            }

            if (direct.Equals(Directions.Left) || direct.Equals(Directions.Right))
            {
                return Rotates.Forward;
            }

            if (direct.Equals(Directions.Bottom) || direct.Equals(Directions.Top))
            {
                return Rotates.Around;
            }
            return Rotates.Side;
            throw new Exception("Такого не должно случаться");
        }

        private static Directions[] GetAroundRing()
        {
            return new Directions[]
                {
                    Directions.Front,
                    Directions.Left,
                    Directions.Back,
                    Directions.Right
                };
        }

        private static Rotates[] GetRotates()
        {
            return new Rotates[]
                {
                    Rotates.Around,
                    Rotates.Side,
                    Rotates.Forward
                };
        }

        private static Directions[] GetSideRing()
        {
            return new Directions[]
                {
                    Directions.Bottom,
                    Directions.Left,
                    Directions.Top,
                    Directions.Right
                };
        }

        private static Directions[] GetForwardRing()
        {
            return new Directions[]
                {
                    Directions.Bottom,
                    Directions.Front,
                    Directions.Top,
                    Directions.Back
                };
        }

        private static Directions[] GetZeroSides()
        {
            return new Directions[]
                {
                    Directions.Front,
                    Directions.Bottom,
                    Directions.Left,
                };
        }

        private static Directions[] GetOrder()
        {
            return new Directions[]
                {
                    Directions.Front,
                    Directions.Back,
                    Directions.Bottom,
                    Directions.Top,
                    Directions.Left,
                    Directions.Right,
                };
        }

        private static Dictionary<Rotates, Directions[]> GetRotateRings()
        {
            var answer = new Dictionary<Rotates, Directions[]>();
            answer.Add(Rotates.Side, GetSideRing());
            answer.Add(Rotates.Around, GetAroundRing());
            answer.Add(Rotates.Forward, GetForwardRing());
            return answer;
        }

        public static Colores ColoresTryParse(char sym)
        {
            if (sym == 'R') return Colores.Red;
            if (sym == 'O') return Colores.Orange;
            if (sym == 'Y') return Colores.Yelow;
            if (sym == 'G') return Colores.Green;
            if (sym == 'B') return Colores.Blue;
            if (sym == 'V') return Colores.Violence;
            if (sym == '.') return Colores.Uncolored;
            return 0;
        }

        public static List<Cube> TakeAngles(List<Cube> cubeKeeper)
        {
            var angls = new List<Cube>();

            angls.AddRange(cubeKeeper.Where(cube => cube.NumberOfColoredSides == 3).
                                      Where(
                                          cube =>
                                          CubeUtils.Order.Where(cube.IsColored)
                                                   .All(direct => !cube.IsColored(CubeUtils.ReverseDirection(direct))
                                              )
                               ));

            angls.AddRange(cubeKeeper.Where(cube => cube.NumberOfColoredSides == 4).
                                      Where(
                                          cube =>
                                          CubeUtils.Order.Where(cube.IsColored)
                                                   .Count(
                                                       direct =>
                                                       cube.IsColored(CubeUtils.ReverseDirection(direct))) == 2
                               ));

            angls.AddRange(cubeKeeper.Where(cube => cube.NumberOfColoredSides > 4));

            cubeKeeper.RemoveAll(cubeForDel => angls.Any(alreadyGetted => cubeForDel.ID.Equals(alreadyGetted.ID)));

            return angls;
        }

        public static List<Cube> TakeEdges(List<Cube> cubeKeeper)
        {
            var answer = new List<Cube>();

            answer.AddRange(cubeKeeper.Where(cube => cube.NumberOfColoredSides == 2).
                                       Where(cube => CubeUtils.Order.All(
                                           (side =>
                                            !cube.IsColored(side) || !cube.IsColored(CubeUtils.ReverseDirection(side)))
                                                         )
                                )
                );
            answer.AddRange(cubeKeeper.Where(cube => cube.NumberOfColoredSides == 3));
            answer.AddRange(cubeKeeper.Where(cube => cube.NumberOfColoredSides == 4));

            cubeKeeper.RemoveAll(cubeForDel => answer.Any(alreadyGetted => cubeForDel.ID.Equals(alreadyGetted.ID)));
            return answer;
        }

        public static List<Cube> TakeSideCubes(List<Cube> cubeKeeper)
        {
            var answer = new List<Cube>();

            answer.AddRange(cubeKeeper.Where(cube => cube.NumberOfColoredSides == 1));

            answer.AddRange(cubeKeeper.Where(cube => cube.NumberOfColoredSides == 2).
                                       Where(cube => CubeUtils.Order.All(
                                           (side =>
                                            !cube.IsColored(side) || cube.IsColored(CubeUtils.ReverseDirection(side)))
                                                         )
                                )
                );

            cubeKeeper.RemoveAll(cubeForDel => answer.Any(alreadyGetted => cubeForDel.ID.Equals(alreadyGetted.ID)));
            return answer;
        }

    }

    public class PAPACube : Cube
    {
        private int DimensionCoeficient;
        private Dictionary<Directions, int> nextForExistingSquare;
        private Dictionary<Directions, int> lastForExistingSquare;
        private Dictionary<Directions, Dictionary<int, Dictionary<int, int>>> PlaceForSizeForExistingSquare;
        private Dictionary<Vector, List<int[]>> readyPlace;

        public void DesignateStartpointsForAngles(Cube cube) 
        {
            foreach (var direct in CubeUtils.ZeroSides)
            {
                var currentSize = cube.ShowSizes[direct];
                if (!PlaceForSizeForExistingSquare[direct].ContainsKey(currentSize))
                {
                    PlaceForSizeForExistingSquare[direct].Add(currentSize, new Dictionary<int, int>());
                }
                if (!PlaceForSizeForExistingSquare[direct][currentSize].ContainsKey(cube.Place[direct]))
                {
                    PlaceForSizeForExistingSquare[direct][currentSize].Add(cube.Place[direct], 0);
                }
                if (nextForExistingSquare[direct] == 0 ||
                    nextForExistingSquare[direct] > cube.ShowSizes[direct] + cube.Place[direct])
                    nextForExistingSquare[direct] = cube.ShowSizes[direct];
            }
        }

        private bool IsExistPlaceForEdge(Cube cube, Directions direct)
        {
            var sizeOnCurrentDirect = cube.ShowSizes[direct];
            //            var placeOnCurrentDirect = cube.Place[direct];

            if (!PlaceForSizeForExistingSquare[direct].ContainsKey(sizeOnCurrentDirect))
            {
                return false;
            }
            if (
                PlaceForSizeForExistingSquare[direct][sizeOnCurrentDirect].Keys.Any(
                    place => PlaceForSizeForExistingSquare[direct][sizeOnCurrentDirect][place] != 0))
            {
                return true;
            }
            return false;
        }

        private int ShowPlaceForEdgeOnDirect(Cube cube, Directions direct)
        {
            var sizeOnCurrentDirect = cube.ShowSizes[direct];
            return
                PlaceForSizeForExistingSquare[direct][sizeOnCurrentDirect].Keys.First(
                    place => PlaceForSizeForExistingSquare[direct][sizeOnCurrentDirect][place] != 0);
        }

        private void TakePlaceOnDirectForEdges(Cube cube, Directions direct)
        {
            var sizeOnCurrentDirect = cube.ShowSizes[direct];
            var placeOnCurrentDirect = cube.Place[direct];

            if (!PlaceForSizeForExistingSquare[direct].ContainsKey(sizeOnCurrentDirect))
            {
                PlaceForSizeForExistingSquare[direct].Add(sizeOnCurrentDirect, new Dictionary<int, int>());
            }

            if (PlaceForSizeForExistingSquare[direct][sizeOnCurrentDirect].ContainsKey(placeOnCurrentDirect))
            {
                PlaceForSizeForExistingSquare[direct][sizeOnCurrentDirect][placeOnCurrentDirect] -= 1;
            }
            else
            {
                PlaceForSizeForExistingSquare[direct][sizeOnCurrentDirect].Add(placeOnCurrentDirect,
                                                                               DimensionCoeficient - 1);
            }
        }

        public bool TryTakePlace(Cube cube)
        {
            var sizeVector = cube.GetSizeState();

            var posiblePoints = getUnbusyPoint(sizeVector);

            if (posiblePoints == null) return false;

            var counter = 0;
            Directions intrestingDirect = Directions.Top;

            int[] goodPoint=null;
             foreach (var posiblePoint in posiblePoints)
            {
                var flag = true;
                goodPoint = posiblePoint;
                for (int i = 0; i < 3; i++)
                {
                    if (cube.Place.ContainsKey(CubeUtils.ZeroSides[i]))
                    {
                        if (posiblePoint[i] != cube.Place[CubeUtils.ZeroSides[i]])
                        {
                            goodPoint = null;
                            flag = false;
                        }
                    }
                    if (posiblePoint[i] < 0)
                    {
                        goodPoint = null;
                        flag = false;
                    }

                }
                if (flag) break;
            }
            
            if (goodPoint!=null){
//                var goodPoint = posiblePoints.Find(place => place[counter].Equals(cube.Place[intrestingDirect]));
                posiblePoints.Remove(goodPoint);
                for (int i = 0; i < 3; i++)
                {
                    if (!cube.Place.ContainsKey(CubeUtils.ZeroSides[i]))
                    {
                        cube.Place.Add(CubeUtils.ZeroSides[i], goodPoint[i]);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<int[]> getUnbusyPoint(Vector sizeVector)
        {
            if (readyPlace.ContainsKey(sizeVector))
            {
                if (readyPlace[sizeVector].Count != 0)
                {
                    return readyPlace[sizeVector];
                }
                return null;
            }
            readyPlace.Add(sizeVector, new List<int[]>());

            IEnumerable<int> FrontHash;
            if (PlaceForSizeForExistingSquare[Directions.Front].ContainsKey(sizeVector.SizeByDirect(Directions.Front)))
            {
                FrontHash =
                    PlaceForSizeForExistingSquare[Directions.Front][sizeVector.SizeByDirect(Directions.Front)].Keys;
            }
            else
            {
                FrontHash = new HashSet<int> {-1};
            }

            IEnumerable<int> BottomHash;
            if (PlaceForSizeForExistingSquare[Directions.Bottom].ContainsKey(sizeVector.SizeByDirect(Directions.Bottom)))
            {
                BottomHash =
                    PlaceForSizeForExistingSquare[Directions.Bottom][sizeVector.SizeByDirect(Directions.Bottom)].Keys;
            }
            else
            {
                BottomHash = new HashSet<int> {-1};
            }

            IEnumerable<int> LeftHash;
            if (PlaceForSizeForExistingSquare[Directions.Left].ContainsKey(sizeVector.SizeByDirect(Directions.Left)))
            {
                LeftHash = PlaceForSizeForExistingSquare[Directions.Left][sizeVector.SizeByDirect(Directions.Left)].Keys;
            }
            else
            {
                LeftHash = new HashSet<int> {-1};
            }

            foreach (var frontHash in FrontHash)
            {
                foreach (var bottomHash in BottomHash)
                {
                    foreach (var leftHash in LeftHash)
                    {
//                        if ((frontHash != 0 && bottomHash != 0) || 
//                            (leftHash != 0 && bottomHash != 0) || 
//                            (frontHash != 0 && leftHash != 0))
                        readyPlace[sizeVector].Add(new int[] {frontHash, bottomHash, leftHash});
                    }
                }
            }
            return getUnbusyPoint(sizeVector);
            throw new Exception("Такого не бывает. :)");
        }

        public void placeMeByOrder(Cube cube)
        {
            foreach (var direct in CubeUtils.ZeroSides)
            {
                if (!cube.Place.ContainsKey(direct))
                {
                    var sizeOnDirect = cube.ShowSizes[direct];

                    int chosenPlace;
                    if (IsExistPlaceForEdge(cube, direct))//Основная ошибка здесь. Связана с тем, что система не видит разницы между разными рёбрами и может попытаться перекинуть куб из одного ребра в другое, произойдёт коллизия, которая не отслеживается. Нужно привязать поиск свободного места к конктерному ребру.
                    {
                        chosenPlace = ShowPlaceForEdgeOnDirect(cube, direct);

                        cube.Place.Add(direct, chosenPlace);
                        TakePlaceOnDirectForEdges(cube, direct);

                        return;
                    }

                    chosenPlace = nextForExistingSquare[direct];

                    nextForExistingSquare[direct] += sizeOnDirect;

                    cube.Place.Add(direct, chosenPlace);
                    TakePlaceOnDirectForEdges(cube, direct);

                    return;
                }
            }
        }

        public void placeMeByColor(Cube cube)
        {
            var sideAgregator = new Dictionary<Directions, int>();
            for (int i = 0; i < 6; i++)
            {
                var side = CubeUtils.Order[i];
                sideAgregator.Add(side, -1);
                if (cube.IsColored(side))
                {
                    sideAgregator[side] = 1;
                }
            }

            var places = cube.Place;

            for (int i = 0; i < 6; i += 2)
            {
                var side = CubeUtils.Order[i];
                if (sideAgregator[CubeUtils.ReverseDirection(side)] == 1)
                {
                    places.Add(side, this.Sizes[side] - cube.ShowSizes[side]);
                }
                if (sideAgregator[side] == 1)
                {
                    if (!places.ContainsKey(side))
                    {
                        places.Add(side, 0);
                    }
                    else
                    {
                        places[side] = 0;
                    }
                }
            }
        }

        public PAPACube(String inputLine, int numberOfAngles)
            : base(inputLine, 0)
        {
            readyPlace = new Dictionary<Vector, List<int[]>>();
            lastForExistingSquare = new Dictionary<Directions, int>();
            nextForExistingSquare = new Dictionary<Directions, int>();
            PlaceForSizeForExistingSquare = new Dictionary<Directions, Dictionary<int, Dictionary<int, int>>>();
            foreach (var direct in CubeUtils.Order)
            {
                nextForExistingSquare.Add(direct, 0);
                PlaceForSizeForExistingSquare.Add(direct, new Dictionary<int, Dictionary<int, int>>());
            }
        }

        public void SetDimentions(int numberOfAngles)
        {
            DimensionCoeficient = numberOfAngles/2;
        }
    }

    public class Cube
    {
        protected List<Rotates> RotateMemory;
        public Dictionary<Directions, int> Place;
        protected Dictionary<Directions, Colores> Side2Colores;
        public int NumberOfColoredSides;
        protected Dictionary<Directions, int> Sizes;
        protected int cubeIndex;

        public Dictionary<Directions, Colores> ShowSideColores
        {
            get { return Side2Colores; }
        }

        public Dictionary<Directions, int> ShowSizes
        {
            get { return Sizes; }
        }

        public Vector GetSizeState()
        {
            var sizeVector = new List<int>(3);
            foreach (var direct in CubeUtils.ZeroSides)
            {
                if (true||!Place.ContainsKey(direct))
                {
                    sizeVector.Add(Sizes[direct]);
                }
                else
                {
                    sizeVector.Add(-1);
                }
            }
            return new Vector(sizeVector.ToArray());
        }

        public int ID
        {
            get { return cubeIndex; }
        }

        public bool IsColored(Directions direction)
        {
            return (!Side2Colores[direction].Equals(Colores.Uncolored));
        }

        public Cube(String inputLine, int newIndex)
        {
            cubeIndex = newIndex;

            var input = inputLine.Split(' ');

            Sizes = new Dictionary<Directions, int>();
            var tempSizesCounter = 0;
            foreach (var direct in CubeUtils.Order)
            {
                Sizes.Add(direct, int.Parse(input[tempSizesCounter++/2]));
            }


            RotateMemory = new List<Rotates>();
            Side2Colores = new Dictionary<Directions, Colores>();
            Place = new Dictionary<Directions, int>();

            var inputColors = input[3].ToCharArray();
            var colorsIndex = 0;
            //передняя, задняя, нижняя, верхняя, левая, правая

            foreach (var side in CubeUtils.Order)
            {
                Side2Colores.Add(side, CubeUtils.ColoresTryParse(inputColors[colorsIndex++]));
            }

            NumberOfColoredSides = 6;
            foreach (var sym in inputColors.Where(sym => sym == '.'))
            {
                NumberOfColoredSides--;
            }
        }

        public void Rotate(Rotates rotateTo)
        {
            RotateMemory.Add(rotateTo);

            for (var j = 1; j < 4; j++)
            {
                SwapColores(CubeUtils.RotateRings[rotateTo][0], CubeUtils.RotateRings[rotateTo][j]);
            }
            SwapSides(CubeUtils.RotateRings[rotateTo][0], CubeUtils.RotateRings[rotateTo][1]);
        }

        private void SwapSides(Directions fOne, Directions aOne)
        {
            var temp = Sizes[fOne];
            Sizes[fOne] = Sizes[aOne];
            Sizes[aOne] = temp;
        }

        public String ToStringAAnswerStyle()
        {
            var stateAfterRotate = ForAnwerSideRotator.StateAfterRotates(this.RotateMemory);
            if (!(stateAfterRotate.ContainsKey(Directions.Front) &&
                  stateAfterRotate.ContainsKey(Directions.Bottom) &&
                  Place.ContainsKey(Directions.Front) &&
                  Place.ContainsKey(Directions.Bottom) &&
                  Place.ContainsKey(Directions.Left)))
            {
                                throw new Exception();
                return "Билиберда";
            }
            return String.Format("{0} {1} {2} {3} {4}",
                                 stateAfterRotate[Directions.Front],
                                 stateAfterRotate[Directions.Bottom],
                                 Place[Directions.Front],
                                 Place[Directions.Bottom],
                                 Place[Directions.Left]);
        }

        public String ToStringAsInputStyle()
        {
            return String.Format("({1},{2},{0})  :  {3},{4},{5},{6},{7},{8}", Sizes[Directions.Front],
                                 Sizes[Directions.Bottom], Sizes[Directions.Left],
                                 Side2Colores[Directions.Front],
                                 Side2Colores[Directions.Back],
                                 Side2Colores[Directions.Bottom],
                                 Side2Colores[Directions.Top],
                                 Side2Colores[Directions.Left],
                                 Side2Colores[Directions.Right]
                );
        }

        public bool IsEqualByColores(Directions[] interestingSides, Dictionary<Directions, Colores> originSides)
        {
            return interestingSides.
                Where(side => (!Side2Colores[side].Equals(Colores.Uncolored))).
                All(side => Side2Colores[side].Equals(originSides[side]));
        }

        private bool IsSimilarByColores(Directions[] interestingSide, Dictionary<Directions, Colores> originSides)
        {
            return interestingSide.
                Where(side => !Side2Colores[side].Equals(Colores.Uncolored)).
                Any(side => Side2Colores[side].Equals(originSides[side]));
        }

        public void Rotate2OriginByColor(Dictionary<Directions, Colores> originSides)
        {
            while (!IsEqualByColores(CubeUtils.Order, originSides))
            {
                var rotateTo = Rotates.Forward;

                var counter = 0;

                while (!this.IsSimilarByColores(CubeUtils.Rotates2Ring(rotateTo), originSides) && counter++ < 4)
                {
                    this.Rotate(rotateTo);
                }

                counter = 0;

                rotateTo = Rotates.Side;

                while (!this.IsSimilarByColores(CubeUtils.Rotates2Ring(rotateTo), originSides) && counter++ < 4)
                {
                    this.Rotate(rotateTo);
                }

                counter = 0;
                rotateTo = Rotates.Around;

                while (!this.IsSimilarByColores(CubeUtils.Rotates2Ring(rotateTo), originSides) && counter++ < 4)
                {
                    this.Rotate(rotateTo);
                }
            }
        }

        private void SwapColores(Directions from, Directions to)
        {
            var temp = Side2Colores[from];
            Side2Colores[from] = Side2Colores[to];
            Side2Colores[to] = temp;
        }

    }

    public class Program
    {

        public static Cube[] cubes = new Cube[1101];

        public static int numOfCubes = 0;

        public static PAPACube readerMainCube()
        {

            return new PAPACube(Console.ReadLine(), 0);
        }

        public static List<Cube> ReaderSmallCubes()
        {
            var answer = new List<Cube>();
            numOfCubes = int.Parse(Console.ReadLine());

            for (var i = 1; i <= numOfCubes; i++)
            {
                var cube = new Cube(Console.ReadLine(), i);
                answer.Add(cube);
                cubes[i] = cube;
            }
            return answer;
        }

        private static void Main()
        {
            PAPACube mainCube;
            List<Cube> cubeList;

            mainCube = readerMainCube();
            cubeList = ReaderSmallCubes();

            foreach (var cube in cubeList)
            {
                cube.Rotate2OriginByColor(mainCube.ShowSideColores);
            }

            foreach (var cube in cubeList)
            {
                mainCube.placeMeByColor(cube);
            }

            var angl = CubeUtils.TakeAngles(cubeList);

            angl.ForEach(mainCube.DesignateStartpointsForAngles);


            var arns = CubeUtils.TakeEdges(cubeList);
            var sides = CubeUtils.TakeSideCubes(cubeList);

            mainCube.SetDimentions(angl.Count);

            foreach (Cube cube in arns)
            {
                mainCube.placeMeByOrder(cube);
            }
            if (!angl.TrueForAll(mainCube.TryTakePlace)) throw new Exception();
            if (!arns.TrueForAll(mainCube.TryTakePlace)) throw  new Exception();
            foreach (Cube side in sides)
            {
                if (!mainCube.TryTakePlace(side))
                {
                    for (int i = 0; i < 6; i++)
                    {
                        var direct = CubeUtils.Order[i];
                        if (side.IsColored(direct))
                        {
                            side.Rotate(CubeUtils.Direction2InvariantRotate(direct));
                            break;
                        }
                    }
                    if (!mainCube.TryTakePlace(side))
                    {
                        throw new Exception("bad");
                    }
                }

            }
                        

            for (var i = 1; i <= numOfCubes; i++)
            {
                Console.WriteLine(cubes[i].ToStringAAnswerStyle());
            }

            Console.ReadLine();
        }
    }

}