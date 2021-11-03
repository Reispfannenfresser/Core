public interface IBlockable {
	void OnBlocked(IObjectBlocker segment);
	void OnFreed(IObjectBlocker segment);
}
