using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] TMP_Text LBtext;
    [SerializeField] TMP_Dropdown dd_lbtype;

    //changed by buttons
    enum CurrLB {
        All,
        Nearby,
    }
    CurrLB currLB = CurrLB.Nearby;//default

    private void OnEnable() {
        OnGetNearbyLB();
    }

    public void RefreshLB() {
        if (currLB == CurrLB.All) {
            OnButtonGetLeaderboard();
        }
        else {
            OnGetNearbyLB();
        }
    }

    //Get Top player scores
    public void OnButtonGetLeaderboard() {
        currLB = CurrLB.All;
        LBtext.text = "Loading...";
        var req = new GetLeaderboardRequest {
            StatisticName = dd_lbtype.options[dd_lbtype.value].text, //playfab leaderboard statistic name
            StartPosition = 0,
            MaxResultsCount = 10,
        };
        PlayFabClientAPI.GetLeaderboard(req, OnLeaderboardGet, OnError);
    }

    //get scores around the player
    public void OnGetNearbyLB() {
        currLB = CurrLB.Nearby;
        LBtext.text = "Loading...";
        var req = new GetLeaderboardAroundPlayerRequest {
            StatisticName = dd_lbtype.options[dd_lbtype.value].text
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(req, OnNearbyLBGet, OnError);
    }

    void OnNearbyLBGet(GetLeaderboardAroundPlayerResult r) {
        string lb = "<color=black>"+ dd_lbtype.options[dd_lbtype.value].text + " Leaderboard</color>\n";
        foreach (var item in r.Leaderboard) {
            string row = "\n" + (item.Position == 0 ? "<color=orange>" : "")+(item.Position + 1) + " : " + item.DisplayName + " | " + item.StatValue + (item.Position == 0 ? "</color>" : "")+"\n";
            lb += row;
        }
        LBtext.text = lb;
    }

    void OnLeaderboardGet(GetLeaderboardResult r) {
        string lb = "<color=black>"+dd_lbtype.options[dd_lbtype.value].text + " Leaderboard</color>\n";
        foreach(var item in r.Leaderboard) {
            string row =  "\n" +(item.Position==0 ? "<color=orange>" : "") + (item.Position+1) + " : " + item.DisplayName + " | " + item.StatValue + (item.Position == 0 ? "</color>" : "") +"\n";
            lb += row;
        }
        LBtext.text = lb;
    }

    void OnError(PlayFabError e) {
        LBtext.text = "Error: " + e.GenerateErrorReport();
    }
}
