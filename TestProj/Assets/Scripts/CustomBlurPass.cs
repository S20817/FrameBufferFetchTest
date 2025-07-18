using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

namespace MyTest
{
    public class CustomBlurPass : ScriptableRenderPass
    {
        public Material BlurMaterial { get; set; }
        public float BlurRadius { get; set; } = 1.0f;
        public FilterTypeEnum filterType { get; set; } = FilterTypeEnum.BoxFilter;
        public bool UseFrameBufferFetch { get; set; } = true;

        public static readonly int BlurRadiusID = Shader.PropertyToID("_Radius");
        public static readonly int FilterTypeID = Shader.PropertyToID("_FILTERTYPE");
        public static readonly string[] FilterTypeKeywords =
            { "_FILTERTYPE_NOFILTER", "_FILTERTYPE_BOXFILTER", "_FILTERTYPE_TENTFILTER" };

        private class PassData
        {
            public TextureHandle Source;
            public Material Mat;
            public int PassIndex;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<UniversalCameraData>();
            var resourceData = frameData.Get<UniversalResourceData>();

            var desc = cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            desc.msaaSamples = 1;
            desc.useMipMap = false;
            var tempTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "Temp Texture", false);

            BlurMaterial.SetFloat(BlurRadiusID, BlurRadius);
            BlurMaterial.SetFloat(FilterTypeID, (float)filterType);
            for (int i = 0; i < FilterTypeKeywords.Length; i++)
            {
                CoreUtils.SetKeyword(BlurMaterial, FilterTypeKeywords[i], i == (int)filterType);
            }

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Custom Blur Pass", out var passData))
            {
                passData.Source = resourceData.activeColorTexture;
                passData.Mat = BlurMaterial;
                passData.PassIndex = 0;

                builder.SetRenderAttachment(tempTexture, 0, AccessFlags.WriteAll);
                builder.UseTexture(passData.Source, AccessFlags.Read);
                builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
                {
                    var cmd = context.cmd;
                    var scaleBias = new Vector4(1, 1, 0, 0);
                    Blitter.BlitTexture(cmd, data.Source, scaleBias, data.Mat, data.PassIndex);
                });
            }

            if (UseFrameBufferFetch)
            {
                renderGraph.AddCopyPass(tempTexture, resourceData.activeColorTexture, "CopyBackToCamera");
            }
            else
            {
                renderGraph.AddBlitPass(tempTexture, resourceData.activeColorTexture, Vector2.one, Vector2.zero, passName: "BlitBackToCamera");
            }
        }
    }
}

