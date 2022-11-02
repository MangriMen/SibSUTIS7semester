attribute vec4 vertexPosition;
attribute vec3 vertexColour;
uniform float vertexAlpha;
attribute vec2 vertexTextureCord;

varying vec3 fragColour;
varying float fragAlpha;
varying vec2 fragTextureCord;

uniform mat4 projection;
uniform mat4 modelView;

void main() {
    gl_Position = projection * modelView * vertexPosition;

    fragColour = vertexColour;
    fragAlpha = vertexAlpha;
    fragTextureCord = vertexTextureCord;
}
