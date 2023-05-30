using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    private int maxHealth = 100;
    [SerializeField] public int currentHealth;
    [SerializeField] private HealthBar healthBar;

    [SerializeField] private LayerMask _collectibleLayer;
    [SerializeField] private LayerMask _enemyLayer;

    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private Animator _aimAnimator;
    private Transform _weaponTransform;
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _weaponRenderer;
    private float _angle;

    private Vector2 _movement;
    private Vector2 _mousePosition;    
    private Vector2 _direction;
    private float _flip;
    private float speed = 2.5f;
    private int _healthRegen = 25;

    // Weapon Change
    public int _weaponState;
    public int totalWeapons;
    private float _fireRate = 0.5f;
    private float _nextFire = 0.0f;
    
    [SerializeField] private Camera cam;
    private Vector2 _previousDirection;

    [SerializeField] private Sprite[] defaultSprites;

    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _bulletTrailPrefab;
    private float _bulletRange = 10f;

    private bool isDead = false;
    private bool _swordState = false;
    private bool isKeyBlocked = false;
    private int reports = 0;
    [SerializeField] private Text _reportText;
    [SerializeField] private Text _hud;
    [SerializeField] private InputField _inputField;

    string[] text = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
    
    string _cipher;
    string _key;

    string _doorcipher;
    string _doorkey;
    int text_length;
    int shift;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _aimAnimator = GameObject.FindGameObjectWithTag("FirePoint").GetComponent<Animator>();
        _weaponTransform = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Transform>();
        _weaponRenderer = GameObject.FindGameObjectWithTag("Weapon").GetComponent<SpriteRenderer>();
        
        _previousDirection = new Vector2(0, -1);
        _flip = 1;

        if (_rigidBody == null)
            Debug.LogError("Player Rigidbody2D is null");

        if (_animator == null)
            Debug.LogError("Player Animator is null");

        // No weapon setting
        _weaponState = 0;
        totalWeapons = 2;

        _reportText.text = "Reports: " + reports + "/2";

        text_length = Random.Range(5, 8);
        for (int i = 0; i < text_length; i++)
            _doorcipher += text[Random.Range(0, text.Length)];

        shift = Random.Range(1, 10);
        foreach (char c in _doorcipher)
            _doorkey += text[(c + shift) % text.Length];

        _inputField.placeholder.GetComponent<Text>().text =  "Cipher : " + _doorcipher;
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.InputField>() != null)
                isKeyBlocked = true;

            else
                isKeyBlocked = false;
        }

        else
            isKeyBlocked = false;

        if (!isDead && !isKeyBlocked)
        {
            if (_weaponState == 0)
            {
                _weaponRenderer.enabled = false;
                speed = 3.5f;
            }

            // Sword
            else if (_weaponState == 1)
            {
                _weaponRenderer.enabled = true;
                _weaponRenderer.sprite = defaultSprites[_weaponState - 1];
                speed = 2.5f;
            }

            // Gun
            else if (_weaponState == 2)
            {
                _weaponRenderer.enabled = true;
                _weaponRenderer.sprite = defaultSprites[_weaponState - 1];
                speed = 2.5f;
            }

            Movement();
            Fire();

            if (Input.GetKeyDown(KeyCode.T))
                _weaponState = (_weaponState + 1) % totalWeapons;

            // Print Cipher
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (reports >= 1)
                    _hud.text = "Cipher : " + _cipher;

                else
                    _hud.text = "You need to find a report to know the cipher!";
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                if (reports == 2)
                    _hud.text = "Key : " + _key;

                else
                    _hud.text = "You need to find a report to know the key!";
            }
        }
        
        /*
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed");
            Interact();
        }
        */
    }

    private void FixedUpdate() 
    {
        if (!isDead)
        {
            // transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);
            Vector3 flip = transform.localScale;

            if (_movement.x > 0 && _flip > 0)
            {
                _flip = -1;
                _spriteRenderer.flipX = true;
                // flip.x *= -1;
                // transform.localScale = flip;
            }

            else if (_movement.x < 0 && _flip < 0)
            {
                _flip = 1;
                _spriteRenderer.flipX = false;
                // flip.x *= -1;
                // _spriteRenderer.flipX = true;
                // transform.eulerAngles = new Vector3(0, 180, 0);
                // transform.localScale = flip;
            }

            else
                transform.localScale = flip;

            _rigidBody.velocity = _movement.normalized * speed;
            // _rigidBody.rotation = _angle;
        }
    }

    private void Movement()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");

        _animator.SetFloat("Speed", _movement.sqrMagnitude);

        if (_movement.sqrMagnitude < 0.001f)
        {
            _animator.SetFloat("Horizontal", _previousDirection.x);
            _animator.SetFloat("Vertical", _previousDirection.y);
        }

        else
        {
            _animator.SetFloat("Horizontal", _movement.x);
            _animator.SetFloat("Vertical", _movement.y);
            _previousDirection = _movement;
        }

        /*
        _mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        _direction = _mousePosition - _rigidBody.position;

        // Get the angle between the player and the mouse
        _angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        */

        /*
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed");
            Interact();
        }
        */
    }

    private void OnTriggerEnter2D(Collider2D Collectible)
    {
        if (Collectible.tag == "Heart" && currentHealth < (maxHealth - _healthRegen))
        {
            currentHealth += _healthRegen;
            healthBar.SetHealth(currentHealth);
            
            Destroy(Collectible.gameObject);
            Debug.Log("Health Regenerated");
        }

        else if (Collectible.tag == "Gun")
        {
            _weaponState = 2;
            totalWeapons = 3;
            Destroy(Collectible.gameObject);
        }
    }

    /*
    private void OnTriggerStay2D(Collider2D Collectible) 
    {
        if (Collectible.tag == "Chest" && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Chest Opened");
        }
    }
    */

    /*
    private void Interact()
    {
        float interactRadius = 1f;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactRadius, _collectibleLayer);
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "Chest")
            {
                totalWeapons += 1;
                _weaponState = totalWeapons - 1;
            }
        }
    }
    */

    /*
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
    */

    private void Fire()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > _nextFire)
        {
            _nextFire = Time.time + _fireRate;
            // if (weaponState == 0)    // No weapon
            // if (weaponSate == 1)     // Sword

            if (_weaponState == 2)
            {
                _aimAnimator.SetTrigger("Attack");

                var hit = Physics2D.Raycast(_firePoint.position, _firePoint.right, _bulletRange);
                var trail = Instantiate(_bulletTrailPrefab, _firePoint.position, transform.rotation);

                var trailScript = trail.GetComponent<BulletTrail>();
                if (hit.collider != null && hit.collider.gameObject.tag == "Enemy")
                {
                    trailScript.SetTargetPosition(hit.point);
                    
                    if (hit.collider.gameObject.GetComponent<EnemyAI>().currentHealth > 10)
                        hit.collider.GetComponent<EnemyAI>().TakeDamage(10);

                    else if (hit.collider.gameObject.GetComponent<EnemyAI>().currentHealth <= 10)
                    {
                        hit.collider.GetComponent<EnemyAI>().TakeDamage(10);
                        reports += 1;
                        _reportText.text = "Reports: " + reports + "/2";

                        if (reports == 1)
                        {
                            // Generate Cipher
                            for (int i = 0; i < text_length; i++)
                                _cipher += text[Random.Range(0, text.Length)];

                            _hud.text = "Cipher : " + _cipher;
                        }

                        if (reports == 2)
                        {
                            // Generate Key
                            foreach (char c in _cipher)
                                _key += text[(c + shift) % 26];
                            _hud.text = "Key : " + _key;
                            // _hud.text = "Door Unlocked";
                        }
                    }
                    // Debug.Log("Hit " + hit.collider.name);
                }

                else
                {
                    trailScript.SetTargetPosition(_firePoint.position + _firePoint.right * _bulletRange);
                }
            }
        }
            // Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);

        // if (Input.GetKeyDown(KeyCode.Space))
            // TakeDamage(20);

        /*
        if (Input.GetMouseButtonDown(0))
        {
            GameObject _bullet = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
            _bullet.GetComponent<Rigidbody2D>().AddForce(_firePoint.up * 20f, ForceMode2D.Impulse);
        }
        */
    }

    public IEnumerator ChangeSprite ()
    {
        yield return new WaitForSeconds (1.0f);
        _spriteRenderer.enabled = false;
    }

    public IEnumerator SwordAnim (Vector3 _initalPosition, Vector3 _finalPosition)
    {
        if (_swordState)
        {
            _weaponTransform.position = _firePoint.position;
            yield return new WaitForSeconds (0.5f);
            _weaponTransform.position = _initalPosition;
            _swordState = false;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
            _animator.SetTrigger("isDead");
            StartCoroutine (ChangeSprite ());
            _weaponRenderer.enabled = false;
            // Destroy(gameObject, 1f);

            // SceneManager.LoadScene(3);
            StartCoroutine(WaitandLoad(3));
        }
    }

    public void ReadStringInput(string s)
    {
        string input = s;
        Debug.Log(s);
        if (input == _doorkey)
        {
            _hud.text = "Door Unlocked";
            GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
            foreach (GameObject door in doors)
                Destroy(door);

            // Level End
            StartCoroutine(WaitandLoad(7));
            // SceneManager.LoadScene(2);
        }

        else
            _hud.text = "Wrong Key";
    }

    IEnumerator WaitandLoad(int level)
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(level);
    }
}
