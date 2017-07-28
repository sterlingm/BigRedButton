using System;
using UnityEngine;

#if NETFX_CORE
using System.Reflection;
#endif

namespace UDB
{
	[AttributeUsage( AttributeTargets.Class, Inherited = true )]
	public class SingletonPrefabAttribute : Attribute
	{
		public readonly string Name;

		public SingletonPrefabAttribute( string name )
		{
			Name = name;
		}
	}


	public abstract class SingletonBehaviour<T, P> : MonoBehaviour
		where T : MonoBehaviour
		where P : MonoBehaviour
	{
		private static T _instance;
		private static bool hasInstance;
		private static object lockObject = new object();


		public static T instance
		{
			get
			{
				return GetInstance();
			}
		}


		static void CreateInstance()
		{
			GameObject gameObject = null;

			if (typeof( P ) == typeof( MonoBehaviour ))
			{
				gameObject = new GameObject();
				gameObject.name = typeof( T ).Name;
			}
			else
			{
				var component = GameObject.FindObjectOfType<P>();
				if (component)
				{
					gameObject = component.gameObject;
				}
				else
				{
					Debug.LogError( "Could not find object with required component " + typeof( P ).Name );
					return;
				}
			}

			Debug.Log( "Creating instance of singleton component " + typeof( T ).Name );
			_instance = gameObject.AddComponent<T>();
			hasInstance = true;
		}


		static T GetInstance()
		{
			lock (lockObject)
			{
				if (hasInstance)
				{
					return _instance;
				}

				var type = typeof( T );
				var objects = FindObjectsOfType<T>();

				if (objects.Length > 0)
				{
					_instance = objects[0];
					hasInstance = true;

					if (objects.Length > 1)
					{
						Debug.LogWarning( "Multiple instances of singleton " + type + " found; destroying all but the first." );
						for (var i = 1; i < objects.Length; i++)
						{
							DestroyImmediate( objects[i].gameObject );
						}
					}

					return _instance;
				}

				#if NETFX_CORE
				var attribute = type.GetTypeInfo().GetCustomAttribute<SingletonPrefabAttribute>();
				#else
				var attribute = Attribute.GetCustomAttribute( type, typeof( SingletonPrefabAttribute ) ) as SingletonPrefabAttribute;
				#endif

				if (attribute == null)
				{
					CreateInstance();
				}
				else
				{
					var prefabName = attribute.Name;
					var gameObject = Instantiate( Resources.Load<GameObject>( prefabName ) ) as GameObject;
					if (gameObject == null)
					{
						Debug.LogError( "Could not find prefab " + prefabName + " for singleton of type " + type + "." );
						CreateInstance();
					}
					else
					{
						gameObject.name = prefabName;

						_instance = gameObject.GetComponent<T>();
						if (_instance == null)
						{
							Debug.LogWarning( "There wasn't a component of type \"" + type + "\" inside prefab \"" + prefabName + "\"; creating one now." );
							_instance = gameObject.AddComponent<T>();
							hasInstance = true;
						}
					}
				}

				(_instance as SingletonBehaviour<T,P>).OnInstanceInit ();

				return _instance;
			}
		}


		protected bool EnforceSingleton()
		{
			lock (lockObject)
			{
				if (hasInstance)
				{
					var objects = FindObjectsOfType<T>();
					for (var i = 0; i < objects.Length; i++)
					{
						if (objects[i].GetInstanceID() != _instance.GetInstanceID())
						{
							DestroyImmediate( objects[i].gameObject );
						}
					}
				}
			}
			return GetInstanceID() == instance.GetInstanceID();
		}


		protected bool EnforceSingletonComponent()
		{
			lock (lockObject)
			{
				if (hasInstance && GetInstanceID() != _instance.GetInstanceID())
				{
					DestroyImmediate( this );
					return false;
				}
			}
			return true;
		}


		void OnDestroy()
		{
			hasInstance = false;
		}

		protected virtual void OnInstanceInit ()
		{
		}

		protected virtual void OnInstanceDeinit ()
		{
		}
	}
}