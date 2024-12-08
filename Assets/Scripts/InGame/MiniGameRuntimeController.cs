
using PlayJam.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayJam.InGame
{
    /// <summary>
    /// 미니게임 실행부
    /// </summary>
    public class MiniGameRuntimeController : BaseMiniGameController
    {
        [Serializable]
        public class MiniGameDic
        {
            public EMiniGame MiniGameKind;
            public MiniGamePlayer MiniGameInstance;
        }

        [SerializeField]
        private List<MiniGameDic> _miniGameDics;

        private MiniGameDic _pickedMiniGame;

        /// <summary>
        /// 게임 초기화
        /// </summary>
        public override void Initialize()
        {
            MiniGameManager.OnMiniGamePrevStart.AddListener(OnMiniGamePrevStart);
            MiniGameManager.OnMiniGameStart.AddListener(OnMiniGameStart);
            MiniGameManager.OnMiniGamePostStart.AddListener(OnMiniGamePostStart);
            MiniGameManager.OnMiniGameEnd.AddListener(OnMiniGameEnd);
            MiniGameManager.OnMiniGamePause.AddListener(OnMiniGamePause);
            MiniGameManager.OnMiniGameResume.AddListener(OnMiniGameResume);
            MiniGameManager.OnMiniGameQuit.AddListener(OnMiniGameQuit);
        }

        private void OnMiniGamePrevStart()
        {
            StartCoroutine(Co_OnMiniGamePrevStart());
        }

        private IEnumerator Co_OnMiniGamePrevStart()
        {
            yield return new WaitForSeconds(2f);

            if (_pickedMiniGame != null)
            {
                _pickedMiniGame.MiniGameInstance.Clear();
                _pickedMiniGame.MiniGameInstance.gameObject.SetActive(false);
                _pickedMiniGame = null;
            }

            for (int i = 0; i < _miniGameDics.Count; i++)
            {
                if (_miniGameDics[i].MiniGameKind == MiniGameSharedData.Instance.CurrentMiniGameData.GameKind)
                {
                    _pickedMiniGame = _miniGameDics[i];
                }
            }

            _pickedMiniGame.MiniGameInstance.gameObject.SetActive(true);
            _pickedMiniGame.MiniGameInstance.Initialize(MiniGameSharedData.Instance.CurrentMiniGameData);

            MiniGameManager.OnMiniGameStart.Invoke();
        }

        private void OnMiniGameStart()
        {
            StartCoroutine(_pickedMiniGame.MiniGameInstance.OnStart());
        }

        private void OnMiniGamePostStart()
        {
            _pickedMiniGame.MiniGameInstance.OnPostStart();
        }

        private void OnMiniGameResume()
        {
            StartCoroutine(_pickedMiniGame.MiniGameInstance.OnResume());
        }

        private void OnMiniGamePause()
        {
            StartCoroutine(_pickedMiniGame.MiniGameInstance.OnPause());
        }

        private void OnMiniGameEnd(bool isSuccess)
        {
            Action callback = new Action(() =>
            {
            });

            if (isSuccess == true)
            {
                SoundManager.Instance.Play(ESoundType.SFX, "GameClear");
                StartCoroutine(_pickedMiniGame.MiniGameInstance.OnSuccess(callback));
            }
            else
            {
                SoundManager.Instance.Play(ESoundType.SFX, "GameFail");
                StartCoroutine(_pickedMiniGame.MiniGameInstance.OnFail(callback));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnMiniGameQuit()
        {
            Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            base.Clear();

            if (_pickedMiniGame != null)
            {
                _pickedMiniGame.MiniGameInstance.Clear();
                _pickedMiniGame.MiniGameInstance.gameObject.SetActive(false);
            }

            _pickedMiniGame = null;
        }
    }
}
