using System.Collections.Generic;
using System.Linq;

public class Event<T> {
	public readonly T owner;

	public Event(T owner) {
		this.owner = owner;
	}

	public delegate void Action(T owner);
	private Dictionary<string, Action> actions = new Dictionary<string, Action>();

	public void RunEvent() {
		foreach (KeyValuePair<string, Action> kvp in actions) {
			kvp.Value(owner);
		}
	}

	public void AddAction(string id, Action action) {
		actions.Add(id, action);
	}

	public void RemoveAction(string id) {
		actions.Remove(id);
	}
}
