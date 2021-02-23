#version 330 core
struct Material {
    sampler2D diffuse;
    sampler2D specular;
    sampler2D emission;
    float shininess;
};

struct Light {
    vec3 position;
    
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    
    float constant;
    float linear;
    float quadratic;
    
    vec3 direction;  // spotDir
    float cutOff;
    float outerCutOff;
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
    vec3 lightDir = normalize(light.position - FragPos);
    float theta = dot(lightDir, normalize(-light.direction));
    
    // calculate I using inner cone and outer cone
    float epsilon = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
    
    if (theta > light.outerCutOff)
    {
        // 环境光
        vec3 ambient  = light.ambient * texture(material.diffuse, TexCoords).rgb;
        // 漫反射
        vec3 norm = normalize(Normal);
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse  = light.diffuse * diff * texture(material.diffuse, TexCoords).rgb;
        // 镜面光
        vec3 viewDir = normalize(viewPos - FragPos);
        vec3 reflectDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
        vec3 specular = light.specular * spec * texture(material.specular, TexCoords).rgb;
        // attenuation
        float d    = length(light.position - FragPos);
        float attenuation = 1.0 / (light.constant + light.linear * d +
                        light.quadratic * (d * d));
        //ambient  *= attenuation;
        diffuse = diffuse * attenuation * intensity;
        specular = specular * attenuation * intensity;
        
        vec3 result = ambient + diffuse + specular;
        FragColor = vec4(result, 1.0);
    }
    else
    {
        FragColor = vec4(light.ambient * vec3(texture(material.diffuse, TexCoords)), 1.0);
    }
}
