#version 100
precision mediump float;

uniform sampler2D texture;

varying vec3 fragColour;
varying float fragAlpha;
varying vec2 fragTextureCord;

void main() {
    gl_FragColor = texture2D(texture, fragTextureCord);
}