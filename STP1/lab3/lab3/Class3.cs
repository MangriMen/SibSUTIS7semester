using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{
    public static class Class3
    {
        public static int MaxOfThree(int first, int second, int third)
        {
            if (first > second && first > third)
            {
                return first;
            }
            if (second > first && second > third)
            {
                return second;
            }
            return third;
        }
        public static int EvenNumbers(int a)
        {
            string b = "";
            while (a > 0)
            {
                if (a % 10 % 2 == 0)
                    b += a % 10;

                a /= 10;

            }
            return int.Parse(b);
        }
        public static int MinNumber(int a)
        {
            int min = a + 1;
            while (a > 0)
            {
                if (min > a % 10)
                {
                    min = a % 10;
                }
                a /= 10;
            }
            return min;
        }
        public static int SumOddNumbers(int[,] matrix)
        {
            int sum = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] % 2 != 0 && i > j)
                    {
                        sum += matrix[i, j];
                    }
                }
            }
            return sum;
        }
    }
}
