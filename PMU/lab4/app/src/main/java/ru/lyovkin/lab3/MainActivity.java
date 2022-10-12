package ru.lyovkin.lab4;

import androidx.appcompat.app.AppCompatActivity;
import android.opengl.GLSurfaceView;
import android.os.Bundle;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        GLSurfaceView GLView = new GLSurfaceView(this);
        GLView.setEGLContextClientVersion(2);
        GLView.setRenderer(new MyRenderEngine());
        setContentView(GLView);
    }
}