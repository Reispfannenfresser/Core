using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConstructionSegment))]
public class ThrusterSegment : MonoBehaviour
{
	[SerializeField]
	float max_power = 3f;
	[SerializeField]
	float acceleration = 0.1f;
	float current_power = 0f;

	[SerializeField]
	private SpriteRenderer fire = null;
	private ConstructionSegment segment = null;

	private void Awake()
	{
		segment = GetComponent<ConstructionSegment>();
	}

	private void Start()
	{
		segment.fixed_update_wrapper.AddAction("Thruster", OnFixedUpdate);
	}

	private void OnFixedUpdate(ConstructionSegment segment)
	{
		if (current_power < max_power)
		{
			current_power += Time.deltaTime * acceleration;
			if (current_power > max_power)
			{
				current_power = max_power;
			}
			segment.rb2d.gravityScale = -current_power;

			Color c = fire.color;
			c.a = Mathf.Min(current_power / max_power, 1);
			fire.color = c;
		}
	}
}
