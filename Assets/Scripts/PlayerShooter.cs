using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rayDistance = 1000f;
    [SerializeField] private LayerMask enemyLayer; // CHANGED: Only shoot enemies

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        // CHANGED: Shoot on left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // CHANGED: Raycast from screen center
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, enemyLayer))
        {
            // CHANGED: Get hit object
            GameObject hitObject = hit.collider.gameObject;
            Debug.Log($"Hit: {hitObject.name}");

            // CHANGED: Check if it's an enemy
            EnemyAI enemy = hitObject.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                // CHANGED: Trigger death + flash red
                enemy.TakeDamage();
                StartCoroutine(FlashRed(hitObject));
            }
        }
    }

    // CHANGED: Flash enemy red for visual feedback
    System.Collections.IEnumerator FlashRed(GameObject enemy)
    {
        Renderer renderer = enemy.GetComponent<Renderer>();
        Color originalColor = renderer.material.color;

        renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        renderer.material.color = originalColor;
    }
}