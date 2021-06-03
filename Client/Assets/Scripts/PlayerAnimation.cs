using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    int animId = 0;
    Dictionary<int, string> specialAnimatios = new Dictionary<int, string>()
    {
        {2, "Jump"},
        {3, "Fall"},
        {5, "Dash"},
        {6, "Uppercut"},
    };
    Dictionary<int, string> animations = new Dictionary<int, string>()
    {
        {0, "Idle"},
        {1, "Run"},
        {2, "Jump"},
        {3, "Fall"},
        {4, "DashHold"},
        {5, "Dash"},
        {6, "Uppercut"},
    };
    float timer;
    bool specialAnim;
    AnimatorClipInfo[] clipInfo;

    void Update()
    {
        if(specialAnim && timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
                specialAnim = false;
        }else if(timer < 0 && specialAnim == false)
        {
            timer = 0;
            animator.Play("Idle");
        }
    }

    public void SetAnimation(int _animId)
    {
        if(animId == _animId)
            return;
        animId = _animId;
        print(_animId);
        if(animations.ContainsKey(_animId))
        {
            animator.Play(animations[_animId]);
        }
    }
}
