using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSegment : ConstructionSegment, IBlockable {
	Animator animator = null;
	AudioSource block_audio = null;
	[SerializeField]
	GameObject shield_circle = null;

	protected override void Initialize() {
		base.Initialize();
		animator = GetComponent<Animator>();
		block_audio = GetComponent<AudioSource>();
	}

	protected override void Place() {
		base.Place();
		block_audio.pitch += Random.value * 0.125f - 0.0625f;

		damageable.on_damaged_wrapper.AddAction("Shield_damage_reduction", e => {
			if (!blocked && !damageable.dead) {
				damageable.last_hp_change /= 2;
				if (!block_audio.isPlaying) {
					block_audio.Play();
				}
			}
		});
	}

	protected override void Block() {
		base.Block();
		shield_circle.SetActive(false);
	}

	protected override void Free() {
		base.Free();
		if (shield_circle != null) {
			shield_circle.SetActive(true);
		}
	}
}
