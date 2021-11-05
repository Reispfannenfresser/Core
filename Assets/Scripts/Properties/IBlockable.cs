public interface IBlockable {
	void OnBlocked(ObjectBlocker blocker);
	void OnFreed(ObjectBlocker blocker);
}
