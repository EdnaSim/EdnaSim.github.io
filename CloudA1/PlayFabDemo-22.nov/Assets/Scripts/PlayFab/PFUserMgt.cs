using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PFUserMgt : MonoBehaviour
{
    [SerializeField] TMP_Text msgbox;
    [Header("InputField")]
    [SerializeField] TMP_InputField if_username; //register
    [SerializeField] TMP_InputField if_email, //register
        if_password, //login and register
        if_password2, //register
        if_displayname, //register
        if_credential; //login
    [Header("Button")]
    [SerializeField] Button CfmRegBtn; //confirm reg
    [SerializeField]
    Button CfmLoginBtn, //confirm login
        WantRegBtn, //open register screen
        WantLoginbtn, //open login screen
        WantGuestLoginBtn;
    [SerializeField] Button BackBtn;
    [SerializeField] Button WantResetPwBtn, CfmResetPwbtn;

    private void OnEnable() {
        OnButtonBack();
    }
    //back to the reg/login btns
    public void OnButtonBack() {
        ToggleIFs(false, false, false, false, false, false, false);
        ToggleWantBtns(true);
        WantResetPwBtn.gameObject.SetActive(false);
        CfmResetPwbtn.gameObject.SetActive(false);
        //reset inputs
        if_email.text = "";
        if_username.text = "";
        if_password.text = "";
        if_password2.text = "";
        if_displayname.text = "";
        if_credential.text = "";
        BackBtn.gameObject.SetActive(false);
    }
    public void OnButtonWantRegister() {
        //on password2, displayname, and cfmregBtn
        ToggleIFs(true, false, false, true, true, true, true);
        ToggleWantBtns(false);
        msgbox.text = "";
    }
    public void OnButtonWantLogin() {
        //on email/usn, password1, and cfmLogin
        ToggleIFs(false, true, true, false, true, false,false);
        ToggleWantBtns(false);
        msgbox.text = "";
        WantResetPwBtn.gameObject.SetActive(true);
    }
    public void OnButtonForgotPw() {
        ToggleIFs(false, false, false, false, false, true, false);
        ToggleWantBtns(false);
        CfmResetPwbtn.gameObject.SetActive(true);
        WantResetPwBtn.gameObject.SetActive(false);
        msgbox.text = "";
    }
    private void ToggleIFs(bool cfmreg, bool cfmlog, bool credential, bool wantReg, bool pw1, bool email, bool usn = true) {
        if_username.gameObject.SetActive(usn);
        if_email.gameObject.SetActive(email);
        if_password.gameObject.SetActive(pw1);
        if_password2.gameObject.SetActive(wantReg);
        if_displayname.gameObject.SetActive(wantReg);
        if_credential.gameObject.SetActive(credential);
        CfmRegBtn.gameObject.SetActive(cfmreg);
        CfmLoginBtn.gameObject.SetActive(cfmlog);
    }
    private void ToggleWantBtns(bool b) {
        WantRegBtn.gameObject.SetActive(b);
        WantLoginbtn.gameObject.SetActive(b);
        WantGuestLoginBtn.gameObject.SetActive(b);
        BackBtn.gameObject.SetActive(!b);
    }

    public void OnButtonRegister() {
        //check if passwords from both IFs match
        if (if_password.text != if_password2.text) {
            msgbox.text = "Passwords do not match.";
            return;
        }
        var regReq = new RegisterPlayFabUserRequest {
            Email = if_email.text,
            Password = if_password.text,
            Username = if_username.text,
        };
        //execute request by calling playfab api
        PlayFabClientAPI.RegisterPlayFabUser(regReq, OnRegSucc, OnError);
    }

    public void OnButtonLogin() {
        //check if player logged in with email or username
        if (if_credential.text.Contains("@")) {
            msgbox.text = "Logging in, please wait...";
            var loginReq = new LoginWithEmailAddressRequest {
                Email = if_credential.text,
                Password = if_password.text,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
                    GetPlayerProfile = true
                }
            };
            PlayFabClientAPI.LoginWithEmailAddress(loginReq, OnLoginSucc, OnError);
        }
        else {
            msgbox.text = "Logging in, please wait...";
            var loginReq = new LoginWithPlayFabRequest {
                Username = if_credential.text,
                Password = if_password.text,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
                    GetPlayerProfile = true
                }
            };
            PlayFabClientAPI.LoginWithPlayFab(loginReq, OnLoginSucc, OnError);
        }
    }

    public void LoginWithCustomID() { //guest
        msgbox.text = "Logging in, please wait...";
        var req = new LoginWithCustomIDRequest {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
                GetPlayerProfile = true,
                ProfileConstraints = new PlayerProfileViewConstraints() {
                    ShowDisplayName = true,
                }
            }
        };
        PlayFabClientAPI.LoginWithCustomID(req, OnGuestLoginSucc, OnError);
    }

    public void OnGuestLoginSucc(LoginResult r) {
        if (r.NewlyCreated) {
            //create guest display name
            GenerateGuestDisplayName();
            //grant the starter items
            PFCloudScripts.Instance.ExeGrantStarterItemsScript();
        }
        else {
            OnLoginSucc(r);
        }
    }

    private void GenerateGuestDisplayName() {
        string name = "Guest" + GenerateRandomString(7);
        //search for an account with this name
        var req = new GetAccountInfoRequest { TitleDisplayName = name };
        PlayFabClientAPI.GetAccountInfo(req, result => {
            //found, name taken, try again
            GenerateGuestDisplayName();
        },
        error=> { //name not found, use it
            var req = new UpdateUserTitleDisplayNameRequest {
                DisplayName = name,
            };
            PlayFabClientAPI.UpdateUserTitleDisplayName(req, GuestDisplayNameUpdate, OnError);
        });    
    }

    void GuestDisplayNameUpdate(UpdateUserTitleDisplayNameResult r) {
        //get rid of all buttons
        OnButtonBack();
        ToggleWantBtns(false);
        BackBtn.gameObject.SetActive(false);

        msgbox.text = "Welcome, " + r.DisplayName + "\nTIP: Guest accounts are tied to your device!";
        StartCoroutine(WaitToLoadScene());
    }

    string GenerateRandomString(int length) {
        string characters = "1234567890";
        string gen = "";

        for (int i=0; i < length; i++) {
            gen += characters[Random.Range(0, characters.Length)];
        }
        return gen;
    }
    
    void OnRegSucc(RegisterPlayFabUserResult r) {
        msgbox.text = "Register Success! " + r.PlayFabId;
        //create player display name
        var req = new UpdateUserTitleDisplayNameRequest {
            DisplayName = if_displayname.text,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(req, OnDisplayNameUpdate, OnError);
        PFCloudScripts.Instance.ExeGrantStarterItemsScript();
        //go back to reg/login btns
        OnButtonBack();
    }
    //handle error
    void OnError(PlayFabError e) {
        msgbox.text = "<color=red>Error: " + e.GenerateErrorReport() + "</color>";
    }

    void OnLoginSucc(LoginResult r) {
        //get rid of all buttons
        OnButtonBack();
        ToggleWantBtns(false);
        BackBtn.gameObject.SetActive(false);

        msgbox.text = "Login Success! Welcome, " + r.InfoResultPayload?.PlayerProfile.DisplayName;
        StartCoroutine(WaitToLoadScene());
    }

    IEnumerator WaitToLoadScene() {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Menu");
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult r) {
        msgbox.text = "Display name updated: " + r.DisplayName;
    }
}
