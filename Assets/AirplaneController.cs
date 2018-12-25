using System;
using System.Collections;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AirplaneController : MonoBehaviour
{
    [Header("Properties")] public float MaxHeight = 3.5f;
    public float Altitude;
    public float Fall = 2f;
    public float ThrustPower = 8f;
    public float Score;

    [Header("GUIs")] public Text ScoreGui;
    public Text WastedGui;
    public Text FireAlertGui;
    public Text FriendTextGui;
    public Slider LevelProgressBarGui;
    public Text CoinCountGui;
    public Text DiamondCountGui;

    [Header("Audio")] public AudioSource FireAlarm;

    [Header("Other")] public GameObject AiPlane;
    public ParticleSystem LeftTrail;
    public ParticleSystem RightTrail;

    [Header("Prefabs")] public GameObject Explosion;
    public GameObject Fire;

    private Vector3 startPosition;
    private bool isAscending;
    private bool isAscendingBecauseOfStart = true;
    private bool isDoingABarrelRoll;
    public bool isDead;
    private bool isControlLost;
    private bool isIgnited;
    private float barrelRoll;

    void Start()
    {
        if (Application.isMobilePlatform)
        {
            Input.gyro.enabled = true;
        }

        var currentCoinCount = PlayerPrefs.GetFloat("Coins", 0);
        CoinCountGui.text = ((int) currentCoinCount).ToString();
        var currentDiamondCount = PlayerPrefs.GetInt("Diamonds", 0);
        DiamondCountGui.text = currentDiamondCount.ToString();

        startPosition = transform.position;
        Invoke("DoABarrelRoll", Random.Range(5f, 20f));
        StartCoroutine("FlipFriendText");
    }

    void DoABarrelRoll()
    {
        /* start only if in control */
        if (!isDead && !isControlLost)
        {
            barrelRoll = 0f;
            isDoingABarrelRoll = true;
        }

        Invoke("DoABarrelRoll", Random.Range(5f, 20f));
    }

    void Update()
    {
        isAscending = isAscendingBecauseOfStart | Input.GetButton("Fire1");
        if (isDead)
        {
            if (Score > 0)
            {
                ScoreGui.text = "You: " + (int) Score + " pts";
            }
            else
            {
                var highscore = 0;
                if (highscore > 0)
                {
                    ScoreGui.text = "Today best: " + (int) highscore + " pts";
                }
                else
                {
                    ScoreGui.text = "No highscore for TODAY!";
                }
            }
        }
        else
        {
            ScoreGui.text = "You: " + (int) Score + " pts";

            if (Input.GetKeyDown(KeyCode.Space))
            {
                UseSpeedPowerUp();
            }
        }


        LevelProgressBarGui.value = Score / 1000f;
    }

    IEnumerator FlipFriendText()
    {
        yield return new WaitForSeconds(Random.Range(5, 8));
        while (true)
        {
            FriendTextGui.gameObject.GetComponent<Animation>().Play();
            yield return new WaitForSeconds(0.5f);
            FriendTextGui.text = "Shakica: " + Random.Range(4000, 16000) + " pts";
            yield return new WaitForSeconds(Random.Range(7, 14));
        }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            /* score */
            Score += 0.5f;

            /* altitude control */

            var lastAltitude = Altitude;
            if (isAscending && !isControlLost)
            {
                Altitude += ThrustPower * Time.fixedDeltaTime;
            }

            Altitude -= Fall * Time.fixedDeltaTime;
            Altitude = Mathf.Clamp(Altitude, -6f, MaxHeight);

            var controllerHorizontal = Input.GetAxis("Horizontal");
            var gyroHorizontal = Input.gyro.gravity.x * 4f;

            var horizontalInput = Mathf.Clamp(controllerHorizontal + gyroHorizontal, -1f, 1f);

            var longitude = -horizontalInput;
            var roll = 45 * -horizontalInput;
            var pitch = 90 * (Altitude - lastAltitude);

            if (isDoingABarrelRoll)
            {
                roll = barrelRoll;
                barrelRoll += 360 * Time.fixedDeltaTime;

                if (barrelRoll > 360)
                {
                    isDoingABarrelRoll = false;
                }
            }

            transform.rotation = Quaternion.Euler(pitch, 0, roll);
            transform.position = startPosition + new Vector3(longitude, Altitude, 0);
        }
    }

    private void TurnOffTrails()
    {
        LeftTrail.Stop();
        RightTrail.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle") && !isDead)
        {
            isDead = true;
            Crash(-3f);
            Instantiate(Explosion, transform);
            TurnOffTrails();
            WastedGui.gameObject.SetActive(true);
            if (Camera.main != null) Camera.main.GetComponent<ScriptedMovement>().StartMovement();
            AiPlane.GetComponent<AIAirplane>().IsDead = false;
            FindObjectOfType<LevelGenerator>().IsDead = true;
            FindObjectOfType<LevelGenerator>().Speed = 0f;
            Invoke("RestartLevel", 2);
        }
        else if (other.gameObject.CompareTag("SpeedPowerUp"))
        {
            UseSpeedPowerUp();
        }
    }

    private void UseSpeedPowerUp()
    {
        FindObjectOfType<LevelGenerator>().StartCoroutine("LevelSpeedup", new LevelGenerator.SpeedUpParams
        {
            Time = 0.5f,
            TargetSpeed = LevelGenerator.DefaultSpeed * 2,
            TargetFov = 90f
        });

        Invoke("TriggerSlowDown", 5f);
    }

    private void TriggerSlowDown()
    {
        FindObjectOfType<LevelGenerator>().StartCoroutine("LevelSpeedup", new LevelGenerator.SpeedUpParams
        {
            Time = 0.5f,
            TargetSpeed = LevelGenerator.DefaultSpeed,
            TargetFov = 70f
        });
    }

    public void Crash(float direction = -5f)
    {
        var rb = GetComponent<Rigidbody>();
        var mT = 4f; // max torque 
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.velocity = new Vector3(0, 2f, direction);
        rb.AddTorque(new Vector3(Random.Range(-mT, mT), Random.Range(-mT, mT), Random.Range(-mT, mT)));
    }

    [Obsolete]
    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartGame()
    {
        // restart
        isDead = false;
        isAscendingBecauseOfStart = true;
        Invoke("StopStartAscension", 0.3f);
        FindObjectOfType<LevelGenerator>().RestartLevel();
    }

    private void StopStartAscension()
    {
        isAscendingBecauseOfStart = false;
    }

    public void Ignite()
    {
        if (!isIgnited)
        {
            isIgnited = true;
            FireAlertGui.gameObject.SetActive(true);
            FireAlarm.Play();
            Instantiate(Fire, transform);
            isControlLost = true;
        }
    }

    public void Explode()
    {
        Debug.Log("Explode");
        Instantiate(Explosion, transform);
        isDead = true;
        Crash();
        TurnOffTrails();
        WastedGui.gameObject.SetActive(true);
        if (Camera.main != null) Camera.main.GetComponent<ScriptedMovement>().StartMovement();
        AiPlane.GetComponent<AIAirplane>().IsDead = false;
        FindObjectOfType<LevelGenerator>().IsDead = true;
        FindObjectOfType<LevelGenerator>().Speed = 0f;
        Invoke("RestartLevel", 2);
    }

    public void GiveCoin()
    {
        var multiplier = PlayerPrefs.GetFloat("CoinMultiplier", 1f);
        var current = PlayerPrefs.GetFloat("Coins", 0);
        var income = 1 * multiplier;
        CoinCountGui.text = ((int) (current + income)).ToString();
        CoinCountGui.gameObject.GetComponent<Animation>().Play(PlayMode.StopAll);
        PlayerPrefs.SetFloat("Coins", current + income);
    }
}