using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private int score = 0; //high score

    private void Awake() {
        Instance = this;
    }

    public int GetHighScore() {
        return score;
    }
    public void UpdateScoreOnPF(int score) {
        var req = new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "HighScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(req, GetScoreFromPF, OnError);
    }

    void GetScoreFromPF(UpdatePlayerStatisticsResult r) {
        var req = new GetPlayerStatisticsRequest {
            StatisticNames = new List<string> {
                "HighScore"
            }
        };
        PlayFabClientAPI.GetPlayerStatistics(req, GetScoreSucc, OnError);
    }

    void GetScoreSucc(GetPlayerStatisticsResult r) {
        score = r.Statistics[0].Value;
    }

    void OnError(PlayFabError e) {
        Debug.Log("Error: " + e.GenerateErrorReport());
    }
}
