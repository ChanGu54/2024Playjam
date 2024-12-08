using System.Collections;
using UnityEngine;

namespace PlayJam.Popup
{
    public class BasePopup : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;

        public bool IsOpening { private set; get; }
        public bool IsClosing { private set; get; }

        public void Open()
        {
            if (IsOpening || IsClosing)
                return;

            IsOpening = true;
            StartCoroutine(Co_Open());
        }

        private IEnumerator Co_Open()
        {
            PrevOpen();

            _animator.Play("OPEN");
            float duration = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            yield return new WaitForSeconds(duration);

            PostOpen();
            IsOpening = false;
        }

        public void Close()
        {
            if (IsOpening || IsClosing)
                return;

            IsClosing = true;
            Clear();
            StartCoroutine(Co_Close());
        }

        private IEnumerator Co_Close()
        {
            PrevClose();

            _animator.Play("CLOSE");
            float duration = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            yield return new WaitForSeconds(duration);

            PostClose();
            IsClosing = false;
        }

        public virtual void PrevOpen()
        {

        }

        public virtual void PostOpen()
        {

        }

        public virtual void PrevClose()
        {

        }

        public virtual void PostClose()
        {

        }

        public virtual void Clear()
        {

        }
    }
}
