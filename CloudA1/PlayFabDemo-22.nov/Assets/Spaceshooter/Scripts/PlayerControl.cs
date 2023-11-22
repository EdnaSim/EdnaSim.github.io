using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boundary{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerControl : MonoBehaviour {
    [SerializeField] SO_PlayerData pdata;
    float bonusDamage = 0;
    bool doubleShot = false;
    GameController gameController;

    private Rigidbody playerRb;
    private AudioSource playerWeapon;
    public float speed;
    public float tiltMultiplier;
    public Boundary boundary;

    [SerializeField] float BaseHealth = 10f;
    float Health;

    [Header("Shooting")]
    public GameObject shot;
    public Transform shotSpawn;
    public Transform shotSpawn2;
    public float fireRate;

    private float nextFire;
    private CharacterSelection characterSelection;

    private void Start() {
        if (gameController == null) gameController = GameController.Instance;
        GameObject cSelectionObject = GameObject.FindWithTag("CharacterSelection");
        if (cSelectionObject != null) {
            characterSelection = cSelectionObject.GetComponent<CharacterSelection>();
        }
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null) {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        playerRb = GetComponent<Rigidbody>();
        playerWeapon = GetComponent<AudioSource>();
    }
    private void OnEnable() {
        if (gameController == null) gameController = GameController.Instance;
        StartCoroutine(GetSkills());
    }

    private void Update() {
        if (gameController.isPaused()) return;
        if (Input.GetButton("Jump") && Time.time > nextFire){
            nextFire = Time.time + fireRate;
            GameObject shot1 = Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            shot1.GetComponent<Bolt>().Damage += bonusDamage;
            if (doubleShot){
                Instantiate(shot, shotSpawn2.position, shotSpawn2.rotation);
                GameObject shot2 = Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                shot2.GetComponent<Bolt>().Damage += bonusDamage;
            }
            playerWeapon.Play();
        }

    }

    private void FixedUpdate() {
        if (gameController.isPaused()) return;
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        playerRb.velocity = new Vector3(moveHorizontal * speed, 0.0f, moveVertical * speed);

        playerRb.position = new Vector3(
            Mathf.Clamp(playerRb.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(playerRb.position.z, boundary.zMin, boundary.zMax)
        );

        playerRb.rotation = Quaternion.Euler(0.0f, 0.0f, -playerRb.velocity.x * tiltMultiplier);
    }

    public void Damage(float damage) {
        Health -= damage;
        //Debug.Log("PLAYER HEALTH: " + Health);
        if (Health <= 0) {
            Health = 0;
            gameController.gameIsOver();
            Destroy(gameObject);
        }
    }

    IEnumerator GetSkills() {
        gameController.Pause(true);
        PFDataManager.Instance.GetSkills();
        yield return new WaitUntil(() => pdata.skillsReady = true);

        foreach(Skill s in pdata.SkillList) {
            if (s.name == "Damage") {
                bonusDamage = (s.level - 1) / 2;
                if (bonusDamage < 0) bonusDamage = 0;
                continue;
            }
            if (s.name == "Double Shot") {
                if (s.level >= 1)
                    doubleShot = true;
                continue;
            }
            if (s.name == "Health") {
                Health = BaseHealth + (s.level * 2);
                continue;
            }
        }
        gameController.Pause(false);
    }
}
