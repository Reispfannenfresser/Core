using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealingSegment : ConstructionSegment {
	[SerializeField]
	int healing_radius = 2;
	[SerializeField]
	float cooldown = 1;
	float current_cooldown = 0;
	[SerializeField]
	int amount = 20;
	[SerializeField]
	int cost = 1;

	[SerializeField]
	LayerMask construction = 0;
	LineRenderer rays = null;
	Animator animator = null;

	AudioSource mend_audio = null;

	protected override void OnPlaced() {
		rays = GetComponent<LineRenderer>();
		animator = GetComponent<Animator>();
		mend_audio = GetComponent<AudioSource>();
		current_cooldown += UnityEngine.Random.value * cooldown;
	}

	protected override void OnFixedUpdate() {
		current_cooldown -= Time.deltaTime;
		if (current_cooldown < 0) {
			current_cooldown += cooldown;
			Mend();
		}
	}

	private void Mend() {
		if (GameController.instance.GetZollars() < cost) {
			return;
		}

		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, healing_radius, construction);

		foreach (Collider2D collider in colliders) {
			ConstructionSegment segment = collider.gameObject.GetComponent<ConstructionSegment>();
			if (segment != null && segment != this && segment.max_hp - segment.hp >= amount) {
				GameController.instance.RemoveZollars(cost);
				rays.SetPosition(0, transform.position);
				rays.SetPosition(1, collider.transform.position);
				segment.Heal(amount);
				animator.SetTrigger("Heal");
				mend_audio.Play();
				return;
			}
		}
	}
}
