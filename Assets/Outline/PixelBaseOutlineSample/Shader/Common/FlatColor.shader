Shader "Hidden/FlatColor"
{
	Properties {
		_MainTex ("Main", 2D) = "white" {}
	}

	SubShader
	{ 
		Tags{ "RenderType" = "FlatColor"  }
		// Blend SrcAlpha OneMinusSrcAlpha
		//for command buffer transparent
		Pass
		{
			Cull off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 color = tex2D(_MainTex,i.uv);
				 if (color.r <= 0.000001)
				 	color = 0;
				 else
				 	color = 1;
				return color;
			}
			ENDCG
		}

		//zTest off
		Pass
		{
			ZTest off ZWrite off Cull off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 color = tex2D(_MainTex,i.uv);
				 if (color.r <= 0.000001)
				 	color = 0;
				 else
				 	color = 1;
				return color;
			}
			ENDCG
		}

	}

	//for replace shader
	SubShader
	{ 
		Tags{ "RenderType" = "Opaque"  }

		Pass
		{
			cull off
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{				
				return 1;
			}
			ENDCG

		}
	}
	FallBack Off
}

		//check edge
		// Pass
		// {
		// 	CGPROGRAM
		// 	#pragma vertex vert
		// 	#pragma fragment frag
		// 	#include "UnityCG.cginc"
			
		// 	sampler2D _MainTex;
		// 	fixed2 _MainTex_TexelSize;

		// 	struct v2f
		// 	{
		// 		float4 vertex : SV_POSITION;
		// 		float2 uv : TEXCOORD0;
		// 		fixed2 taps[4] : TEXCOORD1;

		// 	};
			
		// 	v2f vert (appdata_base v)
		// 	{
		// 		v2f o;
		// 		o.vertex = UnityObjectToClipPos(v.vertex);
		// 		o.uv = v.texcoord.xy;

		// 		o.taps[0] = o.uv + _MainTex_TexelSize * fixed2(1, 1);
		// 		o.taps[1] = o.uv - _MainTex_TexelSize * fixed2(1, 1);;
		// 		o.taps[2] = o.uv + _MainTex_TexelSize * fixed2(1, -1);
		// 		o.taps[3] = o.uv - _MainTex_TexelSize * fixed2(1, -1);
		// 		return o;
		// 	}
			
		// 	fixed4 frag (v2f i) : COLOR
		// 	{				
		// 		fixed4 color = tex2D(_MainTex, i.uv);
				
		// 		fixed rb = tex2D(_MainTex, i.taps[0]).x;   // right buttom
		// 		fixed lt = tex2D(_MainTex, i.taps[1]).x;   // left top
		// 		fixed rt = tex2D(_MainTex, i.taps[2]).x;   // right top 
		// 		fixed lb = tex2D(_MainTex, i.taps[3]).x;   // left buttom

		// 		fixed total = rb + lt + rt + lb;
		// 		fixed weight = abs(color.r * 100 - total);
		// 		color = clamp(1, 0, weight);
	
		// 		return color;
		// 	}
		// 	ENDCG
		// }