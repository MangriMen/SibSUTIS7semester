package ru.lyovkin.lab2

import android.opengl.GLSurfaceView
import android.opengl.GLU
import java.nio.ByteBuffer
import java.nio.ByteOrder
import java.nio.FloatBuffer
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10


class Sphere: GLSurfaceView.Renderer {
    private var rotation = 0f

    private val ambient = floatArrayOf(
        0.9f, 0.3f, 0.4f, 1.0f
    )
    private lateinit var ambientBuffer: FloatBuffer

    private val diffuse = floatArrayOf(
        0.2f, 0.6f, 0.6f, 1.0f
    )
    private lateinit var diffuseBuffer: FloatBuffer

    private val specular = floatArrayOf(
        0.2f * 0.4f, 0.2f * 0.6f, 0.2f * 0.8f, 1f
    )
    private lateinit var specularBuffer: FloatBuffer

    private fun init() {
        var b: ByteBuffer = ByteBuffer.allocateDirect(ambient.size * 4)
        b.order(ByteOrder.nativeOrder())
        ambientBuffer = b.asFloatBuffer()
        ambientBuffer.put(ambient)
        ambientBuffer.position(0)

        b = ByteBuffer.allocateDirect(diffuse.size * 4)
        b.order(ByteOrder.nativeOrder())
        diffuseBuffer = b.asFloatBuffer()
        diffuseBuffer.put(diffuse)
        diffuseBuffer.position(0)

        b = ByteBuffer.allocateDirect(specular.size * 4)
        b.order(ByteOrder.nativeOrder())
        specularBuffer = b.asFloatBuffer()
        specularBuffer.put(specular)
        specularBuffer.position(0)
    }

    private fun draw(gl: GL10) {
        var cos: Float
        var sin: Float
        var r1: Float
        var r2: Float
        var h1: Float
        var h2: Float

        val step = 15.0f

        val v = Array(64) { FloatArray(3) }

        val vbb: ByteBuffer = ByteBuffer.allocateDirect(v.size * v[0].size * 4)
        vbb.order(ByteOrder.nativeOrder())
        val vBuf: FloatBuffer = vbb.asFloatBuffer()

        gl.glEnableClientState(GL10.GL_VERTEX_ARRAY)
        gl.glEnableClientState(GL10.GL_NORMAL_ARRAY)

        var angleA = -90.0f
        var angleB: Float
        while (angleA < 90.0f) {
            var n = 0
            r1 = kotlin.math.cos(angleA * Math.PI / 180.0).toFloat()
            r2 = kotlin.math.cos((angleA + step) * Math.PI / 180.0).toFloat()
            h1 = kotlin.math.sin(angleA * Math.PI / 180.0).toFloat()
            h2 = kotlin.math.sin((angleA + step) * Math.PI / 180.0).toFloat()

            angleB = 0.0f
            while (angleB <= 360.0f) {
                cos = kotlin.math.cos(angleB * Math.PI / 180.0).toFloat()
                sin = -kotlin.math.sin(angleB * Math.PI / 180.0).toFloat()
                v[n][0] = r2 * cos
                v[n][1] = h2
                v[n][2] = r2 * sin
                v[n + 1][0] = r1 * cos
                v[n + 1][1] = h1
                v[n + 1][2] = r1 * sin
                vBuf.put(v[n])
                vBuf.put(v[n + 1])

                n += 2
                if (n > 63) {
                    vBuf.position(0)
                    gl.glVertexPointer(3, GL10.GL_FLOAT, 0, vBuf)
                    gl.glNormalPointer(GL10.GL_FLOAT, 0, vBuf)
                    gl.glDrawArrays(GL10.GL_TRIANGLE_STRIP, 0, n)
                    n = 0
                    angleB -= step
                }
                angleB += step
            }
            vBuf.position(0)

            gl.glVertexPointer(3, GL10.GL_FLOAT, 0, vBuf)
            gl.glNormalPointer(GL10.GL_FLOAT, 0, vBuf)
            gl.glDrawArrays(GL10.GL_TRIANGLE_STRIP, 0, n)

            angleA += step
        }

        gl.glDisableClientState(GL10.GL_VERTEX_ARRAY)
        gl.glDisableClientState(GL10.GL_NORMAL_ARRAY)
    }

    override fun onSurfaceCreated(gl: GL10?, config: EGLConfig?) {
        gl?.glClearDepthf(1.0f)
        gl?.glEnable(GL10.GL_DEPTH_TEST)
        gl?.glDepthFunc(GL10.GL_LEQUAL)
        gl?.glHint(GL10.GL_PERSPECTIVE_CORRECTION_HINT, GL10.GL_FASTEST)

        init()
    }

    override fun onSurfaceChanged(gl: GL10?, width: Int, height: Int) {
        gl?.glViewport(0, 0, width, height)
        gl?.glMatrixMode(GL10.GL_PROJECTION)
        gl?.glLoadIdentity()
        GLU.gluPerspective(gl, 65.0f, width.toFloat() / height, 0.1f, 50.0f)
        gl?.glMatrixMode(GL10.GL_MODELVIEW)
        gl?.glLoadIdentity()
    }

    override fun onDrawFrame(gl: GL10?) {
        gl?.glClear(GL10.GL_COLOR_BUFFER_BIT or GL10.GL_DEPTH_BUFFER_BIT)
        gl?.glLoadIdentity()
        gl?.glTranslatef(0.0f, 0.0f, -3.0f)
        gl?.glRotatef(rotation, 0f, 1.0f, 0f)
        gl?.glEnable(GL10.GL_LIGHTING)
        gl?.glEnable(GL10.GL_LIGHT0)
        gl?.glMaterialfv(GL10.GL_FRONT_AND_BACK, GL10.GL_AMBIENT, ambientBuffer)
        gl?.glMaterialfv(GL10.GL_FRONT_AND_BACK, GL10.GL_DIFFUSE, diffuseBuffer)
        gl?.glMaterialfv(GL10.GL_FRONT_AND_BACK, GL10.GL_SPECULAR, specularBuffer)

        if (gl != null) {
            draw(gl)
        }

        rotation -= 1.15f
    }
}