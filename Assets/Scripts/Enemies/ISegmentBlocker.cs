using System.Collections;
using System.Collections.Generic;

public interface ISegmentBlocker {
	void StopBlocking(ConstructionSegment segment);
	void StartBlocking(ConstructionSegment segment);
}
