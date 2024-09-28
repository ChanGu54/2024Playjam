using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayJam.Utils;
using UnityEngine;

namespace PlayJam.MiniGame
{

    public class MiniGameManager : MonoBehaviour
    {
        /// <summary>
        /// 등록된 컨트롤러
        /// </summary>
        private List<BaseMiniGameController> _controllers;

        [Header("Config값")]
        [SerializeField]
        private List<MiniGameConfig> _miniGameConfigs;

        [Header("선택된 난이도")]
        [SerializeField]
        private EMiniGameDifficulty _miniGameDifficulty;

        /// <summary>
        /// 선택된 Config값
        /// </summary>
        private MiniGameConfig _selectedConfig;
    
        /// <summary>
        /// 게임 시작
        /// </summary>
        public IEnumerator Co_StartGame(EMiniGameDifficulty inDifficulty)
        {
            _miniGameDifficulty = inDifficulty;
            _selectedConfig = _miniGameConfigs.First(x => x.Difficulty == inDifficulty);

            MiniGameSharedData.Instance.Initialize(_selectedConfig);

            RegisterControllers();
            Initialize();
            yield return Co_StartProcess();
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
            }

            for (int i = 0; i < _controllers.Count; i++)
            {
                yield return flags[i].Wait();
            }

            for (int i = 0; i < _controllers.Count; i++)
            {
                WaitForSignal flag = new WaitForSignal();
                StartCoroutine(_controllers[i].Co_StartGame(flag.Signal));
            }

            for (int i = 0; i < _controllers.Count; i++)
            {
                yield return flags[i].Wait();
            }
        }

        /// <summary>
        /// 게임 초기화 
        /// </summary>
        private void Initialize()
        {
            _controllers.ForEach(x => x.Initialize());
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
        /// 게임 종료
        /// </summary>
        public IEnumerator Co_EndGameProcess()
        {
            List<WaitForSignal> flags = new List<WaitForSignal>();

            for (int i = 0; i < _controllers.Count; i++)
            {
                WaitForSignal flag = new WaitForSignal();
                StartCoroutine(_controllers[i].Co_PrevEndGame(flag.Signal));
            }

            for (int i = 0; i < _controllers.Count; i++)
            {
                yield return flags[i].Wait();
            }

            for (int i = 0; i < _controllers.Count; i++)
            {
                WaitForSignal flag = new WaitForSignal();
                StartCoroutine(_controllers[i].Co_EndGame(flag.Signal));
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
            EMiniGameDifficulty cachedDifficulty = _miniGameDifficulty;
            Clear();

            StartCoroutine(Co_StartGame(cachedDifficulty));
        }

        /// <summary>
        /// 데이터 삭제
        /// </summary>
        public void Clear()
        {
            MiniGameSharedData.Instance.Clear();
            _selectedConfig = null;

            _controllers.ForEach(x => x.Clear());
            _controllers.Clear();

            _controllers = null;
        }

        /// <summary>
        /// 컨트롤러 추가
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterController<T>() where T : BaseMiniGameController
        {
            _controllers.Add(Activator.CreateInstance<T>());
        }
    }
}