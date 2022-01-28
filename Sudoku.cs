using System;
using System.Collections.Generic;
using System.Drawing;

namespace CBT
{
    public class Sudoku
    {
        //Sudoku itself
        public int[,] sudoku_matrix;
        public List<Point> fixedlist = new List<Point>();
        //List with the coördinates of the prefixed numbers
        //public List<Tuple<int, int, int, List<int>>> constraints = new List<Tuple<int, int, int, List<int>>>();


        //Methods required to index on a sudoku.
        public int this[int index1, int index2]
        {
            get => sudoku_matrix[index1, index2];
            set => sudoku_matrix[index1, index2] = value;
        }


        //Constructor used when creating the matrix from the input string.
        public Sudoku(string puzzel_string)
        {
            sudoku_matrix = CreateMatrix(puzzel_string);

        }

        public Sudoku(int[,] matrix, List<Point> list)
        {
            sudoku_matrix = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sudoku_matrix[i, j] = matrix[i, j];
                }
            }

            fixedlist = list;
        }

        int[,] CreateMatrix(string c)
        {
            string trimmed = c.TrimStart();
            string[] input = trimmed.Split(' ');
            int k = 0;
            int[,] sudoku = new int[9, 9];
           
            for(int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    int g = Convert.ToInt32(input[k]);
                    if (g != 0)
                        fixedlist.Add(new Point(i, j));
                    sudoku[i, j] = g;
                    k++;
                }
            }
            //foreach (Point i in fixedlist)
                //Console.WriteLine(sudoku[i.X,i.Y]);
            return sudoku;
        }

        // Creates a copy of the matrix.
        public Sudoku CreateCopy()
        {
            return new Sudoku(sudoku_matrix, fixedlist);
        }

        // Pretty prints the sudoku to the screen
        public void PrintSudoku()
        {
            for (int i = 0; i < sudoku_matrix.GetLength(0); i++)
            {
                if (i % 3 == 0 && i != 0)
                    Console.WriteLine("-----------");
                for (int j = 0; j < sudoku_matrix.GetLength(1); j++)
                {
                    if (j % 3 == 0 && j != 0)
                        Console.Write("|");
                    Console.Write("{0}", sudoku_matrix[j, i]);
                }

                Console.WriteLine();
            }
            Console.WriteLine();
        }

        //Gets the a specified column of a matrix
        public int[] getColumn(int c)
        {
            int[] column = new int[9];
            for (int i = 0; i < column.Length; i++)
            {
                column[i] = sudoku_matrix[c, i];
            }
            return column;
        }

        //Gets the specified row of a matrix
        public int[] getRow(int r)
        {
            int[] row = new int[9];
            for (int i = 0; i < row.Length; i++)
            {
                row[i] = sudoku_matrix[i, r];
            }

            return row;
        }

        //Gets the specified block of a matrix
        public int[] getBlock(int r, int c)
        {
            int k = 0;
            int[] block = new int[9];
            // rij 0-2
            if (r >= 0 && r <= 2)
            {
                // block 0-2
                if (c >= 0 && c <= 2)
                {
                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 0; j <= 2; j++)
                        {
                            block[k] = sudoku_matrix[i, j];
                            k++;

                          
                        }
                    }
                }
                //block 3-5
                if (c >= 3 && c <= 5)
                {
                    for (int i = 3; i <= 5; i++)
                    {
                        for (int j = 0; j <= 2; j++)
                        {
                            block[k] = sudoku_matrix[i, j];
                            k++;
                        }
                    }
                    
                }
                // block 6-8
                if (c >= 6 && c <= 8)
                {
                    for (int i = 6; i <= 8; i++)
                    {
                        for (int j = 0; j <= 2; j++)
                        {
                            block[k] = sudoku_matrix[i, j];
                            k++;
                        }
                    }
                    
                }
            }
            // rij 3-5
            if (r >= 3 && r <= 5)
            {
                // block 0-2
                if (c >= 0 && c <= 2)
                {
                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 3; j <= 5; j++)
                        {
                            block[k] = sudoku_matrix[i, j];
                            k++;
                        }
                    }
                  
                }
                // block 3-5 
                if (c >= 3 && c <= 5)
                {
                    for (int i = 3; i <= 5; i++)
                    {
                        for (int j = 3; j <= 5; j++)
                        {
                            block[k] = sudoku_matrix[i, j];
                            k++;
                        }
                    }
                 
                }

                //block 6-8
                if (c >= 6 && c <= 8)
                {
                    for (int i = 6; i <= 8; i++)
                    {
                        for (int j = 3; j <= 5; j++)
                        {
                            block[k] = sudoku_matrix[i, j];
                            k++;
                        }
                    }
                 
                }
            }

            // rij 6-8
            if (r >= 6 && r <= 8)
            {
                //block 0-2
                if (c >= 0 && c <= 2)
                {
                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 6; j <= 8; j++)
                        {
                            block[k] = sudoku_matrix[i, j];
                            k++;
                        }
                    }
                }
                //block 3-5
                if (c >= 3 && c <= 5)
                {
                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 6; j <= 8; j++)
                        {
                            block[k] = sudoku_matrix[i, j];
                            k++;
                        }
                    }

                }
                //block 6-8
                if (c >= 6 && c <= 8)
                {
                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 6; j <= 8; j++)
                        {
                            block[k] = sudoku_matrix[i, j];
                            k++;
                        }
                    }

                }

            }
            return block; 
        }
        /*
        public int[] getBlock(int r, int c)
        {
            int[] block = new int[9];
            int i = 0;
            int col = c;
            while (i < 9)
            {
                block[i] = sudoku_matrix[c, r];
                c++;
                i++;
                if (c == col + 3)
                {
                    c = col;
                    r++;
                }
            }

            /*
            int k = 0; 
            for (int i = r; i <= r + 2; i++)
            {
                for (int j = c; j <= c + 2; j++)
                {
                    block[k] = sudoku_matrix[r, c];
                    k++; 
                }
        }*/


    }
}
