package ru.lyovkin.kp.gl

import android.opengl.Matrix
import android.util.Log
import kotlin.math.PI
import kotlin.math.cos
import kotlin.math.sin
import kotlin.math.tan

class MatrixUtils {
    companion object {
        fun getIdentityMatrix(): FloatArray {
            val tempMatrix = FloatArray(16)
            matrixIdentityFunction(tempMatrix)
            return tempMatrix
        }

        fun matrixIdentityFunction(matrix: FloatArray) {
            matrix[0] = 1.0f
            matrix[1] = 0.0f
            matrix[2] = 0.0f
            matrix[3] = 0.0f
            matrix[4] = 0.0f
            matrix[5] = 1.0f
            matrix[6] = 0.0f
            matrix[7] = 0.0f
            matrix[8] = 0.0f
            matrix[9] = 0.0f
            matrix[10] = 1.0f
            matrix[11] = 0.0f
            matrix[12] = 0.0f
            matrix[13] = 0.0f
            matrix[14] = 0.0f
            matrix[15] = 1.0f
        }

        fun matrixMultiply(destination: FloatArray, operand1: FloatArray, operand2: FloatArray) {
            val result = FloatArray(16)

            for (i in 0..3) {
                for (j in 0..3) {
                    result[4 * i + j] =
                        operand1[j] * operand2[4 * i] + operand1[4 + j] * operand2[4 * i + 1] +
                                operand1[8 + j] * operand2[4 * i + 2] + operand1[12 + j] * operand2[4 * i + 3]
                }
            }

            for (i in 0..15) {
                destination[i] = result[i]
            }
        }

        fun matrixRotateX(matrix: FloatArray, angle: Float) {
            val tempMatrix = FloatArray(16)
            matrixIdentityFunction(tempMatrix)

            tempMatrix[5] = cos(matrixDegreesToRadians(angle))
            tempMatrix[9] = -sin(matrixDegreesToRadians(angle))
            tempMatrix[6] = sin(matrixDegreesToRadians(angle))
            tempMatrix[10] = cos(matrixDegreesToRadians(angle))

            matrixMultiply(matrix, tempMatrix, matrix)
        }

        fun matrixRotateY(matrix: FloatArray, angle: Float) {
            val tempMatrix = FloatArray(16)
            matrixIdentityFunction(tempMatrix)

            tempMatrix[0] = cos(matrixDegreesToRadians(angle))
            tempMatrix[8] = sin(matrixDegreesToRadians(angle))
            tempMatrix[2] = -sin(matrixDegreesToRadians(angle))
            tempMatrix[10] = cos(matrixDegreesToRadians(angle))

            matrixMultiply(matrix, tempMatrix, matrix)
        }

        fun matrixRotateZ(matrix: FloatArray, angle: Float) {
            val tempMatrix = FloatArray(16)
            matrixIdentityFunction(tempMatrix)

            tempMatrix[0] = cos(matrixDegreesToRadians(angle))
            tempMatrix[4] = -sin(matrixDegreesToRadians(angle))
            tempMatrix[1] = sin(matrixDegreesToRadians(angle))
            tempMatrix[5] = cos(matrixDegreesToRadians(angle))

            matrixMultiply(matrix, tempMatrix, matrix)
        }

        fun matrixTranslate(matrix: FloatArray, x: Float, y: Float, z: Float) {
            val tempMatrix = FloatArray(16)
            matrixIdentityFunction(tempMatrix)

            tempMatrix[12] = x
            tempMatrix[13] = y
            tempMatrix[14] = z

            matrixMultiply(matrix, tempMatrix, matrix)
        }

        fun matrixPerspective(
            matrix: FloatArray,
            fieldOfView: Float,
            aspectRatio: Float,
            zNear: Float,
            zFar: Float
        ) {
            val yMax: Float = (zNear * tan(fieldOfView * PI / 360.0)).toFloat()
            val xMax: Float = yMax * aspectRatio
            matrixFrustum(matrix, -xMax, xMax, -yMax, yMax, zNear, zFar)
        }

        private fun matrixFrustum(
            matrix: FloatArray,
            left: Float,
            right: Float,
            bottom: Float,
            top: Float,
            zNear: Float,
            zFar: Float
        ) {
            val temp: Float = 2.0f * zNear
            val xDistance: Float = right - left
            val yDistance: Float = top - bottom
            val zDistance: Float = zFar - zNear
            matrixIdentityFunction(matrix)

            matrix[0] = temp / xDistance
            matrix[5] = temp / yDistance
            matrix[8] = (right + left) / xDistance
            matrix[9] = (top + bottom) / yDistance
            matrix[10] = (-zFar - zNear) / zDistance
            matrix[11] = -1.0f
            matrix[14] = (-temp * zFar) / zDistance
            matrix[15] = 0.0f
        }

        fun matrixScale(matrix: FloatArray, x: Float, y: Float, z: Float) {
            val tempMatrix = FloatArray(16)
            matrixIdentityFunction(tempMatrix)

            tempMatrix[0] = x
            tempMatrix[5] = y
            tempMatrix[10] = z
            matrixMultiply(matrix, tempMatrix, matrix);
        }

        fun matrixDegreesToRadians(degrees: Float): Float {
            return (PI * degrees / 180.0f).toFloat()
        }
    }
}