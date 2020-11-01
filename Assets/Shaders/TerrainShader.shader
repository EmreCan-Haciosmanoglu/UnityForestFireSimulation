Shader "Custom/TerrainShader"
{
	Properties
	{
		_GrassTex("Texture", 2D) = "white" {}
		_RoadTex("Texture", 2D) = "white" {}
		_BlendTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _GrassTex;
			float4 _GrassTex_ST;
			sampler2D _RoadTex;
			float4 _RoadTex_ST;
			sampler2D _BlendTex;
			float4 _BlendTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _GrassTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 grassCol = tex2D(_GrassTex, i.uv);

				fixed4 dirtCol = tex2D(_RoadTex, i.uv);
				fixed4 maskCol = tex2D(_BlendTex, float2(i.uv.x / 100.0, i.uv.y / 100.0));

				float mask = maskCol.x;
				fixed4 col = lerp(grassCol, dirtCol, mask);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
