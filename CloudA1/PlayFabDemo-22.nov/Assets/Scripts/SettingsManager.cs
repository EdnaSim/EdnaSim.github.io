using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] GameObject SettingsMenu;
    [SerializeField] PFAccountLinking AccLink;
    [SerializeField] PFResetPassword ResetPW;
    
    public void OnSettingsBtn() {
        SettingsMenu.SetActive(!SettingsMenu.activeSelf);
    }

    public void OnAccLinkBtn() {
        ResetPW.container.SetActive(false);
        AccLink.OnAccLinkButton();
    }

    public void OnChangePWBtn() {
        AccLink.container.SetActive(false);
        ResetPW.Activate();
    }
}
