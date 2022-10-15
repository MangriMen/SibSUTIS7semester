package ru.lyovkin.lab4;

import javax.microedition.khronos.egl.EGLConfig;
import javax.microedition.khronos.opengles.GL10;

import android.opengl.GLES20;
import android.opengl.GLSurfaceView;
import android.opengl.Matrix;
import android.os.SystemClock;

public class MyRenderEngine implements GLSurfaceView.Renderer
{
    private MySphere sphere;

    float[] vm_matrix = new float[16];
    float[] mvp_matrix = new float[16];
    float[] pvm_matrix = new float[16];
    float[] invert_matrix = new float[16];
    float[] normal_matrix = new float[16];
    float[] rotation_x_matrix = new float[16];
    float[] rotation_y_matrix = new float[16];

    private final static long TIME = 5000;

    private final float[] projection_matrix = new float[16];

    @Override
    public void onSurfaceCreated(GL10 unused, EGLConfig cfg)
    {
        sphere = new MySphere(90, 600);

        GLES20.glClearColor(155, 100, 90, 1.0f);
        GLES20.glEnable(GL10.GL_DEPTH_TEST);
        GLES20.glDepthFunc(GL10.GL_LEQUAL);
    }

    @Override
    public void onSurfaceChanged(GL10 unused, int width, int height)
    {
        GLES20.glViewport(0, 0, width, height);
        Matrix.frustumM(projection_matrix, 0, -(float) width / height, (float) width / height, -1, 1, 1, 20);
    }

    @Override
    public void onDrawFrame(GL10 unused)
    {
        float[] scale_matrix = new float[16];
        float[] final_matrix = new float[16];
        float[] temporary_matrix = new float[16];

        float angle = (float) (Math.cos((float)(SystemClock.uptimeMillis() % TIME) / TIME  *  2 * Math.PI) * 4f);

        GLES20.glClear(GLES20.GL_COLOR_BUFFER_BIT|GLES20.GL_DEPTH_BUFFER_BIT);
        GLES20.glClearColor(1.0f, 0.5f, 0.5f, 0.5f);

        Matrix.setLookAtM(vm_matrix, 0, -1, 0, -1f, -1f, 0f, 0f, 25.0f, 5.0f, 25.0f);
        Matrix.multiplyMM(pvm_matrix, 0, projection_matrix, 0, vm_matrix, 0);

        Matrix.setRotateM(rotation_x_matrix,  0,  0,  5.0f,  5.0f,  0f);
        Matrix.setRotateM(rotation_y_matrix, 0,0, 5.0f, 5.0f, 0);

        Matrix.multiplyMM(temporary_matrix,  0, pvm_matrix, 0, rotation_x_matrix,  0);
        Matrix.multiplyMM(mvp_matrix, 0, temporary_matrix, 0, rotation_y_matrix, 0);

        Matrix.multiplyMM(temporary_matrix, 0, vm_matrix, 0, rotation_x_matrix, 0);
        Matrix.multiplyMM(invert_matrix, 0, temporary_matrix, 0, rotation_y_matrix, 0);

        Matrix.invertM(temporary_matrix, 0, invert_matrix, 0);
        Matrix.transposeM(normal_matrix, 0, temporary_matrix, 0);

        Matrix.setLookAtM(vm_matrix, 0, angle, 1, angle, 0, 0, 0, 1, 1, 1);

        Matrix.setIdentityM(scale_matrix, 0);
        Matrix.scaleM(scale_matrix, 0, 1.5f, 1.5f, 0.5f);
        Matrix.multiplyMM(temporary_matrix, 0, mvp_matrix, 0, scale_matrix, 0);

        Matrix.translateM(final_matrix, 0, temporary_matrix, 0, -1.7f, 0f, 5f);

        sphere.draw(final_matrix, normal_matrix, vm_matrix);
    }
}