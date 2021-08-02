// TODO: 새로운 GUI 시스템 만들기

/*
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ADOLoader.GUI {
    public enum Anchor {
        UpperLeft,
        UpperCenter,
        UpperRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
    }
    
    public abstract class GUIComponent : MonoBehaviour {
        protected GUIComponent(Rect rect, Anchor anchor) {
            Anchor = anchor;
            Rect = rect;
        }

        private GUIComponent _parent;

        public GUIComponent Parent {
            get => _parent;
            set {
                _parent = value;
                var transform = this.transform;
                transform.parent = _parent.transform;
                transform.position = Vector3.zero;
            }
        }
        private RectTransform _rectTransform;

        public RectTransform RectTransform {
            get {
                _rectTransform ??= GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public Anchor Anchor { get; }

        public Rect Rect {
            get {
                var position = RectTransform.localPosition;
                var sizeDelta = RectTransform.sizeDelta;
                return new Rect(position.x, position.y, sizeDelta.x, sizeDelta.y);
            }
            set {
                RectTransform.localPosition = new Vector3(value.x, value.y, 0);
                RectTransform.sizeDelta = new Vector2(value.width, value.height);
            }
        }
    }

    public class Area : GUIComponent {
        internal Area(Rect rect, Anchor anchor) : base(rect, anchor) { }

        private List<GUIComponent> _childs = new();
        public List<GUIComponent> Childs => _childs;

        public void AddChild(GUIComponent component) {
            component.Parent = this;
            _childs.Add(component);
        }
        public void AddChilds(params GUIComponent[] components) {
            foreach (var component in components) {
                AddChild(component);
            }
        }
    }

    public class Text : GUIComponent {
        internal Text(Rect rect, Anchor anchor) : base(rect, anchor) { }
    }
}
*/