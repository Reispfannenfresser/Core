using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOnRestart : MonoBehaviour {
	private void Awake() {
		GameController.instance.delete_on_start.Add(this.gameObject);
	}
}
