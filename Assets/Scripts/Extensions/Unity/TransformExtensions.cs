using UnityEngine;

namespace UDB
{
	public static class TransformExtensions
	{
		#region Public Methods and Operators

		/// <summary>
		///   Resets the local position, rotation and scale of a transform.
		/// </summary>
		/// <param name="transform">Transform to reset.</param>
		public static void Reset(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}
			
		/// <summary>
		/// Sets the x/y/z transform.position using optional parameters, keeping all undefined values as they were before. Can be
		/// called with named parameters like transform.SetPosition(x: 5, z: 10), for example, only changing transform.position.x and z.
		/// </summary>
		/// <param name="transform">The transform to set the transform.position at.</param>
		/// <param name="x">If this is not null, transform.position.x is set to this value.</param>
		/// <param name="y">If this is not null, transform.position.y is set to this value.</param>
		/// <param name="z">If this is not null, transform.position.z is set to this value.</param>
		public static void SetPosition(this Transform transform, float? x = null, float? y = null, float? z = null)
		{
			transform.position = transform.position.Change3(x, y, z);
		}

		public static void SetX(this Transform transform, float x)
		{
			var pos = transform.position;
			pos.x = x;
			transform.position = pos;
		}

		public static void SetY(this Transform transform, float y)
		{
			var pos = transform.position;
			pos.y = y;
			transform.position = pos;
		}

		public static void SetZ(this Transform transform, float z)
		{
			var pos = transform.position;
			pos.z = z;
			transform.position = pos;
		}


		/// <summary>
		/// increasing x coordinate of absolute position.
		/// </summary>
		/// <param name="transform"><see cref="UnityEngine.Transform" /> </param>
		/// < param  name = " deltaValue " > X-coordinate increment values. </ Param >
		public  static  void  AddPositionX (this Transform transform, float  deltaValue )
		{
			Vector3 v = transform.position;
			v.x += deltaValue;
			transform.position = v;
		}

		/// <summary>
		/// increase y coordinate of absolute position.
		/// </summary>
		/// <param name="transform"><see cref="UnityEngine.Transform" /> </param>
		/// < param  name = " deltaValue " > Y-coordinate increment values. </ Param >
		public  static  void  AddPositionY (this Transform transform, float  deltaValue )
		{
			Vector3 v = transform.position;
			v.y += deltaValue;
			transform.position = v;
		}

		/// <summary>
		/// increase the z coordinate of absolute position.
		/// </summary>
		/// <param name="transform"><see cref="UnityEngine.Transform" /> </param>
		/// < param  name = " deltaValue " > Z-coordinate increment values. </ Param >
		public  static  void  AddPositionZ (this Transform transform, float  deltaValue )
		{
			Vector3 v = transform.position;
			v.z += deltaValue;
			transform.position = v;
		}


		/// <summary>
		/// Sets the x/y/z transform.localPosition using optional parameters, keeping all undefined values as they were before. Can be
		/// called with named parameters like transform.SetLocalPosition(x: 5, z: 10), for example, only changing transform.localPosition.x and z.
		/// </summary>
		/// <param name="transform">The transform to set the transform.localPosition at.</param>
		/// <param name="x">If this is not null, transform.localPosition.x is set to this value.</param>
		/// <param name="y">If this is not null, transform.localPosition.y is set to this value.</param>
		/// <param name="z">If this is not null, transform.localPosition.z is set to this value.</param>
		public static void SetLocalPosition(this Transform transform, float? x = null, float? y = null, float? z = null)
		{
			transform.localPosition = transform.localPosition.Change3(x, y, z);
		}

		public static void SetLocalX(this Transform transform, float x)
		{
			var pos = transform.localPosition;
			pos.x = x;
			transform.localPosition = pos;
		}

		public static void SetLocalY(this Transform transform, float y)
		{
			var pos = transform.localPosition;
			pos.y = y;
			transform.localPosition = pos;
		}

		public static void SetLocalZ(this Transform transform, float z)
		{
			var pos = transform.localPosition;
			pos.z = z;
			transform.localPosition = pos;
		}

		/// <summary>
		/// increasing x coordinate of the relative position.
		/// </summary>
		/// <param name="transform"><see cref="UnityEngine.Transform" /> </param>
		/// < param  name = " deltaValue " > X-coordinate value. </ Param >
		public  static  void  AddLocalPositionX (this Transform transform, float  deltaValue )
		{
			Vector3 v = transform.localPosition;
			v.x += deltaValue;
			transform.localPosition = v;
		}

		/// <summary>
		/// increase the y coordinate of the relative position.
		/// </summary>
		/// <param name="transform"><see cref="UnityEngine.Transform" /> </param>
		/// < param  name = " deltaValue " > Y-coordinate value. </ Param >
		public  static  void  AddLocalPositionY (this Transform transform, float  deltaValue )
		{
			Vector3 v = transform.localPosition;
			v.y += deltaValue;
			transform.localPosition = v;
		}

		/// <summary>
		/// increase the relative z-coordinate position.
		/// </summary>
		/// <param name="transform"><see cref="UnityEngine.Transform" /> </param>
		/// < param  name = " deltaValue " > Z-coordinate value. </ Param >
		public  static  void  AddLocalPositionZ (this Transform transform, float  deltaValue )
		{
			Vector3 v = transform.localPosition;
			v.z += deltaValue;
			transform.localPosition = v;
		}

