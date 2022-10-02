package ru.lyovkin.lab2

import android.opengl.GLSurfaceView
import android.opengl.GLU
import java.nio.ByteBuffer
import java.nio.ByteOrder
import java.nio.FloatBuffer
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10


class Cube: GLSurfaceView.Renderer {
    private var rotation: Float = 0f

    private lateinit var vertexBuffer: FloatBuffer
    private lateinit var colorBuffer: FloatBuffer
    private lateinit var indexBuffer: ByteBuffer

    private val vertices: FloatArray = floatArrayOf(
        -1.0f, -1.0f, -1.0f,
        1.0f, -1.0f, -1.0f,
        1.0f,  1.0f, -1.0f,
        -1.0f, 1.0f, -1.0f,
        -1.0f, -1.0f,  1.0f,
        1.0f, -1.0f,  1.0f,
        1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f
    )

    private val colors: FloatArray = floatArrayOf(
        0.953125f, 0.26171875f, 0.2109375f, 1.0f,
        0.953125f, 0.26171875f, 0.2109375f, 1.0f,
        0.953125f, 0.26171875f, 0.2109375f, 1.0f,
        0.953125f, 0.26171875f, 0.2109375f, 1.0f,
        0.953125f, 0.26171875f, 0.2109375f, 1.0f,
        0.953125f, 0.26171875f, 0.2109375f, 1.0f,
        0.953125f, 0.26171875f, 0.2109375f, 1.0f,
        0.953125f, 0.26171875f, 0.2109375f, 1.0f
    )

    private val indexes: ByteArray = byteArrayOf(
        0, 4, 5, 0, 5, 1,
        1, 5, 6, 1, 6, 2,
        2, 6, 7, 2, 7, 3,
        3, 7, 4, 3, 4, 0,
        4, 7, 6, 4, 6, 5,
        3, 0, 1, 3, 1, 2
    )

    private fun init() {
        var byteBuf = ByteBuffer.allocateDirect(vertices.size * 4)
        byteBuf.order(ByteOrder.nativeOrder())
        vertexBuffer = byteBuf.asFloatBuffer()
        vertexBuffer.put(vertices)
        vertexBuffer.position(0)

        byteBuf = ByteBuffer.allocateDirect(colors.size * 4)
        byteBuf.order(ByteOrder.nativeOrder())
        colorBuffer = byteBuf.asFloatBuffer()
        colorBuffer.put(colors)
        colorBuffer.position(0)

        indexBuffer = ByteBuffer.allocateDirect(indexes.size)
        indexBuffer.put(indexes)
        indexBuffer.position(0)
    }

    override fun onSurfaceCreated(gl: GL10?, config: EGLConfig?) {
        gl?.glClearDepthf(1.0f)
        gl?.glEnable(GL10.GL_DEPTH_TEST)
        gl?.glDepthFunc(GL10.GL_LEQUAL)

        gl?.glHint(GL10.GL_PERSPECTIVE_CORRECTION_HINT, GL10.GL_NICEST)

        init()
    }

    override fun onSurfaceChanged(gl: GL10?, width: Int, height: Int) {
        gl?.glViewport(0, 0, width, height)

        gl?.glMatrixMode(GL10.GL_PROJECTION)
        gl?.glLoadIdentity()

        GLU.gluPerspective(
            gl,
            45.0f,
            width.toFloat() / height.toFloat(),
            0.1f, 100.0f)

        gl?.glMatrixMode(GL10.GL_MODELVIEW)
        gl?.glLoadIdentity()
    }

    override fun onDrawFrame(gl: GL10?) {
        gl?.glClear(GL10.GL_COLOR_BUFFER_BIT or GL10.GL_DEPTH_BUFFER_BIT)
        gl?.glLoadIdentity()

        gl?.glTranslatef(0.0f, 0.0f, -10.0f)
        gl?.glRotatef(rotation, 0.01f, 1.0f, 0f)

        gl?.glEnableClientState(GL10.GL_VERTEX_ARRAY)
        gl?.glEnableClientState(GL10.GL_COLOR_ARRAY)

        gl?.glVertexPointer(3, GL10.GL_FLOAT, 0, vertexBuffer)
        gl?.glColorPointer(4, GL10.GL_FLOAT, 0, colorBuffer)

        gl?.glDrawElements(
            GL10.GL_TRIANGLES, 36, GL10.GL_UNSIGNED_BYTE,
            indexBuffer
        )

        gl?.glDisableClientState(GL10.GL_VERTEX_ARRAY)
        gl?.glDisableClientState(GL10.GL_COLOR_ARRAY)

        rotation -= 1.15f
    }
}