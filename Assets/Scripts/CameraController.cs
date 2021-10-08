using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CameraController : MonoBehaviour {
	[SerializeField]
	float current_zoom_level = 1;
	[SerializeField]
	float min_zoom_level = 0.5f;
	[SerializeField]
	float max_zoom_level = 2;
	[SerializeField]
	float zoom_smooth_time = 0.3f;
	[SerializeField]
	float zoom_multiplier = 0.1f;

	[SerializeField]
	float max_shift_distance = 10f;

	private Camera own_camera = null;

	float original_orthographic_size;
	float wanted_zoom_level = 1;
	Vector3 last_mouse_pos = Vector3.zero;
	float zoom_velocity = 0;


	public AudioMixer mixer;

	void Awake() {
		own_camera = GetComponent<Camera>();
		original_orthographic_size = own_camera.orthographicSize;
	}

	void Update() {
		if (GameController.instance.is_paused) {
			return;
		}

		Vector3 new_pos = transform.position;

		if (Input.GetMouseButton(2)) {
			Vector3 mouse_pos = (own_camera.ScreenToWorldPoint(Input.mousePosition) - transform.position);
			if (!Input.GetMouseButtonDown(2)) {
				new_pos -= mouse_pos - last_mouse_pos;
			}
			last_mouse_pos = mouse_pos;
		}

		new_pos.z = 0;

		if (new_pos.magnitude > max_shift_distance) {
			new_pos.Normalize();
			new_pos *= max_shift_distance;
		}

		new_pos.z = -10;

		wanted_zoom_level = Mathf.Clamp(wanted_zoom_level - Input.mouseScrollDelta.y * zoom_multiplier, min_zoom_level, max_zoom_level);
		current_zoom_level = Mathf.SmoothDamp(current_zoom_level, wanted_zoom_level, ref zoom_velocity, zoom_smooth_time);
		own_camera.orthographicSize = original_orthographic_size / own_camera.aspect * current_zoom_level;

		mixer.SetFloat("CameraDistance", Mathf.Log10(0.1f + (1 - (current_zoom_level / max_zoom_level)) * 0.9f) * 20);

		transform.position = new_pos;
	}
}
