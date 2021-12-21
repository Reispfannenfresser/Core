public class EventWrapper<T> {
	private Event<T> e;

	public EventWrapper(Event<T> e) {
		this.e = e;
	}

	public void AddAction(string id, Event<T>.Action action) {
		e.AddAction(id, action);
	}

	public void RemoveAction(string id) {
		e.RemoveAction(id);
	}
}
