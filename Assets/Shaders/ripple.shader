// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ImageEffect/Ripple"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Speed", Range(1, 60)) = 5
        _Power ("Poewr", Range(0, 1)) = .005
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
             
            #include "UnityCG.cginc"
 
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
 
 
            //
            float wave(float2 uv, float2 emitter, float speed, float phase){
                float dst = distance(uv, emitter);
                return pow((0.5 + 0.5 * sin(dst * phase - _Time.y * speed)), 2.0);
            }
 
 
            //
            sampler2D _MainTex;
            float _Speed;
            float _Power;
 
            fixed4 frag (v2f i) : SV_Target
            {
                float2 position = i.uv;
 
                float w = wave(position, float2(0.5, 0.5), _Speed, 200.0);
                w += wave(position, float2(0.6, 0.11), _Speed, 20.0);
                w += wave(position, float2(0.9, 0.6), _Speed, 90.0);
                w += wave(position, float2(0.1, 0.84), _Speed, 150.0);
                w += wave(position, float2(0.32, 0.81), _Speed, 150.0);
                w += wave(position, float2(0.16, 0.211), _Speed, 150.0);
                w += wave(position, float2(0.39, 0.46), _Speed, 150.0);
                w += wave(position, float2(0.51, 0.484), _Speed, 150.0);
                w += wave(position, float2(0.732, 0.91), _Speed, 150.0);
 
                w *= 0.116 * _Power;
                i.uv += w;
                return tex2D(_MainTex, i.uv);
 
            }
            ENDCG
        }
    }
}