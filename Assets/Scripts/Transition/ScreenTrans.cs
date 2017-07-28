using UnityEngine;
using System.Collections;
using System.Reflection;

namespace UDB
{
	public abstract class ScreenTrans : MonoBehaviour
	{
		public enum SourceType
		{
			CameraSnapShot,
			Texture
		}

		public enum ToType
		{
			Camera,
			Texture
		}

		public enum Anchor
		{
			BottomLeft,
			Left,
			TopLeft,
			Top,
			TopRight,
			Right,
			BottomRight,
			Bottom,
			Center
		}

		/// <summary>
		/// Main - use Camera.main
		/// All - use the camera with the highest depth
		/// Target - use cameraTarget
		/// </summary>
		public enum CameraType
		{
			Main,
			All,
			Target,
		}

		public Shader shader;

		public float delay = 1.0f;
		public AnimationCurve curve = new AnimationCurve ();

		//if true, curve is based on 0-1 within delay
		public bool curveNormalized;

		public CameraType cameraType = CameraType.Main;
		public Camera cameraTarget;

		public ToType target = ToType.Camera;

		//for ToType.Texture
		public Texture targetTexture;
	            
		private float curTime;
		private Material material;

		private RenderTexture renderTexture;
		private Vector2 renderTextureScreenSize;

		[HideInInspector]
		public string curSceneName;

		private Coroutine playRout;

		private bool isRenderActive;
		private ScreenTransRender transRender;

		public float CurTime { get { return curTime; } }

		public float CurTimeNormalized { get { return curTime / delay; } }

		/// <summary>
		/// Returns current curve value based on current time
		/// </summary>
		public float CurCurveValue { get { return curve.Evaluate (curveNormalized ? CurTimeNormalized : CurTime); } }

		public bool IsPrepared { get; private set; }

		public bool IsPlaying { get { return playRout != null; } }

		public bool IsRenderActive {
			get { return isRenderActive; }

			set {
				isRenderActive = value;
				if (isRenderActive) {
					if (!transRender) {
						transRender = GetPlayer ();
						if (transRender) {
							transRender.AddRender (this);
						}
					}
				} else if (transRender) {
					transRender.RemoveRender (this);
					transRender = null;
				}
			}
		}

		public Material Material {
			get {
				if (material == null) {
					material = new Material (shader);
				}
				return material;
			}
		}

		private RenderTexture RenderTexture {
			get {
				Vector2 screenSize = new Vector2 (Screen.width, Screen.height);
				if (renderTexture == null || renderTextureScreenSize != screenSize) {
					if (renderTexture) {
						DestroyImmediate (renderTexture);
					}

					renderTexture = new RenderTexture (Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);

					renderTextureScreenSize = screenSize;
				}

				return renderTexture;
			}
		}

		protected virtual void Awake ()
		{
			UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
		}

		protected virtual void Start()
		{
			curSceneName = gameObject.scene.name;
		}

		protected virtual void OnDisable ()
		{
			Log.Low (curSceneName + ": " + this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name);
			End ();
		}

