using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GeneratorSegment : ConstructionSegment {
	[SerializeField]
	private int money_cooldown = 5;
	private float current_money_cooldown = 0;
	[SerializeField]
	private int money_amount = 100;

	private Animator animator = null;

	protected override void OnPlaced() {
		current_money_cooldown = Random.value * money_cooldown;
		animator = GetComponent<Animator>();
	}

	protected override void OnFixedUpdate() {
		if (!GameController.instance.is_practice && GameController.instance.harmful_enemies <= 0) {
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
