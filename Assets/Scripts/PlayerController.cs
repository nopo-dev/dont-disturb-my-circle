using UnityEngine;
using System.Collections;
using BulletFury;

// control player input, movement, and firing
// manage player stats such as hp, damage, fire rate, and spread
public class PlayerController : MonoBehaviour
{
    // serialized script components
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private GameUIHandler _gameUIHandler;
    [SerializeField] private AudioManager _audioManager;

    // script for firing bullets from the player
    [SerializeField] private PlayerBasicBulletFire _playerFire;

    // special ability gameobject
    [SerializeField] private GameObject _special;

    private Animator _animator;

    // current player hp
    private int _currentHP;
    public int CurrentHP
    {
        get { return _currentHP; }
        set
        {
            _currentHP = value;
            _gameStateManager.PlayerHPUpdate();
            _gameUIHandler.SetPlayerCurrentHP(_currentHP);
            _gameUIHandler.SetLifeShop(_currentHP);
        }
    }

    // current player max hp, currently always is 5
    private int _currentMaxHP;
    public int CurrentMaxHP
    {
        get { return _currentMaxHP; }
        set
        {
            _currentMaxHP = value;
            _gameUIHandler.SetPlayerMaxHP(_currentMaxHP);
        }
    }

    // current number of specials the player has
    private int _currentSpecials;
    public int CurrentSpecials
    {
        get { return _currentSpecials; }
        set
        {
            _currentSpecials = value;
            _gameUIHandler.SetPlayerCurrentSpecials(_currentSpecials);
            _gameUIHandler.SetSpecialShop(_currentSpecials);
        }
    }

    // current player max number of specials, currently always is 5
    private int _currentMaxSpecials;
    public int CurrentMaxSpecials
    {
        get { return _currentMaxSpecials; }
        set
        {
            _currentMaxSpecials = value;
            _gameUIHandler.SetPlayerMaxSpecials(_currentMaxSpecials);
        }
    }

    // determined by mouse pointer location, player will fire in this direction
    private Vector2 _lookDirection;
    public Vector2 LookDirection
    {
        get { return _lookDirection; }
        set { _lookDirection = value; }
    }
    
    // modifies the speed of the idle animation, used to change the animation to match
    // the players fire rate
    private float _animationSpeed;
    public float AnimationSpeed
    {
        get { return _animationSpeed; }
        set
        {
            _animationSpeed = value;
            _animator.SetFloat("Speed", _animationSpeed);
        }
    }

    // determines how much damage each bullet deals
    private int _damageLevel;
    public int DamageLevel
    {
        get { return _damageLevel; }
        set
        {
            _damageLevel = value;
            _playerFire.Damage = StatsHelper.damageLevels[_damageLevel];
            _gameUIHandler.SetShopDamage(_damageLevel);
        }
    }

    // determines how fast the player fires bullets
    private int _fireRateLevel;
    public int FireRateLevel
    {
        get { return _fireRateLevel; }
        set
        {
            _fireRateLevel = value;
            _playerFire.FireRate = StatsHelper.fireRateLevels[_fireRateLevel];
            _gameUIHandler.SetShopSpeed(_fireRateLevel);
        }
    }
    
    // determines how many bullets are fired at once, also affects the spread of fire
    private int _numSidesLevel;
    public int NumSidesLevel
    {
        get { return _numSidesLevel; }
        set
        {
            _numSidesLevel = value;
            _playerFire.NumSides = StatsHelper.numSidesLevels[_numSidesLevel];
            ArcLevel = _numSidesLevel;
            _gameUIHandler.SetShopSpread(_numSidesLevel);
        }
    }

    // determines the spread of bullet fire
    private int _arcLevel;
    public int ArcLevel
    {
        get { return _arcLevel; }
        set
        {
            _arcLevel = value;
            _playerFire.Arc = StatsHelper.arcLevels[_arcLevel];
        }
    }

    // invulnerable state for the player (ie if they are damaged)
    private bool _invulnerable;
    public bool Invulnerable
    {
        get { return _invulnerable; }
        set { _invulnerable = value;}
    }

