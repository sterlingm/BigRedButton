using UnityEngine;
using System.Collections;

namespace UDB
{
	/// <summary>
	/// Add this component with ScreenTrans to play when SceneManager changes to new scene
	/// </summary>
	[AddComponentMenu ("UDB/Screen Transition/Player")]
	public class ScreenTransPlayer : MonoBehaviour, SceneManager.ITransition
	{
		int SceneManager.ITransition.priority { get { return 0; } }

		public ScreenTrans transitionOut;
		public ScreenTrans transitionIn;

		void Start()
		{
			SceneManager.instance.AddTransition (this);
		}

		void OnDestroy ()
		{
			if (SceneManager.instance) {
				SceneManager.instance.RemoveTransition (this);
			}
		}

		IEnumerator SceneManager.ITransition.Out ()
		{
			if (transitionOut) {
				transitionOut.Play ();

				while (transitionOut.IsPlaying) {
					yield return null;
				}
			} else if (transitionIn) {
				transitionIn.Prepare ();
				transitionIn.IsRenderActive = true;
			}
		}

		IEnumerator SceneManager.ITransition.In ()
		{
			if (transitionOut) {
				//wait one render
				yield return new WaitForEndOfFrame (); 
				transitionOut.End ();
			}

			if (transitionIn) {
				transitionIn.Play ();

				while (transitionIn.IsPlaying) {
					yield return null;
				}

				transitionIn.End ();
			}
		}
	}
}