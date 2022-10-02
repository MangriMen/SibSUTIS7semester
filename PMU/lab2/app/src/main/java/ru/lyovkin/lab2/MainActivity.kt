package ru.lyovkin.lab2

import android.opengl.GLSurfaceView
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.WindowManager

class MainActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
//        setContentView(R.layout.activity_main)
        window.addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON)

        val g = GLSurfaceView(this)
        g.setRenderer(Sphere())
        g.renderMode = GLSurfaceView.RENDERMODE_CONTINUOUSLY

        setContentView(g)
    }
}