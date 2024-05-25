Shader "ConvertToCode/Dissolve"
{
    Properties
    {
        _BaseTexture ("Base Texture", 2D) = "white" {}
        _AlphaMap("Alpha Map", 2D) = "white"{}
        _NoiseTexture("Noise Texture",2D) = "white"{}
        _AlphaThreshold("Alpha Threshold",float) = 0
        _Thickness("Thickness",Float) = 0
        [HDR]_Color1("Color 1",Color) = (0,0,0)
        [HDR]_Color2("Color 2",Color) = (0,0,0)
        
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
            //-properties

            sampler2D _BaseTexture;
            sampler2D _AlphaMap;
            sampler2D _NoiseTexture;
            float _AlphaThreshold;
            float _Thickness;
            half4 _Color1;
            half4 _Color2;
            
            //-properties
            // make fog work
            #include "UnityCG.cginc"

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

            fixed4 frag (v2f i) : SV_Target
            {
                
                float2 uv = i.uv;
                float4 baseTexture = tex2D(_BaseTexture,uv);
                float redNosie = tex2D(_NoiseTexture,uv).r;
                float redAlphaMap = step(0.2,tex2D(_AlphaMap,uv).r);
                if(_AlphaThreshold <= 0)
                {
                    _AlphaThreshold = 0;
                    _Thickness = 0;
                }
                
                float value2StepSmooth = _AlphaThreshold + _Thickness;
                float stepNoise_1 = saturate(step(_AlphaThreshold,redNosie));
                float stepNoise_2 = saturate(step(redNosie,value2StepSmooth));
                float oneMinusStepNoise_2 = 1 - stepNoise_2;
                float stepSmoothColor = smoothstep(_AlphaThreshold,value2StepSmooth,redNosie);
                
                float4 color = lerp(_Color1,_Color2,stepSmoothColor) * stepNoise_2;
                float4 baseColor = (oneMinusStepNoise_2 * baseTexture + color) * redAlphaMap;

                float clipValue = ((stepNoise_1 * redAlphaMap) - _AlphaThreshold) <= 0 ? -1 : 1;
                clip(clipValue);
                return baseColor;
            }
            ENDCG
        }
    }
}
