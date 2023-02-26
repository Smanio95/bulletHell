using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardEnemyBullet : EnemyBullet
{

    private new void Awake()
    {
        base.Awake();
        currentLevel = (int)Levels.Standard;
    }

    void Update()
    {
        CheckPosition();
        transform.Translate(speed * Time.deltaTime * Vector3.left);
    }

    private void OnEnable()
    {
        if (front != null) transform.position = front.position;
    }

    void CheckPosition()
    {
        if (transform.gameObject.activeSelf && !CommonUtils.CheckVisible(transform.position))
        {
            transform.gameObject.SetActive(false);
        }
    }
}
