using UnityEngine;

public class Damageable : MonoBehaviour {
	[SerializeField]
	public int max_hp = 0;

	public int hp {get; protected set;} = 0;
	public bool dead {get; protected set;} = false;
	public int last_hp_change {get; set;} = 0;

	protected Event<Damageable> on_damaged_event;
	protected Event<Damageable> on_healed_event;
	protected Event<Damageable> on_killed_event;

	public EventWrapper<Damageable> on_damaged_wrapper;
	public EventWrapper<Damageable> on_healed_wrapper;
	public EventWrapper<Damageable> on_killed_wrapper;

	private void Awake() {
		hp = max_hp;

		on_damaged_event = new Event<Damageable>(this);
		on_healed_event = new Event<Damageable>(this);
		on_killed_event = new Event<Damageable>(this);

		on_damaged_wrapper = new EventWrapper<Damageable>(on_damaged_event);
		on_healed_wrapper = new EventWrapper<Damageable>(on_healed_event);
		on_killed_wrapper = new EventWrapper<Damageable>(on_killed_event);
	}

	protected void ChangeHP() {
		hp += last_hp_change;
		if (hp < 0) {
			hp = 0;
			Kill();
		} else if(hp > max_hp) {
			hp = max_hp;
		}
	}

	public void Damage(int amount) {
		if (!dead) {
			last_hp_change = -amount;
			on_damaged_event.RunEvent();
			ChangeHP();
		}
	}

	public void Heal(int amount) {
		if (!dead) {
			last_hp_change = amount;
			on_healed_event.RunEvent();
			ChangeHP();
		}
	}

	public void Kill() {
		if (!dead) {
			hp = 0;
			dead = true;
			on_killed_event.RunEvent();
		}
	}
}
