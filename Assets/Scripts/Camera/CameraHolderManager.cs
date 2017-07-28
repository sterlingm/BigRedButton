using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace UDB
{
	public class CameraHolderManager : MonoBehaviour
	{
		public static CameraHolderManager active;
		public static Dictionary<string, CameraHolderManager> cameraHolderManagers;

		public Camera mainCamera;
		public AudioListener audioListener;

		public bool isLoadingScene = false;

		public bool isActiveScene = false;

		private string curSceneName;

		private GameObject childCamera;

		void Awake ()
		{
			childCamera = gameObject.GetChildWithName ("Main Camera");

			// create sceneController instance if not created yet.
			if (CameraHolderManager.cameraHolderManagers == null) {
				CameraHolderManager.cameraHolderManagers = new Dictionary<string, CameraHolderManager> ();
			}

			// get important components of game object 
			if (mainCamera == null) {
				mainCamera = childCamera.GetComponent<Camera> ();
			}
			if (audioListener == null) {
				audioListener = childCamera.GetComponent<AudioListener> ();
			}

			curSceneName = gameObject.scene.name;
			CameraHolderManager foundManager;
			if (CameraHolderManager.cameraHolderManagers.TryGetValue (curSceneName, out foundManager) == false) {
				Log.Low (curSceneName + ": " + this.GetType ().Name + ": Creating Camera Holder Manager for scene " + curSceneName);
				CameraHolderManager.cameraHolderManagers.Add (curSceneName, this);
			} else {
				Log.Error (curSceneName + ": " + this.GetType ().Name + ": Camera Holder Manager for scene " + curSceneName + " already exists");
			}
		}

		void OnDestroy ()
		{
			CameraHolderManager foundManager;
			if (CameraHolderManager.cameraHolderManagers.TryGetValue (curSceneName, out foundManager)) {
				Log.Low (curSceneName + ": " + this.GetType ().Name + ": Removing Camera Holder Manager for scene " + curSceneName);
				CameraHolderManager.cameraHolderManagers.Remove (curSceneName);
			} else {
				Log.Error (curSceneName + ": " + this.GetType ().Name + ": Cannont remove Camera Holder Manager for scene " + curSceneName + " because it does not  exists");
			}

			if (CameraHolderManager.active == this) {
				CameraHolderManager.active = null;
			}
		}


		public void SetAsActiveScene(bool active)
		{
			isActiveScene = active;

			Log.Low (curSceneName + ": " + this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name  + " : " + isActiveScene);
			if (isActiveScene) {
				if (isLoadingScene && CameraHolderManager.active != null) {
					CameraHolderManager.active.SetAsActiveScene (false);
				}
				CameraHolderManager.active = this;
			}

			mainCamera.enabled = isActiveScene;
			audioListener.enabled = isActiveScene;
		}
	}
}