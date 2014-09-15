Shader "Unlit/Premultiplied Colored Gray"
{
    Properties
    {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
        _Gray("Gray Color", Color) = (0.3,0.3,0.3,1)
    }

    SubShader
    {
        LOD 200

        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            AlphaTest Off
            Fog { Mode Off }
            Offset -1, -1
            ColorMask RGB
            Blend One OneMinusSrcAlpha
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Gray;

            struct appdata_t
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.color = v.color;
                o.texcoord = v.texcoord;
                return o;
            }

            half4 frag (v2f i) : COLOR
            {
                fixed4 col;  
                if (i.color.b < 0.001 && i.color.g >0.999 &&  i.color.r >0.999)  
                {  
                    col = tex2D(_MainTex, i.texcoord);  
                    float grey = dot(col.rgb, _Gray.rgb);              
                    col.rgb = float3(grey, grey, grey);  
                }  
                else  
                {  
                    col = tex2D(_MainTex, i.texcoord) * i.color;  
                }  
                return col; 
            }
            ENDCG
        }
    }
    
    SubShader
    {
        LOD 100

        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }
        
        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            AlphaTest Off
            Fog { Mode Off }
            Offset -1, -1
            ColorMask RGB
            Blend One OneMinusSrcAlpha 
            ColorMaterial AmbientAndDiffuse
            
            SetTexture [_MainTex]
            {
                Combine Texture * Primary
            }
        }
    }
}
