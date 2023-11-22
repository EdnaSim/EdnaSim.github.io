using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class PFAccountLinking : MonoBehaviour
{
    [SerializeField] TMP_Text msgbox;
    public GameObject container;
    [SerializeField]
    TMP_InputField
        if_username,
        if_password,
        if_password2,
        if_email,
        if_displayname;

    public void OnAccLinkButton() {
        container.SetActive(!container.activeSelf);
    }

    public void OnAccountLink() {
        if (if_password.text != if_password2.text) {
            msgbox.text = "Passwords do not match.";
            return;
        }
        var req = new AddUsernamePasswordRequest {
            Email = if_email.text,
            Password = if_password.text,
            Username = if_username.text,
        };
        PlayFabClientAPI.AddUsernamePassword(req, OnLinkSucc, OnError);
    }

    void OnLinkSucc(AddUsernamePasswordResult r) {
        msgbox.text = "Successfully linked!";
        //create player display name
        var req = new UpdateUserTitleDisplayNameRequest {
            DisplayName = if_displayname.text,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(req, OnDisplayNameUpdate, OnError);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult r) {
        msgbox.text = "Display name updated: " + r.DisplayName;
    }

    void OnError(PlayFabError e) {
        msgbox.text = "Error: " + e.GenerateErrorReport();
    }
}
