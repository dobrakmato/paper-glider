using System;
using UnityEngine;

public class SinMover : MonoBehaviour
{
    public Vector3 Range;
    public float Offset;
    public float Speed = 1f;

    private Vector3 _startPos;

    void Start()
    {
        _startPos = transform.localPosition;
    }

    void Update()
    {
        var offset = Mathf.Sin(Offset * 2 * Mathf.PI + Time.time * Speed) * Range;
        transform.localPosition = _startPos + offset;
    }
}