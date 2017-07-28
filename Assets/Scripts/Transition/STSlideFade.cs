using UnityEngine;
using System.Collections;

namespace UDB
{
	[AddComponentMenu ("UDB/Screen Transition/Type - Slide Fade")]
	public class STSlideFade : ScreenTrans
	{
		public SourceType source = SourceType.CameraSnapShot;
		public Texture sourceTexture;

		public Texture alphaMask;

		public AnimationCurve slideCurve;
		public bool slideCurveNormalized;

		public Anchor anchor = Anchor.Left;

		private Vector4 param;

		protected override void OnPrepare ()
		{
			SetSourceTexture (source, sourceTexture);
			Material.SetTexture ("_AlphaMaskTex", alphaMask);

			//Vector2 anchorPt = GetAnchorPoint(anchor);
			//param.x = anchorPt.x;
			//param.y = anchorPt.y;
		}

		protected override void OnUpdate ()
		{
			Material.SetFloat ("_t", CurCurveValue);
			Vector2 scroll = GetUVScroll (anchor, slideCurve.Evaluate (slideCurveNormalized ? CurTimeNormalized : CurTime));
			Material.SetVector ("_Scroll", scroll);
		}
	}
}