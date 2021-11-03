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

	protected override void Initialize() {
		base.Initialize();
		animator = GetComponent<Animator>();
	}

	protected override void Place() {
		base.Place();
		current_money_cooldown = Random.value * money_cooldown;
	}

	protected override void OnFixedUpdate() {
		if (blocked || (!GameController.instance.is_practice && Enemy.harmful_enemies <= 0)) {
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
