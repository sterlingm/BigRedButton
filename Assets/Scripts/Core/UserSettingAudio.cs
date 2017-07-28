using UnityEngine;
using System.Collections;

namespace UDB
{
	[AddComponentMenu ("UDB/Core/UserSettingAudio")]
	public class UserSettingAudio : UserSetting<UserSettingAudio, MonoBehaviour>
	{
		public const string VOlUME_KEY = "volumeMaster";
		public const string SOUNDFX_KEY = "volumeSfx";
		public const string MUSIC_KEY = "volumeMusic";

		//need to debug while listening to music
	#if UNITY_EDITOR
		private const float volumeDefault = 1.0f;
	#else
		private const float volumeDefault = 1.0f;
	#endif

		private float volume;
		private float soundFXVolume;
		private float musicVolume;

		public static float SoundFXVolume {
			get { return UserSettingAudio.instance.GetSoundFXVolume (); }
			set { UserSettingAudio.instance.SetSoundFXVolume (value); }
		}

		public static float MusicVolume {
			get { return UserSettingAudio.instance.GetMusicVolume (); }
			set { UserSettingAudio.instance.SetMusicVolumne (value); }
		}

		public static float Volume {
			get { return UserSettingAudio.instance.GetVolume (); }
			set { UserSettingAudio.instance.SetVolumne (value); }
		}

		protected override void OnInstanceInit ()
		{
			base.OnInstanceInit ();

			//load settings
			volume = userData.GetFloat (VOlUME_KEY, 1.0f);

			soundFXVolume = userData.GetFloat (SOUNDFX_KEY, volumeDefault);

			musicVolume = userData.GetFloat (MUSIC_KEY, volumeDefault);

			AudioListener.volume = volume;
		}

		public float GetSoundFXVolume ()
		{
			return soundFXVolume;
		}

		public void SetSoundFXVolume (float newSoundFXVolume)
		{
			if (soundFXVolume != newSoundFXVolume) {
				soundFXVolume = newSoundFXVolume;
				userData.SetFloat (SOUNDFX_KEY, soundFXVolume);
				RelaySettingsChanged ();
			}
		}

		public float GetMusicVolume ()
		{
			return musicVolume;
		}

		public void SetMusicVolumne (float newMusicVolume)
		{
			if (musicVolume != newMusicVolume) {
				musicVolume = newMusicVolume;
				userData.SetFloat (MUSIC_KEY, musicVolume);
				RelaySettingsChanged ();
			}
		}

		public float GetVolume ()
		{
			return volume;
		}

		public void SetVolumne (float newVolume)
		{
			if (volume != newVolume) {
				volume = newVolume;
				userData.SetFloat (MUSIC_KEY, volume);
				RelaySettingsChanged ();
			}
		}
	}
}