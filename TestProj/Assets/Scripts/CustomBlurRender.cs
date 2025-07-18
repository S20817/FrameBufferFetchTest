using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MyTest
{
    public enum FilterTypeEnum
    {
        NoFilter,
        BoxFilter,
        TentFilter,
    }

    [RequireComponent(typeof(Camera))]
    [ExecuteAlways]
    public class CustomBlurRender : MonoBehaviour
    {
        [SerializeField]
        private Material blurMaterial;
        [SerializeField]
        private float blurRadius = 1;
        [SerializeField]
        private FilterTypeEnum filterType = FilterTypeEnum.BoxFilter;
        [SerializeField]
        private RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        [SerializeField]
        private bool useFrameBufferFetch = true;

        private readonly CustomBlurPass _customBlurPass = new();

        private Camera _camera;
        private Camera MyCamera
        {
            get
            {
                if (_camera == null)
                {
                    _camera = GetComponent<Camera>();
                }
                return _camera;
            }
        }

        private UniversalAdditionalCameraData _cameraData;
        private UniversalAdditionalCameraData MyCameraData
        {
            get
            {
                if (_cameraData == null)
                {
                    _cameraData = MyCamera.GetUniversalAdditionalCameraData();
                }
                return _cameraData;
            }
        }

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera renderCamera)
        {
            if (renderCamera != MyCamera)
            {
                return;
            }

            _customBlurPass.BlurMaterial = blurMaterial;
            _customBlurPass.BlurRadius = blurRadius;
            _customBlurPass.filterType = filterType;
            _customBlurPass.renderPassEvent = renderPassEvent;
            _customBlurPass.UseFrameBufferFetch = useFrameBufferFetch;
            MyCameraData.scriptableRenderer.EnqueuePass(_customBlurPass);
        }
    }
}
