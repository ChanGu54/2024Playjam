using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PlayJam.InGame.PickingCabbage
{
    public class PickingCabbagePlayer : MiniGamePlayer
    {
        [SerializeField]
        private PickingCabbageData _config;

        [SerializeField]
        private List<PickingCabbageTarget> _targets;

        private Queue<PickingCabbageTarget> _targetQueue = new Queue<PickingCabbageTarget>();

        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            base.Clear();

            _config = null;

            _lastTouchPos = Vector3.zero;
            _targetQueue.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inConfig"></param>
        public override void Initialize(MiniGameData inConfig)
        {
            base.Initialize(inConfig);

            _config = inConfig as PickingCabbageData;

            for (int i = 0; i < _targets.Count; i++)
            {
                _targets[i].transform.localPosition = Vector3.zero;
                _targets[i].SpriteRenderer.color = Color.white;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnStart()
        {
            for (int i = 0; i < _targets.Count; i++)
            {
                _targets[i].gameObject.SetActive(true);
                _targets[i].Collider2D.enabled = false;
                _targetQueue.Enqueue(_targets[i]);
            }

            _targetQueue.Peek().Collider2D.enabled = true;

            yield return new WaitForSeconds(0.5f);

            MiniGameManager.OnMiniGamePostStart.Invoke();
        }

        public override void OnPostStart()
        {
            base.OnPostStart();
        }

        private Vector3 _lastTouchPos;

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
            if (Input.GetMouseButton(0) == false)
            {
                _lastTouchPos = Vector3.zero;
                return;
            }

            Touch[] touches = new Touch[] {
                new Touch()
                {
                    position = new Vector2(Input.mousePosition.x, Input.mousePosition.y),
                    phase = _lastTouchPos == Vector3.zero ? TouchPhase.Began : TouchPhase.Moved,
                }
            };
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
                Collider2D hit = Physics2D.OverlapPoint(pos);
                if (hit != null && _targetQueue.Count > 0 && hit.gameObject == _targetQueue.Peek().gameObject)
                    _lastTouchPos = pos;

                return;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (_lastTouchPos == Vector3.zero && _targetQueue.Count <= 0)
                    return;

                Vector3 moveDist = pos - _lastTouchPos;
                PickingCabbageTarget target = _targetQueue.Peek();

                switch (target.PickingDir)
                {
                    case EDir.Left:
                        if (moveDist.x < -200)
                        {
                            _targetQueue.Dequeue().Collider2D.enabled = false;
                            target.transform.DOMoveX(-200, 1f);
                            target.SpriteRenderer.DOColor(new Color(1, 1, 1, 0), 1f);
                            _lastTouchPos = Vector3.zero;

                            if (_targetQueue.Count <= 0)
                            {
                                StartCoroutine(OnSuccess(() => MiniGameManager.OnMiniGameEnd.Invoke(true)));
                            }
                            else
                            {
                                _targetQueue.Peek().Collider2D.enabled = true;
                            }
                        }
                        break;
                    case EDir.Right:
                        if (moveDist.x < 200)
                        {
                            _targetQueue.Dequeue().Collider2D.enabled = false;
                            target.transform.DOMoveX(200, 1f);
                            target.SpriteRenderer.DOColor(new Color(1, 1, 1, 0), 1f);
                            _lastTouchPos = Vector3.zero;

                            if (_targetQueue.Count <= 0)
                            {
                                StartCoroutine(OnSuccess(() => MiniGameManager.OnMiniGameEnd.Invoke(true)));
                            }
                            else
                            {
                                _targetQueue.Peek().Collider2D.enabled = true;
                            }
                        }
                        break;
                }
            }
        }
    }
}