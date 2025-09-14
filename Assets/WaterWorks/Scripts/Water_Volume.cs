using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public RTHandle source;

        private Material _material;

        private RTHandle tempRenderTarget;

        public CustomRenderPass(Material mat)
        {
            _material = mat;

            // Allocate a temporary RTHandle
            tempRenderTarget = RTHandles.Alloc(
                "_TemporaryColourTexture",
                name: "_TemporaryColourTexture"
            );
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Configure doesn’t need to do anything in this case
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Reflection)
            {
                CommandBuffer cmd = CommandBufferPool.Get("Water Volume Pass");

                // Blit with URP 14+ API
                Blitter.BlitCameraTexture(cmd, source, tempRenderTarget, _material, 0);
                Blitter.BlitCameraTexture(cmd, tempRenderTarget, source);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            // Cleanup is not strictly needed with RTHandles (they auto-release), 
            // but you could manually release if you Alloc per-frame.
        }
    }

    [System.Serializable]
    public class _Settings
    {
        public Material material = null;
        public RenderPassEvent renderPass = RenderPassEvent.AfterRenderingSkybox;
    }

    public _Settings settings = new _Settings();

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        if (settings.material == null)
        {
            settings.material = (Material)Resources.Load("Water_Volume");
        }

        m_ScriptablePass = new CustomRenderPass(settings.material);
        m_ScriptablePass.renderPassEvent = settings.renderPass;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Use RTHandle instead of RenderTargetIdentifier
        m_ScriptablePass.source = renderer.cameraColorTargetHandle;
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
