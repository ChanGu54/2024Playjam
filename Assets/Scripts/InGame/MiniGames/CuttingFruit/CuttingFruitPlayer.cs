using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayJam.InGame.CuttingFruit
{
    public class CuttingFruitPlayer : MiniGamePlayer
    {
        [SerializeField]
        private CuttingFruitData _config;

        [SerializeField]
        private TrailRenderer _blade; 

        [SerializeField]
        private Collider2D _bladeCollider;

        [SerializeField]
        private SerializedDictionary<EFruit, CuttingFruitTarget> _fruits;

        private Dictionary<EFruit, float> _flyStartTimeDic = new Dictionary<EFruit, float>();
        private Dictionary<EFruit, float> _flyEndTimeDic = new Dictionary<EFruit, float>();

        private float _maxDuration;

        private Vector3 _lastTouchPos;

        private float _maxX;
        private float _maxY;

        private float _startY;

        public override void Clear()
        {
            base.Clear();

            _config = null;

            _blade.gameObject.SetActive(false);

            _flyStartTimeDic.Clear();
            _flyEndTimeDic.Clear();

            _maxDuration = 0;

            _maxX = 0;
            _maxY = 0;
            _startY = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inConfig"></param>
        public override void Initialize(MiniGameData inConfig)
        {
            base.Initialize(inConfig);

            _config = inConfig as CuttingFruitData;

            if (_config == null)
                return;

            foreach (var val in _fruits.Values)
            {
                val.Initialize();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnStart()
        {
            float nativeTotalTime = MiniGameSharedData.Instance.Config.PlayTime;
            float totalTime = MiniGameSharedData.Instance.CurStageTime;
            float gravityCalculated = _fruits.OrderByDescending(x => x.Value.GravityScale).First().Value.GravityScale;

            _maxX = MiniGameSharedData.Instance.RightBottomPos.x - 150;
            _maxY = MiniGameSharedData.Instance.LeftTopPos.y - 100;

            _startY = MiniGameSharedData.Instance.RightBottomPos.y - 150;

            float diffY = _maxY - _startY;
            float maxVelocityY = Mathf.Sqrt(2 * gravityCalculated * 9.81f * diffY);
            _maxDuration = 2 * diffY / maxVelocityY;

            yield return new WaitForSeconds(0.5f);
            MiniGameManager.OnMiniGamePostStart.Invoke();
        }

        public override void OnPostStart()
        {
            base.OnPostStart();

            _blade.gameObject.SetActive(true);

            float totalTime = MiniGameSharedData.Instance.CurStageTime;
            float lastFlyTime = totalTime - _maxDuration - 0.1f;
            float startTimeTerm = lastFlyTime / (_fruits.Count - 1);
            int index = 0;

            System.Random rand = new System.Random();
            _fruits = new SerializedDictionary<EFruit, CuttingFruitTarget>(_fruits.OrderBy(x => rand.Next()).ToDictionary(item => item.Key, item => item.Value));

            foreach (var fruitData in _fruits)
            {
                float startTime = startTimeTerm * index++;
                _flyStartTimeDic.Add(fruitData.Key, startTime);
                Debug.Log($"{fruitData.Key} : {startTime}");
            }
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

            List<EFruit> endedOnThisFrame = new List<EFruit>();
            List<EFruit> startedOnThisFrame = new List<EFruit>();

            EFruit fruitToDelete = EFruit.NULL;

            if (_flyStartTimeDic.Count <= 0)
            {
                bool isCleared = true;

                foreach (var pair in _flyEndTimeDic)
                {
                    CuttingFruitTarget flyingFruitTarget = _fruits[pair.Key];
                    if (flyingFruitTarget.IsFlying == true)
                    {
                        isCleared = false;
                        break;
                    }
                }

                if (_flyEndTimeDic.Count > 0 && isCleared == true)
                {
                    _flyEndTimeDic.Clear();
                    StartCoroutine(OnSuccess(() => MiniGameManager.OnMiniGameEnd.Invoke(true)));
                }
            }

            foreach (var pair in _flyEndTimeDic)
            {
                CuttingFruitTarget flyingFruitTarget = _fruits[pair.Key];

                if (pair.Value <= MiniGameSharedData.Instance.SpendTime)
                {
                    fruitToDelete = pair.Key;
                    if (flyingFruitTarget.IsFlying == true)
                    {
                        StartCoroutine(OnFail(() => MiniGameManager.OnMiniGameEnd.Invoke(false)));
                    }
                    break;
                }
            }

            if (fruitToDelete != EFruit.NULL)
            {
                _flyEndTimeDic.Remove(fruitToDelete);
            }

            fruitToDelete = EFruit.NULL;

            foreach (var pair in _flyStartTimeDic)
            {
                if (pair.Value <= MiniGameSharedData.Instance.SpendTime)
                {
                    fruitToDelete = pair.Key;
                    Vector2 startPos = new Vector2(UnityEngine.Random.Range(-_maxX, _maxX), MiniGameSharedData.Instance.RightBottomPos.y - 100);
                    Vector2 endPos = new Vector2(UnityEngine.Random.Range(-_maxX / 2f, _maxX / 2f), UnityEngine.Random.Range(_maxY - 100, _maxY));
                    float duration = _fruits[pair.Key].Fly(startPos, endPos);
                    float endTime = pair.Value + duration;
                    _flyEndTimeDic.Add(pair.Key, endTime);

                    Debug.Log($"Start {pair.Key} : {endTime}");
                    break;
                }
            }

            if (fruitToDelete != EFruit.NULL)
            {
                _flyStartTimeDic.Remove(fruitToDelete);
            }

#if UNITY_EDITOR
            Touch[] touches;

            if (Input.GetMouseButton(0) == false)
            {
                if (_lastTouchPos != Vector3.zero)
                {
                    touches = new Touch[] {
                    new Touch()
                        {
                            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y),
                            phase = TouchPhase.Ended,
                        }
                    };
                }
                else
                {
                    return;
                }

                _lastTouchPos = Vector3.zero;
            }
            else
            {
                touches = new Touch[] {
                new Touch()
                {
                    position = new Vector2(Input.mousePosition.x, Input.mousePosition.y),
                    phase = _lastTouchPos == Vector3.zero ? TouchPhase.Began : TouchPhase.Moved,
                }
            };
            }
#else
            if (Input.touchCount <= 0)
            {
                _lastTouchPos = Vector3.zero;
                return;
            }

            Touch[] touches = Input.touches;
#endif
            Touch touch = touches[0];
            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                _lastTouchPos = pos;
                _bladeCollider.enabled = true;
                _blade.transform.position = _lastTouchPos;

                return;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                _lastTouchPos = pos;    
                _blade.transform.position = _lastTouchPos;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _bladeCollider.enabled = false;
            }
        }
    }
}
