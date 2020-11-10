Shader "BumpkinRat/WoodGrain"
{
    Properties{
        _WoodRGB("RGB Mask", 2D) = "red" {}
        _BaseColor("Base Color", Color) = (0,0,1,1)
        _OuterColor("Outer Color", Color) = (0,1,0,1)
        _InnerColor("Inner Color", Color) = (1,0,0,1)
        _RepeatX("X Tiling", Range(1, 20)) = 10
        _RepeatY("Y Tiling", Range(1, 20)) = 10
    }

        SubShader
        {

            Pass {

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        sampler2D _WoodRGB;
        sampler2D _WoodRGB_ST;

        float4 _BaseColor;
        float4 _OuterColor;
        float4 _InnerColor;

        float _RepeatX;
        float _RepeatY;

        float4 getColor(float4 original) {
            return ((original.r * _InnerColor) * .33) + ((original.g * _OuterColor) * .33) + ((original.b * _BaseColor) * .33);
        }

        struct appdata
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            fixed4 diffuse : COLOR0;
            float4 vertex : SV_POSITION;
        };

        v2f vert(appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            return o;
        }

        fixed4 frag(v2f i) : SV_Target
        {
            // sample the texture
            float4 col = tex2D(_WoodRGB, float2(i.uv.x * _RepeatX, i.uv.y * _RepeatY));
            float4 finalColor = getColor(col);

            return finalColor;
        }



        ENDCG
            }


    }

    

        FallBack "Diffuse"
}
