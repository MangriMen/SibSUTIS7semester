package ru.lyovkin.lab1_part_1

import org.junit.Test

import org.junit.Assert.*

/**
 * Example local unit test, which will execute on the development machine (host).
 *
 * See [testing documentation](http://d.android.com/tools/testing).
 */
class ExampleUnitTest {
    @Test
    fun testMaxPos() {
        val expectedValue = 4
        val actualValue = MyMath.max(4, 2)

        assertEquals(expectedValue, actualValue)
    }

    @Test
    fun testMaxMixed1() {
        val expectedValue = 20
        val actualValue = MyMath.max(-40, 20)

        assertEquals(expectedValue, actualValue)
    }

    @Test
    fun testMaxMixed2() {
        val expectedValue = 100
        val actualValue = MyMath.max(100, -1)

        assertEquals(expectedValue, actualValue)
    }

    @Test
    fun testMaxNeg() {
        val expectedValue = -20
        val actualValue = MyMath.max(-100, -20)

        assertEquals(expectedValue, actualValue)
    }

    @Test
    fun testMinPos() {
        val expectedValue = 2
        val actualValue = MyMath.min(40, 2)

        assertEquals(expectedValue, actualValue)
    }

    @Test
    fun testMinMixed1() {
        val expectedValue = -4
        val actualValue = MyMath.min(-4, 2)

        assertEquals(expectedValue, actualValue)
    }

    @Test
    fun testMinMixed2() {
        val expectedValue = -50
        val actualValue = MyMath.min(63, -50)

        assertEquals(expectedValue, actualValue)
    }

    @Test
    fun testMinNeg() {
        val expectedValue = -100
        val actualValue = MyMath.min(-100, -20)

        assertEquals(expectedValue, actualValue)
    }
}