using UnityEngine;
using System.Collections;

namespace UDB
{
	/// <summary>
	/// Use this if you want to have your first scene as a boot-up, then immediately load the next.
	/// </summary>
	[AddComponentMenu ("UDB/Core/SceneInitializer")]
	public class SceneInitializer : MonoBehaviour
	{
		[SerializeField]
		string toScene = "start";
		//the scene to load to once initScene is finish

		IEnumerator Start ()
		{
			//wait for one update to ensure all initialization has occured
			yield return new WaitForFixedUpdate ();

			SceneManager.instance.LoadScene (toScene);
		}
	}
}