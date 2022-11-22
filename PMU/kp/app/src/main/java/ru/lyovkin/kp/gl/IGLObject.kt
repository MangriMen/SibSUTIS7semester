package ru.lyovkin.kp.gl

import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10

interface IGLObject {
    var x: Float
    var y: Float
    var z: Float

    var rotateX: Float
    var rotateY: Float
    var rotateZ: Float

    var scaleX: Float
    var scaleY: Float
    var scaleZ: Float

    fun setColor(r: Float, g: Float, b: Float, a: Float = 1f)
    fun setPosition(x: Float, y: Float, z: Float)
    fun setRotation(x: Float, y: Float, z: Float)
    fun setScale(x: Float, y: Float, z: Float)

    fun setLightDirection(x: Float, y: Float, z: Float)

    fun onSurfaceCreated(gl: GL10, config: EGLConfig)
    fun onSurfaceChanged(gl: GL10, width: Int, height: Int)
    fun onDrawFrame(gl: GL10)
}