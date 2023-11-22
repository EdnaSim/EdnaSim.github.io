using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using PlayFab;

public class CharacterSelection : MonoBehaviour {
    public static CharacterSelection Instance;
    //GameObject[] characters;
    List<GameObject> characters = new List<GameObject>();
    string charIndexName;//bcuz index is inaccurate sometimes
    [SerializeField] SO_ShopModels itemList;
    int index;
    private void Awake() {
        Instance = this;
    }
    void Start() {
        index = PlayerPrefs.GetInt("SelectedCharacter");
        charIndexName = PlayerPrefs.GetString("SelectedName");
        if (index >= characters.Count || index <0) {
            index = 0;
        }
        RefreshOwnedCharacters();
        //characters = new GameObject[transform.childCount];
        //Debug.Log(transform.childCount);
        //for (int i = 0; i < transform.childCount; i++) {
        //    characters.Add(transform.GetChild(i).gameObject);
        //    characters[i].SetActive(false);
        //}
    }

    public void RefreshOwnedCharacters() {
        StartCoroutine(RefreshCharacterCoroutine());
    }

    IEnumerator RefreshCharacterCoroutine() {
        InventoryManager.Instance.UpdatePlayerInventory();
        //wait for the request to finish
        yield return new WaitUntil(()=>InventoryManager.Instance.ready);
        
        foreach (KeyValuePair<string, ItemInstance> item in InventoryManager.Instance.playerInventory) {
            //find characters owned and not yet instantiated
            GameObject go = itemList.ShopModelList.Find((GameObject g) => g.GetComponent<CatalogItemInfo>().id == item.Key);
            if (characters.Contains(go) || FindInChildren(go.GetComponent<CatalogItemInfo>())) continue; //alr instantiated
            if (go.GetComponent<CatalogItemInfo>().type == CatalogItemInfo.TYPE.CHARACTER) {
                AddNewCharacter(go);
            }
        }
        if (characters.Count > 0 /*&& characters[index]*/) {
            GameObject charac = characters.Find((x) => x.name == charIndexName);
            index = characters.IndexOf(charac);
            if (index < 0) index = 0;
            //characters[index].SetActive(true);
            if (charac != null)
                charac.SetActive(true);
            if (SceneManager.GetActiveScene().name == "Main") {
                //if in game, need to activate PlayerControl to control the ship
                //characters[index].GetComponent<PlayerControl>().enabled = true;
                if (charac != null)
                    charac.GetComponent<PlayerControl>().enabled = true;
            }
        }
    }

    bool FindInChildren(CatalogItemInfo info) {
        //find certain item id in CharactersList (this) children
        CatalogItemInfo[] childList = transform.GetComponentsInChildren<CatalogItemInfo>(true);
        foreach (CatalogItemInfo child in childList) {
            if (child.id == info.id) return true;
        }
        return false;
    }

    public void toggleLeft() {
        if (characters.Count <= 0 || index >= characters.Count) return;
        characters[index].SetActive(false);
        if (index == 0) {
            index = transform.childCount - 1;
        } else {
            index--;
        }
        if (index < 0) index = 0;
        characters[index].SetActive(true);
    }

    public void toggleRight() {
        if (characters.Count <= 0 || index >= characters.Count) return;
        characters[index].SetActive(false);
        if(index == transform.childCount-1){
            index = 0;
        }
        else{
            index++;
        }
        characters[index].SetActive(true);
    }

    public void selectCharacterAndStart(){
        if (characters.Count <= 0) return;
        PlayerPrefs.SetInt("SelectedCharacter", index);
        PlayerPrefs.SetString("SelectedName", characters[index].name);
        SceneManager.LoadScene("Main");
    }
    public int getIndex(){
        return index;
    }

    public void AddNewCharacter(GameObject go) {
        GameObject g = Instantiate(go, transform);
        characters.Add(g);
        g.SetActive(false);
    }
}
