using Core.Src.Config;
using Core.Src.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Src.Common
{
    public class Panel:MonoBehaviour
    {
        public const int BASE_DEPTH = 5;
        public const int EACH_POPUP_DEPTH = 5;
        /// <summary>
        /// not include fix depth panel
        /// </summary>
        private static List<Panel> _panels = new List<Panel>();
        public static List<Panel> panels { get { return _panels; } }
        /// <summary>
        /// include fix depth panel
        /// </summary>
        private static List<Panel> _allPanels = new List<Panel>();
        public static List<Panel> allPanels { get { return _allPanels; } }

        public float popOutTime = 0.15f;
        public EaseType popOutType = EaseType.easeInOutBack;

        public bool isPopup = true;
        public bool hasMask = true;

        private GameObject _mask;
        
        private IDictionary<UIPanel, int> _homeDepth = new Dictionary<UIPanel, int>();
        private bool _isInitedComplete = false;
        private bool _isStarted = false;

        /// <summary>
        /// 计算当前Panel的最高可用层级
        /// </summary>
        public int maxDepth { get { return BASE_DEPTH + _panels.Count * EACH_POPUP_DEPTH + 1; } }

        virtual public bool isFixDepth { get { return false; } }

        void Awake()
        {
            DisableTouch();
        }

        void Start()
        {
            UIPanel[] uiPanels = GetComponentsInChildren<UIPanel>(true);
            foreach (UIPanel uip in uiPanels)
            {
                _homeDepth.Add(uip, uip.depth);
            }
            _isStarted = true;
            _PreInit();
        }

        private void _PreInit()
        {
            if (hasMask)
            {
                _mask = Pool.Get(ResConfig.GUI_MASK, transform);
                Vector2 v = _GetMaxSize();
                UISprite ms = _mask.GetComponent<UISprite>();
                ms.width = (int)v.x;
                ms.height = (int)v.y;
                BoxCollider bc = _mask.GetComponent<BoxCollider>();
                bc.center = new Vector3(0f, 0f, 1.0f);
                bc.size = new Vector3(v.x, v.y, 1.0f);
            }

            if (isPopup)
            {
                iTweenExtensions.ScaleFrom(gameObject, Vector3.one * 0.01f, popOutTime, 0f, popOutType);
                Engine.instance.DoAfter(popOutTime + 0.02f, _PreOpened);
            }

            UIAnchor[] anchors = GetComponentsInChildren<UIAnchor>(true);
            foreach (UIAnchor a in anchors)
            {
                a.uiCamera = Scene.instance.uiCamera2D;
            }

            _AdjustUIPanel();

            _Init();
            
            if (!isPopup)
            {
                _PreOpened();
            }
        }

        private void _PreOpened()
        {
            _isInitedComplete = true;
            EnableTouch();

            _Opened();
        }

        private void _AdjustUIPanel()
        {
            if (!isFixDepth)
            {
                foreach (UIPanel uip in _homeDepth.Keys)
                {
                    uip.depth = _homeDepth[uip] + maxDepth;
                }
                _panels.Add(this);
            }
            _allPanels.Add(this);
        }

        protected virtual void _Init()
        {

        }

        protected virtual void _Opened()
        {
            
        }

        protected virtual void _Dispose()
        {

        }

        void OnDestroy()
        {
            if (!_isInitedComplete) EnableTouch();
            if (!_isStarted) return;

            if(_panels.Contains(this)) _panels.Remove(this);
            if(_allPanels.Contains(this)) _allPanels.Remove(this);

            _Dispose();

            iTween tween = GetComponent<iTween>();
            if (tween != null) Destroy(tween);

            if (_mask != null)
            {
                Destroy(_mask);
                _mask = null;
            }
        }
        
        public void DisableTouch()
        {
            UICamera.disableTouch = true;
        }

        public void EnableTouch()
        {
            UICamera.disableTouch = false;
        }

        protected Vector2 _GetMaxSize()
        {
            Vector2 v = new Vector2();
            if (AspectUtility.screenWidth * UIConfig.instance.uiSize.y > AspectUtility.screenHeight * UIConfig.instance.uiSize.x)
            {
                v.x = UIConfig.instance.uiSize.y * AspectUtility.screenWidth / AspectUtility.screenHeight;
                v.y = UIConfig.instance.uiSize.y;
            }
            else
            {
                v.x = UIConfig.instance.uiSize.x;
                v.y = UIConfig.instance.uiSize.x * AspectUtility.screenHeight / AspectUtility.screenWidth;
            }
            return v;
        }
    }
}
