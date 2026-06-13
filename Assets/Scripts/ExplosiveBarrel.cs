using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [SerializeField] private Collider explosionCollider; // Big box collider
    [SerializeField] private GameObject explosionVFXPrefab; // Explosion VFX
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private float _chainExplosionRadius = 5f; // Radius for chain reaction
    [SerializeField] private float _vfxDuration = 3f; // Duration of explosion VFX

    private bool _hasExploded = false; // To prevent multiple explosions

    public void Explode()
    {
        if (_hasExploded) return; // To prevent multiple explosions
        _hasExploded = true;

        // Instantiate explosion VFX at barrel position
        if (explosionVFXPrefab != null)
        {
            GameObject vfx = Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, _vfxDuration); // Destroy VFX after duration
        }

        // Play explosion sound
        AudioManager.instance.PlayExplosionVFX(AudioManager.instance.explosionVFX);

        // Damage all enemies in explosion area
        Collider[] hits = Physics.OverlapBox(explosionCollider.bounds.center, explosionCollider.bounds.extents, Quaternion.identity, enemyLayer);

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<EnemyAI>(out var enemy))
            {
                enemy.TakeDamage();
            }
        }

        // Chain reaction: Explode nearby barrels
        Collider[] nearbyBarrels = Physics.OverlapSphere(transform.position, _chainExplosionRadius);
        foreach (Collider barrel in nearbyBarrels)
        {
            if (barrel.TryGetComponent<ExplosiveBarrel>(out var explosiveBarrel) && explosiveBarrel != this)
            {
                explosiveBarrel.Explode();
            }
        }

        // Destroy barrel
        gameObject.SetActive(false);
        Destroy(gameObject, _vfxDuration + 0.3f); // Wait for VFX to finish before destroying barrel
    }
}