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
                    _instance = GameObject.Find("CharacterManager")?.GetComponent<CharacterManager>();
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        public void Initialize()
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

        public MainCharacter GetMainCharacter(Transform inParent)
        {
            MainCharacter mainCharacter = Instantiate(_mainCharacterPrefab.gameObject, inParent).GetComponent<MainCharacter>();
            mainCharacter.Initialize(EEmotion.IDLE, UserDataHelper.Instance.EquippedCostume);
            return mainCharacter;
        }
    }
}