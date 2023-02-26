using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyInterface
{
    public float health = 100;

    public float speed = 5f;

    public Transform bulletsParent;

    public EnemyManager.EnemyTypes type;

    protected int currentLevel;

    [Header("Movement")]
    [SerializeField] protected float movementSeconds = 3;
    [SerializeField] protected CharacterController characterController;

    [Header("Shoot")]
    [SerializeField] protected float shootingSec = 1.5f;
    [SerializeField] protected Transform front;

    [Header("Dmg")]
    public float bodyDmg = 50;
    public SpriteRenderer _mesh;

    [Header("Audio")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] AudioClip dmgTakenClip;

    public Vector3 GetDirection(Vector3 position, bool rightSide = false)
    {
        
        if (!rightSide)
        {
            int rndValue = Random.Range(0, position.y <= 0 ? EnemyConstants.UPWARD_MOVES.Length : EnemyConstants.DOWNWARD_MOVES.Length);
            return position.y <= 0 ? EnemyConstants.UPWARD_MOVES[rndValue] : EnemyConstants.DOWNWARD_MOVES[rndValue];
        }
        else
        {
            int rndValue = Random.Range(0, position.y <= 0 ? EnemyConstants.RIGHT_SIDE_UPWARD_MOVES.Length : EnemyConstants.RIGHT_SIDE_DOWNWARD_MOVES.Length);
            return position.y <= 0 ? EnemyConstants.RIGHT_SIDE_UPWARD_MOVES[rndValue] : EnemyConstants.RIGHT_SIDE_DOWNWARD_MOVES[rndValue];
        }
    }

    public override void TakeDmg(float dmg, ref int enemyKilled)
    {
        health -= dmg;
        if (health <= 0)
        {
            enemyKilled++;
            EnemyDeath();
        }
        else
        {
            audioSource.PlayOneShot(dmgTakenClip);
            StartCoroutine(ChangeMesh());
        }
    }

    protected void Update()
    {
        if (!CommonUtils.CheckVisible(transform.position))
        {
            EnemyDeath();
        }
    }

    public override void EnemyDeath()
    {
        gameObject.SetActive(false);
    }

    protected void OnDisable()
    {
        if (type != EnemyManager.EnemyTypes.Boss) EnemyManager.enemyList[currentLevel].Enqueue(this);
    }

    IEnumerator ChangeMesh()
    {
        _mesh.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        _mesh.color = Color.white;
    }

    protected void OnEnable()
    {
        _mesh.color = Color.white;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

}
