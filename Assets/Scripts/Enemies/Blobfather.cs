using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blobfather : Blob {
	protected AudioSource puke_audio = null;
	protected float original_pitch = 0;

	[SerializeField]
	protected Transform mouth = null;
	[SerializeField]
	protected GameObject shot = null;

	[SerializeField]
	protected int shot_amount = 3;
	[SerializeField]
	protected float shot_spread = 5;

	protected override void OnSpawned() {
		base.OnSpawned();
		puke_audio = GetComponent<AudioSource>();
		original_pitch = puke_audio.pitch;
	}

	protected override void OnConnected(ConstructionSegment segment) {

	}

	private void OnTriggerEnter2D(Collider2D other) {
		ConstructionSegment segment = other.gameObject.GetComponent<ConstructionSegment>();
		if (segment != null && !blocked_segments.Contains(segment)) {
			(this as ISegmentBlocker).StartBlocking(segment);
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		ConstructionSegment segment = other.gameObject.GetComponent<ConstructionSegment>();
		if (segment != null && blocked_segments.Contains(segment)) {
			(this as ISegmentBlocker).StopBlocking(segment);
		}
	}

	private void Puke() {
		puke_audio.pitch = original_pitch + Random.value * 0.25f - 0.125f;
		puke_audio.Play();
		Vector3 direction = -mouth.position;
		direction.Normalize();
		mouth.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

		for (int i = 0; i < shot_amount; i++) {
			Instantiate(shot, mouth.position + new Vector3(Random.value * 0.1f, Random.value * 0.1f, 0), mouth.rotation);
			mouth.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + (Random.value * 2 * shot_spread - shot_spread));
		}
	}
}
