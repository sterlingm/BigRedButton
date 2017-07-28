using UnityEngine;

namespace UDB
{
	public static class CameraExtensions
	{
		#region Public Methods and Operators

		/// <summary>
		/// Calculates the size of the viewport at a given distance from a perspective camera.
		/// </summary>
		/// <param name="camera">The Camera.</param>
		/// <param name="distance">The positive distance from the camera.</param>
		/// <param name="aspectRatio">Optionally: An aspect ratio to use. If 0 is set, camera.aspect is used.</param>
		/// <returns>The size of the viewport at the given distance.</returns>
		public static Vector2 CalculateViewportWorldSizeAtDistance(this Camera camera, float distance, float aspectRatio = 0)
		{
			if (aspectRatio == 0) {
				aspectRatio = camera.aspect;
			}

			var viewportHeightAtDistance = 2.0f * Mathf.Tan(0.5f * camera.fieldOfView * Mathf.Deg2Rad) * distance;
			var viewportWidthAtDistance = viewportHeightAtDistance * aspectRatio;

			return new Vector2(viewportWidthAtDistance, viewportHeightAtDistance);
		}


		/// <summary>
		/// By default, the view frustum is arranged symmetrically around the camera’s centre line but it doesn’t necessarily need to be. 
		/// The frustum can be made “oblique”, which means that one side is at a smaller angle to the centre line than the opposite side. 
		/// The effect is rather like taking a printed photograph and cutting one edge off. This makes the perspective on one side of the 
		/// image seem more condensed giving the impression that the viewer is very close to the object visible at that edge. An example of 
		/// how this can be used is a car racing game where the frustum might be flattened at its bottom edge. This would make the viewer 
		/// seem closer to the road, accentuating the feeling of speed.
		/// 
		/// The horizObl and vertObl values set the amount of horizontal and vertical obliqueness, respectively. 
		/// A value of zero indicates no obliqueness. A positive value shifts the frustum rightwards or upwards, thereby flattening the left 
		/// or bottom side. A negative value shifts leftwards or downwards and consequently flattens the right or top side of the frustum.
		/// 
		/// A value of 1 or –1 in either variable indicates that one side of the frustum is completely flat against the centreline. 
		/// It is possible although seldom necessary to use values outside this range.
		/// </summary>
		/// <param name="camera">The Camera.</param>
		/// <param name="horizObl">The horizObl is the amount of horizontal obliqueness.</param>
		/// <param name="vertObl">The vertObl is the amount of vertical obliqueness.</param>
		public static void SetObliqueness(this Camera camera, float horizObl, float vertObl) 
		{
			Matrix4x4 mat  = camera.projectionMatrix;
			mat[0, 2] = horizObl;
			mat[1, 2] = vertObl;
			camera.projectionMatrix = mat;
		}

		// The height of the frustum at a given distance (both in world units) can be obtained with the following formula:
		// var frustumHeight = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

		// …and the process can be reversed to calculate the distance required to give a specified frustum height:
		// var distance = frustumHeight * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

		// It is also possible to calculate the FOV angle when the height and distance are known:
		// var camera.fieldOfView = 2.0f * Mathf.Atan(frustumHeight * 0.5f / distance) * Mathf.Rad2Deg;

		#endregion
	}
}