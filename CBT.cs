using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CBT
{
    public class CBT
    {

        public static void cbt(Sudoku s)
        {
            s.PrintSudoku(); 
            Solve(s);
        }

        // Column constraint dictionary 
        public static Dictionary<int,List<int>> ConstraintC(Sudoku s)
        {
            Dictionary<int, List<int>> c_constraint = new Dictionary<int, List<int>>();
            for (int i = 0; i < 9; i++)
            {
                List<int> myValues = new List<int>(){ 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                int [] column = s.getColumn(i);
                for(int j = 0; j<9; j++)
                {
                    if (column[j] != 0)
                        myValues.Remove(column[j]);
                }
                c_constraint.Add(i, myValues);
            }
            return c_constraint;
        }

        // Row constraint dictionary 
        public static Dictionary<int, List<int>> ConstraintR( Sudoku s)
        {
            Dictionary<int, List<int>> r_constraint = new Dictionary<int, List<int>>();
            for (int i = 0; i < 9; i++)
            {
                List<int> myValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                int[] row = s.getRow(i);
                for (int j = 0; j < 9; j++)
                {
                    if (row[j] != 0)
                        myValues.Remove(row[j]);
                }
                r_constraint.Add(i, myValues);
            }

            return r_constraint;
        }

        // Block constraint dictionary
        public static Dictionary<int, List<int>> ConstraintB(Sudoku s)
        {

            Dictionary<int, List<int>> b_constraint = new Dictionary<int, List<int>>();

            for (int i = 0; i < 9; i += 3)
            {
                for (int j = 0; j < 9; j += 3)
                {
                    List<int> myValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                    int[] block = s.getBlock(i, j);
                    int b = DetermineBlock(i, j);
                    for (int k = 0; k < 9; k++)
                    {
                        if (block[k] != 0)
                            myValues.Remove(block[k]);
                    }

                    b_constraint.Add(b, myValues);
                }
            }
            return b_constraint;
        }

        // Updating dictionary after assigning value
        public static Dictionary<int, List<int>> UpdateList(Dictionary<int,List<int>> d, int key, int v)
        {
            d[key].Remove(v);
            return d; 
        }

        public static Dictionary<int, List<int>> AddList(Dictionary<int, List<int>> d, int key, int v)
        {
            d[key].Add(v);
            return d;
        }

        // Solve algorithm 
        static void Solve(Sudoku s)
        {
            int row = 0;
            int col = 0;
            Dictionary<int, List<int>> column_c = ConstraintC(s);
            Dictionary<int, List<int>> row_c = ConstraintR(s);
            Dictionary<int, List<int>> block_c = ConstraintB(s);
           
            List<List<int>> thrashing = new List<List<int>>();
            List<int> partial = new List<int>(); 

            int herhalingen = 0;

            // gaat nog over gefixeerde waardes heen
            // kan niet verder dan 1 terug

            while (row <= 8 && col <= 9)
            {
                List<int> part = partial;
                List<int> no = new List<int> { 9,8,7,6,5,4,3,2,1 };

                //inhoud van thrashing bekijken:
                /*
                for(int t=0; t < thrashing.Count; t++)
                {
                    Console.WriteLine("## thrashlist nr "+t);
                    foreach(int thrashint in thrashing[t])
                    {
                        Console.WriteLine("# "+thrashint);
                    }
                    if (t < 11 || t==64 || t==65)
                    {
                        s.PrintSudoku();
                    }
                    if(t>30)
                    {
                        Console.WriteLine("ERROR");
                        return;
                    }
                }
                */

                herhalingen++;
                if (herhalingen > 200)
                {
                    return;
                }

                
                if (col == 9)
                {
                    row++;
                    col = 0;
                }

                Point pj = new Point(col, row);
                if (s.fixedlist.Contains(pj))
                    col++;

                if (s[col, row] != 0)
                {
                    for (int j = no.Count - 1; j >= 0; j--)
                    {
                        if(no[j] <= s[col,row])
                        {
                            no.Remove(no[j]);
                        }
                    }
                }

                for (int j = no.Count-1; j >= -1; j--)
                {
                    bool isEmpty = IsEmpty(no);
                    int col_for = col + 1;
                    int row_for = row;


                    if (isEmpty)
                    {
                        if (col == 0 && row == 0)
                        {
                            Console.WriteLine("No solution found");
                            return;
                        }

                        if (col == 0 && row > 0)
                        {
                            row--;
                            col = 8;
                            Point q = new Point(col, row);
                            if (s.fixedlist.Contains(q) && col > 0)
                                col--;
                            break;
                        }

                        else
                        {
                            thrashing.Add(new List<int>(part));
                            if(part.Count>0)
                                part.RemoveAt(part.Count-1);
                            
                            s[col,row] = 0;
                            col--;
                            
                            Point p = new Point(col, row);
                            if (s.fixedlist.Contains(p) && col > 0)
                                col--;
                            //s.PrintSudoku();
                            break; 
                        }
                    }
                        
                    if (col_for==9)
                    {
                        row_for++;
                        col_for = 0;
                    }

                    if (Partial(s, no[j], row, col))
                    {
                        s[col, row] = no[j];
                        part.Add(s[col, row]);
                        
                        Console.WriteLine(no[j]);
                        s.PrintSudoku();
                        
                        if (thrashing.Contains(part))
                        {
                            no.Clear(); 
                            Console.WriteLine("thrashing is gebruikt");
                        }

                        Point p = new Point(col_for, row_for);
                        if (s.fixedlist.Contains(p) && !isEmpty)
                            col_for++;
                        
                        if (Forward(s, row_for, col_for) && !isEmpty)
                        {
                            col++;
                            break;
                        }

                        else
                        {
                            thrashing.Add(new List<int>(part));
                            part.RemoveAt(part.Count - 1);
                            
                            s[col, row] = 0;
                            no.Remove(no[j]);
                        }
                    }
                    else
                        no.Remove(no[j]);
                }
                
            }

            Console.WriteLine("Solution found");
            s.PrintSudoku();
        }

        public static bool IsEmpty<T>(List<T> list)
        {
            if (list == null)
            {
                return true;
            }

            return !list.Any();
        }

        //checkt de domeinen die veranderd zijn, dus de rij/kolom/blok als die nu niet leeg zijn dan is het goed. 
        static bool Forward(Sudoku s, int row, int col)
        {
            int[] r = s.getRow(row);
            int[] c = s.getColumn(col);
            int[] b = s.getBlock(row, col);

            for (int i = 1; i <= 9; i++)
            {
                if (!r.Contains(i) && !c.Contains(i) && !b.Contains(i))
                    return true;
            }
            return false; 

        }

        static bool Partial(Sudoku s, int value, int row, int col)
        {
            int[] r = s.getRow(row);
            int[] c = s.getColumn(col);
            int[] b = s.getBlock(row, col);

            if (r.Contains(value))
                return false;
            if (c.Contains(value))
                return false;
            if (b.Contains(value))
                return false;
            return true;

        }

        public static int DetermineBlock(int r, int c)
        {
            // rij 0-2
            if (r >= 0 && r <= 2)
            {
                // block 0-2
                if (c >= 0 && c <= 2)
                    return 1;

                //block 3-5
                if (c >= 3 && c <= 5)
                    return 2;
                // block 6-8
                if (c >= 6 && c <= 8)
                    return 3; 
            }
            // rij 3-5
            if (r >= 3 && r <= 5)
            {
                // block 0-2
                if (c >= 0 && c <= 2)
                    return 4; 
                
            // block 3-5 
                if (c >= 3 && c <= 5)
                    return 5;

                //block 6-8
                if (c >= 6 && c <= 8)
                    return 6;
            }

            // rij 6-8
            if (r >= 6 && r <= 8)
            {
                //block 0-2
                if (c >= 0 && c <= 2)
                    return 7;
                //block 3-5
                if (c >= 3 && c <= 5)
                    return 8;
                //block 6-8
                if (c >= 6 && c <= 8)
                    return 9; 

            }
            return 100;
        }

    }
}
