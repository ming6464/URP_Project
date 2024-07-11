Shader "ShaderCode/Stencil_C_Shader"
{
    Properties
    {
        [IntRange] _StencilID ("Stencil ID",Range(0,255)) = 0
    }
    SubShader
    {
        Tags 
        { 
             "RenderType" = "Opaque"
             "Queue" = "Geometry-1"
             "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            Blend Zero One
            ZWrite Off
            Stencil
            {
                Ref 1
                Comp NotEqual
                Pass Keep
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            
            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata a)
            {
                v2f o;
                o.vertex = a.vertex;
                return o;
            }

            fixed4 frag(v2f v) : SV_Target
            {
                return fixed4(1,1,1,1);
            }
            
            ENDCG
        }
    }
}
