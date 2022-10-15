package ru.lyovkin.lab3.entity

import android.content.Context
import android.graphics.BitmapFactory
import android.opengl.GLUtils
import java.nio.ByteBuffer
import java.nio.ByteOrder
import java.nio.FloatBuffer
import javax.microedition.khronos.opengles.GL10
import kotlin.math.cos
import kotlin.math.sin

class Globe(
    private var stacks: Int,
    private var slices: Int,
    radius: Float,
    private var squash: Float,
    gl: GL10,
    context: Context,
    imageId: Boolean,
    resourceId: Int
) {
    private var vertexData: FloatBuffer? = null
    private var normalData: FloatBuffer? = null
    private var colorData: FloatBuffer? = null
    private var textureData: FloatBuffer? = null
    private var scale = 0f
    var pos = floatArrayOf(0.0f, 0.0f, 0.0f)
    private fun init(
        stacks: Int,
        slices: Int,
        radius: Float,
        squash: Float,
        gl: GL10,
        context: Context,
        imageId: Boolean,
        resourceId: Int
    ) {
        var textData: FloatArray? = null
        var blue = 0f
        var red = 1.0f
        var vIndex = 0 //vertex index
        var cIndex = 0 //color index
        var nIndex = 0 //normal index
        var tIndex = 0 //texture index
        if (imageId) {
            createTexture(gl, context, resourceId)
        }
        scale = radius
        this.squash = squash
        val colorIncrement = 1.0f / stacks.toFloat()
        this.stacks = stacks
        this.slices = slices

        val vertexData = FloatArray(3 * ((this.slices * 2 + 2) * this.stacks))
        val colorData = FloatArray(4 * (this.slices * 2 + 2) * this.stacks)
        val normalData = FloatArray(3 * ((this.slices * 2 + 2) * this.stacks))
        if (imageId) textData =
            FloatArray(2 * ((this.slices * 2 + 2) * this.stacks)) // массив координат для текстуры
        var thetaIdx: Int

        // Широта
        var phiIdx = 0
        while (phiIdx < this.stacks) {

            // Первый круг.
            val phi0 = Math.PI.toFloat() * (phiIdx.toFloat() * (1.0f / this.stacks.toFloat()) - 0.5f)

            // Следующий.
            val phi1 =
                Math.PI.toFloat() * ((phiIdx + 1).toFloat() * (1.0f / this.stacks.toFloat()) - 0.5f)
            val cosPhi0 = cos(phi0.toDouble()).toFloat()
            val sinPhi0 = sin(phi0.toDouble()).toFloat()
            val cosPhi1 = cos(phi1.toDouble()).toFloat()
            val sinPhi1 = sin(phi1.toDouble()).toFloat()
            var cosTheta: Float
            var sinTheta: Float

            thetaIdx = 0
            while (thetaIdx < this.slices) {
                val theta =
                    (2.0f * Math.PI.toFloat() * thetaIdx.toFloat() * (1.0 / (this.slices - 1).toFloat())).toFloat()
                cosTheta = cos(theta.toDouble()).toFloat()
                sinTheta = sin(theta.toDouble()).toFloat()

                vertexData[vIndex] = scale * cosPhi0 * cosTheta
                vertexData[vIndex + 1] = scale * (sinPhi0 * this.squash)
                vertexData[vIndex + 2] = scale * (cosPhi0 * sinTheta)
                vertexData[vIndex + 3] = scale * cosPhi1 * cosTheta
                vertexData[vIndex + 4] = scale * (sinPhi1 * this.squash)
                vertexData[vIndex + 5] = scale * (cosPhi1 * sinTheta)

                normalData[nIndex + 3] = cosPhi1 * cosTheta
                normalData[nIndex + 5] = cosPhi1 * sinTheta
                normalData[nIndex + 4] = sinPhi1
                if (textData != null) {   // вычисление координат текстуры
                    val texX = thetaIdx.toFloat() * (1.0f / (this.slices - 1).toFloat())
                    textData[tIndex] = texX
                    textData[tIndex + 1] = phiIdx.toFloat() * (1.0f / this.stacks.toFloat())
                    textData[tIndex + 2] = texX
                    textData[tIndex + 3] = (phiIdx + 1).toFloat() * (1.0f / this.stacks.toFloat())
                }
                colorData[cIndex] = red
                colorData[cIndex + 1] = 0f
                colorData[cIndex + 2] = blue
                colorData[cIndex + 4] = red
                colorData[cIndex + 5] = 0f
                colorData[cIndex + 6] = blue
                colorData[cIndex + 3] = 1.0.toFloat()
                colorData[cIndex + 7] = 1.0.toFloat()
                cIndex += 2 * 4
                vIndex += 2 * 3
                nIndex += 2 * 3
                if (textData != null) // продвижение массива координат для хранения след. набора значений
                    tIndex += 2 * 2
                blue += colorIncrement
                red -= colorIncrement

                vertexData[vIndex + 3] = vertexData[vIndex - 3]
                vertexData[vIndex] = vertexData[vIndex + 3]
                vertexData[vIndex + 4] = vertexData[vIndex - 2]
                vertexData[vIndex + 1] = vertexData[vIndex + 4]
                vertexData[vIndex + 5] = vertexData[vIndex - 1]
                vertexData[vIndex + 2] = vertexData[vIndex + 5]
                normalData[nIndex + 3] = normalData[nIndex - 3]
                normalData[nIndex] = normalData[nIndex + 3]
                normalData[nIndex + 4] = normalData[nIndex - 2]
                normalData[nIndex + 1] = normalData[nIndex + 4]
                normalData[nIndex + 5] = normalData[nIndex - 1]
                normalData[nIndex + 2] = normalData[nIndex + 5]
                if (textData != null) {
                    textData[tIndex + 2] = textData[tIndex - 2]
                    textData[tIndex] = textData[tIndex + 2]
                    textData[tIndex + 3] = textData[tIndex - 1]
                    textData[tIndex + 1] = textData[tIndex + 3]
                }
                thetaIdx++
            }
            phiIdx++
        }
        pos[0] = 0.0f
        pos[1] = 0.0f
        pos[2] = 0.0f
        this.vertexData = makeFloatBuffer(vertexData)
        this.normalData = makeFloatBuffer(normalData)
        this.colorData = makeFloatBuffer(colorData)
        textData?.let {
            textureData = makeFloatBuffer(it)
        }
    }

    fun draw(gl: GL10) {
        with(gl) {
            glMatrixMode(GL10.GL_MODELVIEW)
            glEnableClientState(GL10.GL_VERTEX_ARRAY)
            glEnableClientState(GL10.GL_NORMAL_ARRAY)
            glEnableClientState(GL10.GL_COLOR_ARRAY)
            if (textureData != null) {
                glEnable(GL10.GL_TEXTURE_2D)
                glEnableClientState(GL10.GL_TEXTURE_COORD_ARRAY)
                glBindTexture(GL10.GL_TEXTURE_2D, textures[0])
                glTexCoordPointer(2, GL10.GL_FLOAT, 0, textureData)
            }
            glVertexPointer(3, GL10.GL_FLOAT, 0, vertexData)
            glNormalPointer(GL10.GL_FLOAT, 0, normalData)
            glColorPointer(4, GL10.GL_FLOAT, 0, colorData)
            glDrawArrays(GL10.GL_TRIANGLE_STRIP, 0, (slices + 1) * 2 * (stacks - 1) + 2)
            glDisable(GL10.GL_TEXTURE_2D)
            glDisableClientState(GL10.GL_TEXTURE_COORD_ARRAY)
        }
    }

    fun setPosition(x: Float, y: Float, z: Float) {
        pos[0] = x
        pos[1] = y
        pos[2] = z
    }

    private val textures = IntArray(1)
    private fun createTexture(gl: GL10, context: Context, resource: Int) {
        val tempImage = BitmapFactory.decodeResource(context.resources, resource)
        with(gl) {
            glGenTextures(1, textures, 0)
            glBindTexture(GL10.GL_TEXTURE_2D, textures[0])
            GLUtils.texImage2D(GL10.GL_TEXTURE_2D, 0, tempImage, 0)
            glTexParameterf(
                GL10.GL_TEXTURE_2D,
                GL10.GL_TEXTURE_MIN_FILTER,
                GL10.GL_LINEAR.toFloat()
            )
        }
        tempImage.recycle()
    }

    companion object {
        private fun makeFloatBuffer(arr: FloatArray) =
            ByteBuffer.allocateDirect(arr.size * 4).apply {
                order(ByteOrder.nativeOrder())
            }.asFloatBuffer().apply {
                put(arr)
                position(0)
            }
    }

    init {
        init(stacks, slices, radius, squash, gl, context, imageId, resourceId)
    }
}