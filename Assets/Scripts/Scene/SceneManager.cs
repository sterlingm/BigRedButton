using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace UDB
{
	[AddComponentMenu ("UDB/Core/SceneManager")]
	public class SceneManager : SingletonBehaviour<SceneManager, MonoBehaviour>
	{
		public enum Mode
		{
			Single = 0,
			Additive = 1
		}

		public interface ITransition
		{
			/// <summary>
			/// Higher priority executes first.
			/// </summary>
			int priority { get; }

			IEnumerator Out ();

			IEnumerator In ();
		}

		[Tooltip ("Use additive if you want to start the game with a root scene, and having one scene to append/replace as you load new scene.")]
		[SerializeField]
		public Mode mode = Mode.Additive;


		[Tooltip ("Used for Additive Mode. Check if you want to use a loading scene.")]
		[SerializeField]
		public bool useLoadingScene = false;

		//used for additive mode
		[Tooltip ("Used for Additive Mode. Name of loading scene.")]
		[SerializeField]
		public string loadingSceneName = "Loading";


		[Tooltip ("Used for Additive Mode. Check if you want to use a loading scene.")]
		[SerializeField]
		public bool useRootScene = false;

		[Tooltip ("Used for Additive Mode. Name of root scene.")]
		[SerializeField]
		public string rootSceneName;
		private Scene rootScene;

		private Scene curScene;

		private List<ITransition> transitions;

		//added scene to current one, filled by AddScene(), cleared out when a new scene is loaded via LoadScene
		private List<Scene> scenesAdded;

		private Queue<string> scenesToAdd;
		private Coroutine sceneAddRout;

		//scenes to remove via UnloadAddedScene
		private Queue<Scene> scenesToRemove;

		private Coroutine sceneRemoveRout;

		private string sceneToLoadInLoading;
		private string sceneLoading;

		public Mode SceneLoadingMode {
			get { return mode; }
			set {
				if (IsLoading) {
					Log.Error ("Current scene is still loading, can't set mode.");
					return;
				}
				mode = value;
			}
		}
			
		public int CurrentLevel { get; private set; }

		public Scene CurrentScene { get { return curScene; } }

		public bool IsLoading { get; private set; }

		public static string RootSceneName { get { return SceneManager.instance.rootSceneName; } }

		public delegate void OnSceneCallback ();

		public delegate void OnSceneBoolCallback (bool b);

		public delegate void OnSceneStringCallback (string nextScene);

		public delegate void OnSceneDataCallback (Scene scene);

		/// <summary>
		/// Called just before the transition to change scene, useful for things like locking input, cancelling processes, etc.
		/// </summary>
		public event OnSceneCallback SceneChangeStartCallback;

		/// <summary>
		/// Called after transition (e.g. load screen, save states); and before unloading current scene, and then load new scene.
		/// </summary>
		public event OnSceneStringCallback SceneChangeCallback;

		/// <summary>
		/// Called after new scene is loaded, before transition in
		/// </summary>
		public event OnSceneCallback SceneChangePostCallback;

		/// <summary>
		/// Called after a new scene is added via AddScene
		/// </summary>
		public event OnSceneDataCallback SceneAddedCallback;

		/// <summary>
		/// Called after a scene has unloaded via UnloadAddedScene
		/// </summary>
		public event OnSceneDataCallback SceneRemovedCallback;

		protected override void OnInstanceInit ()
		{
			if (useRootScene) {
				rootScene = UnitySceneManager.GetActiveScene ();
				rootSceneName = rootScene.name;
			}
			curScene = UnitySceneManager.GetActiveScene ();

			transitions = new List<ITransition> ();

			scenesAdded = new List<Scene> ();
			scenesToAdd = new Queue<string> ();

			scenesToRemove = new Queue<Scene> ();

			DontDestroyOnLoad (this);
		}

		public void SetCurrentSceneAsRoot()
		{
			rootScene =  UnitySceneManager.GetActiveScene ();
			rootSceneName = rootScene.name;
		}

		public void AddTransition (ITransition trans)
		{
			Log.Low (this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name);

			//only add if it doesn't exist
			if (transitions.Contains (trans) == false) {
				bool isAdded = false;

				for (int i = 0; i < transitions.Count; i++) {
					if (trans.priority > transitions [i].priority) {
						transitions.Insert (i, trans);
						isAdded = true;
						break;
					}
				}

				if (isAdded == false) {
					transitions.Add (trans);
				}
			}
		}

		public void RemoveTransition (ITransition trans)
		{
			Log.Low (this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name);
			transitions.Remove (trans);
		}

		public void LoadScene (string scene)
		{
			Log.Low (this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name + " : " + scene);

			//can't allow this
			if (IsLoading) {
				Log.Error ("Current scene is still loading, can't load: " + scene);
				return;
			}
				
			LoadSceneMode loadMode = LoadSceneMode.Single;
			bool unloadCurrent = false;

			//check if we are loading additive
			if (SceneLoadingMode == Mode.Additive) {
				loadMode = LoadSceneMode.Additive;
				unloadCurrent = true;

				bool isNotLoadingScene = curScene.name != loadingSceneName;

				if (useLoadingScene && isNotLoadingScene) {
					sceneToLoadInLoading = scene;
					scene = loadingSceneName;

					StartCoroutine (DoLoadLoadingScene (scene, loadMode, unloadCurrent));
					return;
				}
			}
	        
			StartCoroutine (DoLoadScene (scene, loadMode, unloadCurrent, false));
		}

		/// <summary>
		/// Load back root. This can be used to reset the entire game.
		/// </summary>
		public void LoadRoot ()
		{
			Log.Low (this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name);

			LoadScene (rootScene.name);
		}

		public void Reload ()
		{
			Log.Low (this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name);

			if (string.IsNullOrEmpty (curScene.name) == false) {
				LoadScene (curScene.name);
			}
		}

		/// <summary>
		/// Add a scene to the current scene. Listen via sceneAddedCallback to know when it's added
		/// </summary>
		public void AddScene (string sceneName)
		{
			Log.Low (this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name + " ADD " + sceneName);

			//can't allow this
			if (IsLoading) {
				Log.Error ("Current scene is still loading, can't add: " + sceneName);
				return;
			}

			scenesToAdd.Enqueue (sceneName);

			if (sceneAddRout == null) {
				sceneAddRout = StartCoroutine (DoAddScene ());
			}
		}

		/// <summary>
		/// Unload a scene loaded via AddScene
		/// </summary>
		public void UnloadAddedScene (string sceneName)
		{
			//can't allow this
			if (IsLoading) {
				Log.Error ("Current scene is still loading, can't remove: " + sceneName);
				return;
			}

			for (int i = 0; i < scenesAdded.Count; i++) {
				if (scenesAdded [i].name.IsEqual(sceneName)) {
					scenesToRemove.Enqueue (scenesAdded [i]);

					scenesAdded.RemoveAt (i);

					if (sceneRemoveRout == null) {
						
						foreach (Scene scene in scenesToRemove) {
							Log.Low (this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name + " REMOVING " + scene.name);
						}

						sceneRemoveRout = StartCoroutine (DoUnloadScene ());
					}

					return;
				}
			}

			//check the queue
			if (scenesToAdd.Contains (sceneName)) {
				//reconstruct the queue excluding the sceneName
				var newSceneQueue = new Queue<string> ();
				while (scenesToAdd.Count > 0) {
					var s = scenesToAdd.Dequeue ();
					if (s != sceneName) {
						newSceneQueue.Enqueue (s);
					}
				}
				scenesToAdd = newSceneQueue;
			}
		}

		/// <summary>
		/// Unloads all added scenes loaded via AddScene
		/// </summary>
		public void UnloadAddedScenes ()
		{
			for (int i = 0; i < scenesAdded.Count; i++) {
				scenesToRemove.Enqueue (scenesAdded [i]);
			}

			ClearAddSceneData ();

			if (sceneRemoveRout == null) {

				foreach (Scene scene in scenesToRemove) {
					Log.Low (this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name + " REMOVING " + scene.name);
				}

				sceneRemoveRout = StartCoroutine (DoUnloadScene ());
			}
		}

		public void UnloadUnnecessaryScenes(string toScene)
		{
			for (int i = 0; i < scenesAdded.Count; i++) {
				if (scenesAdded [i].name != toScene) {
					scenesToRemove.Enqueue (scenesAdded [i]);
				}
			}

			ClearAddSceneData ();

			if (sceneRemoveRout == null) {

				foreach (Scene scene in scenesToRemove) {
					Log.Low (this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name + " REMOVING " + scene.name);
				}

				sceneRemoveRout = StartCoroutine (DoUnloadScene ());
			}
		}

		public void SwitchActiveScene()
		{
			SceneController foundSceneController;
			if (SceneController.sceneControllers.TryGetValue (sceneLoading, out foundSceneController)) {
				foundSceneController.ActivateScene ();
			} else {
				Log.Low (this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name + " COULDNT FIND LOADING SCENE: " + sceneLoading);
			}
		}

		private void ClearAddSceneData ()
		{
			scenesAdded.Clear ();
			scenesToAdd.Clear ();

			if (sceneAddRout != null) {
				StopCoroutine (sceneAddRout);
				sceneAddRout = null;
			}
		}

		private bool IsRootScene(Scene scene)
		{
			if (useRootScene && rootScene != null) {
				return scene == rootScene;
			}
			return false;
		}

		private bool IsNotRootScene(Scene scene)
		{
			if (useRootScene && rootScene != null) {
				return scene != rootScene;
			}
			return true;
		}

		IEnumerator DoLoadLoadingScene (string toScene, LoadSceneMode mode, bool unloadCurrent)
		{
			scenesToAdd.Enqueue (loadingSceneName);

			if (sceneAddRout == null) {
				sceneAddRout = StartCoroutine (DoAddScene ());
			}

			while (sceneAddRout != null) {
				yield return null;
			}

			StartCoroutine (DoLoadScene (toScene, mode, unloadCurrent, true));
		}
			
		IEnumerator DoLoadScene (string toScene, LoadSceneMode mode, bool unloadCurrent, bool isLoadingScene)
		{
			sceneLoading = toScene;

			IsLoading = true;

			//about to change scene
			if (SceneChangeStartCallback != null) {
				SceneChangeStartCallback ();
			}

			//play out transitions
			for (int i = 0; i < transitions.Count; i++) {
				yield return transitions [i].Out ();
			}

			//wait for scene add to finish
			while (sceneAddRout != null) {
				yield return null;
			}

			//unload added scenes
			UnloadUnnecessaryScenes(toScene);

			//wait for scene remove to finish
			while (sceneRemoveRout != null) {
				yield return null;
			}

			//scene is about to change
			if (SceneChangeCallback != null) {
				SceneChangeCallback (toScene);
			}

			bool doLoad = true;

			if (mode == LoadSceneMode.Additive) {

				// unload scene if it is not the root scene
				if (unloadCurrent && IsNotRootScene(curScene)) {
					var sync = UnitySceneManager.UnloadSceneAsync (curScene);

					if (sync != null) {
						while (sync.isDone == false) {
							yield return null;
						}
					}
				}

				//load only if it doesn't exist
				doLoad = UnitySceneManager.GetSceneByName (toScene).IsValid () == false;
			}

			//load
			if (doLoad) {
				var sync = UnitySceneManager.LoadSceneAsync (toScene, mode);

				//something went wrong
				if (sync == null) {
					IsLoading = false;
					yield break;
				}

				while (sync.isDone == false) {
					yield return null;
				}

				Log.Low (this.GetType ().Name + " at DoLoadScene: LOADED " + toScene);

			} else {
				yield return null;
			}

			curScene = UnitySceneManager.GetSceneByName (toScene);
			UnitySceneManager.SetActiveScene (curScene);

			if (SceneChangePostCallback != null) {
				SceneChangePostCallback ();
			}

			//play in transitions
			for (int i = 0; i < transitions.Count; i++) {
				yield return transitions [i].In ();
			}

			IsLoading = false;

			if (useLoadingScene && isLoadingScene) {
				SceneController loadingSceneController;
				if (SceneController.sceneControllers.TryGetValue (loadingSceneName, out loadingSceneController)) {
					loadingSceneController.SetSceneToLoad (sceneToLoadInLoading);
				}
			}
		}

		IEnumerator DoAddScene ()
		{
			while (scenesToAdd.Count > 0) {
				//wait for scene removes to finish
				while (sceneRemoveRout != null) {
					yield return null;
				}

				var sceneName = scenesToAdd.Dequeue ();

				var sync = UnitySceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Additive);

				while (sync.isDone == false) {
					yield return null;
				}


				Log.Low (this.GetType ().Name + " at DoAddScene: ADDED " + sceneName);

				var sceneAdded = UnitySceneManager.GetSceneByName (sceneName);
				scenesAdded.Add (sceneAdded);

				if (SceneAddedCallback != null) {
					SceneAddedCallback (sceneAdded);
				}
			}
			sceneAddRout = null;
		}

		IEnumerator DoUnloadScene ()
		{
			while (scenesToRemove.Count > 0) {
				var scene = scenesToRemove.Dequeue ();

				var sync = UnitySceneManager.UnloadSceneAsync (scene);

				while (sync.isDone == false) {
					yield return null;
				}

				if (SceneRemovedCallback != null) {
					SceneRemovedCallback (scene);
				}
			}

			sceneRemoveRout = null;
		}
	}
}