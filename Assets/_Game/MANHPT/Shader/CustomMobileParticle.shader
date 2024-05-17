// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Horus/Mobile/Particles"
{
    Properties
    {
        [Toggle]_BillBoard("BillBoard", Range(0, 1)) = 0
        [Toggle]_UseLerpColor("Use Lerp Color", Range(0, 1)) = 0
        [HDR]_Color1("BrightColor", Color) = (1, 1, 1, 1)
        _Color2("DarkColor", Color) = (1, 1, 1, 1)
        [Header(Base)][HDR]_TintColor("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _MainTex("Particle Texture", 2D) = "white" {}
        _Boost ("Boost", Range(1, 10)) = 2
        _Pull("Pull", Range(-10, 10)) = 0
        _Clip("Clip", Range(0,1)) = 0.01
        [Enum(UnityEngine.Rendering.CullMode)]_Cull ("Cull", Range(0, 1)) = 0
        [Toggle]_ZWrite ("ZWrite", Range(0, 1)) = 0
        [Toggle]_EnableFog ("Fog", Range(0,1)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_ZTest ("ZTest", Range(0, 8)) = 4
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcFactor ("Source Factor", float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstFactor ("Destination Factor", float) = 10
        [Space(50)]_Line("----------------------------------------------------------------------------------------------", float) = 0
        [Header(Bloom)][Toggle]_Bloom ("Bloom", Range(0,1)) = 0
        [HDR]_BloomColor("Bloom Color", Color) = (1, 1, 1, 1)
        _Exposure("Exposure", Range(0, 10)) = 1
        [Toggle]_Blur("Blur", Range(0,1)) = 0
        _BlurStrength("Blur Strength", Range(0, 10)) = 1
        [Space(50)]_Line("----------------------------------------------------------------------------------------------", float) = 0
        [Header(Smoothness)][Toggle]_Dissolve("Dissolve", Range(0, 1)) = 0
        _DissolveColor("Dissolve Color", Color) = (1, 1, 1, 1)
        _DissolveTexture("Dissolve Texture", 2D) = "white" {}
        _DissolveTextureVelocity("Dissolve Texture Velocity", Vector) = (0, 0, 0, 0)
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    sampler2D _MainTex;
    sampler2D _DissolveTexture;
    float4 _MainTex_ST;
    float4 _DissolveTexture_ST;
    float4 _MainTex_TexelSize;
    float4 _TintColor;
    float4 _BloomColor;
    float4 _DissolveColor;
    float4 _DissoveTextureVelocity;
    float4 _Color1;
    float4 _Color2;
    float _Boost;
    float _Clip;
    float _Pull;
    float _Exposure;
    float _BlurStrength;

    struct appdata_t
    {
        float4 position : POSITION;
        float4 texcoord : TEXCOORD0;
        float4 dissolveUV : TEXCOORD1;
        fixed4 color : COLOR;
    };

    struct v2f
    {
        float4 position : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        float2 dissolveUV : TEXCOORD1;
        fixed4 color : COLOR;

        #if _ENABLEFOG_ON
        UNITY_FOG_COORDS(2)
        #endif
    };

    v2f vert(appdata_t v)
    {
        v2f o;

        #if _BILLBOARD_ON
        float3 cameraPosition = mul(unity_WorldToObject, _WorldSpaceCameraPos);
        float3 cameraDirection = normalize(cameraPosition - v.position.xyz);

        float4 origin = float4(0, 0, 0, 1);
        float4 worldOrigin = mul(UNITY_MATRIX_M, origin);
        float4 viewOrigin = mul(UNITY_MATRIX_V, worldOrigin);
        float4 worldToViewTranslation = viewOrigin - worldOrigin;


        float4 worldPos = mul(UNITY_MATRIX_M, v.position);
        float4 viewPos = worldPos + worldToViewTranslation;
        viewPos.xyz += cameraDirection * _Pull;

        float4 clipPos = mul(UNITY_MATRIX_P, viewPos);

        o.position = clipPos;
        #else
        o.position = UnityObjectToClipPos(v.position);
        #endif


        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
        o.dissolveUV = v.dissolveUV * _DissolveTexture_ST.xy + _DissolveTexture_ST.zw + _DissoveTextureVelocity.xy *
            _Time.y;
        o.color = v.color;
        #if _ENABLEFOG_ON
        UNITY_TRANSFER_FOG(o, o.position);
        #endif
        return o;
    }

    fixed4 frag(v2f i) : SV_Target
    {
        float4 combineColor;
        #if _USELERPCOLOR_ON
        combineColor = lerp(_Color1, _Color2, i.color.a);
        #else
        combineColor = _TintColor;
        #endif
        float4 mainTexture = tex2D(_MainTex, i.texcoord);
        float4 col;
        #if _DISSOLVE_ON
        float4 vertexColor = i.color;
        vertexColor.a = 1;
        col = _Boost * vertexColor * combineColor * mainTexture;
        float4 dissolveTexture = tex2D(_DissolveTexture, i.dissolveUV);
        float dissolveValue = 1 - i.color.a;
        float dissolveValue1 = saturate(dissolveValue + 0.1);
        float rim = saturate(step(dissolveValue, dissolveTexture.r) - step(dissolveValue1, dissolveTexture.r));
        float4 rimColor = _DissolveColor * rim * mainTexture;
        col += rimColor;
        clip(dissolveTexture.r - dissolveValue);
        #else
        col = _Boost * i.color * combineColor * mainTexture;
        #endif

        #if _BLOOM_ON
        #if _BLUR_ON
        float3 gaussianBlur =
            tex2D(_MainTex, i.texcoord + _MainTex_TexelSize.xy * float2(-1, -1) * _BlurStrength) +
            tex2D(_MainTex, i.texcoord + _MainTex_TexelSize.xy * float2(-1, 1) * _BlurStrength) +
            tex2D(_MainTex, i.texcoord + _MainTex_TexelSize.xy * float2(1, -1) * _BlurStrength) +
            tex2D(_MainTex, i.texcoord + _MainTex_TexelSize.xy * float2(1, 1) * _BlurStrength) +

            tex2D(_MainTex, i.texcoord + _MainTex_TexelSize.xy * float2(0, -1) * _BlurStrength) * 2 +
            tex2D(_MainTex, i.texcoord + _MainTex_TexelSize.xy * float2(0, 1) * _BlurStrength) * 2 +
            tex2D(_MainTex, i.texcoord + _MainTex_TexelSize.xy * float2(-1, 0) * _BlurStrength) * 2 +
            tex2D(_MainTex, i.texcoord + _MainTex_TexelSize.xy * float2(1, 0) * _BlurStrength) * 2 +

            mainTexture * 4;
        gaussianBlur = gaussianBlur * 0.0625 * _BloomColor;
        col.rgb += gaussianBlur;
        #else
        col.rgb += mainTexture * _BloomColor;
        #endif
        col.rgb = 1 - exp(-col.rgb * _Exposure);
        #endif
        clip(col.a - _Clip);

        #if _ENABLEFOG_ON
        UNITY_APPLY_FOG(i.fogCoord, col);
        #endif
        return col;
    }
    ENDCG

    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"
        }
        Blend [_SrcFactor] [_DstFactor]
        Cull [_Cull]
        Lighting off
        ZWrite [_ZWrite]
        ZTest [_ZTest]
        Fog
        {
            Mode off
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma shader_feature _ENABLEFOG_ON
            #pragma multi_compile_fog
            #pragma shader_feature _BLOOM_ON
            #pragma shader_feature _BLUR_ON
            #pragma shader_feature _DISSOLVE_ON
            #pragma shader_feature _USELERPCOLOR_ON
            #pragma shader_feature _BILLBOARD_ON
            ENDCG
        }
    }
}