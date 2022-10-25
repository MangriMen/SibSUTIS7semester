package ru.lyovkin.kp

import android.content.Context
import android.graphics.BitmapFactory
import android.opengl.GLES20
import android.opengl.GLSurfaceView
import android.opengl.GLUtils
import ru.lyovkin.kp.figures.Sphere
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10


class MyRenderer(ctx: Context): GLSurfaceView.Renderer {
    private val objects: MutableList<GLObject> = mutableListOf()
    private val bgColorR: Float = 1.0f
    private val bgColorG: Float = 1.0f
    private val bgColorB: Float = 0.0f
    private val bgColorA: Float = 1.0f

    companion object {
        fun loadTextures(ctx:Context, gl: GL10, textures: IntArray, texturesSize: IntArray) {
            gl.glGenTextures(1, texturesSize, 0)
            for (i in textures.indices) {
                gl.glBindTexture(GL10.GL_TEXTURE_2D, texturesSize[i])
                gl.glTexParameterf(
                    GL10.GL_TEXTURE_2D,
                    GL10.GL_TEXTURE_MIN_FILTER,
                    GL10.GL_LINEAR.toFloat()
                )

                val inputStream = ctx.resources.openRawResource(textures[i])
                val bitmap = BitmapFactory.decodeStream(inputStream)
                GLUtils.texImage2D(GL10.GL_TEXTURE_2D, 0, bitmap, 0)
                bitmap.recycle()
            }
        }
    }

    init {
        objects.add(Sphere(ctx))
    }

    override fun onSurfaceCreated(gl: GL10, config: EGLConfig) {
        GLES20.glClearColor(bgColorR, bgColorG, bgColorB, bgColorA)
        GLES20.glEnable(GLES20.GL_DEPTH_TEST);
        GLES20.glDepthFunc(GLES20.GL_LEQUAL);

        for (obj in objects) {
            obj.onSurfaceCreated(gl, config)
        }
    }

    override fun onSurfaceChanged(gl: GL10, width: Int, height: Int) {
        GLES20.glViewport(0, 0, width, height);

        for (obj in objects) {
            obj.onSurfaceChanged(gl, width, height)
        }
    }

    override fun onDrawFrame(gl: GL10) {
        GLES20.glClear(GLES20.GL_COLOR_BUFFER_BIT or GLES20.GL_DEPTH_BUFFER_BIT)
        GLES20.glClearColor(bgColorR, bgColorG, bgColorB, bgColorA)

        for (obj in objects) {
            obj.onDrawFrame(gl)
        }
    }
}