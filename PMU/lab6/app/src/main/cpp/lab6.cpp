#include <jni.h>
#include <string>
#include <GLES2/gl2.h>
#include <GLES/gl.h>
#include <GLES3/gl3.h>

extern "C"
JNIEXPORT void JNICALL
Java_ru_lyovkin_lab6_JNIWrapper_onsurfacecreated(JNIEnv *env, jobject cls) {
    glClearColor(0.5f, 0.5f, 1.0f, 0.0f);
    glMatrixMode(GL_PROJECTION);
    glOrthof(-8, 8, -8, 8, -8, 8);
    glEnable(GL_DEPTH_TEST); // подключение теста глубины
    glMatrixMode(GL_MODELVIEW);
}

extern "C"
JNIEXPORT void JNICALL
Java_ru_lyovkin_lab6_JNIWrapper_onsurfacechanged(JNIEnv *env, jobject cls, jint width,
                                                 jint height) {
}

GLfloat a[12] = {
        -1, 1, 0,
        -1, -1, 0,
        1, -1, 0,
        1, 1, 0
};

GLfloat texCoords[8] = { // Texture coords for the above face (NEW)
        0.0f, 1.0f,  // A. left-bottom (NEW)
        1.0f, 1.0f,  // B. right-bottom (NEW)
        0.0f, 0.0f,  // C. left-top (NEW)
        1.0f, 0.0f   // D. right-top (NEW)
};

int angle = 0;

extern "C"
JNIEXPORT void JNICALL Java_ru_lyovkin_lab6_JNIWrapper_ondrawframe(JNIEnv *env, jobject cls) {
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
    glLoadIdentity();
    glScalef(2, 1, 2);

    glEnableClientState(GL_VERTEX_ARRAY);
    glEnableClientState(GL_TEXTURE_COORD_ARRAY);
    glEnable(GL_TEXTURE_2D);

    angle = (angle == 360) ? 0 : angle + 2;
    glRotatef(angle, 1, 1, 0);
    //лицевая грань
    glPushMatrix();
    glVertexPointer(3, GL_FLOAT, 0, a);
    glTexCoordPointer(2, GL_FLOAT, 0, texCoords);
    glDrawArrays(GL_TRIANGLE_FAN, 0, 4);
//    glColor4f(
//            1.0f,
//            0.0f,
//            0.0f,
//            1.0f
//    );
    glPopMatrix();


    //задняя
    glPushMatrix();
    glTranslatef(0, 0, -2);
//    glVertexPointer(3, GL_FLOAT, 0, a);
    glTexCoordPointer(2, GL_FLOAT, 0, texCoords);
    glDrawArrays(GL_TRIANGLE_FAN, 0, 4);
    glPopMatrix();
//верхняя
    glPushMatrix();
    glTranslatef(0, 1, -1);
    glRotatef(90, 1, 0, 0);
    glVertexPointer(3, GL_FLOAT, 0, a);
    glTexCoordPointer(2, GL_FLOAT, 0, texCoords);
    glDrawArrays(GL_TRIANGLE_FAN, 0, 4);
    glPopMatrix();

//нижняя
    glPushMatrix();
    glRotatef(90, 1, 0, 0);
    glTranslatef(0, -1, 1);
    glVertexPointer(3, GL_FLOAT, 0, a);
    glTexCoordPointer(2, GL_FLOAT, 0, texCoords);
    glDrawArrays(GL_TRIANGLE_FAN, 0, 4);
    glPopMatrix();
//левая

    glPushMatrix();
    glRotatef(90, 0, 1, 0);
    glTranslatef(1, 0, -1);
    glVertexPointer(3, GL_FLOAT, 0, a);
    glTexCoordPointer(2, GL_FLOAT, 0, texCoords);
    glDrawArrays(GL_TRIANGLE_FAN, 0, 4);
    glPopMatrix();

//правая
    glPushMatrix();
    glRotatef(90, 0, 1, 0);
    glTranslatef(1, 0, 1);
    glVertexPointer(3, GL_FLOAT, 0, a);
    glTexCoordPointer(2, GL_FLOAT, 0, texCoords);
    glDrawArrays(GL_TRIANGLE_FAN, 0, 4);
    glPopMatrix();

    glDisable(GL_TEXTURE_2D);
    glDisableClientState(GL_TEXTURE_COORD_ARRAY);

    glDisableClientState(GL_VERTEX_ARRAY);
}

// Write C++ code here.
//
// Do not forget to dynamically load the C++ library into your application.
//
// For instance,
//
// In MainActivity.java:
//    static {
//       System.loadLibrary("lab6");
//    }
//
// Or, in MainActivity.kt:
//    companion object {
//      init {
//         System.loadLibrary("lab6")
//      }
//    }