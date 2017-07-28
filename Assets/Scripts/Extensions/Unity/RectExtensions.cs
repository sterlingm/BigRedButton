using UnityEngine;

namespace UDB
{
	public static class RectExtensions
	{
		#region Public Methods and Operators


		/// <summary>
		/// Extends/shrinks the rect by extendDistance to each side and gets a random position from the resulting rect.
		/// </summary>
		/// <param name="rect">The Rect.</param>
		/// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
		/// <returns>A random position inside the extended rect.</returns>
		public static Vector2 RandomPosition(this Rect rect, float extendDistance = 0f)
		{
			return new Vector2(Random.Range(rect.xMin - extendDistance, rect.xMax + extendDistance),
				Random.Range(rect.yMin - extendDistance, rect.yMax + extendDistance));
		}

		/// <summary>
		/// Gets a random subrect of the given width or height inside this rect.
		/// </summary>
		/// <param name="rect">The Rect.</param>
		/// <param name="width">The target width of the subrect. Clamped to the width of the given rect.</param>
		/// <param name="height">The target height of the subrect. Clamped to the height of the given rect.</param>
		/// <returns>A random subrect with the given width and height.</returns>
		public static Rect RandomSubRect(this Rect rect, float width, float height)
		{
			width = Mathf.Min(rect.width, width);
			height = Mathf.Min(rect.height, height);

			var halfWidth = width / 2f;
			var halfHeight = height / 2f;

			var centerX = Random.Range(rect.xMin + halfWidth, rect.xMax - halfWidth);
			var centerY = Random.Range(rect.yMin + halfHeight, rect.yMax - halfHeight);

			return new Rect(centerX - halfWidth, centerY - halfHeight, width, height);
		}

		/// <summary>
		/// Extends/shrinks the rect by extendDistance to each side and then restricts the given vector to the resulting rect.
		/// </summary>
		/// <param name="rect">The Rect.</param>
		/// <param name="position">A position that should be restricted to the rect.</param>
		/// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
		/// <returns>The vector, clamped to the Rect.</returns>
		public static Vector2 Clamp2(this Rect rect, Vector2 position, float extendDistance = 0f)
		{
			return new Vector2(Mathf.Clamp(position.x, rect.xMin - extendDistance, rect.xMax + extendDistance),
				Mathf.Clamp(position.y, rect.yMin - extendDistance, rect.yMax + extendDistance));
		}

		/// <summary>
		/// Extends/shrinks the rect by extendDistance to each side and then restricts the given vector to the resulting rect.
		/// The z component is kept.
		/// </summary>
		/// <param name="rect">The Rect.</param>
		/// <param name="position">A position that should be restricted to the rect.</param>
		/// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
		/// <returns>The vector, clamped to the Rect.</returns>
		public static Vector3 Clamp3(this Rect rect, Vector3 position, float extendDistance = 0f)
		{
			return new Vector3(Mathf.Clamp(position.x, rect.xMin - extendDistance, rect.xMax + extendDistance),
				Mathf.Clamp(position.y, rect.yMin - extendDistance, rect.yMax + extendDistance),
				position.z);
		}

		/// <summary>
		/// Extends/shrinks the rect by extendDistance to each side.
		/// </summary>
		/// <param name="rect">The Rect.</param>
		/// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
		/// <returns>The rect, extended/shrunken by extendDistance to each side.</returns>
		public static Rect Extend(this Rect rect, float extendDistance)
		{
			var copy = rect;
			copy.xMin -= extendDistance;
			copy.xMax += extendDistance;
			copy.yMin -= extendDistance;
			copy.yMax += extendDistance;
			return copy;
		}

		/// <summary>
		/// Extends/shrinks the rect by extendDistance to each side and then checks if a given point is inside the resulting rect.
		/// </summary>
		/// <param name="rect">The Rect.</param>
		/// <param name="position">A position that should be restricted to the rect.</param>
		/// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
		/// <returns>True if the position is inside the extended rect.</returns>
		public static bool Contains(this Rect rect, Vector2 position, float extendDistance)
		{
			return (position.x > rect.xMin + extendDistance) &&
				(position.y > rect.yMin + extendDistance) &&
				(position.x < rect.xMax - extendDistance) &&
				(position.y < rect.yMax - extendDistance);
		}

		/// <summary>
		/// Creates an array containing the four corner points of a Rect.
		/// </summary>
		/// <param name="rect">The Rect.</param>
		/// <returns>An array containing the four corner points of the Rect.</returns>
		public static Vector2[] GetCornerPoints(this Rect rect)
		{
			return new[]
			{
				new Vector2(rect.xMin, rect.yMin),
				new Vector2(rect.xMax, rect.yMin),
				new Vector2(rect.xMax, rect.yMax),
				new Vector2(rect.xMin, rect.yMax)
			};
		}

		#endregion
	}
}