using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ConstructionSegment))]
public class MenderSegment : MonoBehaviour {
	[SerializeField]
	int mending_cost = 1;
	[SerializeField]
	int mending_radius = 2;
	[SerializeField]
	float mending_cooldown = 1;
	float current_mending_cooldown = 0;
	[SerializeField]
	int mending_amount = 20;

	[SerializeField]
	LayerMask construction = 0;

	LineRenderer rays = null;
	Animator animator = null;
	AudioSource mend_audio = null;
	ConstructionSegment segment = null;

	private void Awake() {
		rays = GetComponent<LineRenderer>();
		animator = GetComponent<Animator>();
		mend_audio = GetComponent<AudioSource>();
		segment = GetComponent<ConstructionSegment>();

		mend_audio.pitch += UnityEngine.Random.value * 0.125f - 0.0625f;
		current_mending_cooldown += UnityEngine.Random.value * mending_cooldown;
	}

	private void Start() {
		segment.fixed_update_wrapper.AddAction("Mender", OnFixedUpdate);
	}

	private void OnFixedUpdate(ConstructionSegment segment) {
		current_mending_cooldown -= Time.deltaTime;
		if (current_mending_cooldown < 0) {
			current_mending_cooldown += mending_cooldown;
			Mend();
		}
	}

	private void Mend() {
		if (GameController.instance.GetZollars() < mending_cost) {
			return;
		}

		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, mending_radius, construction);
		foreach (Collider2D collider in colliders) {
			ConstructionSegment segment = collider.gameObject.GetComponent<ConstructionSegment>();
			if (segment != null && segment != this && segment.damageable.max_hp - segment.damageable.hp >= mending_amount) {
				GameController.instance.RemoveZollars(mending_cost);
				rays.SetPosition(0, transform.position);
				rays.SetPosition(1, collider.transform.position);
				segment.damageable.Heal(mending_amount);
				animator.SetTrigger("Heal");
				mend_audio.Play();
				return;
			}
		}
	}
}
