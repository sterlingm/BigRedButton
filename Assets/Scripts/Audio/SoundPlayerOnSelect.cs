using UnityEngine;
using System.Collections;

namespace UDB
{
	[AddComponentMenu ("UDB/Audio/SoundPlayerOnSelect")]
	public class SoundPlayerOnSelect : SoundPlayer
	{
		public bool onDeselect;

		void OnSelect (bool yes)
		{
			if ((onDeselect == false && yes) || 
			    (onDeselect && yes == false)) {
				Play ();
			}
		}
	}
}