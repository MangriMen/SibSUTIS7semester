package ru.lyovkin.kp.figures

import android.content.Context
import android.opengl.GLES20.*
import android.util.Log
import ru.lyovkin.kp.Utils
import ru.lyovkin.kp.gl.GLObject
import ru.lyovkin.kp.gl.ShaderUtils
import java.nio.FloatBuffer
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10

class Triangle(val ctx: Context): GLObject {
    private val tag = this.javaClass.simpleName

    private var _angle = floatArrayOf(0f, 0f, 0f)
    private var _position = floatArrayOf(0f, 0f, 0f)
    private var _scale = floatArrayOf(1f, 1f, 1f)

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
        get() = _angle[0]
        set(value) {
            _angle[0] = value
        }

    override var rotateY: Float
        get() = _angle[1]
        set(value) {
            _angle[1] = value
        }

    override var rotateZ: Float
        get() = _angle[2]
        set(value) {
            _angle[2] = value
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

    private val vertexShader = (
            "attribute vec4 vPosition;\n"
                    + "void main()\n"
                    + "{\n"
                    + "  gl_Position = vPosition;\n"
                    + "}\n"
            )

    private val fragmentShader = (
            "precision mediump float;\n"
                    + "void main()\n"
                    + "{\n"
                    + "  gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);\n"
                    + "}\n"
            )

    private val vertices = floatArrayOf(
        0.0f, 1.0f,
        -1.0f, -1.0f,
        1.0f, -1.0f
    )
    private val verticesBuffer: FloatBuffer = Utils.createBuffer(vertices)

    private var program: Int = 0
    private var vertexLocation: Int = 0

    private fun setupGraphics(w: Int, h: Int): Boolean {
        program = ShaderUtils.createProgram(vertexShader, fragmentShader)
        if (program == 0) {
            Log.e(tag, "Could not create program")
            return false
        }
        vertexLocation = glGetAttribLocation(program, "vPosition")
        glViewport(0, 0, w, h)
        return true
    }

    override fun onSurfaceCreated(gl: GL10, config: EGLConfig) {
    }

    override fun onSurfaceChanged(gl: GL10, width: Int, height: Int) {
        setupGraphics(width, height)
    }

    override fun onDrawFrame(gl: GL10) {
        glUseProgram(program)
        glVertexAttribPointer(vertexLocation, 2, GL_FLOAT, false, 0, verticesBuffer)
        glEnableVertexAttribArray(vertexLocation)
        glDrawArrays(GL_TRIANGLES, 0, 3)
    }
}