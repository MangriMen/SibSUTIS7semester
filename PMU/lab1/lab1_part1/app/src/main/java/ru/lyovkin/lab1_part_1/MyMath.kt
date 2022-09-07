package ru.lyovkin.lab1_part_1

class MyMath
{
    companion object {
        fun min(a: Int, b: Int): Int {
            return if (a < b) a else b
        }

        fun max(a: Int, b: Int): Int {
            return if (a > b) a else b
        }
    }
}