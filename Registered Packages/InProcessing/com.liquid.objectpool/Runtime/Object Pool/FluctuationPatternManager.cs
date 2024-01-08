using System.Collections.Generic;

namespace Liquid.ObjectPool
{
    public class FluctuationPatternManager
    {
        public enum MatchResultEnum
        {
            NONE,
            MATCH,
            NOT_MATCH,
            MATCH_BUT_DATA_EXHAUST
        }

        public FluctuationPattern GetFirstMatch(
            List<int> originalValues,
            MatchResultEnum matchResult = MatchResultEnum.MATCH)
        {
            FluctuationPattern newer = new FluctuationPattern(originalValues);
            if (!newer.IsAvailable)
                return null;

            foreach (FluctuationPatternRecord p in patterns)
            {
                if (Match(p, newer, out float score) == matchResult)
                {
                    return p.Pattern;
                }
            }

            return null;
        }

        public FluctuationPattern GetBestMatch(
            List<int> originalValues,
            MatchResultEnum matchResult = MatchResultEnum.MATCH)
        {
            FluctuationPattern newer = new FluctuationPattern(originalValues);
            if (!newer.IsAvailable)
                return null;

            float bestScore = float.MinValue;
            FluctuationPattern bestMath = null;
            foreach (FluctuationPatternRecord p in patterns)
            {
                if (Match(p, newer, out float curScore) == matchResult &&
                    curScore > bestScore)
                {
                    bestScore = curScore;
                    bestMath = p.Pattern;
                }
            }

            return bestMath;
        }

        public void AddPattern(List<int> originalValues)
        {
            FluctuationPattern newer = new FluctuationPattern(originalValues);
            if (!newer.IsAvailable)
                return;

            FluctuationPatternRecord sameOne = patterns.Find(p => p.Pattern.Token == newer.Token);
            if (sameOne == null)
            {
                if (patterns.Count >= ObjectPoolManager.Instance.MaximumPatternCount)
                    RemoveColdest();

                patterns.Add(new FluctuationPatternRecord(newer));
            }
            else
            {
                ++sameOne.MatchCount;
            }
        }

        private void RemoveColdest()
        {
            if (patterns.Count == 0)
                return;

            int coldestIdx = 0;
            int minCount = int.MinValue;
            for (int i = 0; i<patterns.Count; ++i)
            {
                FluctuationPatternRecord p = patterns[i];
                if (p.MatchCount <= minCount)
                {
                    coldestIdx = i;
                    minCount = p.MatchCount;
                }
            }

            patterns.RemoveAt(coldestIdx);
        }

        private MatchResultEnum Match(FluctuationPatternRecord target, FluctuationPattern newer, out float score)
        {
            bool isNoPrediction = newer.ChangesCount >= target.Pattern.ChangesCount;

            float rr = target.Pattern.GoodnessOfFit(newer);
            if (rr >= ObjectPoolManager.Instance.FitGoodnees)
            {
                score = rr;
                ++target.MatchCount;
                if (isNoPrediction)
                    return MatchResultEnum.MATCH_BUT_DATA_EXHAUST;
                else
                    return MatchResultEnum.MATCH;
            }
            score = 0;
            return MatchResultEnum.NOT_MATCH;
        }

        private List<FluctuationPatternRecord> patterns = new List<FluctuationPatternRecord>();
    }
}