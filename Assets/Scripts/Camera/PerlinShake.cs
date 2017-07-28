using UnityEngine;
using System.Collections;

namespace UDB
{
	public class PerlinShake : MonoBehaviour
	{
		public static PerlinShake instance;

		public float duration = 0.5f;
		public float speed = 1.0f;
		public float magnitude = 0.1f;

		public float commonDuration = 1.0f;
		public float commonSpeed = 50.0f;
		public float commonMagnitude = 0.07f;

		public bool test = false;

		public Transform _transform;

		public enum State
		{
			normal,
			shakning
		}

		public State currentState;

		Vector3 originalCamPos;

		void Awake ()
		{
			_transform = this.transform;

			instance = this;
			currentState = State.normal;
			originalCamPos = _transform.localPosition;
		}

		// -------------------------------------------------------------------------
		public void PlayShake (float duration, float speed, float magnitude)
		{
			this.duration = duration;
			this.speed = speed;
			this.magnitude = magnitude;
			_transform.localPosition = originalCamPos;

			currentState = State.shakning;

			StopAllCoroutines ();
			StartCoroutine ("Shake");

		}

		public void CommonShake ()
		{

			duration = commonDuration;
			speed = commonSpeed;
			magnitude = commonMagnitude;

			currentState = State.shakning;

			StopAllCoroutines ();
			StartCoroutine ("Shake");
		}

		// -------------------------------------------------------------------------
		void Update ()
		{
			if (test) {
				test = false;
				PlayShake (duration, speed, magnitude);
			}

			switch (currentState) {
				case State.shakning: 
					///print (this.transform.GetInstanceID());
					//print("Position: " + this.transform.localPosition);
					break;


			}

		}

		// -------------------------------------------------------------------------
		IEnumerator Shake ()
		{

			float elapsed = 0.0f;

			float randomStart = Random.Range (-1000.0f, 1000.0f);

			while (elapsed < duration) {


				elapsed += Time.deltaTime;			

				float percentComplete = elapsed / duration;			

				// We want to reduce the shake from full power to 0 starting half way through
				float damper = 1.0f - Mathf.Clamp (2.0f * percentComplete - 1.0f, 0.0f, 1.0f);

				// Calculate the noise parameter starting randomly and going as fast as speed allows
				float alpha = randomStart + speed * percentComplete;

				// map noise to [-1, 1]
				//float x = Util.Noise.GetNoise(alpha, 0.0f, 0.0f) * 2.0f - 1.0f;
				//float y = Util.Noise.GetNoise(0.0f, alpha, 0.0f) * 2.0f - 1.0f;

				float x = Mathf.PerlinNoise (alpha, 0.0f) * 2.0f - 1.0f;
				float y = Mathf.PerlinNoise (0.0f, alpha) * 2.0f - 1.0f;

				x = x * (magnitude * damper) + originalCamPos.x;
				y = y * (magnitude * damper) + originalCamPos.y;

				_transform.localPosition = new Vector3 (x, y, originalCamPos.z);

				yield return null;
			}

			currentState = State.normal;
			_transform.localPosition = originalCamPos;
		}

	}
}