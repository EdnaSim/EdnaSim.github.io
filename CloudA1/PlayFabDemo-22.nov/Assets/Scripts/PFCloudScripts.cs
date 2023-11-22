using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PFCloudScripts : MonoBehaviour
{
    public static PFCloudScripts Instance;
    [SerializeField] TMP_Text msgbox;

    private void Start() {
        Instance = this;
    }

    public void ExeGrantStarterItemsScript() {
        var req = new ExecuteCloudScriptRequest {
            FunctionName = "UnlockStarterItems",
            FunctionParameter = new { }
        };
        PlayFabClientAPI.ExecuteCloudScript(req, OnExecSucc, OnError);
    }

    public void ExeGetMOTDScript() {
        var req = new ExecuteCloudScriptRequest {
            FunctionName = "GetMOTD",
            FunctionParameter = new { }
        };
        string motd = "No MOTD :(";
        PlayFabClientAPI.ExecuteCloudScript(req, (ExecuteCloudScriptResult r) => { 
            motd = r?.FunctionResult?.ToString(); 
            PFTest.Instance.UpdateMOTD(motd);
        },
        OnError);
    }

    void OnExecSucc(ExecuteCloudScriptResult r) {
        Debug.Log("CLOUDSCRIPT RETURNED:  " +r?.FunctionResult?.ToString());
    }

    void OnError(PlayFabError e) {
        msgbox.text = "<color=red>Error: " + e.GenerateErrorReport() + "</color>";
    }
}
