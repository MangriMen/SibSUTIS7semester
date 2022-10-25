package ru.lyovkin.kp

import android.content.Context
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10

interface GLObject {
    val ctx:Context

    fun onSurfaceCreated(gl: GL10, config: EGLConfig)
    fun onSurfaceChanged(gl: GL10, width: Int, height: Int)
    fun onDrawFrame(gl: GL10)
}