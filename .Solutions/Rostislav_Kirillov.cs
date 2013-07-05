using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
 Пример
* исх данные
    2 2 2 ROYGBV
    2
    1 2 2 R.YGBV
    2 2 1 YGVBO.

* результат
    F D 0 0 0
    R F 1 0 0
 * 
 * x = R-F
 * y = L-R
 * z = D-T
 * 
 * */
namespace KonturImpossibru{

    public enum color {   
                    Red =       'R',
                    Orange =    'O',
                    Yellow =    'Y',
                    Green =     'G',
                    Blue =      'B',
                    Violet =    'V',
                    Grey =      '.',
                    Any =       '*',
                };
    public enum side {    Front = 'F',
                    Back =  'B',
                    Down =  'D',
                    Up =   'U',
                    Left =  'L',
                    Right=  'R',
                };
    public enum axis
    {
        X = 'X',
        Y = 'Y',
        Z = 'Z',
    };
    public class Program
    {

        private static int count = 0;
        private static Solver solver;
        public static void Main(string[] args)
        {
            Piece first = new Piece(getNextPiece(), 0);
            var tmp = getNextPiece();
            int.TryParse(tmp[0], out count); //read 'n'
            solver = new Solver(first, count);
            for (int i = 0; i < count; i++)
            {
                solver.Add(new Piece(getNextPiece(), i));
            }
            //do useful stuff
            solver.run();
            //write down answer
            foreach (var p in solver.List)
            {
                Console.WriteLine(p.ToAns());
            }

        }
        private static String[] getNextPiece() {
            return Console.ReadLine().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }
        public static void Swap<T>(ref T a, ref T b) {
            T tmp;
            tmp = a;
            a = b;
            b = tmp;
        }
    }
    public class Solver
    {
        private bool splitX = false;
        private bool splitY = false;
        private bool splitZ = false;
        private Piece cube; //'parallelepiped' is so looong
        private MagicStructure magic;
        private int startPiece;
        public Solver(Piece p, int i)
        {
            this.cube = p;
            this.magic = new MagicStructure(i);
        }
        public void Add(Piece p)
        {
            this.magic.Add(p);
        }
        public void run() //work here
        {
            //1D solution only :(
            Piece first;
            Edges mask = cube.getAngle(0); //left down front
            Point3D maskSize = new Point3D(0, 0, 0);
            this.startPiece = this.magic.getByMask(mask, maskSize); //set as used
            List<int> edges = new List<int>();
            edges.Add(this.startPiece);
            first = this.magic.getPiece(this.startPiece);
            maskSize = new Point3D(first.Size);
            Point3D pos = new Point3D(0,0,0);
            mask = new Edges(first.Edges);
            int cur = -1;
            Piece currentPiece;

            if (first.Edges.back.color == color.Grey)
                this.splitX = true;
            if (first.Edges.right.color == color.Grey)
                this.splitY = true;
            if (first.Edges.top.color == color.Grey)
                this.splitZ = true;

            if (this.splitX)
            {
                maskSize.x = 0; //any depth (X size)
                mask.front.color = color.Grey;
                mask.back.color = color.Grey;
                //take all middle pieces between 2 angles
                cur = this.magic.getByMask(mask, maskSize);
                while (cur != -1)
                {
                    edges.Add(cur);
                    cur = this.magic.getByMask(mask, maskSize);
                }
                mask.back.color = this.cube.Edges.back.color;
                edges.Add(this.magic.getByMask(mask, maskSize));
                foreach (int p in edges)
                {
                    currentPiece = this.magic.getPiece(p);
                    currentPiece.position = new Point3D(pos);
                    pos.x += currentPiece.Size.x;
                }
            }
            else if (this.splitY)
            {
                maskSize.y = 0; //any width (Y size)
                mask.left.color = color.Grey;
                mask.right.color = color.Grey;
                //take all middle pieces between 2 angles
                cur = this.magic.getByMask(mask, maskSize);
                while (cur != -1)
                {
                    edges.Add(cur);
                    cur = this.magic.getByMask(mask, maskSize);
                }
                mask.right.color = this.cube.Edges.right.color;
                edges.Add(this.magic.getByMask(mask, maskSize));
                foreach (int p in edges)
                {
                    currentPiece = this.magic.getPiece(p);
                    currentPiece.position = new Point3D(pos);
                    pos.y += currentPiece.Size.y;
                }
            }
            else if (this.splitZ)
            {
                maskSize.z = 0; //any height (Z size)
                mask.down.color = color.Grey;
                mask.top.color = color.Grey;
                //take all middle pieces between 2 angles
                cur = this.magic.getByMask(mask, maskSize);
                while (cur != -1)
                {
                    edges.Add(cur);
                    cur = this.magic.getByMask(mask, maskSize);
                }
                mask.top.color = this.cube.Edges.top.color;
                edges.Add(this.magic.getByMask(mask, maskSize));
                foreach (int p in edges)
                {
                    currentPiece = this.magic.getPiece(p);
                    currentPiece.position = new Point3D(pos);
                    pos.z += currentPiece.Size.z;
                }
            }
            else
            {
                //cube is just one piece
            }

        }
        public List<Piece> List
        {
            get
            {
                return this.magic.List;
            }
        }
    }
    public class Piece
    {
        private Edges edges;
        private Edges initial_edges;
        private Point3D size = new Point3D();
        private Point3D initial_size;
        public Point3D position = new Point3D(0, 0, 0);
        private int order = 0;
        public bool used = false;
        public Piece(String[] data) : this(data,0)
        {
        }
        public Piece(String[] data, int order)
        {
            //Timus testing system supplies valid data; no error-check
            int.TryParse(data[0], out this.size.x); //F<>B
            int.TryParse(data[1], out this.size.z); //D<>T
            int.TryParse(data[2], out this.size.y); //L<>R
            this.initial_size = new Point3D(this.size.x, this.size.y, this.size.z);
            //edges: tokens 3..8
            coloredSide[] sides = {
                                      new coloredSide((color)data[3][0], (side)'F'),
                                      new coloredSide((color)data[3][1], (side)'B'),
                                      new coloredSide((color)data[3][2], (side)'D'),
                                      new coloredSide((color)data[3][3], (side)'U'),
                                      new coloredSide((color)data[3][4], (side)'L'),
                                      new coloredSide((color)data[3][5], (side)'R'),
                                  };
            this.initial_edges = new Edges(sides);
            this.edges = new Edges(this.initial_edges); //copy everything
            this.order = order;
            
        }
        public void rotateX(int n)
        {
            this.edges.rotateX(n);
            this.size.rotateX(n);
            
        }
        public void rotateY(int n)
        {
            this.edges.rotateY(n);
            this.size.rotateY(n);
        }
        public void rotateZ(int n)
        {
            this.edges.rotateZ(n);
            this.size.rotateZ(n);
        }
        public void reset()
        {
            this.edges = new Edges(this.initial_edges);
            this.size = new Point3D(this.initial_size.x, this.initial_size.y, this.initial_size.z);
        }
        public Edges getAngle(int i)
        {
            //0 = front left down, 1 = front left top
            //...6 = back right top, 7 = back right down
            Edges result = new Edges(this.Edges);
            
            if (i < 4) //front
                result.back.color = color.Any;
            else //back
                result.front.color = color.Any;

            if (i==0 || i==1 || i==4 || i==5) //left
                result.right.color = color.Any;
            else  //right
                result.left.color = color.Any;

            if (i==0 || i==3 || i==4 || i==7) //down
                result.top.color = color.Any;
            else //top
                result.down.color = color.Any;

            return result;

        }
        public String ToAns()
        {
            String sides = this.edges.toAns();
            String pos = this.position.x.ToString() + ' ' +
                this.position.z.ToString() + ' ' +
                this.position.y.ToString();
            return sides + pos;
        }
        public override String ToString()
        {
            return "sz(" + this.size.ToString() + ") " +
                         "pos(" + this.position.ToString() + ") " +
                         "cols(" + this.edges.ToString() + ") used:" + this.used.ToString();
                         //+ " # " + this.ToAns();  
        }
        public Point3D Size
        {
            get
            {
                return this.size;
            }
        }
        public Edges Edges
        {
            get
            {
                return this.edges;
            }
        }
        private bool MaskMatch(Edges mask)
        {
            color[] m = mask.ToArray();
            color[] e = this.edges.ToArray();
            for (int i = 0; i < e.Length; i++)
            {
                if(e[i] != m[i] && m[i] != color.Any) {
                    return false;
                }
            }
            return true;
        }
        public bool Match(Edges mask, Point3D size)
        {
            //color 'Any' means we don't care,
            //size == 0 => we don't care too.
            //bruteforce piece until it matches the mask
            if (this.used)
                return false;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.reset();
                    this.rotateY(i);
                    this.rotateX(j);
                    if (this.MaskMatch(mask) && this.size.MaskMatch(size))
                    {
                        this.used = true;
                        return true; //piece is already rotated to match everything
                    }
                }
            }
            for (int i = 1; i < 4; i+=2)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.reset();
                    this.rotateZ(i);
                    this.rotateX(j);
                    if (this.MaskMatch(mask) && this.size.MaskMatch(size))
                    {
                        this.used = true;
                        return true; //piece is already rotated to match everything
                    }
                }
            }
            this.reset();
            return false;
        }
    }
    public class Edges
    {
        public coloredSide front;
        public coloredSide back;
        public coloredSide down;
        public coloredSide top;
        public coloredSide left;
        public coloredSide right;
        public Edges(color[] colors)
        {
            this.front = new coloredSide((color)colors[0], (side)'F');
            this.back = new coloredSide((color)colors[1], (side)'B');
            this.down = new coloredSide((color)colors[2], (side)'D');
            this.top = new coloredSide((color)colors[3], (side)'U');
            this.left = new coloredSide((color)colors[4], (side)'L');
            this.right = new coloredSide((color)colors[5], (side)'R');
        }
        public Edges(coloredSide[] sides)
        {
            this.front = sides[0];
            this.back = sides[1];
            this.down = sides[2];
            this.top = sides[3];
            this.left = sides[4];
            this.right = sides[5];
            
        }
        public Edges(Edges a)
        {
            /* probably a bad thing */
            this.front = a.front;
            this.back = a.back;
            this.down = a.down;
            this.top = a.top;
            this.left = a.left;
            this.right = a.right;
        }
        public void rotateY(int n)
        {
            bool forward = true;
            if (n < 0)
            {
                forward = false;
            }
            n = Math.Abs(n) % 4;  //significant rotation angle
            if (n == 0)
            {
                return;
            }
            if (forward)
            {
                for (int i = 0; i < n; i++)
                {
                    Program.Swap(ref this.front, ref this.top);
                    Program.Swap(ref this.top, ref this.back);
                    Program.Swap(ref this.back, ref this.down);
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    Program.Swap(ref this.back, ref this.down);
                    Program.Swap(ref this.top, ref this.back);
                    Program.Swap(ref this.front, ref this.top);
                    
                    
                }
            }
        }
        public void rotateX(int n)
        {
            bool clockwise = true;
            if (n < 0)
            {
                clockwise = false;
            }
            n = Math.Abs(n) % 4;  //significant rotation angle
            if (n == 0)
            {
                return;
            }
            if (clockwise)
            {
                for (int i = 0; i < n; i++)
                {
                    Program.Swap(ref this.top, ref this.left);
                    Program.Swap(ref this.left, ref this.down);
                    Program.Swap(ref this.down, ref this.right);
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    Program.Swap(ref this.right, ref this.down);
                    Program.Swap(ref this.down, ref this.left);
                    Program.Swap(ref this.left, ref this.top);


                }
            }
        }
        public void rotateZ(int n)
        {
            bool clockwise = true;
            if (n < 0)
            {
                clockwise = false;
            }
            n = Math.Abs(n) % 4;  //significant rotation angle
            if (n == 0)
            {
                return;
            }
            if (clockwise)
            {
                for (int i = 0; i < n; i++)
                {
                    Program.Swap(ref this.front, ref this.right);
                    Program.Swap(ref this.right, ref this.back);
                    Program.Swap(ref this.back, ref this.left);
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    Program.Swap(ref this.left, ref this.back);
                    Program.Swap(ref this.back, ref this.right);
                    Program.Swap(ref this.right, ref this.front);


                }
            }
        }
        public override string ToString()
        {
            return this.front.color + ", " +
                   this.back.color + ", " +
                   this.down.color + ", " +
                   this.top.color + ", " +
                   this.left.color + ", " +
                   this.right.color;

        }
        public color[] ToArray() {
            return new color[] {
                        this.front.color,
                        this.back.color,
                        this.down.color,
                        this.top.color,
                        this.left.color,
                        this.right.color,
                    };
        }
        public String ToShortString()
        {
            return new String(new char[] {
                        (char)this.front.color,
                        (char)this.back.color,
                        (char)this.down.color,
                        (char)this.top.color,
                        (char)this.left.color,
                        (char)this.right.color}
                    );
                   
        }
        public String toAns()
        {
            return (char)this.front.side + " " +(char)this.down.side + " ";
        }
    }
    public class Point3D
    {
        public int x; //F-B
        public int y; //L-R
        public int z; //D-T
        public Point3D()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }
        public Point3D(int a, int b, int c)
        {
            this.x = a;
            this.y = b; 
            this.z = c;
        }
        public Point3D(Point3D p)
        {
            this.x = p.x;
            this.y = p.y;
            this.z = p.z;
        }
        public override string ToString()
        {
            return this.x + ", " +
                   this.y + ", " +
                   this.z;

        }
        public void rotateX(int n)
        {
            
            n = Math.Abs(n) % 4;  //significant rotation angle
            if (n == 0)
            {
                return;
            }
            
            for (int i = 0; i < n; i++)
            {
                Program.Swap(ref this.z, ref this.y);
            }   
        }
        public void rotateY(int n)
        {

            n = Math.Abs(n) % 4;  //significant rotation angle
            if (n == 0)
            {
                return;
            }

            for (int i = 0; i < n; i++)
            {
                Program.Swap(ref this.z, ref this.x);
            }
        }
        public void rotateZ(int n)
        {

            n = Math.Abs(n) % 4;  //significant rotation angle
            if (n == 0)
            {
                return;
            }

            for (int i = 0; i < n; i++)
            {
                Program.Swap(ref this.x, ref this.y);
            }

        }
        public bool MaskMatch(Point3D p)
        {
            if (    (this.x == p.x || p.x == 0)
                &&  (this.y == p.y || p.y == 0)
                &&  (this.z == p.z || p.z == 0)
                )
                return true;
            return false;
        }
        
    }
    public struct coloredSide
    {
        public coloredSide(color c, side s)
        {
            this.color = c;
            this.side = s;
        }
        public color color;
        public side side;
    }
    public class MagicStructure
    {
        private List<Piece> stuff;
        public MagicStructure(int n)
        {
            this.stuff = new List<Piece>(n);
        }
        public void Add(Piece p)
        {
            this.stuff.Add(p);
        }
        public int getByMask(Edges colors, Point3D size)
        {
            int result = -1;
            for (int i = 0; i < stuff.Count; i++ )
            {
                if (this.stuff[i].Match(colors, size))
                {
                    result = i;
                    break;
                }
            }
            return result;
        }
        public List<Piece> List {
            get
            {
                return this.stuff;
            }
        }
        public Piece getPiece(int i)
        {
            return this.stuff[i];
        }
        
    }
}
