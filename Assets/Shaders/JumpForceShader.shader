Shader "Unlit/JumpForceShader"
{
    Properties
    {
        _HoldTime ("HoldTime", Range(0,1)) = 1
        _ColorA ("Full Color",Color) = (0,1,0,0)
        _ColorB ("Empty Color",Color) = (1,0,0,0)
        _UpThreshold ("Upper Threshold", Range(0,1)) = 1
        _LowThreshold ("Lower Threshold", Range(0,1)) = 1
        _BorderWidth ("Border Width",Range(0,.9)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _ColorA;
            float4 _ColorB;
            float _UpThreshold;
            float _LowThreshold;
            float _BorderWidth;

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            float _HoldTime;

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float InverseLerp(float a, float b, float v)
            {
                return(v-a)/(b-a);
            }
            
            float4 frag (Interpolators i) : SV_Target
            {
                float tHealthColor = saturate(InverseLerp(_LowThreshold,_UpThreshold,_HoldTime));
                
                float3 healthBarColor = lerp(_ColorB,_ColorA,tHealthColor);
                float3 bgColor = float4(0,0,0,0);
                float healthBarMask = _HoldTime > i.uv.x;
                

                //clip(healthBarMask-0.5);
                float2 coords = i.uv;
                coords.x *=8;
                float2 pointOnLineSeg = float2(clamp(coords.x,0.5,7.5),0.5);
                float sdf = distance(coords,pointOnLineSeg)*2-1;
                float border = sdf + _BorderWidth;
                clip(-sdf);
                float pd = fwidth(border);
                float borderMask = 1-saturate(border/pd);
                //return float4(borderMask.xxx,1);
                float3 outColor = lerp(bgColor,healthBarColor,1);
                return float4(outColor*borderMask*healthBarMask,1);
                return float4(healthBarColor*healthBarMask*borderMask,1);
                //return float4(healthBarColor.rgb * healthBarMask,1);
                
            }
            ENDCG
        }
    }
}
