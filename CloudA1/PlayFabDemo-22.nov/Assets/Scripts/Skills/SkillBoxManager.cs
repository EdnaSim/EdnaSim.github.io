using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class JSListWrapper<T> {
    public List<T> list;
    public JSListWrapper(List<T> list) => this.list = list;
}
public class SkillBoxManager : MonoBehaviour
{
    public static SkillBoxManager Instance;
    [SerializeField] SO_PlayerData pdata;
    [SerializeField] SkillBox[] SkillBoxes;
    [SerializeField] TMP_Text msgbox, SCDisplay;
    [SerializeField] Button b_save, b_back, b_reset;
    int EndCost = 0;
    [HideInInspector]public bool ready = true;

    private void Start() {
        Instance = this;
        LoadJSON();
        SCDisplay.text = InventoryManager.Instance.scReady? pdata.SC.ToString() : "Loading...";
        b_save.gameObject.SetActive(false);
    }

    //invoked by Save btn click
    public void Save() {
        StartCoroutine(SavingRoutine());
    }
    IEnumerator SavingRoutine() {
        //Pay the end cost
        if (EndCost > 0)
            InventoryManager.Instance.LoseSC(EndCost);
        else if (EndCost < 0)
            InventoryManager.Instance.GainSC(-EndCost);
        yield return new WaitUntil(() => InventoryManager.Instance.scReady);
        //save
        SendJSON();
    }

    public void SendJSON() {
        StartCoroutine(GracePeriod());
        List<Skill> skillList = new List<Skill>();
        foreach (SkillBox box in SkillBoxes) {
            skillList.Add(box.ReturnSkill());
        }
        string stringListAsJSON = JsonUtility.ToJson(new JSListWrapper<Skill>(skillList));
        Debug.Log("JSON data prepared: " + stringListAsJSON);
        var req = new UpdateUserDataRequest {
            Data = new Dictionary<string, string> {
                {"Skills", stringListAsJSON}
            }
        };
        PlayFabClientAPI.UpdateUserData(req, OnJSONDataSent, OnError);
    }

    public void LoadJSON() {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnJSONDataReceived, OnError);
    }

    void OnJSONDataSent(UpdateUserDataResult r) {
        SCDisplay.text = pdata.SC.ToString();
        LoadJSON();
    }

    void OnJSONDataReceived(GetUserDataResult r) {
        //Debug.Log("Received JSON data");
        if (r.Data !=null && r.Data.ContainsKey("Skills")){
            Debug.Log(r.Data["Skills"].Value);
            JSListWrapper<Skill> jlw = JsonUtility.FromJson<JSListWrapper<Skill>>(r.Data["Skills"].Value);
            for (int i = 0; i < SkillBoxes.Length; i++) {
                if (i >= jlw.list.Count) break;
                SkillBoxes[i].SetUI(jlw.list[i]);
            }
        }
        ready = true;
    }

    IEnumerator GracePeriod() {
        //wait for json to send (which sets ready to true)
        ready = false;
        b_back.enabled = false;
        b_reset.enabled = false;
        msgbox.text = "Saving... Please wait";
        yield return new WaitUntil(() => ready);
        //once done, wait a little for playfab to update
        msgbox.text = "Hoping PlayFab updates... Please wait";
        yield return new WaitForSeconds(3f);
        msgbox.text = "Done!";
        b_back.enabled = true; 
        b_reset.enabled = true;
    }

    void OnError(PlayFabError e) {
        ready = true;
        Debug.Log("Error" + e.GenerateErrorReport());
    }

    public void OnResetUpgrades() {
        EndCost = 0;
        foreach (SkillBox sb in SkillBoxes) {
            sb.ResetLevel();
        }
        //update UI
        ChangeEndCost(0);
    }
    
    public void ChangeEndCost(int amt) {
        //endcost to be paid only when b_save is clicked
        EndCost += amt;
        if (EndCost > 0)
            msgbox.text = "Mineral cost of upgrades: " + EndCost;
        else if (EndCost < 0)
            msgbox.text = "Minerals refunded: " + EndCost;
        else
            msgbox.text = "";
        //only show save btn when player wants to level something
        if (EndCost != 0)
            b_save.gameObject.SetActive(true);
        else
            b_save.gameObject.SetActive(false);
    }

    public bool CanAfford(int extraCost) {
        if (pdata.SC >= EndCost + extraCost) return true;

        return false;
    }

    public void BackToMainScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
