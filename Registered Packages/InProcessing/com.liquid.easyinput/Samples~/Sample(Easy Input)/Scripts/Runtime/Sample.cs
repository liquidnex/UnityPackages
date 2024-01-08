using UnityEngine;

namespace Liquid.EasyInput.Samples.EasyInputSample
{
    public class Sample : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            EasyInputManager.Instance.Mouse.OnClickBeganCallback += OnClickBegan;

            EasyInputManager.Instance.Mouse.OnClickMovedCallback += OnClickMoved;

            EasyInputManager.Instance.Mouse.OnClickEndedCallback += OnClickEnded;

            EasyInputManager.Instance.Keyboard.OnKeyDownCallback += OnSpaceKeyDown;
        }

        private void OnClickBegan(Vector2 pos, bool b, int button = 0)
        {
            if (b)
                return;

            Debug.Log(string.Format("On Click Began, Pos: {0}, Is Pointer Over UI: {1}, Button Idx: {2}", pos, b, button));
        }

        private void OnClickMoved(Vector2 pos, float deltaPerSec, bool b, int button = 0)
        {
            if (b)
                return;

            Debug.Log(string.Format("On Click Moved, Pos: {0}, Speed: {1}, Is Pointer Over UI: {2}, Button Idx: {3}", pos, deltaPerSec, b, button));
        }

        private void OnClickEnded(Vector2 pos, bool b, int button = 0)
        {
            if (b)
                return;

            Debug.Log(string.Format("On Click Ended, Pos: {0}, Is Pointer Over UI: {1}, Button Idx: {2}", pos, b, button));
        }

        private void OnSpaceKeyDown(KeyCode k = KeyCode.Space)
        {
            Debug.Log(string.Format("{0} Key Down", k));
        }
    }
}