using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Audio")] public AudioSource FireAlarm;

    [Header("Other")] public GameObject AiPlane;
    public ParticleSystem LeftTrail;
    public ParticleSystem RightTrail;

    [Header("Prefabs")] public GameObject Explosion;
    public GameObject Fire;

    private Vector3 startPosition;
    private bool isAscending;
    private bool isDoingABarrelRoll;
    private bool isDead;
    private bool isControlLost;
    private bool isIgnited;
    private float barrelRoll;

    void Start()
    {
        if (Application.isMobilePlatform)
        {
            Input.gyro.enabled = true;
        }

        startPosition = transform.position;
        Invoke("DoABarrelRoll", Random.Range(5f, 20f));
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
        isAscending = Input.GetButton("Fire1");
        ScoreGui.text = "You: " + (int)Score + " pts";
        LevelProgressBarGui.value = Score / 1000f;
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
            Crash();
            Instantiate(Explosion, transform);
            TurnOffTrails();
            WastedGui.gameObject.SetActive(true);
            if (Camera.main != null) Camera.main.GetComponent<ScriptedMovement>().StartMovement();
            AiPlane.GetComponent<AIAirplane>().IsDead = false;
            FindObjectOfType<LevelGenerator>().IsDead = true;
            FindObjectOfType<LevelGenerator>().Speed = 0f;
            Invoke("RestartLevel", 2);
        }
    }

    private void Crash()
    {
        var rb = GetComponent<Rigidbody>();
        var mT = 4f; // max torque 
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.velocity = new Vector3(0, 2f, -5f);
        rb.AddTorque(new Vector3(Random.Range(-mT, mT), Random.Range(-mT, mT), Random.Range(-mT, mT)));
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
}