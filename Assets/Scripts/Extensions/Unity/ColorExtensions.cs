using UnityEngine;

namespace UDB
{
	public static class ColorExtensions
	{
		#region Public Methods and Operators


		public static Color WithAlpha(this Color c, float newAlpha)
		{
			return new Color(c.r, c.g, c.b, newAlpha);
		}

		public static Color WithAlpha(this Color c, byte newAlpha)
		{
			Color32 color = c;
			return new Color32(color.r, color.g, color.b, newAlpha);
		}

		public static Color WithBlue(this Color c, float newBlue)
		{
			return new Color(c.r, c.g, newBlue, c.a);
		}

		public static Color WithBlue(this Color c, byte newBlue)
		{
			Color32 color = c;
			return new Color32(color.r, color.g, newBlue, color.a);
		}

		public static Color WithGreen(this Color c, float newGreen)
		{
			return new Color(c.r, newGreen, c.b, c.a);
		}

		public static Color WithGreen(this Color c, byte newGreen)
		{
			Color32 color = c;
			return new Color32(color.r, newGreen, color.b, color.a);
		}

		public static Color WithRed(this Color c, float newRed)
		{
			return new Color(newRed, c.g, c.b, c.a);
		}

		public static Color WithRed(this Color c, byte newRed)
		{
			Color32 color = c;
			return new Color32(newRed, color.g, color.b, color.a);
		}

		/// <summary>
		/// Makes a copy of the Color with changed r/g/b/a values, keeping all undefined values as they were before. Can be
		/// called with named parameters like color.Change(g: 0, a: 0.5), for example, only changing the g and a components.
		/// </summary>
		/// <param name="color">The Color to be copied with changed values.</param>
		/// <param name="r">If this is not null, the r component is set to this value.</param>
		/// <param name="g">If this is not null, the g component is set to this value.</param>
		/// <param name="b">If this is not null, the b component is set to this value.</param>
		/// <param name="a">If this is not null, the a component is set to this value.</param>
		/// <returns>A copy of the Color with changed values.</returns>
		public static Color Change(this Color color, float? r = null, float? g = null, float? b = null, float? a = null)
		{
			if (r.HasValue) color.r = r.Value;
			if (g.HasValue) color.g = g.Value;
			if (b.HasValue) color.b = b.Value;
			if (a.HasValue) color.a = a.Value;
			return color;
		}

		/// <summary>
		/// Makes a copy of the vector with a changed alpha value.
		/// </summary>
		/// <param name="color">The Color to copy.</param>
		/// <param name="a">The new a component.</param>
		/// <returns>A copy of the Color with a changed alpha.</returns>
		public static Color ChangeAlpha(this Color color, float a)
		{
			color.a = a;
			return color;
		}


		#endregion
	}
}