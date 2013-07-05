using System;
using System.Collections;
using System.Collections.Generic;

namespace ns {
    enum Faces { F, B, D, U, L, R };
    public class Asd {
        static void addToListDict<K, V>(IDictionary<K, LinkedList<V>> dict, K key, V value) {
            if (!dict.ContainsKey(key))
                dict[key] = new LinkedList<V>();
            dict[key].AddLast(value);
        }

        public static void Main(string[] args) {

            Part general = new Part();
            general.Scan(-1);
            int n = Int32.Parse(Console.ReadLine());
            Part[] parts = new Part[n];
            for (int i = 0; i < n; i++) {
                parts[i] = new Part();
                parts[i].Scan(i);
                parts[i].Normalize();
            }
            SortedDictionary<ColorWithTriplet, LinkedList<int>> externalDict
                = new SortedDictionary<ColorWithTriplet, LinkedList<int>>();
            SortedDictionary<Triplet, LinkedList<int>> internalDict
                = new SortedDictionary<Triplet, LinkedList<int>>();
            SortedDictionary<char[], LinkedList<int>> ledgeDict
                = new SortedDictionary<char[], LinkedList<int>>(new ColorCmp());
            Part[, ,] angles = new Part[2, 2, 2];
            for (int i = 0; i < n; i++) {
                if (parts[i].IsAngle()) {
                    bool angleSetted = false;
                    for (int a = 0; a < 2 && !angleSetted; a++)
                        for (int b = 0; b < 2 && !angleSetted; b++)
                            for (int c = 0; c < 2 && !angleSetted; c++) {
                                Part curRotateToAngle = parts[i].GetRotateToAngleAndCheck(a, b, c, general);
                                if (curRotateToAngle != null) {
                                    parts[i] = curRotateToAngle;
                                    angles[a, b, c] = curRotateToAngle;
                                    angleSetted = true;
                                }
                            }
                } else if (parts[i].IsInternal())
                    addToListDict<Triplet, int>(internalDict, parts[i].ToTriplet(), i);
                else if (parts[i].IsLedge()) {
                    addToListDict<char[], int>(ledgeDict, (char[])parts[i].colors.Clone(), i);
                    addToListDict<ColorWithTriplet, int>(externalDict, parts[i].ToColorWithTriplet(), i);
                } else // parts[i] on face
                    addToListDict<ColorWithTriplet, int>(externalDict, parts[i].ToColorWithTriplet(), i);
            }
            char[] colorLedgeX = angles[0, 0, 0].GetNormalizedColorWithoutFace(Faces.F);
            int x = (ledgeDict.ContainsKey((char[])colorLedgeX.Clone()) ? ledgeDict[(char[])colorLedgeX.Clone()].Count : 0)
                + 1 + Convert.ToInt32(angles[1, 0, 0] != null);
            char[] colorLedgeY = angles[0, 0, 0].GetNormalizedColorWithoutFace(Faces.L);
            int y = (ledgeDict.ContainsKey((char[])colorLedgeY.Clone()) ? ledgeDict[(char[])colorLedgeY.Clone()].Count : 0)
                + 1 + Convert.ToInt32(angles[0, 1, 0] != null);
            char[] colorLedgeZ = angles[0, 0, 0].GetNormalizedColorWithoutFace(Faces.D);
            int z = (ledgeDict.ContainsKey((char[])colorLedgeZ.Clone()) ? ledgeDict[(char[])colorLedgeZ.Clone()].Count : 0)
                + 1 + Convert.ToInt32(angles[0, 0, 1] != null);
            int[, ,] cube = new int[x, y, z];
            for (int xpos = 0; xpos < x; xpos++)
                for (int ypos = 0; ypos < y; ypos++)
                    for (int zpos = 0; zpos < z; zpos++)
                        cube[xpos, ypos, zpos] = -1;
            // setting angles
            for (int a = 0; a < 2; a++)
                for (int b = 0; b < 2; b++)
                    for (int c = 0; c < 2; c++)
                        if (angles[a, b, c] != null)
                            cube[(x - 1) * a, (y - 1) * b, (z - 1) * c] = angles[a, b, c].inputPosition;
            Faces[] ledgeFaces = { Faces.F, Faces.L, Faces.D };
            // setting ledges adjacent to [0, 0, 0] angle
            for (int i = 0; i < 3; i++) {
                char[] normColor = angles[0, 0, 0].GetNormalizedColorWithoutFace(ledgeFaces[i]);
                char[] color = angles[0, 0, 0].GetColorWithoutFace(ledgeFaces[i]);
                if (!ledgeDict.ContainsKey(normColor))
                    continue;
                LinkedList<int> curLst = ledgeDict[normColor];
                int pos = 1;
                foreach (int it in curLst) { // [pos, 0, 0] for x, [0, pos, 0] for y, [0, 0, pos] for z
                    cube[pos * Convert.ToInt32(i == 0),
                         pos * Convert.ToInt32(i == 1),
                         pos * Convert.ToInt32(i == 2)] = it;
                    parts[it].RotateToColorAndDim(color, 6, -1, -1, -1);
                    pos++;
                }
            }
            // setting other ledges (not adjacent to [0, 0, 0] angle)
            for (int a = 0; a < 2; a++) {
                for (int b = 0; b < 2; b++) {
                    for (int c = 0; c < 2; c++) {
                        if ((a == 0 && b == 0 && c == 0) || (angles[a, b, c] == null))
                            continue;
                        int[] select = { a, b, c };
                        int[,] selectDim = {
                            { -1, angles[a, b, c].d, angles[a, b, c].h },
                            { angles[a, b, c].w, angles[a, b, c].d, -1 },
                            { angles[a, b, c].w, -1, angles[a, b, c].h }
                        };
                        int[] selectLastPos = { x - 2, y - 2, z - 2 };
                        for (int selectIdx = 0; selectIdx < 3; selectIdx++) { // select ways where a==0 or b==0 or c==0
                            if (select[selectIdx] != 0)
                                continue;
                            char[] color = angles[a, b, c].GetColorWithoutFace(ledgeFaces[selectIdx]);
                            int w = selectDim[selectIdx, 0];
                            int d = selectDim[selectIdx, 1];
                            int h = selectDim[selectIdx, 2];
                            for (int pos = 1; pos <= selectLastPos[selectIdx]; pos++) {
                                int wtmp = w, dtmp = d, htmp = h;
                                Part curSymmetryPart = parts[cube[ // [pos, 0, 0] for a==0, [0, pos, 0] for b==0, [0, 0, pos] for c==0
                                    pos * Convert.ToInt32(selectIdx == 0),
                                    pos * Convert.ToInt32(selectIdx == 1),
                                    pos * Convert.ToInt32(selectIdx == 2)]];
                                if (wtmp == -1) // get unknown dimension
                                    wtmp = curSymmetryPart.w;
                                if (dtmp == -1)
                                    dtmp = curSymmetryPart.d;
                                if (htmp == -1)
                                    htmp = curSymmetryPart.h;
                                Part tmpPart = new Part(color, wtmp, dtmp, htmp, -1);
                                tmpPart.Normalize();
                                LinkedList<int> curLst = externalDict[tmpPart.ToColorWithTriplet()];
                                int idx = curLst.Last.Value;
                                if (selectIdx == 0)
                                    cube[pos, b * (y - 1), c * (z - 1)] = idx;
                                else if (selectIdx == 1)
                                    cube[a * (x - 1), pos, c * (z - 1)] = idx;
                                else
                                    cube[a * (x - 1), b * (y - 1), pos] = idx;
                                curLst.RemoveLast();
                                parts[idx].RotateToColorAndDim(color, 6, -1, -1, -1);
                            }
                        }
                    }
                }
            }
            // Face setting
            for (int xpos = 0; xpos < x; xpos++) {
                for (int ypos = 0; ypos < y; ypos++) {
                    for (int zpos = 0; zpos < z; zpos++) {
                        int cntFaceCoord = Convert.ToInt32(xpos == 0 || xpos == x - 1)
                            + Convert.ToInt32(ypos == 0 || ypos == y - 1)
                            + Convert.ToInt32(zpos == 0 || zpos == z - 1);
                        if (cube[xpos, ypos, zpos] != -1 || cntFaceCoord != 1) // is unsetted face
                            continue;
                        int w = -1, d = -1, h = -1;
                        char[] candidateColor;

                        int[,] direction = { { 0, 1, 1 }, { 1, 0, 1 }, { 1, 1, 0 } };
                        if (zpos == 0 || zpos == z - 1) {
                            candidateColor = parts[cube[0, ypos, zpos]].GetColorWithoutFace(Faces.F);
                            d = parts[cube[0, ypos, zpos]].d;
                            h = parts[cube[0, ypos, zpos]].h;
                            w = parts[cube[xpos, 0, zpos]].w;
                        } else {
                            candidateColor = parts[cube[xpos, ypos, 0]].GetColorWithoutFace(Faces.D);
                            w = parts[cube[xpos, ypos, 0]].w;
                            h = parts[cube[xpos, ypos, 0]].h;
                            if (cube[xpos, 0, zpos] != -1)
                                d = parts[cube[xpos, 0, zpos]].d;
                            else
                                d = parts[cube[0, ypos, zpos]].d;
                        }
                        Part tmp = new Part(candidateColor, w, d, h, -1);
                        tmp.Normalize();
                        LinkedList<int> curPartIdxLst = externalDict[tmp.ToColorWithTriplet()];
                        int curPartIdx = curPartIdxLst.Last.Value;
                        curPartIdxLst.RemoveLast();
                        parts[curPartIdx].RotateToColorAndDim(candidateColor, 6, w, d, h);
                        cube[xpos, ypos, zpos] = curPartIdx;
                    }
                }
            }
            // Internal setting
            for (int xpos = 1; xpos < x - 1; xpos++) {
                for (int ypos = 1; ypos < y - 1; ypos++) {
                    for (int zpos = 1; zpos < z - 1; zpos++) {
                        char[] color = ("..." + "...").ToCharArray();
                        int w = parts[cube[xpos, ypos, 0]].w;
                        int h = parts[cube[xpos, ypos, 0]].h;
                        int d = parts[cube[0, ypos, zpos]].d;
                        Part tmpPart = new Part(color, w, d, h, -1);
                        tmpPart.Normalize();
                        LinkedList<int> curPartIdxLst = internalDict[tmpPart.ToTriplet()];
                        int curPartIdx = curPartIdxLst.Last.Value;
                        curPartIdxLst.RemoveLast();
                        parts[curPartIdx].RotateToColorAndDim(color, 6, w, d, h);
                        cube[xpos, ypos, zpos] = curPartIdx;
                    }
                }
            }


            int[] xcoord = new int[x], ycoord = new int[y], zcoord = new int[z];
            xcoord[0] = 0;
            for (int xpos = 1; xpos < x; xpos++)
                xcoord[xpos] = xcoord[xpos - 1] + parts[cube[xpos - 1, 0, 0]].w;
            ycoord[0] = 0;
            for (int ypos = 1; ypos < y; ypos++)
                ycoord[ypos] = ycoord[ypos - 1] + parts[cube[0, ypos - 1, 0]].h;
            zcoord[0] = 0;
            for (int zpos = 1; zpos < z; zpos++)
                zcoord[zpos] = zcoord[zpos - 1] + parts[cube[0, 0, zpos - 1]].d;
            // Output
            OutputSymbol[] outputSymbols = new OutputSymbol[n];
            for (int xpos = 0; xpos < x; xpos++)
                for (int ypos = 0; ypos < y; ypos++)
                    for (int zpos = 0; zpos < z; zpos++)
                        outputSymbols[cube[xpos, ypos, zpos]]
                            = new OutputSymbol(parts[cube[xpos, ypos, zpos]], xcoord[xpos], ycoord[ypos], zcoord[zpos]);
            for (int i = 0; i < n; i++)
                Console.WriteLine("{0} {1} {2} {3} {4}", outputSymbols[i].Front, outputSymbols[i].Down,
                    outputSymbols[i].xpos, outputSymbols[i].zpos, outputSymbols[i].ypos);
        }
    }

