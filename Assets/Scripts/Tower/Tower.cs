using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField]
    private float _timeBetweenAttacks;
    [SerializeField]
    private float _attackRadius;
    [SerializeField]
    private Projectile _projectile;
    private Enemy _targetEnemy = null;
    private float _attackCounter;
    private bool _isAttacking = false;

    private List<Projectile> _projectiles = new List<Projectile>();

    private void Update()
    {
        _attackCounter -= Time.deltaTime;
        if(_targetEnemy == null || _targetEnemy.IsDead)
        {
            Enemy nearestEnemy = GetNearestEnemyInRange();
            if(nearestEnemy != null && Vector2.Distance(nearestEnemy.transform.localPosition, transform.localPosition) <= _attackRadius)
            {
                _targetEnemy = nearestEnemy;
            }
        } else
        {
            if(_attackCounter <= 0)
            {
                _isAttacking = true;
                Attack();
                _attackCounter = _timeBetweenAttacks;
            } else
            {
                _isAttacking = false;
            }
            if (Vector2.Distance(transform.localPosition, _targetEnemy.transform.localPosition) > _attackRadius)
            {
                _targetEnemy = null;
            }
        }

        
    }


    private void Attack()
    {
        Projectile newProjectile = Instantiate(_projectile) as Projectile;
        newProjectile.transform.localPosition = transform.localPosition;
        RegisterProjectile(newProjectile);

        if(_targetEnemy == null)
        {
            UnregisterProjectile(newProjectile);
        } else
        {
            // Move projectile to enemy
            StartCoroutine(MoveProjectile(newProjectile));
        }
        PlayProjectileAudio(newProjectile);
    }

    private void PlayProjectileAudio(Projectile projectile) 
    {
        if(projectile.ProjectileType == proType.arrow)
        {
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Arrow);
        }
        else if(projectile.ProjectileType == proType.fireball)
        {
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Fireball);
        }
        else if(projectile.ProjectileType == proType.rock)
        {
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Rock);
        }
    }

    IEnumerator MoveProjectile(Projectile projectile)
    {
        while(GetTargetDistance(_targetEnemy) > 0.2f && projectile != null && _targetEnemy != null)
        {
            var dir = _targetEnemy.transform.localPosition - transform.localPosition;
            var angleDirection = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);
            projectile.transform.localPosition = Vector2.MoveTowards(projectile.transform.localPosition, _targetEnemy.transform.localPosition, 5f * Time.deltaTime);
            yield return null;
        }
        if(projectile != null || _targetEnemy == null)
        {
            Destroy(projectile.gameObject);
            yield return null;
        }
    }

    private float GetTargetDistance(Enemy thisEnemy)
    {
        if(thisEnemy == null)
        {
            thisEnemy = GetNearestEnemyInRange();
            if(thisEnemy == null)
            {
                return 0f;
            }
        }
        return Mathf.Abs(Vector2.Distance(transform.localPosition, thisEnemy.transform.localPosition));
    }

    private List<Enemy> GetEnemiesInRange()
    {
        List<Enemy> _enemiesInRange = new List<Enemy>();
        foreach(Enemy enemy in GameManager.Instance._enemyList)
        {
            if (Vector2.Distance(enemy.transform.localPosition, transform.localPosition) <= _attackRadius)
            {
                _enemiesInRange.Add(enemy);
            }
        }
        return _enemiesInRange;
    }

    private Enemy GetNearestEnemyInRange()
    {
        Enemy nearestEnemy = null;
        float smallestDistance = float.PositiveInfinity;
        foreach(Enemy enemy in GetEnemiesInRange())
        {
            if(Vector2.Distance(this.transform.localPosition, enemy.transform.localPosition) < smallestDistance)
            {
                smallestDistance = Vector2.Distance(this.transform.localPosition, enemy.transform.localPosition);
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }

    private void RegisterProjectile(Projectile projectile)
    {
        _projectiles.Add(projectile);
    }

    private void UnregisterProjectile(Projectile projectile)
    {
        Destroy(projectile);
        _projectiles.Remove(projectile);
    }

    public void DestroyAllProjectiles()
    {
        foreach(Projectile projectile in _projectiles)
        {
            Destroy(projectile.gameObject);
        }
        _projectiles.Clear();
    }

}
