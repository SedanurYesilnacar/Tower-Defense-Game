using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Enemy : MonoBehaviour
{
    private int _target;
    [SerializeField]
    private Transform _exitPoint;
    [SerializeField]
    private List<Transform> _wayPoints;
    [SerializeField]
    private float _navigationUpdate;
    [SerializeField]
    private float _healthPoints;
    [SerializeField]
    private int rewardAmount;

    private Collider2D _enemyCollider;
    private Transform _enemyTransform;
    private float _navigationTime;
    private bool _isDead = false;
    private Animator _anim;


    public bool IsDead
    {
        get
        {
            return _isDead;
        }
    }
    

    private void Start()
    {
        GameManager.Instance.RegisterEnemy(this);
        _wayPoints = new List<Transform>();
        _enemyTransform = GetComponent<Transform>();
        _enemyCollider = GetComponent<Collider2D>();
        _anim = GetComponent<Animator>();
        foreach(GameObject gameObject in GameObject.FindGameObjectsWithTag("Checkpoint"))
        {
            _wayPoints.Add(gameObject.transform);
        }
        _wayPoints.Add(GameObject.FindGameObjectWithTag("Finish").transform);
    }

    private void Update()
    {
        if(_wayPoints != null && !_isDead)
        {
            _navigationTime += Time.deltaTime;
            if(_navigationTime > _navigationUpdate)
            {
                if(_target < _wayPoints.Count)
                {
                    _enemyTransform.position = Vector2.MoveTowards(_enemyTransform.position, _wayPoints[_target].position, _navigationTime);
                } else
                {
                    _enemyTransform.position = Vector2.MoveTowards(_enemyTransform.position, _exitPoint.position, _navigationTime);
                }
                _navigationTime = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Checkpoint"))
        {
            _target++;
        }
        else if(collision.CompareTag("Finish"))
        {
            GameManager.Instance.RoundEscaped += 1;
            GameManager.Instance.TotalEscaped += 1;
            GameManager.Instance.UnregisterEnemy(this);
            GameManager.Instance.isWaveOver();
        }
        else if(collision.CompareTag("Projectile"))
        {
            Projectile projectile = new Projectile();
            projectile = collision.GetComponent<Projectile>();
            int damage = projectile.AttackStrength;
            EnemyHit(damage);
            Destroy(collision.gameObject);
        }
    }

    private void EnemyHit(int damage)
    {
        if(_healthPoints > 0)
        {
            _healthPoints -= damage;
            _anim.Play("Hurt1");
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Hit);
            if(_healthPoints <= 0)
            {
                //die
                _anim.SetTrigger("didDie");
                Die();
            }
        }
    }


    private void Die()
    {
        GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Death);
        GameManager.Instance.TotalKilled += 1;
        GameManager.Instance.AddMoney(rewardAmount);
        GameManager.Instance.isWaveOver();
        _isDead = true;
        _enemyCollider.enabled = false;
    }
}