    struct OutputSymbol {
        public char Front, Down;
        public int xpos, ypos, zpos;
        public OutputSymbol(Part p, int xpos, int ypos, int zpos) {
            Front = p.orient[(int)Faces.F];
            Down = p.orient[(int)Faces.D];
            this.xpos = xpos;
            this.ypos = ypos;
            this.zpos = zpos;
        }
    }

    class Part : ICloneable {
        public int w, d, h;
        public char[] colors, orient;
        public int inputPosition;

        
        public Part() {
            w = d = h = inputPosition = -1;
            orient = colors = null;
        }
 
        public Part(char[] colors, int w, int d, int h, int pos) {
            this.colors = (char[])colors.Clone();
            this.w = w;
            this.d = d;
            this.h = h;
            this.inputPosition = pos;
            orient = "??????".ToCharArray();
        }
 
        // works only on normalized parts!
        public bool IsAngle() {
            return colors[(int)Faces.F] != '.' && colors[(int)Faces.D] != '.'
               && (colors[(int)Faces.L] != '.' || colors[(int)Faces.R] != '.');
        }

        public void Scan(int position) {
            this.inputPosition = position;
            string[] line = Console.ReadLine().Split(new char[1] { ' ' });
            w = Int32.Parse(line[0]);
            d = Int32.Parse(line[1]);
            h = Int32.Parse(line[2]);
            colors = new char[6];
            for (int i = 0; i < 6; i++)
                colors[i] = line[3][i];
            orient = "FBDULR".ToCharArray();
        }

