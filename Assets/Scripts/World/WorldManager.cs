using System.Collections;
using System.Collections.Generic;
using PlayJam.Character;
using PlayJam.InGame;
using PlayJam.Popup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayJam.World
{
    /// <summary>
    /// 
    /// </summary>
    public class WorldManager : MonoBehaviour
    {
        [SerializeField]
        private MainCharacter _characterShown;

        [SerializeField]
        private Button _btnStart;

        [SerializeField]
        private Button _btnRank;

        [SerializeField]
        private Button _btnGameList;

        [SerializeField]
        private Button _btnCostume;

        [SerializeField]
        private Button _btnSettings;

        [SerializeField]
        private MiniGameManager _miniGameManager;

        [SerializeField]
        private Animator _animGrass;

        [SerializeField]
        private Animator _animAri;

        [SerializeField]
        private GameObject _worldUI;

        [SerializeField]
        private TextMeshProUGUI _txtCarrot;

        private bool _isPlaying = false;

        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            _btnStart.onClick.AddListener(OnClickBtnStart);
            _btnRank.onClick.AddListener(OnClickBtnRank);
            _btnGameList.onClick.AddListener(OnClickBtnGameList);
            _btnCostume.onClick.AddListener(OnClickBtnCostume);
            _btnSettings.onClick.AddListener(OnClickBtnSettings);

            RefreshCoinCount();
            RefreshCostume();
        }

        private void OnClickBtnStart()
        {
            if (_isPlaying == true)
                return;

            _miniGameManager.gameObject.SetActive(true);
            StartCoroutine(_miniGameManager.Co_StartGame(false));

            SetPlayingFlag(true);
        }

        public void SetPlayingFlag(bool isPlaying)
        {
            _isPlaying = isPlaying;
        }

        public void OnGameStart()
        {
            _worldUI.gameObject.SetActive(false);
            _animAri.Play("Ari_FlyOut");
        }

        public void OnGameEnd()
        {
            _worldUI.gameObject.SetActive(true);
            _animAri.Play("Ari_FlyIn");
            ShowAnimGrass();
        }

        public void HideAnimGrass()
        {
            if (!_animGrass.GetCurrentAnimatorStateInfo(0).IsName("HIDE"))
                _animGrass.Play("HIDE");
        }

        public void ShowAnimGrass()
        {
            if (_animGrass.GetCurrentAnimatorStateInfo(0).IsName("HIDE"))
                _animGrass.Play("SHOW");
        }

        private void OnClickBtnRank()
        {
            if (_isPlaying == true)
                return;
        }

        private void OnClickBtnGameList()
        {
            if (_isPlaying == true)
                return;
        }

        private void OnClickBtnCostume()
        {
            if (_isPlaying == true)
                return;

            PopupManager.Instance.Show<PopupCostume>();
        }

        private void OnClickBtnSettings()
        {
            PopupManager.Instance.Show<PopupSettings>();
        }

        public void RefreshCoinCount()
        {
            _txtCarrot.text = UserDataHelper.Instance.Coin.ToString();
        }

        public void RefreshCostume()
        {
            _characterShown.SetCostume(UserDataHelper.Instance.EquippedCostume);
        }
    }
}
