package ru.lyovkin.kp

import android.content.Context
import android.opengl.GLES20.*
import android.opengl.GLSurfaceView
import ru.lyovkin.kp.figures.Object
import ru.lyovkin.kp.gl.GLObject
import java.nio.charset.StandardCharsets
import java.util.stream.Collectors
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10


class MyRenderer(ctx: Context) : GLSurfaceView.Renderer {
    private val objects: MutableList<GLObject> = mutableListOf()
    private val bgColorR: Float = 0.8f
    private val bgColorG: Float = 1.0f
    private val bgColorB: Float = 0.1f
    private val bgColorA: Float = 1.0f

    init {
        val vertexShader = Utils.inputStreamToString(ctx.assets.open("shaders/default_shader.vert"))
        val colorShader = Utils.inputStreamToString(ctx.assets.open("shaders/color_shader.frag"))
//        val textureShader = Utils.inputStreamToString(ctx.assets.open("shaders/texture_shader.frag"))

        val table = Object.fromInputStream(ctx.assets.open("models/table.obj"), vertexShader, colorShader)
        table.z = -10f
        table.y = -1.5f
        table.scaleX = 0.02f
        table.scaleY = 0.02f
        table.scaleZ = 0.02f

        val glass = Object.fromInputStream(ctx.assets.open("models/glass.obj"), vertexShader, colorShader)
        glass.y = -0.5f
        glass.z = -10f
        glass.scaleX = 0.1f
        glass.scaleY = 0.1f
        glass.scaleZ = 0.1f
        glass.setColor(1f, 1f, 1f, 0.2f)

        val apple = Object.fromInputStream(ctx.assets.open("models/apple.obj"), vertexShader, colorShader)
        apple.x = 0.7f
        apple.y = -0.4f
        apple.z = -10f
        apple.scaleX = 0.5f
        apple.scaleY = 0.5f
        apple.scaleZ = 0.5f
        apple.setColor(1f, 0f, 0f)

        val pear = Object.fromInputStream(ctx.assets.open("models/pear.obj"), vertexShader, colorShader)
        pear.x = 1.2f
        pear.y = -0.1f
        pear.z = -10f
        pear.scaleX = 0.05f
        pear.scaleY = 0.05f
        pear.scaleZ = 0.05f
        pear.rotateX = -45f
        pear.setColor(0.5f, 1f, 0.8f)

        val banana = Object.fromInputStream(ctx.assets.open("models/banana.obj"), vertexShader, colorShader)
        banana.x = -0.7f
        banana.y = -0.4f
        banana.z = -10f
        banana.scaleX = 0.05f
        banana.scaleY = 0.05f
        banana.scaleZ = 0.05f
        banana.setColor(0f, 1f, 0f)

        val pineapple = Object.fromInputStream(ctx.assets.open("models/pineapple.obj"), vertexShader, colorShader)
        pineapple.x = -1.2f
        pineapple.y = -0.3f
        pineapple.z = -10f
        pineapple.scaleX = 0.04f
        pineapple.scaleY = 0.04f
        pineapple.scaleZ = 0.04f
        pineapple.setColor(0.7f, 0.8f, 0f)

        objects.add(glass)
        objects.add(apple)
        objects.add(pear)
        objects.add(banana)
        objects.add(pineapple)
        objects.add(table)
    }

    override fun onSurfaceCreated(gl: GL10, config: EGLConfig) {
        glEnable(GL_DEPTH_TEST)
        glDepthFunc(GL_LEQUAL)

        glEnable(GL_BLEND)
        glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA)

        for (obj in objects) {
            obj.onSurfaceCreated(gl, config)
        }
    }

    override fun onSurfaceChanged(gl: GL10, width: Int, height: Int) {
        glViewport(0, 0, width, height)

        for (obj in objects) {
            obj.onSurfaceChanged(gl, width, height)
        }
    }

    override fun onDrawFrame(gl: GL10) {
//        glClearColor(bgColorR, bgColorG, bgColorB, bgColorA)
        glClear(GL_COLOR_BUFFER_BIT or GL_DEPTH_BUFFER_BIT)

        for (obj in objects) {
            (obj as Object).rotateY += 1f
            obj.onDrawFrame(gl)
        }
    }
}