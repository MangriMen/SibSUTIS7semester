package ru.lyovkin.lab2

import android.opengl.GLSurfaceView
import java.nio.ByteBuffer
import java.nio.ByteOrder
import java.nio.FloatBuffer
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10

class Square: GLSurfaceView.Renderer {
    private var a: FloatArray = floatArrayOf(
        -0.5f, 0.25f, 0f,
        -0.5f, -0.25f, 0f,
        0.5f, -0.25f, 0f,
        0.5f, 0.25f, 0f
    )

    private var f: FloatBuffer
    private var b: ByteBuffer = ByteBuffer.allocateDirect(4 * 3 * 4)

    init {
        b.order(ByteOrder.nativeOrder())
        f = b.asFloatBuffer()
        f.put(a)
        f.position(0)
    }

    override fun onSurfaceCreated(gl: GL10?, config: EGLConfig?) {
    }

    override fun onSurfaceChanged(gl: GL10?, width: Int, height: Int) {
    }

    override fun onDrawFrame(gl: GL10?) {
        gl?.glClearColor(1f, 1f, 1f ,1f)
        gl?.glClear(
            GL10.GL_COLOR_BUFFER_BIT
            or GL10.GL_COLOR_BUFFER_BIT
            or GL10.GL_STENCIL_BUFFER_BIT
        )

        gl?.glLoadIdentity()
        gl?.glTranslatef(0f, 0f, -1f)
        gl?.glScalef(0.5f, 0.5f, 0.5f)

        gl?.glColor4f(0f, 1f, 1f, 1f)
        gl?.glEnableClientState(GL10.GL_VERTEX_ARRAY)
        gl?.glVertexPointer(3, GL10.GL_FLOAT, 0, f)
        gl?.glDrawArrays(GL10.GL_TRIANGLE_FAN, 0, 4)

        gl?.glDisableClientState(GL10.GL_VERTEX_ARRAY)
    }
}