using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOnRestart : MonoBehaviour {
	private int unique_id = 0;

	private void Start() {
		unique_id = ObjectRegistry<DeleteOnRestart>.Add(this);
	}

	private void OnDestroy() {
		ObjectRegistry<DeleteOnRestart>.Remove(unique_id);
	}
}
