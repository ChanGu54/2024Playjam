using System.Collections.Generic;
using PlayJam.Character.NPC;
using UnityEngine;

namespace PlayJam.Character
{
    public class CharacterManager : MonoBehaviour
    {
        #region Singleton
        private static CharacterManager _instance;

        public static CharacterManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CharacterManager>();
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        #endregion

        [SerializeField]
        private MainCharacter _mainCharacterPrefab;

        [SerializeField]
        private EHat _curHat;

        [SerializeField]
        private EAddOn _curAddOn;

        [SerializeField]
        private EBody _curBody;

        [SerializeField]
        private List<Customer> _customers;

        public MainCharacter GetMainCharacter(Transform inParent)
        {
            MainCharacter mainCharacter = Instantiate(_mainCharacterPrefab.gameObject, inParent).GetComponent<MainCharacter>();
            mainCharacter.Initialize(EEmotion.IDLE, EArm.IDLE, _curBody, _curHat, _curAddOn, true);
            return mainCharacter;
        }

        public Customer GetRandomCustomers(Transform inParent)
        {
            Customer customerElected = _customers[Random.Range(0, _customers.Count)];
            Customer customerInstance = Instantiate(customerElected.gameObject, inParent).GetComponent<Customer>();
            return customerInstance;
        }
    }
}