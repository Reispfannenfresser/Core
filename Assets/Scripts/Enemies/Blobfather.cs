using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Blobfather : Blob {
	protected AudioSource puke_audio = null;
	protected float original_pitch = 0;

	[SerializeField]
	protected Transform mouth = null;
	[SerializeField]
	protected GameObject shot = null;

	[SerializeField]
	protected float shot_spread = 5;
	[SerializeField]
	protected float shot_amount_increase_time = 5;
	protected float current_shot_amount_increase_time = 0;
	protected int shot_amount = 1;

	protected new void Awake() {
		base.Awake();

		puke_audio = GetComponent<AudioSource>();
		original_pitch = puke_audio.pitch;
	}

	protected new void Start() {
		base.Start();

		enemy.on_spawned_wrapper.AddAction("BlobFather", OnSpawned);
		enemy.fixed_update_wrapper.AddAction("BlobFather", OnFixedUpdate);
	}

	private void OnSpawned(Enemy e) {
		current_shot_amount_increase_time = shot_amount_increase_time;
	}

	private void OnFixedUpdate(Enemy e) {
		current_shot_amount_increase_time -= Time.deltaTime;
		if (current_shot_amount_increase_time < 0) {
			current_shot_amount_increase_time += shot_amount_increase_time;
			shot_amount++;
		}
	}

	private void OnTriggerEnter2D(Collider2D other) {
		IBlockable blockable = other.gameObject.GetComponent<IBlockable>();
		if (blockable != null) {
			object_blocker.StartBlocking(blockable);
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		IBlockable blockable = other.gameObject.GetComponent<IBlockable>();
		if (blockable != null) {
			object_blocker.StopBlocking(blockable);
		}
	}

	private void Puke() {
		puke_audio.pitch = original_pitch + Random.value * 0.25f - 0.125f;
		puke_audio.Play();
		Vector3 direction = -mouth.position;
		direction = direction.normalized;
		mouth.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

		for (int i = 0; i < shot_amount; i++) {
			Instantiate(shot, mouth.position + new Vector3(Random.value * 0.1f, Random.value * 0.1f, 0), mouth.rotation);
			mouth.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + (Random.value * 2 * shot_spread - shot_spread));
		}
	}
}
