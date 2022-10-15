package ru.lyovkin.lab6

object JNIWrapper {
    external fun onsurfacecreated()
    external fun onsurfacechanged(width: Int, height: Int)
    external fun ondrawframe()

    init {
        System.loadLibrary("lab6")
    }
}