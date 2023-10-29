using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class AttackCollider : MonoBehaviour
{

    public MMFeedbacks enemyHitFeedback;
    public float Knockback;
    public float KnockbackY;
    public bool enemyStandUp;
    public string enemyHitAnim;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetFeedback(MMFeedbacks enemyHitFeedback)
    {
        this.enemyHitFeedback = enemyHitFeedback;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
