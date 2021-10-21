using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherSegment : ConstructionSegment {
	[SerializeField]
	GameObject bomb = null;
	[SerializeField]
	float cooldown = 10;
	float current_cooldown = 0;

	Animator animator = null;

	AudioSource launch_audio = null;

	[SerializeField]
	Bomb loaded_bomb = null;

	protected override void OnPlaced() {
		animator = GetComponent<Animator>();
		launch_audio = GetComponent<AudioSource>();
		Reload();
		current_cooldown += Random.value * cooldown;
	}

	protected override void OnFixedUpdate() {
		if (current_cooldown > 0) {
			current_cooldown -= Time.deltaTime;
		} else {
			current_cooldown = 0;
		}

		if (loaded_bomb != null) {
			SpriteRenderer bomb_sr = loaded_bomb.gameObject.GetComponent<SpriteRenderer>();
			Color color = bomb_sr.color;
			float progress = current_cooldown / cooldown;
			color.a = 1 - (int)(progress * 3)/3f;
			bomb_sr.color = color;

			if (current_cooldown == 0) {
				Fire();
			}
		}
	}

	private void Fire() {
		loaded_bomb.transform.SetParent(null, true);
		loaded_bomb.Launch();
		loaded_bomb = null;
		animator.SetTrigger("Launch");
	}

	private void Reload() {
		current_cooldown = cooldown;
		loaded_bomb = Instantiate(bomb, transform).GetComponent<Bomb>();
		SpriteRenderer bomb_sr = loaded_bomb.gameObject.GetComponent<SpriteRenderer>();
		Color color = bomb_sr.color;
		color.a = 0;
		bomb_sr.color = color;
	}
}
