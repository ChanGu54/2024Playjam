using PlayJam.Sound;
using System;
using System.Collections;
using UnityEngine;

namespace PlayJam.InGame.CuttingFruit
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class CuttingFruitTarget : MonoBehaviour
    {
        /// <summary>
        /// 0번 : Left, 1번 : Right
        /// </summary>
        [SerializeField]
        private Rigidbody2D[] _fragments;

        [SerializeField]
        private Rigidbody2D _rb2D;

        [SerializeField]
        private SpriteRenderer _rend;

        [SerializeField]
        private float _initGravityScale = 100;

        public float GravityScale;

        private float _gravity = 9.81f;
        private float _realGravity;

        public bool IsFlying { private set; get; } = false;

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            float nativeTotalTime = MiniGameSharedData.Instance.Config.PlayTime;
            float totalTime = MiniGameSharedData.Instance.CurStageTime;

            GravityScale = nativeTotalTime / totalTime * _initGravityScale;
            _realGravity = _gravity * GravityScale;

            Array.ForEach(_fragments, (x) =>
            {
                x.velocity = Vector3.zero;
                x.mass = 1;
                x.gravityScale = GravityScale;
                x.transform.localPosition = Vector3.zero;
                x.bodyType = RigidbodyType2D.Static;
                x.gameObject.SetActive(false);
            });

            _rb2D.velocity = Vector3.zero;
            _rb2D.angularVelocity = 0;
            _rb2D.mass = 1;
            _rb2D.gravityScale = GravityScale;
            _rb2D.rotation = 0;
            _rb2D.transform.localPosition = Vector3.zero;
            _rb2D.transform.rotation = Quaternion.identity;
            _rb2D.bodyType = RigidbodyType2D.Static;

            _rend.color = new Color(0, 0, 0, 0);
        }

        public float Fly(Vector3 inStartPos, Vector3 inDestPos)
        {
            _rend.color = Color.white;

            Vector3 diff = inDestPos - inStartPos;
            float horizontalDist = new Vector3(diff.x, 0, diff.z).magnitude;

            // 포물선 운동 계산 수정
            float timeToTarget;
            Vector3 startVelocity;

            if (diff.y <= 0)
            {
                // 목표가 시작점보다 낮거나 같은 경우
                float timeToFall = Mathf.Sqrt(-2 * diff.y / _realGravity);
                timeToTarget = timeToFall;

                startVelocity = new Vector3(
                    diff.x / timeToTarget,
                    0,
                    diff.z / timeToTarget);
            }
            else
            {
                // 목표가 시작점보다 높은 경우
                // v^2 = u^2 + 2as 공식 활용
                float velocityY = Mathf.Sqrt(2 * _realGravity * diff.y);
                timeToTarget = (velocityY + Mathf.Sqrt(velocityY * velocityY + 2 * _realGravity * diff.y)) / _realGravity;

                startVelocity = new Vector3(
                    diff.x / timeToTarget,
                    velocityY,
                    diff.z / timeToTarget);
            }

            // 물체 위치 및 속도 설정
            transform.position = inStartPos;
            _rb2D.isKinematic = false;
            _rb2D.velocity = startVelocity;
            _rb2D.angularVelocity = UnityEngine.Random.Range(-60, 60);

            Debug.Log($"Distance: {horizontalDist:F2}m, Height Diff: {diff.y:F2}m");
            Debug.Log($"Initial Velocity: {startVelocity}, Expected Time: {timeToTarget:F2}s");

            IsFlying = true;
            return timeToTarget;
        }

        public IEnumerator OnCut()
        {
            IsFlying = false;

            _rend.color = new Color(1, 1, 1, 0);
            _rb2D.bodyType = RigidbodyType2D.Static;

            for (int i = 0; i < _fragments.Length; i++)
            {
                _fragments[i].gameObject.SetActive(true);
                _fragments[i].bodyType = RigidbodyType2D.Dynamic;
                _fragments[i].gravityScale = GravityScale * 5;
            }

            SoundManager.Instance.Play(ESoundType.SFX, "CuttingFruit");

            yield return null;

            for (int i = 0; i < _fragments.Length; i++)
            {
                _fragments[i].AddForce(new Vector2(i == 0 ? -5000 : 5000, _realGravity * 10));
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (IsFlying == true && collision.CompareTag("Touch") == true)
            {
                StartCoroutine(OnCut());
            }
        }

        public void Clear()
        {
            IsFlying = false;

            Array.ForEach(_fragments, (x) =>
            {
                x.velocity = Vector3.zero;
                x.mass = 1;
                x.gravityScale = GravityScale;
                x.transform.localPosition = Vector3.zero;
                x.bodyType = RigidbodyType2D.Static;
                x.gameObject.SetActive(false);
            });

            _rb2D.velocity = Vector3.zero;
            _rb2D.angularVelocity = 0;
            _rb2D.mass = 1;
            _rb2D.rotation = 0;
            _rb2D.transform.rotation = Quaternion.identity;
            _rb2D.gravityScale = GravityScale;
            _rb2D.transform.localPosition = Vector3.zero;
            _rb2D.bodyType = RigidbodyType2D.Static;

            _rend.color = new Color(0, 0, 0, 0);
        }
    }
}
