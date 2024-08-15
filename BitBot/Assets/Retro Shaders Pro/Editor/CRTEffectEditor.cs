namespace RetroShadersPro.URP
{
    using UnityEditor.Rendering;
    using UnityEngine.Rendering.Universal;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

#if UNITY_2022_2_OR_NEWER
    [CustomEditor(typeof(CRTSettings))]
#else
    [VolumeComponentEditor(typeof(CRTSettings))]
#endif
    public class CRTEffectEditor : VolumeComponentEditor
    {
        SerializedDataParameter showInSceneView;
        SerializedDataParameter enabled;
        SerializedDataParameter distortionStrength;
        SerializedDataParameter backgroundColor;
        SerializedDataParameter rgbTex;
        SerializedDataParameter rgbStrength;
        SerializedDataParameter scanlineTex;
        SerializedDataParameter scanlineStrength;
        SerializedDataParameter scanlineSize;
        SerializedDataParameter scrollSpeed;
        SerializedDataParameter pixelSize;
        SerializedDataParameter aberrationStrength;
        SerializedDataParameter brightness;
        SerializedDataParameter contrast;
        SerializedDataParameter enableInterlacing;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<CRTSettings>(serializedObject);
            showInSceneView = Unpack(o.Find(x => x.showInSceneView));
            enabled = Unpack(o.Find(x => x.enabled));
            distortionStrength = Unpack(o.Find(x => x.distortionStrength));
            backgroundColor = Unpack(o.Find(x => x.backgroundColor));
            rgbTex = Unpack(o.Find(x => x.rgbTex));
            rgbStrength = Unpack(o.Find(x => x.rgbStrength));
            scanlineTex = Unpack(o.Find(x => x.scanlineTex));
            scanlineStrength = Unpack(o.Find(x => x.scanlineStrength));
            scanlineSize = Unpack(o.Find(x => x.scanlineSize));
            scrollSpeed = Unpack(o.Find(x => x.scrollSpeed));
            pixelSize = Unpack(o.Find(x => x.pixelSize));
            aberrationStrength = Unpack(o.Find(x => x.aberrationStrength));
            brightness = Unpack(o.Find(x => x.brightness));
            contrast = Unpack(o.Find(x => x.contrast));
            enableInterlacing = Unpack(o.Find(x => x.enableInterlacing));
        }

        public override void OnInspectorGUI()
        {
            if (!RetroShaderUtility.CheckEffectEnabled<CRTEffect>())
            {
                EditorGUILayout.HelpBox("The CRT effect must be added to your renderer's Renderer Features list.", MessageType.Error);
                if (GUILayout.Button("Add CRT Renderer Feature"))
                {
                    RetroShaderUtility.AddEffectToPipelineAsset<CRTEffect>();
                }
            }

            PropertyField(showInSceneView);
            PropertyField(enabled);
            PropertyField(distortionStrength);
            PropertyField(backgroundColor);
            PropertyField(rgbTex);
            PropertyField(rgbStrength);
            PropertyField(scanlineTex);
            PropertyField(scanlineStrength);
            PropertyField(scanlineSize);
            PropertyField(scrollSpeed);
            PropertyField(pixelSize);
            PropertyField(aberrationStrength);
            PropertyField(brightness);
            PropertyField(contrast);
            PropertyField(enableInterlacing);
        }
    }
}
