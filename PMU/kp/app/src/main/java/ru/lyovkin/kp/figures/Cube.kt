package ru.lyovkin.kp.figures

import android.content.Context
import android.opengl.GLES20.*
import android.util.Log
import ru.lyovkin.kp.Utils
import ru.lyovkin.kp.gl.GLObject
import ru.lyovkin.kp.gl.MatrixUtils
import ru.lyovkin.kp.gl.ShaderUtils
import java.nio.FloatBuffer
import java.nio.ShortBuffer
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10


class Cube(val ctx: Context) : GLObject {
    private val tag = this.javaClass.simpleName

    private val vertexShader = (
            "attribute vec4 vertexPosition;\n"
                    + "attribute vec3 vertexColour;\n"
                    + "varying vec3 fragColour;\n"
                    + "uniform mat4 projection;\n"
                    + "uniform mat4 modelView;\n"
                    + "void main()\n"
                    + "{\n"
                    + "    gl_Position = projection * modelView * vertexPosition;\n"
                    + "    fragColour = vertexColour;\n"
                    + "}\n"
            )

    private val fragmentShader = (
            "precision mediump float;\n"
                    + "varying vec3 fragColour;\n"
                    + "void main()\n"
                    + "{\n"
                    + "    gl_FragColor = vec4(fragColour, 1.0);\n"
                    + "}\n"
            )

    private val vertices = floatArrayOf(
        -1.0f, 1.0f, -1.0f, /* Back. */
        1.0f, 1.0f, -1.0f,
        -1.0f, -1.0f, -1.0f,
        1.0f, -1.0f, -1.0f,
        -1.0f, 1.0f, 1.0f, /* Front. */
        1.0f, 1.0f, 1.0f,
        -1.0f, -1.0f, 1.0f,
        1.0f, -1.0f, 1.0f,
        -1.0f, 1.0f, -1.0f, /* Left. */
        -1.0f, -1.0f, -1.0f,
        -1.0f, -1.0f, 1.0f,
        -1.0f, 1.0f, 1.0f,
        1.0f, 1.0f, -1.0f, /* Right. */
        1.0f, -1.0f, -1.0f,
        1.0f, -1.0f, 1.0f,
        1.0f, 1.0f, 1.0f,
        -1.0f, -1.0f, -1.0f, /* Top. */
        -1.0f, -1.0f, 1.0f,
        1.0f, -1.0f, 1.0f,
        1.0f, -1.0f, -1.0f,
        -1.0f, 1.0f, -1.0f, /* Bottom. */
        -1.0f, 1.0f, 1.0f,
        1.0f, 1.0f, 1.0f,
        1.0f, 1.0f, -1.0f
    )
    private val verticesBuffer: FloatBuffer = Utils.createBuffer(vertices)

    private val colors = floatArrayOf(
        1.0f, 0.0f, 0.0f,
        1.0f, 0.0f, 0.0f,
        1.0f, 0.0f, 0.0f,
        1.0f, 0.0f, 0.0f,
        0.0f, 1.0f, 0.0f,
        0.0f, 1.0f, 0.0f,
        0.0f, 1.0f, 0.0f,
        0.0f, 1.0f, 0.0f,
        0.0f, 0.0f, 1.0f,
        0.0f, 0.0f, 1.0f,
        0.0f, 0.0f, 1.0f,
        0.0f, 0.0f, 1.0f,
        1.0f, 1.0f, 0.0f,
        1.0f, 1.0f, 0.0f,
        1.0f, 1.0f, 0.0f,
        1.0f, 1.0f, 0.0f,
        0.0f, 1.0f, 1.0f,
        0.0f, 1.0f, 1.0f,
        0.0f, 1.0f, 1.0f,
        0.0f, 1.0f, 1.0f,
        1.0f, 0.0f, 1.0f,
        1.0f, 0.0f, 1.0f,
        1.0f, 0.0f, 1.0f,
        1.0f, 0.0f, 1.0f
    )
    private val colorBuffer: FloatBuffer = Utils.createBuffer(colors)

    private val colorIndices = shortArrayOf(
        0,
        2,
        3,
        0,
        1,
        3,
        4,
        6,
        7,
        4,
        5,
        7,
        8,
        9,
        10,
        11,
        8,
        10,
        12,
        13,
        14,
        15,
        12,
        14,
        16,
        17,
        18,
        16,
        19,
        18,
        20,
        21,
        22,
        20,
        23,
        22
    )
    private val colorIndicesBuffer: ShortBuffer = Utils.createBuffer(colorIndices)

    private var program: Int = 0

    private var vertexLocation = 0
    private var vertexColourLocation = 0
    private var projectionLocation = 0
    private var modelViewLocation = 0

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
        projectionLocation = glGetUniformLocation(program, "projection")
        modelViewLocation = glGetUniformLocation(program, "modelView")

        MatrixUtils.matrixPerspective(
            projectionMatrix,
            45f,
            width.toFloat() / height.toFloat(),
            0.1f,
            100f
        )

        glViewport(0, 0, width, height)
        return true
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

    override fun onSurfaceCreated(gl: GL10, config: EGLConfig) {
    }

    override fun onSurfaceChanged(gl: GL10, width: Int, height: Int) {
        setupGraphics(width, height)
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

        glUniformMatrix4fv(projectionLocation, 1, false, projectionMatrix, 0)
        glUniformMatrix4fv(modelViewLocation, 1, false, modelViewMatrix, 0)

        glDrawElements(GL_TRIANGLES, 36, GL_UNSIGNED_SHORT, colorIndicesBuffer)
    }
}