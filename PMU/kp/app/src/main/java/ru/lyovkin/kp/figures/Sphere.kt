package ru.lyovkin.kp.figures

import android.content.Context
import android.opengl.GLES20
import ru.lyovkin.kp.GLObject
import ru.lyovkin.kp.MyRenderer
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10

class Sphere(override val ctx: Context): GLObject {
    override fun onSurfaceCreated(gl: GL10, config: EGLConfig) {
//        MyRenderer.loadTextures(ctx,)
    }

    override fun onSurfaceChanged(gl: GL10, width: Int, height: Int) {
    }

    override fun onDrawFrame(gl: GL10) {
    }
}