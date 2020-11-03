Shader "Bumpkin Rat/ToonOutline"
{
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Diffuse", 2D) = "white"{}
        _ToonRamp("Toon Ramp", 2D) = "white"{}
        _Shadow("Shadow Strength", Range(0, 0.55)) = 0.5

        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _Outline("Outline Width", Range(0, 1)) = 0.1

        _Noise("Noise Texture", 2D) = "white"{}
        _NoiseStrength("Noise Strength", Range(0, 10)) = 1
    }

        SubShader
    {

        CGPROGRAM
        #pragma surface surf ToonRamp
        
        float4 _Color;
        sampler2D _MainTex;
        sampler2D _ToonRamp;
        float _Shadow;


        half4 LightingToonRamp(SurfaceOutput s, fixed3 lightDir, fixed atten) {
            half NdotL = dot(s.Normal, lightDir);

            float h = NdotL * 0.5 + 0.5;
            float rh = h;
            float3 ramp = tex2D(_ToonRamp, rh).rgb;

            half4 col;
            col.rgb = s.Albedo * _LightColor0.rgb * ramp;
            col.a = s.Alpha;
            return col;
        }

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            float diff = dot(o.Normal, IN.viewDir);
            float h = diff * 0.5 + _Shadow;
            float2 rh = h;
            o.Albedo = ((tex2D(_ToonRamp, rh) + _Color) * (tex2D(_MainTex, IN.uv_MainTex))).rgb;
        }
        ENDCG

            Pass {

            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal: NORMAL;
                float4 texcoord: TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            float _Outline;
            float4 _OutlineColor;
            sampler2D _Noise;
            float _NoiseStrength;

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 norm = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
                float2 offset = TransformViewToProjection(norm.xy);
                float4 noise = tex2D(_Noise, v.texcoord.xy, 0, 0) * _NoiseStrength;

                o.pos.xy += offset * o.pos.z  * (_Outline );
                o.color = _OutlineColor;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target{
                return i.color;
            }


            ENDCG
        }
    }
        FallBack "Diffuse"
}