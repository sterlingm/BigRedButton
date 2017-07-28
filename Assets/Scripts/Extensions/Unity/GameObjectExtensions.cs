using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace UDB
{
	public static class GameObjectExtensions
	{
		#region Public Methods and Operators

		public static T GetComponentInChild<T>(this GameObject parent, string childName) where T : Component
		{
			var t = TryGetComponentInChild<T>(parent, childName);
			if (t == null) throw new KeyNotFoundException();
			return t;
		}

		public static T TryGetComponentInChild<T>(this GameObject parent, string childName) where T : Component
		{
			return GetComponentsInChild<T>(parent, childName).FirstOrDefault();
		}

		public static IEnumerable<T> GetComponentsInChild<T>(this GameObject parent, string childName) where T : Component
		{
			return parent.transform.GetComponentsInChildren<T>().Where(t => t.name == childName);
		}

		/// <summary>
		///   Instantiates a new game object and parents it to this one.
		///   Resets position, rotation and scale and inherits the layer.
		/// </summary>
		/// <param name="parent">Game object to add the child to.</param>
		/// <returns>New child.</returns>
		public static GameObject AddChild(this GameObject parent)
		{
			return parent.AddChild("New Game Object");
		}

		/// <summary>
		///   Instantiates a new game object and parents it to this one.
		///   Resets position, rotation and scale and inherits the layer.
		/// </summary>
		/// <param name="parent">Game object to add the child to.</param>
		/// <param name="name">Name of the child to add.</param>
		/// <returns>New child.</returns>
		public static GameObject AddChild(this GameObject parent, string name)
		{
			var go = AddChild(parent, (GameObject)null);
			go.name = name;
			return go;
		}

		/// <summary>
		///   Instantiates a prefab and parents it to this one.
		///   Resets position, rotation and scale and inherits the layer.
		/// </summary>
		/// <param name="parent">Game object to add the child to.</param>
		/// <param name="prefab">Prefab to instantiate.</param>
		/// <returns>New prefab instance.</returns>
		public static GameObject AddChild(this GameObject parent, GameObject prefab)
		{
			var go = prefab != null ? GameObject.Instantiate(prefab) : new GameObject();
			if (go == null || parent == null) {
				return go;
			}

			var transform = go.transform;
			transform.SetParent(parent.transform);
			transform.Reset();
			go.layer = parent.layer;
			return go;
		}

		/// <summary>
		///   Destroys all children of a object.
		/// </summary>
		/// <param name="gameObject">Game object to destroy all children of.</param>
		public static void DestroyChildren(this GameObject gameObject)
		{
			foreach (var child in gameObject.GetChildren())
			{
				// Hide immediately.
				child.SetActive(false);

				if (Application.isEditor && !Application.isPlaying) {
					GameObject.DestroyImmediate(child);
				} else {
					GameObject.Destroy(child);
				}
			}
		}

		/// <summary>
		///   Selects all ancestors (parent, grandparent, etc.) of a game object.
		/// </summary>
		/// <param name="gameObject">Game object to select the ancestors of.</param>
		/// <returns>All ancestors of the object.</returns>
		public static IEnumerable<GameObject> GetAncestors(this GameObject gameObject)
		{
			var parent = gameObject.transform.parent;

			while (parent != null) {
				yield return parent.gameObject;
				parent = parent.parent;
			}
		}

		/// <summary>
		///   Selects all ancestors (parent, grandparent, etc.) of a game object,
		///   and the game object itself.
		/// </summary>
		/// <param name="gameObject">Game object to select the ancestors of.</param>
		/// <returns>
		///   All ancestors of the game object,
		///   and the game object itself.
		/// </returns>
		public static IEnumerable<GameObject> GetAncestorsAndSelf(this GameObject gameObject)
		{
			yield return gameObject;

			foreach (var ancestor in gameObject.GetAncestors()) {
				yield return ancestor;
			}
		}

		/// <summary>
		///   Selects all children of a game object.
		/// </summary>
		/// <param name="gameObject">Game object to select the children of.</param>
		/// <returns>All children of the game object.</returns>
		public static IEnumerable<GameObject> GetChildren(this GameObject gameObject)
		{
			return (from Transform child in gameObject.transform select child.gameObject);
		}


		/// <summary>
		///   Selects child with given name of a game object.
		/// </summary>
		/// <param name="gameObject">Game object to select the child of.</param>
		/// <param name="name">Name of the child to search for.</param>
		/// <returns>Child of the game object with given name.</returns>
		public static GameObject GetChildWithName(this GameObject gameObject, string name)
		{
			foreach (GameObject go in gameObject.GetChildren()) {
				
				if (String.Compare(go.name, name) == 0) {
					return go;
				}
			}
			return null;
		}

		/// <summary>
		///   Selects all descendants (children, grandchildren, etc.) of a game object.
		/// </summary>
		/// <param name="gameObject">Game object to select the descendants of.</param>
		/// <returns>All descendants of the game object.</returns>
		public static IEnumerable<GameObject> GetDescendants(this GameObject gameObject)
		{
			foreach (var child in gameObject.GetChildren())
			{
				yield return child;

				// Depth-first.
				foreach (var descendant in child.GetDescendants()) {
					yield return descendant;
				}
			}
		}

		/// <summary>
		///   Selects all descendants (children, grandchildren, etc.) of a
		///   game object, and the game object itself.
		/// </summary>
		/// <param name="gameObject">Game object to select the descendants of.</param>
		/// <returns>
		///   All descendants of the game object,
		///   and the game object itself.
		/// </returns>
		public static IEnumerable<GameObject> GetDescendantsAndSelf(this GameObject gameObject)
		{
			yield return gameObject;

			foreach (var descendant in gameObject.GetDescendants()) {
				yield return descendant;
			}
		}

		/// <summary>
		///   Gets the hierarchy root of the game object.
		/// </summary>
		/// <param name="gameObject">Game object to get the root of.</param>
		/// <returns>Root of the specified game object.</returns>
		public static GameObject GetRoot(this GameObject gameObject)
		{
			var root = gameObject.transform;

			while (root.parent != null) {
				root = root.parent;
			}

			return root.gameObject;
		}

		/// <summary>
		///   Indicates whether the a game object is an ancestor of another one.
		/// </summary>
		/// <param name="gameObject">Possible ancestor.</param>
		/// <param name="descendant">Possible descendant.</param>
		/// <returns>
		///   <c>true</c>, if the game object is an ancestor of the other one, and
		///   <c>false</c> otherwise.
		/// </returns>
		public static bool IsAncestorOf(this GameObject gameObject, GameObject descendant)
		{
			return gameObject.GetDescendants().Contains(descendant);
		}

		/// <summary>
		///   Indicates whether the a game object is a descendant of another one.
		/// </summary>
		/// <param name="gameObject">Possible descendant.</param>
		/// <param name="ancestor">Possible ancestor.</param>
		/// <returns>
		///   <c>true</c>, if the game object is a descendant of the other one, and
		///   <c>false</c> otherwise.
		/// </returns>
		public static bool IsDescendantOf(this GameObject gameObject, GameObject ancestor)
		{
			return gameObject.GetAncestors().Contains(ancestor);
		}


		/// <summary>
		///   Gets the component of type <typeparamref name="T" /> if the game object has one attached,
		///   and adds and returns a new one if it doesn't.
		/// </summary>
		/// <typeparam name="T">Type of the component to get or add.</typeparam>
		/// <param name="gameObject">Game object to get the component of.</param>
		/// <returns>
		///   Component of type <typeparamref name="T" /> attached to the game object.
		/// </returns>
		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
		{
			return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
		}

		/// <summary>
		///   Returns the full path of a game object, i.e. the names of all
		///   ancestors and the game object itself.
		/// </summary>
		/// <param name="gameObject">Game object to get the path of.</param>
		/// <returns>Full path of the game object.</returns>
		public static string GetPath(this GameObject gameObject)
		{
			return
				gameObject.GetAncestorsAndSelf()
					.Reverse()
					.Aggregate(string.Empty, (path, go) => path + "/" + go.name)
					.Substring(1);
		}

		/// <summary>
		///   Sets the layer of the game object.
		/// </summary>
		/// <param name="gameObject">Game object to set the layer of.</param>
		/// <param name="layerName">Name of the new layer.</param>
		public static void SetLayer(this GameObject gameObject, string layerName)
		{
			var layer = LayerMask.NameToLayer(layerName);
			gameObject.layer = layer;
		}

		/// <summary>
		/// Assigns a layer to this GameObject and all its children recursively.
		/// </summary>
		/// <param name="gameObject">The GameObject to start at.</param>
		/// <param name="layer">The layer to set.</param>
		public static void AssignLayerToHierarchy(this GameObject gameObject, int layer)
		{
			var transforms = gameObject.GetComponentsInChildren<Transform>();
			for (var i = 0; i < transforms.Length; i++) {
				transforms[i].gameObject.layer = layer;
			}
		}

		/// <summary>
		/// When <see cref="UnityEngine.Object.Instantiate(UnityEngine.Object)"/> is called on a prefab named
		/// "Original", the resulting instance will be named "Original(Clone)". This method changes the name
		/// back to "Original" by stripping everything after and including the first "(Clone)" it finds. If no
		/// "(Clone)" is found, the name is left unchanged.
		/// </summary>
		/// <param name="gameObject">The GameObject to change the name of.</param>
		public static void StripCloneFromName(this GameObject gameObject)
		{
			gameObject.name = gameObject.GetNameWithoutClone();
		}

		/// <summary>
		/// When <see cref="UnityEngine.Object.Instantiate(UnityEngine.Object)"/> is called on a prefab named
		/// "Original", the resulting instance will be named "Original(Clone)". This method returns the name
		/// without "(Clone)" by stripping everything after and including the first "(Clone)" it finds. If no
		/// "(Clone)" is found, the name is returned unchanged.
		/// </summary>
		/// <param name="gameObject">The GameObject to return the original name of.</param>
		public static string GetNameWithoutClone(this GameObject gameObject)
		{
			var gameObjectName = gameObject.name;

			var clonePartIndex = gameObjectName.IndexOf("(Clone)", StringComparison.Ordinal);
			if (clonePartIndex == -1) {
				return gameObjectName;
			}

			return gameObjectName.Substring(0, clonePartIndex);
		}
			
		#endregion
	}
}