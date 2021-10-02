using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	private Camera own_camera = null;

	float original_orthographic_size;
	float wanted_zoom_level = 1;
	float last_mouse_y;
	float zoom_velocity = 0;

	void Awake() {
		own_camera = GetComponent<Camera>();
		original_orthographic_size = own_camera.orthographicSize;
	}

	void Update() {
		Vector3 new_pos = transform.position;

		if (Input.GetMouseButton(2)) {
			float mouse_y = (own_camera.ScreenToWorldPoint(Input.mousePosition) - transform.position).y;
			if (!Input.GetMouseButtonDown(2)) {
				new_pos.y -= mouse_y - last_mouse_y;
			}
			last_mouse_y = mouse_y;
		}

		wanted_zoom_level = Mathf.Clamp(wanted_zoom_level - Input.mouseScrollDelta.y * zoom_multiplier, min_zoom_level, max_zoom_level);
		current_zoom_level = Mathf.SmoothDamp(current_zoom_level, wanted_zoom_level, ref zoom_velocity, zoom_smooth_time);
		own_camera.orthographicSize = original_orthographic_size / own_camera.aspect * current_zoom_level;

		if (new_pos.y > 15) {
			new_pos.y = 15;
		}

		if (new_pos.y < -5) {
			new_pos.y = -5;
		}

		transform.position = new_pos;
	}
}
