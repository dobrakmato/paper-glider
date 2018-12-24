using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleConstraints : MonoBehaviour
{
    public bool X;
    public bool Y;
    public bool Z;
    public int MaxParticles = 100;
    private ParticleSystem _system;

    private ParticleSystem.Particle[] _particles; /* do not allocate in update() */

    private void Awake()
    {
        _system = GetComponent<ParticleSystem>();
        _particles = new ParticleSystem.Particle[MaxParticles];
    }

    void Update()
    {
        var count = _system.GetParticles(_particles);
        for (int i = 0; i < count; i++)
        {
            var tmp = _particles[i].position;
            _particles[i].position = new Vector3(X ? transform.position.x : tmp.x, Y ? transform.position.y : tmp.y,
                Z ? transform.position.z : tmp.z);
        }

        _system.SetParticles(_particles, count);
    }
}