#version 330 core
struct Material {
    sampler2D diffuse;
    sampler2D specular;
    sampler2D emission;
    float shininess;
};

struct Light {
    vec3 direction;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Material material;
uniform Light light;
uniform vec3 viewPos;
uniform float matrixMove;
uniform float matrixLight;

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;

out vec4 FragColor;

void main()
{
    // 环境光
    vec3 ambient  = light.ambient * vec3(texture(material.diffuse, TexCoords).rgb);
    // 漫反射
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(-light.direction);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse  = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords).rgb);
    
    // 镜面光
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec * (vec3(1.0) - vec3(texture(material.specular, TexCoords)));
    // emission
    vec3 emission = vec3(matrixLight) * texture(material.emission, vec2(TexCoords.x, TexCoords.y + matrixMove)).rgb;
    vec3 result = ambient + diffuse + specular + emission;
    FragColor = vec4(result, 1.0);
}
