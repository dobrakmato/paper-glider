using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAirplane : MonoBehaviour
{
    public GameObject following;

    private Vector3 _offset;

    void Start()
    {
        _offset = transform.position - following.transform.position;
    }

    void Update()
    {
        transform.position = following.transform.position + _offset;
    }
}