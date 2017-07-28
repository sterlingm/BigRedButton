using UnityEngine;
using System.Collections;
using System.Reflection;

namespace UDB
{
	/// <summary>
	/// Transition from source to solid color
	/// </summary>
	[AddComponentMenu ("UDB/Screen Transition/Type - Pixelate Transition")]
	public class STPixelate : ScreenTrans
	{
		public SourceType source = SourceType.CameraSnapShot;
		public Texture sourceTexture;

		public AnimationCurve pixelCurve;
		public bool pixelationNormalized;

		protected override void OnPrepare ()
		{
			base.OnPrepare ();
			SetSourceTexture (source, sourceTexture);
		}

		protected override void OnUpdate ()
		{
			float t = pixelationNormalized ? CurTimeNormalized : CurTime;

			float cellSize = pixelCurve.Evaluate(t);

			Material.SetFloat("_CellSize", cellSize);
		}
	}
}