using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDB
{
	/// <summary>
	/// Simple behaviour to load a scene on Start
	/// </summary>
	[AddComponentMenu ("UDB/Core/SceneAutoLoad")]
	public class SceneAutoLoad : MonoBehaviour
	{
		[SerializeField]
		SceneAssetPath scene;

		[SerializeField]
		float delay = 0f;

		[SerializeField]
		bool destroyAfter;

		IEnumerator Start ()
		{
			if (delay > 0f) {
				yield return new WaitForSeconds (delay);
			}

			SceneManager.instance.LoadScene (scene.name);

			if (destroyAfter) {
				Destroy (gameObject);
			}
		}
	}
}