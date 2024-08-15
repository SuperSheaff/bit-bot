namespace RetroShadersPro.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    [System.Serializable, VolumeComponentMenu("Retro Shaders Pro/CRT")]
    public class CRTSettings : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Should the effect be visible in the Scene View?")]
        public BoolParameter showInSceneView = new BoolParameter(true);

        [Tooltip("Should the effect be rendered?")]
        public BoolParameter enabled = new BoolParameter(false);

        [Header("Distortion Settings")]

        [Tooltip("Strength of the distortion. Values above zero cause CRT screen-like distortion; values below zero bulge outwards.")]
        public ClampedFloatParameter distortionStrength = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

        [Tooltip("Color of the background around the 'screen'.")]
        public ColorParameter backgroundColor = new ColorParameter(Color.black);

        [Header("RGB Overlay Settings")]

        [Tooltip("RGB Overlay texture.")]
        public TextureParameter rgbTex = new TextureParameter(null);

        [Tooltip("Strength of the RGB overlay effect.")]
        public ClampedFloatParameter rgbStrength = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

        [Header("Scanline Settings")]

        [Tooltip("Scanlines texture.")]
        public TextureParameter scanlineTex = new TextureParameter(null);

        [Tooltip("Strength of the scanline effect.")]
        public ClampedFloatParameter scanlineStrength = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

        [Tooltip("Pixel size of the scanlines.")]
        public ClampedIntParameter scanlineSize = new ClampedIntParameter(8, 1, 64);

        [Tooltip("Scroll speed of scanlines vertically.")]
        public ClampedFloatParameter scrollSpeed = new ClampedFloatParameter(0.0f, 0.0f, 10.0f);

        [Header("Pixelation Settings")]

        [Tooltip("Size of each new 'pixel' in the image.")]
        public ClampedIntParameter pixelSize = new ClampedIntParameter(1, 1, 256);

        [Header("Chromatic Aberration Settings")]

        [Tooltip("Amount of color channel separation at the screen edges.")]
        public ClampedFloatParameter aberrationStrength = new ClampedFloatParameter(0.5f, 0.0f, 10.0f);

        [Header("Brightness Adjustment Settings")]

        [Tooltip("Global brightness control, 1 = no change.")]
        public ClampedFloatParameter brightness = new ClampedFloatParameter(1.0f, 0.0f, 3.0f);

        [Tooltip("Global contrast modifier.")]
        public ClampedFloatParameter contrast = new ClampedFloatParameter(1.0f, 0.0f, 3.0f);

        [Header("Interlacing Settings")]

        public BoolParameter enableInterlacing = new BoolParameter(false);

        public bool IsActive()
        {
            return enabled.value && active;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}

