package ru.lyovkin.kp

import java.io.InputStream
import java.nio.*
import java.nio.charset.StandardCharsets
import java.util.stream.Collectors

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

        fun inputStreamToString(inputStream: InputStream): String {
            return inputStream.bufferedReader(StandardCharsets.UTF_8).lines().collect(Collectors.joining("\n"))
        }
    }
}