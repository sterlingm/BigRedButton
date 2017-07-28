using UnityEngine;
using System.Collections;
using System.Reflection;

namespace UDB
{
	/// <summary>
	/// Transition from source to solid color
	/// </summary>
	[AddComponentMenu ("UDB/Screen Transition/Type - Fade To Color")]
	public class STFadeToColor : ScreenTrans
	{
		public Color color = Color.black;

		protected override void OnPrepare ()
		{
			base.OnPrepare ();
			Material.SetColor ("_Color", color);
		}

		protected override void OnUpdate ()
		{
			Material.SetFloat ("_t", CurCurveValue);
		}
	}
}