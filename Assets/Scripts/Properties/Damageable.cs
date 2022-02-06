using UnityEngine;

public class Damageable {
	public readonly int max_hp;
	public int hp {get; protected set;} = 0;
	public bool dead {get; protected set;} = false;
	public int last_hp_change {get; set;} = 0;

	protected Event<Damageable> on_damaged_event;
	protected Event<Damageable> on_healed_event;
	protected Event<Damageable> on_killed_event;
	protected Event<Damageable> on_hp_changed_event;

	public EventWrapper<Damageable> on_damaged_wrapper;
	public EventWrapper<Damageable> on_healed_wrapper;
	public EventWrapper<Damageable> on_killed_wrapper;
	public EventWrapper<Damageable> on_hp_changed_wrapper;

	public Damageable(int max_hp) {
		this.max_hp = max_hp;
		hp = max_hp;

		on_damaged_event = new Event<Damageable>(this);
		on_healed_event = new Event<Damageable>(this);
		on_killed_event = new Event<Damageable>(this);
		on_hp_changed_event = new Event<Damageable>(this);

		on_damaged_wrapper = new EventWrapper<Damageable>(on_damaged_event);
		on_healed_wrapper = new EventWrapper<Damageable>(on_healed_event);
		on_killed_wrapper = new EventWrapper<Damageable>(on_killed_event);
		on_hp_changed_wrapper = new EventWrapper<Damageable>(on_hp_changed_event);
	}

	protected void ChangeHP() {
		if (last_hp_change == 0) {
			return;
		}

		hp += last_hp_change;
		if (hp <= 0) {
			hp = 0;
		} else if(hp > max_hp) {
			hp = max_hp;
		}
		on_hp_changed_event.RunEvent();
		if (hp == 0) {
			Kill();
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
			last_hp_change = -hp;
			ChangeHP();
			dead = true;
			on_killed_event.RunEvent();
		}
	}
}
