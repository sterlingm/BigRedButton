using UnityEngine;

namespace UDB
{
	public abstract class UserSetting<T, P> : SingletonBehaviour<T, P> 
		where T : MonoBehaviour
		where P : MonoBehaviour
	{
		[SerializeField]
		protected UserData userData;
		//this is where to grab the settings, set to blank to grab from current gameObject

		public delegate void Callback (T us);

		public event Callback changeCallback;

		public void Save ()
		{
			userData.Save ();
		}

		protected override void OnInstanceInit ()
		{
			if (userData == null) {
				userData = GetComponent<UserData> ();
			}
		}

		protected void RelaySettingsChanged ()
		{
			if (changeCallback != null) {
				changeCallback (this as T);
			}
		}
	}
}