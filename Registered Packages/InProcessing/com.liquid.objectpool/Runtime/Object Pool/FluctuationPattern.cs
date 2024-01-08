using System.Collections.Generic;
using UnityEngine;

namespace Liquid.ObjectPool
{
    public class FluctuationPattern
    {
        public FluctuationPattern(List<int> count)
        {
            originalValues = new List<int>(count.ToArray());

            if (originalValues == null ||
                originalValues.Count < 2)
                return;

            token = originalValues[0].ToString();
            for (int i = 1; i < count.Count; ++i)
            {
                token += "-";
                token += count[i].ToString();

                int frontIdx = i - 1;
                int delta = count[i] - count[frontIdx];
                changes.Add(delta);
            }
            isAvailable = true;
        }

        public float GoodnessOfFit(FluctuationPattern another)
        {
            List<int> a = changes;
            List<int> b = another.changes;
            if (a.Count == 0 ||
                b.Count == 0)
                return 0;

            List<int> dataWithFit;
            List<int> dataWithoutFit;
            if (a.Count > b.Count)
            {
                dataWithFit = a.GetRange(0, b.Count);
                dataWithoutFit = b;
            }
            else if (a.Count < b.Count)
            {
                dataWithFit = a;
                dataWithoutFit = b.GetRange(0, b.Count);
            }
            else
            {
                dataWithFit = a;
                dataWithoutFit = b;
            }

            float ssr = SSR(dataWithFit, dataWithoutFit);
            float sst = SST(dataWithoutFit);

            if (ssr == 0 && sst == 0)
                return 1;
            if (sst == 0)
                return 0;

            float rr = ssr / sst;
            return rr;
        }

        public int? PeakValue(int startIdx = 0)
        {
            if (startIdx <= 0 ||
                startIdx >= originalValues.Count)
                return null;

            int maxValue = 0;
            for (int i = startIdx; i < originalValues.Count; ++i)
            {
                int value = originalValues[i];
                if (value > maxValue)
                    maxValue = value;
            }
            return maxValue;
        }

        private float SST(List<int> dataWithoutFit)
        {
            float mean = (float)Sum(dataWithoutFit)/dataWithoutFit.Count;

            List<float> list = new List<float>();
            foreach (int d in dataWithoutFit)
            {
                list.Add(Mathf.Pow(d - mean, 2));
            }
            return Sum(list);
        }

        private float SSR(List<int> dataWithFit, List<int> dataWithoutFit)
        {
            float mean = (float)Sum(dataWithoutFit) / dataWithoutFit.Count;

            List<float> list = new List<float>();
            foreach (int d in dataWithFit)
            {
                list.Add(Mathf.Pow(d - mean, 2));
            }
            return Sum(list);
        }

        private static int Sum(List<int> list)
        {
            if (list == null)
                return 0;

            int sum = 0;
            foreach (int n in list)
            {
                sum += n;
            }
            return sum;
        }

        private static float Sum(List<float> list)
        {
            if (list == null)
                return 0;

            float sum = 0;
            foreach (int n in list)
            {
                sum += n;
            }
            return sum;
        }

        public bool IsAvailable
        { 
            get => isAvailable;
        }

        public string Token
        {
            get => token;
        }

        public int OriginalValuesCount
        {
            get => originalValues.Count;
        }

        public int ChangesCount
        {
            get => changes.Count;
        }

        private readonly bool isAvailable = false;
        private readonly List<int> originalValues = new List<int>();
        private readonly List<int> changes = new List<int>();
        private readonly string token;
    }
}