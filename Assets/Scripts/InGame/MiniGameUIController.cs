using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlayJam.InGame.UI;
using PlayJam.Utils;
using UnityEngine;

namespace PlayJam.InGame
{
    /// <summary>
    /// UI 관리
    /// </summary>
    public class MiniGameUIController : BaseMiniGameController
    {
        [SerializeField]
        private MiniGameTimerUI _timerUI;

        [SerializeField]
        private MiniGameTransitionUI _transitionUI;

        private List<BaseMiniGameUI> _miniGameUIs;

        public override void Initialize()
        {
            _miniGameUIs = new List<BaseMiniGameUI>
            {
                _timerUI,
                _transitionUI
            };

            for (int i = 0; i < _miniGameUIs.Count; i++)
            {
                _miniGameUIs[i].Initialize();
            }

            MiniGameManager.OnMiniGameStart.AddListener(OnMiniGameStart);
            MiniGameManager.OnMiniGamePause.AddListener(OnMiniGamePause);
            MiniGameManager.OnMiniGameResume.AddListener(OnMiniGameResume);
            MiniGameManager.OnMiniGameEnd.AddListener(OnMiniGameEnd);
            MiniGameManager.OnMiniGameQuit.AddListener(OnMiniGameQuit);
        }

        public override IEnumerator Co_PostInitialize(Action inEndCallback)
        {
            for (int i = 0; i < _miniGameUIs.Count; i++)
            {
                _miniGameUIs[i].gameObject.SetActive(true);
            }

            yield return null;

            List<WaitForSignal> flags = new List<WaitForSignal>();

            for (int i = 0; i < _miniGameUIs.Count; i++)
            {
                flags.Add(new WaitForSignal());
                yield return _miniGameUIs[i].OnPostInitialize(flags[i].Signal);
            }

            for (int i = 0; i < _miniGameUIs.Count; i++)
            {
                yield return flags[i].Wait();
            }

            inEndCallback.Invoke();
        }

        public void OnMiniGameStart()
        {
            for (int i = 0; i < _miniGameUIs.Count; i++)
            {
                _miniGameUIs[i].OnMiniGameStart();
            }
        }

        public void OnMiniGamePause()
        {
            for (int i = 0; i < _miniGameUIs.Count; i++)
            {
                _miniGameUIs[i].OnMiniGamePause();
            }
        }

        public void OnMiniGameResume()
        {
            for (int i = 0; i < _miniGameUIs.Count; i++)
            {
                _miniGameUIs[i].OnMiniGameResume();
            }
        }

        public void OnMiniGameEnd(bool isSuccess)
        {
            StartCoroutine(Co_OnMiniGameEnd(isSuccess));
        }

        private IEnumerator Co_OnMiniGameEnd(bool isSuccess)
        {
            List<WaitForSignal> flags = new List<WaitForSignal>();

            for (int i = 0; i < _miniGameUIs.Count; i++)
            {
                WaitForSignal flag = new WaitForSignal();
                StartCoroutine(_miniGameUIs[i].Co_OnMiniGameEnd(isSuccess, flag.Signal));
                flags.Add(flag);
            }

            for (int i = 0; i < flags.Count; i++)
            {
                yield return flags[i].Wait();
            }

            if (MiniGameSharedData.Instance.HeartCount > 0)
                MiniGameManager.OnMiniGamePrevStart.Invoke();
            else
            {
                MiniGameManager.WorldManager.OnGameEnd();
                MiniGameManager.OnMiniGameQuit.Invoke();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnMiniGameQuit()
        {
            List<WaitForSignal> flags = new List<WaitForSignal>();

            for (int i = 0; i < _miniGameUIs.Count; i++)
            {
                flags.Add(new WaitForSignal());
                StartCoroutine(_miniGameUIs[i].Co_OnMiniGameQuit(flags.Last().Signal));
            }

            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => flags.TrueForAll(x => x.HasSignal()));
                Clear();

                MiniGameManager.WorldManager.SetPlayingFlag(false);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            for (int i = 0; i < _miniGameUIs.Count; i++)
            {
                _miniGameUIs[i].gameObject.SetActive(false);
            }

            _miniGameUIs.ForEach(x => x.Clear());
            _miniGameUIs = null;
        }
    }
}