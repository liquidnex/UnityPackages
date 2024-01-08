using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Liquid.EasyInput
{
    /// <summary>
    /// Touchpad controller.
    /// </summary>
    public class TouchpadInputer : IInputer
    {
        private struct MoveEvent
        {
            public MoveEvent(
                int fingerID,
                Vector2 position,
                Vector2 deltaPosition
            )
            {
                FingerID = fingerID;
                Position = position;
                DeltaPosition = deltaPosition;
            }

            public readonly int FingerID;
            public readonly Vector2 Position;
            public readonly Vector2 DeltaPosition;
        }

        /// <summary>
        /// Callback when touch is start.
        /// </summary>
        /// <param name="multipleTouch">
        /// Returns true if more than one finger is touching at this time,
        /// otherwise returns false.
        /// </param>
        /// <param name="fingerID">Current finger id.</param>
        /// <param name="pos">Touch position.</param>
        /// <param name="isPointerOverUI">Whether the pointer is over ui game object.</param>
        public delegate void OnTouchBegan(bool multipleTouch, int fingerID, Vector2 pos, bool isPointerOverUI);

        /// <summary>
        /// Callback when touch moves.
        /// </summary>
        /// <param name="multipleTouch">
        /// Returns true if more than one finger is touching at this time,
        /// otherwise returns false.
        /// </param>
        /// <param name="fingerID">Current finger id.</param>
        /// <param name="pos">Touch position.</param>
        /// <param name="deltaPerSec">
        /// The average moving distance per second,
        /// which is used to reflect the variable of moving speed.
        /// </param>
        public delegate void OnTouchMoved(bool multipleTouch, int fingerID, Vector2 pos, float deltaPerSec);

        /// <summary>
        /// Callback when pinch.
        /// </summary>
        /// <param name="fingerIDs">Current finger id.</param>
        /// <param name="positions">Touch positions of the two touching fingers.</param>
        /// <param name="deltaPerSec">
        /// The average moving distance per second,
        /// which is used to reflect the variable of moving speed.
        /// </param>
        public delegate void OnTouchPinch(List<int> fingerIDs, List<Vector2> positions, float deltaPerSec);

        /// <summary>
        /// Callback on touch cancel.
        /// </summary>
        /// <param name="multipleTouch">
        /// Returns true if more than one finger is touching at this time,
        /// otherwise returns false.
        /// </param>
        /// <param name="fingerID">Current finger id.</param>
        /// <param name="pos">Touch position.</param>
        public delegate void OnTouchCancel(bool multipleTouch, int fingerID, Vector2 pos);

        /// <summary>
        /// Callback when touch ends.
        /// </summary>
        /// <param name="multipleTouch">
        /// Returns true if more than one finger is touching at this time,
        /// otherwise returns false.
        /// </param>
        /// <param name="fingerID">Current finger id.</param>
        /// <param name="pos">Touch position.</param>
        public delegate void OnTouchEnded(bool multipleTouch, int fingerID, Vector2 pos);

        public void OnUpdate()
        {
            List<MoveEvent> moveEvents = new List<MoveEvent>();
            bool multipleTouch = Input.touchCount > 1;

            float deltaTime = Time.deltaTime;
            for (int i = 0; i < Input.touchCount; ++i)
            {
                Touch t = Input.touches[i];
                Vector2 pos = t.position;

                switch (t.phase)
                {
                    case TouchPhase.Began:
                        bool isPointerOverUI = EventSystem.current == null ? false : EventSystem.current.IsPointerOverGameObject(t.fingerId);
                        if (OnTouchBeganCallback != null)
                            OnTouchBeganCallback(multipleTouch, t.fingerId, pos, isPointerOverUI);
                        break;
                    case TouchPhase.Moved:
                        moveEvents.Add(new MoveEvent(t.fingerId, pos, t.deltaPosition));
                        break;
                    case TouchPhase.Canceled:
                        if (OnTouchCancelCallback != null)
                            OnTouchCancelCallback(multipleTouch, t.fingerId, pos);
                        break;
                    case TouchPhase.Ended:
                        if (OnTouchBeganCallback != null)
                            OnTouchEndedCallback(multipleTouch, t.fingerId, pos);
                        break;
                }
            }

            HandleMoveEvent(moveEvents, deltaTime);
        }

        /// <summary>
        /// Add callback when touch is start.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void AddOnTouchBegan(OnTouchBegan func)
        {
            if (func == null)
                return;

            OnTouchBeganCallback += func;
        }

        /// <summary>
        /// Add callback when touch moves.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void AddOnTouchMoved(OnTouchMoved func)
        {
            if (func == null)
                return;

            OnTouchMovedCallback += func;
        }

        /// <summary>
        /// Add callback when pinch.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void AddOnTouchPinch(OnTouchPinch func)
        {
            if (func == null)
                return;

            OnTouchPinchCallback += func;
        }

        /// <summary>
        /// Add callback on touch cancel.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void AddOnTouchCancel(OnTouchCancel func)
        {
            if (func == null)
                return;

            OnTouchCancelCallback += func;
        }

        /// <summary>
        /// Add callback when touch ends.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void AddOnTouchEnded(OnTouchEnded func)
        {
            if (func == null)
                return;

            OnTouchEndedCallback += func;
        }

        /// <summary>
        /// Remove callback when touch is start.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void RemoveOnTouchBegan(OnTouchBegan func)
        {
            if (func == null)
                return;

            OnTouchBeganCallback -= func;
        }

        /// <summary>
        /// Remove callback when touch moves.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void RemoveOnTouchMoved(OnTouchMoved func)
        {
            if (func == null)
                return;

            OnTouchMovedCallback -= func;
        }

        /// <summary>
        /// Remove callback when pinch.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void RemoveOnTouchPinch(OnTouchPinch func)
        {
            if (func == null)
                return;

            OnTouchPinchCallback -= func;
        }

        /// <summary>
        /// Remove callback on touch cancel.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void RemoveOnTouchCancel(OnTouchCancel func)
        {
            if (func == null)
                return;

            OnTouchCancelCallback -= func;
        }

        /// <summary>
        /// Remove callback when touch ends.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void RemoveOnTouchEnded(OnTouchEnded func)
        {
            if (func == null)
                return;

            OnTouchEndedCallback -= func;
        }

        /// <summary>
        /// Remove all callbacks when touch is start.
        /// </summary>
        public void ClearOnTouchBegan()
        {
            foreach (OnTouchBegan d in OnTouchBeganCallback.GetInvocationList())
            {
                OnTouchBeganCallback -= d;
            }
            OnTouchBeganCallback = null;
        }

        /// <summary>
        /// Remove all callbacks when touch moves.
        /// </summary>
        public void ClearOnTouchMoved()
        {
            foreach (OnTouchMoved d in OnTouchMovedCallback.GetInvocationList())
            {
                OnTouchMovedCallback -= d;
            }
            OnTouchMovedCallback = null;
        }

        /// <summary>
        /// Remove all callbacks when pinch.
        /// </summary>
        public void ClearOnTouchPinch()
        {
            foreach (OnTouchPinch d in OnTouchPinchCallback.GetInvocationList())
            {
                OnTouchPinchCallback -= d;
            }
            OnTouchPinchCallback = null;
        }

        /// <summary>
        /// Remove all callbacks on touch canceled.
        /// </summary>
        public void ClearOnTouchCancel()
        {
            foreach (OnTouchCancel d in OnTouchCancelCallback.GetInvocationList())
            {
                OnTouchCancelCallback -= d;
            }
            OnTouchCancelCallback = null;
        }

        /// <summary>
        /// Remove all callbacks when touch ends.
        /// </summary>
        public void ClearOnTouchEnded()
        {
            foreach (OnTouchEnded d in OnTouchEndedCallback.GetInvocationList())
            {
                OnTouchEndedCallback -= d;
            }
            OnTouchEndedCallback = null;
        }

        /// <summary>
        /// Remove all callbacks.
        /// </summary>
        public void Clear()
        {
            ClearOnTouchBegan();
            ClearOnTouchMoved();
            ClearOnTouchPinch();
            ClearOnTouchCancel();
            ClearOnTouchEnded();
        }

        private void HandleMoveEvent(List<MoveEvent> moveEvents, float deltaTime)
        {
            if (OnTouchMovedCallback == null &&
                OnTouchPinchCallback == null)
                return;

            if (OnTouchPinchCallback != null)
            {
                for (int i = 0; i < moveEvents.Count - 1; ++i)
                {
                    for (int j = i + 1; j < moveEvents.Count; ++j)
                    {
                        float angle = Vector2.Angle(moveEvents[i].DeltaPosition, moveEvents[j].DeltaPosition);
                        if (angle >= 90)
                        {
                            List<int> fingerIDs 
                                = new List<int> { 
                                    moveEvents[i].FingerID, 
                                    moveEvents[j].FingerID 
                                };
                            List<Vector2> positions
                                = new List<Vector2> {
                                    moveEvents[i].Position,
                                    moveEvents[j].Position
                                };

                            float deltaMagnitude = Mathf.Abs((moveEvents[i].DeltaPosition+moveEvents[j].DeltaPosition).magnitude);
                            OnTouchPinchCallback(fingerIDs, positions, deltaMagnitude/deltaTime);
                            return;
                        }
                    }
                }
            }

            if (OnTouchMovedCallback != null)
            {
                foreach (MoveEvent i in moveEvents)
                {
                    OnTouchMovedCallback(moveEvents.Count>1, i.FingerID, i.Position, i.DeltaPosition.magnitude/deltaTime);
                }
            }
        }

        /// <summary>
        /// Callback of OnTouchBegan.
        /// </summary>
        public event OnTouchBegan OnTouchBeganCallback;

        /// <summary>
        /// Callback of OnTouchMoved.
        /// </summary>
        public event OnTouchMoved OnTouchMovedCallback;

        /// <summary>
        /// Callback of OnTouchPinch.
        /// </summary>
        public event OnTouchPinch OnTouchPinchCallback;

        /// <summary>
        /// Callback of OnTouchCancel.
        /// </summary>
        public event OnTouchCancel OnTouchCancelCallback;

        /// <summary>
        /// Callback of OnTouchEnded.
        /// </summary>
        public event OnTouchEnded OnTouchEndedCallback;
    }
}