using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayJam.Utils;
using PlayJam.World;
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

        [Header("특정 게임만 테스트할 때 사용")]
        [SerializeField]
        private bool _isTesting = false;

        [SerializeField]
        private EMiniGame _testTarget;

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
        public static UnityEvent OnMiniGameQuit;
        public static UnityEvent<bool> OnMiniGameEnd;

        private static WorldManager _worldManager;

        public static WorldManager WorldManager { get => _worldManager; }

        /// <summary>
        /// 게임 시작
        /// </summary>
        public IEnumerator Co_StartGame(bool isTestGame)
        {
            if (_worldManager == null)
                _worldManager = GameObject.Find("WorldManager")?.GetComponent<WorldManager>();

            if (isTestGame == true)
                MiniGameSharedData.Instance.Initialize(_miniGameConfig, _miniGames, _isTesting, _testTarget);
            else
                MiniGameSharedData.Instance.Initialize(_miniGameConfig, _miniGames, false, _testTarget);

            OnMiniGamePrevStart = new UnityEvent();
            OnMiniGameStart = new UnityEvent();
            OnMiniGamePostStart = new UnityEvent();
            OnMiniGameResume = new UnityEvent();
            OnMiniGamePause = new UnityEvent();
            OnMiniGameEnd = new UnityEvent<bool>();
            OnMiniGameQuit = new UnityEvent();


            // Datas should be refreshed first.
            if (_worldManager != null)
            {
                OnMiniGameStart.AddListener(_worldManager.HideAnimGrass);
                OnMiniGameEnd.AddListener((b) => _worldManager.ShowAnimGrass());
            }

            OnMiniGamePrevStart.AddListener(MiniGameSharedData.Instance.OnMiniGamePrevStart);
            OnMiniGameStart.AddListener(MiniGameSharedData.Instance.OnMiniGameStart);
            OnMiniGamePostStart.AddListener(MiniGameSharedData.Instance.OnMiniGamePostStart);
            OnMiniGameResume.AddListener(MiniGameSharedData.Instance.OnMiniGameResume);
            OnMiniGamePause.AddListener(MiniGameSharedData.Instance.OnMiniGamePause);
            OnMiniGameEnd.AddListener(MiniGameSharedData.Instance.OnMiniGameEnd);
            OnMiniGameQuit.AddListener(MiniGameSharedData.Instance.OnMiniGameQuit);

            RegisterControllers();
            Initialize();

            // 메인 컨트롤러 클리어
            OnMiniGameQuit.AddListener(Clear);

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
            RegisterController<MiniGameCustomerController>();
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
                StartCoroutine(_controllers[i].Co_PostInitialize(flag.Signal));
                flags.Add(flag);
            }

            for (int i = 0; i < _controllers.Count; i++)
            {
                yield return flags[i].Wait();
            }

            if (_worldManager != null)
                _worldManager.OnGameStart();

            flags.ForEach(x => x.Clear());

            for (int i = 0; i < _controllers.Count; i++)
            {
                StartCoroutine(_controllers[i].Co_PrevStartGame(flags[i].Signal));
            }

            for (int i = 0; i < _controllers.Count; i++)
            {
                yield return flags[i].Wait();
            }

            flags.ForEach(x => x.Clear());

            for (int i = 0; i < _controllers.Count; i++)
            {
                StartCoroutine(_controllers[i].Co_StartGame(flags[i].Signal));
            }

            for (int i = 0; i < _controllers.Count; i++)
            {
                yield return flags[i].Wait();
            }

            OnMiniGamePrevStart.Invoke();

            // 가장 마지막에 처리            OnMiniGameQuit.AddListener(Quit);
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
        /// 데이터 삭제
        /// </summary>
        public void Clear()
        {
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
        /// 
        /// </summary>
        public void Quit()
        {
            OnMiniGameQuit.RemoveAllListeners();
            OnMiniGameQuit = null;
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