using System;
using UnityEngine;
using UnityEngine.Events;

namespace ADOLoader.Component {
    public class GUILayout : MonoBehaviour {
        public UnityEvent OnGUIEvent = new UnityEvent();

        private void OnGUI() {
            OnGUIEvent.Invoke();
        }
    }
}