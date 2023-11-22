using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance;
    [HideInInspector] public Dictionary<string, CatalogItem> catalog = new Dictionary<string, CatalogItem>();
    List<CatalogItemUI> ShownItems = new List<CatalogItemUI>();
    
    [Header("Externals")]
    [SerializeField] SO_ShopModels models;
    [SerializeField] GameObject ItemPrefab;
    [SerializeField] Camera RTCam;//render texture cam
    RenderTexture baseRT;

    [Header("Buttons")]
    [SerializeField] Button b_OpenShop;
    [SerializeField] Button b_CloseShop;
    [SerializeField] Button b_OpenUpgrades; //just to set enabled

    [Header("Areas")]
    [SerializeField] GameObject MainShop;
    [SerializeField] GameObject ContentArea;

    [Header("Text")]
    [SerializeField] TMP_Text msgbox;
    [SerializeField] TMP_Text CurrencyCounter;

    private void Awake() {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitTillInvReady());
        MainShop.SetActive(false);
        AccessCatalog();
        baseRT = new RenderTexture(256, 256,16);
    }

    IEnumerator WaitTillInvReady() {
        b_OpenUpgrades.enabled = false;
        b_OpenShop.enabled = false;
        yield return new WaitUntil(() => InventoryManager.Instance.scReady && InventoryManager.Instance.ready);
        b_OpenShop.enabled = true;
        b_OpenUpgrades.enabled = true;
    }

    //pressed button to open shop
    public void OnOpenShop() {
        InventoryManager.Instance.UpdatePlayerInventory();
        UpdateSCCounter();
        b_OpenShop.gameObject.SetActive(false);
        MainShop.SetActive(true);
    }
    //pressed button in the shop to close
    public void OnCloseShop() {
        b_OpenShop.gameObject.SetActive(true);
        MainShop.SetActive(false);
    }

    void AccessCatalog() {
        InventoryManager.Instance.UpdatePlayerInventory();
        var req = new GetCatalogItemsRequest() {
            CatalogVersion = "ShopCatalog"
        };
        PlayFabClientAPI.GetCatalogItems(req, GetCataSucc, OnError);
    }

    //https://stackoverflow.com/questions/63928572/how-do-i-stop-a-camera-from-rendering-to-a-rendertexture
    void GetCataSucc(GetCatalogItemsResult r) {
        if (ContentArea == null) return;
        //instantiate the stuff available for sale (prefab with CatalogItemUI)
        foreach (CatalogItem item in r.Catalog) {
            if (InventoryManager.Instance.playerInventory.ContainsKey(item.ItemId) && !item.IsStackable)
                continue; //already owned
            catalog.Add(item.ItemId, item);

            //get the GO of the ship/item
            GameObject _model = models.ShopModelList.Find((GameObject g) => g.GetComponent<CatalogItemInfo>().id == item.ItemId);
            GameObject model = Instantiate(_model);
            //prepare the model for getting its picture (render texture)
            model.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            //UICamera is the only layer that the shop cam can see. (main cam can't)
            model.layer = LayerMask.NameToLayer("UICamera");
            SetChildrenLayer(model, "UICamera");
            RenderTexture rt = new RenderTexture(baseRT);
            RenderTexture.active = rt; //lets rt start rendering
            //set cam into pos to get the pic of the item
            RTCam.targetTexture = rt;
            RTCam.transform.position = new Vector3(model.transform.position.x, model.transform.position.y, model.transform.position.z -3f);//lower z to show top
            RTCam.Render(); //force immediately rendering
            //copy the rendertexture from the camera to this tex2d, to set as the image of the item box
            Texture2D tex2d = new Texture2D(256, 256, TextureFormat.ARGB32, false);
            tex2d.ReadPixels(new Rect(0, 0, 256, 256), 0, 0, false);
            tex2d.Apply();
            model.SetActive(false); //dont obscure the next item

            //itemPrefab - a prefab containing the text box and Buy btn for an entry into the shop
            CatalogItemUI itemUI = Instantiate(ItemPrefab, ContentArea.transform).GetComponent<CatalogItemUI>();
            //set description, price, and name of the item
            itemUI.Desc.text = item.DisplayName+ "\nPrice: " + item.VirtualCurrencyPrices["SC"] + "\n" + item.Description;
            itemUI.msgbox = msgbox;
            if (model != null)
                itemUI.model = model;
            itemUI.RTImage.texture = tex2d; //set the "picture" of the item
            itemUI.id = item.ItemId;
            ShownItems.Add(itemUI);
            //reset so nothing else is rendered into the rt
            RenderTexture.active = null;
            RTCam.targetTexture = null;
        }
    }
    public void UpdateSCCounter() {
        CurrencyCounter.text = "Minerals: " + InventoryManager.Instance.pdata.SC.ToString();
    }

    public void SetShownItemInactive(string id) {
        ShownItems.Find((CatalogItemUI item) => item.id == id).gameObject.SetActive(false);
    }

    public void RefreshShop() {
        //remove the items in shop that have been bought
        foreach (CatalogItemUI i in ShownItems) {
            if (InventoryManager.Instance.playerInventory.ContainsKey(i.id)) {
                Destroy(i.gameObject);
                ShownItems.Remove(i);
                RefreshShop();
                break;
            }
        }
        UpdateSCCounter();
    }

    void OnError(PlayFabError e) {
        Debug.Log("Error: " + e.GenerateErrorReport());
    }

    void SetChildrenLayer(GameObject parent, string layer) {
        for (int i=0; i < parent.transform.childCount; i++) {
            parent.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer(layer);
            SetChildrenLayer(parent.transform.GetChild(i).gameObject, layer);
        }
    }
}
