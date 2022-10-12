package ru.lyovkin.lab5;


import android.content.Context;
import android.opengl.GLSurfaceView;

public class MyClassSurfaceView extends GLSurfaceView {
    private WaterRenderer renderer;

    public MyClassSurfaceView(Context context) {
        super(context);
        setEGLContextClientVersion(2);
        renderer = new WaterRenderer(context);
        setRenderer(renderer);
        setRenderMode(GLSurfaceView.RENDERMODE_CONTINUOUSLY);
    }
}
