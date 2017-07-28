using UnityEngine;

using System.Collections.Generic;
using System.Reflection;

namespace UDB
{
	[AddComponentMenu ("UDB/Screen Transition/Render")]
	[RequireComponent (typeof (Camera))]
	public class ScreenTransRender : MonoBehaviour
	{
		private List<ScreenTrans> renderTransList;

		private string curSceneName;

		void Awake ()
		{
			curSceneName = gameObject.scene.name;
			renderTransList = new List<ScreenTrans> ();
		}

		void OnRenderImage (RenderTexture source, RenderTexture destination)
		{
			if (renderTransList.Count > 0) {
				for (int i = 0; i < renderTransList.Count; i++) {
					if (renderTransList [i]) {
						renderTransList [i].OnRenderImage (source, destination);
					}
				}
			} else {
				Graphics.Blit (source, destination);
			}
		}

		public void AddRender (ScreenTrans trans)
		{
			Log.Low (curSceneName + ": " + this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name);

			if (renderTransList.Contains (trans) == false) {
				renderTransList.Add (trans);
			}

			enabled = true;
		}

		public void RemoveRender (ScreenTrans trans)
		{
			Log.Low (curSceneName + ": " + this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name);

			renderTransList.Remove (trans);

			if (renderTransList.Count == 0) {
				enabled = false;
			}
		}
	}
}