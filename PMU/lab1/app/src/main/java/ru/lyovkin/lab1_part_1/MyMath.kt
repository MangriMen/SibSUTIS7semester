package ru.lyovkin.lab1_part_1

class MyMath
{
    companion object {
        /**
         * @param a
         * @param b
         * @return min of a and b
         */
        fun min(a: Int, b: Int): Int {
            return if (a < b) a else b
        }

        /**
         * @param a
         * @param b
         * @return max of a and b
         */
        fun max(a: Int, b: Int): Int {
            return if (a > b) a else b
        }
    }
}