using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 5f;

    private IDamageDealer _owner;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    public void Init(IDamageDealer owner)
    {
        _owner = owner;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (_owner == null) return;

        if (col.TryGetComponent<IDamageable>(out var h))
        {
            // Friendly fire check
            if (_owner is MonoBehaviour ownerMb && ownerMb.CompareTag(col.tag))
                return;

            h.TakeDamage(_owner.Damage);
        }

        Destroy(gameObject);
    }
}
