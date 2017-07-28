using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace UDB
{
	public class SceneController : MonoBehaviour
	{
		public static SceneController active;

		public static Dictionary<string, SceneController> sceneControllers;

		public bool isLoadingScene = false;

		public bool isActiveScene = false;

		public bool isSplashScene = false;

		private string sceneToLoad;

		private string curSceneName;

		private CameraHolderManager sceneCameraHolder;

		void Awake ()
		{
			// create sceneController instance if not created yet.
			if (SceneController.sceneControllers == null) {
				SceneController.sceneControllers = new Dictionary<string, SceneController> ();
			}

			curSceneName = gameObject.scene.name;

			SceneController foundController;
			if (SceneController.sceneControllers.TryGetValue (curSceneName, out foundController) == false) {
				Log.Low (curSceneName + ": " + this.GetType ().Name + ": Creating Scene Controller for scene " + curSceneName);
				SceneController.sceneControllers.Add (curSceneName, this);
			} else {
				Log.Error (curSceneName + ": " + this.GetType ().Name + ": Scene Controller for scene " + curSceneName + " already exists");
			}
		}

		// Use this for initialization
		void Start ()
		{
			SceneManager.instance.SceneAddedCallback += OnSceneAddedCallback;
			SceneManager.instance.SceneChangePostCallback += OnSceneChangePostCallback;

			CameraHolderManager foundManager;
			if (CameraHolderManager.cameraHolderManagers.TryGetValue (curSceneName, out foundManager)) {
				Log.None (curSceneName + ": " + this.GetType ().Name + ": Camera Holder Manager for scene " + curSceneName + " FOUND");
				sceneCameraHolder = foundManager;
			} else {
				Log.Error (curSceneName + ": " + this.GetType ().Name + ": Camera Holder Manager for scene " + curSceneName + " cannot be found");
			}

			// set as active SceneController if the static active instance is null or if this is a loading scene SceneController
			if (SceneController.active == null) {
				ActivateScene ();
			}
		}

		void OnDestroy ()
		{
			Log.Low (curSceneName + ": " + this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name);

			SceneController foundController;
			if (SceneController.sceneControllers.TryGetValue (curSceneName, out foundController)) {
				Log.Low (curSceneName + ": " + this.GetType ().Name + ": Removing Scene Controller for scene " + curSceneName);
				SceneController.sceneControllers.Remove (curSceneName);
			} else {
				Log.Error (curSceneName + ": " + this.GetType ().Name + ": Cannont remove Scene Controller for scene " + curSceneName + " because it does not  exists");
			}

			if (SceneController.active == this) {
				SceneController.active = null;
			}

			if (SceneManager.instance != null) {
				SceneManager.instance.SceneAddedCallback -= OnSceneAddedCallback;
				SceneManager.instance.SceneChangePostCallback -= OnSceneChangePostCallback;

				SceneManager.instance.SwitchActiveScene ();
			}
		}

		void OnSceneAddedCallback (Scene scene)
		{
			Log.Low (curSceneName + ": " + this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name);
			if (isLoadingScene && scene.name.IsEqual(sceneToLoad)) {
				SceneManager.instance.LoadScene (scene.name);
			}
		}

		void OnSceneChangePostCallback ()
		{
			Log.Low (curSceneName + ": " + this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name);
			if (isLoadingScene) {

				if (isActiveScene == false) {
					ActivateScene ();
				}
			}
		}
			
		public void SetSceneToLoad (string newSceneToLoad)
		{
			if (isLoadingScene) {
				Log.Low (curSceneName + ": " + this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name);

				sceneToLoad = newSceneToLoad;

	//			SceneManager.instance.AddScene (sceneToLoad);
			}
		}

		public void ActivateScene()
		{
			Log.Low (curSceneName + ": " + this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name);

			if (SceneController.active != null) {
				print (SceneController.active);


				SceneController.active.SetAsActiveScene (false);
			}
			SceneController.active = this;
			SetAsActiveScene (true);
		}

		private void SetAsActiveScene(bool active)
		{
			isActiveScene = active;

			Log.Low (curSceneName + ": " + this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name  + " : " + isActiveScene);
			try {
				sceneCameraHolder.SetAsActiveScene (isActiveScene);
			} catch {
				Log.Error (curSceneName + ": " + this.GetType ().Name + ": Cannot activate scene because Camera Holder Manager for scene " + curSceneName + " cannot be found");
			}
		}
	}
}