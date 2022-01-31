using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CBT
{
    public class CBT
    {

        //cbt is the main method that gets called in our program. 
        //In turn, cbt calls upon our Solve-method with the starting-state sudoku (s) as argument.
        public static void cbt(Sudoku s)
        {
            s.PrintSudoku(); 
            Solve(s);
        }

        //The following three methods fill a dictionary with constraints per column, row and block respectively:

        //Constraints for columns:
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

        //Constraints for rows:
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

        //Constraints for the 3x3-blocks:
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

        ////Solve algorithm 
        //This method doesn't yet work 100%, as indicated in our report. 
        
        //What it does seem to do properly: 
        // - it is filling the sudoku,
        // - after every "fill" (=value assignment of a certain sudoku-spot) it checks whether that move leads to a partial solution,
        // - it keeps track of constraints using the ConstraintC-, -R and -B-methods.

        //Where it goes wrong:
        // - it is almost able to reduce the sudoku-problem; it (partly) keeps track of unsuccessfull combinations of numbers that were filled in the sudoku, 
        //   in the "thrashing"-list. But during the filling of the second row, it gets stuck in a loop. See our report for more details.

        static void Solve(Sudoku s)
        {
            //Setting initials values and constraint-dictionaries:
            int row = 0;
            int col = 0;
            Dictionary<int, List<int>> column_c = ConstraintC(s);
            Dictionary<int, List<int>> row_c = ConstraintR(s);
            Dictionary<int, List<int>> block_c = ConstraintB(s);
           
            List<List<int>> thrashing = new List<List<int>>();
            List<int> between_assignment = new List<int>(); 

            //Optional: setting a maximum number of repeats to prevent endless loop:
            int repeats = 0;

            //Entering the loop that will end when it reaches the final sudoku-spot, or when it reaches an earlier break/return-statement:
            while (row <= 8 && col <= 9)
            {
                //Instantiating part as the list that will track the assignment done so far and domain as the list of possible numbers (nr's 1-9)
                //to assign to each sudoku-spot:
                List<int> part = between_assignment;
                List<int> domain = new List<int> { 9,8,7,6,5,4,3,2,1 };
                
                //Optional max repeats:
                repeats++;
                if (repeats > 100)
                {
                    return;
                }

                //To check if the loop needs to go down one row:
                if (col == 9)
                {
                    row++;
                    col = 0;
                }

                //To check if the current sudoku-spot contains a fixed number:
                Point pj = new Point(col, row);
                if (s.fixedlist.Contains(pj) && !IsEmpty(domain))
                    col++;

                //To check whether any assignments have been done before and if so, removing them from the no-list:
                if (s[col, row] != 0)
                {
                    for (int j = domain.Count - 1; j >= 0; j--)
                    {
                        if(domain[j] <= s[col,row])
                        {
                            domain.Remove(domain[j]);
                        }
                    }
                }

                //Entering a for-loop that will run per sudoku-spot to fill it up with a number from the domain:
                for (int j = domain.Count-1; j >= -1; j--)
                {
                    bool isEmpty = IsEmpty(domain);
                    int col_for = col + 1;
                    int row_for = row;

                    //Check if the domain is empty or not and if so, it backtracks:
                    if (isEmpty)
                    {
                        if (col == 0 && row == 0)
                        {
                            Console.WriteLine("No solution found");
                            return;
                        }

                        //Backtracking if it needs to go back up one row again:
                        if (col == 0 && row > 0)
                        {
                            row--;
                            col = 8;
                            Point q = new Point(col, row);
                            if (s.fixedlist.Contains(q) && col > 0)
                                col--;
                            break;
                        }

                        //Backtracking it just needs to move back left one spot;
                        else
                        {
                            thrashing.Add(new List<int>(part));
                            if(part.Count>0 && !Partial(s, s[col,row],row,col))
                                part.RemoveAt(part.Count-1);
                            
                            s[col,row] = 0;
                            col--;

                            Point p = new Point(col, row);
                            if (s.fixedlist.Contains(p) && col > 0)
                                col--;
                            s.PrintSudoku();
                            break; 
                        }
                    }
                        
                    if (col_for==9)
                    {
                        row_for++;
                        col_for = 0;
                    }

                    //Checking if the current value-assignment is a partial solution:
                    if (Partial(s, domain[j], row, col))
                    {
                        s[col, row] = domain[j];
                        part.Add(s[col, row]);
                        
                        s.PrintSudoku();
                        Console.WriteLine(domain[j]);
                        
                        //This is one place where it seems to go wrong: it needs to check whether a certain "thrashing" combination has occured before and if so,
                        //it should go backtracking again. But it doesn't do this correctly... it goes wrong somewhere with filling and updating the part/thrashing lists.
                        if (thrashing.Contains(part))
                        {
                            domain.Clear(); 
                        }

                        //Checking for a fixed number again:
                        Point p = new Point(col_for, row_for);
                        if (s.fixedlist.Contains(p))
                            col_for++;
                        
                        //Doing a dynamic forward check and if this check it good, it moves on:
                        if (Forward(s, row_for, col_for))
                        {
                            col++;
                            break;
                        }

                        //...and if not, it adds the current value-assignments to our thrashing list:
                        else
                        {
                            thrashing.Add(new List<int>(part));
                            part.RemoveAt(part.Count - 1);
                            
                            s[col, row] = 0;
                            domain.Remove(domain[j]);
                        }
                    }
                    else
                        domain.Remove(domain[j]);
                }
            }
            Console.WriteLine("Solution found");
            s.PrintSudoku();
        }

        //A method for checking whether a list is empty (used to check the domains):
        public static bool IsEmpty<T>(List<T> list)
        {
            if (list == null)
            {
                return true;
            }

            return !list.Any();
        }

        //The method used for forward-checking:
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

        //The method for checking if a current assignment is a partial solution:
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

        //The method for determining in which 3x3-block you are, based on what column and row you're in (counting from left to right, top to bottom, 9 blocks in total):
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
