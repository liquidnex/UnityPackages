using System.Collections.Generic;

namespace Liquid.ObjectPool
{
    public class FluctuationPatternRecord
    {
        public FluctuationPatternRecord(FluctuationPattern pattern)
        {
            Pattern = pattern;
            MatchCount = 0;
        }

        public FluctuationPattern Pattern;
        public int MatchCount;
    }
}