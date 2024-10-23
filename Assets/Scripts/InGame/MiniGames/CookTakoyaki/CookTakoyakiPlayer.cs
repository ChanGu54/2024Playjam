using DG.Tweening;
using PlayJam.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayJam.InGame.CookTakoyaki
{
    /// <summary>
    /// 
    /// </summary>
    public class CookTakoyakiPlayer : MiniGamePlayer
    {
        [SerializeField]
        private List<Collider2D> _touchPoints;

        [SerializeField]
        private List<GameObject> _takoyakiObjs;

        [SerializeField]
        private GameObject _takoyakiStick;

        [SerializeField]
        private CookTakoyakiData _config;

        private List<GameObject> _cachedClonedObjs = new List<GameObject>();

        private int _activeTakoyakiCount;

        public override void Clear()
        {
            base.Clear();

            _config = null;
            _activeTakoyakiCount = 0;

            for (int i = 0; i < _cachedClonedObjs.Count; i++)
            {
                Destroy(_cachedClonedObjs[i]);
            }

            StopAllCoroutines();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inConfig"></param>
        public override void Initialize(MiniGameData inConfig)
        {
            base.Initialize(inConfig);

            _config = inConfig as CookTakoyakiData;

            if (_config == null)
                return;

            _activeTakoyakiCount = UnityEngine.Random.Range(_config.TakoyakiMinCount, _config.TakoyakiMaxCount + 1);
            int deactiveTakoyakiCount = _config.TakoyakiMaxCount - _activeTakoyakiCount;

            for (int i = 0; i < _takoyakiObjs.Count; i++)
            {
                _takoyakiObjs[i].SetActive(false);
            }

            List<int> indicesToActive = Enumerable.Range(0, _takoyakiObjs.Count).ToList();
            while (deactiveTakoyakiCount > 0)
            {
                int removeIndex = UnityEngine.Random.Range(0, indicesToActive.Count);
                indicesToActive.RemoveAt(removeIndex);
                deactiveTakoyakiCount--;
            }

            for (int i = 0; i < indicesToActive.Count; i++)
            {
                _takoyakiObjs[indicesToActive[i]].SetActive(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnStart()
        {
            yield return null;
            MiniGameManager.OnMiniGamePostStart.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (IsPlaying == false)
            {
                return;
            }

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) == false)
            {
                return;
            }

            Touch[] touches = new Touch[] {
                new Touch()
                {
                    position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
                }
            };
#else
            if (Input.touchCount <= 0)
            {
                return;
            }

            Touch[] touches = Input.touches;
#endif

            for (int i = 0; i < touches.Length; i++)
            {
                Touch touch = touches[i];

                if (touch.phase != TouchPhase.Began)
                {
                    continue;
                }

                Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
                // 이미지의 충돌 범위 확인
                Collider2D hit = Physics2D.OverlapPoint(pos);

                for (int j = 0; j < _touchPoints.Count; j++)
                {
                    if (_takoyakiObjs[j].activeSelf == true && hit != null && hit == _touchPoints[j])
                    {
                        StartCoroutine(Co_TakeOutTakoyaki(j));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator Co_TakeOutTakoyaki(int inIndex)
        {
            _activeTakoyakiCount--;
            bool isClearStage = false;

            if (_activeTakoyakiCount <= 0)
                isClearStage = true;

            if (isClearStage == true)
            {
                // 일단 미니게임 시간 일시정지
                MiniGameManager.OnMiniGamePause.Invoke();
            }

            GameObject clonedTakoyaki = Instantiate(_takoyakiObjs[inIndex], _takoyakiObjs[inIndex].transform.parent);
            clonedTakoyaki.transform.localPosition = Vector3.zero;
            _takoyakiObjs[inIndex].SetActive(false);

            _cachedClonedObjs.Add(clonedTakoyaki);

            WaitForSignal flag = new WaitForSignal();
            Vector3 startPos = new Vector3(clonedTakoyaki.transform.position.x / 2f, -700, -3);

            GameObject clonedTakoyakiStick = Instantiate(_takoyakiStick, _takoyakiStick.transform.parent);
            clonedTakoyakiStick.transform.localPosition = startPos;
            _cachedClonedObjs.Add(clonedTakoyakiStick);

            clonedTakoyakiStick.transform.DOMove(clonedTakoyaki.transform.position, 0.3f).SetEase(Ease.InOutQuad).onComplete = flag.Signal;
            yield return flag.Wait();

            clonedTakoyaki.transform.SetParent(clonedTakoyakiStick.transform);
            clonedTakoyaki.transform.localPosition = new(0, 0, -1);

            clonedTakoyakiStick.transform.DOMove(startPos, 0.3f).SetEase(Ease.InQuad).onComplete = flag.Signal;
            yield return flag.Wait();

            if (isClearStage == true)
            {
                // 연출 보여줄거면 보여주고 게임 종료
                StartCoroutine(OnSuccess(() => MiniGameManager.OnMiniGameEnd.Invoke(true)));
            }
        }
    }
}
