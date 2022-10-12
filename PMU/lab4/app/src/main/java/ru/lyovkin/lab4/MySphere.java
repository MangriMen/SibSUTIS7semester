package ru.lyovkin.lab4;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.FloatBuffer;
import java.nio.ShortBuffer;
import android.opengl.GLES20;

public class MySphere {

    private final FloatBuffer VertexBuffer;
    private final FloatBuffer ColorBuffer;
    private final FloatBuffer NormalsBuffer;

    private final int MyApplication;

    static final int COORDS_COUNT_PER_VERTEX = 3;

    private float [] Vertices;
    private float [] Normals;
    private float [] Colors;

    private int VERTEX_COUNT;

    float[] LightDirection = {5f, 10f, 35.0f};

    private void CreateSphere(int LATITUDES, int LONGITUDES) {
        Vertices = new float[LATITUDES * LONGITUDES * 6 * 3];
        Normals = new float[LATITUDES * LONGITUDES * 6 * 3];
        Colors = new float[LATITUDES * LONGITUDES * 6 * 3];

        int TriangleIndex = 0;

        VERTEX_COUNT = Vertices.length/COORDS_COUNT_PER_VERTEX;

        for(int i = 0; i< LATITUDES; ++i) {

            double LatitudeFirst = Math.PI * (-0.5 + (double)i / LATITUDES);
            double Z_SizeFirst = Math.sin(LatitudeFirst);
            double Zr_SizeFirst = Math.cos(LatitudeFirst);

            double LatitudeSecond = Math.PI * (-0.5 + (double)(i+1)/LATITUDES);
            double Z_SizeSecond = Math.sin(LatitudeSecond);
            double Zr_SizeSecond = Math.cos(LatitudeSecond);

            for (int j = 0; j< LONGITUDES; ++j) {
                double Longitude = 2 * Math.PI * (double)(j-1)/LONGITUDES;
                double X_SizeFirst = Math.cos(Longitude);
                double Y_SizeFirst = Math.sin(Longitude);

                Longitude = 2* Math.PI * (double)j/LONGITUDES;
                double X_SizeSecond = Math.cos(Longitude);
                double Y_SizeSecond = Math.sin(Longitude);

                Vertices[TriangleIndex* 9] = (float)(X_SizeFirst * Zr_SizeFirst);
                Vertices[TriangleIndex* 9 + 1] = (float)(Y_SizeFirst * Zr_SizeFirst);
                Vertices[TriangleIndex* 9 + 2] = (float)(Z_SizeFirst);
                Vertices[TriangleIndex* 9 + 3] = (float)(X_SizeFirst * Zr_SizeSecond);
                Vertices[TriangleIndex* 9 + 4] = (float)(Y_SizeFirst * Zr_SizeSecond);
                Vertices[TriangleIndex* 9 + 5] = (float)(Z_SizeSecond);
                Vertices[TriangleIndex* 9 + 6] = (float)(X_SizeSecond * Zr_SizeFirst);
                Vertices[TriangleIndex* 9 + 7] = (float)(Y_SizeSecond * Zr_SizeFirst);
                Vertices[TriangleIndex* 9 + 8] = (float)(Z_SizeFirst);

                TriangleIndex++;

                Vertices[TriangleIndex* 9] = (float)(X_SizeSecond * Zr_SizeFirst);
                Vertices[TriangleIndex* 9 + 1] = (float)(Y_SizeSecond * Zr_SizeFirst);
                Vertices[TriangleIndex* 9 + 2] = (float)(Z_SizeFirst);
                Vertices[TriangleIndex* 9 + 3] = (float)(X_SizeFirst * Zr_SizeSecond);
                Vertices[TriangleIndex* 9 + 4] = (float)(Y_SizeFirst * Zr_SizeSecond);
                Vertices[TriangleIndex* 9 + 5] = (float)(Z_SizeSecond);
                Vertices[TriangleIndex* 9 + 6] = (float)(X_SizeSecond * Zr_SizeSecond);
                Vertices[TriangleIndex* 9 + 7] = (float)(Y_SizeSecond * Zr_SizeSecond);
                Vertices[TriangleIndex* 9 + 8] = (float)(Z_SizeSecond);

                for (int counter = -9; counter<9; counter++) {
                    Normals[TriangleIndex * 9 + counter] = Vertices[TriangleIndex * 9 + counter];
                    if ((TriangleIndex * 9 + counter) % 3 == 1) {
                        Colors[TriangleIndex*9+counter] = 1;
                    } else {
                        Colors[TriangleIndex * 9 + counter] = 0;
                    }
                }
                TriangleIndex++;
            }
        }
    }

