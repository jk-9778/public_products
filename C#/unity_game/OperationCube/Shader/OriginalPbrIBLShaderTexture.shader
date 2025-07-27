Shader "Unlit/OriginalSimplePbrIBLShaderTexture"
{
	Properties
	{
		_AlbedoMap("アルベドマップ", 2D) = "white" {}
		_MetallicSmoothnessMap("メタリックスムースネスマップ", 2D) = "white" {}
		_NormalMap("法線マップ", 2D) = "bump" {}
		_AoMap("アンビエントオクルージョンマップ", 2D) = "white" {}

		_LightColor("ライトカラー", Color) = (1.0, 1.0, 1.0, 1)

		_IBLDiffuseMap("IBL拡散反射マップ", Cube) = "" {}
		_IBLSpecularMap("IBL鏡面反射マップ", Cube) = "" {}
		_IBLBRDFMap("IBL_BRDFマップ", 2D) = "white" {}
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
				float4 tangent : TANGENT;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 position : TEXCOORD1;
				float3 normal : TEXCOORD2;
				float3 tangent : TEXCOORD3;
				float3 binormal : TEXCOORD4;
				float4 vertex : SV_POSITION;
			};

			//アルベドマップ
			uniform sampler2D _AlbedoMap;
			//メタリックスムースネスマップ
			uniform sampler2D _MetallicSmoothnessMap;
			//法線マップ
			uniform sampler2D _NormalMap;
			//アンビエントオクルージョンマップ
			uniform sampler2D _AoMap;

			uniform float3 _LightColor;

			UNITY_DECLARE_TEXCUBE(_IBLDiffuseMap);
			UNITY_DECLARE_TEXCUBE(_IBLSpecularMap);
			uniform sampler2D _IBLBRDFMap;
			//IBL鏡面反射マップのMIPMAP数
			#define SPECCUBE_LOD_STEPS 9

			static const float PI = 3.1415926;


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

			//IBLの計算
			float3 ImageBasedLighting(float3 N, float3 R, float NoV, float3 diffuseColor, float3 specularColor, float perceptualRoughness) {
				float3 Ld = UNITY_SAMPLE_TEXCUBE(_IBLDiffuseMap, N).rgb * diffuseColor * Fd_Lambert();
				//float3 Ld = UNITY_SAMPLE_TEXCUBE_LOD(_IBLSpecularMap, N, SPECCUBE_LOD_STEPS - 0.5f).rgb * diffuseColor * Fd_Lambert();
				float3 Lld = UNITY_SAMPLE_TEXCUBE_LOD(_IBLSpecularMap, R, perceptualRoughness * SPECCUBE_LOD_STEPS).rgb;
				float3 Ldfg = tex2D(_IBLBRDFMap, float2(NoV, perceptualRoughness)).xyz;
				float3 Lr = (specularColor * Ldfg.x + Ldfg.y) * Lld;
				return Ld + Lr;
			}

			v2f vert(appdata v)
			{
				v2f o;
				//ワールド座標系の頂点データ
				o.position = mul(unity_ObjectToWorld, v.vertex);
				//ワールド座標系の法線ベクトル
				o.normal = UnityObjectToWorldNormal(v.normal);
				//ワールド座標系の接ベクトル
				o.tangent = UnityObjectToWorldNormal(v.tangent.xyz);
				//従法線ベクトルを求める
				o.binormal = cross(o.normal, o.tangent) * v.tangent.w * unity_WorldTransformParams.w;
				//テクスチャ座標の出力
				o.uv = v.uv;
				//ワールド・ビュー・プロジェクション変換
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//アルベドカラーに変換
				float3 albedoColor = ConvertToLinear(tex2D(_AlbedoMap, i.uv));
				//メタリックスムースネス
				float4 metaricSmoothness = tex2D(_MetallicSmoothnessMap, i.uv);
				//アンビエントオクルージョン
				float4 ao = tex2D(_AoMap, i.uv);

				//拡散反射光カラーに変換
				float3 diffuseColor = lerp(albedoColor.rgb, 0.0f, metaricSmoothness.r);
				//鏡面反射光カラーに変換
				float3 specularColor = lerp(0.04f, albedoColor.rgb, metaricSmoothness.r);
				//ラフネスに変換(0.0(ツルツル)～1.0(ザラザラ))
				float percetualRoughness = SmoothnessToPercetualRoughness(metaricSmoothness.a);
				//リニア空間のラフネスに変換
				float linearRoughness = PercetualRoughnessToRoughness(percetualRoughness);

				//各種ベクトルを求める
				float3x3 TBN = float3x3(normalize(i.tangent), normalize(i.binormal), normalize(i.normal));
				float3 N = mul(UnpackNormal(tex2D(_NormalMap, i.uv)), TBN);
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
				float3 irradiance = NoL * (_LightColor * PI);

				float3 Fd = DiffuseBRDF(diffuseColor);
				float3 Fr = SpecularBRDF(specularColor, NoH, NoV, NoL, LoH, linearRoughness);
				float3 BRDF = (Fd + Fr) * irradiance;

				//イメージベースドライティング
				float3 IBL = ImageBasedLighting(N, R, NoV, diffuseColor, specularColor, percetualRoughness);

				//最終的なカラーの計算
				float3 finalColor = (BRDF + IBL) * ao;
				//ノンリニアカラーに変換して出力
				finalColor = ConvertToNoneLinear(finalColor);
				return fixed4(finalColor, 1.0);
			}
			ENDCG
		}
	}
}
