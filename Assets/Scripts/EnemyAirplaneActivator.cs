using Level;
using UnityEngine;


public class EnemyAirplaneActivator : MonoBehaviour
{
    public GameObject BrakingEnemy;
    public GameObject Target;

    private void Start()
    {
        Target = FindObjectOfType<AirplaneController>().gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<AirplaneController>() != null)
        {
            var p = Target.transform.position;
            var oy = LevelRandom.Range(-3f, 3f);
            var ox = Mathf.Abs(p.x) < 0.5 ? LevelRandom.Range(-1f, 1f) : 0f; 
            if (Mathf.Abs(oy - p.y) < 1f) oy += 1f;
            Instantiate(BrakingEnemy, new Vector3(-p.x + ox, Mathf.Clamp(p.y + oy, 0f, 5f), 9f), Quaternion.identity);
            Destroy(gameObject);
        }
    }
}