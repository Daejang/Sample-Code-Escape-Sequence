// Authors: Kalby Jang
// Copyright © 2021 DigiPen - All Rights Reserved

using UnityEngine;

public class DamageIndicatorArrow : MonoBehaviour 
{
    public DamageIndicatorArrow( GameObject theArrow, Transform target, float remainingTime )
    {
        this.theArrow      = theArrow;
        this.target        = target;
        this.remainingTime = remainingTime;
    }

    public GameObject theArrow;
    public Transform  target;
    public float      remainingTime;

    




 

}
