using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("health")]
    [SerializeField] RectTransform currentHealth;
    [SerializeField] PlayerController player;

    [Header("transparency")]
    [SerializeField] float transparencyPercentage = 20;
    [SerializeField] Transform screenBarPosition;
    [SerializeField] RawImage currentHealthImg;
    [SerializeField] RawImage healthBarImg;
    [SerializeField] Outline outline;

    private float actualPercentage;
    private float currentHealthInitialTransparency;
    private float healthBarInitialTransparency;
    private float outlineInitialTransparency;
    private bool under = false;

    private void Start()
    {
        actualPercentage = transparencyPercentage / 100;
        currentHealthInitialTransparency = currentHealthImg.color.a;
        healthBarInitialTransparency = healthBarImg.color.a;
        outlineInitialTransparency = outline.effectColor.a;
    }

    void Update()
    {
        // update health bar
        currentHealth.anchorMax = new Vector3(player.health / 100, currentHealth.anchorMax.y);

        // transparency
        TransparencyEffect();
    }

    private void TransparencyEffect()
    {
        if (player.transform.position.y < screenBarPosition.position.y && under == false)
        {
            under = true;
            ChangeTransparency();
        }

        if (player.transform.position.y >= screenBarPosition.position.y && under == true)
        {
            under = false;
            ChangeTransparency();
        }

    }

    private void ChangeTransparency(bool reset = false)
    {
        currentHealthImg.color = new Color(currentHealthImg.color.r, currentHealthImg.color.g, currentHealthImg.color.b, !under ? currentHealthInitialTransparency : currentHealthImg.color.a * actualPercentage);
        healthBarImg.color = new Color(healthBarImg.color.r, healthBarImg.color.g, healthBarImg.color.b, !under ? healthBarInitialTransparency : healthBarImg.color.a * actualPercentage);
        outline.effectColor = new Color(outline.effectColor.r, outline.effectColor.g, outline.effectColor.b, !under ? outlineInitialTransparency : outline.effectColor.a * actualPercentage);
    }

}
