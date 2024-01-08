using System.Collections.Generic;
using UnityEngine;

namespace Liquid.Samples.TypeUtilSample
{
    interface ITest<T>
    {
        void Func(T t);
    }

    class Test: ITest<int>
    {
        public void Func(int i){}
    }

    class Test1
    {
        public List<string> s0 = new List<string>{"s0.0", "s0.1"};
        protected string s1 = "s1";
        private string s2 = "s2";
    }

    public class Sample : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            bool f0 = d.GetType().IsSubClassOfRawGeneric(typeof(Dictionary<,>));
            Debug.Log(f0);

            Test t = new Test();
            bool f1 = t.GetType().HasImplementedRawGenereic(typeof(ITest<>));
            Debug.Log(f1);

            Test1 t1 = new Test1();
            List<string> result = TypeUtil.GetFields<Test1, string>(t1, true);
            foreach(string s in result)
            {
                Debug.Log(s);
            }
        }
    }
}