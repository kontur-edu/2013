using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubik
{
    internal class Part
    {
        // measurements
        public int W { get; set; }
        public int D { get; set; }
        public int H { get; set; }
        // coords
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        // angle - угловой, edge - бортовой, central - центральный на грани, inner - внутренний, невидимый
        public string Type { get; private set; }
        public string Colors { get; set; }
        // обозначение граней, которые должны быть спереди и снизу соответственно при правильной ориентации
        // F-front, B-back, D-down, U-up, L-left, R-right
        public char FrontFace { get; set; }
        public char DownFace { get; set; }
        // отображает текущее состояние граней у детали
        public char[] CurState { get; set; }

        public void SetCoords(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        // Important: "Colors" field is required
        public void DefineType()
        {
            int groups = 0;
            if (Colors[0] != '.' || Colors[1] != '.')
                ++groups;
            if (Colors[2] != '.' || Colors[3] != '.')
                ++groups;
            if (Colors[4] != '.' || Colors[5] != '.')
                ++groups;
            switch (groups)
            {
                case 3:
                    Type = "angle"; break;
                case 2:
                    Type = "edge"; break;
                case 1:
                    Type = "central"; break;
                case 0:
                    Type = "inner"; break;
            }
        }

        public void Orient(string initColors)
        {
            int firstColorPos = 0;
            while (Colors[firstColorPos] == '.')
                firstColorPos++;
            char colorA = Colors[firstColorPos];
            var colorARightPos = initColors.IndexOf(colorA);

            var faces = "FBDULR";
            
            // switch the rotate from RotateY to RotateZ and vice versa
            // 0 - RotateY is used, 1 - RotateZ
            int switchYZ = 0;
    
            if (Type == "angle" || Type == "edge")
            {
                int lastColorPos = Colors.Length - 1; // 5
                while (Colors[lastColorPos] == '.')
                    lastColorPos--;
                char colorB = Colors[lastColorPos];

                var colorBRightPos = initColors.IndexOf(colorB);

                // counter of iterations. if is 4 then RotateY or RotateZ is needed
                int counter = 0;

                // through the all rotations until we find out the correct one. Max 24 rotations are possible
                while (CurState[colorARightPos] != faces[firstColorPos]
                       || CurState[colorBRightPos] != faces[lastColorPos])
                {
                    RotateX();
                    counter++;
                    if (counter == 4)
                    {
                        if (switchYZ == 0)
                            RotateY();
                        else
                            RotateZ();
                        switchYZ ^= 1;
                        counter = 0;
                    }
                }
            }
            if (Type == "central")
            {
                // through the 6 rotations until we find out the correct one. 6 possible colors for every face
                while (CurState[colorARightPos] != faces[firstColorPos])
                {
                    if (switchYZ == 0)
                        RotateY();
                    else
                        RotateZ();
                    switchYZ ^= 1;
                }
            }
            FrontFace = CurState[0];
            DownFace = CurState[2];

            UpdateMeasurements();
        }

        // cyclic shift by 2  and  s1 <-> s2
        // i.e.  0 -> 2 -> 1 -> 3 -> 0
        private void Rotate(ref char s0, ref char s1, ref char s2, ref char s3)
        {
            var tmp = s0;
            s0 = s3;
            s3 = s1;
            s1 = s2;
            s2 = tmp;
        }
        public void RotateX()
        {
            Rotate(ref CurState[2], ref CurState[3], ref CurState[4], ref CurState[5]);
        }
        public void RotateY()
        {
            Rotate(ref CurState[0], ref CurState[1], ref CurState[4], ref CurState[5]);
        }
        public void RotateZ()
        {
            Rotate(ref CurState[0], ref CurState[1], ref CurState[2], ref CurState[3]);
        }

        // change the order of measurements
        public void UpdateMeasurements()
        {
            var faceOrder = new char[3];
            for (int i = 0; i < CurState.Length; i++)
            {
                if (CurState[i] == 'F' || CurState[i] == 'D' || CurState[i] == 'L')
                    faceOrder[i / 2] = CurState[i];
            }

            switch (faceOrder[0])
            {
                case 'F':
                    if (faceOrder[1] != 'D')
                    {
                        var tmp = D;
                        D = H;
                        H = tmp;
                    }
                    break;
                case 'D':
                    if (faceOrder[1] == 'F')
                    {
                        var tmp = W;
                        W = D;
                        D = tmp;
                    }
                    else
                    {
                        var tmp = D;
                        D = H;
                        H = W;
                        W = tmp;
                    }
                    break;
                case 'L':
                    if (faceOrder[1] == 'F')
                    {
                        var tmp = D;
                        D = W;
                        W = H;
                        H = tmp;
                    }
                    else
                    {
                        var tmp = W;
                        W = H;
                        H = tmp;
                    }
                    break;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string[] tokens = Console.ReadLine().Split(' ');
            int w = int.Parse(tokens[0]);
            int d = int.Parse(tokens[1]);
            int h = int.Parse(tokens[2]);
            var initColors = tokens[3];

            // arrays of angle and edge parts
            var angleParts = new int[8];
            int angleLen = 0;
            var edgeParts = new int[1000];
            int edgeLen = 0;

            int xCount = 0;
            int yCount = 0;
            int zCount = 0;

            int n = int.Parse(Console.ReadLine());
            var parts = new List<Part>();
            for (int i = 0; i < n; ++i)
            {
                tokens = Console.ReadLine().Split(' ');

                var part = new Part();
                part.W = int.Parse(tokens[0]);
                part.D = int.Parse(tokens[1]);
                part.H = int.Parse(tokens[2]);
                part.Colors = tokens[3];
                part.DefineType();
                part.CurState = new[] { 'F', 'B', 'D', 'U', 'L', 'R' };
                part.Orient(initColors);
                parts.Add(part);
                if (part.Type == "angle" || part.Type == "edge")
                {
                    if (part.Type == "angle")
                        angleParts[angleLen++] = i;
                    else
                        edgeParts[edgeLen++] = i;

                    UpdateXYZCounts(part.Colors, initColors, ref xCount, ref yCount, ref zCount);
                }
            }

            var cube = new int[xCount, yCount, zCount];
            for (int i = 0; i < xCount; i++)
            {
                for (int j = 0; j < yCount; j++)
                {
                    for (int k = 0; k < zCount; k++)
                    {
                        cube[i, j, k] = -1;
                    }
                }
            }

            for (int i = 0; i < angleLen; i++)
                DefineAngleLocation(parts[angleParts[i]], angleParts[i], cube, initColors);

            for (int i = 0; i < edgeLen; i++)
                DefineEdgeLocation(parts[edgeParts[i]], edgeParts[i], cube, initColors);

            SortEdgeParts(parts, cube);

            var sortedW = new int[Math.Max(xCount-2, 0)];
            for (int i = 0; i < sortedW.Length; i++)
                sortedW[i] = parts[cube[i + 1, 0, 0]].W;

            var sortedD = new int[Math.Max(yCount - 2, 0)];
            for (int j = 0; j < sortedD.Length; j++)
                sortedD[j] = parts[cube[0, j + 1, 0]].D;

            var sortedH = new int[Math.Max(zCount - 2, 0)];
            for (int k = 0; k < sortedH.Length; k++)
                sortedH[k] = parts[cube[0, 0, k + 1]].H;
            
            
            // Finally, central and inner types of part
            for (int i = 0; i < n; i++)
            {
                Part part = parts[i];
                if (part.Type == "angle" || part.Type == "edge")
                    continue;
                if (part.Type == "central")
                    FindPlaceForCentralInCube(part, i, initColors, cube, sortedW, sortedD, sortedH);
            }

            // SetCoords to all of parts in cube
            parts[cube[0, 0, 0]].SetCoords(0, 0, 0);
            for (int i = 1; i < xCount; i++)
            {
                Part prev = parts[cube[i - 1, 0, 0]];
                parts[cube[i, 0, 0]].SetCoords(prev.X + prev.W, prev.Y, prev.Z);
            }
            for (int i = 0; i < xCount; i++)
            {
                for (int j = 1; j < yCount; j++)
                {
                    Part prev = parts[cube[i, j - 1, 0]];
                    parts[cube[i, j, 0]].SetCoords(prev.X, prev.Y + prev.D, prev.Z);
                }
            }
            for (int i = 0; i < xCount; i++)
            {
                for (int j = 0; j < yCount; j++)
                {
                    for (int k = 1; k < zCount; k++)
                    {
                        Part prev = parts[cube[i, j, k - 1]];
                        parts[cube[i, j, k]].SetCoords(prev.X, prev.Y, prev.Z + prev.H);
                    }
                }
            }


            foreach (var part in parts)
                Console.WriteLine(String.Format("{0} {1} {2} {3} {4}", part.FrontFace, part.DownFace, part.X, part.Y, part.Z));
        }


        private static void FindPlaceForCentralInCube(Part part, int i, string initColors, int[, ,] cube,
            int[] sortedW, int[] sortedD, int[] sortedH)
        {
            var xCount = cube.GetLength(0);
            var yCount = cube.GetLength(1);
            var zCount = cube.GetLength(2);

            var colPos = 0;
            while (part.Colors[colPos] == '.')
                colPos++;
            var colorFaceNum = initColors.IndexOf(part.Colors[colPos]);
            // front or back 
            if (colorFaceNum == 0 || colorFaceNum == 1)
            {
                var startCubeX = colorFaceNum == 0 ? 0 : xCount - 1;
                var startCubeY = BinarySearch(part.D, sortedD) + 1;
                var startCubeZ = BinarySearch(part.H, sortedH) + 1;

                if (startCubeY == 0 || startCubeZ == 0
                    || !CanFindPlaceForCentralInCube(cube, startCubeX, startCubeY, startCubeZ, i, part.D, part.H, sortedD, sortedH))
                {
                    part.RotateX();
                    var tmp = part.D;
                    part.D = part.H;
                    part.H = tmp;

                    startCubeY = BinarySearch(part.D, sortedD) + 1;
                    startCubeZ = BinarySearch(part.H, sortedH) + 1;
                    CanFindPlaceForCentralInCube(cube, startCubeX, startCubeY, startCubeZ, i, part.D, part.H, sortedD, sortedH);
                }
            }
            if (colorFaceNum == 2 || colorFaceNum == 3)
            {
                var startCubeX = BinarySearch(part.W, sortedW) + 1;
                var startCubeY = colorFaceNum == 0 ? 0 : yCount - 1;
                var startCubeZ = BinarySearch(part.H, sortedH) + 1;

                if (startCubeX == 0 || startCubeZ == 0
                    || !CanFindPlaceForCentralInCube(cube, startCubeX, startCubeY, startCubeZ, i, part.W, part.H, sortedW, sortedH))
                {
                    part.RotateY();
                    var tmp = part.W;
                    part.W = part.H;
                    part.H = tmp;

                    startCubeX = BinarySearch(part.W, sortedW) + 1;
                    startCubeZ = BinarySearch(part.H, sortedH) + 1;
                    CanFindPlaceForCentralInCube(cube, startCubeX, startCubeY, startCubeZ, i, part.W, part.H, sortedW, sortedH);
                }
            }
            if (colorFaceNum == 4 || colorFaceNum == 5)
            {
                var startCubeX = BinarySearch(part.W, sortedW) + 1;
                var startCubeY = BinarySearch(part.D, sortedD) + 1;
                var startCubeZ = colorFaceNum == 0 ? 0 : zCount - 1;

                if (startCubeX == 0 || startCubeY == 0
                    || !CanFindPlaceForCentralInCube(cube, startCubeX, startCubeY, startCubeZ, i, part.W, part.D, sortedW, sortedD))
                {
                    part.RotateZ();
                    var tmp = part.W;
                    part.W = part.D;
                    part.D = tmp;

                    startCubeX = BinarySearch(part.W, sortedW) + 1;
                    startCubeY = BinarySearch(part.D, sortedD) + 1;
                    CanFindPlaceForCentralInCube(cube, startCubeX, startCubeY, startCubeZ, i, part.W, part.D, sortedW, sortedD);
                }
            }
        }

        private static bool CanFindPlaceForCentralInCube(int[,,] cube, int x, int y, int z, int partNum, int valueA, int valueB, int[] sortedA, int[] sortedB)
        {
            var success = false;
            while (z-1 < sortedB.Length && sortedB[z-1] == valueB && !success)
            {
                while (y - 1 < sortedA.Length && sortedA[y - 1] == valueA && !success)
                {
                    if (cube[x, y, z] == -1)
                    {
                        cube[x, y, z] = partNum;
                        success = true;
                    }
                    y++;
                }
                z++;
            }
            return success;
        }

        // returns the first index or -1 if not found
        private static int BinarySearch(int x, int[] a)
        {
            if (a.Length == 0)
                return -1;
            if (a[0] > x)
                return -1;
            if (a[a.Length - 1] < x)
                return -1;

            int first = 0;
            // the next after last one
            int last = a.Length;
            while (first < last)
            {
                int mid = (first + last) / 2;
                if (x <= a[mid])
                    last = mid;
                else
                    first = mid + 1;
            }

            if (a[last] == x)
                return last;
            return -1;
        }

        private static void SortEdgeParts(List<Part> parts, int[,,] cube)
        {
            var cubeCopy = new int[cube.GetLength(0), cube.GetLength(1), cube.GetLength(2)];
            Array.Copy(cube, cubeCopy, cube.Length);
            SortX(parts, cube, cubeCopy); 
            SortY(parts, cube, cubeCopy); 
            SortZ(parts, cube, cubeCopy); 
        }

        // sort all the edges that are parallel Ox axis
        private static void SortX(List<Part> parts, int[,,] cube, int[,,] cubeCopy)
        {
            if (cube.GetLength(0) <= 2)
                return;
            int len = cube.GetLength(0) - 2; // -2 due to angle parts
            var image = new int[len];
            var imageCopy = new int[len];
            for (int i = 0; i < len; i++)
                imageCopy[i] = i + 1;
            SortingByVariousYZ(parts, cube, cubeCopy, len, image, imageCopy, 0, 0);
            if (cube.GetLength(2) - 1 > 0)
                SortingByVariousYZ(parts, cube, cubeCopy, len, image, imageCopy, 0, cube.GetLength(2) - 1);
            if (cube.GetLength(1) - 1 > 0)
                SortingByVariousYZ(parts, cube, cubeCopy, len, image, imageCopy, cube.GetLength(1) - 1, 0);
            if (cube.GetLength(2) - 1 > 0 && cube.GetLength(1) - 1 > 0)    
                SortingByVariousYZ(parts, cube, cubeCopy, len, image, imageCopy, cube.GetLength(1) - 1, cube.GetLength(2) - 1);
        }
        private static void SortingByVariousYZ(List<Part> parts, int[,,] cube, int[,,] cubeCopy, int len, int[] image, int[] imageCopy, int y, int z)
        {
            Array.Copy(imageCopy, image, len);
            Array.Sort(image, delegate(int a, int b)
            {
                var part1 = parts[cube[a, y, z]];
                var part2 = parts[cube[b, y, z]];
                return part1.W.CompareTo(part2.W);
            });
            for (int i = 0; i < len; i++)
                cube[i + 1, y, z] = cubeCopy[image[i], y, z];
        }


        // sort all the edges that are parallel Oy axis
        private static void SortY(List<Part> parts, int[, ,] cube, int[, ,] cubeCopy)
        {
            if (cube.GetLength(1) <= 2)
                return;
            int len = cube.GetLength(1) - 2; // -2 due to angle parts
            var image = new int[len];
            var imageCopy = new int[len];
            for (int i = 0; i < len; i++)
                imageCopy[i] = i + 1;
            SortingByVariousXZ(parts, cube, cubeCopy, len, image, imageCopy, 0, 0);
            if (cube.GetLength(2) - 1 > 0)
                SortingByVariousXZ(parts, cube, cubeCopy, len, image, imageCopy, 0, cube.GetLength(2) - 1);
            if (cube.GetLength(0) - 1 > 0)
                SortingByVariousXZ(parts, cube, cubeCopy, len, image, imageCopy, cube.GetLength(0) - 1, 0);
            if (cube.GetLength(2) - 1 > 0 && cube.GetLength(0) - 1 > 0)
                SortingByVariousXZ(parts, cube, cubeCopy, len, image, imageCopy, cube.GetLength(0) - 1, cube.GetLength(2) - 1);
        }
        private static void SortingByVariousXZ(List<Part> parts, int[, ,] cube, int[, ,] cubeCopy, int len, int[] image, int[] imageCopy, int x, int z)
        {
            Array.Copy(imageCopy, image, len);
            Array.Sort(image, delegate(int a, int b)
            {
                var part1 = parts[cube[x, a, z]];
                var part2 = parts[cube[x, b, z]];
                return part1.D.CompareTo(part2.D);
            });
            for (int i = 0; i < len; i++)
                cube[x, i + 1, z] = cubeCopy[x, image[i], z];
        }


        // sort all the edges that are parallel Oz axis
        private static void SortZ(List<Part> parts, int[, ,] cube, int[, ,] cubeCopy)
        {
            if (cube.GetLength(2) <= 2)
                return;
            int len = cube.GetLength(2) - 2; // -2 due to angle parts
            var image = new int[len];
            var imageCopy = new int[len];
            for (int i = 0; i < len; i++)
                imageCopy[i] = i + 1;
            SortingByVariousXY(parts, cube, cubeCopy, len, image, imageCopy, 0, 0);
            if (cube.GetLength(1) - 1 > 0)
                SortingByVariousXY(parts, cube, cubeCopy, len, image, imageCopy, 0, cube.GetLength(1) - 1);
            if (cube.GetLength(0) - 1 > 0)
                SortingByVariousXY(parts, cube, cubeCopy, len, image, imageCopy, cube.GetLength(0) - 1, 0);
            if (cube.GetLength(1) - 1 > 0 && cube.GetLength(0) - 1 > 0)
                SortingByVariousXY(parts, cube, cubeCopy, len, image, imageCopy, cube.GetLength(0) - 1, cube.GetLength(1) - 1);
        }
        private static void SortingByVariousXY(List<Part> parts, int[, ,] cube, int[, ,] cubeCopy, int len, int[] image, int[] imageCopy, int x, int y)
        {
            Array.Copy(imageCopy, image, len);
            Array.Sort(image, delegate(int a, int b)
            {
                var part1 = parts[cube[x, y, a]];
                var part2 = parts[cube[x, y, b]];
                return part1.H.CompareTo(part2.H);
            });
            for (int i = 0; i < len; i++)
                cube[x, y, i + 1] = cubeCopy[x, y, image[i]];
        }



        private static void DefineEdgeLocation(Part part, int partNum, int[, ,] cube, string initColors)
        {
            int cubeX = -1, cubeY = -1, cubeZ = -1; // coords in cube
            if (part.Colors.Contains(initColors[5]))
                cubeZ = cube.GetLength(2) - 1;
            if (part.Colors.Contains(initColors[3]))
                cubeY = cube.GetLength(1) - 1;
            if (part.Colors.Contains(initColors[1]))
                cubeX = cube.GetLength(0) - 1;
            if (part.Colors.Contains(initColors[4]))
                cubeZ = 0;
            if (part.Colors.Contains(initColors[2]))
                cubeY = 0;
            if (part.Colors.Contains(initColors[0]))
                cubeX = 0;

            if (cubeX == -1)
            {
                cubeX = 0;
                while (cube[cubeX, cubeY, cubeZ] != -1)
                    cubeX++;
            }
            else
            {
                if (cubeY == -1)
                {
                    cubeY = 0;
                    while (cube[cubeX, cubeY, cubeZ] != -1)
                        cubeY++;
                }
                else
                {
                    cubeZ = 0;
                    while (cube[cubeX, cubeY, cubeZ] != -1)
                        cubeZ++;
                }
            }
            cube[cubeX, cubeY, cubeZ] = partNum;
        }

        private static void DefineAngleLocation(Part part, int partNum, int[,,] cube, string initColors)
        {
            int cubeX, cubeY, cubeZ; // coords in cube
            if (part.Colors.Contains(initColors[0]))
                cubeX = 0;
            else
                cubeX = cube.GetLength(0) - 1;

            if (part.Colors.Contains(initColors[2]))
                cubeY = 0;
            else
                cubeY = cube.GetLength(1) - 1;

            if (part.Colors.Contains(initColors[4]))
                cubeZ = 0;
            else
                cubeZ = cube.GetLength(2) - 1;
            cube[cubeX, cubeY, cubeZ] = partNum;
        }

        private static void UpdateXYZCounts(string colors, string initColors, ref int xCount, ref int yCount, ref int zCount)
        {
            if (colors.Contains(initColors[0]))
            {
                if (colors.Contains(initColors[2]))
                {
                    zCount++;
                    if (colors.Contains(initColors[4]))
                    {
                        xCount++;
                        yCount++;
                    }
                }
                else
                {
                    if (colors.Contains(initColors[4]))
                        yCount++;
                }
            }
            else
            {
                if (colors.Contains(initColors[2]))
                {
                    if (colors.Contains(initColors[4]))
                        xCount++;
                }
            }
        }

    }
}
