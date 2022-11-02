package ru.lyovkin.kp.figures

import android.opengl.GLES20.*
import android.util.Log
import de.javagl.obj.Obj
import de.javagl.obj.ObjData
import de.javagl.obj.ObjReader
import de.javagl.obj.ObjUtils
import ru.lyovkin.kp.MainActivity
import ru.lyovkin.kp.Utils
import ru.lyovkin.kp.gl.GLObject
import ru.lyovkin.kp.gl.MatrixUtils
import ru.lyovkin.kp.gl.ShaderUtils
import ru.lyovkin.kp.gl.TextureUtils
import java.io.InputStream
import java.nio.FloatBuffer
import java.nio.IntBuffer
import java.nio.charset.StandardCharsets
import java.util.stream.Collectors
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10


class Object(
    private val verticesBuffer: FloatBuffer,
    private val indicesBuffer: IntBuffer,
    private val texCords: FloatBuffer,
    private var colorBuffer: FloatBuffer = verticesBuffer,
    private var alpha: Float = 1f,
    private var vertexShader: String = "",
    private var fragmentShader: String = "",
) : GLObject {
    companion object {
        private fun fromObj(obj: Obj, vertexShader: String, fragmentShader: String): Object {
            val indices = ObjData.getFaceVertexIndices(obj)
            val vertices = ObjData.getVertices(obj)
            var texCoords = ObjData.getTexCoords(obj, 2)
            texCoords = Utils.createBuffer(FloatArray(vertices.capacity()) { 0f })

            return Object(vertices, indices, texCoords, vertices, 1f, vertexShader, fragmentShader)
        }

        fun fromInputStream(inputStream: InputStream, vertexShader: String, fragmentShader: String): Object {
            return fromObj(
                ObjUtils.convertToRenderable(
                    ObjReader.read(inputStream)
                ),
                vertexShader,
                fragmentShader
            )
        }
    }

    private val tag = this.javaClass.simpleName

    private var program: Int = 0

    private var textureId = 0

    private var vertexLocation = 0
    private var vertexColourLocation = 0
    private var vertexAlphaLocation = 0
    private var textureCordLocation = 0
    private var projectionLocation = 0
    private var modelViewLocation = 0
    private var samplerLocation = 0

    private var projectionMatrix = MatrixUtils.getIdentityMatrix()
    private var modelViewMatrix = MatrixUtils.getIdentityMatrix()

    private var _rotate = floatArrayOf(0f, 0f, 0f)
    private var _position = floatArrayOf(0f, 0f, 0f)
    private var _scale = floatArrayOf(1f, 1f, 1f)

    private fun setupGraphics(width: Int, height: Int): Boolean {
        program = ShaderUtils.createProgram(vertexShader, fragmentShader)

        if (program == 0) {
            Log.e(tag, "Could not create program")
            return false
        }

        vertexLocation = glGetAttribLocation(program, "vertexPosition")
        vertexColourLocation = glGetAttribLocation(program, "vertexColour")
        vertexAlphaLocation = glGetUniformLocation(program, "vertexAlpha")
        textureCordLocation = glGetAttribLocation(program, "vertexTextureCord")
        projectionLocation = glGetUniformLocation(program, "projection")
        modelViewLocation = glGetUniformLocation(program, "modelView")
        samplerLocation = glGetUniformLocation(program, "texture")

        MatrixUtils.matrixPerspective(
            projectionMatrix,
            45f,
            width.toFloat() / height.toFloat(),
            0.1f,
            100f
        )

        glViewport(0, 0, width, height)

        textureId = TextureUtils.loadTexture()
        return textureId != 0
    }

    init {
        setColor(0.8f, 0.8f, 0.9f)
    }

    override var x: Float
        get() = _position[0]
        set(value) {
            _position[0] = value
        }

    override var y: Float
        get() = _position[1]
        set(value) {
            _position[1] = value
        }

    override var z: Float
        get() = _position[2]
        set(value) {
            _position[2] = value
        }

    override var rotateX: Float
        get() = _rotate[0]
        set(value) {
            _rotate[0] = value
        }

    override var rotateY: Float
        get() = _rotate[1]
        set(value) {
            _rotate[1] = value
        }

    override var rotateZ: Float
        get() = _rotate[2]
        set(value) {
            _rotate[2] = value
        }

    override var scaleX: Float
        get() = _scale[0]
        set(value) {
            _scale[0] = value
        }

    override var scaleY: Float
        get() = _scale[1]
        set(value) {
            _scale[1] = value
        }

    override var scaleZ: Float
        get() = _scale[2]
        set(value) {
            _scale[2] = value
        }

    fun setColor(r: Float, g: Float, b: Float, a: Float = 1f) {
        alpha = a

        val color = FloatArray(colorBuffer.capacity()) { 0f }
        var counter = 0
        for (i in color.indices) {
            when (counter) {
                0 -> {
                    color[i] = r
                    counter++
                }
                1 -> {
                    color[i] = g
                    counter++
                }
                2 -> {
                    color[i] = b
                    counter = 0
                }
            }
        }
        colorBuffer = Utils.createBuffer(color)
    }

    override fun onSurfaceCreated(gl: GL10, config: EGLConfig) {
    }

    override fun onSurfaceChanged(gl: GL10, width: Int, height: Int) {
        val result = setupGraphics(width, height)
        if (!result) {
            Log.e(tag, "Error setup graphics")
        }
        else {
            Log.i(tag, "Graphics set up")
        }
    }

    override fun onDrawFrame(gl: GL10) {
        MatrixUtils.matrixIdentityFunction(modelViewMatrix)

        MatrixUtils.matrixRotateX(modelViewMatrix, rotateX)
        MatrixUtils.matrixRotateY(modelViewMatrix, rotateY)
        MatrixUtils.matrixRotateZ(modelViewMatrix, rotateZ)

        MatrixUtils.matrixScale(modelViewMatrix, scaleX, scaleY, scaleZ)

        MatrixUtils.matrixTranslate(modelViewMatrix, x, y, z)

        glUseProgram(program)

        glVertexAttribPointer(vertexLocation, 3, GL_FLOAT, false, 0, verticesBuffer)
        glEnableVertexAttribArray(vertexLocation)

        glVertexAttribPointer(vertexColourLocation, 3, GL_FLOAT, false, 0, colorBuffer)
        glEnableVertexAttribArray(vertexColourLocation)

        glUniform1f(vertexAlphaLocation, alpha)

        glVertexAttribPointer(textureCordLocation, 2, GL_FLOAT, false, 0, texCords)
        glEnableVertexAttribArray(textureCordLocation)

        glUniformMatrix4fv(projectionLocation, 1, false, projectionMatrix, 0)
        glUniformMatrix4fv(modelViewLocation, 1, false, modelViewMatrix, 0)

        glUniform1i(samplerLocation, 0)

        glDrawElements(GL_TRIANGLES, indicesBuffer.capacity(), GL_UNSIGNED_INT, indicesBuffer)
    }
}