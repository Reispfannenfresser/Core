using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneySegment : ConstructionSegment {
	[SerializeField]
	private int money_cooldown = 5;
	private float current_money_cooldown = 0;
	[SerializeField]
	private int money_amount = 100;

	protected override void OnPlaced() {
		current_money_cooldown = money_cooldown;
	}

	protected override void OnFixedUpdate() {
		if (GameController.instance.harmful_enemies <= 0) {
			return;
		}

		current_money_cooldown -= Time.deltaTime;
		if (current_money_cooldown < 0) {
			current_money_cooldown += money_cooldown;
			GameController.instance.AddZollars(money_amount);
		}
	}
}
