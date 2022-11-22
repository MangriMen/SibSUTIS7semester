#version 100
precision mediump float;

uniform sampler2D texture;

varying vec3 fragColor;
varying float fragAlpha;
varying vec2 fragTextureCord;

void main() {
    vec4 calculatedColor = texture2D(texture, fragTextureCord) * vec4(vec3(fragColor), fragAlpha);
    clamp(calculatedColor, 0.0, 1.0);
    gl_FragColor = calculatedColor;
}