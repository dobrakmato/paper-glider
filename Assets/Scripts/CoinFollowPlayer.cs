using UnityEngine;

public class CoinFollowPlayer : MonoBehaviour
{
    public GameObject Player;
    public float Speed;

    private void Update()
    {
        if (Player != null)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, Player.transform.position, Speed * Time.deltaTime);
        }
    }
}