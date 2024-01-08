using Liquid.CommonUtils;
using System;
using UnityEngine;

namespace Liquid.Samples.FactorySample
{
    public class Item : Product
    {
        protected Item(string name)
        {
            Name = name;
        }

        protected virtual void OnProductCreate()
        {
            Debug.Log(string.Format("Item '{0}': Call OnProductCreate.", Name));
        }

        protected virtual void OnProductDestroy()
        {
            Debug.Log(string.Format("Item '{0}': Call OnProductDestroy.", Name));
        }

        public string Name = "";
    }

    public class ItemManager : Factory<ItemManager, Item> { }


    public class Apple : Item 
    {
        private Apple(string name)
            :base(name){ }

        private static Apple ProductCreate(ItemManager factory, params object[] args)
        {
            if (args.Length != 1 || !(args[0] is string))
                return null;
            string name = args[0] as string;

            Debug.Log(string.Format("Apple '{0}': Call ProductCreate", name));

            return new Apple(name);
        }

        protected override void OnProductCreate()
        {
            Debug.Log(string.Format("Apple '{0}': Call OnProductCreate.", Name));
        }

        private static bool ProductDestroy(ItemManager factory, Apple product)
        {
            Debug.Log(string.Format("Apple '{0}': Call ProductDestroy.", product.Name));

            return true;
        }

        protected override void OnProductDestroy()
        {
            Debug.Log(string.Format("Apple '{0}': Call OnProductDestroy.", Name));
        }
    }

    public class Orange : Item
    {
        private Orange(string name)
            : base(name) { }
    }

    public class Sample : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            ItemManager f = new ItemManager();

            Type appleType = typeof(Apple);
            f.Create(appleType, "Apple1");
            Apple a2 =
                (Apple)f.Create(appleType, "Apple2");
            f.Remove(a2);

            f.Create<Orange>("Orange1");
            f.Create<Orange>("Orange2");

            int cnt = f.Count<Item>(true);
            Debug.Log(cnt);
        }
    }
}