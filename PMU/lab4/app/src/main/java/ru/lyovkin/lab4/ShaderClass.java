package ru.lyovkin.lab4;

import static android.opengl.GLES20.GL_COMPILE_STATUS;
import static android.opengl.GLES20.GL_FRAGMENT_SHADER;
import static android.opengl.GLES20.GL_LINK_STATUS;
import static android.opengl.GLES20.GL_VERTEX_SHADER;
import static android.opengl.GLES20.glAttachShader;
import static android.opengl.GLES20.glCompileShader;
import static android.opengl.GLES20.glCreateProgram;
import static android.opengl.GLES20.glCreateShader;
import static android.opengl.GLES20.glDeleteProgram;
import static android.opengl.GLES20.glDeleteShader;
import static android.opengl.GLES20.glGetProgramiv;
import static android.opengl.GLES20.glGetShaderiv;
import static android.opengl.GLES20.glLinkProgram;
import static android.opengl.GLES20.glShaderSource;

public class ShaderClass {
    public static int ShaderInitialize(int Vertex, int Fragment) {
        final int initialize_id = glCreateProgram();
        glAttachShader(initialize_id, Vertex);
        glAttachShader(initialize_id, Fragment);
        glLinkProgram(initialize_id);

        final int[] isShaderLinked = new int[1];
        glGetProgramiv(initialize_id, GL_LINK_STATUS, isShaderLinked, 0);

        if (isShaderLinked[0] == 0) {
            glDeleteProgram(initialize_id);
            return -1;
        }

        return initialize_id;
    }

    static int ShaderHandler(int TYPE, String shader) {
        final int shader_id = glCreateShader(TYPE);

        if (shader_id == 0) {
            return -1;
        }

        glShaderSource(shader_id, shader);
        glCompileShader(shader_id);
        final int[] isShaderOkay = new int[1];

        glGetShaderiv(shader_id, GL_COMPILE_STATUS, isShaderOkay, 0);
        if (isShaderOkay[0] == 0) {
            glDeleteShader(shader_id);

            return -1;
        }

        return shader_id;
    }

    public static int CreateShaders() {
        String Vertex = "uniform mat4 uMVPMatrix, uMVMatrix, uNormalMat;" +
                "attribute vec4 vPosition;" +
                "attribute vec4 vColor;" +
                "attribute vec3 vNormal;" +
                "varying vec4 varyingColor;" +
                "varying vec3 varyingNormal;" +
                "varying vec3 varyingPos;" +
                "void main()" +
                "{" +
                "varyingColor = vColor;" +
                "varyingNormal= vec3(uNormalMat * vec4(vNormal, 0.0));" +
                "varyingPos = vec3(uMVMatrix * vPosition);" +
                "gl_Position = uMVPMatrix * vPosition;" +
                "}";
        String Fragment = "precision mediump float;" +
                "varying vec4 varyingColor; varying vec3 varyingNormal;" +
                "varying vec3 varyingPos;" +
                "uniform vec3 lightDir;" +
                "void main()" +
                "{" +
                "float Ns = 13.0;" +
                "float kd = 0.7, ks = 1.0;" +
                "vec4 light = vec4(1.0, 1.0, 1.0, 1.0);" +
                "vec4 lightS = vec4(1.0, 1.0, 0.2, 1.0);" +
                "vec3 Nn = normalize(varyingNormal);" +
                "vec3 Ln = normalize(lightDir);" +
                "vec4 diffuse = kd * light * max(dot(Nn, Ln), 0.0);" +
                "vec3 Ref = reflect(Nn, Ln);" +
                "float spec = pow(max(dot(Ref, normalize(varyingPos)), 0.0), Ns);" +
                "vec4 specular = lightS * ks * spec;" +
                "gl_FragColor = varyingColor * diffuse + specular;" +
                "}";

        int VertexShader = ShaderHandler(GL_VERTEX_SHADER, Vertex);
        int FragmentShader = ShaderHandler(GL_FRAGMENT_SHADER, Fragment);

        return ShaderInitialize(VertexShader, FragmentShader);
    }
}
