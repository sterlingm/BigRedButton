using System;
using UnityEngine;

namespace UDB
{
	public static class ArrayExtensions
	{
		#region Public Methods and Operators

		public static void Shuffle(this Array array) 
		{
			for(int i = 0, max = array.Length; i < max; i++) {
				int r = UnityEngine.Random.Range(i, max);
				object obj = array.GetValue(i);
				object robj = array.GetValue(r);
				array.SetValue(robj, i);
				array.SetValue(obj, r);
			}
		}

		public static void Shuffle(this Array array, int start, int length) 
		{
			for(int i = start; i < length; i++) {
				int r = UnityEngine.Random.Range(i, length);
				object obj = array.GetValue(i);
				object robj = array.GetValue(r);
				array.SetValue(robj, i);
				array.SetValue(obj, r);
			}
		}

		#endregion
	}
}