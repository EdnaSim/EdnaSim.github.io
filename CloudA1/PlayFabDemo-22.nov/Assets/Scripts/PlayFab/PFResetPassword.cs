using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class PFResetPassword : MonoBehaviour
{
    [SerializeField] TMP_Text msgbox;
    public GameObject container;

    public void Activate() {
        container.SetActive(!container.activeSelf);
    }

    //change password
    public void ResetPassword() {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), SendEmail, OnError);
    }

    void SendEmail(GetAccountInfoResult r) {
        msgbox.text = "Sending email to " + r.AccountInfo.PrivateInfo.Email + "...";
        var req = new SendAccountRecoveryEmailRequest {
            Email = r.AccountInfo.PrivateInfo.Email,
            TitleId = PlayFabSettings.staticSettings.TitleId
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(req, OnEmailSentSucc, OnError);
    }

    void OnEmailSentSucc(SendAccountRecoveryEmailResult r) {
        msgbox.text = "Successfully sent email! Proceed to the email to change your password";
    }

    void OnError(PlayFabError e) {
        msgbox.text = "Error: " + e.GenerateErrorReport();
    }
}
