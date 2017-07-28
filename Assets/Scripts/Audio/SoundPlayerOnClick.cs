using UnityEngine;
using System.Collections;

namespace UDB
{
	[AddComponentMenu ("UDB/Audio/SoundPlayerOnClick")]
	public class SoundPlayerOnClick : SoundPlayer
	{
		void OnClick ()
		{
			Play ();
		}
	}
}