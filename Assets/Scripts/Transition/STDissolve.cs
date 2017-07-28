using UnityEngine;
using System.Collections;

namespace UDB
{
	[AddComponentMenu ("UDB/Screen Transition/Type - Dissolve")]
	public class STDissolve : ScreenTrans
	{
		public SourceType source = SourceType.CameraSnapShot;
		public Texture sourceTexture;

		public Texture dissolveTexture;
		public AnimationCurve dissolvePower;
		public bool dissolvePowerNormalized;

		public Texture emissionTexture;
		public float emissionThickness = 0.03f;

		private Vector4 param;

		protected override void OnPrepare ()
		{
			SetSourceTexture (source, sourceTexture);
			Material.SetTexture ("_DissolveTex", dissolveTexture);
			Material.SetTexture ("_EmissionTex", emissionTexture);
			param.y = emissionThickness;
		}

		protected override void OnUpdate ()
		{
			Material.SetFloat ("_t", CurCurveValue);
			param.x = dissolvePower.Evaluate (dissolvePowerNormalized ? CurTimeNormalized : CurTime);
			Material.SetVector ("_Params", param);
		}
	}
}