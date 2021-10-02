using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentMelter : MonoBehaviour {
	[SerializeField]
	int damage = 1;

	Collider2D own_collider = null;

	[SerializeField]
	LayerMask construction = 0;

	ContactFilter2D contact_filter = new ContactFilter2D();
	Collider2D[] to_melt = new Collider2D[10];

	private void Start() {
		own_collider = GetComponent<Collider2D>();
		contact_filter.useLayerMask = true;
		contact_filter.layerMask = construction;
	}

	private void FixedUpdate() {
		int size = Physics2D.OverlapCollider(own_collider, contact_filter, to_melt);

		for (int i = 0; i < size; i++) {
			ConstructionSegment segment = to_melt[i].gameObject.GetComponent<ConstructionSegment>();
			if (segment == null || !segment.meltable) {
				return;
			}
			segment.Damage(damage);
		}

	}
}
