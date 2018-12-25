using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedMovement : MonoBehaviour
{
    public Vector3 Movement = new Vector3(0, 0, 1f);
    public float Speed = 1f;

    private Vector3 target;
    private float smoothTime = 3F;
    private Vector3 velocity = Vector3.zero;
    private float _f;

    public void StartMovement()
    {
        _f = 1f;
        target = transform.position + Movement;
        StartCoroutine("Move");
    }

    IEnumerator Move()
    {
        while (_f > 0f)
        {
            _f -= Time.deltaTime * Speed;
            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
            yield return null; // wait for next frame
        }
    }
}