    // focused state, slows movement speed and lessens the spread of bullet fire
    private bool _focused;
    public bool Focused
    {
        get { return _focused; }
        set { _focused = value; }
    }

    // auto fire toggle
    private bool _toggleFire;
    // is the player trying to fire bullets
    private bool _isFiring;
    // is the player currently in the using special animation
    private bool _isUsingSpecial;

    // use the special ability
    // the bool useSpecial determines if a special charge is consumed
    // it must also be passed to the ClearScreen coroutine
    // in order to determine the visual effect that is created
    public void UseSpecial(bool useSpecial)
    {
        // if a special is being used, consume a charge and play the sfx
        if (useSpecial)
        {
            CurrentSpecials--;
            _audioManager.PlaySound("Bomb");
        }
        // ensure the player cannot repeatedly use specials
        // as there is no reason to do so
        _isUsingSpecial = true;
        // enable the SpriteRenderer component for the special gameobject in scene
        _special.GetComponent<SpriteRenderer>().enabled = true;
        StartCoroutine(ClearScreen(useSpecial));
    }

    // used when the player either gets hit or uses the special ability
    // as it is essentially the same effect
    IEnumerator ClearScreen(bool useSpecial)
    {
        // cache important variables to lessen the amount of GetComponent calls

        // collides with bullets
        BulletCollider bulletCollider = _special.GetComponent<BulletCollider>();
        // collides with enemies
        CircleCollider2D circleCollider = _special.GetComponent<CircleCollider2D>();
        // sprite of the special
        SpriteRenderer spriteRenderer = _special.GetComponent<SpriteRenderer>();
        // move the gameobject to the location at which the special was used/player was hit
        _special.transform.position = transform.position;
        // enable both collision with bullets and collision with enemies
        bulletCollider.enabled = true;
        circleCollider.enabled = true;

        // if actually using a special, make the visual effect more apparent than being hit
        float alpha = useSpecial ? 1f : 0.5f;

        // while the radius of the effect is lower than the diagonal distance across the stage
        // to ensure that every enemy and bullet will be hit, grow the gameobject transform
        // and also apply a lerp to fade the alpha to 0 to make a dissipating effect
        float radius = 1;
        while (radius <= 23f)
        {
            radius += Time.deltaTime * 25f;
            bulletCollider.Radius = radius;
            _special.transform.localScale = Vector3.one * radius;
            if (spriteRenderer.enabled == true)
            {
                Color c = spriteRenderer.color;
                c.a = Mathf.Lerp(alpha, 0f, radius / 23f);
                spriteRenderer.color = c;
            }
            yield return null;
        }

        // reset variables so that the gameobject can be reused the next time it is needed
        bulletCollider.Radius = 1;
        _special.transform.localScale = Vector3.one;
        bulletCollider.enabled = false;
        circleCollider.enabled = false;
        if (spriteRenderer.enabled == true)
            spriteRenderer.enabled = false;
        
        // allow the player to use the special again
        _isUsingSpecial = false;
    }


    // initialize stats and animator component (has to be done in start rather than awake
    // in order to preserve execution order and prevent null exceptions)
    private void Start()
    {
        _animator = gameObject.GetComponent<Animator>();

        // set private variables
        _currentMaxHP = StatsHelper.startingMaxHP;
        _currentHP = StatsHelper.startingHP;
        _currentMaxSpecials = StatsHelper.startingMaxSpecials;
        _currentSpecials = StatsHelper.startingSpecials;

        // set properties, which also initializes the UI
        CurrentMaxHP = _currentMaxHP;
        CurrentHP = _currentHP;
        CurrentMaxSpecials = _currentMaxSpecials;
        CurrentSpecials = _currentSpecials;
        DamageLevel = 0;
        FireRateLevel = 0;
        NumSidesLevel = 0;
        ArcLevel = 0;
    }

