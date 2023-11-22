using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PFDataManager : MonoBehaviour
{
    public static PFDataManager Instance;
    [SerializeField] TMP_Text LevelDisplay;
    [SerializeField] SO_PlayerData pdata;

    private void Awake() {
        Instance = this;
        //reset pdata otherwise new accounts will have the data of the last logged in acc
        pdata.Reset();
        GetData();
    }

    public void GainXP(int amt) {
        int XptoNext = XpToNextLevel();
        int overflow = 0;
        if (amt >= XptoNext) {
            //store overflow
            overflow = amt - XptoNext;
        }
        pdata.XP += amt;
        //level up if possible
        if (pdata.XP >= XptoNext) {
            pdata.Level++;
            pdata.XP = overflow;
        }
        //send xp and level to PF
        SendDataToPlayFab();
        //Send the player's level to PF leaderboard "Level"
        UpdateLBLevelOnPF();
    }

    public void SendDataToPlayFab() {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() {
            Data = new Dictionary<string, string>() {
            {"XP", pdata.XP.ToString()},
            {"Level", pdata.Level.ToString() },
        }
        },
        result => Debug.Log("Successfully updated user data"),
        error => {
            Debug.Log("Got error setting user XP");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void GetData() {
        pdata.XpLevelReady = false;
        if(LevelDisplay != null)LevelDisplay.text = "Loading...";
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), 
        result => {
            if (result.Data != null) {
                if (result.Data.ContainsKey("XP")) {
                    pdata.XP = int.Parse(result.Data["XP"].Value);
                }
                if (result.Data.ContainsKey("Level")) {
                    pdata.Level = int.Parse(result.Data["Level"].Value);
                }
            }
            pdata.XpLevelReady = true;
            UpdateLevelDisplay();
            UpdateLBLevelOnPF();
        }, 
        (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
            pdata.XpLevelReady = true;
        });
    }

    public void GetSkills() {
        pdata.skillsReady = false;
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnSkillDataReceived, OnError);
    }

    void OnSkillDataReceived(GetUserDataResult r) {
        //Debug.Log("Received JSON data");
        if (r.Data != null && r.Data.ContainsKey("Skills")) {
            JSListWrapper<Skill> jlw = JsonUtility.FromJson<JSListWrapper<Skill>>(r.Data["Skills"].Value);
            pdata.SkillList = jlw.list;
            pdata.skillsReady = true;
        }
    }

    void UpdateLevelDisplay() {
        if (LevelDisplay == null) return;
        LevelDisplay.text = "Level " + pdata.Level + "\n\nXP to level up: " + XpToNextLevel();
    }

    int XpToNextLevel() {
        return (pdata.Level * 15 + 100) - pdata.XP;
    }

    //send level as leaderboard statistic
    public void UpdateLBLevelOnPF() {
        var req = new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "Level",
                    Value = pdata.Level
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(req, GetLevelFromPF, OnError);
    }

    void GetLevelFromPF(UpdatePlayerStatisticsResult r) {
        var req = new GetPlayerStatisticsRequest {
            StatisticNames = new List<string> {
                "Level"
            }
        };
        PlayFabClientAPI.GetPlayerStatistics(req, GetLevelSucc, OnError);
    }

    void GetLevelSucc(GetPlayerStatisticsResult r) {
        //Debug.Log("got LB level");
    }

    void OnError(PlayFabError e) {
        Debug.LogError("Error" + e.GenerateErrorReport());
    }
}