		/// <summary>
		/// Copies the position and rotation from another transform to this transform.
		/// </summary>
		/// <param name="transform">The transform to set the position/rotation at.</param>
		/// <param name="source">The transform to take the position/rotation from.</param>
		public static void CopyPositionAndRotatationFrom(this Transform transform, Transform source)
		{
			transform.position = source.position;
			transform.rotation = source.rotation;
		}


		/// <summary>
		/// increase the x component relative dimensions.
		/// </summary>
		/// <param name="transform"><see cref="UnityEngine.Transform" /> </param>
		/// <param name="deltaValue">x delta</param>
		public  static  void  AddLocalScaleX (this Transform transform, float  deltaValue )
		{
			Vector3 v = transform.localScale;
			v.x += deltaValue;
			transform.localScale = v;
		}

		/// <summary>
		/// increase the y component relative dimensions.
		/// </summary>
		/// <param name="transform"><see cref="UnityEngine.Transform" /> </param>
		/// <param name="deltaValue">y delta</param>
		public  static  void  AddLocalScaleY (this Transform transform, float  deltaValue )
		{
			Vector3 v = transform.localScale;
			v.y += deltaValue;
			transform.localScale = v;
		}

		/// <summary>
		/// increase the z-component of the relative sizes.
		/// </summary>
		/// <param name="transform"><see cref="UnityEngine.Transform" /> </param>
		/// <param name="deltaValue">z delta</param>
		public  static  void  AddLocalScaleZ (this Transform transform, float  deltaValue )
		{
			Vector3 v = transform.localScale;
			v.z += deltaValue;
			transform.localScale = v;
		}

		/// <summary>
		/// Sets the x/y/z transform.localScale using optional parameters, keeping all undefined values as they were before. Can be
		/// called with named parameters like transform.SetLocalScale(x: 5, z: 10), for example, only changing transform.localScale.x and z.
		/// </summary>
		/// <param name="transform">The transform to set the transform.localScale at.</param>
		/// <param name="x">If this is not null, transform.localScale.x is set to this value.</param>
		/// <param name="y">If this is not null, transform.localScale.y is set to this value.</param>
		/// <param name="z">If this is not null, transform.localScale.z is set to this value.</param>
		public static void SetLocalScale(this Transform transform, float? x = null, float? y = null, float? z = null)
		{
			transform.localScale = transform.localScale.Change3(x, y, z);
		}

		/// <summary>
		/// Sets the x/y/z transform.lossyScale using optional parameters, keeping all undefined values as they were before. Can be
		/// called with named parameters like transform.SetLossyScale(x: 5, z: 10), for example, only changing transform.lossyScale.x and z.
		/// </summary>
		/// <param name="transform">The transform to set the transform.lossyScale at.</param>
		/// <param name="x">If this is not null, transform.lossyScale.x is set to this value.</param>
		/// <param name="y">If this is not null, transform.lossyScale.y is set to this value.</param>
		/// <param name="z">If this is not null, transform.lossyScale.z is set to this value.</param>
		public static void SetLossyScale(this Transform transform, float? x = null, float? y = null, float? z = null)
		{
			var lossyScale = transform.lossyScale.Change3(x, y, z);

			transform.localScale = Vector3.one;
			transform.localScale = new Vector3(lossyScale.x / transform.lossyScale.x,
				lossyScale.y / transform.lossyScale.y,
				lossyScale.z / transform.lossyScale.z);

		}

		/// <summary>
		/// Sets the x/y/z transform.eulerAngles using optional parameters, keeping all undefined values as they were before. Can be
		/// called with named parameters like transform.SetEulerAngles(x: 5, z: 10), for example, only changing transform.eulerAngles.x and z.
		/// </summary>
		/// <param name="transform">The transform to set the transform.eulerAngles at.</param>
		/// <param name="x">If this is not null, transform.eulerAngles.x is set to this value.</param>
		/// <param name="y">If this is not null, transform.eulerAngles.y is set to this value.</param>
		/// <param name="z">If this is not null, transform.eulerAngles.z is set to this value.</param>
		public static void SetEulerAngles(this Transform transform, float? x = null, float? y = null, float? z = null)
		{
			transform.eulerAngles = transform.eulerAngles.Change3(x, y, z);
		}

		/// <summary>
		/// Sets the x/y/z transform.localEulerAngles using optional parameters, keeping all undefined values as they were before. Can be
		/// called with named parameters like transform.SetLocalEulerAngles(x: 5, z: 10), for example, only changing transform.localEulerAngles.x and z.
		/// </summary>
		/// <param name="transform">The transform to set the transform.localEulerAngles at.</param>
		/// <param name="x">If this is not null, transform.localEulerAngles.x is set to this value.</param>
		/// <param name="y">If this is not null, transform.localEulerAngles.y is set to this value.</param>
		/// <param name="z">If this is not null, transform.localEulerAngles.z is set to this value.</param>
		public static void SetLocalEulerAngles(this Transform transform, float? x = null, float? y = null, float? z = null)
		{
			transform.localEulerAngles = transform.localEulerAngles.Change3(x, y, z);
		}

		#endregion
	}
}