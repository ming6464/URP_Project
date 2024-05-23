Shader "Tutorials/Benjamin/shader_code_1"
{
    Properties
    {
        _Color("color 1", color) = (1,1,1,1)
        _Texture("Texture 1", 2D) = "white"{}
        _AnimateXY("Animate X Y", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass  
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            //property
            float _t;
            sampler2D _Texture;
            float4 _Texture_ST;
            float4 _AnimateXY;
            //
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0; 
            };
            
            struct v2f
            {
                
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv,_Texture);
                o.uv += _AnimateXY.xy * _Time.y;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                fixed4  texColor = tex2D(_Texture,i.uv);
                return texColor;
            }
            ENDCG
        }
    }
}
