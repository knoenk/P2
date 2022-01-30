using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CBT
{
    /*public static class DictionaryExtensions
    {
        public static void AddOrUpdate(this Dictionary<string, List<string>> targetDictionary, string key, string entry)
        {
            if (!targetDictionary.ContainsKey(key))
                targetDictionary.Add(key, new List<string>());

            targetDictionary[key].Add(entry);
        }
    }*/

    public class CBT
    {

        public static void cbt(Sudoku s)
        {
            //Dictionary<int, List<int>> column_c = ConstraintC(s);
            //Dictionary<int, List<int>> row_c = ConstraintR(s);
            //Dictionary<int, List<int>> block_c = ConstraintB(s);
            s.PrintSudoku(); 
            //foreach (var item in row_c)
            //{
            //    Console.WriteLine($"{item.Key}: {string.Join(", ", item.Value)}");
            //}

            //if (row_c[7].Contains(2))
            //    Console.WriteLine("Zit erin");
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

            // gaat nog over gefixeerde waardes heen
            // kan niet verder dan 1 terug

            while (row <= 8 && col <= 9)
            {
                List<int> part = partial;
                List<int> no = new List<int> { 9,8,7,6,5,4,3,2,1 };

                //inhoud van thrashing bekijken:
                for(int t=0; t < thrashing.Count; t++)
                {
                    Console.WriteLine("## thrashlist nr "+t);
                    foreach(int thrashint in thrashing[t])
                    {
                        Console.WriteLine("# "+thrashint);
                    }
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

                        if (col == 0 && row != 0)
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
                            s[row, col] = 0;
                            col--;
                            Point p = new Point(col, row);
                            if (s.fixedlist.Contains(p) && col > 0)
                                col--;
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
                        }

                        Point p = new Point(col_for, row_for);
                        if (s.fixedlist.Contains(p))
                            col_for++;
                        
                        if (Forward(s, row_for, col_for))
                        {
                            col++;
                            break;
                        }
                        else
                        {
                            thrashing.Add(new List<int>(part));
                            part.Remove(s[col,row]);
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

        /*for (int i = 1; i <= 9; i++)
                {
                    if (Partial(s, i, row, col))
                    {
                        s[col, row] = i;
                        

                        int col_for = col + 1;
                        int row_for = row; 
                        if (col_for == 9)
                        {
                            row_for++;
                            col_for = 0;
                        }

                        Point j = new Point(col_for, row_for);
                        if (s.fixedlist.Contains(j))
                            col_for++;

                        if (Forward(s, row_for, col_for))
                        {
                            col++;
                            break;

                        }
                        else
                        {
                            // Hier gaat het nu mis, hij moet als forward leeg is en er geen andere optie is voor het getal zelf, hem naar 0 zetten en naar het vorige getal
                            // gaan dus uit deze loop
                            s[col, row] = 0;
                            no.Remove(i);
                            
                            if (isEmpty)
                            {
                                break;
                            }

                        }

                    }
                }*/
        /*while (!Partial(s,i,row,col) && i != 9)
        {
            i++;
        }

        s[col, row] = i;

        Console.WriteLine(i);
        s.PrintSudoku();

        int col_for = col + 1;
        if (col_for == 9)
        {
            row++;
            col_for = 0;
            col = 0;
        }

        Point j = new Point(col_for, row);
        if (s.fixedlist.Contains(j))
            col_for++;

        if (Forward(s,row,col_for))
        {
            col++;
        }
        else
        {
            Point p = new Point(col, row);
            if (!s.fixedlist.Contains(p))
            {
                s[col, row] = 0;
            }
        }*/




        //for (int row = 0; row <= 8; row++)
        //{
        //    for (int col = 0; col <= 9; col++)
        //    {


        //        int block = DetermineBlock(row, col);

        //        for (int i = 1; i <= 9; i++)
        //        {
        //            if (Partial(r, c, b, i, row, col, block))
        //            {
        //                s[col, row] = i;
        //                Dictionary<int, List<int>> col_up = UpdateList(c, col, i);
        //                Dictionary<int, List<int>> row_up = UpdateList(r, row, i);
        //                Dictionary<int, List<int>> block_up = UpdateList(b, block, i);
        //                int col_forward = col + 1;
        //                if (s[row, col_forward] != 0)
        //                    col_forward++;
        //                if (col_forward == 9)
        //                {
        //                    row++;
        //                    col_forward = 0;
        //                }

        //                if (Forward(row_up, col_up, block_up, row, col_forward, row, col_forward))
        //                {
        //                    c = col_up;
        //                    r = row_up;
        //                    b = block_up;
        //                }
        //                else
        //                {
        //                    s[row, col] = 0;
        //                    if (s[row, col - 1] != 0)
        //                        col;
        //                    if (col == 9)
        //                    {
        //                        row++;
        //                        col = 0;
        //                    }
        //                }

        //            }
        //        }

        //    }

        //    s.PrintSudoku();

        //}

        /*if (row == 8 && col == 9)
return true;

if (col==9)
{
row++;
col = 0;
}

if (s[row, col] != 0)
return Solve(s, row, col + 1, r, c, b);
*/


        //while(row <= 8 && col <= 8)
        //{
        //    if (s[row, col] != 0)
        //        col++;

        //    
        //    for(int i =1; i<= 9; i++)
        //    {
        //        if (Partial(r, c, b, i, row, col, block))
        //        {
        //            s[col, row] = i;
        //            c = UpdateList(c, col, i);
        //            r = UpdateList(r, row, i);
        //            b = UpdateList(b, block, i);
        //            col++;
        //        }

        //    }

        //    if (col == 9)
        //    {
        //        row++;
        //        col = 0;
        //    }

        //}

        /*
        for (int i = 1; i <= 9; i++)
        {
            if (Partial(r,c,b,i,row,col,block))
            {

                newsudoku[col, row] = i;
                Console.WriteLine(newsudoku[col, row]);
                newsudoku.PrintSudoku();
                Dictionary<int, List<int>> col_up = UpdateList(c, col, i);
                Dictionary<int, List<int>> row_up = UpdateList(r, row, i);
                Dictionary<int, List<int>> block_up = UpdateList(b, block, i);

                //foreach (var item in col_up)
                //{
                //    Console.WriteLine("Col" + $"{item.Key}: {string.Join(", ", item.Value)}");
                //}

                //foreach (var item in row_up)
                //{
                //    Console.WriteLine("Row" + $"{item.Key}: {string.Join(", ", item.Value)}");
                //}

                //foreach (var item in block_up)
                //{
                //    Console.WriteLine("Block" + $"{item.Key}: {string.Join(", ", item.Value)}");
                //}

                if (Solve(newsudoku, row, col + 1, row_up, col_up, block_up))
                    return true;
            }
            s[col,row] = 0;
        }
        return false;
        */

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
