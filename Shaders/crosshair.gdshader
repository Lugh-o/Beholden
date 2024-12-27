shader_type canvas_item;

uniform float radius : hint_range(0.0, 1.0) = 0.1; // Raio do círculo
uniform float thickness : hint_range(0.0, 0.5) = 0.0001; // Espessura da borda
uniform bool normal;

uniform float size : hint_range(0.0, 1.0) = 0.1;
uniform float separation : hint_range(0.0, 1.0) = 0.0;
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

void fragment() {
    vec2 uv = UV;
    vec2 center = vec2(0.5, 0.5);
    vec2 offset = uv - center;

    float dist = length(offset);

	// Calcula as distâncias em relação ao centro
    float dist_x = abs(uv.x - center.x);
    float dist_y = abs(uv.y - center.y);


    // Define os limites da borda
    float outer_radius = radius;
    float inner_radius = radius - thickness;
	if(!normal){
		if (dist >= inner_radius && dist <= outer_radius) {
        	// Inverte a cor do que está atrás da borda
        	COLOR = texture(TEXTURE, UV);
			COLOR.rgb = vec3(1.0) - texture(screen_texture,SCREEN_UV).rgb;
    	}
	else {
        	// Fundo transparente fora da borda
        	discard;
    }
	}
	else {
		// Define a separação entre os traços
    float adjusted_size = size + separation;

    // Desenha os traços horizontais e verticais
    bool is_in_horizontal = (dist_y < thickness) && (dist_x > adjusted_size - 0.3) && (dist_x < adjusted_size - 0.28);
    bool is_in_vertical = (dist_x < thickness) && (dist_y > adjusted_size - 0.3) && (dist_y < adjusted_size - 0.28);

    if (is_in_horizontal || is_in_vertical) {
        // Inverte a cor do que está atrás da borda
        	COLOR = texture(TEXTURE, UV);
			COLOR.rgb = vec3(1.0) - texture(screen_texture,SCREEN_UV).rgb;
    } else {
        discard; // Fora dos traços, transparente
    }
}

}
