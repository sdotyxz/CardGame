using Core.Src.Common;
using Core.Src.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Src.Module.Demo.View
{
    public class DemoPanel:Panel
    {
        private DemoUI _ui;
        protected override void _Init()
        {
            _ui = GetComponent<DemoUI>();
            Pool.GetComponent<UIEventListener>(_ui.btnButton).onClick += _OnClickBtn;

            base._Init();
        }

        private void _OnClickBtn(UnityEngine.GameObject go)
        {
            _ui.txtLabel.text = _ui.txtInput.value;
        }

        protected override void _Dispose()
        {
            Pool.GetComponent<UIEventListener>(_ui.btnButton).onClick -= _OnClickBtn;

            base._Dispose();
        }
    }
}