        public void Swap(ref int x, ref int y) {
            int tmp = x;
            x = y;
            y = tmp;
        }

        public char[] GetNormalizedColorWithoutFace(Faces f) {
            Part tmp = (Part)this.Clone();
            tmp.colors[(int)f] = '.';
            tmp.Normalize();
            return tmp.colors;
        }

        public char[] GetColorWithoutFace(Faces f) {
            Part tmp = (Part)this.Clone();
            tmp.colors[(int)f] = '.';
            return tmp.colors;
        }

        public void Normalize() {
            // priority is "YVROGB." (first is better)
            int newFrontPos = 0;
            for (int i = 0; i < 6; i++)
                if (colors[newFrontPos] < colors[i])
                    newFrontPos = i;
            char newFrontSym = colors[newFrontPos];
            if (newFrontPos == 2 || newFrontPos == 3)
                RotateLURD();
            while (colors[(int)Faces.F] != newFrontSym)
                RotateLFRB();
            int newDownPos = 2;
            for (int i = 2; i < 6; i++)
                if (colors[newDownPos] < colors[i])
                    newDownPos = i;
            char newDownSym = colors[newDownPos];
            while (colors[(int)Faces.D] != newDownSym)
                RotateLURD();
            if (colors[(int)Faces.F] == '.') {
                // w <= d <= h
                int[] tmp = { w, d, h };
                Array.Sort<int>(tmp);
                RotateToColorAndDim(colors, 6, tmp[0], tmp[1], tmp[2]);
            } else if (colors[(int)Faces.D] == '.')
                // d <= h
                if (d > h)
                    RotateLURD();
        }

