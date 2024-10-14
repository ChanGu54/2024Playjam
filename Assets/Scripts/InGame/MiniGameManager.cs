using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayJam.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace PlayJam.InGame
{
    /// <summary>
    /// 
    /// </summary>
    public class MiniGameManager : MonoBehaviour
    {
        /// <summary>
        /// 등록된 컨트롤러
        /// </summary>
        private List<BaseMiniGameController> _controllers;

        /// <summary>
        /// 
        /// </summary>
        [Header("Config값")]
        [SerializeField]
        private MiniGameConfig _miniGameConfig;

        [Header("미니게임들")]
        [SerializeField]
        private List<MiniGameData> _miniGames;

        public static UnityEvent OnMiniGamePrevStart;
        public static UnityEvent OnMiniGameStart;
        public static UnityEvent OnMiniGamePostStart;
        public static UnityEvent OnMiniGameResume;
        public static UnityEvent OnMiniGamePause;
        public static UnityEvent<bool> OnMiniGameEnd;

        private void Awake()
        {
            StartCoroutine(Co_StartGame());
        }

        /// <summary>
        /// 게임 시작
        /// </summary>
        public IEnumerator Co_StartGame()
        {
            MiniGameSharedData.Instance.Initialize(_miniGameConfig, _miniGames);
            OnMiniGamePrevStart = new UnityEvent();
            OnMiniGameStart = new UnityEvent();
            OnMiniGamePostStart = new UnityEvent();
            OnMiniGameResume = new UnityEvent();
            OnMiniGamePause = new UnityEvent();
            OnMiniGameEnd = new UnityEvent<bool>();

            // Datas should be refreshed first.
            OnMiniGamePrevStart.AddListener(MiniGameSharedData.Instance.OnMiniGamePrevStart);
            OnMiniGameStart.AddListener(MiniGameSharedData.Instance.OnMiniGameStart);
            OnMiniGamePostStart.AddListener(MiniGameSharedData.Instance.OnMiniGamePostStart);
            OnMiniGameResume.AddListener(MiniGameSharedData.Instance.OnMiniGameResume);
            OnMiniGamePause.AddListener(MiniGameSharedData.Instance.OnMiniGamePause);
            OnMiniGameEnd.AddListener(MiniGameSharedData.Instance.OnMiniGameEnd);

            RegisterControllers();
            Initialize();
            yield return Co_StartProcess();
        }

        /// <summary>
        /// 컨트롤러 등록
        /// </summary>
        private void RegisterControllers()
        {
            _controllers = new List<BaseMiniGameController>();

            RegisterController<MiniGameTimeController>();
            RegisterController<MiniGameRuntimeController>();
            RegisterController<MiniGameUIController>();
        }

        /// <summary>
        /// 게임 시작
        /// </summary>
        /// <returns></returns>
        private IEnumerator Co_StartProcess()
        {
            List<WaitForSignal> flags = new List<WaitForSignal>();

            for (int i = 0; i < _controllers.Count; i++)
            {
                WaitForSignal flag = new WaitForSignal();
                StartCoroutine(_controllers[i].Co_PrevStartGame(flag.Signal));
                flags.Add(flag);
            }

            for (int i = 0; i < _controllers.Count; i++)
            {
                yield return flags[i].Wait();
            }

            flags.Clear();

            for (int i = 0; i < _controllers.Count; i++)
            {
                WaitForSignal flag = new WaitForSignal();
                StartCoroutine(_controllers[i].Co_StartGame(flag.Signal));
                flags.Add(flag);
            }

            for (int i = 0; i < _controllers.Count; i++)
            {
                yield return flags[i].Wait();
            }

            OnMiniGamePrevStart.Invoke();
        }

        /// <summary>
        /// 게임 초기화 
        /// </summary>
        private void Initialize()
        {
            _controllers.ForEach(x => x.Initialize());
        }

        /// <summary>
        /// 게임 종료
        /// </summary>
        public IEnumerator Co_EndGameProcess()
        {
            List<WaitForSignal> flags = new List<WaitForSignal>();

            for (int i = 0; i < _controllers.Count; i++)
            {
                WaitForSignal flag = new WaitForSignal();
                StartCoroutine(_controllers[i].Co_PrevEndGame(flag.Signal));
                flags.Add(flag);
            }

            for (int i = 0; i < _controllers.Count; i++)
            {
                yield return flags[i].Wait();
            }

            flags.Clear();

            for (int i = 0; i < _controllers.Count; i++)
            {
                WaitForSignal flag = new WaitForSignal();
                StartCoroutine(_controllers[i].Co_EndGame(flag.Signal));
                flags.Add(flag);
            }

            for (int i = 0; i < _controllers.Count; i++)
            {
                yield return flags[i].Wait();
            }
        }

        /// <summary>
        /// 게임 재시작
        /// </summary>
        public void RestartGame()
        {
            Clear();
            StartCoroutine(Co_StartGame());
        }

        /// <summary>
        /// 데이터 삭제
        /// </summary>
        public void Clear()
        {
            MiniGameSharedData.Instance.Clear();

            _controllers.ForEach(x => x.Clear());
            _controllers.Clear();

            _controllers = null;

            OnMiniGamePrevStart.RemoveAllListeners();
            OnMiniGameStart.RemoveAllListeners();
            OnMiniGamePostStart.RemoveAllListeners();
            OnMiniGameResume.RemoveAllListeners();
            OnMiniGamePause.RemoveAllListeners();
            OnMiniGameEnd.RemoveAllListeners();

            OnMiniGamePrevStart = null;
            OnMiniGameStart = null;
            OnMiniGamePostStart = null;
            OnMiniGameResume = null;
            OnMiniGamePause = null;
            OnMiniGameEnd = null;
        }

        /// <summary>
        /// 컨트롤러 추가
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterController<T>() where T : BaseMiniGameController
        {
            _controllers.Add(GetComponent<T>());
        }
    }
}