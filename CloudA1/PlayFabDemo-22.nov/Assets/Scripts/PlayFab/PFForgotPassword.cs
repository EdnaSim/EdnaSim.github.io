using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class PFForgotPassword : MonoBehaviour
{
    [SerializeField] TMP_Text msgbox;
    [SerializeField] TMP_InputField if_email;
    
    public void OnForgotPWBtn() {
        var req = new SendAccountRecoveryEmailRequest {
            Email = if_email.text,
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
