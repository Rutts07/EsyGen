using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Hero : MonoBehaviour {

    float      m_speed = 5.0f;
    float      m_jumpForce = 15f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_Bandit       m_groundSensor;
    private SpriteRenderer      _spriteRenderer;
    private bool                m_grounded = false;
    private bool                m_combatIdle = false;
    private float               _flip;

    [SerializeField] private Transform _attackPoint;
    [SerializeField] private LayerMask _enemyLayers;
    [SerializeField] private LayerMask _wallLayers;
    [SerializeField] private HealthBar_Boss healthBar;
    private float _attackRange = 1f;

    private int maxHealth = 100;
    public int currentHealth;
    private bool isDead = false;

    private float _fireRate = 0.5f;
    private float _nextFire = 0.0f;

    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();

        currentHealth = maxHealth;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (!isDead)
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

            // -- Handle input and movement --
            float inputX = Input.GetAxis("Horizontal");

            // Swap direction of sprite depending on walk direction
            if (inputX < 0)
                transform.localScale = new Vector3(-1.25f, 1.25f, 1.0f);

            else if (inputX > 0)
                transform.localScale = new Vector3(1.25f, 1.25f, 1.0f);

            /*
            if (inputX > 0)
                _spriteRenderer.flipX = true;

            else if (inputX < 0)
                _spriteRenderer.flipX = false;
            */

            // Move
            // Disable Latching
            // Collider2D isLatchedEnemy = Physics2D.OverlapCircle(transform.position, _attackRange, _enemyLayers);
            Collider2D isLatchedWalls = Physics2D.OverlapCircle(transform.position, _attackRange, _wallLayers);
        
            if (isLatchedWalls == null)
                m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

            else
            {
                if (inputX < 0)
                {
                    // if (isLatchedEnemy != null && isLatchedEnemy.transform.position.x - transform.position.x > 0)
                        // m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

                    if (isLatchedWalls != null && isLatchedWalls.transform.position.x - transform.position.x > 0)
                        m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

                    else
                        m_body2d.velocity = new Vector2(0, m_body2d.velocity.y);
                }

                else if (inputX > 0)
                {
                    // if (isLatchedEnemy != null && isLatchedEnemy.transform.position.x - transform.position.x < 0)
                        // m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

                    if (isLatchedWalls != null && isLatchedWalls.transform.position.x - transform.position.x < 0)
                        m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

                    else
                        m_body2d.velocity = new Vector2(0, m_body2d.velocity.y);
                }

                else
                    m_body2d.velocity = new Vector2(0, m_body2d.velocity.y);
            }

            //Set AirSpeed in animator
            m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

        
            // -- Handle Animations --
            /*
            //Death
            if (Input.GetKeyDown("e")) {
                if(!m_isDead)
                    m_animator.SetTrigger("Death");
                else
                    m_animator.SetTrigger("Recover");

                m_isDead = !m_isDead;
            }
            
            //Hurt
            else if (Input.GetKeyDown("q"))
                m_animator.SetTrigger("Hurt");
            */

            //Attack
            if(Input.GetMouseButtonDown(0) && Time.time > _nextFire) 
            {
                _nextFire = Time.time + _fireRate;
                Attack();
            }

            //Change between idle and combat idle
            /*
            else if (Input.GetKeyDown("f"))
                m_combatIdle = !m_combatIdle;
            */

            //Jump
            else if (Input.GetKeyDown("space") && m_grounded) {
                m_animator.SetTrigger("Jump");
                m_grounded = false;
                m_animator.SetBool("Grounded", m_grounded);
                m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
                m_groundSensor.Disable(0.2f);
            }

            //Run
            else if (Mathf.Abs(inputX) > Mathf.Epsilon)
                m_animator.SetInteger("AnimState", 2);

            //Combat Idle
            else if (m_combatIdle)
                m_animator.SetInteger("AnimState", 1);

            //Idle
            else
                m_animator.SetInteger("AnimState", 0);
        }
    }

    private void Attack()
    {
        m_animator.SetTrigger("Attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange, _enemyLayers);
        foreach(Collider2D enemy in hitEnemies)
        {
            if (enemy.GetComponent<Boss>().currentHealth > 10)
                enemy.GetComponent<Boss>().TakeDamage(10);

            else
            {
                enemy.GetComponent<Boss>().TakeDamage(enemy.GetComponent<Boss>().currentHealth);
                StartCoroutine(WaitandLoad(4));
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            m_animator.SetTrigger("Death");
            isDead = true;

            // End Game
            StartCoroutine(WaitandLoad(3));
            // SceneManager.LoadScene(3);
        }

        /*
        if (currentHealth <= 0)
            Die();
        */
    }

    void OnDrawGizmosSelected()
    {
        if (_attackPoint == null)
            return;

        Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
    } 

    private IEnumerator WaitandLoad(int level)
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(level);
    }
}
