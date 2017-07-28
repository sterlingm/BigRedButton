using UnityEngine;
using System.Collections.Generic;

namespace UDB
{
	public static class Vector3Extensions
	{
		#region Public Methods and Operators

		public static Vector2 XY(this Vector3 v)
		{
			return new Vector2(v.x, v.y);
		}

		public static Vector2 XZ(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		public static Vector2 YZ(this Vector3 v)
		{
			return new Vector2(v.y, v.z);
		}

		public static Vector3 WithX(this Vector3 v, float newX)
		{
			return new Vector3(newX, v.y, v.z);
		}

		public static Vector3 WithY(this Vector3 v, float newY)
		{
			return new Vector3(v.x, newY, v.z);
		}

		public static Vector3 WithZ(this Vector3 v, float newZ)
		{
			return new Vector3(v.x, v.y, newZ);
		}

		/// <summary>
		/// Makes a copy of the Vector3 with changed x/y/z values, keeping all undefined values as they were before. Can be
		/// called with named parameters like vector.Change3(x: 5, z: 10), for example, only changing the x and z components.
		/// </summary>
		/// <param name="vector">The Vector3 to be copied with changed values.</param>
		/// <param name="x">If this is not null, the x component is set to this value.</param>
		/// <param name="y">If this is not null, the y component is set to this value.</param>
		/// <param name="z">If this is not null, the z component is set to this value.</param>
		/// <returns>A copy of the Vector3 with changed values.</returns>
		public static Vector3 Change3(this Vector3 vector, float? x = null, float? y = null, float? z = null)
		{
			if (x.HasValue) vector.x = x.Value;
			if (y.HasValue) vector.y = y.Value;
			if (z.HasValue) vector.z = z.Value;
			return vector;
		}

		/// <summary>
		/// Calculates the average position of an array of Vector3.
		/// </summary>
		/// <param name="array">The input array.</param>
		/// <returns>The average position.</returns>
		public static Vector3 CalculateCentroid(this Vector3[] array)
		{
			var sum = new Vector3();
			var count = array.Length;
			for (var i = 0; i < count; i++) {
				sum += array[i];
			}
			return sum / count;
		}

		/// <summary>
		/// Calculates the average position of a List of Vector3.
		/// </summary>
		/// <param name="list">The input list.</param>
		/// <returns>The average position.</returns>
		public static Vector3 CalculateCentroid(this List<Vector3> list)
		{
			var sum = new Vector3();
			var count = list.Count;
			for (var i = 0; i < count; i++) {
				sum += list[i];
			}
			return sum / count;
		}

		#endregion
	}
}