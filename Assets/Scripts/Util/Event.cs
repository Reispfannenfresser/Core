using System.Collections.Generic;
using System.Linq;

public class Event<T> {
	public delegate void Action(T caller);
	private Dictionary<string, Action> actions = new Dictionary<string, Action>();

	public void RunEvent(T caller) {
		actions.AsParallel().ForAll(pair => pair.Value(caller));
	}

	public void AddAction(string id, Action action) {
		actions.Add(id, action);
	}

	public void RemoveAction(string id) {
		actions.Remove(id);
	}
}
