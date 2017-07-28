using UnityEngine;
using System;

namespace UDB
{
	/// <summary>
	/// Load script from Resources/'path'.prefab if not found on scene, with persistent (default: false)
	/// </summary>
	[AttributeUsage (AttributeTargets.Class, Inherited = true)]
	public class PrefabFromResourceAttribute : Attribute
	{
		public readonly string path;

		public PrefabFromResourceAttribute (string path)
		{
			this.path = path;
		}

		public GameObject InstantiateGameObject ()
		{
			if (String.IsNullOrEmpty (path)) {
				Log.Error ("Path is empty.");
				return null;
			}

			//check if in scene
			GameObject go = GameObject.Find (path);
			if (!go) {
				var resGO = Resources.Load<GameObject> (path);
				if (resGO) {
					go = UnityEngine.Object.Instantiate (resGO) as GameObject;
					go.name = path;

				} else
					Log.Error ("Could not find Prefab \"" + path + "\" on Resources.");
			}

			return go;
		}

		public virtual T Instantiate<T> () where T : MonoBehaviour
		{
			T ret;

			var gameObject = InstantiateGameObject ();

			ret = gameObject.GetComponentInChildren<T> ();
			if (!ret) {
				Log.Warning ("There wasn't a component of type \"" + typeof (T) + "\" inside prefab \"" + path + "\". Creating one.");
				ret = gameObject.AddComponent<T> ();
			}

			return ret;
		}
	}

	/// <summary>
	/// Load from Resources/core.prefab if not found on scene, persistent=true
	/// </summary>
	[AttributeUsage (AttributeTargets.Class, Inherited = true)]
	public class PrefabCoreAttribute : PrefabFromResourceAttribute
	{
		//private static GameObject mCoreGameObject = null;

		public PrefabCoreAttribute () : base ("Core")
		{

		}
	}
}