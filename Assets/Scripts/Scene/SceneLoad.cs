using UnityEngine;

namespace UDB
{
	/// <summary>
	/// Simple behaviour to load a scene via Execute, useful for UI, or some timeline editor
	/// </summary>
	[AddComponentMenu ("UDB/Core/SceneLoad")]
	public class SceneLoad : MonoBehaviour
	{
		[SerializeField]
		SceneAssetPath scene;

		public void Execute ()
		{
			SceneManager.instance.LoadScene (scene.name);
		}
	}
}