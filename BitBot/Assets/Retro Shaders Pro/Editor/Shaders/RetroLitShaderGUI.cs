using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.Rendering;

namespace RetroShadersPro.URP
{
    internal class RetroLitShaderGUI : ShaderGUI
    {
        MaterialProperty baseColorProp = null;
        const string baseColorName = "_BaseColor";
        const string baseColorLabel = "Base Color";

        MaterialProperty baseTexProp = null;
        const string baseTexName = "_BaseMap";
        const string baseTexLabel = "Base Texture";

        MaterialProperty resolutionLimitProp = null;
        const string resolutionLimitName = "_ResolutionLimit";
        const string resolutionLimitLabel = "Resolution Limit";

        MaterialProperty snapsPerUnitProp = null;
        const string snapsPerUnitName = "_SnapsPerUnit";
        const string snapsPerUnitLabel = "Snaps Per Meter";

        MaterialProperty colorBitDepthProp = null;
        const string colorBitDepthName = "_ColorBitDepth";
        const string colorBitDepthLabel = "Color Depth";

        MaterialProperty colorBitDepthOffsetProp = null;
        const string colorBitDepthOffsetName = "_ColorBitDepthOffset";
        const string colorBitDepthOffsetLabel = "Color Depth Offset";

        MaterialProperty ambientLightProp = null;
        const string ambientLightName = "_AmbientLight";
        const string ambientLightLabel = "Ambient Light Strength";

        MaterialProperty ambientToggleProp = null;
        const string ambientToggleName = "_USE_AMBIENT_OVERRIDE";
        const string ambientToggleLabel = "Ambient Light Override";

        MaterialProperty useAffineTexturesProp = null;
        const string useAffineTexturesName = "_USE_AFFINE_TEXTURES";
        const string useAffineTexturesLabel = "Affine Texture Mapping";

        MaterialProperty usePointFilteringProp = null;
        const string usePointFilteringName = "_USE_POINT_FILTER";
        const string usePointFilteringLabel = "Point Filtering";

        MaterialProperty alphaClipProp = null;
        const string alphaClipName = "_AlphaClip";
        const string alphaClipLabel = "Alpha Clip";

        MaterialProperty alphaClipThresholdProp = null;
        const string alphaClipThresholdName = "_Cutoff";
        const string alphaClipThresholdLabel = "Threshold";

        private MaterialProperty cullProp;
        private const string cullName = "_Cull";
        private const string cullLabel = "Render Face";

        private static readonly string[] surfaceTypeNames = Enum.GetNames(typeof(SurfaceType));
        private static readonly string[] renderFaceNames = Enum.GetNames(typeof(RenderFace));

        private enum SurfaceType
        {
            Opaque = 0,
            Transparent = 1
        }

        private enum RenderFace
        {
            Front = 2,
            Back = 1,
            Both = 0
        }

        private SurfaceType surfaceType = SurfaceType.Opaque;
        private RenderFace renderFace = RenderFace.Front;
        private bool showSurfaceOptions = false;

