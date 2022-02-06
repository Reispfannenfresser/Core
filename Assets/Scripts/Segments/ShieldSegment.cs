using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ConstructionSegment))]
public class ShieldSegment : MonoBehaviour {
	[SerializeField]
	GameObject shield_circle = null;

	Animator animator = null;
	AudioSource block_audio = null;
	ConstructionSegment segment = null;

	private void Awake() {
		animator = GetComponent<Animator>();
		block_audio = GetComponent<AudioSource>();
		segment = GetComponent<ConstructionSegment>();

		block_audio.pitch += Random.value * 0.125f - 0.0625f;
	}

	private void Start() {
		segment.damageable.on_damaged_wrapper.AddAction("Shield", OnDamaged);
		segment.on_blocked_wrapper.AddAction("Shield", OnBlocked);
		segment.on_freed_wrapper.AddAction("Shield", OnFreed);
	}

	private void OnDamaged(Damageable damageable) {
		if (!segment.blocked && !damageable.dead) {
			damageable.last_hp_change /= 2;
			if (!block_audio.isPlaying) {
				block_audio.Play();
			}
		}
	}

	private void OnBlocked(ConstructionSegment segment) {
		shield_circle.SetActive(false);
		segment.damageable.on_damaged_wrapper.RemoveAction("Shield");
	}

	private void OnFreed(ConstructionSegment segment) {
		shield_circle.SetActive(true);
		segment.damageable.on_damaged_wrapper.AddAction("Shield", OnDamaged);
	}
}
