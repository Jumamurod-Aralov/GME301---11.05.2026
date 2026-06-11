using UnityEngine;

public class Barrier : MonoBehaviour
{
    [SerializeField] private int health = 3;

    public void TakeDamage()
    {
        health--;
        Debug.Log($"Barrier Health: {health}");

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}