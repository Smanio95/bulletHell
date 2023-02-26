using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEnemyBullet : EnemyBullet
{
    public Position position = Position.Straight;

    public enum Position{
        Left,
        Straight,
        Right
    }

    private new void Awake()
    {
        base.Awake();
        currentLevel = (int)Levels.Tank;
    }

    void Update()
    {
        CheckPosition();
        transform.Translate(speed * Time.deltaTime * -1 * transform.right, Space.World);
    }

    private void OnEnable()
    {
        if (front != null) transform.position = front.position;

        switch (position)
        {
            case Position.Left:
                transform.Rotate(Vector3.forward, 45);
                break;
            case Position.Right:
                transform.Rotate(Vector3.forward, -45);
                break;
            default:
            case Position.Straight:
                break;
        }
    }

    void CheckPosition()
    {
        if (transform.gameObject.activeSelf && !CommonUtils.CheckVisible(transform.position))
        {
            transform.gameObject.SetActive(false);
        }
    }
}
