using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

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
        _shootLayer = LayerMask.GetMask("Enemy", "Barrier","Column");

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (!GameManager.Instance.IsGameActive()) return;
        if (_ammoCount <= 0) return;

        // Play weapon fire ONCE per shot
        AudioManager.instance.PlayWeaponFire(AudioManager.instance.weaponFire);

        // Raycast from screen center
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, _shootLayer))
        {
            Debug.Log($"Hit: {hit.collider.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

            // Check enemy
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage();
                return;
            }

            // Check barrier
            Barrier barrier = hit.collider.GetComponent<Barrier>();
            if (barrier != null)
            {
                barrier.TakeDamage();
                AudioManager.instance.PlayBarrierHit(AudioManager.instance.barrierHit);
                return;
            }

            // Anything else on Barrier layer/tag plays barrier hit sound
            if (hit.collider.CompareTag("Barrier"))
            {
                AudioManager.instance.PlayBarrierHit(AudioManager.instance.barrierHit);
            }
        }

        _ammoCount--;
        UIManager.Instance.UpdateAmmo(_ammoCount);
    }

    public void ResetAmmo()
    {
        _ammoCount = 30;
    }
}