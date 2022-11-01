package ru.lyovkin.kp.gl

import android.opengl.GLES20
import android.util.Log

class ShaderUtils {
    companion object {
        private val tag = ShaderUtils::class.java.simpleName

        private fun loadShader(shaderType: Int, shaderSource: String): Int {
            var shader = GLES20.glCreateShader(shaderType)
            if (shader != 0) {
                GLES20.glShaderSource(shader, shaderSource)
                GLES20.glCompileShader(shader)

                val compiled = IntArray(1)
                GLES20.glGetShaderiv(shader, GLES20.GL_COMPILE_STATUS, compiled, 0)

                if (compiled[0] == 0) {
                    val infoLen = IntArray(1)
                    GLES20.glGetShaderiv(shader, GLES20.GL_INFO_LOG_LENGTH, infoLen, 0)

                    if (infoLen[0] != 0) {
                        val logStr = GLES20.glGetShaderInfoLog(shader)
                        Log.e(tag, "Type: $shaderType\n$logStr")
                    }

                    GLES20.glDeleteShader(shader)
                    shader = 0
                }
            }

            return shader
        }

        fun createProgram(vertexSource: String, fragmentSource: String): Int {
            val vertexShader = loadShader(GLES20.GL_VERTEX_SHADER, vertexSource)
            if (vertexShader == 0) {
                return 0
            }

            val fragmentShader = loadShader(GLES20.GL_FRAGMENT_SHADER, fragmentSource)
            if (fragmentShader == 0) {
                return 0
            }

            var program = GLES20.glCreateProgram()
            GLES20.glAttachShader(program, vertexShader)
            GLES20.glAttachShader(program, fragmentShader)

            GLES20.glLinkProgram(program)
            val linkStatus = intArrayOf(GLES20.GL_FALSE)
            GLES20.glGetProgramiv(program, GLES20.GL_LINK_STATUS, linkStatus, 0)

            if (linkStatus[0] != GLES20.GL_TRUE) {
                val bufLength = IntArray(1)
                GLES20.glGetProgramiv(program, GLES20.GL_INFO_LOG_LENGTH, bufLength, 0)

                if (bufLength[0] == 0) {
                    val logStr = GLES20.glGetProgramInfoLog(program)
                    Log.e(tag, logStr)
                }

                GLES20.glDeleteProgram(program)
                program = 0
            }

            return program
        }
    }
}