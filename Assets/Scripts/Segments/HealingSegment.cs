using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingSegment : ConstructionSegment {
	[SerializeField]
	int amount = 1;
	[SerializeField]
	int radius = 2;
	[SerializeField]
	float cooldown = 1;
	float current_cooldown = 0;
	[SerializeField]
	LayerMask construction = 0;

	[SerializeField]
	int cost = 5;

	LineRenderer rays = null;
	Animator animator = null;

	AudioSource mend_audio = null;
	bool is_playing = false;

	public static int sound_amount = 0;

	protected override void OnPlaced() {
		rays = GetComponent<LineRenderer>();
		animator = GetComponent<Animator>();
		mend_audio = GetComponent<AudioSource>();
	}

	void FixedUpdate() {
		current_cooldown -= Time.deltaTime;
		if (current_cooldown < 0) {
			current_cooldown += cooldown;

			if (GameController.instance.GetZollars() < cost) {
				return;
			}

			Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, construction);

			foreach (Collider2D collider in colliders) {
				ConstructionSegment segment = collider.gameObject.GetComponent<ConstructionSegment>();
				if (segment != null && segment != this && segment.hp < segment.max_hp) {
					GameController.instance.RemoveZollars(cost);
					rays.SetPosition(0, transform.position);
					rays.SetPosition(1, collider.transform.position);
					segment.Heal(amount);
					animator.SetTrigger("Heal");

					if (sound_amount < 4 && !mend_audio.isPlaying) {
						mend_audio.Play();
						is_playing = true;
						sound_amount += 1;
					}
					if (is_playing && !mend_audio.isPlaying) {
						sound_amount -= 1;
					}
					return;
				}
			}
		}
	}

	protected override void OnDestroyed() {
		if (is_playing) {
			sound_amount -= 1;
		}
	}

	protected override void OnDeleted() {
		if (is_playing) {
			sound_amount -= 1;
		}
	}
}
