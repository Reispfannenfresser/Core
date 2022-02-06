public interface IBlockable {
	void Block(ObjectBlocker blocker);
	void Free(ObjectBlocker blocker);
}
