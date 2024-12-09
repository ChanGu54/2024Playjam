using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using PlayJam.Character;
using PlayJam.World;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayJam.Popup
{
    public enum CostumeCellState
    {
        Equip,
        UnEquip,
        Buy,
    }

    public class CostumeCell : MonoBehaviour
    {
        [SerializeField]
        private MainCharacter _ani;

        [SerializeField]
        private Button _btnBuy;

        [SerializeField]
        private Button _btnEquip;

        [SerializeField]
        private TextMeshProUGUI _txtCost;

        [SerializeField]
        private Image _imgBuy;

        [SerializeField]
        private Image _imgBuyFail;

        [SerializeField]
        private TextMeshProUGUI _txtEquip;

        [SerializeField]
        private WorldManager _worldManager;

        [SerializeField]
        private PopupCostume _popupParent;

        private Tweener _tweenShake;

        private CostumeCellState _state;
        private ECostume _costume;

        public void Initialize(ECostume[] inOwnedCostume, ECostume inCostume)
        {
            _costume = inCostume;

            if (inOwnedCostume != null && inOwnedCostume.Contains(_costume))
            {
                if (UserDataHelper.Instance.EquippedCostume == _costume)
                {
                    _state = CostumeCellState.Equip;
                }
                else
                {
                    _state = CostumeCellState.UnEquip;
                }
            }
            else
            {
                _state = CostumeCellState.Buy;
            }

            _ani.SetCostume(_costume);

            SetUI();
        }

        private void SetUI()
        {
            _btnBuy.onClick.RemoveAllListeners();
            _btnEquip.onClick.RemoveAllListeners();

            _btnBuy.onClick.AddListener(OnClickBuy);
            _btnEquip.onClick.AddListener(OnClickEquip);

            _txtCost.text = UserDataHelper.Instance.CostumeCost.ToString();

            switch (_state)
            {
                case CostumeCellState.Buy:
                    _btnBuy.gameObject.SetActive(true);
                    _btnEquip.gameObject.SetActive(false);
                    _imgBuyFail.gameObject.SetActive(false);
                    _imgBuy.gameObject.SetActive(true);
                    break;
                case CostumeCellState.Equip:
                    _btnBuy.gameObject.SetActive(false);
                    _btnEquip.gameObject.SetActive(true);
                    _txtEquip.text = "EQUIP";
                    break;
                case CostumeCellState.UnEquip:
                    _btnBuy.gameObject.SetActive(false);
                    _btnEquip.gameObject.SetActive(true);
                    _txtEquip.text = "UNEQUIP";
                    break;
            }
        }

        private void OnClickBuy()
        {
            if (UserDataHelper.Instance.Coin >= UserDataHelper.Instance.CostumeCost)
            {
                UserDataHelper.Instance.Coin -= UserDataHelper.Instance.CostumeCost;
                _worldManager.RefreshCoinCount();
                _popupParent.RefreshCoinCount();
                UserDataHelper.Instance.AddOwnedCostumes(_costume);
                _popupParent.RefreshCells();
            }
            else
            {
                if (_tweenShake != null)
                {
                    _tweenShake.Kill();
                    _tweenShake = null;
                }

                _imgBuyFail.gameObject.SetActive(true);
                _imgBuy.gameObject.SetActive(false);

                _tweenShake = _imgBuyFail.transform.DOShakePosition(1, 3).OnComplete(() =>
                {
                    _imgBuyFail.gameObject.SetActive(false);
                    _imgBuy.gameObject.SetActive(true);
                });
            }
        }

        private void OnClickEquip()
        {
            if (_state == CostumeCellState.Equip)
            {
                _state = CostumeCellState.UnEquip;
                UserDataHelper.Instance.EquippedCostume = ECostume.IDLE;
            }
            else
            {
                _state = CostumeCellState.Equip;
                UserDataHelper.Instance.EquippedCostume = _costume;
            }

            _popupParent.RefreshAniCostume();
            _popupParent.RefreshCells();

            SetUI();
        }
    }
}
