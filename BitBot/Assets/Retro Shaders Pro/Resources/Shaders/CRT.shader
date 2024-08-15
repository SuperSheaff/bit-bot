Shader "Retro Shaders Pro/Post Processing/CRT"
{
	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"RenderPipeline" = "UniversalPipeline"
		}

		Pass
		{
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment frag
			#pragma shader_feature_local_fragment _INTERLACING_ON

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

			TEXTURE2D(_InputTexture);
			TEXTURE2D(_RGBTex);
			TEXTURE2D(_ScanlineTex);

#if UNITY_VERSION < 600000
			float4 _BlitTexture_TexelSize;
#endif
			float4 _BackgroundColor;
			float _DistortionStrength;
			int _Size;
			float _RGBStrength;
			float _ScanlineStrength;
			float _ScrollSpeed;
			float _AberrationStrength;
			float _Brightness;
			float _Contrast;
			int _Interlacing;

            float4 frag (Varyings i) : SV_Target
            {
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				// Apply barrel distortion to UVs.
				float2 UVs = i.texcoord - 0.5f;
				float2 offset = UVs;
				UVs = UVs * (1 + _DistortionStrength * length(UVs) * length(UVs)) + 0.5f;

				// Apply chromatic aberration
				float2 redUVs = UVs + offset * _AberrationStrength * _BlitTexture_TexelSize.xy;
				float2 blueUVs = UVs - offset * _AberrationStrength * _BlitTexture_TexelSize.xy;

				float red = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, redUVs).r;
				float green = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, UVs).g;
				float blue = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, blueUVs).b;

				float3 col = float3(red, green, blue);

				// Apply brightness and contrast modifiers.
				col = saturate(col * _Brightness);
				col = col - _Contrast * (col - 1.0f) * col * (col - 0.5f);

				// Apply RGB overlay texture.
				float2 rgbUV = UVs * _ScreenParams.xy / _Size;
				float3 rgbCells = SAMPLE_TEXTURE2D(_RGBTex, sampler_LinearRepeat, rgbUV).rgb;

				col = lerp(col, col * rgbCells, _RGBStrength);

				// Apply scanline overlay texture.
				rgbUV.y += _Time.y * _ScrollSpeed;
				float3 scanlines = SAMPLE_TEXTURE2D(_ScanlineTex, sampler_LinearRepeat, rgbUV).rgb;

				col = lerp(col, col * scanlines, _ScanlineStrength);

				// Apply interlacing if enabled.
#ifdef _INTERLACING_ON
				float3 inputCol = SAMPLE_TEXTURE2D(_InputTexture, sampler_LinearClamp, i.texcoord).rgb;
				col = lerp(col, inputCol, floor(UVs.y * _BlitTexture_TexelSize.w + _Interlacing) % 2.0f);
#endif

				// Apply border to pixels outside barrel distortion 0-1 range.
				col = (UVs.x >= 0.0f && UVs.x <= 1.0f && UVs.y >= 0.0f && UVs.y <= 1.0f) ? col : _BackgroundColor;

				return float4(col, 1.0f);
            }
            ENDHLSL
        }
    }
}
