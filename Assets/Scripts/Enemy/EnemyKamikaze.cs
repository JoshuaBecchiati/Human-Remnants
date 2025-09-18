using UnityEngine;

public class EnemyKamikaze : Enemy, IDamageable, IDamageDealer
{
    [Header("Attack")]
    [SerializeField] private LayerMask _damageableLayers;
    public float Damage => _damage;


    // Update is called once per frame
    void Update()
    {
        if (!_isPlayerInSight) _newPoint = transform.position;
        Movement();
        Explode();
    }

    private void Explode()
    {
        // Controllo se il giocatore è abbastanza vicino
        if (Vector3.Distance(transform.position, Player.position) > _attackRange)
            return;

        // Recupero tutti i collider nella sfera che appartengono ai layer target
        Collider[] colliders = Physics.OverlapSphere(transform.position, _attackRange, _damageableLayers);

        foreach (var col in colliders)
        {
            if (col.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(Damage);
            }
        }
        Destroy(gameObject);
    }
}