using UnityEngine;
using System.Collections.Generic;

using System;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace UDB
{
	[AddComponentMenu ("Lib/Core/UserData")]
	public abstract class UserData : MonoBehaviour, System.Collections.IEnumerable
	{
		public enum Action
		{
			Load,
			Save
		}

		[System.Serializable]
		public struct Data
		{
			public string name;
			public object obj;
		}

		public delegate void OnAction (UserData ud, Action act);

		//set if you want to grab this via Get
		public string id;

		//set to true if you want this userdata to be UserData.main
		[SerializeField]
		bool isMain = false;

		public bool loadOnStart = true;
		public bool autoSave = true;

		public event OnAction actCallback;

		private Dictionary<string, object> values = null;

		private Dictionary<string, object> valuesSnapshot = null;

		private static UserData main = null;
		private static Dictionary<string, UserData> instances = new Dictionary<string, UserData> ();
		private static bool mainExists = false;
		private static bool mainPrefabCoreCheck = false;


		public bool Started { get; private set; }

		public int ValueCount { get { return values != null ? values.Count : 0; } }

		/// <summary>
		///  Make sure one of the UserData is set as 'main'=true
		/// </summary>
		public static UserData Main { 
			get {
				if (mainExists == false) {
					//instantiate core if we haven't yet, this will only happen once in runtime
					if (mainPrefabCoreCheck == false) { 
						mainPrefabCoreCheck = true;

						//instantiating the gameobject ought to fill in the main UserData via Awake if there is any.
						var attribute = Attribute.GetCustomAttribute (typeof (UserData), typeof (PrefabCoreAttribute)) as PrefabCoreAttribute;
						if (attribute != null) {
							attribute.InstantiateGameObject ();
						}
					}
				}
				return main; 
			} 
		}



		void Awake ()
		{
			if (isMain) {
				main = this;
				mainExists = true;
			}

			if (string.IsNullOrEmpty (id) == false) {
				if (instances.ContainsKey (id)) {
					Log.Warning ("UserData " + id + " already exists.");
				} else {
					instances.Add (id, this);
				}
			}

			if (loadOnStart) {
				LoadOnStart ();
			}
		}

		void Start ()
		{
			Started = true;
		}

		void OnDestroy ()
		{
			if (main == this) {
				main = null;
				mainExists = false;
			}

			if (instances != null && string.IsNullOrEmpty (id) == false) {
				instances.Remove (id);
			}
		}

		void OnApplicationQuit ()
		{
			if (autoSave) {
				Save ();
			}
		}

		void SceneChange (string toScene)
		{
			if (autoSave) {
				Save ();
			}
		}

		public static UserData GetInstance (string aId)
		{
			UserData ret;
			instances.TryGetValue (aId, out ret);
			return ret;
		}

		public string[] GetKeys (System.Predicate<KeyValuePair<string, object>> predicate)
		{
			List<string> items = new List<string> (values.Count);
			foreach (KeyValuePair<string, object> pair in values) {
				if (predicate (pair)) {
					items.Add (pair.Key);
				}
			}

			return items.ToArray ();
		}

		public System.Collections.IEnumerator GetEnumerator ()
		{
			if (values == null) {
				return null;
			}
			return values.GetEnumerator ();
		}

		public void SnapshotSave ()
		{
			valuesSnapshot = values != null ? new Dictionary<string, object> (values) : null;
		}

		public void SnapshotRestore ()
		{
			if (valuesSnapshot != null) {
				values = new Dictionary<string, object> (valuesSnapshot);

				if (actCallback != null) {
					actCallback (this, Action.Load);
				}
			}
		}

		public void SnapshotDelete ()
		{
			valuesSnapshot = null;
		}

		public void SnapshotPreserve (string key)
		{
			if (valuesSnapshot != null) {
				object val;
				if (values.TryGetValue (key, out val)) {
					if (valuesSnapshot.ContainsKey (key)) {
						valuesSnapshot [key] = val;
					} else {
						valuesSnapshot.Add (key, val);
					}
				}
			}
		}

		public void Load ()
		{
			Data[] dat;

			byte[] raw = LoadRawData ();
			if (raw != null && raw.Length > 0) {
				dat = LoadData (raw);

				values = new Dictionary<string, object> ();
				foreach (Data datum in dat) {
					values.Add (datum.name, datum.obj);
				}

				if (actCallback != null) {
					actCallback (this, Action.Load);
				}
			}
		}

		public void Save ()
		{
			if (values != null) {
				if (actCallback != null) {
					actCallback (this, Action.Save);
				}

				List<Data> dat = new List<Data> (values.Count);
				foreach (KeyValuePair<string, object> pair in values) {
					dat.Add (new Data () { name = pair.Key, obj = pair.Value });
				}
				SaveData (dat.ToArray ());
			}
		}

		public void Delete ()
		{
			if (values != null) {
				values.Clear ();
			}
			DeleteRawData ();
		}

		public bool HasKey (string name)
		{
			return values != null && values.ContainsKey (name);
		}

		public System.Type GetType (string name)
		{
			object ret;
			if (values != null && values.TryGetValue (name, out ret)) {
				if (ret != null) {
					return ret.GetType ();
				}
			}
			return null;
		}

		public int GetInt (string name, int defaultValue = 0)
		{
			object ret;
			if (values != null && values.TryGetValue (name, out ret)) {
				if (ret is int) {
					return System.Convert.ToInt32 (ret);
				}
			}

			return defaultValue;
		}

		public void SetInt (string name, int value)
		{
			if (values == null) {
				values = new Dictionary<string, object> ();
			}

			if (values.ContainsKey (name) == false) {
				values.Add (name, value);
			} else {
				values [name] = value;
			}
		}

		public float GetFloat (string name, float defaultValue = 0)
		{
			object ret;
			if (values != null && values.TryGetValue (name, out ret)) {
				if (ret is float) {
					return System.Convert.ToSingle (ret);
				}
			}

			return defaultValue;
		}

		public void SetFloat (string name, float value)
		{
			if (values == null) {
				values = new Dictionary<string, object> ();
			}

			if (!values.ContainsKey (name)) {
				values.Add (name, value);
			} else {
				values [name] = value;
			}
		}

		public string GetString (string name, string defaultValue = "")
		{
			object ret;
			if (values != null && values.TryGetValue (name, out ret)) {
				if (ret is string) {
					return System.Convert.ToString (ret);
				}
			}

			return defaultValue;
		}

		public void SetString (string name, string value)
		{
			if (values == null) {
				values = new Dictionary<string, object> ();
			}

			if (!values.ContainsKey (name)) {
				values.Add (name, value);
			} else {
				values [name] = value;
			}
		}

		public void DeleteAllByNameContain (string nameContains)
		{
			if (values != null) {
				//ew
				foreach (string key in new List<string>(values.Keys)) {
					if (key.Contains (nameContains)) {
						values.Remove (key);
					}
				}
			}
		}

		public void Delete (string name)
		{
			if (values != null) {
				values.Remove (name);
			}
		}

		////////////////////////////////////////////
		// Implements
		////////////////////////////////////////////

		protected virtual void LoadOnStart ()
		{
			Load ();
		}

		protected abstract byte[] LoadRawData ();

		protected abstract void SaveRawData (byte[] dat);

		protected abstract void DeleteRawData ();

		private Data[] LoadData (byte[] dat)
		{
			Data[] ret = null;

			BinaryFormatter bf = new BinaryFormatter ();
			using (MemoryStream ms = new MemoryStream (dat)) {
				ret = (Data[])bf.Deserialize (ms);
			}

			return ret == null ? new Data[0] : ret;
		}

		private void SaveData (Data[] dat)
		{
			BinaryFormatter bf = new BinaryFormatter ();
			MemoryStream ms = new MemoryStream ();
			bf.Serialize (ms, dat);
			SaveRawData (ms.GetBuffer ());
		}
	}
}