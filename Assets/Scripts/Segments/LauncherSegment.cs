using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherSegment : ConstructionSegment {
	[SerializeField]
	GameObject bomb = null;
	[SerializeField]
	float cooldown = 5;
	float current_cooldown = 0;

	[SerializeField]
	float reload_time = 2;
	float current_reload_time = 0;

	AudioSource launch_audio = null;

	[SerializeField]
	GameObject fg = null;

	[SerializeField]
	Bomb loaded_bomb = null;
	SpriteRenderer bomb_sr = null;

	protected override void Initialize() {
		base.Initialize();
		launch_audio = GetComponent<AudioSource>();
	}

	protected override void Place() {
		base.Place();
		Reload();
		current_cooldown += Random.value * cooldown;
	}

	protected override void OnFixedUpdate() {
		if (blocked) {
			return;
		}

		if (loaded_bomb == null) {
			current_reload_time -= Time.deltaTime;

			if(current_reload_time <= 0) {
				Reload();
			}
		} else {
			current_cooldown -= Time.deltaTime;

			Color color = bomb_sr.color;
			float progress = current_cooldown / cooldown;
			color.a = 1 - (int)(progress * 3)/3f;
			bomb_sr.color = color;

			if (current_cooldown <= 0) {
				Fire();
			}
		}
	}

	private void Fire() {
		loaded_bomb.transform.SetParent(null, true);
		loaded_bomb.Launch();
		launch_audio.Play();

		loaded_bomb = null;
		bomb_sr = null;

		fg.SetActive(false);

		current_reload_time = reload_time + current_cooldown;
	}

	private void Reload() {
		loaded_bomb = Instantiate(bomb, transform).GetComponent<Bomb>();

		bomb_sr = loaded_bomb.gameObject.GetComponent<SpriteRenderer>();
		Color color = bomb_sr.color;
		color.a = 0;
		bomb_sr.color = color;

		fg.SetActive(true);

		current_cooldown = cooldown + current_reload_time;
	}
}
