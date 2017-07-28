using System;
using System.Collections;
using System.Collections.Generic;

namespace UDB
{
	/// <summary>
	/// Simple caching to minimize allocation, when order of items does not matter.
	/// </summary>
	public class CacheList<T> : IEnumerable<T>
	{
		public T this [int ind] {
			get {
				if (ind >= count) {
					throw new IndexOutOfRangeException ();
				}

				return items [ind];
			}
		}

		public int Count { get { return count; } }

		public int Capacity { get { return items.Length; } }

		public bool IsFull { get { return Count - Capacity >= 0; } }

		private T[] items;
		private int count;

		public CacheList (int capacity)
		{
			count = 0;
			items = new T[capacity];
		}

		public IEnumerator<T> GetEnumerator ()
		{
			for (int i = 0; i < count; i++)
				yield return items [i];
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		/// <summary>
		/// Double the amount of capacity
		/// </summary>
		public void Expand ()
		{
			Array.Resize (ref items, items.Length * 2);
		}

		/// <summary>
		/// Increase capacity by given amount.
		/// </summary>
		public void Expand (int amount)
		{
			if (amount <= 0) {
				throw new ArgumentException (string.Format ("Invalid amount ({0})" + amount));
			}
			Array.Resize (ref items, items.Length + amount);
		}

		public void Add (T item)
		{
			if (IsFull) {
				throw new InvalidOperationException (string.Format ("Maximum capacity of {0} reached.", items.Length));
			}
			items [count] = item;
			count++;
		}

		/// <summary>
		/// Remove and return the first available item.
		/// </summary>
		public T Remove ()
		{
			if (count > 0) {
				T ret = items [0];

				items [0] = items [count - 1];
				count--;
				items [count] = default(T);

				return ret;
			}
			return default(T);
		}

		public bool Remove (T item)
		{
			for (int i = 0; i < count; i++) {
				if (items [i].Equals (item)) {
					//move last item to this index, decrement count, then 'nullify'
					items [i] = items [count - 1];
					count--;
					items [count] = default(T);
					return true;
				}
			}
			return false;
		}

		public int RemoveAll (Predicate<T> match)
		{
			int c = 0;
			for (int i = 0; i < count; i++) {
				if (match (items [i])) {
					items [i] = items [count - 1];
					count--;
					items [count] = default(T);

					i--;
					c++;
				}
			}
			return c;
		}

		public void Clear ()
		{
			for (int i = 0; i < count; i++) {
				items [i] = default(T);
			}
			count = 0;
		}

		public bool Exists (T item)
		{
			for (int i = 0; i < count; i++) {
				if (items [i].Equals (item)) {
					return true;
				}
			}
			return false;
		}
	}
}