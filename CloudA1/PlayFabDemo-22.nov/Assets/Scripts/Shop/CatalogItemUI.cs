using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class CatalogItemUI : MonoBehaviour
{
    public Transform ModelParent;
    [HideInInspector]public TMP_Text Desc;
    [SerializeField] Button buyBtn;
    [HideInInspector]public TMP_Text msgbox;
    public GameObject model; //TODO: for 3D display in shop
    public RawImage RTImage;
    public string id;
    public CatalogItemInfo.TYPE type;

    public void OnButtonBuy() {
        var req = new PurchaseItemRequest {
            ItemId = id,
            Price = (int)ShopController.Instance.catalog[id].VirtualCurrencyPrices["SC"],
            VirtualCurrency = "SC"
        };
        PlayFabClientAPI.PurchaseItem(req, OnPurchaseSucc, OnError);
    }

    void OnPurchaseSucc(PurchaseItemResult r) {
        msgbox.text = "Purchased";
        InventoryManager.Instance.QuickChangePDataSC(-(int)ShopController.Instance.catalog[id].VirtualCurrencyPrices["SC"]);
        ShopController.Instance.SetShownItemInactive(id); //in case the thing deletes too slow
        CharacterSelection.Instance.RefreshOwnedCharacters(); //calls updateinventory()
        ShopController.Instance.RefreshShop();
    }

    void OnError(PlayFabError e) {
        msgbox.text = "Error purchasing: " + e.GenerateErrorReport();
    }
}
