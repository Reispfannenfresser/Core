using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	[SerializeField]
	SegmentPlacer segment_placer = null;
	[SerializeField]
	GameObject[] segments = new GameObject[0];

	public static GameController instance = null;

	public HashSet<Enemy> enemies = new HashSet<Enemy>();

	void Awake() {
		instance = this;
	}

	public void AddEnemy(Enemy enemy) {
		enemies.Add(enemy);
	}

	public void RemoveEnemy(Enemy enemy) {
		enemies.Remove(enemy);
	}

	void Start() {
		instance.SetSegment(segments[0]);
	}

	public void SetSegment(GameObject to_place) {
		segment_placer.SetSegment(to_place);
	}
}
