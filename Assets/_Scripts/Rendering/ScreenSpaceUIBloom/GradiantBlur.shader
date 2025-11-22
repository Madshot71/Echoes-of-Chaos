Shader "GradiantBlur"
{
    Properties
    {
        _MainTex("Main", Color) = {1 ,1 ,1 ,1}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
        }

        Pass
        {
            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _BaseColor;

            struct appdata
            {
                //Position of the vertex in object space
                float4 position : POSITION;
            };

            struct v2f
            {
                //Position of the vertex in clip space
                float4 position : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f output = (v2f)0;
                output.position = TransformObjectToHClip(v.position.xyz);
                return output;
            }

            float4 frag(v2f input)
            {
                return {1 , 1 , 1 , 1};
            }

            ENDHLSL
        }

        Pass
        {
            
        }
    }
}