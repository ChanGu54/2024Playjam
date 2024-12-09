using System;
using System.Collections.Generic;
using System.Linq;
using PlayJam.Character;
using PlayJam.World;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayJam.Popup
{
    public class PopupCostume : BasePopup
    {
        [SerializeField]
        private CostumeCell _cellPrefab;

        [SerializeField]
        private WorldManager _worldManager;

        [SerializeField]
        private MainCharacter _characterMainInUI;

        [SerializeField]
        private RectTransform _cellRoot;

        [SerializeField]
        private Button _btnX;

        [SerializeField]
        private TextMeshProUGUI _txtCash;

        private ECostume[] _allCostume;

        private List<CostumeCell> _costumeCellInstantiated;

        public override void PrevOpen()
        {
            base.PrevOpen();

            RefreshAniCostume();

            _allCostume = Enum.GetValues(typeof(ECostume)).Cast<ECostume>().ToArray();

            if (_costumeCellInstantiated == null || _costumeCellInstantiated.Count <= 0)
                InstantiateCells();
            else
                RefreshCells();

            _cellRoot.offsetMin = new Vector2(0, _cellRoot.offsetMin.y);

            _btnX.onClick.RemoveAllListeners();
            _btnX.onClick.AddListener(PopupManager.Instance.Exit<PopupCostume>);

            _txtCash.transform.parent.gameObject.SetActive(true);

            RefreshCoinCount();
        }

        private void InstantiateCells()
        {
            _costumeCellInstantiated = new List<CostumeCell>();

            for (int i = 0; i < _allCostume.Length; i++)
            {
                GameObject go = Instantiate(_cellPrefab.gameObject, _cellRoot);
                go.SetActive(true);

                _costumeCellInstantiated.Add(go.GetComponent<CostumeCell>());
            }

            RefreshCells();
        }

        public void RefreshCells()
        {
            ECostume[] ownedCostume = UserDataHelper.Instance.GetOwnedCostumes();

            for (int i = 0; i < _costumeCellInstantiated.Count; i++)
            {
                _costumeCellInstantiated[i].Initialize(ownedCostume, _allCostume[i]);
            }
        }

        public void RefreshAniCostume()
        {
            _worldManager.RefreshCostume();
            _characterMainInUI.SetCostume(UserDataHelper.Instance.EquippedCostume);
        }

        public void RefreshCoinCount()
        {
            _txtCash.text = UserDataHelper.Instance.Coin.ToString();
        }

        public override void PostClose()
        {
            base.PrevClose();
            _txtCash.transform.parent.gameObject.SetActive(false);
        }
    }
}
