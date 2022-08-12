Shader "Unlit/HealthBarShader"
{
    Properties
    {
        [NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}
        _Health ("Health", Range(0,1)) = 1
        _LowHealth ("Critical Health Value", Range(0,1)) = 0.1
        _ColorA ("Full Color",Color) = (0,1,0,0)
        _ColorB ("Empty Color",Color) = (1,0,0,0)
        _UpThreshold ("Upper Threshold", Range(0,1)) = 1
        _LowThreshold ("Lower Threshold", Range(0,1)) = 1
        _Frequency ("Frequency",Range(0,10)) = 1
        _Amplitude ("Amplitude",Range(0,1)) = 1
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
            float _Frequency;
            float _Amplitude;

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

            sampler2D _MainTex;
            float _Health;
            float _LowHealth;

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
                //float tHealthColor = saturate(InverseLerp(_LowThreshold,_UpThreshold,_Health));
                float3 healthBarColor = tex2D(_MainTex,float2(_Health,i.uv.y));

                float flash = cos(_Time.y*_Frequency* (_Health<_LowHealth))*_Amplitude;
                
                //float3 healthBarColor = lerp(_ColorB,_ColorA,tHealthColor);
                float3 bgColor = float3(0,0,0);
                float healthBarMask = _Health > i.uv.x;

                clip(healthBarMask-0.5);
                
                //float3 outColor = lerp(bgColor,healthBarColor,healthBarMask);

                return float4(healthBarColor*flash*healthBarMask,1);
                //return float4(healthBarColor.rgb * healthBarMask,1);
                
            }
            ENDCG
        }
    }
}
