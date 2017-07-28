using UnityEngine;
using System.Collections;

namespace UDB
{
	/// <summary>
	/// Transition from source to destination
	/// </summary>
	[AddComponentMenu ("UDB/Screen Transition/Type - Cross Fade")]
	public class STCrossFade : ScreenTrans
	{
		public SourceType source = SourceType.CameraSnapShot;
		public Texture sourceTexture;

		protected override void OnPrepare ()
		{
			base.OnPrepare ();
			SetSourceTexture (source, sourceTexture);
		}

		protected override void OnUpdate ()
		{
			Material.SetFloat ("_t", CurCurveValue);
		}
	}
}