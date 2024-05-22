Shader "Tutorials/Benjamin/shader_code_1"
{
    Properties
    {
        _t("value color",float) = 1
        _Color("color 1", color) = (1,1,1,1)
        _Texture("Texture 1", 2D) = "white"{}
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
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 Getcolor(sampler2D sampler,float2 uv)
            {
                float4 color = tex2D(sampler, uv);
                return color;
            }

            float4 frag (v2f i) : SV_Target
            {
                fixed4  texColor = tex2D(_Texture,i.uv);
                fixed4 col = fixed4(i.uv,0,0);
                return Getcolor(_Texture,i.uv);
            }
            ENDCG
        }
    }
}
