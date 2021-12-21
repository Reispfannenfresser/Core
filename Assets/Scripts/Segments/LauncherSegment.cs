using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherSegment : ConstructionSegment {
	[SerializeField]
	GameObject bomb = null;

	[SerializeField]
	float launch_cooldown = 5;
	float current_launch_cooldown = 0;

	AudioSource launch_audio = null;

	[SerializeField]
	GameObject fg = null;

	[SerializeField]
	Bomb loaded_bomb = null;
	SpriteRenderer bomb_sr = null;

	int state = 0;

	protected override void Initialize() {
		base.Initialize();
		launch_audio = GetComponent<AudioSource>();
	}

	protected override void OnFixedUpdate() {
		if (blocked) {
			return;
		}

		switch(state) {
			case 0: // reload
				Reload();
				state++;
				break;
			case 1: // wait
				current_launch_cooldown -= Time.deltaTime;
				Color color = bomb_sr.color;
				float progress = current_launch_cooldown / launch_cooldown;
				color.a = 1 - (int)(progress * 3)/3f;
				bomb_sr.color = color;

				if (current_launch_cooldown <= 0) {
					state++;
				}
				break;
			case 2: // fire
				Fire();
				state++;
				break;
			case 3: // wait
				break;
		}
	}

	private void Fire() {
		loaded_bomb.transform.SetParent(null, true);
		loaded_bomb.Launch();
		launch_audio.Play();

		loaded_bomb.detonate_event.AddAction("free_launcher", event => {
			loaded_bomb = null;
			bomb_sr = null;
			state = 0;
		});

		fg.SetActive(false);
	}

	private void Reload() {
		loaded_bomb = Instantiate(bomb, transform).GetComponent<Bomb>();

		current_launch_cooldown = launch_cooldown;

		bomb_sr = loaded_bomb.gameObject.GetComponent<SpriteRenderer>();
		Color color = bomb_sr.color;
		color.a = 0;
		bomb_sr.color = color;

		fg.SetActive(true);
	}

	protected override void OnDestroyed() {
		base.OnDestroyed();

		if (state == 3) {
			loaded_bomb.detonate_event.RemoveAction("free_launcher");
			loaded_bomb.Detonate();
		}
	}
}
