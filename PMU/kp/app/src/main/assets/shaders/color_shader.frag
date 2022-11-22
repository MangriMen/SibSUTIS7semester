#version 100
precision mediump float;

uniform sampler2D texture;

varying vec3 fragColor;
varying float fragAlpha;
varying vec2 fragTextureCord;

void main() {
    gl_FragColor = vec4(fragColor, fragAlpha);
}