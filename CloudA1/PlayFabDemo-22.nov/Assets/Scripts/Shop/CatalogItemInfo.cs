using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CatalogItemInfo : MonoBehaviour
{
    //to be stored in SO_ShopModel for ShopController to access
    public string id; //PF id
    public TYPE type;

    public enum TYPE {
        CHARACTER,
        GUN
    }
}
