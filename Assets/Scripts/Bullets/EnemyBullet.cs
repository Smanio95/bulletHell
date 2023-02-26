using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float dmg = 15;
    public float speed = 10;
    public float offset = 1;
    public Transform front;

    protected int currentLevel;

    protected void Awake()
    {
        GameManager.OnLevelChange += DisableSelf;
    }

    private void OnDestroy()
    {
        GameManager.OnLevelChange -= DisableSelf;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDmg(dmg);
            DisableSelf();
        }
    }

    protected void OnDisable()
    {
        EnemyManager.enemyBulletList[currentLevel].Enqueue(this);
    }

    private void DisableSelf()
    {
        transform.gameObject.SetActive(false);
    }

}
