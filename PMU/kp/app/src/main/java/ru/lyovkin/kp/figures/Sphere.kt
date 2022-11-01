package ru.lyovkin.kp.figures

import android.content.Context
import ru.lyovkin.kp.gl.GLObject
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10

class Sphere(val ctx: Context): GLObject {
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

    override fun onSurfaceCreated(gl: GL10, config: EGLConfig) {
//        MyRenderer.loadTextures(ctx,)
    }

    override fun onSurfaceChanged(gl: GL10, width: Int, height: Int) {
    }

    override fun onDrawFrame(gl: GL10) {
    }
}