    // update loop, responsible for input related to the player
    private void Update()
    {
        if (CanAct())
        {
            // movement input
            Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            // focus input
            _focused = Input.GetKey("left shift");

            // aim input
            Vector2 aimInput = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _lookDirection = (aimInput - (Vector2)transform.position).normalized;

            // fire input
            _isFiring = Input.GetMouseButton(0);
            if (Input.GetMouseButtonDown(1))
                _toggleFire = !_toggleFire;

            // special input
            if (!_isUsingSpecial && Input.GetKeyDown("space") && CurrentSpecials >= 1)
            {
                UseSpecial(true);
            }

            // movement and movement animation setting
            if (movementInput.magnitude > 0)
            {
                Vector3 movement = new Vector3(movementInput.x, movementInput.y).normalized;
                if (_focused)
                    transform.position += StatsHelper.focusMovementSpeed * Time.deltaTime * movement;
                else
                    transform.position += StatsHelper.movementSpeed * Time.deltaTime * movement;
            }
            if (aimInput.x - transform.position.x > 0)
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            else if (aimInput.x - transform.position.x < 0)
                gameObject.GetComponent<SpriteRenderer>().flipX = false;

            // clamp player position to the stage
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -6f, 6f),
                Mathf.Clamp(transform.position.y, -9.5f, 9.685f));
        }
    }

    // take damage upon touching an enemy (all of the current enemies are slimes)
    // will rework upon adding more enemies to the game
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Contains("Slime"))
        {
            TakeDamage();
        }
    }

    // unused functions for now
    /*public void IncreaseHearts()
    {
        if (CurrentHP < CurrentMaxHP)
            CurrentHP++;
    }

    public void IncreaseMaxHearts()
    {
        if (CurrentMaxHP < StatsHelper.maxHP)
        {
            CurrentMaxHP++;
            CurrentHP++;
        }
    }

    public void IncreaseSpecials()
    {
        if (CurrentSpecials < CurrentMaxSpecials)
            CurrentSpecials++;
    }

    public void IncreaseMaxSpecials()
    {
        if (CurrentMaxSpecials < StatsHelper.maxSpecials)
        {
            CurrentMaxSpecials++;
            CurrentSpecials++;
        }
    }*/

    // take damage (fixed at 1 hp per damaging instance)
    public void TakeDamage()
    {
        // check if player is currently invulnerable
        if (!Invulnerable)
        {
            // perform the lesser special effect without consuming a charge
            UseSpecial(false);
            _audioManager.PlaySound("PlayerDamage");

            // if the player will not die from the damage, reset their combo count
            // and turn them invulnerable
            if (CurrentHP >= 1)
            {
                CurrentHP--;
                _gameStateManager.ResetCombo();
                StartCoroutine(TurnInvulnerable());
            }
        }
    }

    // turn the player invulnerable for a time, to prevent additional damage
    // only seems to be useful in boss fights, as normal enemies and all bullets
    // are destroyed upon colliding with the special gameobject
    IEnumerator TurnInvulnerable()
    {
        Invulnerable = true;
        float time = 0;
        Color spriteColor = gameObject.GetComponent<SpriteRenderer>().color;

        // while invulnerable, apply a flickering effect to the sprite to signify invulnerability
        while (time <= StatsHelper.invulnerableTime)
        {
            spriteColor.a = 0.5f;
            gameObject.GetComponent<SpriteRenderer>().color = spriteColor;
            yield return new WaitForSeconds(0.1f);
            time += 0.1f;
            spriteColor.a = 0.75f;
            gameObject.GetComponent<SpriteRenderer>().color = spriteColor;
            yield return new WaitForSeconds(0.1f);
            time += 0.1f;
        }

        // reset variables, and allow the player to be damaged again
        spriteColor.a = 1f;
        gameObject.GetComponent<SpriteRenderer>().color = spriteColor;
        Invulnerable = false;
    }

    // used to determine whether the player should be able to act
    // during this stage of the game
    public bool CanAct()
    {
        return (_gameStateManager.CanPlayerAct());
    }

    // used to determine whether the player is allowed to fire bullets
    // the player can only fire while inside the magic circle on the ground
    public bool CanFire()
    {
        return  (_gameStateManager.CanPlayerFire());
    }

    // used to determine whether the player intends to fire bullets
    public bool IsFiring()
    {
        return _toggleFire || _isFiring;
    }
}
