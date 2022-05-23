Shader "Unlit/Shader1"
{
    Properties  //input data
    { 
        _ColorA ("ColorA", Color) = (1,1,1,1)
        _ColorB ("ColorB", Color) = (1,1,1,1)
        _ColorStart ("Color Start", Range(0,1)) = 1
        _ColorEnd ("Color End", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            //ZWrite off
            //Blend One One //additive
            //Blend DstColor Zero //multiplicative
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define TAU 6.28318530718
            
            float4 _ColorA;
            float4 _ColorB;
            float _ColorStart;
            float _ColorEnd;
            
            struct MeshData // per-vertex mesh data
            {
                float4 vertex : POSITION; // vertex position
                float3 normals : NORMAL;
                // float4 tangent : TANGENT;
                // float4 color : COLOR;
                float2 uv0 : TEXCOORD0; // uv0 coordinates
                //float2 uv1 : TEXCOORD1; // uv1 coordinates
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION; // clip space position
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex); // converts local space to clip space
                o.normal = v.normals;
                o.uv = v.uv0; //(v.uv0+_Offset) * _Scale; // simple pass through
                return o;
            }

            float InverseLerp(float a, float b, float v)
            {
                return((v-a)/(b-a));
            }
            
            float4 frag (Interpolators i) : SV_Target
            {
                //float t = saturate(InverseLerp(_ColorStart,_ColorEnd,i.uv.x));
                //float t = abs(frac(i.uv.x * 5) * 2 - 1);

                
                
                float xOffset = cos(i.uv.y*TAU*8)*.01;
                float t = cos((i.uv.x - xOffset + _Time.x)* TAU * 5) *.5+.5;
                //return t;
                float4 outColor = lerp(_ColorA,_ColorB,t);
                return outColor;
            }
            ENDCG
        }
    }
}