        // Rotate ->Left->Up->Right->Down->
        public void RotateLURD() {
            char[][] names = { colors, orient };
            for (int i = 0; i < 2; i++) {
                char tmpL = names[i][(int)Faces.L];
                names[i][(int)Faces.L] = names[i][(int)Faces.D];
                names[i][(int)Faces.D] = names[i][(int)Faces.R];
                names[i][(int)Faces.R] = names[i][(int)Faces.U];
                names[i][(int)Faces.U] = tmpL;
            }
            Swap(ref d, ref h);
        }

        // Rotate ->Left->Front->Right->Back->
        public void RotateLFRB() {
            char[][] names = { colors, orient };
            for (int i = 0; i < 2; i++) {
                char tmpL = names[i][(int)Faces.L];
                names[i][(int)Faces.L] = names[i][(int)Faces.B];
                names[i][(int)Faces.B] = names[i][(int)Faces.R];
                names[i][(int)Faces.R] = names[i][(int)Faces.F];
                names[i][(int)Faces.F] = tmpL;
            }
            Swap(ref w, ref h);
        }

        // a, b, c from { 0, 1 }
        // get new part from this, rotated to angle on position [a, b, c], if rotate is possible
        // null, if rotate impossible
        public Part GetRotateToAngleAndCheck(int a, int b, int c, Part general) {
            char[] generalAngleColors = (char[])general.colors.Clone();
            int[] aSave = { 0, 1 };
            int[] bSave = { 4, 5 };
            int[] cSave = { 2, 3 };
            generalAngleColors[aSave[a ^ 1]] = '?';
            generalAngleColors[bSave[b ^ 1]] = '?';
            generalAngleColors[cSave[c ^ 1]] = '?';
            Part res = (Part)this.Clone();
            if (res.RotateToColorAndDim(generalAngleColors, 3, -1, -1, -1))
                return res;
            return null;
        }

