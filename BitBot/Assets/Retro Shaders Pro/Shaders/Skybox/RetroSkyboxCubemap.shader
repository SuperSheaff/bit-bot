Shader "Retro Shaders Pro/Skybox/Retro Skybox (Cubemap)"
{
    Properties
    {
		[MainColor] [HDR] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
		[NoScaleOffset] _BaseCubemap("Base Cubemap", Cube) = "grey" {}
		_Rotation("Rotation", Range(0.0, 360.0)) = 0.0
		_ResolutionLimit("Resolution Limit (Power of 2)", Integer) = 128
		_ColorBitDepth("Color Values", Integer) = 16
		_ColorBitDepthOffset("Color Values Offset", Range(0.0, 1.0)) = 0.0
    }
    SubShader
    {
		Tags
		{
			"RenderType" = "Background"
			"Queue" = "Background"
			"PreviewType" = "Skybox"
			"RenderPipeline" = "UniversalPipeline"
		}

		Cull Off
		ZWrite Off

        Pass
        {
			HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"

			#define EPSILON 1e-06

            struct appdata
            {
				float4 positionOS : Position;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
				float4 positionCS : SV_Position;
				float3 uv : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
            };

			TEXTURECUBE(_BaseCubemap);

			CBUFFER_START(UnityPerMaterial)
				float4 _BaseColor;
				float4 _BaseCubemap_TexelSize;
				float4 _BaseCubemap_ST;
				float _Rotation;
				int _ResolutionLimit;
				int _ColorBitDepth;
				float _ColorBitDepthOffset;
			CBUFFER_END

			// From: https://github.com/TwoTailsGames/Unity-Built-in-Shaders/blob/master/DefaultResourcesExtra/Skybox-Cubed.shader
			float3 RotateAroundYInDegrees(float3 vertex, float degrees)
			{
				float alpha = degrees * PI / 180.0;
				float sina, cosa;
				sincos(alpha, sina, cosa);
				float2x2 m = float2x2(cosa, -sina, sina, cosa);
				return float3(mul(m, vertex.xz), vertex.y).xzy;
			}

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 positionRotated = RotateAroundYInDegrees(v.positionOS, _Rotation);
				o.positionCS = TransformObjectToHClip(positionRotated);
				o.uv = v.positionOS.xyz;

				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				int targetResolution = (int)log2(_ResolutionLimit);
				int actualResolution = (int)log2(_BaseCubemap_TexelSize.zw);
				int lod = actualResolution - targetResolution;

				float4 baseColor = _BaseColor * SAMPLE_TEXTURECUBE_LOD(_BaseCubemap, sampler_PointRepeat, i.uv, lod);

				int r = (baseColor.r - EPSILON) * _ColorBitDepth;
				int g = (baseColor.g - EPSILON) * _ColorBitDepth;
				int b = (baseColor.b - EPSILON) * _ColorBitDepth;

				float divisor = _ColorBitDepth - 1.0f;

				float3 posterizedColor = float3(r, g, b) / divisor;
				posterizedColor += 1.0f / _ColorBitDepth * _ColorBitDepthOffset;

				return float4(posterizedColor, baseColor.a);
			}
            ENDHLSL
        }
    }
}
