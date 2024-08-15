namespace RetroShadersPro.URP
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;
#if UNITY_6000_0_OR_NEWER
    using UnityEngine.Rendering.RenderGraphModule;
#endif

    public class CRTEffect : ScriptableRendererFeature
    {
        CRTRenderPass pass;

        public override void Create()
        {
            pass = new CRTRenderPass();
            name = "CRT";
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var settings = VolumeManager.instance.stack.GetComponent<CRTSettings>();

            if (settings != null && settings.IsActive())
            {
                renderer.EnqueuePass(pass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            pass.Dispose();
            base.Dispose(disposing);
        }

        class CRTRenderPass : ScriptableRenderPass
        {
            private Material material;
            private RTHandle tempTexHandle;
            private RTHandle interlaceTexHandle;

            private int frameCounter = 0;

            public CRTRenderPass()
            {
                profilingSampler = new ProfilingSampler("CRT Effect");
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

#if UNITY_6000_0_OR_NEWER
                requiresIntermediateTexture = true;
#endif
            }

            private void CreateMaterial()
            {
                var shader = Shader.Find("Retro Shaders Pro/Post Processing/CRT");

                if (shader == null)
                {
                    Debug.LogError("Cannot find shader: \"Retro Shaders Pro/Post Processing/CRT\".");
                    return;
                }

                material = new Material(shader);
            }

            private static RenderTextureDescriptor GetCopyPassDescriptor(RenderTextureDescriptor descriptor)
            {
                descriptor.msaaSamples = 1;
                descriptor.depthBufferBits = (int)DepthBits.None;

                var settings = VolumeManager.instance.stack.GetComponent<CRTSettings>();
                int width = descriptor.width / settings.pixelSize.value;
                int height = descriptor.height / settings.pixelSize.value;

                descriptor.width = width;
                descriptor.height = height;

                return descriptor;
            }

            private static RenderTextureDescriptor GetInterlaceDescriptor(RenderTextureDescriptor descriptor)
            {
                descriptor.msaaSamples = 1;
                descriptor.depthBufferBits = (int)DepthBits.None;

                return descriptor;
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                ResetTarget();
                
                RenderingUtils.ReAllocateIfNeeded(ref tempTexHandle, GetCopyPassDescriptor(cameraTextureDescriptor));
                RenderingUtils.ReAllocateIfNeeded(ref interlaceTexHandle, GetInterlaceDescriptor(cameraTextureDescriptor));

                base.Configure(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if(material == null)
                {
                    CreateMaterial();
                }

                var settings = VolumeManager.instance.stack.GetComponent<CRTSettings>();

                if (renderingData.cameraData.isSceneViewCamera && !settings.showInSceneView.value)
                {
                    return;
                }

                if(renderingData.cameraData.isPreviewCamera)
                {
                    return;
                }

                CommandBuffer cmd = CommandBufferPool.Get();

                var rgbTex = settings.rgbTex.value == null ? Texture2D.whiteTexture : settings.rgbTex.value;
                var scanlineTex = settings.scanlineTex.value == null ? Texture2D.whiteTexture : settings.scanlineTex.value;

                // Set CRT effect properties.
                material.SetColor("_BackgroundColor", settings.backgroundColor.value);
                material.SetFloat("_DistortionStrength", settings.distortionStrength.value);
                material.SetTexture("_RGBTex", rgbTex);
                material.SetFloat("_RGBStrength", settings.rgbStrength.value);
                material.SetTexture("_ScanlineTex", scanlineTex);
                material.SetFloat("_ScanlineStrength", settings.scanlineStrength.value);
                material.SetInt("_Size", settings.scanlineSize.value);
                material.SetFloat("_ScrollSpeed", settings.scrollSpeed.value);
                material.SetFloat("_AberrationStrength", settings.aberrationStrength.value);
                material.SetFloat("_Brightness", settings.brightness.value);
                material.SetFloat("_Contrast", settings.contrast.value);
                material.SetInteger("_Interlacing", frameCounter++ % 2);
                material.SetTexture("_InputTexture", interlaceTexHandle);

                if(settings.enableInterlacing.value)
                {
                    material.EnableKeyword("_INTERLACING_ON");
                }
                else
                {
                    material.DisableKeyword("_INTERLACING_ON");
                }

                RTHandle cameraTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;

                using (new ProfilingScope(cmd, profilingSampler))
                {
                    // Perform the Blit operations for the CRT effect.
                    using (new ProfilingScope(cmd, profilingSampler))
                    {
                        Blit(cmd, cameraTargetHandle, tempTexHandle);
                        Blit(cmd, tempTexHandle, cameraTargetHandle, material, 0);

                        if (settings.enableInterlacing.value)
                        {
                            Blit(cmd, cameraTargetHandle, interlaceTexHandle);
                        }
                    }
                }  

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }

            public void Dispose()
            {
                tempTexHandle?.Release();
            }

#if UNITY_6000_0_OR_NEWER

            private class CopyPassData
            {
                public TextureHandle inputTexture;
            }

            private class MainPassData
            {
                public Material material;
                public TextureHandle inputTexture;
            }

            private static void ExecuteCopyPass(RasterCommandBuffer cmd, RTHandle source)
            {
                Blitter.BlitTexture(cmd, source, new Vector4(1, 1, 0, 0), 0.0f, false);
            }

            private static void ExecuteMainPass(RasterCommandBuffer cmd, RTHandle source, Material material)
            {
                // Set CRT effect properties.
                var settings = VolumeManager.instance.stack.GetComponent<CRTSettings>();
                material.SetColor("_BackgroundColor", settings.backgroundColor.value);
                material.SetFloat("_DistortionStrength", settings.distortionStrength.value);
                material.SetTexture("_ScanlineTex", settings.scanlineTex.value);
                material.SetFloat("_ScanlineStrength", settings.scanlineStrength.value);
                material.SetInt("_Size", settings.scanlineSize.value);
                material.SetFloat("_ScrollSpeed", settings.scrollSpeed.value);
                material.SetFloat("_AberrationStrength", settings.aberrationStrength.value);

                Blitter.BlitTexture(cmd, source, new Vector4(1, 1, 0, 0), material, 0);
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                Debug.LogWarning("Interlacing on Unity 6 not yet implemented.");
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

                UniversalRenderer renderer = (UniversalRenderer)cameraData.renderer;
                var colorCopyDescriptor = GetCopyPassDescriptor(cameraData.cameraTargetDescriptor);
                TextureHandle copiedColor = TextureHandle.nullHandle;

                // Perform the intermediate copy pass (source -> temp).
                copiedColor = UniversalRenderer.CreateRenderGraphTexture(renderGraph, colorCopyDescriptor, "_CRTColorCopy", false);

                using (var builder = renderGraph.AddRasterRenderPass<CopyPassData>("CRT_CopyColor", out var passData, profilingSampler))
                {
                    passData.inputTexture = resourceData.activeColorTexture;

                    builder.UseTexture(resourceData.activeColorTexture, AccessFlags.Read);
                    builder.SetRenderAttachment(copiedColor, 0, AccessFlags.Write);
                    builder.SetRenderFunc((CopyPassData data, RasterGraphContext context) => ExecuteCopyPass(context.cmd, data.inputTexture));
                }

                // Perform main pass (temp -> source).
                using (var builder = renderGraph.AddRasterRenderPass<MainPassData>("CRT_MainPass", out var passData, profilingSampler))
                {
                    passData.material = material;
                    passData.inputTexture = copiedColor;

                    builder.UseTexture(copiedColor, AccessFlags.Read);
                    builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);
                    builder.SetRenderFunc((MainPassData data, RasterGraphContext context) => ExecuteMainPass(context.cmd, data.inputTexture, data.material));
                }
            }

#endif
        }
    }
}

