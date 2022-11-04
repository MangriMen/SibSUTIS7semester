package ru.lyovkin.kp.gl

import android.opengl.GLES20.*
import android.util.Log
import de.javagl.obj.Obj
import de.javagl.obj.ObjData
import de.javagl.obj.ObjReader
import de.javagl.obj.ObjUtils
import ru.lyovkin.kp.Utils
import java.io.InputStream
import java.nio.FloatBuffer
import java.nio.IntBuffer
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10


class GLObject(
    private val indicesBuffer: IntBuffer,
    private val verticesBuffer: FloatBuffer,
    private val texCordsBuffer: FloatBuffer,
    private val normalBuffer: FloatBuffer,
    private var vertexShader: String = "",
    private var fragmentShader: String = "",
    private val texture: InputStream? = null,
    private var colorBuffer: FloatBuffer = verticesBuffer,
    private var alpha: Float = 1f,
) : IGLObject {
    private val tag = this.javaClass.simpleName

    companion object {
        private fun fromObj(
            vertexShader: String,
            fragmentShader: String,
            obj: Obj,
            texture: InputStream? = null
        ): GLObject {
            val indices = ObjData.getFaceVertexIndices(obj)
            val vertices = ObjData.getVertices(obj)
            val texCoords = ObjData.getTexCoords(obj, 2)
            val normals = ObjData.getNormals(obj)

            return GLObject(
                indices,
                vertices,
                texCoords,
                normals,
                vertexShader,
                fragmentShader,
                texture,
            )
        }

        fun fromInputStream(
            vertexShader: String,
            fragmentShader: String,
            model: InputStream,
            texture: InputStream? = null
        ): GLObject {
            return fromObj(
                vertexShader,
                fragmentShader,
                ObjUtils.convertToRenderable(
                    ObjReader.read(model)
                ),
                texture,
            )
        }
    }

    /* GL ids */
    private var programId = 0
    private var textureId = 0

    /* Vertex shader */
    private var vertexLocation = 0
    private var vertexColorLocation = 0
    private var vertexAlphaLocation = 0
    private var textureCordLocation = 0
    private var vertexNormalLocation = 0
    private var lightDirection = 0
    private var projectionLocation = 0
    private var modelViewLocation = 0

    private var projectionMatrix = MatrixUtils.getIdentityMatrix()
    private var modelViewMatrix = MatrixUtils.getIdentityMatrix()

    /* Fragment shader */
    private var samplerLocation = 0

    /* Object fields */
    private var _rotate = floatArrayOf(0f, 0f, 0f)
    private var _position = floatArrayOf(0f, 0f, 0f)
    private var _scale = floatArrayOf(1f, 1f, 1f)
    private var _lightDirection = floatArrayOf(0f, 0f, 0f)

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

    init {
        setColor(0.8f, 0.8f, 0.9f)
    }

    private fun setupGraphics(width: Int, height: Int): Boolean {
        programId = ShaderUtils.createProgram(vertexShader, fragmentShader)
        if (programId == 0) {
            Log.e(tag, "Could not create program")
            return false
        }

        if (texture != null) {
            textureId = TextureUtils.loadTexture(texture)
        }

        vertexLocation = glGetAttribLocation(programId, "vertexPosition")
        vertexColorLocation = glGetAttribLocation(programId, "vertexColor")
        vertexAlphaLocation = glGetUniformLocation(programId, "vertexAlpha")
        textureCordLocation = glGetAttribLocation(programId, "vertexTextureCord")
        vertexNormalLocation = glGetAttribLocation(programId, "vertexNormal")
        lightDirection = glGetUniformLocation(programId, "lightDirection")
        projectionLocation = glGetUniformLocation(programId, "projection")
        modelViewLocation = glGetUniformLocation(programId, "modelView")

        samplerLocation = glGetUniformLocation(programId, "texture")

        MatrixUtils.matrixPerspective(
            projectionMatrix,
            45f,
            width.toFloat() / height.toFloat(),
            0.1f,
            100f
        )

        glViewport(0, 0, width, height)

        return textureId != 0
    }

    override fun setColor(r: Float, g: Float, b: Float, a: Float) {
        val color = FloatArray(colorBuffer.capacity()) { 0f }
        for (i in 0 until color.indices.count() step 3) {
            color[i] = r
            color[i + 1] = g
            color[i + 2] = b
        }
        colorBuffer = Utils.createBuffer(color)
        alpha = a
    }

    override fun setPosition(x: Float, y: Float, z: Float) {
        this.x = x
        this.y = y
        this.z = z
    }

    override fun setRotation(x: Float, y: Float, z: Float) {
        this.rotateX = x
        this.rotateY = y
        this.rotateZ = z
    }

    override fun setScale(x: Float, y: Float, z: Float) {
        this.scaleX = x
        this.scaleY = y
        this.scaleZ = z
    }

    override fun setLightDirection(x: Float, y: Float, z: Float) {
        this._lightDirection[0] = x
        this._lightDirection[1] = y
        this._lightDirection[2] = z
    }

    override fun onSurfaceCreated(gl: GL10, config: EGLConfig) {
    }

    override fun onSurfaceChanged(gl: GL10, width: Int, height: Int) {
        setupGraphics(width, height)
    }

    override fun onDrawFrame(gl: GL10) {
        /* Matrix setup */
        MatrixUtils.matrixIdentityFunction(modelViewMatrix)

        MatrixUtils.matrixRotateX(modelViewMatrix, rotateX)
        MatrixUtils.matrixRotateY(modelViewMatrix, rotateY)
        MatrixUtils.matrixRotateZ(modelViewMatrix, rotateZ)

        MatrixUtils.matrixScale(modelViewMatrix, scaleX, scaleY, scaleZ)

        MatrixUtils.matrixTranslate(modelViewMatrix, x, y, z)

        /* Shaders program */
        glUseProgram(programId)

        /* Position */
        glVertexAttribPointer(vertexLocation, 3, GL_FLOAT, false, 0, verticesBuffer)
        glEnableVertexAttribArray(vertexLocation)

        /* Color */
        glVertexAttribPointer(vertexColorLocation, 3, GL_FLOAT, false, 0, colorBuffer)
        glEnableVertexAttribArray(vertexColorLocation)

        glUniform1f(vertexAlphaLocation, alpha)

        glVertexAttribPointer(textureCordLocation, 2, GL_FLOAT, false, 0, texCordsBuffer)
        glEnableVertexAttribArray(textureCordLocation)

        /* Normals */
        if (normalBuffer.capacity() > 0) {
            glVertexAttribPointer(vertexNormalLocation, 3, GL_FLOAT, false, 0, normalBuffer)
            glEnableVertexAttribArray(vertexNormalLocation)
        }

        /* Light */
        glUniform3fv(lightDirection, 1, _lightDirection, 0)

        /* Texture */
        glActiveTexture(GL_TEXTURE0)
        glBindTexture(GL_TEXTURE_2D, textureId)
        glUniform1i(samplerLocation, 0)

        /* Matrix */
        glUniformMatrix4fv(projectionLocation, 1, false, projectionMatrix, 0)
        glUniformMatrix4fv(modelViewLocation, 1, false, modelViewMatrix, 0)

        /* Drawing */
        glDrawElements(GL_TRIANGLES, indicesBuffer.capacity(), GL_UNSIGNED_INT, indicesBuffer)
    }
}