using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UDB
{
	/// <summary>
	/// Provides a lot of convenience methods for handling Unity data types, either in the form of extension methods
	/// or as static helper methods.
	/// </summary>
	public static class UnityHelper
	{
		#region Camera

		public static Camera[] GetAllCameraDepthSorted() 
		{
			Camera[] cams = Camera.allCameras;
			System.Array.Sort<Camera>(cams, CameraCompareDepth);
			return cams;
		}

		static int CameraCompareDepth(Camera c1, Camera c2) 
		{
			return Mathf.RoundToInt(c1.depth - c2.depth);
		}

		#endregion


		#region Vector2/3/4

		/// <summary>
		/// Creates a Vector2 with a length of 1 pointing towards a certain angle.
		/// </summary>
		/// <param name="angleRad">The angle in radians.</param>
		/// <returns>The Vector2 pointing towards the angle.</returns>
		public static Vector2 CreateVector2AngleRad(float angleRad)
		{
			return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
		}

		/// <summary>
		/// Creates a Vector2 with a length of 1 pointing towards a certain angle.
		/// </summary>
		/// <param name="angleDeg">The angle in degrees.</param>
		/// <returns>The Vector2 pointing towards the angle.</returns>
		public static Vector2 CreateVector2AngleDeg(float angleDeg)
		{
			return CreateVector2AngleRad(angleDeg * Mathf.Deg2Rad);
		}

		/// <summary>
		/// Framerate-independent eased lerping to a target value, slowing down the closer it is.
		/// 
		/// If you call
		/// 
		///     currentValue = UnityHelper.EasedLerpVector3(currentValue, Vector2.one, 0.75f);
		/// 
		/// each frame (e.g. in Update()), starting with a currentValue of Vector2.zero, then after 1 second
		/// it will be approximately (0.75|0.75) - which is 75% of the way between Vector2.zero and Vector2.one.
		/// 
		/// Adjusting the target or the percentPerSecond between calls is also possible.
		/// </summary>
		/// <param name="current">The current value.</param>
		/// <param name="target">The target value.</param>
		/// <param name="percentPerSecond">How much of the distance between current and target should be covered per second?</param>
		/// <param name="deltaTime">How much time passed since the last call.</param>
		/// <returns>The interpolated value from current to target.</returns>
		public static Vector2 EasedLerpVector2(Vector2 current, Vector2 target, float percentPerSecond, float deltaTime = 0f)
		{
			var t = MathHelper.EasedLerpFactor(percentPerSecond, deltaTime);
			return Vector2.Lerp(current, target, t);
		}

		/// <summary>
		/// Framerate-independent eased lerping to a target value, slowing down the closer it is.
		/// 
		/// If you call
		/// 
		///     currentValue = UnityHelper.EasedLerpVector3(currentValue, Vector3.one, 0.75f);
		/// 
		/// each frame (e.g. in Update()), starting with a currentValue of Vector3.zero, then after 1 second
		/// it will be approximately (0.75|0.75|0.75) - which is 75% of the way between Vector3.zero and Vector3.one.
		/// 
		/// Adjusting the target or the percentPerSecond between calls is also possible.
		/// </summary>
		/// <param name="current">The current value.</param>
		/// <param name="target">The target value.</param>
		/// <param name="percentPerSecond">How much of the distance between current and target should be covered per second?</param>
		/// <param name="deltaTime">How much time passed since the last call.</param>
		/// <returns>The interpolated value from current to target.</returns>
		public static Vector3 EasedLerpVector3(Vector3 current, Vector3 target, float percentPerSecond, float deltaTime = 0f)
		{
			var t = MathHelper.EasedLerpFactor(percentPerSecond, deltaTime);
			return Vector3.Lerp(current, target, t);
		}

		/// <summary>
		/// Framerate-independent eased lerping to a target value, slowing down the closer it is.
		/// 
		/// If you call
		/// 
		///     currentValue = UnityHelper.EasedLerpVector4(currentValue, Vector4.one, 0.75f);
		/// 
		/// each frame (e.g. in Update()), starting with a currentValue of Vector4.zero, then after 1 second
		/// it will be approximately (0.75|0.75|0.75) - which is 75% of the way between Vector4.zero and Vector4.one.
		/// 
		/// Adjusting the target or the percentPerSecond between calls is also possible.
		/// </summary>
		/// <param name="current">The current value.</param>
		/// <param name="target">The target value.</param>
		/// <param name="percentPerSecond">How much of the distance between current and target should be covered per second?</param>
		/// <param name="deltaTime">How much time passed since the last call.</param>
		/// <returns>The interpolated value from current to target.</returns>
		public static Vector4 EasedLerpVector4(Vector4 current, Vector4 target, float percentPerSecond, float deltaTime = 0f)
		{
			var t = MathHelper.EasedLerpFactor(percentPerSecond, deltaTime);
			return Vector4.Lerp(current, target, t);
		}

		#endregion

		#region PlayerPrefs

		/// <summary>
		/// Returns the value corresponding to the key in the preference file if it exists.
		/// If it doesn't exist, it will return defaultValue.
		/// (Internally, the value is stored as an int with either 0 or 1.)
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value if none is given.</param>
		/// <returns>The value corresponding to key in the preference file if it exists, else the default value.</returns>
		public static bool PlayerPrefsGetBool(string key, bool defaultValue = false)
		{
			return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
		}

		/// <summary>
		/// Sets the value of the preference entry identified by the key.
		/// (Internally, the value is stored as an int with either 0 or 1.)
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value to set the preference entry to.</param>
		public static void PlayerPrefsSetBool(string key, bool value)
		{
			PlayerPrefs.SetInt(key, value ? 1 : 0);
		}

		#endregion
	}
}