using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConstructionSegment))]
public class GeneratorSegment : MonoBehaviour {
	[SerializeField]
	private int money_cooldown = 5;
	private float current_money_cooldown = 0;
	[SerializeField]
	private int money_amount = 100;

	private Animator animator = null;
	protected ConstructionSegment segment = null;

	private void Awake() {
		animator = GetComponent<Animator>();
		segment = GetComponent<ConstructionSegment>();

		current_money_cooldown = Random.value * money_cooldown;
	}

	private void Start() {
		segment.fixed_update_wrapper.AddAction("Generator", OnFixedUpdate);
	}

	private void OnFixedUpdate(ConstructionSegment segment) {
		if (!GameController.instance.is_practice && Enemy.harmful_enemies <= 0) {
			return;
		}

		current_money_cooldown -= Time.deltaTime;
		if (current_money_cooldown < 0) {
			current_money_cooldown += money_cooldown;
			GameController.instance.AddZollars(money_amount);
			animator.SetTrigger("Generate");
		}
	}
}
