using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2
{
    public class Class2
    {
        public static int FindMax(int first, int second)
        {
            if (first > second)
            {
                return first;
            }
            return second;
        }
        public static float ElementsSum(float[,] matrix)
        {
            float sum = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1) - i; j++)
                {
                    sum += matrix[i,j];
                }
            }
            return sum;
        }
        public static float MinElement(float[,] matrix)
        {
            float min = matrix[0,0];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1) - i; j++)
                {
                    if (matrix[i,j] < min)
                    {
                        min = matrix[i,j];
                    }
                }
            }
            return min;
        }
    }
}
