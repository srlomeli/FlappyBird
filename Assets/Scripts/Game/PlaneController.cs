using System;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    private Rigidbody2D rb2D;

    [SerializeField] private float gravityScale = 5f;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float maxVeloctiyRoation = 75;
    [SerializeField] private float rotationMultiplier = 0;
    [SerializeField] private float maxVeloctiyY = 10;

    [SerializeField] private float deathForce = 2;

    private bool isDead;
    private bool isOnFloor;

    public static Action OnGameOver;

    private void OnEnable()
    {
        GameManager.OnGameStart += OnStart;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= OnStart;
    }

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        if (!isDead)
        {
            rb2D.linearVelocityX = movementSpeed;
            bool jumpKeyPressed = Input.GetKeyDown(KeyCode.Space) | Input.GetMouseButtonDown(0);
            rb2D.linearVelocityY = jumpKeyPressed ? jumpForce : rb2D.linearVelocityY;
            rb2D.linearVelocityY = Mathf.Clamp(rb2D.linearVelocityY,-maxVeloctiyY, jumpForce);

            if(jumpKeyPressed)
            AudioManager.instance.jumpAudioSource.Play();

        }

        transform.eulerAngles = new Vector3(0, 0, Mathf.Clamp(rb2D.linearVelocityY, -maxVeloctiyRoation, maxVeloctiyRoation) * rotationMultiplier);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.transform.CompareTag("Deadly"))
        {
            Vector2 contactPoint = collision.contacts[0].point;
            Vector2 forceDirection = ((Vector2)transform.position - contactPoint).normalized;
            rb2D.AddForce(forceDirection * deathForce, ForceMode2D.Impulse);
            OnDeath();
        }
        else
        {
            isOnFloor = true;
        }
         
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.transform.CompareTag("Deadly"))
        {
            isOnFloor = false;
        }
    }

    private void OnDeath()
    {
        isDead = true;
        OnGameOver?.Invoke();
        AudioManager.instance.deathSource.Play();
    }

    public void OnStart()
    {
        rb2D.gravityScale = gravityScale;
    }
}
