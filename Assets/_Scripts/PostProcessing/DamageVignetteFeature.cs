using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

// Add this to your Universal Render Pipeline Asset's Renderer Features
public class DamageVignetteFeature : ScriptableRendererFeature
{
    class DamageVignettePass : ScriptableRenderPass
    {
        private Material mat;
        private RTHandle tempTexture;

        public DamageVignettePass(Material material) => mat = material;

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (mat == null) return;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            TextureHandle source = resourceData.activeColorTexture;

            RenderTextureDescriptor desc = cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            TextureHandle destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_TempDamageVignette", false);

            RenderGraphUtils.BlitMaterialParameters blitParams = new RenderGraphUtils.BlitMaterialParameters(source, destination, mat, 0);
            renderGraph.AddBlitPass(blitParams, passName: "DamageVignette");
        }

        public void Dispose() => tempTexture?.Release();
    }

    [System.Serializable]
    public class Settings
    {
        public Shader shader;
    }

    public Settings settings = new Settings();
    private DamageVignettePass pass;
    private Material material;

    public override void Create()
    {
        if (settings.shader == null) return;
        material = CoreUtils.CreateEngineMaterial(settings.shader);
        pass = new DamageVignettePass(material);
        pass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null) return;
        renderer.EnqueuePass(pass);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(material);
        pass?.Dispose();
    }
}