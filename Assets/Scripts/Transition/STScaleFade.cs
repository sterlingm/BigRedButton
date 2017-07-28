using UnityEngine;
using System.Collections;

namespace UDB
{
	[AddComponentMenu ("UDB/Screen Transition/Type - Scale Fade")]
	public class STScaleFade : ScreenTrans
	{
		public SourceType source = SourceType.CameraSnapShot;
		public Texture sourceTexture;

		public Texture alphaMask;

		public AnimationCurve scaleCurveX;
		public AnimationCurve scaleCurveY;
		public bool scaleCurveNormalized;

		public Anchor anchor = Anchor.Center;

		private Vector4 param;

		protected override void OnPrepare ()
		{
			SetSourceTexture (source, sourceTexture);
			Material.SetTexture ("_AlphaMaskTex", alphaMask);
			Vector2 anchorPt = GetAnchorPoint (anchor);
			param.x = anchorPt.x;
			param.y = anchorPt.y;
		}

		protected override void OnUpdate ()
		{
			Material.SetFloat ("_t", CurCurveValue);
			float t = scaleCurveNormalized ? CurTimeNormalized : CurTime;
			param.z = scaleCurveX.Evaluate (t);
			param.w = scaleCurveY.Evaluate (t);
			Material.SetVector ("_Params", param);
		}
	}
}