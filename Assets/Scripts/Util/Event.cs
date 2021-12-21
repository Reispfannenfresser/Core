using System.Collections.Generic;
using System.Linq;

public class Event<T> {
	public readonly T owner;

	public Event(T owner) {
		this.owner = owner;
	}

	public delegate void Action(Event<T> e);
	private Dictionary<string, Action> actions = new Dictionary<string, Action>();

	public void RunEvent() {
		actions.AsParallel().ForAll(pair => pair.Value(this));
	}

	public void AddAction(string id, Action action) {
		actions.Add(id, action);
	}

	public void RemoveAction(string id) {
		actions.Remove(id);
	}
}
