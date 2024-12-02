using System.Collections;
using System.Collections.Generic;
using PlayJam.Character;
using PlayJam.InGame;
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
        private GameObject _worldUI;

        private bool _isPlaying = false;

        private void Awake()
        {
            _characterShown.SetCostume(UserDataHelper.Instance.EquippedCostume);
            _btnStart.onClick.AddListener(OnClickBtnStart);
            _btnRank.onClick.AddListener(OnClickBtnRank);
            _btnGameList.onClick.AddListener(OnClickBtnGameList);
            _btnCostume.onClick.AddListener(OnClickBtnCostume);
            _btnSettings.onClick.AddListener(OnClickBtnSettings);
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
        }

        public void OnGameEnd()
        {
            _worldUI.gameObject.SetActive(true);
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
        }

        private void OnClickBtnSettings()
        {
            if (_isPlaying == true)
                return;
        }
    }
}
