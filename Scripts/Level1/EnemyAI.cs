using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _enemy;
    [SerializeField] private LayerMask _playerLayer;
    private Animator _animator;

    private int maxHealth = 100;
    [SerializeField] public int currentHealth;
    [SerializeField] private HealthBar healthBar;

    private float _speed = 100f;
    private float _nextWayPointDistance = 3f;

    private Path _path;
    private int _currentWayPoint = 0;
    // private bool _reachedEndOfPath = false;
    private float _moveRange = 5f;
    private float _attackRange = 0.5f;

    private Seeker _seeker;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _initialPosition;
    private Vector2 _targetPosition;

    private bool playerDead = false;

    // Start is called before the first frame update
    void Start()
    {
        _seeker = GetComponent<Seeker>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = _enemy.GetComponent<Animator>();

        _initialPosition = new Vector2(transform.position.x, transform.position.y);
        _targetPosition = new Vector2(_target.position.x, _target.position.y);
        
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    private void UpdatePath()
    {
        if (!playerDead)
            _targetPosition = new Vector2(_target.position.x, _target.position.y);

        Vector2 distance = new Vector2(_targetPosition.x - _initialPosition.x, _targetPosition.y - _initialPosition.y);
        if (distance.magnitude < _moveRange)
        {
            // _animator.SetBool("IsMoving", true);
            if (_seeker.IsDone())
                _seeker.StartPath(_rigidbody2D.position, _targetPosition, OnPathComplete);
        }

        else
        {
            // _animator.SetBool("IsMoving", false);
            _seeker.StartPath(_rigidbody2D.position, _initialPosition, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWayPoint = 0;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
            GameObject.Destroy(this.gameObject);
    }

    private void Update() 
    {
    }

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, _attackRange, _playerLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.gameObject.tag == "Player")
            {
                if (enemy.gameObject.GetComponent<Player>().currentHealth > 0)
                {
                    _animator.SetTrigger("Attack");
                    enemy.GetComponent<Player>().TakeDamage(1);
                }

                else
                {
                    playerDead = true;
                    _targetPosition = new Vector2(_initialPosition.x, _initialPosition.y);
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Path following
        if (_path == null)
            return;

        if (_currentWayPoint >= _path.vectorPath.Count)
        {
            // Debug.Log("Reached end of path");
            Attack();
            // _reachedEndOfPath = true;
            return;
        }

        // else
            // _reachedEndOfPath = false;

        Vector2 direction = ((Vector2)_path.vectorPath[_currentWayPoint] - _rigidbody2D.position).normalized;
        Vector2 force = direction * _speed * Time.deltaTime;
        _rigidbody2D.velocity = direction * _speed * Time.deltaTime;

        float distance = Vector2.Distance(_rigidbody2D.position, _path.vectorPath[_currentWayPoint]);

        if (distance < _nextWayPointDistance)
            _currentWayPoint++;

        // Fliiping the sprites
        if (force.x >= 0.01f && _enemy.localScale.x > 0)
            _enemy.localScale = new Vector3(-_enemy.localScale.x, _enemy.localScale.y, _enemy.localScale.z);

        else if (force.x <= -0.01f && _enemy.localScale.x < 0)
            _enemy.localScale = new Vector3(-_enemy.localScale.x, _enemy.localScale.y, _enemy.localScale.z);
    }
}
