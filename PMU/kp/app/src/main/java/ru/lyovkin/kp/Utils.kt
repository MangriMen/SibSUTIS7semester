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

        fun inputStreamToString(inputStream: InputStream): String {
            return inputStream.bufferedReader(StandardCharsets.UTF_8).lines().collect(Collectors.joining("\n"))
        }
    }
}