Shader "Custom/Cross Fade"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _TexStart ("Start Texture ", 2D) = "white" {}
        _TexEnd ("End Texture", 2D) = "white" {}
        _Fade ("Fade", Range(0, 1)) = 0
        _Alpha ("Alpha", Range(0, 1)) = 1
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] _FlipStart("Flip Start Texture", Float) = 0
        [MaterialToggle] _FlipEnd("Flip End Texture", Float) = 0
    }
   
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector"="True" 
            "RenderType" = "Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "ForceNoShadowCasting"="True"
        }
        Cull Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4    _MainTex_ST;
            sampler2D _TexStart;
            fixed4    _TexStart_ST;
            sampler2D _TexEnd;
            fixed4    _TexEnd_ST;
            float     _Fade;
            float     _Alpha;
            fixed4    _Color;
            float     _FlipStart;
            float     _FlipEnd;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color    : COLOR;
            };

            struct v2f
            {
                float4 vertex    : POSITION;
                float2 texcoord  : TEXCOORD0;
                fixed4 color    : COLOR;
            };

            v2f vert(appdata_t IN)
            {
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
            }
           
            fixed4 frag (v2f i) : COLOR
            {
                float2 start_uv = float2( i.texcoord.x, i.texcoord.y );
                float2 end_uv = float2( i.texcoord.x, i.texcoord.y );
                if ( _FlipStart != 0 )
                    start_uv = float2( 1.0 - i.texcoord.x, i.texcoord.y );
                if ( _FlipEnd != 0 )
                    end_uv = float2( 1.0 - i.texcoord.x, i.texcoord.y );
                fixed4 texStart = tex2D(_TexStart, start_uv);
                fixed4 texEnd = tex2D(_TexEnd, end_uv);
                fixed4 crosstex = ( texStart * texStart.a * (1.0-_Fade) ) + ( texEnd * texEnd.a * _Fade );
                return (crosstex * i.color) * _Alpha;
            }
            ENDCG
        }
    }
    Fallback off
}