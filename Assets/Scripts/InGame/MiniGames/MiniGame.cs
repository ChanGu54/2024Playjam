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
            float elapsedTime = 0f; // ��� �ð�
            float angle = 0;
            Vector3 initPos = inTransform.localPosition;

            while (elapsedTime < inDuration)
            {
                // ��� �ð��� �������� ������ ���
                float percentage = elapsedTime / inDuration; // 0���� 1 ���̷� ���� ���
                angle = percentage * 2 * Mathf.PI; // ���� �� ���� ���� ���� ���

                // ���� x, y ��ǥ�� ���
                float x = Mathf.Cos(angle) * inRadius;
                float y = Mathf.Sin(angle) * inRadius;

                // ��ü�� ��ġ�� ������Ʈ (z���� �״�� ����)
                inTransform.localPosition = new Vector3(x + initPos.x, y + initPos.y, inTransform.localPosition.z);

                // ��� �ð��� ����
                elapsedTime += Time.deltaTime;

                // �� �������� ��ٸ��� �ٽ� ����
                yield return null;
            }

            // �Ϸ� �� ���� ��ġ�� ������ ��ǥ�� ���� (��Ȯ�� ��ġ)
            inTransform.localPosition = new Vector3(Mathf.Cos(2 * Mathf.PI) * inRadius + initPos.x, Mathf.Sin(2 * Mathf.PI) * inRadius + initPos.y, inTransform.localPosition.z);
        }
    }
}
