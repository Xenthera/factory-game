shader_type spatial;

uniform vec2 grid_scale = vec2(1.0, 1.0);
uniform vec3 camera_position = vec3(0.0, 0.0, 0.0);

uniform float roughness;

uniform sampler2D tex : source_color;

void vertex() {
	// Called for every vertex the material is visible on.

}

void fragment() {
	// Called for every pixel the material is visible on.
	// Adjust UV based on the camera's position
    vec2 uv = UV * grid_scale + vec2(camera_position.x, camera_position.z);
    uv = fract(uv); // Keep UV values in the range [0, 1]

    // Sample the texture
    vec4 tex_color = texture(tex, uv);
    ALBEDO = tex_color.rgb;

	ROUGHNESS = 1.0;
	METALLIC = 0.0;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
