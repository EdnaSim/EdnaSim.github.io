using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidHealth : MonoBehaviour
{
    public float Health = 10f;
    [SerializeField] DestroyByContact destroy;

    public void Damage(float damage) {
        Health -= damage;
        if (Health <= 0) {
            Health = 0;
            destroy.Destroy();
        }
    }
}
