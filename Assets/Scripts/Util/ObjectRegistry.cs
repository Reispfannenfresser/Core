using System.Collections.Generic;

public class ObjectRegistry<T> {
	private static int id_count = 0;
	public static int object_count {get; private set;} = 0;
	public static Dictionary<int, T> objects {get; private set;} = new Dictionary<int, T>();

	public static int Add(T element) {
		objects.Add(id_count, element);
		object_count++;
		return id_count++;
	}

	public static void Remove(int id) {
		if (objects.ContainsKey(id)) {
			objects.Remove(id);
			object_count--;
		}
	}
}
