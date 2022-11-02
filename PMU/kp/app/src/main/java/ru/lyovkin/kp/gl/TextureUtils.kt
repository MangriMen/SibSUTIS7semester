package ru.lyovkin.kp.gl

import android.content.Context
import android.opengl.GLES20.*
import ru.lyovkin.kp.Utils

class TextureUtils {
    companion object {
        fun loadTexture(): Int {
            /* Texture Object Handle. */
            val textureId = IntArray(1)

            /* 3 x 3 Image,  R G B A Channels RAW Format. */
            val pixels = Utils.createBuffer(intArrayOf(
                    18,  140, 171, 255, /* Some Colour Bottom Left. */
                    143, 143, 143, 255, /* Some Colour Bottom Middle. */
                    255, 255, 255, 255, /* Some Colour Bottom Right. */
                    255, 255, 0,   255, /* Yellow Middle Left. */
                    0,   255, 255, 255, /* Some Colour Middle. */
                    255, 0,   255, 255, /* Some Colour Middle Right. */
                    255, 0,   0,   255, /* Red Top Left. */
                    0,   255, 0,   255, /* Green Top Middle. */
                    0,   0,   255, 255, /* Blue Top Right. */
            ))

            /* Use tightly packed data. */
            glPixelStorei(GL_UNPACK_ALIGNMENT, 1)
            /* Generate a texture object. */
            glGenTextures(1, textureId, 0)
            /* Activate a texture. */
            glActiveTexture(GL_TEXTURE0)
            /* Bind the texture object. */
            glBindTexture(GL_TEXTURE_2D, textureId[0])
            /* Load the texture. */
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, 3, 3, 0, GL_RGBA, GL_UNSIGNED_BYTE, pixels)
            /* Set the filtering mode. */
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST)

            return textureId[0]
        }
    }
}