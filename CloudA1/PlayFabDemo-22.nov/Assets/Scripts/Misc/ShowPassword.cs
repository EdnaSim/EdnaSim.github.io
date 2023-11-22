using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowPassword : MonoBehaviour
{
    [SerializeField] Button btn;
    TMP_InputField field;
    bool isShown = false;
    Sprite StartSprite;

    // Start is called before the first frame update
    void Start()
    {
        field = GetComponent<TMP_InputField>();
        if (!isShown) {
            field.contentType = TMP_InputField.ContentType.Password;
        }
        StartSprite = btn.image.sprite;
    }

    //when going back and forth between menus, coming back will turn off visibility
    private void OnEnable() {
        if (isShown)
            ChangeVisibility(false);
    }

    public void OnBtnPress() {
        ChangeVisibility(!isShown);
    }

    //based on the isShown bool
    private void ChangeVisibility(bool b) {
        isShown = b;
        field.contentType = isShown ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        field.ForceLabelUpdate();
        btn.image.sprite = isShown ? btn.spriteState.pressedSprite : StartSprite;
    }
}
