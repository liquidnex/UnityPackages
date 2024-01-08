using UnityEngine;
using UnityEngine.UI;

namespace Liquid.BriskUI
{
    public static class ButtonEventPassExtension 
    {
        public static void AddEventPasser(this Button button)
        {
            if (button.gameObject == null)
                return;

            button.gameObject.AddComponent<EventPasser>();
        }

        public static void RemoveEventPasser(this Button button)
        {
            if (button.gameObject == null)
                return;

            GameObject.Destroy(button.gameObject.GetComponent<EventPasser>());
        }
    }
}