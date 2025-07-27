Shader "Unlit/OriginalSimpleShader"
{
	Properties
	{
		_AlbedoMap("アルベドマップ", 2D) = "white" {}
		_AlbedoColor("アルベドカラー", color) = (1.0, 1.0, 1.0, 1)
		_Metallic("メタリック", Range(0.0, 1.0)) = 0
		_Smoothness("スムースネス", Range(0.0, 1.0)) = 0.0
		_LightColor("ライトカラー", Color) = (1.0, 1.0, 1.0, 1)
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			Tags{"LightMode" = "ForwardBase"}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 position : TEXCOORD1;
				float3 normal : TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			uniform sampler2D _AlbedoMap;
			uniform float4 _AlbedoColor;
			uniform float _Metallic;
			uniform float _Smoothness;
			uniform float3 _LightColor;

			static const float PI = 3.1415926f;


			//ノンリニア空間からリニア空間に変換
			float3 ConvertToLinear(float3 color) {
				return pow(color, 2.2);
			}

			//リニア空間からノンリニア空間に変換
			float3 ConvertToNoneLinear(float3 color) {
				return pow(color, 1 / 2.2);
			}

			//スムースネスをラフネスに変換
			float SmoothnessToPercetualRoughness(float smoothness) {
				return (1.0f - smoothness);
			}

			//リニア空間のラフネスに変換
			float PercetualRoughnessToRoughness(float percetualRoughness) {
				return percetualRoughness * percetualRoughness;
			}

			//5乗
			float Pow5(float x) {
				return x * x * x * x * x;
			}

			//ラフネル反射
			float3 F_Schlick(float cosA, float3 f0) {
				return f0 + (1.0 - f0) * Pow5(1.0 - cosA);
			}

			//マイクロファセット分布関数 Trowbridge-Reitz (GGX)
			float D_GGX(float NoH, float linearRoughness) {
				float a2 = linearRoughness * linearRoughness;
				float f = (NoH * a2 - NoH) * NoH + 1.0;
				return a2 / (PI * f * f);
			}

			//可視性関数
			float V_SmithGGXCorrelatedFast(float NoV, float NoL, float linearRoughness) {
				float a = linearRoughness;
				float GGXV = NoL * (NoV * (1.0 - a) + a);
				float GGXL = NoV * (NoL * (1.0 - a) + a);
				return 0.5f / (GGXV + GGXL + 1e-5);
			}

			//鏡面反射BRDF(Cook-Torrance approximation)
			float3 SpecularBRDF(float3 specularColor, float NoH, float NoV, float NoL, float LoH, float linearRoughness) {
				float D = D_GGX(NoH, linearRoughness);
				float V = V_SmithGGXCorrelatedFast(NoV, NoL, linearRoughness);
				float3 F = F_Schlick(LoH, specularColor);
				return D * V * F;
			}

			//拡散反射
			float Fd_Lambert() {
				return 1.0 / PI;
			}

			//拡散反射BRDF
			float3 DiffuseBRDF(float3 diffuseColor) {
				return diffuseColor * Fd_Lambert();
			}

			v2f vert(appdata v)
			{
				v2f o;
				//ワールド座標系の頂点データ
				o.position = mul(unity_ObjectToWorld, v.vertex);
				//ワールド座標系の法線ベクトル
				o.normal = UnityObjectToWorldNormal(v.normal);
				//テクスチャ座標の出力
				o.uv = v.uv;
				//ワールド・ビュー・プロジェクション変換
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//アルベドカラーに変換
				float3 albedoColor = ConvertToLinear(tex2D(_AlbedoMap, i.uv) * _AlbedoColor.rgb);
				//拡散反射光カラーに変換
				float3 diffuseColor = lerp(albedoColor.rgb, 0.0f, _Metallic);
				//鏡面反射光カラーに変換
				float3 specularColor = lerp(0.04f, albedoColor.rgb, _Metallic);
				//ラフネスに変換(0.0(ツルツル)～1.0(ザラザラ))
				float percetualRoughness = SmoothnessToPercetualRoughness(_Smoothness);
				//リニア空間のラフネスに変換
				float linearRoughness = PercetualRoughnessToRoughness(percetualRoughness);

				float3 N = normalize(i.normal);
				float3 L = normalize(UnityWorldSpaceLightDir(i.position));
				float3 V = normalize(UnityWorldSpaceViewDir(i.position));
				float3 H = normalize(L + V);
				float3 R = reflect(-V, N);
				//各種ベクトルの内積
				float NoV = abs(dot(N, V));
				float NoL = saturate(dot(N, L));
				float NoH = saturate(dot(N, H));
				float LoV = saturate(dot(L, V));
				float LoH = saturate(dot(L, H));

				//放射照度の計算
				float3 irradiance = NoL * (_LightColor * PI) + 0.5;

				float3 Fd = DiffuseBRDF(diffuseColor);
				float3 Fr = SpecularBRDF(specularColor, NoH, NoV, NoL, LoH, linearRoughness);
				float3 BRDF = (Fd + Fr) * irradiance;

				float3 finalColor = ConvertToNoneLinear(BRDF);
				return fixed4(finalColor, 1.0);
			}
			ENDCG
		}
	}
}
