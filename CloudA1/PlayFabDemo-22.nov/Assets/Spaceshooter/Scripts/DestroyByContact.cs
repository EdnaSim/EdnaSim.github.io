using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByContact : MonoBehaviour {

    public GameObject explosion;
    public GameObject playerExplosion;
    public int scoreValue;
    GameController gameController;
    bool isDestroyed = false;
    AsteroidHealth hp;

    private void Start() {
        hp = GetComponent<AsteroidHealth>();
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if(gameControllerObject != null){
            gameController = gameControllerObject.GetComponent<GameController>();
        } 
        else{
            Debug.Log("GameController object not found");
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
            other.GetComponent<PlayerControl>().Damage(hp.Health);
        }
        //if(other.tag != "Boundary"){
        //    Instantiate(explosion, transform.position, transform.rotation);
        //    if(other.tag == "Player"){
        //        Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
        //        gameController.gameIsOver();
        //    }
        //    gameController.addScore(scoreValue);
        //    Destroy(other.gameObject);
        //    Destroy(gameObject);
        //}
    }

    public void Destroy() {
        if (isDestroyed) return;

        isDestroyed = true;
        Instantiate(explosion, transform.position, transform.rotation);
        gameController.addScore(scoreValue);
        Destroy(gameObject);
    }
}
