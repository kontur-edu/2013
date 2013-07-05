// Реализовал ограничение №1 "чем с большим значением N будет способно справиться ваше решение, тем лучше."
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace _1843
{
    class Program
    {
        public static bool UnLockSearching;
        public static List<List<Piece>> OrderedPices;
        public static List<List<Piece>> OrderedPatterns;

        static void Main(string[] args)
        {
            var thread = new Thread(Start) {Priority = ThreadPriority.Highest};
            thread.Start();
        }
        public static void Start()
        {
            var timer = DateTime.Now;
            
            var input = Console.In.ReadToEnd().Split('\n');            

            const int minPieceCountForMultiThread = 1200;

            var piecesCount = Convert.ToInt32(input[1]);

            var bigPiece = new Piece(input[0]);

            var piecesList = new List<Piece>();
            piecesList.Capacity = piecesCount;
            for (int i = 2; i < 2 + piecesCount; i++)
                piecesList.Add(new Piece(input[i]));

            foreach (var piece in piecesList)
                piece.Transfom(bigPiece);

            var wSections = FindSections(bigPiece, "Down", "Left", "Front", "Back", piecesList, 0);
            var dSections = FindSections(bigPiece, "Front", "Left", "Down", "Up", piecesList, 1);
            var hSections = FindSections(bigPiece, "Front", "Down", "Left", "Right", piecesList, 2);

            DoParallelOut(piecesList);

            var patternList = GetPatternList(bigPiece, wSections, dSections, hSections);

            DoFinding(piecesCount, piecesList, patternList, minPieceCountForMultiThread);
        }

        private static List<Piece> GetPatternList(Piece bigPiece, List<int> wSections, List<int> dSections, List<int> hSections)
        {
            var wSectionCount = wSections.Count;
            var dSectionCount = dSections.Count;
            var hSectionCount = hSections.Count;
            var patternList = new List<Piece>();
            patternList.Capacity = wSectionCount * hSectionCount * dSectionCount;
            int wPosition = 0;
            for (int w = 0; w < wSectionCount; w++)
            {
                int hPosition = 0;
                for (int h = 0; h < hSectionCount; h++)
                {
                    int dPosition = 0;
                    for (int d = 0; d < dSectionCount; d++)
                    {
                        var patternPiece = CreatePatternPiece(bigPiece, wSectionCount, hSectionCount, dSectionCount, wSections[w], dSections[d], hSections[h], w, d, h);
                        patternPiece.wPosition = wPosition;
                        patternPiece.dPosition = dPosition;
                        patternPiece.hPosition = hPosition;
                        patternList.Add(patternPiece);
                        dPosition += dSections[d];
                    }
                    hPosition += hSections[h];
                }
                wPosition += wSections[w];
            }
            return patternList;
        }

        private static void DoFinding(int piecesCount, List<Piece> pieceArray, List<Piece> patternList, int maxPieceCount)
        {
            if (piecesCount > maxPieceCount)
            {
                UnLockSearching = false;
                OrderedPices = new List<List<Piece>>();
                OrderedPatterns = new List<List<Piece>>();
				for(int i = 0; i < 4; i++)
				{
					ParallelFinding(i);
				}                

                var averageSum = 0;
                var averageSumList = pieceArray.Where(x=> x.doubleDotCount == 3 ).Select(x => x.sizeSum).ToList();
				
                if (averageSumList.Count != 0)
                    averageSum = (int) averageSumList.Average();

                OrderedPices.Add(   pieceArray.Where(x => x.doubleDotCount == 3 && x.sizeSum >= averageSum ).ToList());
                OrderedPices.Add(   pieceArray.Where(x => x.doubleDotCount == 3 && x.sizeSum < averageSum  ).ToList());
                OrderedPices.Add(   pieceArray.Where(x => x.doubleDotCount == 2).ToList());
                OrderedPices.Add(   pieceArray.Where(x => x.doubleDotCount < 2).ToList());

                OrderedPatterns.Add(patternList.Where(x => x.doubleDotCount == 3 && x.sizeSum >= averageSum).ToList());
                OrderedPatterns.Add(patternList.Where(x => x.doubleDotCount == 3 && x.sizeSum < averageSum ).ToList());
                OrderedPatterns.Add(patternList.Where(x => x.doubleDotCount == 2).ToList());
                OrderedPatterns.Add(patternList.Where(x => x.doubleDotCount < 2).ToList());

                UnLockSearching = true;
            }
            else
            {
                SequentialFinding(patternList, pieceArray.ToArray());
            }
        }

        private static void SequentialFinding(List<Piece> patternList, Piece[] pieceArray)
        {
            foreach (var pattern in patternList)
            {
                var analog = new Piece();
                var data = pieceArray.Where(x => !x.used && x.sizeSum == pattern.sizeSum && x.doubleDotCount == pattern.doubleDotCount);
                analog = FindAnaloguePiece(data, pattern);
                if (analog != null)
                {
                    analog.used = true;
                    analog.wPosition = pattern.wPosition;
                    analog.dPosition = pattern.dPosition;
                    analog.hPosition = pattern.hPosition;
                }
            }
        }

        private static void ParallelFinding(int i)
        {
            var thrad = new Thread(() =>
            {
                while (!UnLockSearching)
                {
                    Thread.Sleep(4);
                }
                var pieceArray = OrderedPices[i].ToArray();
                var patternList = OrderedPatterns[i];
                
                foreach (var pattern in patternList)
                {
                    var analog = new Piece();
                    var data = pieceArray.Where(x => !x.used && x.sizeSum == pattern.sizeSum && x.doubleDotCount == pattern.doubleDotCount);
                    analog = FindAnaloguePiece(data, pattern);
                    if (analog != null)
                    {
                        analog.used = true;
                        analog.wPosition = pattern.wPosition;
                        analog.dPosition = pattern.dPosition;
                        analog.hPosition = pattern.hPosition;
                    }
                }
            }) { Priority = ThreadPriority.Highest };
            thrad.Start();

        }

        public static void DoParallelOut(List<Piece> piecesList)
        {
            var th = new Thread(() =>
            {
                foreach (var piece in piecesList)
                {
                    while (piece.used != true)
                        Thread.Sleep(4);

                    Console.WriteLine(piece.faceNames[0] + " " + piece.faceNames[2] + " " + piece.wPosition + " " + piece.dPosition + " " + piece.hPosition);
                }
            }) { Priority = ThreadPriority.Highest };
            th.Start();
        }

        public static Piece FindAnaloguePiece(IEnumerable<Piece> pile, Piece patternPiece)
        {
            foreach (var piece in pile)
            {
                if (Equal(piece, patternPiece)) return piece;
            }
            if (patternPiece.doubleDotCount >= 2)
            {
                foreach (var piece in pile)
                {
                    if (DeepEqual(piece, patternPiece)) return piece;
                }
            }
            return null;
        }

        public static bool Equal(Piece piece, Piece patternPiece)
        {
            if (piece.sizes[0] == patternPiece.sizes[0] &&
                piece.sizes[1] == patternPiece.sizes[1] &&
                piece.sizes[2] == patternPiece.sizes[2] &&
                piece.faceColors[0] == patternPiece.faceColors[0] &&
                piece.faceColors[1] == patternPiece.faceColors[1] &&
                piece.faceColors[2] == patternPiece.faceColors[2] &&
                piece.faceColors[3] == patternPiece.faceColors[3] &&
                piece.faceColors[4] == patternPiece.faceColors[4] &&
                piece.faceColors[5] == patternPiece.faceColors[5])
                return true;

            return false;
        }

        public static bool DeepEqual(Piece piece, Piece patternPiece)
        {
            if (piece.faceColors[0] == patternPiece.faceColors[0] &&
                piece.faceColors[1] == patternPiece.faceColors[1] &&
                piece.faceColors[2] == patternPiece.faceColors[2] &&
                piece.faceColors[3] == patternPiece.faceColors[3] &&
                piece.faceColors[4] == patternPiece.faceColors[4] &&
                piece.faceColors[5] == patternPiece.faceColors[5])
            {
                for (int i = 0; i < piece.altSizes.Count; i++)
                {
                    if (piece.altSizes[i][0] == patternPiece.sizes[0] &&
                        piece.altSizes[i][1] == patternPiece.sizes[1] &&
                        piece.altSizes[i][2] == patternPiece.sizes[2])
                    {
                        piece.faceNames = piece.altFaceNames[i];
                        piece.sizes = piece.altSizes[i];
                        return true;
                    }
                }
            }
            return false;
        }

        public static Piece CreatePatternPiece(Piece bigPiece, int wSectionCount, int hSectionCount, int dSectionCount, int wsize, int dsize, int hsize, int w, int d, int h)
        {
            var piece = new Piece();
            piece.sizes[0] = wsize;
            piece.sizes[1] = dsize;
            piece.sizes[2] = hsize;

            if (w == 0)
                piece.faceColors[0] = bigPiece.faceColors[0];
            if (w == wSectionCount - 1)
                piece.faceColors[1] = bigPiece.faceColors[1];
            if (d == 0)
                piece.faceColors[2] = bigPiece.faceColors[2];
            if (d == dSectionCount - 1)
                piece.faceColors[3] = bigPiece.faceColors[3];
            if (h == 0)
                piece.faceColors[4] = bigPiece.faceColors[4];
            if (h == hSectionCount - 1)
                piece.faceColors[5] = bigPiece.faceColors[5];

            piece.doubleDotCount = piece.DoubleDotCount();
            piece.sizeSum = piece.sizes.Sum();

            return piece;
        }

        public static List<int> FindSections(Piece bigPiece, string firstFaceToFinding, string secondFaceToFinding, string firstFaceToOrdering, string secondFaceToOrdering, List<Piece> piecesList, int colorGroupIndex)
        {
            var edge = FindEdgePieces(bigPiece, firstFaceToFinding, secondFaceToFinding, piecesList);
            OrderEdge(bigPiece, firstFaceToOrdering, secondFaceToOrdering, ref edge);
            return edge.Select(x => x.sizes[colorGroupIndex]).ToList();
        }

        public static void OrderEdge(Piece bigPiect, string firstFaceToOrdering, string secondFaceToOrdering, ref List<Piece> piecePile)
        {
            var firstFaceNumber = Piece.GetFaceNumberByName(firstFaceToOrdering);
            var secondFaceNumber = Piece.GetFaceNumberByName(secondFaceToOrdering);
            if (piecePile.Count > 1)
            {
                var startPiece = piecePile.FirstOrDefault(x => x.faceColors.Any(c => bigPiect.faceColors[firstFaceNumber] == c));
                var finishPiece = piecePile.FirstOrDefault(x => x.faceColors.Any(c => bigPiect.faceColors[secondFaceNumber] == c));
                piecePile.Remove(startPiece);
                piecePile.Insert(0, startPiece);
                if (startPiece != finishPiece)
                {
                    piecePile.Remove(finishPiece);
                    piecePile.Add(finishPiece);
                }
            }
        }

        public static List<Piece> FindEdgePieces(Piece bigPiect, string firstFace, string secondFace, List<Piece> piecesList)
        {
            var firstFaceNumber = Piece.GetFaceNumberByName(firstFace);
            var secondFaceNumber = Piece.GetFaceNumberByName(secondFace);
            return piecesList.Where(piece => piece.faceColors.Any(color => bigPiect.faceColors[firstFaceNumber] == color)
                && piece.faceColors.Any(color => bigPiect.faceColors[secondFaceNumber] == color)).ToList();
        }

        public class Piece
        {
            public int wPosition = 0, dPosition = 0, hPosition = 0;
            public int[] sizes = new int[3];
            public int sizeSum;
            public int doubleDotCount;
            public char[] faceColors = new char[6];
            public char[] faceNames = new char[6];
            public List<int[]> altSizes = new List<int[]>();
            public List<char[]> altFaceNames = new List<char[]>();
            public string outStr;
            public bool used = false;

            public Piece()
            {
                FillFaceNames();
                for (int i = 0; i < 6; i++)
                {
                    faceColors[i] = '.';
                }
            }
            public Piece(string pieceStr)
            {
                var str = pieceStr.Trim().Split(' ');
                FillFaceNames();

                for (int i = 0; i < 6; i++)
                {
                    faceColors[i] = str[3].Trim()[i];
                }
                for (int i = 0; i < 3; i++)
                {
                    sizes[i] = Convert.ToInt32(str[i]);
                }
                sizeSum = sizes.Sum();
            }
            public static int GetFaceNumberByName(string faceName)
            {
                if (faceName == "Front") return 0;
                if (faceName == "Back") return 1;
                if (faceName == "Down") return 2;
                if (faceName == "Up") return 3;
                if (faceName == "Left") return 4;
                if (faceName == "Right") return 5;
                return 0;
            }
            public void FillFaceNames()
            {
                faceNames[0] = 'F';
                faceNames[1] = 'B';
                faceNames[2] = 'D';
                faceNames[3] = 'U';
                faceNames[4] = 'L';
                faceNames[5] = 'R';
            }
            public static void Swap(char[] array, int index)
            {
                char temp = array[index + 1];
                array[index + 1] = array[index];
                array[index] = temp;
            }
            public static void Swap(char[] array, int oldGroupIndex, int newGroupIndex)
            {
                char replacingFaceFirst = array[newGroupIndex];
                char replacingFaceSecond = array[newGroupIndex + 1];

                array[newGroupIndex] = array[oldGroupIndex];
                array[newGroupIndex + 1] = array[oldGroupIndex + 1];

                array[oldGroupIndex] = replacingFaceSecond;
                array[oldGroupIndex + 1] = replacingFaceFirst;
            }
            public void Turn180(int groupIndex)
            {
                var doubleDotIndexes = DoubleDotIndexes();
                if (doubleDotIndexes.Count != 0)
                {
                    Swap(faceColors, groupIndex);
                    Swap(faceColors, doubleDotIndexes[0] * 2);

                    Swap(faceNames, groupIndex);
                    Swap(faceNames, doubleDotIndexes[0] * 2);
                }
                else
                {
                    Swap(faceColors, groupIndex);
                    Swap(faceColors, 4);

                    Swap(faceNames, groupIndex);
                    Swap(faceNames, 4);
                }
            }
            public void Turn90(int oldGroupIndex, int newGroupIndex)
            {
                Swap(faceColors, oldGroupIndex, newGroupIndex);
                Swap(faceNames, oldGroupIndex, newGroupIndex);

                var oldSizeGroupIndex = oldGroupIndex / 2;
                var newSizeGroupIndex = newGroupIndex / 2;
                SwapSizes(sizes, oldSizeGroupIndex, newSizeGroupIndex);
            }
            public void SwapSizes(int[] sizes, int oldGroupIndex, int newGroupIndex)
            {
                var sizetmp = sizes[oldGroupIndex];
                sizes[oldGroupIndex] = sizes[newGroupIndex];
                sizes[newGroupIndex] = sizetmp;
            }
            public int DoubleDotCount()
            {
                int doubleDotCount = 0;

                if (faceColors[0] == '.' && faceColors[1] == '.') doubleDotCount++;

                if (faceColors[2] == '.' && faceColors[3] == '.') doubleDotCount++;

                if (faceColors[4] == '.' && faceColors[5] == '.') doubleDotCount++;

                return doubleDotCount;
            }
            public static bool FaceColorsEqual(char[] faceColors, char[] patternFaceColors)
            {
                if (faceColors[0] == patternFaceColors[0] &&
                    faceColors[1] == patternFaceColors[1] &&
                    faceColors[2] == patternFaceColors[2] &&
                    faceColors[3] == patternFaceColors[3] &&
                    faceColors[4] == patternFaceColors[4] &&
                    faceColors[5] == patternFaceColors[5])
                    return true;

                return false;
            }
            public void Transfom(Piece basisPiece)
            {
                for (int basicColorIndex = 0; basicColorIndex < 6; basicColorIndex++)
                {
                    var colorExist = faceColors.FirstOrDefault(x => x == basisPiece.faceColors[basicColorIndex]);
                    if (colorExist != 0)
                    {
                        var colorIndex = Array.IndexOf(faceColors, colorExist);
                        if (colorIndex != basicColorIndex)
                        {
                            var colorGroupIndex = colorIndex - colorIndex % 2;
                            var basicColorGroupIndex = basicColorIndex - basicColorIndex % 2;
                            if (colorGroupIndex == basicColorGroupIndex)
                            {
                                Turn180(basicColorGroupIndex);
                            }
                            else
                            {
                                Turn90(colorGroupIndex, basicColorGroupIndex);
                                colorIndex = Array.IndexOf(faceColors, colorExist);
                                if (colorIndex != basicColorIndex)
                                {
                                    Turn180(basicColorGroupIndex);
                                }
                            }
                        }
                    }
                }
                var doubleDotIndexes = DoubleDotIndexes();
                doubleDotCount = doubleDotIndexes.Count;
                AddAlternativePositions(doubleDotIndexes);
            }
            private void AddAlternativePositions(List<int> doubleDotIndexes)
            {
                if (doubleDotCount > 1)
                {
                    var altsize = new int[3];
                    var alternativeFaceNames = new char[6];

                    if (doubleDotCount == 3)
                    {
                        altSizes.Capacity = 5;
                        altFaceNames.Capacity = 5;

                        AddAltSizes(0, 2, 1);
                        alternativeFaceNames = faceNames.ToArray();
                        Swap(alternativeFaceNames, 2, 4);
                        AddAltFaceNames(alternativeFaceNames);

                        AddAltSizes(1, 0, 2);
                        alternativeFaceNames = faceNames.ToArray();
                        Swap(alternativeFaceNames, 0, 2);
                        AddAltFaceNames(alternativeFaceNames);

                        AddAltSizes(1, 2, 0);
                        alternativeFaceNames = faceNames.ToArray();
                        Swap(alternativeFaceNames, 4, 0);
                        Swap(alternativeFaceNames, 0, 2);
                        AddAltFaceNames(alternativeFaceNames);

                        AddAltSizes(2, 1, 0);
                        alternativeFaceNames = faceNames.ToArray();
                        Swap(alternativeFaceNames, 0, 4);
                        AddAltFaceNames(alternativeFaceNames);

                        AddAltSizes(2, 0, 1);
                        alternativeFaceNames = faceNames.ToArray();
                        Swap(alternativeFaceNames, 2, 0);
                        Swap(alternativeFaceNames, 0, 4);
                        AddAltFaceNames(alternativeFaceNames);
                    }
                    else
                    {
                        altsize = sizes.ToArray();
                        SwapSizes(altsize, doubleDotIndexes[0], doubleDotIndexes[1]);
                        altSizes.Add(altsize);

                        alternativeFaceNames = faceNames.ToArray();
                        Swap(alternativeFaceNames, doubleDotIndexes[0] * 2, doubleDotIndexes[1] * 2);
                        altFaceNames.Add(alternativeFaceNames);
                    }
                }
            }
            public List<int> DoubleDotIndexes()
            {
                var doubleDotIndexes = new List<int>();

                if (faceColors[0] == '.' && faceColors[1] == '.') doubleDotIndexes.Add(0);
                if (faceColors[2] == '.' && faceColors[3] == '.') doubleDotIndexes.Add(1);
                if (faceColors[4] == '.' && faceColors[5] == '.') doubleDotIndexes.Add(2);

                return doubleDotIndexes;
            }
            private void AddAltFaceNames(char[] alternativeFaceNames)
            {
                var res = new char[6];
                res = alternativeFaceNames.ToArray();
                altFaceNames.Add(res);
            }
            public void AddAltSizes(int w, int d, int h)
            {
                int[] altsize = new int[3];
                altsize[0] = sizes[w];
                altsize[1] = sizes[d];
                altsize[2] = sizes[h];
                altSizes.Add(altsize);
            }
        }
    }
}
