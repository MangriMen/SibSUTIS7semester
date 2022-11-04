package ru.lyovkin.kp.gl

import android.content.Context
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10

class GLObjectGroup(val ctx: Context) : IGLObject {
    private val objects: MutableList<IGLObject> = mutableListOf()

    private var _rotate = floatArrayOf(0f, 0f, 0f)
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
        get() = _rotate[0]
        set(value) {
            _rotate[0] = value
        }

    override var rotateY: Float
        get() = _rotate[1]
        set(value) {
            _rotate[1] = value
        }

    override var rotateZ: Float
        get() = _rotate[2]
        set(value) {
            _rotate[2] = value
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

    override fun setColor(r: Float, g: Float, b: Float, a: Float) {
    }

    override fun setPosition(x: Float, y: Float, z: Float) {
    }

    override fun setRotation(x: Float, y: Float, z: Float) {
    }

    override fun setScale(x: Float, y: Float, z: Float) {
    }

    override fun setLightDirection(x: Float, y: Float, z: Float) {
    }

    fun add(obj: IGLObject) {
        objects.add(obj)
    }

    override fun onSurfaceCreated(gl: GL10, config: EGLConfig) {
        for (obj in objects) {
            obj.onSurfaceCreated(gl, config)
        }
    }

    override fun onSurfaceChanged(gl: GL10, width: Int, height: Int) {
        for (obj in objects) {
            obj.onSurfaceChanged(gl, width, height)
        }
    }

    override fun onDrawFrame(gl: GL10) {
        for (obj in objects) {
//            obj.x += x
//            obj.y += y
//            obj.z += z
//
//            obj.rotateX += rotateX
//            obj.rotateY += rotateY
//            obj.rotateZ += rotateZ
//
//            obj.scaleX += scaleX
//            obj.scaleY += scaleY
//            obj.scaleZ += scaleZ

            obj.onDrawFrame(gl)

//            obj.x -= x
//            obj.y -= y
//            obj.z -= z
//
//            obj.rotateX -= rotateX
//            obj.rotateY -= rotateY
//            obj.rotateZ -= rotateZ
//
//            obj.scaleX -= scaleX
//            obj.scaleY -= scaleY
//            obj.scaleZ -= scaleZ
        }
    }
}