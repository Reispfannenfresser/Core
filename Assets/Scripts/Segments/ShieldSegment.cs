using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSegment : ConstructionSegment {
	Animator animator = null;
	AudioSource block_audio = null;
	[SerializeField]
	GameObject shield_circle = null;

	protected override void OnBlocked() {
		shield_circle.SetActive(false);
	}

	protected override void OnUnBlocked() {
		shield_circle.SetActive(true);
	}

	protected override void OnPlaced() {
		animator = GetComponent<Animator>();
		block_audio = GetComponent<AudioSource>();
	}

	protected override void OnDamaged(int amount) {
		animator.SetTrigger("Block");
		if (!block_audio.isPlaying) {
			block_audio.Play();
		}
		Heal(amount / 2);
	}
}