		protected virtual void OnDestroy ()
		{
			if (material) {
				DestroyImmediate (material);
			}

			UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		/// <summary>
		/// Depending on cameraType, set cameraTarget to the appropriate reference.
		/// </summary>
		public Camera GetCameraTarget ()
		{
			switch (cameraType) {
				case CameraType.Main:
					return Camera.main;
				case CameraType.Target:
					return cameraTarget ? cameraTarget : Camera.main;
				case CameraType.All:
					Camera[] cams = Camera.allCameras;
					float maxDepth = float.MinValue;
					int maxIndex = -1;
					for (int i = 0; i < cams.Length; i++) {
						if (cams [i].depth > maxDepth) {
							maxDepth = cams [i].depth;
							maxIndex = i;
						}
					}
					return maxIndex == -1 ? Camera.main : cams [maxIndex];
			}

			return null;
		}

		/// <summary>
		/// Certain transitions need to prepare before Play can be called (e.g. cross fade)
		/// </summary>
		public void Prepare ()
		{
			Log.Low (curSceneName + ": " + this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name);

			if (IsPrepared == false) {
				curTime = 0f;

				OnPrepare ();
				OnUpdate (); //do one update

				IsPrepared = true;
			}
		}

		/// <summary>
		/// Call this to start rendering. Note: will continue to render after duration, you will need to call End to stop rendering.
		/// </summary>
		public void Play ()
		{
			Log.Low (curSceneName + ": " + this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name);

			if (playRout != null) {
				StopCoroutine (playRout);
			}

			playRout = StartCoroutine (DoPlay ());
		}

		/// <summary>
		/// Call this to stop rendering.  Note: make sure to call this after Play
		/// </summary>
		public void End ()
		{
			Log.Low (curSceneName + ": " + this.GetType ().Name + " at " + MethodBase.GetCurrentMethod ().Name);

			curTime = delay;
			IsPrepared = false;
			IsRenderActive = false;

			if (playRout != null) {
				StopCoroutine (playRout);
				playRout = null;
			}

			OnFinish ();

			if (renderTexture) {
				DestroyImmediate (renderTexture);
				renderTexture = null;
			}
		}

		protected void SetSourceTexture (SourceType source, Texture sourceTexture)
		{
			switch (source) {
				case SourceType.CameraSnapShot:
					switch (cameraType) {
						case CameraType.Main:
							Material.SetTexture ("_SourceTex", CameraSnapshot (Camera.main));
							break;
						case CameraType.Target:
							Material.SetTexture ("_SourceTex", CameraSnapshot (cameraTarget ? cameraTarget : Camera.main));
							break;
						case CameraType.All:
							Material.SetTexture ("_SourceTex", CameraSnapshot (UnityHelper.GetAllCameraDepthSorted ()));
							break;
					}
					break;

				case SourceType.Texture:
					Material.SetTexture ("_SourceTex", sourceTexture);
					break;
			}
		}

		protected Vector2 GetUVScroll (Anchor anchor, float t)
		{
			Vector2 ret = Vector2.zero;
			switch (anchor) {
				case Anchor.TopLeft:
					ret.x = -1f + t * 2f;
					ret.y = -1f + (1f - t) * 2f;
					break;
				case Anchor.Top:
					ret.x = 0f;
					ret.y = -1f + (1f - t) * 2f;
					break;
				case Anchor.TopRight:
					ret.x = -1f + (1f - t) * 2f;
					ret.y = -1f + (1f - t) * 2f;
					break;
				case Anchor.Right:
					ret.x = -1f + (1f - t) * 2f;
					ret.y = 0f;
					break;
				case Anchor.BottomRight:
					ret.x = -1f + (1f - t) * 2f;
					ret.y = -1f + t * 2f;
					break;
				case Anchor.Bottom:
					ret.x = 0f;
					ret.y = -1f + t * 2f;
					break;
				case Anchor.BottomLeft:
					ret.x = -1f + t * 2f;
					ret.y = -1f + t * 2f;
					break;
				case Anchor.Left:
					ret.x = -1f + t * 2f;
					ret.y = 0f;
					break;
				default:
					ret.x = 0f;
					ret.y = 0f;
					break;
			}

			return ret;
		}

		protected Vector2 GetAnchorPoint (Anchor anchor)
		{
			Vector2 ret = Vector2.zero;
			switch (anchor) {
				case Anchor.TopLeft:
					ret.x = -1f;
					ret.y = 1f;
					break;
				case Anchor.Top:
					ret.x = 0f;
					ret.y = 1f;
					break;
				case Anchor.TopRight:
					ret.x = 1f;
					ret.y = 1f;
					break;
				case Anchor.Right:
					ret.x = 1f;
					ret.y = 0f;
					break;
				case Anchor.BottomRight:
					ret.x = 1f;
					ret.y = -1f;
					break;
				case Anchor.Bottom:
					ret.x = 0f;
					ret.y = -1f;
					break;
				case Anchor.BottomLeft:
					ret.x = -1f;
					ret.y = -1f;
					break;
				case Anchor.Left:
					ret.x = -1f;
					ret.y = 0f;
					break;
				default:
					ret.x = 0f;
					ret.y = 0f;
					break;
			}

			ret = ret / 2f + new Vector2 (0.5f, 0.5f);

			return ret;
		}


	#region internal

		void OnSceneLoaded (UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
		{
			if (mode == UnityEngine.SceneManagement.LoadSceneMode.Single) {
				//restore render?
				if (isRenderActive) {
					if (!transRender) {
						transRender = GetPlayer ();
						if (transRender) {
							//re-establish
							transRender.AddRender (this); 
						} else {
							//abort
							End (); 
						}
					}
				}
			}
		}

		IEnumerator DoPlay ()
		{
			Prepare ();

			IsRenderActive = true;

			if (!transRender) {
				End ();
				yield break;
			}
	        
			var wait = new WaitForEndOfFrame ();

			while (curTime < delay) {
				yield return wait;

				curTime = Mathf.Min (curTime + Time.smoothDeltaTime, delay);

				OnUpdate ();
			}

			playRout = null;
		}

		ScreenTransRender GetPlayer ()
		{
			Camera cam = GetCameraTarget ();
			if (cam) {
				var player = cam.GetComponent<ScreenTransRender> ();
				if (!player) {
					player = cam.gameObject.AddComponent<ScreenTransRender> ();
				}
				return player;
			}
			return null;
		}

		Texture CameraSnapshot (Camera cam)
		{
			//RenderTexture lastRT = RenderTexture.active;
			//RenderTexture.active = renderTexture;
			//GL.Clear(false, true, Color.clear);
			if (!cam.targetTexture) {
				cam.targetTexture = renderTexture;
				cam.Render ();
				cam.targetTexture = null;
			}
			//RenderTexture.active = lastRT;

			return renderTexture;
		}

		Texture CameraSnapshot (Camera[] cams)
		{
			//RenderTexture lastRT = RenderTexture.active;
			//RenderTexture.active = renderTexture;
			//GL.Clear(false, true, Color.clear);

			//NOTE: assumes cams are in the correct depth order
			for (int i = 0; i < cams.Length; i++) {
				if (!cams [i].targetTexture) {
					cams [i].targetTexture = renderTexture;
					cams [i].Render ();
					cams [i].targetTexture = null;
				}
			}

			//RenderTexture.active = lastRT;

			return renderTexture;
		}

	#endregion

	#region implements

		protected virtual void OnPrepare ()
		{
		}

		protected virtual void OnUpdate ()
		{
		}

		protected virtual void OnFinish ()
		{
		}

		public virtual void OnRenderImage (Texture source, RenderTexture destination)
		{
			//_MainTex = target
			if (material) {
				Graphics.Blit (target == ToType.Camera ? source : targetTexture, destination, material);
			} else {
				Graphics.Blit (source, destination);
			}
		}

	#endregion
	}
}