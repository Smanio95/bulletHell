using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Func")]
    public CharacterController characterController;
    [SerializeField] Transform ownBody;
    public SpriteRenderer _mesh;
    public bool isDead = false;
    [SerializeField] LayerMask enemyMask;
    public bool endOfLevel = false;

    [Header("Movement")]
    public float speed = 5;

    [Header("Shooting")]
    [SerializeField] PlayerBullet bulletPrefab;
    [SerializeField] Transform bulletsParent;
    [SerializeField] Animator chargeAnimator;
    [SerializeField] float chargeMultiplicator = 2;

    [Header("Life")]
    public float health = 100;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip shotClip;
    [SerializeField] AudioClip chargeShotClip;
    [SerializeField] AudioClip dmgTakenClip;
    [SerializeField] AudioClip deathClip;


    public static Queue<PlayerBullet> bullets;

    [Header("InGame")]
    public int enemyKilled;

    void Start()
    {
        if (bulletPrefab == null)
        {
            Debug.Log("player cant shoot without bullets!");
        }
        bullets = new Queue<PlayerBullet>();
        health = 100;
        enemyKilled = 0;
    }

    void Update()
    {
        if (endOfLevel) return;

        Movement();
        Shoot();
        CheckCollision();
    }

    void CheckCollision()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, characterController.radius + 0.1f, enemyMask);
        if (colliders.Length > 0)
        {
            Enemy enemy = colliders[0].GetComponent<Enemy>();
            TakeDmg(enemy.bodyDmg);
            enemy.gameObject.SetActive(false);
        }
    }

    void Movement()
    {
        float xPos = Input.GetAxis("Horizontal");
        float yPos = Input.GetAxis("Vertical");

        characterController.Move(speed * Time.deltaTime * (Vector3.right * xPos + Vector3.up * yPos));
    }

    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Charge());
        }
    }

    IEnumerator Charge()
    {
        float elapsed = 0;
        float multiplicator = 1;
        bool animationSet = false;

        while (elapsed < 3 && Input.GetKey(KeyCode.Space))
        {

            if (elapsed > 0.5f)
            {
                multiplicator += Time.deltaTime;

                // animations
                if (!animationSet) chargeAnimator.SetBool(AnimationBools.charge, true);
            }

            elapsed += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        PlayerBullet bullet = GetPlayerBullet(multiplicator == 1 ? multiplicator : multiplicator * chargeMultiplicator);

        audioSource.PlayOneShot(multiplicator < 2 ? shotClip : chargeShotClip);

        bullet.gameObject.SetActive(true);
        //animations
        chargeAnimator.SetBool(AnimationBools.charge, false);
    }

    PlayerBullet GetPlayerBullet(float multiplicator)
    {
        PlayerBullet bullet;
        if (bullets.Count == 0)
        {
            bullet = Instantiate(bulletPrefab, bulletsParent);
            bullet.playerBody = ownBody;
            bullet.father = this;
        }
        else
        {
            bullet = bullets.Dequeue();
        }
        bullet.multiplicator = multiplicator;
        return bullet;
    }

    public void TakeDmg(float dmg)
    {
        health -= dmg;
        StartCoroutine(ChangeMesh());
        if (health <= 0)
        {
            PlayerDeath();
        }
        else
        {
            audioSource.PlayOneShot(dmgTakenClip);
        }
    }

    public void PlayerDeath()
    {
        isDead = true;
        audioSource.PlayOneShot(deathClip);
    }

    IEnumerator ChangeMesh()
    {
        _mesh.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        _mesh.color = Color.white;
    }

}
