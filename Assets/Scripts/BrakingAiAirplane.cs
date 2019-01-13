using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BrakingAiAirplane : MonoBehaviour
{
    public GameObject Player;

    public float StartBrakingDistance = 15f;
    public float BrakingTime = 3f;

    public GameObject Explosion;

    public float Altitude;
    public float Speed = 1f;
    public float Fall = 2f;
    public float ThrustPower = 8f;
    public bool ShouldFlyUp;
    public bool IsDead = true;
    public bool IsAttacking;

    private float _zPos;
    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
        Player = FindObjectOfType<AirplaneController>().gameObject;
    }

    void Update()
    {
        if (!IsDead)
        {
            _zPos += -Speed * Time.deltaTime;
            var lastAltitude = Altitude;

            if (ShouldFlyUp)
            {
                Altitude += (Speed > 0 ? ThrustPower / 2 : ThrustPower) * Time.fixedDeltaTime;
            }

            var targetX = 0f;
            if (_zPos < -3f)
            {
                var dx = Player.transform.position.x - transform.position.x;
                targetX = transform.position.x + dx * Mathf.Clamp01(Time.deltaTime * 2f);
            }

            Altitude -= Fall * Time.fixedDeltaTime;
            var pitch = 90 * (Altitude - lastAltitude);

            transform.rotation = Quaternion.Euler(pitch, 0, Mathf.Sin(Time.time * 2f) * 15);
            transform.position = _startPosition + new Vector3(targetX, Altitude, _zPos);

            if (Vector3.Distance(transform.position, Player.transform.position) > StartBrakingDistance && !IsAttacking)
            {
                StartCoroutine("Attack");
            }

            if (transform.position.z > 5f)
            {
                IsAttacking = false;
                Speed = 16;
            }

            if (IsAttacking)
            {
                if (Speed > -10f && transform.position.y - 2f < Player.transform.position.y)
                {
                    ShouldFlyUp = true;
                    CancelInvoke("StopFlyingUp");
                    Invoke("StopFlyingUp", 0.05f);
                }
            }
        }
    }

    private IEnumerator Attack()
    {
        IsAttacking = true;
        var targetSpeed = 2 * Speed;
        while (Speed > -20)
        {
            Speed = 12 - Mathf.SmoothDamp(Speed, targetSpeed, ref Speed, BrakingTime, 20f);
            yield return null;
        }

        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        ShouldFlyUp = true;
        Invoke("StopFlyingUp", 0.2f);
    }

    private void StopFlyingUp()
    {
        ShouldFlyUp = false;
    }

    public void Explode()
    {
        Instantiate(Explosion, transform);
        IsDead = true;
        var rb = GetComponent<Rigidbody>();
        var mT = 16f; // max torque 
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.velocity = new Vector3(0, 2f, -5f);
        rb.AddTorque(new Vector3(Random.Range(-mT, mT), Random.Range(-mT, mT), Random.Range(-mT, mT)));
    }
}