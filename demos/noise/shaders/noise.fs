uniform sampler2D noise_texture;
uniform float time;

varying vec2 uvs;

struct gradient {
  vec3 start;
  vec3 stop;
};

gradient gradient_new(vec3 start, vec3 stop) {
  gradient g;
  g.start = start;
  g.stop = stop;
  return g;
}

vec3 gradient_lookup(gradient g, float amount) {
  return mix(g.start, g.stop, amount);
}

float perlin_noise(sampler2D noise, vec2 uvs, int octaves) {
  
  vec2 resolution = vec2(1280, 720) / vec2(512, 512);
  
  float total = 0;
  for(int i = 0; i < octaves; i++) {
    float amplitude = pow( 0.5, octaves-i );
    float wavelength = pow(2,i);
    total += amplitude * texture2D( noise, (uvs * resolution) / wavelength ).g;
  }
  
  return total;
}

void main()
{
  vec2 offset = uvs;

  vec2 first, second;

  first.x = perlin_noise(noise_texture, offset + vec2(0.0, 0.0) * vec2(0.101,0.13) * time * 1.1, 5);
  first.y = perlin_noise(noise_texture, offset + vec2(0.1753, 0.1211) * vec2(-0.12,0.11) * time * 1.22, 6);

  second.x = perlin_noise(noise_texture, offset + vec2(0.1111, 0.1222) * vec2(-0.14,-0.09) * time * 1.31, 8);
  second.y = perlin_noise(noise_texture, offset + vec2(0.1652, 0.2111) * vec2(0.16,-0.14) * time * 1.44, 9);
  
  vec2 final = offset + first * 2.1 + second * 3.1;
  
  float total = perlin_noise(noise_texture, final, 10);
  
  gradient g_first, g_second, g_final, g_one, g_two;
  g_first = gradient_new( vec3(206, 210, 219) / 255, vec3(75, 97, 89) / 255);
  g_second = gradient_new( vec3(99, 63, 33) / 255, vec3(237, 197, 128) / 255);
  g_final = gradient_new( vec3(49, 89, 52) / 255, vec3(142, 124, 156) / 255);
  g_one = gradient_new( vec3(218, 232, 235) / 255, vec3(29, 174, 207) / 255);
  g_two = gradient_new( vec3(181, 177, 165) / 255, vec3(209, 183, 98) / 255);
  
  vec3 color = gradient_lookup(g_one, first.x * 1);
  color = mix(color, gradient_lookup(g_two, second.x * 0.5), first.y * 2);
  color = mix(color, gradient_lookup(g_second, second.y * 1), first.x * second.y);
  
  gl_FragColor = vec4(color * 2, 1.0) * mix(pow(total, 2), 0.5, 0.25);
}
