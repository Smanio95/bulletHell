using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastBullet : EnemyBullet
{
    private Vector3 originalScale;
    private Vector3 originalPos;

    [Header("Creation")]
    [SerializeField] float creationSpeed = 2;
    [SerializeField] float creationSize = 50;
    [SerializeField] float minimizationSpeed = 1.5f;
    public FastEnemy shooter;
    public bool rightSided;
    public bool endOfAnimation = false;

    private new void Awake()
    {
        base.Awake();
        currentLevel = (int)Levels.Fast;
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (!shooter.gameObject.activeSelf)
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }

    IEnumerator Create()
    {
        Vector3 newScale = new Vector3(creationSize, originalScale.y, originalScale.z);
        float elapsed = 0;
        float rate = (1 / creationSpeed);
        while (elapsed < 1)
        {
            elapsed += Time.deltaTime * rate;
            transform.localScale = Vector3.Lerp(transform.localScale, newScale, elapsed);

            transform.position = Vector3.Lerp(transform.position,
                new Vector3(!rightSided ? originalPos.x - creationSize / 2 : originalPos.x + creationSize / 2, originalPos.y, originalPos.z),
                elapsed);

            yield return null;
        }
        StartCoroutine(Minimize());
    }

    IEnumerator Minimize()
    {
        Vector3 newScale = new Vector3(transform.localScale.x, 0f, transform.localScale.z);
        float elapsed = 0;
        float rate = (1 / minimizationSpeed);
        while (transform.localScale.y > 0.005f)
        {
            elapsed += Time.deltaTime * rate;
            transform.localScale = Vector3.Lerp(transform.localScale, newScale, elapsed);
            yield return null;
        }
        gameObject.SetActive(false);
    }



    private void OnEnable()
    {
        if(front != null) transform.position = front.position;

        endOfAnimation = false;
        originalPos = transform.position;
        transform.localScale = originalScale;
        StartCoroutine(Create());
    }

    private new void OnDisable()
    {
        base.OnDisable();
        endOfAnimation = true;
    }
}
