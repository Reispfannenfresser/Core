using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectBlocker : MonoBehaviour {
	protected HashSet<IBlockable> blocked_objects = new HashSet<IBlockable>();
	public int block_count {get; protected set;} = 0;

	public void StartBlocking(IBlockable blockable) {
		if (blocked_objects.Contains(blockable)) {
			return;
		}

		blocked_objects.Add(blockable);
		block_count++;

		blockable.OnBlocked(this);
	}

	public void StopBlocking(IBlockable blockable) {
		if (!blocked_objects.Contains(blockable)) {
			return;
		}

		blockable.OnFreed(this);

		blocked_objects.Remove(blockable);
		block_count--;
	}

	public void OnFreed(IBlockable blockable) {
		if (!blocked_objects.Contains(blockable)) {
			return;
		}

		blocked_objects.Remove(blockable);
		block_count--;
	}

	public void FreeEverything() {
		foreach (IBlockable blockable in blocked_objects) {
			if (blockable != null) {
				blockable.OnFreed(this);
			}
		}

		blocked_objects.Clear();
		block_count = 0;
	}
}