        // rotates to color and all dimensions
        // if new dimension is -1 then this dimension it is not checked
        // cntEqColors is count of colors that must be equals
        // newcolor is new color of cube
        public bool RotateToColorAndDim(char[] newcolor, int cntEqColors, int neww, int newd, int newh) {
            for (int i = 0; i < 6; i++) { // rotate horizontal
                for (int j = 0; j < 4; j++) { // rotate LURD and back to begin stage
                    RotateLURD();
                    int eqColors = 0;
                    for (int k = 0; k < 6; k++)
                        if (newcolor[k] == colors[k])
                            eqColors++;
                    if (eqColors == cntEqColors && (w == neww || neww == -1)
                        && (d == newd || newd == -1) && (h == newh || newh == -1))
                        return true;
                }
                RotateLFRB();
                if (i == 3) {
                    RotateLURD();
                    RotateLFRB();
                } else if (i == 4)
                    RotateLFRB();
            }
            return false;
        }

        // works only on normalized parts!
        public bool IsInternal() {
            return colors[(int)Faces.F] == '.';
        }

        // works only on normalized parts!
        public bool IsLedge() {
            return !IsAngle() && colors[(int)Faces.F] != '.' && colors[(int)Faces.D] != '.';
        }

        public Triplet ToTriplet() {
            return new Triplet(w, d, h);
        }

        public ColorWithTriplet ToColorWithTriplet() {
            return new ColorWithTriplet(colors, ToTriplet());
        }

        public object Clone() {
            Part res = new Part();
            res.colors = (char[])colors.Clone();
            res.orient = (char[])orient.Clone();
            res.w = w;
            res.d = d;
            res.h = h;
            res.inputPosition = inputPosition;
            return res;
        }
    }

    class ColorCmp : IComparer<char[]> {
        public int Compare(char[] a, char[] b) {
            for (int i = 0; i < 6; i++) {
                if (a[i] < b[i])
                    return -1;
                else if (a[i] > b[i])
                    return 1;
            }
            return 0;
        }
    }

    class Triplet : IComparable<Triplet>, ICloneable {
        public int x, y, z;

        public Triplet(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public int CompareTo(Triplet b) {
            if (x == b.x && y == b.y && z == b.z)
                return 0;
            if (x < b.x || (x == b.x && (y < b.y || (y == b.y && z < b.z))))
                return -1;
            return 1;
        }

        public object Clone() {
            return new Triplet(x, y, z);
        }
    }

    class ColorWithTriplet : IComparable<ColorWithTriplet> {
        public char[] color;
        public Triplet p;
        static ColorCmp colorCmp = new ColorCmp();

        public int CompareTo(ColorWithTriplet b) {
            int cmpColors = colorCmp.Compare(color, b.color);
            int cmpPairs = p.CompareTo(b.p);
            if (cmpColors == 0 && cmpPairs == 0)
                return 0;
            if (cmpColors < 0 || (cmpColors == 0 && cmpPairs < 0))
                return -1;
            return 1;
        }

        public ColorWithTriplet(char[] color, Triplet p) {
            this.color = (char[])color.Clone();
            this.p = (Triplet)p.Clone();
        }
    }
}