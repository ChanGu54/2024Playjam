using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace PlayJam.Character.NPC
{
    public enum ECustomerType
    {
        Pinky,
        Lucy,
        Blue,
        Micky,
        Mongyi,
    }

    public class Customer : MonoBehaviour
    {
        [SerializeField]
        private SerializedDictionary<EFood, GameObject> _foodToGoDic;

        public ECustomerType CustomerType;

        public void ShowFoodResource(EFood inFood)
        {
            foreach (var pair in _foodToGoDic)
            {
                pair.Value.gameObject.SetActive(pair.Key == inFood);
            }
        }
    }
}