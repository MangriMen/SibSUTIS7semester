package ru.lyovkin.kp.gl

import android.content.Context
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10

interface GLObject {
    var x: Float
    var y: Float
    var z: Float

    var rotateX: Float
    var rotateY: Float
    var rotateZ: Float

    var scaleX: Float
    var scaleY: Float
    var scaleZ: Float

    fun onSurfaceCreated(gl: GL10, config: EGLConfig)
    fun onSurfaceChanged(gl: GL10, width: Int, height: Int)
    fun onDrawFrame(gl: GL10)
}