using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace PlayJam.Popup
{
    public class PopupManager : MonoBehaviour
    {
        #region Singleton
        private static PopupManager _instance;

        public static PopupManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.Find("PopupManager")?.GetComponent<PopupManager>();
                    _instance.Initialize();
                }

                return _instance;
            }
        }
        #endregion

        [SerializeField]
        private Image _dimmed;

        [SerializeField]
        private List<BasePopup> _popups;

        private BasePopup _activePopup;

        private TweenerCore<Color, Color, ColorOptions> _tweenInstance;

        public void Initialize()
        {
            _activePopup = null;
            _dimmed.gameObject.SetActive(false);
            _tweenInstance = null;
        }

        public void Show<T>() where T : BasePopup
        {
            if (_activePopup != null)
            {
                Debug.LogWarning("[PopupManager.Show] 팝업 이미 1개 켜져있음");
                return;
            }

            for (int i = 0; i < _popups.Count; i++)
            {
                if (_popups[i] is T)
                {
                    _activePopup = _popups[i];
                    break;
                }
            }

            if (_activePopup != null)
            {
                if (_activePopup.IsOpening || _activePopup.IsClosing)
                {
                    _activePopup = null;
                    Debug.LogWarning("[PopupManager.Show] 팝업 뽑았는데 팝업 상태가 이상하네?");
                    return;
                }

                _activePopup.Open();

                if (_tweenInstance != null)
                {
                    _tweenInstance.Kill();
                    _tweenInstance = null;
                }

                _dimmed.gameObject.SetActive(true);
                _tweenInstance = DOTween.ToAlpha(() => _dimmed.color, x => _dimmed.color = x, 0.9f, 1 / 6f).OnComplete(() =>
                {
                    _tweenInstance = null;
                });
            }
        }

        public void Exit<T>() where T : BasePopup
        {
            if (_activePopup == null)
            {
                Debug.LogWarning("[PopupManager.Exit] 팝업 나갈려고 했는데 왜 팝업이 없지?");
                return;
            }

            if (_activePopup is not T)
            {
                Debug.LogWarning("[PopupManager.Exit] 왜 다른 팝업이 열려있지?");
                return;
            }

            if (_activePopup.IsOpening || _activePopup.IsClosing)
            {
                Debug.LogWarning("[PopupManager.Exit] 오픈 중이거나 종료 중");
                return;
            }    

            _activePopup.Close();

            if (_tweenInstance != null)
            {
                _tweenInstance.Kill();
                _tweenInstance = null;
            }

            _tweenInstance = DOTween.ToAlpha(() => _dimmed.color, x => _dimmed.color = x, 0, 1 / 6f).OnComplete(() => {
                _dimmed.gameObject.SetActive(false);
                _tweenInstance = null;
            });
            _activePopup = null;
        }
    }
}