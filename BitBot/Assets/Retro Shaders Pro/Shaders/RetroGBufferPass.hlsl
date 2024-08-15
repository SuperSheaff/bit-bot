#ifndef RETRO_GBUFFER_PASS_INCLUDED
#define RETRO_GBUFFER_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"

struct appdata
{
	float4 positionOS : Position;
	float4 color : COLOR;
	float3 normalOS : NORMAL;
	float2 uv : TEXCOORD0;
	float2 staticLightmapUV : TEXCOORD1;
#ifdef DYNAMICLIGHTMAP_ON
	float2 dynamicLightmapUV : TEXCOORD2;
#endif
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
	float4 positionCS : SV_Position;
	float4 color : COLOR;
#if _USE_AFFINE_TEXTURES_ON
	noperspective float2 uv : TEXCOORD0;
#else
	float2 uv : TEXCOORD0;
#endif

	DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 1);
	float3 normalWS : TEXCOORD2;
	float3 positionWS : TEXCOORD3;

#ifdef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
	float4 shadowCoord : TEXCOORD4;
#endif

#ifdef DYNAMICLIGHTMAP_ON
	float2 dynamicLightmapUV : TEXCOORD5;
#endif

	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
};

FragmentOutput GBuffer(InputData inputData, SurfaceData surfaceData)
{
	FragmentOutput o;

	uint materialFlags = 0;
	materialFlags |= kMaterialFlagSpecularHighlightsOff;
	float materialFlagsPacked = PackMaterialFlags(materialFlags);

	float3 normalWS = PackNormal(inputData.normalWS);

	Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, inputData.shadowMask);
	MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, inputData.shadowMask);
	float3 giEmission = (inputData.bakedGI * surfaceData.albedo) + surfaceData.emission;

	o.GBuffer0.rgb = surfaceData.albedo;
	o.GBuffer0.a = materialFlagsPacked;

	o.GBuffer1.rgb = surfaceData.specular;
	o.GBuffer1.a = 0.0f;

	o.GBuffer2.rgb = normalWS;
	o.GBuffer2.a = surfaceData.smoothness;

	o.GBuffer3.rgb = giEmission; // GI + Emission.
	o.GBuffer3.a = 1.0f;

#if OUTPUT_SHADOWMASK
	o.GBUFFER_SHADOWMASK = inputData.shadowMask;
#endif

	return o;
}

v2f gBufferVert(appdata v)
{
	v2f o = (v2f)0;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_TRANSFER_INSTANCE_ID(v, o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	float4 positionVS = mul(UNITY_MATRIX_MV, v.positionOS);
	positionVS = floor(positionVS * _SnapsPerUnit) / _SnapsPerUnit;
	o.positionCS = mul(UNITY_MATRIX_P, positionVS);

	o.positionWS = mul(UNITY_MATRIX_M, v.positionOS);
	o.normalWS = TransformObjectToWorldNormal(v.normalOS);
	o.uv = TRANSFORM_TEX(v.uv, _BaseMap);

	OUTPUT_LIGHTMAP_UV(v.staticLightmapUV, unity_LightmapST, o.staticLightmapUV);
	OUTPUT_SH(o.normalWS, o.vertexSH);

#ifdef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
	o.shadowCoord = TransformWorldToShadowCoord(o.positionWS);
#endif

#ifdef DYNAMICLIGHTMAP_ON
	o.dynamicLightmapUV = v.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#endif

	o.color = v.color;

	return o;
}

FragmentOutput gBufferFrag(v2f i)
{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

	int targetResolution = (int)log2(_ResolutionLimit);
	int actualResolution = (int)log2(_BaseMap_TexelSize.zw);
	int lod = actualResolution - targetResolution;

#if _USE_POINT_FILTER_ON
	float4 baseColor = _BaseColor * SAMPLE_TEXTURE2D_LOD(_BaseMap, sampler_PointRepeat, i.uv, lod) * i.color;
#else
	float4 baseColor = _BaseColor * SAMPLE_TEXTURE2D_LOD(_BaseMap, sampler_LinearRepeat, i.uv, lod) * i.color;
#endif

	Alpha(baseColor.a, _BaseColor, _Cutoff);

#ifdef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
	float4 shadowCoord = i.shadowCoord;
#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
	float4 shadowCoord = TransformWorldToShadowCoord(i.positionWS);
#else
	float4 shadowCoord = 0;
#endif

	InputData inputData = (InputData)0;
	inputData.positionCS = i.positionCS;
	inputData.positionWS = i.positionWS;
	inputData.normalWS = i.normalWS;
	inputData.viewDirectionWS = normalize(GetWorldSpaceViewDir(i.positionWS));
	inputData.shadowCoord = shadowCoord;
	inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.positionCS);
	inputData.shadowMask = SAMPLE_SHADOWMASK(i.staticLightmapUV);

#ifdef DYNAMICLIGHTMAP_ON
	inputData.bakedGI = SAMPLE_GI(i.staticLightmapUV, i.dynamicLightmapUV, i.vertexSH, normalWS);
#else
	inputData.bakedGI = SAMPLE_GI(i.staticLightmapUV, i.vertexSH, i.normalWS);
#endif

	SurfaceData surfaceData = (SurfaceData)0;
	surfaceData.albedo = baseColor.rgb;
	surfaceData.alpha = baseColor.a;
	surfaceData.emission = 0.0f;
	surfaceData.metallic = 0.0f;
	surfaceData.occlusion = 1.0f;
	surfaceData.smoothness = 0.0f;
	surfaceData.specular = 0.0f;
	//surfaceData.normalTS = normalTS;
	
	FragmentOutput output = GBuffer(inputData, surfaceData);
	return output;
}

#endif // RETRO_GBUFFER_PASS_INCLUDED