        private void FindProperties(MaterialProperty[] props)
        {
            baseColorProp = FindProperty(baseColorName, props, true);
            baseTexProp = FindProperty(baseTexName, props, true);
            resolutionLimitProp = FindProperty(resolutionLimitName, props, true);
            snapsPerUnitProp = FindProperty(snapsPerUnitName, props, true);
            colorBitDepthProp = FindProperty(colorBitDepthName, props, true);
            colorBitDepthOffsetProp = FindProperty(colorBitDepthOffsetName, props, true);
            ambientLightProp = FindProperty(ambientLightName, props, false);
            ambientToggleProp = FindProperty(ambientToggleName, props, false);
            useAffineTexturesProp = FindProperty(useAffineTexturesName, props, false);
            usePointFilteringProp = FindProperty(usePointFilteringName, props, false);

            //surfaceTypeProp = FindProperty(kSurfaceTypeProp, props, false);
            cullProp = FindProperty(cullName, props, true);
            alphaClipProp = FindProperty(alphaClipName, props, true);
            alphaClipThresholdProp = FindProperty(alphaClipThresholdName, props, true);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (materialEditor == null)
            {
                throw new ArgumentNullException("No MaterialEditor found (RetroLitShaderGUI).");
            }

            Material material = materialEditor.target as Material;

            FindProperties(properties);

            surfaceType = (SurfaceType)material.GetFloat("_Surface");
            renderFace = (RenderFace)material.GetFloat("_Cull");

            showSurfaceOptions = EditorGUILayout.Foldout(showSurfaceOptions, "Surface Options", EditorStyles.foldoutHeader);

            if(showSurfaceOptions)
            {
                EditorGUI.indentLevel++;

                // Display opaque/transparent options.
                bool surfaceTypeChanged = false;
                EditorGUI.BeginChangeCheck();
                {
                    surfaceType = (SurfaceType)EditorGUILayout.EnumPopup("Surface Type", surfaceType);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    surfaceTypeChanged = true;
                }

                // Display culling options.
                EditorGUI.BeginChangeCheck();
                {
                    renderFace = (RenderFace)EditorGUILayout.EnumPopup(cullLabel, renderFace);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    switch (renderFace)
                    {
                        case RenderFace.Both:
                            {
                                material.SetFloat("_Cull", 0);
                                break;
                            }
                        case RenderFace.Back:
                            {
                                material.SetFloat("_Cull", 1);
                                break;
                            }
                        case RenderFace.Front:
                            {
                                material.SetFloat("_Cull", 2);
                                break;
                            }
                    }
                }

                // Display alpha clip options.
                EditorGUI.BeginChangeCheck();
                {
                    materialEditor.ShaderProperty(alphaClipProp, alphaClipLabel);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    surfaceTypeChanged = true;
                }

                bool alphaClip;

                if (surfaceTypeChanged)
                {
                    switch (surfaceType)
                    {
                        case SurfaceType.Opaque:
                            {
                                material.SetOverrideTag("RenderType", "Opaque");
                                material.SetFloat("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                                material.SetFloat("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                                material.SetFloat("_ZWrite", 1);
                                material.SetFloat("_Surface", 0);

                                alphaClip = material.GetFloat(alphaClipName) >= 0.5f;
                                if (alphaClip)
                                {
                                    material.EnableKeyword("_ALPHATEST_ON");
                                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                                    material.SetOverrideTag("RenderType", "TransparentCutout");
                                }
                                else
                                {
                                    material.DisableKeyword("_ALPHATEST_ON");
                                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
                                    material.SetOverrideTag("RenderType", "Opaque");
                                }


                                break;
                            }
                        case SurfaceType.Transparent:
                            {
                                alphaClip = material.GetFloat(alphaClipName) >= 0.5f;
                                if (alphaClip)
                                {
                                    material.EnableKeyword("_ALPHATEST_ON");
                                }
                                else
                                {
                                    material.DisableKeyword("_ALPHATEST_ON");
                                }
                                material.SetOverrideTag("RenderType", "Transparent");
                                material.SetFloat("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                                material.SetFloat("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                                material.SetFloat("_ZWrite", 0);
                                material.SetFloat("_Surface", 1);

                                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                                break;
                            }
                    }
                }

                alphaClip = material.GetFloat(alphaClipName) >= 0.5f;
                if (alphaClip)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(alphaClipThresholdProp, alphaClipThresholdLabel);
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.LabelField("Retro Properties", EditorStyles.boldLabel);

            materialEditor.ShaderProperty(baseColorProp, baseColorLabel);
            materialEditor.ShaderProperty(baseTexProp, baseTexLabel);
            materialEditor.ShaderProperty(resolutionLimitProp, resolutionLimitLabel);
            materialEditor.ShaderProperty(snapsPerUnitProp, snapsPerUnitLabel);
            materialEditor.ShaderProperty(colorBitDepthProp, colorBitDepthLabel);
            materialEditor.ShaderProperty(colorBitDepthOffsetProp, colorBitDepthOffsetLabel);

            if (ambientLightProp != null)
            {
                materialEditor.ShaderProperty(ambientToggleProp, ambientToggleLabel);

                bool alphaClip = material.GetFloat(ambientToggleName) >= 0.5f;

                if (alphaClip)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(ambientLightProp, ambientLightLabel);
                    EditorGUI.indentLevel--;
                }
            }

            materialEditor.ShaderProperty(useAffineTexturesProp, useAffineTexturesLabel);
            materialEditor.ShaderProperty(usePointFilteringProp, usePointFilteringLabel);

            //base.OnGUI(materialEditor, properties);
        }
    }
}
