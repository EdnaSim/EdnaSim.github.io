using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public SO_PlayerData pdata;
    //key is itemID
    [HideInInspector] public Dictionary<string, ItemInstance> playerInventory = new Dictionary<string, ItemInstance>();
    [HideInInspector] public bool ready = true;
    [HideInInspector] public bool scReady = true;

    private void Awake() {
        Instance = this;
        UpdatePlayerInventory();
    }

    private void Start() {
        ShopController.Instance.UpdateSCCounter();
    }

    public void UpdatePlayerInventory() {
        ready = false;
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetInvSucc, OnError);
    }

    //gain Space Currency (Minerals)
    public void GainSC(int amt) {
        scReady = false;
        var req = new AddUserVirtualCurrencyRequest {
            Amount = amt,
            VirtualCurrency = "SC"
        };
        PlayFabClientAPI.AddUserVirtualCurrency(req, OnChangeSCSucc, OnError);
    }

    public void LoseSC(int amt) {
        scReady = false;
        var req = new SubtractUserVirtualCurrencyRequest {
            Amount = amt,
            VirtualCurrency = "SC"
        };
        PlayFabClientAPI.SubtractUserVirtualCurrency(req, OnChangeSCSucc, OnError);
    }

    //used when buying, and buying doesnt call LoseSC
    public void QuickChangePDataSC(int amt) {
        pdata.SC += amt;
    }

    void OnGetInvSucc(GetUserInventoryResult r) {
        pdata.SC = r.VirtualCurrency["SC"];

        //update inventory
        playerInventory.Clear();
        foreach (ItemInstance i in r.Inventory) {
            if (playerInventory.ContainsKey(i.ItemId)) continue; //prevent error
            playerInventory.Add(i.ItemId, i);
        }
        ready = true;
    }

    void OnChangeSCSucc(ModifyUserVirtualCurrencyResult r) {
        pdata.SC = r.Balance;
        scReady = true;
    }

    void OnError(PlayFabError e) {
        scReady = true;
        Debug.Log("Error: " + e.GenerateErrorReport());
    }
}
