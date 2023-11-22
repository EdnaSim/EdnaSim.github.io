using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using PlayFab;

public class GameController : MonoBehaviour {
    public static GameController Instance;
    public Vector3 positionAsteroid;
    public GameObject asteroid;
    public GameObject asteroid2;
    public GameObject asteroid3;
    public int hazardCount;
    public float startWait;
    public float spawnWait;
    public float waitForWaves;
    public Text scoreText;
    public Text gameOverText;
    public Text restartText;
    public Text mainMenuText;

    private bool restart;
    private bool gameOver;
    private int score;
    private List<GameObject> asteroids;
    private bool paused = false;

    private void Start() {
        Instance = this;
        asteroids = new List<GameObject> {
            asteroid,
            asteroid2,
            asteroid3
        };
        gameOverText.text = "";
        restartText.text = "Press P to pause";
        mainMenuText.text = "";
        restart = false;
        gameOver = false;
        score = 0;
        StartCoroutine(spawnWaves());
        updateScore();
    }

    private void Update() {
        if(restart || paused){
            if(Input.GetKey(KeyCode.R)){
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            } 
            else if(Input.GetKey(KeyCode.Q)){
                SceneManager.LoadScene("Menu");
            }
        }
        if (gameOver && !restart) {
            restartText.text = "Press R to restart game";
            mainMenuText.text = "Press Q to go back to main menu";
            restart = true;
            InventoryManager.Instance.GainSC(score);
            ScoreManager.Instance.UpdateScoreOnPF(score);
            PFDataManager.Instance.GainXP(score/2);
        }
        //pause game
        if (!restart && Input.GetKeyDown(KeyCode.P)) {
            Pause(!paused);
        }

    }

    private IEnumerator spawnWaves(){
        yield return new WaitForSeconds(startWait);
        while(true){
            for (int i = 0; i < hazardCount;i++){
                Vector3 position = new Vector3(Random.Range(-positionAsteroid.x, positionAsteroid.x), positionAsteroid.y, positionAsteroid.z);
                Quaternion rotation = Quaternion.identity;
                Instantiate(asteroids[Random.Range(0,3)], position, rotation);
                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waitForWaves);
            if(gameOver){
                break;
            }
        }
    }

    public void gameIsOver(){
        gameOverText.text = "Game Over";
        gameOver = true;
    }

    public void addScore(int score){
        this.score += score;
        updateScore();
    }

    void updateScore(){
        scoreText.text = "Score:" + score;
    }

    public bool isPaused() { return paused; }
    public void Pause(bool b) {
        paused = b;
        Time.timeScale = paused ? 0f : 1f;
        if (paused) {
            restartText.text = "Press R to restart game";
            mainMenuText.text = "Press Q to go back to main menu";
        }
        else {
            restartText.text = "Press P to pause";
            mainMenuText.text = "";
        }
    }
}
