Shader "Ray Shaders/UnlitToon"
{
	Properties
	{
		_Color("Color", Color) = (0.5, 0.65, 1, 1)
		_MainTex("Main Texture", 2D) = "white" {}	
		_DotRange("DotProduct Shadow Range", Range(-1.0, 1.0)) = 0
		_ColorCount("Color Split Count", INT) = 1
	}
	SubShader
	{
		Tags
		{
			"LightMode" = "ForwardBase"
			"PassFlags" = "OnlyDirectional"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;				
				float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _DotRange;
			int _ColorCount;
			float4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);

				return o;
			}
			

			float4 frag (v2f i) : SV_Target
			{
				float3 normal = normalize(i.worldNormal);
				float NdotL = dot(_WorldSpaceLightPos0, normal);
				//float lightIntensity = NdotL > _DotRange ? 1 : 0;
				
				float lightIntensity;
				
				if (NdotL > _DotRange)
				{
					for (int i = 0; i < _ColorCount; i++)
					{
						if (NdotL >=  ((1.0 - _DotRange) / _ColorCount) * i)
						{
							lightIntensity = ((1.0 - _DotRange) / _ColorCount) * (i + 1);
						}
						else 
						{
							break;
						}
					}
				}
				else
				{
					lightIntensity = 0;
				}

				float4 sample = tex2D(_MainTex, i.uv);

				return _Color * sample * lightIntensity;
			}
			ENDCG
		}
	}
}