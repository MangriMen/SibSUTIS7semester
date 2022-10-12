package ru.lyovkin.lab6
import android.content.Context
import android.graphics.BitmapFactory
import android.opengl.GLSurfaceView
import android.opengl.GLUtils
import ru.lyovkin.lab6.JNIWrapper.ondrawframe
import ru.lyovkin.lab6.JNIWrapper.onsurfacechanged
import ru.lyovkin.lab6.JNIWrapper.onsurfacecreated
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10


internal class RendererWrapper(private var c: Context) :
    GLSurfaceView.Renderer {
    private fun loadGLTexture(gl: GL10) {
        gl.glGenTextures(1, textures, 0)
        for (i in texture_name.indices) {
            gl.glBindTexture(GL10.GL_TEXTURE_2D, textures[i])
            gl.glTexParameterf(
                GL10.GL_TEXTURE_2D,
                GL10.GL_TEXTURE_MIN_FILTER,
                GL10.GL_LINEAR.toFloat()
            )
            val `is` = c.resources.openRawResource(texture_name[i])
            val bitmap = BitmapFactory.decodeStream(`is`)
            GLUtils.texImage2D(GL10.GL_TEXTURE_2D, 0, bitmap, 0)
            bitmap.recycle()
        }
    }

    override fun onSurfaceCreated(gl: GL10, config: EGLConfig) {
        loadGLTexture(gl)
        onsurfacecreated()
    }

    override fun onSurfaceChanged(gl: GL10, width: Int, height: Int) {
        onsurfacechanged(width, height)
    }

    override fun onDrawFrame(gl: GL10) {
        ondrawframe()
    }

    companion object {
        var texture_name = intArrayOf(
            R.drawable.paint
        )
        var textures = IntArray(texture_name.size)
    }
}