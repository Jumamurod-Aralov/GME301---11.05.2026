using UnityEngine;
using System.Collections;

public class Barrier : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float regenDelay = 10f;
    [SerializeField] private Renderer meshRenderer;

    private float currentHealth;
    private Material material;
    private bool isRegenerating = false;

    void Start()
    {
        currentHealth = maxHealth;
        if (meshRenderer == null)
            meshRenderer = GetComponent<Renderer>();
        material = meshRenderer.material;
    }

    public void TakeDamage()
    {
        if (isRegenerating) return;

        currentHealth--;
        Debug.Log($"Barrier Hit! Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            DisableBarrier();
        }
    }

    private void DisableBarrier()
    {
        meshRenderer.enabled = false;
        GetComponent<Collider>().enabled = false;
        StartCoroutine(RegenerateBarrier());
    }

    private IEnumerator RegenerateBarrier()
    {
        isRegenerating = true;
        yield return new WaitForSeconds(regenDelay);

        // Restore
        currentHealth = maxHealth;
        meshRenderer.enabled = true;
        GetComponent<Collider>().enabled = true;
        isRegenerating = false;

        Debug.Log("Barrier Regenerated!");
    }
}