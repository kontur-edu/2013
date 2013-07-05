// Problem 1843, ID 4899208, WA 19

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace test_task
{
    class Program
    {
        static void Main(string[] args)
        {
            Task task = new Task();
            task.init();
            task.solve();
        }
    }

    class Task
    {
        static int[][] rot = new int[24][]; 
        static int[][] dimRot = new int[24][];
        static string side = "FBDULR";
        
        struct piece
        {
            public int[] dim; // dim[] = {w, d, h}
            public string col;
            public int rotNumb;
            public int[] res; // res[] = {x, y, z}
            public piece(int w, int d, int h, string col) 
            {
                dim = new int[3] { w, d, h };
                res = new int[3];
                this.col = col;
                rotNumb = 0;
            }
        };

        piece ppd; // main parallelepiped
        piece[] p;
        int n;

        string[] readString()
        {
            return Console.ReadLine().Split(new char[] { ' ', '\t', '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);
        }

        public void init()
        {
            // init rotations
            rot[0] = new int[6] { 0, 1, 2, 3, 4, 5 }; dimRot[0] = new int[3] { 0, 1, 2 };
            rot[1] = new int[6] { 0, 1, 4, 5, 3, 2 }; dimRot[1] = new int[3] { 0, 2, 1 };
            rot[2] = new int[6] { 0, 1, 3, 2, 5, 4 }; dimRot[2] = new int[3] { 0, 1, 2 };
            rot[3] = new int[6] { 0, 1, 5, 4, 2, 3 }; dimRot[3] = new int[3] { 0, 2, 1 };

            rot[4] = new int[6] { 3, 2, 0, 1, 4, 5 }; dimRot[4] = new int[3] { 1, 0, 2 };
            rot[5] = new int[6] { 3, 2, 4, 5, 1, 0 }; dimRot[5] = new int[3] { 1, 2, 0 };
            rot[6] = new int[6] { 3, 2, 1, 0, 5, 4 }; dimRot[6] = new int[3] { 1, 0, 2 };
            rot[7] = new int[6] { 3, 2, 5, 4, 0, 1 }; dimRot[7] = new int[3] { 1, 2, 0 };

            rot[8] = new int[6] { 1, 0, 3, 2, 4, 5 }; dimRot[8] = new int[3] { 0, 1, 2 };
            rot[9] = new int[6] { 1, 0, 4, 5, 2, 3 }; dimRot[9] = new int[3] { 0, 2, 1 };
            rot[10] = new int[6] { 1, 0, 2, 3, 5, 4 }; dimRot[10] = new int[3] { 0, 1, 2 };
            rot[11] = new int[6] { 1, 0, 5, 4, 3, 2 }; dimRot[11] = new int[3] { 0, 2, 1 };

            rot[12] = new int[6] { 2, 3, 1, 0, 4, 5 }; dimRot[12] = new int[3] { 1, 0, 2 };
            rot[13] = new int[6] { 2, 3, 4, 5, 0, 1 }; dimRot[13] = new int[3] { 1, 2, 0 };
            rot[14] = new int[6] { 2, 3, 0, 1, 5, 4 }; dimRot[14] = new int[3] { 1, 0, 2 };
            rot[15] = new int[6] { 2, 3, 5, 4, 1, 0 }; dimRot[15] = new int[3] { 1, 2, 0 };

            rot[16] = new int[6] { 4, 5, 2, 3, 1, 0 }; dimRot[16] = new int[3] { 2, 1, 0 };
            rot[17] = new int[6] { 4, 5, 1, 0, 3, 2 }; dimRot[17] = new int[3] { 2, 0, 1 };
            rot[18] = new int[6] { 4, 5, 3, 2, 0, 1 }; dimRot[18] = new int[3] { 2, 1, 0 };
            rot[19] = new int[6] { 4, 5, 0, 1, 2, 3 }; dimRot[19] = new int[3] { 2, 0, 1 };

            rot[20] = new int[6] { 5, 4, 0, 1, 3, 2 }; dimRot[20] = new int[3] { 2, 0, 1 };
            rot[21] = new int[6] { 5, 4, 3, 2, 1, 0 }; dimRot[21] = new int[3] { 2, 1, 0 };
            rot[22] = new int[6] { 5, 4, 1, 0, 2, 3 }; dimRot[22] = new int[3] { 2, 0, 1 };
            rot[23] = new int[6] { 5, 4, 2, 3, 0, 1 }; dimRot[23] = new int[3] { 2, 1, 0 };

            // init vars
            string[] tok = readString();
            ppd = new piece(int.Parse(tok[0]), int.Parse(tok[1]), int.Parse(tok[2]), tok[3]);
            tok = readString();
            n = int.Parse(tok[0]);
            p = new piece[n];
            for (int i = 0; i < n; ++i)
            {
                tok = readString();
                p[i] = new piece(int.Parse(tok[0]), int.Parse(tok[1]), int.Parse(tok[2]), tok[3]);
            }
        }

        string rotateCol(string s, int rotNumb) // rotate color 
        {
            string res = "";
            for (int i = 0; i < 6; ++i)
                res += s[rot[rotNumb][i]];
            return res;
        }
        piece rotatePiece(piece t, int rotNumb) 
        {
            piece res = new piece(t.dim[dimRot[rotNumb][0]],
                    t.dim[dimRot[rotNumb][1]], 
                    t.dim[dimRot[rotNumb][2]],
                    rotateCol(t.col, rotNumb));
            res.rotNumb = rotNumb;
            return res;
        }
        
        void proc(int cmd, int col)
        {
            bool[] was = new bool[n];
            int[] cur = new int[3];
            for (int i = 0; i < n; ++i)
            {
                if (p[i].col[col] != '.')
                {
                    was[i] = true;
                    p[i].res = new int[3] { 0, 0, 0 };
                    cur[cmd] = p[i].dim[cmd];
                    break;
                }
            }
            int last = -1;
            for (int i = 0; i < n; ++i)
            {
                if (was[i]) continue;
                if (p[i].col[col + 1] != '.')
                {
                    last = i;
                    continue;
                }
                was[i] = true;
                Array.Copy(cur, p[i].res, 3);
                cur[cmd] += p[i].dim[cmd];
            }
            if (last != -1)
                p[last].res = cur;
        }

        public void solve()
        {
            // rotate all pieces into the right position
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < 24; ++j)
                {
                    piece t = rotatePiece(p[i], j);
                    bool ok = true;
                    for (int k = 0; k < 6; ++k)
                    {
                        if (t.col[k] != ppd.col[k] && 
                            t.col[k] != '.')
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (ok)
                    {
                        p[i] = t;
                        break;
                    }
                }
            }

            // direction of layers
            if (p[0].dim[0] == ppd.dim[0] &&
                p[0].dim[2] == ppd.dim[2])
                proc(1, 2);
            else if (p[0].dim[0] == ppd.dim[0] &&
                p[0].dim[1] == ppd.dim[1])
                proc(2, 4);
            else proc(0, 0);

            for (int i = 0; i < n; ++i)
            {
                int indFront = rot[p[i].rotNumb][0];
                int indDown = rot[p[i].rotNumb][2];
                Console.Write("{0} {1} {2} {3} {4}\n", side[indFront],
                    side[indDown], p[i].res[0], p[i].res[1], p[i].res[2]);
            }
        }
    }
}
