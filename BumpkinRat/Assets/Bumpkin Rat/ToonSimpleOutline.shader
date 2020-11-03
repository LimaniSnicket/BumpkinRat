Shader "Bumpkin Rat/Toon"
{
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Diffuse", 2D) = "white"{}
        _ToonRamp("Toon Ramp", 2D) = "white"{}
        _Shadow("Shadow Strength", Range(0, 0.55)) = 0.5

        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _Outline("Outline Width", Range(0, 0.001)) = 0.0001
    }

        SubShader
    {
            ZWrite Off
        Tags{
            "Queue" = "Transparent"
        }

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        float4 _OutlineColor;
        float _Outline;

        struct Input
        {
            float2 uv_MainTex;
        };

        void vert(inout appdata_full v) {
            v.vertex.xyz += v.normal * _Outline;
        }

        void surf(Input IN, inout SurfaceOutput o) {
            o.Emission = _OutlineColor.rgb;
        }

        ENDCG

        ZWrite On
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
    }
        FallBack "Diffuse"
}