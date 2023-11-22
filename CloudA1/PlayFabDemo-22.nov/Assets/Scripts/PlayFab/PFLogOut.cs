using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;

public class PFLogOut : MonoBehaviour
{
    public void OnButtonLogOut() {
        PlayFabClientAPI.ForgetAllCredentials();
        SceneManager.LoadScene("SampleScene");
    }
}
