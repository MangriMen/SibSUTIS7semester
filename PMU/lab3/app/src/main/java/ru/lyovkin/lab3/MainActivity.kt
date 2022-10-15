package ru.lyovkin.lab3

import android.opengl.GLSurfaceView
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import ru.lyovkin.lab3.entity.SolarSystem

class MainActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        val view = GLSurfaceView(this)
        view.setRenderer(SolarSystem(this))
        setContentView(view)
    }
}