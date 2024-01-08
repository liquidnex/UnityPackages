using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Liquid.BriskUI
{
    public class EventPasser :
        MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            PassEvent(eventData, ExecuteEvents.pointerClickHandler);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PassEvent(eventData, ExecuteEvents.pointerDownHandler);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PassEvent(eventData, ExecuteEvents.pointerUpHandler);
        }

        private void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function)
            where T : IEventSystemHandler
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);

            int sourceIdx = results.FindIndex(
                r => r.gameObject == gameObject
            );
            if (sourceIdx == -1)
                return;

            int targetIdx = sourceIdx + 1;
            if (targetIdx < 0 ||
                targetIdx >= results.Count)
                return;

            GameObject passTarget = results[targetIdx].gameObject;
            if (passTarget == null)
                return;

            ExecuteEvents.Execute(passTarget, data, function);
        }
    }
}