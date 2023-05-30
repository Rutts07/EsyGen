using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private Transform _hero;
    private Animator            m_animator;
    // private Rigidbody2D         m_body2d;
    private Sensor_Bandit       m_groundSensor;
    private bool                m_grounded = false;
    public bool               m_isDead = false;
    private bool              player_Dead = false;

    private int maxHealth = 100;
    public int currentHealth;
    private bool _flip = false;

    [SerializeField] private Transform _attackPoint;
    [SerializeField] private LayerMask _playerLayers;
    [SerializeField] private HealthBar_Boss healthBar;
    private float _attackRange = 1.0f;
    public int _attackDamage = 15;

    private float _fireRate = 0.75f;
    private float _nextFire = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        // m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
        currentHealth = maxHealth;

        m_animator.SetInteger("AnimState", 2);
    }

    // Update is called once per frame
    void Update()
    {
        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State()) {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if(m_grounded && !m_groundSensor.State()) {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        if (Time.time > _nextFire && !m_isDead && !player_Dead)
        {   
            _nextFire = Time.time + _fireRate;
            Attack();
        }

        //Set AirSpeed in animator
        // m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);
    }

    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > _hero.position.x && _flip)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            _flip = false;
        }

        else if (transform.position.x < _hero.position.x && !_flip)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            _flip = true;
        }
    }

    public void Attack()
    {
        if (Vector2.Distance(transform.position, _hero.position) < _attackRange)
        {
            m_animator.SetTrigger("Attack");
            if (_hero.GetComponent<Hero>().currentHealth > _attackDamage)
                _hero.GetComponent<Hero>().TakeDamage(_attackDamage);

            else
            {
                _hero.GetComponent<Hero>().TakeDamage(_attackDamage);
                m_animator.SetInteger("AnimState", 1);
                player_Dead = true;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        m_animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            m_animator.SetTrigger("Death");
            m_isDead = true;
        }
    }

    /*
    void Die()
    {
        // Debug.Log("Boss died");
        // Destroy(gameObject);
    }
    */
}
