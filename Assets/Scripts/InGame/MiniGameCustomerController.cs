using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PlayJam.Character.NPC;
using PlayJam.Utils;
using UnityEngine;
using static PlayJam.InGame.MiniGameRuntimeController;

namespace PlayJam.InGame
{
    /// <summary>
    /// 미니게임 손님 처리
    /// </summary>
    public class MiniGameCustomerController : BaseMiniGameController
    {
        [SerializeField]
        private Transform _customerRoot;

        [SerializeField]
        private List<Customer> _allCustomers;

        private List<Customer> _customerList = new List<Customer>();

        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            base.Clear();

            for (int i = 0; i < _customerList.Count; i++)
            {
                Destroy(_customerList[i].gameObject);
            }

            _customerList.Clear();
        }

        /// <summary>
        /// 게임 초기화
        /// </summary>
        public override void Initialize()
        {
            List<MiniGameData> electedMiniGames = MiniGameSharedData.Instance.ReservedMiniGameDatas;
            List<MiniGameData> allElectedMiniGames = new List<MiniGameData>();
            allElectedMiniGames.Add(MiniGameSharedData.Instance.CurrentMiniGameData);
            allElectedMiniGames.AddRange(electedMiniGames);

            float maxDuration = 1f;
            float durationPerDist = maxDuration / 880f;
            float moveTimePerCustomerPos = 140 * durationPerDist;
            float walkAnimDuration = moveTimePerCustomerPos / 2;

            for (int i = 0; i < allElectedMiniGames.Count; i++)
            {
                Customer electedCustomerPrefab = _allCustomers[UnityEngine.Random.Range(0, _allCustomers.Count)];
                Customer customerInstance = Instantiate(electedCustomerPrefab.gameObject, _customerRoot).GetComponent<Customer>();
                customerInstance.transform.localPosition = new Vector3(600, 0, 0);
                customerInstance.ShowFoodResource(allElectedMiniGames[i].Food);
                float endX = (i - 2) * 140; // -280 -140 0 140 280
                float moveDist = endX - 600;
                float moveTime = durationPerDist * Mathf.Abs(moveDist);
                float waitTime = i == 0 ? 0 : maxDuration - moveTime;

                StartCoroutine(TimerCoroutine(waitTime, () =>
                {
                    for (int j = 0; j < (moveTime / walkAnimDuration) / 2f; j++)
                    {
                        StartCoroutine(TimerCoroutine(walkAnimDuration * (j * 2), () => customerInstance.transform.DOLocalMoveY(25, walkAnimDuration).SetEase(Ease.Linear)));
                        StartCoroutine(TimerCoroutine(walkAnimDuration * (j * 2 + 1), () => customerInstance.transform.DOLocalMoveY(0, walkAnimDuration).SetEase(Ease.Linear)));
                    }
                    customerInstance.transform.DOLocalMoveX(endX, moveTime).SetEase(Ease.Linear);
                }));

                _customerList.Add(customerInstance);
            }

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
        }

        private void OnMiniGameStart()
        {
        }

        private void OnMiniGamePostStart()
        {
        }

        private void OnMiniGameResume()
        {
        }

        private void OnMiniGamePause()
        {
        }

        private void OnMiniGameEnd(bool isSuccess)
        {
            float maxDuration = 3f;
            float durationPerDist = maxDuration / 880f;
            float moveTimePerCustomerPos = 140 * durationPerDist;
            float walkAnimDuration = moveTimePerCustomerPos / 6;

            List<MiniGameData> electedMiniGames = MiniGameSharedData.Instance.ReservedMiniGameDatas;

            Customer electedCustomerPrefab = _allCustomers[UnityEngine.Random.Range(0, _allCustomers.Count)];
            Customer customerInstance = Instantiate(electedCustomerPrefab.gameObject, _customerRoot).GetComponent<Customer>();
            customerInstance.transform.localPosition = new Vector3(480, 0, 0);
            customerInstance.ShowFoodResource(electedMiniGames[electedMiniGames.Count - 1].Food);
            _customerList.Add(customerInstance);

            if (_customerList != null && _customerList.Count == 7)
            {
                Destroy(_customerList[0].gameObject);
                _customerList.RemoveAt(0);
            }

            for (int i = 0; i < _customerList.Count; i++)
            {
                for (int j = 0; j < (moveTimePerCustomerPos / walkAnimDuration) / 2f; j++)
                {
                    int cachedI = i;

                    StartCoroutine(TimerCoroutine(walkAnimDuration * (j * 2),
                        () =>
                        _customerList[cachedI].transform.DOLocalMoveY(25, walkAnimDuration).SetEase(Ease.Linear)
                        ));
                    StartCoroutine(TimerCoroutine(walkAnimDuration * (j * 2 + 1),
                        () =>
                        _customerList[cachedI].transform.DOLocalMoveY(0, walkAnimDuration).SetEase(Ease.Linear)
                        ));
                }

                if (i == 0 || i == _customerList.Count - 1)
                    _customerList[i].transform.DOLocalMoveX(_customerList[i].transform.localPosition.x - 200, moveTimePerCustomerPos).SetEase(Ease.Linear);
                else
                    _customerList[i].transform.DOLocalMoveX(_customerList[i].transform.localPosition.x - 140, moveTimePerCustomerPos).SetEase(Ease.Linear);
            }
        }

        private void OnMiniGameQuit()
        {
            float fixedDuration = 1f;
            float walkAnimDuration = 1/6f;

            List<WaitForSignal> flags = new List<WaitForSignal>();
            
            for (int i = 0; i < _customerList.Count; i++)
            {
                for (int j = 0; j < (fixedDuration / walkAnimDuration) / 2f; j++)
                {
                    int cachedI = i;

                    WaitForSignal new1 = new();
                    WaitForSignal new2 = new();

                    flags.Add(new1);
                    flags.Add(new2);

                    StartCoroutine(TimerCoroutine(walkAnimDuration * (j * 2),
                        () =>
                        _customerList[cachedI].transform.DOLocalMoveY(25, walkAnimDuration).SetEase(Ease.Linear).OnComplete(new1.Signal)
                        )) ;
                    StartCoroutine(TimerCoroutine(walkAnimDuration * (j * 2 + 1),
                        () =>
                        _customerList[cachedI].transform.DOLocalMoveY(0, walkAnimDuration).SetEase(Ease.Linear).OnComplete(new2.Signal)
                        ));
                }

                WaitForSignal new3 = new();
                flags.Add(new3);

                _customerList[i].transform.DOLocalMoveX(-420, fixedDuration).SetEase(Ease.Linear).OnComplete(new3.Signal);
            }

            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => flags.TrueForAll(y => y.HasSignal()));
                Clear();
            });
        }

        private IEnumerator TimerCoroutine(float duration, System.Action onComplete)
        {
            yield return new WaitForSeconds(duration);
            onComplete?.Invoke();
        }
    }
}