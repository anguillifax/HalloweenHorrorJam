Shader "Momentum/Transition" {
	Properties {
		_Mode("Mode", Int) = 0
		_MainTex("Texture", 2D) = "white" {}
		_Size("Size", Vector) = (100, 100, 0, 0)
		_Background("Background", Color) = (0, 0, 0, 0)
		_Progress("Progress", Range(0, 1)) = 0.5
		_Position("Center Point", Vector) = (100, 100, 0, 0)
		_Stipple0("Stipple Pattern (row 0)", Vector) = (0, 0, 0, 0)
		_Stipple1("Stipple Pattern (row 1)", Vector) = (0, 0, 0, 0)
		_Stipple2("Stipple Pattern (row 2)", Vector) = (0, 0, 0, 0)
		_Stipple3("Stipple Pattern (row 3)", Vector) = (0, 0, 0, 0)
	}

	SubShader {
		Tags { 
			"RenderType" = "Overlay" 
			"PreviewType" = "Plane"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			// =========================================================
			// Variables
			// =========================================================

			struct VertexOutput {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			int _Mode;
			sampler2D _MainTex;
			int2 _Size;
			fixed4 _Background;
			float _Progress;
			float2 _Position;
			half4 _Stipple0;
			half4 _Stipple1;
			half4 _Stipple2;
			half4 _Stipple3;

			float2 pos;

			// =========================================================
			// Util
			// =========================================================
			
			half GetMatrix(half4 raw, int idx) {
				switch (idx) {
					case 0: return raw.x;
					case 1: return raw.y;
					case 2: return raw.z;
					case 3: return raw.w;
					default: return raw.x;
				}
			}

			/// Anything less than progress is 0. Anything above progress is 1.
			half SmoothSlice(float progress, float lowerbound, float upperbound, float value, float smoothsize) {
				float cutoff = lowerbound + (distance(upperbound, lowerbound) + smoothsize) * progress;
				return max(0.0, smoothstep(cutoff - smoothsize, cutoff, value));
			}

			// =========================================================
			// Fade
			// =========================================================

			half CalcFade() {
				return _Progress;
			}

			// =========================================================
			// Iris
			// =========================================================

			half CalcIris() {
				float d = distance(pos, _Position);
				float max_rad = length(float2(_Size.x, _Size.y));
				return SmoothSlice(pow(1.0 - _Progress, 2.0), 0.0, max_rad, d, 2.0);
			}

			// =========================================================
			// Mosaic
			// =========================================================

			int GetStippleMatrix(int x, int y) {
				switch (y) {
					case 0: return GetMatrix(_Stipple0, x);
					case 1: return GetMatrix(_Stipple1, x);
					case 2: return GetMatrix(_Stipple2, x);
					case 3: return GetMatrix(_Stipple3, x);
					default: return GetMatrix(_Stipple0, 0);
				}
			}

			half CalcStipple(int x, int y) {
				return GetStippleMatrix(x, y) / 16.0;
			}

			half MixVisible(half visibility, half pattern) {
				float pat_vis = pattern * pow(visibility, lerp(0.5, 2.0, visibility));
				return clamp(visibility + pat_vis, 0.0, 1.0);
			}

			half CalcMosaic() {
				float2 raw = floor(pos % 4.0);
				half stipple = CalcStipple(int(raw.x), int(raw.y));
				return MixVisible(_Progress, stipple);
			}

			// =========================================================
			// Arrow
			// =========================================================

			half GetArrowMask() {
				if (pos.x - (2.0 * _Progress - 1.0) * _Size.x < pos.y) {
					return 1.0;
				} else {
					return 0.0;
				}
			}

			half CalcArrowIn() {
				return GetArrowMask();
			}

			half CalcArrowOut() {
				return 1.0 - GetArrowMask();
			}

			// =========================================================
			// Common Functions
			// =========================================================

			VertexOutput vert(appdata_base input) {
				VertexOutput o;
				o.pos = UnityObjectToClipPos(input.vertex);
				o.uv = input.texcoord;
				return o;
			}

			half GetTransitionAlpha() {
				switch (_Mode) {
					case 0: return CalcFade();
					case 1: return CalcIris();
					case 2: return CalcMosaic();
					case 3: return CalcArrowIn();
					case 4: return CalcArrowOut();
					default: return 0.0;
				}
			}

			fixed4 frag(VertexOutput vout) : COLOR {
				pos = vout.uv * _Size;
				half alpha = GetTransitionAlpha();
				return fixed4(_Background.rgb, alpha);
			}

			ENDCG
		}
	}
}
