using System;
using System.Collections;
//using DefaultNamespace;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerMover : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    [SerializeField] private float _speed;
    [SerializeField] private SpriteRenderer _SpriteRenderer;
    [SerializeField] private float _jumpForce;
    // _groundChecker - коло яке поверне нам всі колайдери які є в радіусі кола 
    [SerializeField] private Transform _groundChecker;
    [SerializeField] private float _groundCheckerRadius;
    // 
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private Collider2D _headCollider;
    [SerializeField] private float _headCheckerRadius;
    [SerializeField] private Transform _headChecker;
    //[SerializeField] private bool _faceRight;
    [SerializeField] private int _maxHp;

    [Header("Animator")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _crouchAnimationKey;
    [SerializeField] private string _runAnimatorKey;
   //[SerializeField] private string _jumpAnimatorKey;
    [SerializeField] private string _hurtAnimationKey;
    [SerializeField] private string _attackAnimatorKey;
    [SerializeField] private string _castAnimatorKey;

    [Header("UI")]
    [SerializeField] private TMP_Text _coinsAmountText;
    [SerializeField] private Slider _hpBar;

    private float _horizontalDirection;
    private float _verticalDirection;
    private float _direction;
    private bool _jump;
    private bool _crawl;
    private int _coinsAmount;
    private int _currentHp;
    private float _lastHurtTime;
    private bool _atk;

    public bool CanClimb { set; get; }
    public int Coins { set; get; }

    public int CoinsAmount
    {
        get => _coinsAmount;
        set
        {
            _coinsAmount = value;
            _coinsAmountText.text = value.ToString();
        }
    }

    private int CurrentHp
    {
        get => _currentHp;
        set
        {
            _currentHp = value;
            _hpBar.value = _currentHp;
        }
    }

    // Start is called before the first frame update
    void Start()
    {      
        _hpBar.maxValue = _maxHp;
        CurrentHp = _maxHp;
        CoinsAmount = 0;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.RightArrow))
            Debug.Log(message: "Right");
        if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.LeftArrow))
            Debug.Log(message: "Left");
        */
        //_direction = Input.GetAxisRaw("Horizontal"); //-1(A, <-) 1(D,->) //gamepad (<-\->)
        _horizontalDirection = Input.GetAxisRaw("Horizontal");
        _verticalDirection = Input.GetAxisRaw("Vertical");
        //_animator.SetFloat(_runAnimatorKey, Mathf.Abs(_horizontalDirection));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jump = true;
        }
        //_rigidbody.velocity = Vector2.right * direction * _speed; // x = 1,  y =0
        //if (_direction > 0 && _SpriteRenderer.flipX == true)// можно без == true
        if (_horizontalDirection > 0 && _SpriteRenderer.flipX){
            _SpriteRenderer.flipX = false;
        }
        else if (_horizontalDirection < 0 && !_SpriteRenderer.flipX)
        {
            _SpriteRenderer.flipX = true;
        }

        _crawl = Input.GetKey(KeyCode.C);
        _atk = Input.GetKey(KeyCode.X);

    }

 
    private void FixedUpdate()
    {
        _rigidbody.velocity = new Vector2(x: _direction * _speed, y: _rigidbody.velocity.y); // x = 1,  y =0

        /*
        if (_animator.GetBool(_hurtAnimationKey))
        {
            if (canJump && Time.time - _lastHurtTime > 0.2f)
            {

                _animator.SetBool(_hurtAnimationKey, false);
            }

            _needToAttack = false;
            _needToCast = false;
            return;
        }
        */

        //Climb 
        if (CanClimb)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _verticalDirection * _speed);
            _rigidbody.gravityScale = 0;
        }
        else{
            _rigidbody.gravityScale = 1;
        }
        
        bool canJump = Physics2D.OverlapCircle(_groundChecker.position, _groundCheckerRadius, _whatIsGround);
        bool canStand = !Physics2D.OverlapBox(_headChecker.position, _headChecker.position, _headCheckerRadius, _whatIsGround);
       // _headCollider.enabled = !_crawl && canStand;
        _headCollider.enabled = !_crawl;

        _animator.SetBool(_crouchAnimationKey, !_headCollider.enabled);
        _animator.SetInteger(_runAnimatorKey, (int)_horizontalDirection);
        _animator.SetBool(_attackAnimatorKey, _atk);
        

        if (_jump && canJump)
        {
            _rigidbody.AddForce(Vector2.up * _jumpForce);
            _jump = false;
        }

        if (!_headCollider.enabled)
        {
            return;
        }

        _rigidbody.velocity = new Vector2(_horizontalDirection * _speed, _rigidbody.velocity.y);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_groundChecker.position, _groundCheckerRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_headChecker.position, _headCheckerRadius);
    }

    public void AddHp(int hpPoints)
    {
        int missingHp = _maxHp - CurrentHp;
        int pointToAdd = missingHp > hpPoints ? hpPoints : missingHp;
        StartCoroutine(RestoreHp(pointToAdd));
    }

    private IEnumerator RestoreHp(int pointToAdd)
    {
        while (pointToAdd != 0)
        {
            pointToAdd--;
            CurrentHp++;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void TakeDamage(int damage, float pushPower = 0, float posX = 0)
    {
        if (_animator.GetBool(_hurtAnimationKey))
        {
            return;
        }

        Debug.Log(damage);
        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            Debug.Log("Died");
            gameObject.SetActive(false);
            Invoke(nameof(ReloadScene), 1f);
        }

        
        if (pushPower != 0 && Time.time - _lastHurtTime > 0.5f)
        {
            _lastHurtTime = Time.time;
            int direction = posX > transform.position.x ? -1 : 1;
            _rigidbody.AddForce(new Vector2(direction * pushPower / 4, pushPower));
            _animator.SetBool(_hurtAnimationKey, true);
        }
        
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
