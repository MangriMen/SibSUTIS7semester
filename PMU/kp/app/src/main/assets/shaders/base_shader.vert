attribute vec4 vertexPosition;
attribute vec3 vertexColor;
uniform float vertexAlpha;
attribute vec2 vertexTextureCord;
attribute vec3 vertexNormal;
uniform vec3 lightDirection;

varying vec3 fragColor;
varying float fragAlpha;
varying vec2 fragTextureCord;

uniform mat4 projection;
uniform mat4 modelView;

void main() {
    /* [Setup scene vectors.] */
    vec3 transformedVertexNormal = normalize((modelView * vec4(vertexNormal, 0.0)).xyz);
    vec3 inverseLightDirection = normalize(lightDirection);
    fragColor = vec3(0.0);
    /* [Setup scene vectors.] */

    /* [Calculate the diffuse component.] */
    vec3 diffuseLightIntensity = vec3(0.2, 0.2, 0.2);
    vec3 vertexDiffuseReflectionConstant = vertexColor;
    float normalDotLight = max(0.0, dot(transformedVertexNormal, inverseLightDirection));
    fragColor += normalDotLight * vertexDiffuseReflectionConstant * diffuseLightIntensity;
    /* [Calculate the diffuse component.] */

    /* [Calculate the ambient component.] */
    vec3 ambientLightIntensity = vec3(0.1, 0.1, 0.1);
    vec3 vertexAmbientReflectionConstant = vertexColor;
    fragColor += vertexAmbientReflectionConstant * ambientLightIntensity;
    /* [Calculate the ambient component.] */

    /* [Calculate the specular component.] */
    vec3 inverseEyeDirection = normalize(vec3(0.0, 0.0, 1.0));
    vec3 specularLightIntensity = vec3(1.0, 1.0, 1.0);
    vec3 vertexSpecularReflectionConstant = vec3(1.0, 1.0, 1.0);
    float shininess = 2.0;
    vec3 lightReflectionDirection = reflect(vec3(0) - inverseLightDirection, transformedVertexNormal);
    float normalDotReflection = max(0.0, dot(inverseEyeDirection, lightReflectionDirection));
    fragColor += pow(normalDotReflection, shininess) * vertexSpecularReflectionConstant * specularLightIntensity;
    /* [Calculate the specular component.] */

    /* Make sure the fragment colour is between 0 and 1. */
    clamp(fragColor, 0.0, 1.0);

    gl_Position = projection * modelView * vertexPosition;

    fragAlpha = vertexAlpha;
    fragTextureCord = vertexTextureCord;
}
