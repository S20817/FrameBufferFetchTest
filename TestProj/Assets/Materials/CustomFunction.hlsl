#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

void TentFilter_float(float2 uv, Texture2D source, float2 radius, out float4 outColor)
{
    float4 offset = radius.xyxy * float4(1.0, 1.0, -1.0, 0.0);
    float4 sum = 0;
    sum += SAMPLE_TEXTURE2D(source, sampler_PointClamp, uv - offset.xy);
    sum += SAMPLE_TEXTURE2D(source, sampler_PointClamp, uv - offset.wy) * 2.0;
    sum += SAMPLE_TEXTURE2D(source, sampler_PointClamp, uv - offset.zy);
    sum += SAMPLE_TEXTURE2D(source, sampler_PointClamp, uv + offset.zw) * 2.0;
    sum += SAMPLE_TEXTURE2D(source, sampler_PointClamp, uv) * 4.0;
    sum += SAMPLE_TEXTURE2D(source, sampler_PointClamp, uv + offset.xw) * 2.0;
    sum += SAMPLE_TEXTURE2D(source, sampler_PointClamp, uv + offset.zy);
    sum += SAMPLE_TEXTURE2D(source, sampler_PointClamp, uv + offset.wy) * 2.0;
    sum += SAMPLE_TEXTURE2D(source, sampler_PointClamp, uv + offset.xy);
    sum *= 0.0625;
    outColor = sum;
}

void BoxFilter_float(float2 uv, Texture2D source, float2 radius, out float4 outColor)
{
    float4 offset = radius.xyxy * float4(-1.0, -1.0, 1.0, 1.0);
    float4 sum = 0;
    sum += SAMPLE_TEXTURE2D(source, sampler_PointClamp, uv + offset.xy);
    sum += SAMPLE_TEXTURE2D(source, sampler_PointClamp, uv + offset.zy);
    sum += SAMPLE_TEXTURE2D(source, sampler_PointClamp, uv + offset.xw);
    sum += SAMPLE_TEXTURE2D(source, sampler_PointClamp, uv + offset.zw);
    sum *= 0.25;
    outColor = sum;
}

void NoFilter_float(float2 uv, Texture2D source, float2 radius, out float4 outColor)
{
    outColor = SAMPLE_TEXTURE2D(source, sampler_PointClamp, uv);
}
