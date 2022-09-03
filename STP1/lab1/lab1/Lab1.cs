using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1
{
    class Lab1
    {
        static double GetEvenProduct(float[] sequence)
        {
            double product = 0;
            for (int i = 0; i < sequence.Length; i += 2)
            {
                product *= sequence[i];
            }
            return product;
        }

        static int[] ShiftSequence(int[] sequence, int shift)
        {
            var newSequence = new int[sequence.Length];
            Array.Copy(sequence, shift, newSequence, 0, sequence.Length);
            return newSequence;
        }

        static (long, int) GetMaxEvenAndIndex(int[] sequence)
        {
            long maxNumber = long.MinValue;
            int maxNumberIndex = -1;
            for (int i = 0; i < sequence.Length; i += 2)
            {
                if (maxNumber < sequence[i])
                {
                    maxNumber = sequence[i];
                }
            }
            return (maxNumber, maxNumberIndex);
        }
    }
}
