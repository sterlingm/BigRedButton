using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace UDB
{
	[AddComponentMenu ("UDB/Audio/MusicManager")]
	public class MusicManager : MonoBehaviour
	{
		public enum AutoPlayType
		{
			None,
			Order,
			Shuffled
		}

		private enum State
		{
			None,
			Playing,
			Changing
		}

		[System.Serializable]
		public class MusicData
		{
			public string name;
			public AudioSource source;
			public float loopDelay = 0.0f;
			public bool loop = true;

			[System.NonSerialized]
			public float defaultVolume;
		}

		public delegate void OnMusicFinish (string curMusicName);

		/// <summary>
		/// Callback when music is done playing.  Make sure the audio source 'loop' is set to false
		/// </summary>
		public event OnMusicFinish MusicFinishCallback;

		public static MusicManager instance = null;

		public MusicData[] playlist;

		public float changeFadeOutDelay;

		public string playOnStart;

		public AutoPlayType autoPlay = AutoPlayType.None;

		public bool restartOnAppFocus;

		private const double RATE = 44100;

		private State musicState = State.None;

		private Dictionary<string, MusicData> musicDict;

		private MusicData curMusic;

		private MusicData nextMusic;

		private float curTime = 0;

		private int curAutoplayIndex = -1;

		public bool IsPlaying { get { return musicState == State.Playing; } }

		void Awake ()
		{
			if (instance != null) {
				instance.Stop (false);
				DestroyImmediate (instance.gameObject);
			}

			if (instance == null) {
				instance = this;

				UserSettingAudio.instance.changeCallback += UserSettingsChanged;

				musicDict = new Dictionary<string, MusicData> (playlist.Length);
				foreach (MusicData dat in playlist) {
					dat.defaultVolume = dat.source.volume;
					musicDict.Add (dat.name, dat);
				}

				if (autoPlay == AutoPlayType.Shuffled) {
					playlist.Shuffle ();
				}

			} else {
				DestroyImmediate (gameObject);
			}
		}

		// Use this for initialization
		void Start ()
		{
			if (string.IsNullOrEmpty (playOnStart) == false) {
				Play (playOnStart, true);
			} else if (autoPlay != AutoPlayType.None) {
				curAutoplayIndex = -1;
				AutoPlaylistNext ();
			}
		}

		// Update is called once per frame
		void Update ()
		{
			switch (musicState) {
				case State.None:
					break;
				case State.Playing:
					if (!(curMusic.source.loop || curMusic.source.isPlaying)) {
						if (autoPlay != AutoPlayType.None) {
							AutoPlaylistNext ();
						} else if (curMusic.loop) {
							// Loop Current Music
							curMusic.source.volume = curMusic.defaultVolume * UserSettingAudio.MusicVolume;
							if (curMusic.loopDelay > 0) {
								curMusic.source.Play ((ulong)System.Math.Round (RATE * ((double)curMusic.loopDelay)));
							} else {
								curMusic.source.Play ();
							}
						} else {
							SetState (State.None);
						}
						// Callback
						if (MusicFinishCallback != null) {
							MusicFinishCallback (curMusic.name);
						}
					}
					break;
				case State.Changing:
					curTime += Time.deltaTime;
					if (curTime >= changeFadeOutDelay) {
						curMusic.source.Stop ();

						if (nextMusic != null) {
							curMusic = nextMusic;
							nextMusic = null;

							SetState (State.Playing);
						} else {
							SetState (State.None);
						}
					} else {
						curMusic.source.volume = curMusic.defaultVolume * (1.0f - curTime / changeFadeOutDelay) * UserSettingAudio.MusicVolume;
					}
					break;
			}
		}

		void OnDestroy ()
		{
			if (instance == this) {
				instance = null;

				if (UserSettingAudio.instance) {
					UserSettingAudio.instance.changeCallback -= UserSettingsChanged;
				}

				MusicFinishCallback = null;
			}
		}

		void OnApplicationFocus (bool focus)
		{
			bool isPlaying = musicState == State.Playing;
			bool curMusicIsNotNull = curMusic != null;
			if (restartOnAppFocus && focus && isPlaying && curMusicIsNotNull) {
				curMusic.source.Stop ();
				curMusic.source.Play ();
			}
		}

		void UserSettingsChanged (UserSettingAudio us)
		{
			if (curMusic != null) {
				switch (musicState) {
					case State.Playing:
						curMusic.source.volume = curMusic.defaultVolume * us.GetMusicVolume ();
						break;
				}
			}
		}

		public bool Exists (string name)
		{
			return musicDict.ContainsKey (name);
		}

		public void Play (string name, bool immediate)
		{
			MusicData nextMusicTrack;
			if (musicDict.TryGetValue (name, out nextMusicTrack) == false) {
				Log.Warning ("Unknown music: " + name);
				return;
			}

			if (curMusic == null || (immediate && curMusic != nextMusicTrack)) {
				Stop (false);
				curMusic = nextMusicTrack;
				curMusic.source.volume = curMusic.defaultVolume * UserSettingAudio.MusicVolume;
				curMusic.source.Play ();
				SetState (State.Playing);
			} else if (curMusic != nextMusicTrack) {
				nextMusic = nextMusicTrack;
				SetState (State.Changing);
			}

			//determine index for auto playlist
			if (autoPlay != AutoPlayType.None) {
				for (int i = 0; i < playlist.Length; i++) {
					if (playlist [i].name == name) {
						curAutoplayIndex = i;
						break;
					}
				}
			}
		}

		public void Stop (bool fade)
		{
			if (musicState != State.None) {
				if (fade) {
					nextMusic = null;
					SetState (State.Changing);
				} else {
					curMusic.source.Stop ();
					SetState (State.None);
				}
			}
		}

		private void AutoPlaylistNext ()
		{
			Stop (false);

			// Increment the autoplay index
			curAutoplayIndex++;
			if (curAutoplayIndex >= playlist.Length) {
				curAutoplayIndex = 0;
			}

			curMusic = playlist [curAutoplayIndex];
			curMusic.source.volume = curMusic.defaultVolume * UserSettingAudio.MusicVolume;
			curMusic.source.Play ();
			SetState (State.Playing);
		}

		private void SetState (State state)
		{
			musicState = state;
			curTime = 0;

			if (musicState == State.None) {
				curMusic = null;
			}
		}
	}
}