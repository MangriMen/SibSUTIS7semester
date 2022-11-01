package ru.lyovkin.kp

import java.nio.*

class Utils {
    companion object {
        fun createBuffer(array: FloatArray): FloatBuffer = ByteBuffer
            .allocateDirect(array.size * Float.SIZE_BYTES)
            .order(ByteOrder.nativeOrder())
            .asFloatBuffer().apply {
                put(array)
                position(0)
            }

        fun createBuffer(array: IntArray): IntBuffer = ByteBuffer
            .allocateDirect(array.size * Int.SIZE_BYTES)
            .order(ByteOrder.nativeOrder())
            .asIntBuffer().apply {
                put(array)
                position(0)
            }

        fun createBuffer(array: ShortArray): ShortBuffer = ByteBuffer
            .allocateDirect(array.size * Short.SIZE_BYTES)
            .order(ByteOrder.nativeOrder())
            .asShortBuffer().apply {
                put(array)
                position(0)
            }
    }
}