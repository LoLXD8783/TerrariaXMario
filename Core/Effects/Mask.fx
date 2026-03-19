sampler uImage : register(s0);

float4 Mask(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage, coords);
    float tolerance = 0.01;
    bool isWhite = all(abs(color.rgb - float3(1.0, 1.0, 1.0)) < tolerance);
    
    if (isWhite)
        return float4(color.rgb, 0.0);
    
    return color;
}

technique Technique1
{
    pass MaskPass
    {
        PixelShader = compile ps_3_0 Mask();
    }
};