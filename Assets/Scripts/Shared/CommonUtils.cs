using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonUtils : MonoBehaviour
{
    public static bool FiftyFifty { get { return UnityEngine.Random.value > 0.5f; } }

    public static bool CheckVisible(Vector3 position, bool isPlayer = false)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
        return !isPlayer
            ? screenPoint.z > 0 && screenPoint.x > -0.1 && screenPoint.x < 1.1 && screenPoint.y > -0.1 && screenPoint.y < 1.1
            : screenPoint.x > -0.03;
    }

    public static float GetAnimationLength(Animator animator, string clipName)
    {
        float length = -1;
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length && length == -1; i++)
        {
            if (clips[i].name == clipName)
            {
                length = clips[i].length;
            }
        }
        return length;
    }
}