    public MySphere(int LATITUDES, int LONGITUDES) {
        CreateSphere(LATITUDES, LONGITUDES);
        ByteBuffer bb = ByteBuffer.allocateDirect(Vertices.length*4);
        bb.order(ByteOrder.nativeOrder());

        VertexBuffer = bb.asFloatBuffer();
        VertexBuffer.put(Vertices);
        VertexBuffer.position(0);

        ByteBuffer bb2 = ByteBuffer.allocateDirect(Colors.length * 4);
        bb2.order(ByteOrder.nativeOrder());
        ColorBuffer = bb2.asFloatBuffer();
        ColorBuffer.put(Colors);
        ColorBuffer.position(0);

        ByteBuffer bb3 = ByteBuffer.allocateDirect(Normals.length * 4);
        bb3.order(ByteOrder.nativeOrder());

        NormalsBuffer = bb3.asFloatBuffer();
        NormalsBuffer.put(Normals);
        NormalsBuffer.position(0);

        short[] drawOrder = {0, 1, 2, 0, 2, 3};
        ByteBuffer dlb = ByteBuffer.allocateDirect(drawOrder.length * 2);
        dlb.order(ByteOrder.nativeOrder());
        ShortBuffer drawListBuffer = dlb.asShortBuffer();
        drawListBuffer.put(drawOrder);
        drawListBuffer.position(0);

        MyApplication = ShaderClass.CreateShaders();
    }

    public void draw(float[] mvpMatrix, float [] normalMat, float [] mvMat)
    {
        GLES20.glUseProgram(MyApplication);
        int mPositionHandle = GLES20.glGetAttribLocation(MyApplication, "vPosition");
        GLES20.glEnableVertexAttribArray(mPositionHandle);
        int vertexStride = COORDS_COUNT_PER_VERTEX * 4;
        GLES20.glVertexAttribPointer(mPositionHandle, COORDS_COUNT_PER_VERTEX, GLES20.GL_FLOAT,
                false, vertexStride, VertexBuffer);

        int light = GLES20.glGetUniformLocation(MyApplication, "lightDir");
        GLES20.glUniform3fv(light, 1, LightDirection, 0);
        int mColorHandle = GLES20.glGetAttribLocation(MyApplication, "vColor");

        GLES20.glEnableVertexAttribArray(mColorHandle);
        GLES20.glVertexAttribPointer(mColorHandle, COORDS_COUNT_PER_VERTEX, GLES20.GL_FLOAT,
                false, vertexStride, ColorBuffer);

        int mNormalHandle = GLES20.glGetAttribLocation(MyApplication, "vNormal");
        GLES20.glEnableVertexAttribArray(mNormalHandle);
        GLES20.glVertexAttribPointer(mNormalHandle, COORDS_COUNT_PER_VERTEX, GLES20.GL_FLOAT,
                false, vertexStride, NormalsBuffer);

        int mMVPMatrixHandle = GLES20.glGetUniformLocation(MyApplication, "uMVPMatrix");
        int mNormalMatHandle = GLES20.glGetUniformLocation(MyApplication, "uNormalMat");

        int MVMatHandle = GLES20.glGetUniformLocation(MyApplication, "uMVMatrix");

        GLES20.glUniformMatrix4fv(mMVPMatrixHandle, 1, false, mvpMatrix, 0);
        GLES20.glUniformMatrix4fv(mNormalMatHandle, 1, false, normalMat, 0);
        GLES20.glUniformMatrix4fv(MVMatHandle, 1, false, mvMat, 0);

        GLES20.glDrawArrays(GLES20.GL_TRIANGLES, 0, VERTEX_COUNT);

        GLES20.glDisableVertexAttribArray(mPositionHandle);
        GLES20.glDisableVertexAttribArray(mColorHandle);
        GLES20.glDisableVertexAttribArray(mNormalHandle);
    }
}
