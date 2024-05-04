Shader "Unlit/Shader1"
{
    Properties
    {
//        _MainTex ("Texture", 2D) = "white" {}
        _Value ("Value",Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert 
            // #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct MeshData
            { // mesh data trên mỗi vertex
                float4 vertex : POSITION; // vị trí vertex
                float2 uv : TEXCOORD0; // uv tọa độ
                float2 uv1 : TEXCOORD1;
                float3 normals : NORMAL;
                float4 color : COLOR;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            // sampler2D _MainTex; // khai báo biến
            // float4 _MainTex_ST;
            float _Value;

            v2f vert (MeshData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            // fixed4 frag (v2f i) : SV_Target
            // {
            //     // sample the texture
            //     // fixed4 col = tex2D(_MainTex, i.uv);
            //     // apply fog
            //     UNITY_APPLY_FOG(i.fogCoord, col);
            //     return col;
            // }
            ENDCG
        }
    }
}