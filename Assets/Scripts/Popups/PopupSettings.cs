using PlayJam.Sound;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayJam.Popup
{
    public enum ESettingPopupState
    {
        SETTING,
        CREDIT,
    }

    public class PopupSettings : BasePopup
    {
        [SerializeField]
        private GameObject _goRootSetting;

        [SerializeField]
        private GameObject _goRootCredit;

        [Header("세팅 관련")]
        [SerializeField]
        private Button _btnConvertToCredit;

        [SerializeField]
        private Button _btnBGM;
        [SerializeField]
        private Image _imgBGMHandle;

        [SerializeField]
        private Button _btnSFX;
        [SerializeField]
        private Image _imgSFXHandle;

        [SerializeField]
        private TMP_InputField _inputName;
        [SerializeField]
        private Button _btnNameConfirm;

        [SerializeField]
        private Button _btnX;

        [Header("크레딧 관련")]
        [SerializeField]
        private Button _btnBack;

        [SerializeField]
        private RectTransform _rtContent;
        [SerializeField]
        private ScrollRect _scrollRectCredit;
        [SerializeField]
        private DragHandler _scrollRectHandler;

        private Vector3 _creditStartPos = new Vector3(0, 280, 0);
        private Vector3 _creditEndPos = new Vector3(0, 1257, 0);

        private ESettingPopupState _state;

        public override void PrevOpen()
        {
            _state = ESettingPopupState.SETTING;

            SetUI(_state);
        }

        private void SetUI(ESettingPopupState inState)
        {
            switch (inState)
            {
                case ESettingPopupState.SETTING:
                    _goRootSetting.SetActive(true);
                    _goRootCredit.SetActive(false);

                    InitializeSettingPopup();

                    break;
                case ESettingPopupState.CREDIT:
                    _goRootSetting.SetActive(false);
                    _goRootCredit.SetActive(true);

                    InitializeCreditPopup();

                    break;
            }
        }

        private void InitializeSettingPopup()
        {
            _btnX.onClick.RemoveAllListeners();
            _btnX.onClick.AddListener(PopupManager.Instance.Exit<PopupSettings>);

            _btnConvertToCredit.onClick.RemoveAllListeners();
            _btnConvertToCredit.onClick.AddListener(OnClickBtnCredit);

            _btnBGM.onClick.RemoveAllListeners();
            _btnSFX.onClick.RemoveAllListeners();

            _btnBGM.onClick.AddListener(OnClickBtnBGM);
            _btnSFX.onClick.AddListener(OnClickBtnSFX);

            _inputName.text = UserDataHelper.Instance.Name;
            _btnNameConfirm.onClick.RemoveAllListeners();
            _btnNameConfirm.onClick.AddListener(OnClickBtnNameConfirm);

            SetBGMUI();
            SetSFXUI();
        }

        private void OnClickBtnCredit()
        {
            _state = ESettingPopupState.CREDIT;

            SetUI(_state);
        }

        private void OnClickBtnNameConfirm()
        {
            UserDataHelper.Instance.Name = _inputName.text;
        }

        private void OnClickBtnBGM()
        {
            if (UserDataHelper.Instance.IsBGMOn == true)
            {
                UserDataHelper.Instance.IsBGMOn = false;
                (_imgBGMHandle.transform as RectTransform).anchoredPosition = new Vector3(-48, 0.48f, 0);
                SoundManager.Instance.Stop(ESoundType.BGM);
            }
            else if (UserDataHelper.Instance.IsBGMOn == false)
            {
                UserDataHelper.Instance.IsBGMOn = true;
                (_imgBGMHandle.transform as RectTransform).anchoredPosition = new Vector3(48, 0.48f, 0);
                SoundManager.Instance.Play(ESoundType.BGM, "MainBGM", true);
            }
        }

        private void OnClickBtnSFX()
        {
            if (UserDataHelper.Instance.IsSFXOn == true)
            {
                UserDataHelper.Instance.IsSFXOn = false;
                (_imgSFXHandle.transform as RectTransform).anchoredPosition = new Vector3(-48, 0.48f, 0);
                SoundManager.Instance.Stop(ESoundType.SFX);
            }
            else if (UserDataHelper.Instance.IsSFXOn == false)
            {
                UserDataHelper.Instance.IsSFXOn = true;
                (_imgSFXHandle.transform as RectTransform).anchoredPosition = new Vector3(48, 0.48f, 0);
            }
        }

        private void SetBGMUI()
        {
            if (UserDataHelper.Instance.IsBGMOn == true)
            {
                (_imgBGMHandle.transform as RectTransform).anchoredPosition = new Vector3(48, 0.48f, 0);
            }
            else if (UserDataHelper.Instance.IsBGMOn == false)
            {
                (_imgBGMHandle.transform as RectTransform).anchoredPosition = new Vector3(-48, 0.48f, 0);
            }
        }

        private void SetSFXUI()
        {
            if (UserDataHelper.Instance.IsSFXOn == true)
            {
                (_imgSFXHandle.transform as RectTransform).anchoredPosition = new Vector3(48, 0.48f, 0);
            }
            else if (UserDataHelper.Instance.IsSFXOn == false)
            {
                (_imgSFXHandle.transform as RectTransform).anchoredPosition = new Vector3(-48, 0.48f, 0);
            }
        }

        private void InitializeCreditPopup()
        {
            _btnBack.onClick.RemoveAllListeners();
            _btnBack.onClick.AddListener(OnClickBtnBack);

            _rtContent.anchoredPosition = _creditStartPos;
            _scrollRectHandler.OnBeginDragEvent = new UnityEngine.Events.UnityEvent();
            _scrollRectHandler.OnDragEvent = new UnityEngine.Events.UnityEvent();
            _scrollRectHandler.OnEndDragEvent = new UnityEngine.Events.UnityEvent();

            _scrollRectHandler.OnBeginDragEvent.AddListener(OnBeginDrag);
            _scrollRectHandler.OnDragEvent.AddListener(OnDrag);
            _scrollRectHandler.OnEndDragEvent.AddListener(OnEndDrag);

            StartCoroutine(Co_ShowCreditAutoScroll(0));
        }

        private IEnumerator Co_ShowCreditAutoScroll(float waitTime = 0f)
        {
            yield return new WaitForSeconds(waitTime);

            while (_rtContent.anchoredPosition.y < _creditEndPos.y)
            {
                _rtContent.anchoredPosition = _rtContent.anchoredPosition + Vector2.up;
                yield return null;
            }
        }

        private void OnBeginDrag()
        {
            StopAllCoroutines();
        }

        private void OnDrag()
        {

        }

        private void OnEndDrag()
        {
            StartCoroutine(Co_ShowCreditAutoScroll(1));
        }

        private void OnClickBtnBack()
        {
            _state = ESettingPopupState.SETTING;

            SetUI(_state);
        }

        public override void Clear()
        {
            StopAllCoroutines();
            base.Clear();
        }
    }
}
