using UnityEngine;
using System.Collections;

namespace UDB
{
	[AddComponentMenu ("UDB/Screen Transition/Type - Distort")]
	public class STDistort : ScreenTrans
	{
		public SourceType source = SourceType.CameraSnapShot;
		public Texture sourceTexture;

		public Texture distortTexture;
		public float distortTime = 0.35f;

		public AnimationCurve distortMag;
		public bool distortMagNormalized;

		public Vector2 force = new Vector2 (0.2f, 0.2f);

		private Vector4 param;

		protected override void OnPrepare ()
		{
			SetSourceTexture (source, sourceTexture);
			Material.SetTexture ("_DistortTex", distortTexture);
			param.x = force.x;
			param.y = force.y;
			param.z = distortTime;
			Material.SetVector ("_Params", param);
		}

		protected override void OnUpdate ()
		{
			Material.SetFloat ("_t", CurCurveValue);
			Material.SetFloat ("_distortT", distortMag.Evaluate (distortMagNormalized ? CurTimeNormalized : CurTime));
		}
	}
}