using System.Threading;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rayDistance = 1000f;
    [SerializeField] private LayerMask _shootLayer; // Enemy AI + Barrier
    [SerializeField] private int _ammoCount = 30;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        _shootLayer = LayerMask.GetMask("Enemy", "Barrier");

        // Task - Player - 5th
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Shoot on left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (!GameManager.Instance.IsGameActive()) return; // Check if game active

        int ammo = UIManager.Instance.GetCurrentAmmo(); // Get Ammo From UI
        if (_ammoCount <= 0) return;

        // Raycast from screen center
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, _shootLayer))
        {
            Debug.Log($"Hit: {hit.collider.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy != null)
                enemy.TakeDamage();

            Barrier barrier = hit.collider.GetComponent<Barrier>();
            if (barrier != null)
                barrier.TakeDamage();
        }

        _ammoCount--; // Reduce Ammo
        UIManager.Instance.UpdateAmmo(_ammoCount);
    }

    public void ResetAmmo()
    {
        _ammoCount = 30;
    }
}