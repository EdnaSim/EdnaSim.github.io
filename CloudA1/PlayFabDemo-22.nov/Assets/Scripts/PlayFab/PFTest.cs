using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;

public class PFTest : MonoBehaviour
{
    public static PFTest Instance;
    [SerializeField] TMP_Text MOTDBox;
    const float TimeBetweenMOTDUpdates = 5f;
    float motdTimer = 0f;

    public void Start() {
        Instance = this;
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) {
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "F52E1";
        }
        var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void Update() {
        motdTimer += Time.deltaTime;
        if (motdTimer >= TimeBetweenMOTDUpdates) {
            motdTimer = 0f;
            PFCloudScripts.Instance.ExeGetMOTDScript();
        }
    }

    private void OnLoginSuccess(LoginResult result) {
        Debug.Log("Congratulations, you made your first successful API call!");
        //ClientGetTitleData();
        PFCloudScripts.Instance.ExeGetMOTDScript();
    }

    private void OnLoginFailure(PlayFabError error) {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    public void ClientGetTitleData() { //using PFCloudScripts instead
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
            result => {
                if (result.Data == null || !result.Data.ContainsKey("MOTD"))
                    MOTDBox.text = "No MOTD :(";
                else
                    MOTDBox.text = "MOTD: " + result.Data["MOTD"];
            },
            error => {
                MOTDBox.text = "Error getting title data: " + error.GenerateErrorReport();
            }
        );
    }

    public void UpdateMOTD(string _motd) {
        MOTDBox.text = _motd;
    }
}
