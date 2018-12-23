using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AirplaneController : MonoBehaviour
{
    public float MaxHeight = 3.5f;
    public float Altitude;
    public float Fall = 2f;
    public float ThrustPower = 8f;
    public float Score;

    public Text ScoreGui;
    public Text WastedGui;
    public GameObject AiPlane;
    public GameObject Explosion;

    private Vector3 startPosition;
    private bool isAsceding;
    private bool isDoingABarrelRoll;
    private bool isDead;
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
        barrelRoll = 0f;
        isDoingABarrelRoll = true;
        Invoke("DoABarrelRoll", Random.Range(5f, 20f));
    }

    void Update()    
    {
        isAsceding = Input.GetButton("Fire1");
        ScoreGui.text =  (Score/10).ToString("0.00") + "%";
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            /* score */
            Score += 0.5f;

            /* altitude control */
            var lastAltitude = Altitude;
            if (isAsceding)
            {
                Altitude += ThrustPower * Time.fixedDeltaTime;
            }

            Altitude -= Fall * Time.fixedDeltaTime;
            Altitude = Mathf.Clamp(Altitude, -6f, MaxHeight);

            var controllerHorizontal = Input.GetAxis("Horizontal");
            var gyroHorizontal = Input.gyro.gravity.x * 3.5f;

            var horizontalInput = Mathf.Clamp(controllerHorizontal + gyroHorizontal, -1f, 1f);

            var longtitude = -horizontalInput;
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
            transform.position = startPosition + new Vector3(longtitude, Altitude, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with " + other.gameObject);
        if (other.gameObject.CompareTag("Obstacle"))
        {
            isDead = true;
            Instantiate(Explosion, transform);
            WastedGui.gameObject.SetActive(true);
            AiPlane.GetComponent<AIAirplane>().IsDead = false;
            FindObjectOfType<LevelGenerator>().IsDead = true;
            FindObjectOfType<LevelGenerator>().Speed = 0f;
            Invoke("RestartLevel", 2);
        }
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}