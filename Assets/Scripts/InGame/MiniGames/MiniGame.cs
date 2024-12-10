using System;
using System.Collections;
using UnityEngine;

namespace PlayJam.InGame
{
    public class MiniGamePlayer : MonoBehaviour
    {
        public bool IsPlaying = false;

        public Transform Hand;
        public Animator HandAnimator;

        public virtual void Initialize(MiniGameData inData)
        {

        }

        public virtual IEnumerator OnStart()
        {
            yield return null;
        }

        public virtual void OnPostStart()
        {
            IsPlaying = true;
        }

        public virtual IEnumerator OnPause()
        {
            IsPlaying = false;
            yield return null;
        }

        public virtual IEnumerator OnResume()
        {
            IsPlaying = true;
            yield return null;
        }

        public virtual IEnumerator OnSuccess(Action inCallback)
        {
            IsPlaying = false;
            yield return null;
            inCallback?.Invoke();
        }

        public virtual IEnumerator OnFail(Action inCallback)
        {
            IsPlaying = false;
            yield return null;
            inCallback?.Invoke();
        }

        public virtual void Clear()
        {
            IsPlaying = false;
        }

        public IEnumerator Co_DrawCircle(Transform inTransform, float inRadius, float inDuration)
        {
            float elapsedTime = 0f; // 경과 시간
            float angle = 0;
            Vector3 initPos = inTransform.localPosition;

            while (elapsedTime < inDuration)
            {
                // 경과 시간을 기준으로 각도를 계산
                float percentage = elapsedTime / inDuration; // 0에서 1 사이로 비율 계산
                angle = percentage * 2 * Mathf.PI; // 원을 한 바퀴 도는 각도 계산

                // 원의 x, y 좌표를 계산
                float x = Mathf.Cos(angle) * inRadius;
                float y = Mathf.Sin(angle) * inRadius;

                // 객체의 위치를 업데이트 (z값은 그대로 유지)
                inTransform.localPosition = new Vector3(x + initPos.x, y + initPos.y, inTransform.localPosition.z);

                // 경과 시간을 증가
                elapsedTime += Time.deltaTime;

                // 한 프레임을 기다리고 다시 실행
                yield return null;
            }

            // 완료 후 원의 위치를 마지막 좌표로 설정 (정확한 위치)
            inTransform.localPosition = new Vector3(Mathf.Cos(2 * Mathf.PI) * inRadius + initPos.x, Mathf.Sin(2 * Mathf.PI) * inRadius + initPos.y, inTransform.localPosition.z);
        }
    }
}
