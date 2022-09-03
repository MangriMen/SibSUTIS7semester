using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1
{
    public class Modules
    {
        public static double GetEvenProduct(float[] sequence)
        {
            double product = 1;
            for (int i = 0; i < sequence.Length; i += 2)
            {
                product *= sequence[i];
            }
            return product;
        }

        public static int[] ShiftSequence(int[] sequence, int shift)
        {
            int[] newSequence = new int[sequence.Length];

            Array.Copy(sequence, 0, newSequence, shift, sequence.Length - shift);
            Array.Copy(sequence, sequence.Length - shift, newSequence, 0, shift);

            return newSequence;
        }

        public static (long, int) GetMaxEvenAndIndex(int[] sequence)
        {
            long maxNumber = long.MinValue;
            int maxNumberIndex = -1;
            for (int i = 0; i < sequence.Length; i += 2)
            {
                if (maxNumber < sequence[i])
                {
                    maxNumber = sequence[i];
                    maxNumberIndex = i;
                }
            }
            return (maxNumber, maxNumberIndex);
        }
    }
}
