public interface IObjectBlocker {
	void StopBlocking(IBlockable blockable);
	void StartBlocking(IBlockable blockable);
}
