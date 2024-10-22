using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using static PlayJam.InGame.StackHamburger.StackHamburgerData;

namespace PlayJam.InGame.StackHamburger
{
    public class StackHamburgerIngredient : MonoBehaviour
    {
        [SerializeField]
        private Collider2D _collider;

        public EElement Element;

        public UnityEvent<StackHamburgerIngredient> OnTrigger;
        public UnityEvent<StackHamburgerIngredient> OnFail;

        public bool IsCatched;

        public Tween Tween;

        public void Initialize()
        {
            OnTrigger = new UnityEvent<StackHamburgerIngredient>();
            OnFail = new UnityEvent<StackHamburgerIngredient>();

            if (Tween != null)
            {
                Tween.Kill();
                Tween = null;
            }

            IsCatched = false;
        }

        public async void Move(Vector3 inStartPos, Vector3 inEndPos, float inDuration)
        {
            transform.position = inStartPos;
            Tween = transform.DOMove(inEndPos, inDuration).SetEase(Ease.Linear);
            await Tween.AsyncWaitForCompletion();

            if (Tween != null)
            {
                Tween.Kill();
                Tween = null;
                OnFail.Invoke(this);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsCatched == true && other.tag == "Ingredient")
                OnTrigger.Invoke(other.GetComponent<StackHamburgerIngredient>());

            if (IsCatched == false && Tween != null && Tween.IsComplete() == false && other.tag == "Character")
                OnTrigger.Invoke(this);
        }
    }
}