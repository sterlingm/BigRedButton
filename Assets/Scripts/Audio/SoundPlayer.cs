using UnityEngine;
using System.Collections;


namespace UDB
{
	/// <summary>
	/// Base class for playing sounds, need to inherit from this in order to allow global sound settings to affect.
	/// </summary>
	[RequireComponent (typeof (AudioSource))]
	[AddComponentMenu ("UDB/Audio/SoundPlayer")]
	public class SoundPlayer : MonoBehaviour
	{
		[SerializeField]
		AudioSource target;

		/// <summary>
		/// Play the sound whenever it is enabled
		/// </summary>
		public bool playOnActive = false;

		/// <summary>
		/// Delay of the sound. If playOnActive is set to true, playDelay is set to 0.0f
		/// </summary>
		public float playDelay = 0.0f;

		private bool started = false;
		private float defaultVolume = 1.0f;

		public bool IsPlaying { 
			get { return target.isPlaying; } 
		}

		public float DefaultVolume { 
			get { return defaultVolume; } 
			set { defaultVolume = value; } 
		}

		public AudioSource Target {
			get { return target; }
			set {
				if (target != value) {
					target = value;
					if (target) {
						InitTarget ();
					}
				}
			}
		}

		protected virtual void Awake ()
		{
			if (target) {
				InitTarget ();
			} else {
				target = GetComponent<AudioSource> ();
			}

			UserSettingAudio.instance.changeCallback += UserSettingsChanged;
		}

		// Use this for initialization
		protected virtual void Start ()
		{
			started = true;

			if (playOnActive) {
				Play ();
			}
		}

		protected virtual void OnEnable ()
		{
			if (started && playOnActive) {
				Play ();
			}
		}

		protected virtual void OnDestroy ()
		{
			if (UserSettingAudio.instance) {
				UserSettingAudio.instance.changeCallback -= UserSettingsChanged;
			}
		}

		void UserSettingsChanged (UserSettingAudio us)
		{
			target.volume = defaultVolume * us.GetSoundFXVolume ();
		}

		public virtual void Play ()
		{
			target.volume = defaultVolume * UserSettingAudio.SoundFXVolume;
			if (playDelay > 0.0f) {
				target.PlayDelayed (playDelay);
			} else {
				target.Play ();
			}
		}

		public virtual void Stop ()
		{
			target.Stop ();
		}

		private void InitTarget ()
		{
			target.playOnAwake = false;
			defaultVolume = target.volume;
			target.volume = defaultVolume * UserSettingAudio.SoundFXVolume;
		}
	}
}