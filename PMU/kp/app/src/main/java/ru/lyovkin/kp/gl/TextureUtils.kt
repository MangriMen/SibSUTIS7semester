package ru.lyovkin.kp.gl

import android.graphics.BitmapFactory
import android.graphics.Rect
import android.opengl.GLES20.*
import android.opengl.GLUtils
import java.io.InputStream


class TextureUtils {
    companion object {
        fun loadTexture(texture: InputStream): Int {
            val textureHandle = IntArray(1)
            glGenTextures(1, textureHandle, 0)
            if (textureHandle[0] != 0) {
                val options = BitmapFactory.Options()
                options.inScaled = false // No pre-scaling

                // Read in the resource
                val bitmap = BitmapFactory.decodeStream(texture, Rect(0, 0, 0, 0), options)

                // Bind to the texture in OpenGL
                glBindTexture(GL_TEXTURE_2D, textureHandle[0])

                // Set filtering
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST)
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST)

                // Load the bitmap into the bound texture.
                GLUtils.texImage2D(GL_TEXTURE_2D, 0, bitmap, 0)

                // Recycle the bitmap, since its data has been loaded into OpenGL.
                bitmap?.recycle()
            }

            if (textureHandle[0] == 0) {
                throw RuntimeException("Error loading texture.")
            }

            return textureHandle[0]
        }
    }
}