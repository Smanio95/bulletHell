using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public PlayerController father;
    public Transform playerBody;
    public float multiplicator = 1;

    [SerializeField] float offset = 1;
    [SerializeField] float speed = 10;
    [SerializeField] float dmg = 25;

    private Vector3 originalScale;
    private float originalDmg;

    private void Awake()
    {
        originalScale = transform.localScale;
        originalDmg = dmg;
    }

    void Update()
    {
        CheckPosition();
        transform.Translate(speed * Time.deltaTime * Vector3.right);
    }

    private void OnEnable()
    {
        Vector3 playerPos = playerBody.position;
        transform.position = new Vector3(playerPos.x + offset, playerPos.y, playerPos.z);
        transform.localScale = originalScale * multiplicator;
        dmg = originalDmg * multiplicator;
    }

    private void OnDisable()
    {
        PlayerController.bullets.Enqueue(this);
    }

    void CheckPosition()
    {
        if (transform.gameObject.activeSelf && !CommonUtils.CheckVisible(transform.position))
        {
            transform.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDmg(dmg, ref father.enemyKilled);
            transform.gameObject.SetActive(false);
        }
    }

}
