using System;
using UnityEngine;

public class AIAirplane : MonoBehaviour
{
    public float Altitude;
    public float Speed = 1f;
    public float Fall = 2f;
    public float ThrustPower = 8f;
    public bool IsFlyingUp;
    public bool IsDead = true;

    private float _zPos;
    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        if (!IsDead)
        {
            _zPos += -Speed * Time.deltaTime;
            var lastAltitude = Altitude;
            if (IsFlyingUp)
            {
                Altitude += ThrustPower * Time.fixedDeltaTime;
            }

            Altitude -= Fall * Time.fixedDeltaTime;
            var pitch = 90 * (Altitude - lastAltitude);

            transform.rotation = Quaternion.Euler(pitch, 0, Mathf.Sin(Time.time * 2f) * 15);
            transform.position = _startPosition + new Vector3(0, Altitude, _zPos);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IsFlyingUp = true;
        Invoke("StopFlyingUp", 0.3f);
    }

    private void StopFlyingUp()
    {
        IsFlyingUp = false;
    }
}