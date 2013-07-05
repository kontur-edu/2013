using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace ConsoleApplication1
{
    enum TypePiece { Left, Center, Right }
    enum CharacterPiece { Width, Depth, Height }

    class Parallelepiped
    {
        public int Width;
        public int Depth;
        public int Height;
        public string ColorParallelepiped;
        public int[] Displacement = new int[6];

        public TypePiece Type = TypePiece.Center;
        public CharacterPiece Character;

       public Parallelepiped(int Width, int Depth, int Height, string ColorParallelepiped)
        {            
            this.Width = Width;
            this.Depth = Depth;
            this.Height = Height;
            this.ColorParallelepiped = ColorParallelepiped;
            for (int i = 0; i < 6; ++i)
            {                
                Displacement[i] = i;
            }                   
        }             

       public static Parallelepiped ConsoleRead()
       {
           string[] tokens = Console.ReadLine().Split(' ');

           int Width = int.Parse(tokens[0]);
           int Depth = int.Parse(tokens[1]);
           int Height = int.Parse(tokens[2]);
           string ColorParallelepiped = tokens[3];

           return new Parallelepiped(Width, Depth, Height, ColorParallelepiped);           
       }

       public void LeadToStandard(Parallelepiped Standard)
       {
           for (int j = 0; j<2;j++)
           {           
               for (int i = 0; i < 5; i += 2)
               {

                   while (this.ColorParallelepiped[i] != Standard.ColorParallelepiped[i])
                   {

                       int pos = Standard.ColorParallelepiped.IndexOf(this.ColorParallelepiped[i]);
                   
                       if (this.ColorParallelepiped[i] == '.' && this.ColorParallelepiped[i + 1] == '.')
                       {
                           break;
                       }

                       if (this.ColorParallelepiped[i] == '.' && this.ColorParallelepiped[i + 1] != '.' && this.ColorParallelepiped[i + 1] == Standard.ColorParallelepiped[i + 1])
                       {
                           break;
                       }

                       if (this.ColorParallelepiped[i] == '.' && this.ColorParallelepiped[i + 1] != '.' && this.ColorParallelepiped[i + 1] != Standard.ColorParallelepiped[i + 1])
                       {
                          pos = Standard.ColorParallelepiped.IndexOf(this.ColorParallelepiped[i+1]);
                      
                       }

                       if (pos % 2 == 1)
                       {
                           pos -= 1;
                       }

                       if (Math.Abs(pos - i) == 1)
                       {
                           pos = Math.Min(i, pos);

                           if (pos % 2 == 1)
                           {
                               pos -= 1;
                           }

                           Swap(pos, pos);
                       }
                       else
                       {
                           Swap(i, pos);
                       }
                  }
               }
           }

           DefinitionType();
       }

       void DefinitionType()
       {
           int pos = this.ColorParallelepiped.IndexOf(".");

           if (pos == -1)
           {
               this.Type = TypePiece.Left;
               return;
           }

           if (pos % 2 == 1)
           {
               if (this.ColorParallelepiped[pos - 1] != '.')
               {
                   this.Type = TypePiece.Left;
               }               
           }
           else
           {

               if (this.ColorParallelepiped[pos + 1] != '.')
               {
                   this.Type = TypePiece.Right;
               }
           }

           if (pos <= 1)
           {
               this.Character = CharacterPiece.Width;
           }

           if (pos >= 2 && pos <= 3)
           {
               this.Character = CharacterPiece.Depth;
           }

           if (pos >= 4)
           {
               this.Character = CharacterPiece.Height;
           }
       }  

       void Swap(int pos1, int pos2)
       {

           if (pos1 > pos2)
           {
               int temp = pos1;
               pos1 = pos2;
               pos2 = temp;
           }
                     
           if (pos1 <= 1 && pos2 >= 2 && pos2 <= 3)
           {
               int temp = this.Width;
               this.Width = this.Depth;
               this.Depth = temp;
           }

           if (pos1 <= 1 && pos2 >=4)
           {
               int temp = this.Width;
               this.Width = this.Height;
               this.Height = temp;
           }

           if (pos1 >= 2 && pos1 <= 3 && pos2 >= 4)
           {
               int temp = this.Depth;
               this.Depth = this.Height;
               this.Height = temp;
           }

           string tempstr = "";

           if (pos2 == pos1)
           {
               if (pos1 % 2 == 1)
               {
                   pos1 -= 1;
                   pos2 = pos1;
               }

               if (pos1 >= 4)
               {
                   if (this.ColorParallelepiped[0] == '.')
                   {
                       pos1 = 0;
                   }

                   if (this.ColorParallelepiped[2] == '.')
                   {
                       pos1 = 2;
                   }
               }
               else
               {
                   pos2 = pos1 + 2;
               }
                            

               if (pos2 <= 3)
               {
                    tempstr = ColorParallelepiped.Substring(pos2 + 2);
               }

                string strpos1 = ColorParallelepiped.Substring(pos1 + 1, 1) + ColorParallelepiped.Substring(pos1, 1);
                string strpos2 = ColorParallelepiped.Substring(pos2 + 1, 1) + ColorParallelepiped.Substring(pos2, 1);

                this.ColorParallelepiped = ColorParallelepiped.Substring(0, pos1) + strpos1 + ColorParallelepiped.Substring(pos1 + 2, pos2 - pos1 - 2) + strpos2 + tempstr;

                int[] temp = new int[2];
                temp[0] = this.Displacement[pos1];
                temp[1] = this.Displacement[pos1 + 1];
                this.Displacement[pos1] = temp[1];
                this.Displacement[pos1 + 1] = temp[0];

                temp[0] = this.Displacement[pos2];
                temp[1] = this.Displacement[pos2 + 1];
                this.Displacement[pos2] = temp[1];
                this.Displacement[pos2 + 1] = temp[0];

           }
           else
           {
               if (pos2 % 2 == 0)
               {
                   if (pos2 <= 3)
                   {
                       tempstr = ColorParallelepiped.Substring(pos2 + 2);
                   }

                   string strpos1 = ColorParallelepiped.Substring(pos1, 2);
                   string strpos2 = ColorParallelepiped.Substring(pos2 + 1, 1) + ColorParallelepiped.Substring(pos2, 1);

                   this.ColorParallelepiped = ColorParallelepiped.Substring(0, pos1) + strpos2 + ColorParallelepiped.Substring(pos1 + 2, pos2 - pos1 - 2) + strpos1 + tempstr;
                                     

                   int[] temp = new int[2];
                   temp[0] = this.Displacement[pos1];
                   temp[1] = this.Displacement[pos1 + 1];
                   this.Displacement[pos1] = this.Displacement[pos2 + 1];
                   this.Displacement[pos1 + 1] = this.Displacement[pos2];
                   this.Displacement[pos2] = temp[0];
                   this.Displacement[pos2 + 1] = temp[1];

               }
               else
               {
                   if (pos2 <= 3)
                   {
                       tempstr = ColorParallelepiped.Substring(pos2 + 1);
                   }
                  
                   string strpos1 = ColorParallelepiped.Substring(pos1 + 1, 1) + ColorParallelepiped.Substring(pos1, 1);
                   string strpos2 = ColorParallelepiped.Substring(pos2 - 1, 2);

                   this.ColorParallelepiped = ColorParallelepiped.Substring(0, pos1) + strpos2 + ColorParallelepiped.Substring(pos1 + 2, pos2 - pos1 - 3) + strpos1 + tempstr;
                   
                   int[] temp = new int[2];
                   temp[0] = this.Displacement[pos1];
                   temp[1] = this.Displacement[pos1 + 1];
                   this.Displacement[pos1] = this.Displacement[pos2 - 1];
                   this.Displacement[pos1 + 1] = this.Displacement[pos2];
                   this.Displacement[pos2 - 1] = temp[1];
                   this.Displacement[pos2] = temp[0];
               }
           }

       }

       public int LengthCharacter()
       {
           int len = 0;

           switch (this.Character)
           {
               case CharacterPiece.Width:
                   len =  this.Width;
                   break;
               case CharacterPiece.Depth:
                   len =  this.Depth;
                   break;
               case CharacterPiece.Height:
                   len =  this.Height;
                   break;                   
           }

           return len;

       }


       public string StrCoord(int LenCharact)
       {
           string str = LenCharact.ToString();

           switch (this.Character)
           {
               case CharacterPiece.Width:
                   str = str + " 0 0";
                   break;
               case CharacterPiece.Depth:
                   str = "0 " + str + " 0";
                   break;
               case CharacterPiece.Height:
                   str = "0 0 " + str;
                   break;
           }

           return str;
       }

                   
    }
        
    class Program
    {     

        static void Main()
        {
            var standard = Parallelepiped.ConsoleRead();   
            int NumberParallelepiped = int.Parse(Console.ReadLine());

            Parallelepiped[] ArrayParallelepipeds = new Parallelepiped[NumberParallelepiped];

            int IndexLeftPiece = 0;
            CharacterPiece charact;

            for (int i = 0; i < NumberParallelepiped; ++i)
            {
                ArrayParallelepipeds[i] = Parallelepiped.ConsoleRead();
                ArrayParallelepipeds[i].LeadToStandard(standard);

                if (ArrayParallelepipeds[i].Type == TypePiece.Left)
                {
                    IndexLeftPiece = i;
                    charact = ArrayParallelepipeds[i].Character;
                    standard.Character = charact;
                }                   
            }

            int CumulativeLength = ArrayParallelepipeds[IndexLeftPiece].LengthCharacter();

            string StandardLocation = "FBDULR";

            for (int i = 0; i < NumberParallelepiped; ++i)
            {
                string outstr = "";                                  

                char front = StandardLocation[ArrayParallelepipeds[i].Displacement[0]];
                char lower = StandardLocation[ArrayParallelepipeds[i].Displacement[2]];

                outstr += front + " " + lower + " ";

                if (ArrayParallelepipeds[i].Type == TypePiece.Left)
                {
                    outstr += ArrayParallelepipeds[i].StrCoord(0);
                }

                if (ArrayParallelepipeds[i].Type == TypePiece.Right)
                {
                    outstr += ArrayParallelepipeds[i].StrCoord(standard.LengthCharacter() - ArrayParallelepipeds[i].LengthCharacter());                    
                }

                if (ArrayParallelepipeds[i].Type == TypePiece.Center)
                {
                    outstr += ArrayParallelepipeds[i].StrCoord(CumulativeLength);
                    CumulativeLength += ArrayParallelepipeds[i].LengthCharacter();
                }

                Console.WriteLine(outstr);
            }     
                                    
        }
        
    }
}
