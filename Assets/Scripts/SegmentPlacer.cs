using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentPlacer : MonoBehaviour {
	[SerializeField]
	LayerMask construction = 0;

	GameObject to_place = null;
	bool can_place = false;

	[SerializeField]
	Camera current_camera = null;

	Animator animator;
	SpriteRenderer sprite_renderer;

	int cost = 0;

	float to_place_radius = 0.5f;

	void Awake() {
		animator = GetComponent<Animator>();
		sprite_renderer = GetComponent<SpriteRenderer>();
	}

	void FixedUpdate() {
		Vector3 new_pos = current_camera.ScreenToWorldPoint(Input.mousePosition);
		new_pos.z = 0;
		transform.position = new_pos;

		if (GameController.instance.GetZollars() < cost) {
			sprite_renderer.color = new Color(0.75f, 0.5f, 0.5f);
		} else {
			sprite_renderer.color = Color.white;
		}

		can_place = Physics2D.OverlapCircle(transform.position, to_place_radius - ConstructionSegment.max_overlap, construction) == null && Physics2D.OverlapCircle(transform.position, to_place_radius + ConstructionSegment.max_distance, construction) != null;
		Color color = sprite_renderer.color;
		color.a = can_place ? 0.75f : 0.25f;
		sprite_renderer.color = color;
	}

	public void SetSegment(GameObject segment_object) {
		ConstructionSegment segment = segment_object.GetComponent<ConstructionSegment>();
		if (segment == null) {
			return;
		}

		cost = segment.GetCost();
		to_place = segment_object;
		to_place_radius = segment.GetRadius();
		sprite_renderer.sprite = segment_object.GetComponent<SpriteRenderer>().sprite;
	}

	void Update() {
		if (can_place && Input.GetMouseButtonDown(0)) {
			Place();
		}

		if (!can_place && Input.GetMouseButtonDown(1)) {
			Collider2D collider = Physics2D.OverlapPoint(transform.position, construction);
			if (collider == null) {
				return;
			}

			ConstructionSegment segment = collider.gameObject.GetComponent<ConstructionSegment>();
			if (segment != null && segment.IsDeletable()) {
				segment.Delete();
			}
		}
	}

	void Place() {
		if (to_place == null || GameController.instance.GetZollars() < cost) {
			return;
		}

		GameObject new_object = GameObject.Instantiate(to_place, transform.position, transform.rotation);

		GameController.instance.RemoveZollars(cost);
	}
}
