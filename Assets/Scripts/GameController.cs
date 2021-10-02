using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour {
	[SerializeField]
	SegmentPlacer segment_placer = null;
	[SerializeField]
	GameObject[] segments = new GameObject[0];
	[SerializeField]
	GameObject[] enemy_types = new GameObject[0];
	[SerializeField]
	CoreSegment core = null;

	int current_wave = 0;
	float next_wave_in = 0;

	public static GameController instance = null;

	public HashSet<Enemy> enemies = new HashSet<Enemy>();

	void Awake() {
		instance = this;
	}

	public void FixedUpdate() {
		next_wave_in -= Time.deltaTime;
		if (next_wave_in < 0) {
			NextWave();
		}
	}

	public void NextWave() {
		int spawn_amount = current_wave;
		while (spawn_amount > 0) {
			int index = UnityEngine.Random.Range(0, Math.Min(enemy_types.Length, spawn_amount));
			SpawnEnemy(enemy_types[index]);
			spawn_amount -= index + 1;
		}
		current_wave += 1;
		next_wave_in = current_wave;
	}

	private void SpawnEnemy(GameObject enemy) {
		Instantiate(enemy, transform.position, transform.rotation);
	}

	public void AddEnemy(Enemy enemy) {
		enemies.Add(enemy);
	}

	public void RemoveEnemy(Enemy enemy) {
		enemies.Remove(enemy);
	}

	public void StopCoreRotation(float time) {
		core.StopRotating(time);
	}

	void Start() {
		instance.SetSegment(segments[0]);
	}

	public void SetSegment(GameObject to_place) {
		segment_placer.SetSegment(to_place);
	}
}
