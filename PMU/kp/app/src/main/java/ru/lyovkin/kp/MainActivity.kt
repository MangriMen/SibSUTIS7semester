package ru.lyovkin.kp

import android.app.ActivityManager
import android.opengl.GLSurfaceView
import android.os.Build
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Toast

class MainActivity : AppCompatActivity() {
    private  val isProbablyEmulator: Boolean
        get() = (Build.FINGERPRINT.startsWith("generic")
                || Build.FINGERPRINT.startsWith("unknown")
                || Build.MODEL.contains("google_sdk")
                || Build.MODEL.contains("Emulator")
                || Build.MODEL.contains("Android SDK built for x86"))
    private var glSurfaceView: GLSurfaceView? = null
    private var isRendererSet = false

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        val activityManager = getSystemService(ACTIVITY_SERVICE) as ActivityManager
        val configurationInfo = activityManager.deviceConfigurationInfo

        val isSupportES2 = configurationInfo.reqGlEsVersion >= 0x20000 || isProbablyEmulator

        if (!isSupportES2) {
            Toast.makeText(
                this,
                "This device does not support OpenGL ES 2.0",
                Toast.LENGTH_LONG
            ).show()
            return
        }

        glSurfaceView = GLSurfaceView(this)

        if (isProbablyEmulator) {
            glSurfaceView?.setEGLConfigChooser(
                8, 8, 8,
                8, 16, 0
            )
        }
        glSurfaceView?.setEGLContextClientVersion(2)
        glSurfaceView?.setRenderer(MyRenderer(this))
        glSurfaceView?.renderMode = GLSurfaceView.RENDERMODE_CONTINUOUSLY
        isRendererSet = true

        setContentView(glSurfaceView)
    }

    override fun onPause() {
        super.onPause()
        if (isRendererSet) {
            glSurfaceView?.onPause()
        }
    }

    override fun onResume() {
        super.onResume()
        if (isRendererSet) {
            glSurfaceView?.onResume()
        }
    }
}