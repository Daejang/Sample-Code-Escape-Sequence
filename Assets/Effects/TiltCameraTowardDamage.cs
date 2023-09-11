// Authors: Kalby Jang
// Copyright © 2021 DigiPen - All Rights Reserved

using System;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class TiltCameraTowardDamage : MonoBehaviour, IDamageable
{
    
    #region Class Members

    public Transform tiltPivot;

    public float     tiltIntensity = 1;
    public float     tiltAmount;
    
    public Transform testTarget;

    
    private Animator  anim;
    
    private Vector3    sourceXZ;
    private Vector3    positionXZ;
    private Vector3    tiltDirection;
    private Vector3    localTiltDirection;
    private Quaternion targetRotation;

    private int animGotHitHash = Animator.StringToHash( "Got Hit" );


    
    
    #endregion

    #region Unity Methods
    
    void Awake()
    {
        anim = GetComponent<Animator>();

        tiltAmount = 0;
    }

    void LateUpdate()
    {
        // TiltTowardsDamage(testTarget.position);
        
        UpdateCameraTilt();
    }

    #endregion

    #region Class Methods

    public void Damage( DamageInfo damageInfo )
    {
        TiltTowardsDamage( damageInfo.owner.transform.position );
    }

    public void TiltTowardsDamage( Vector3 source )
    {
        
        // if (anim.GetCurrentAnimatorStateInfo( 0 ).IsName( "Camera Tilt" )) return;
        
        var position = transform.position;
        // sourceXZ      = new Vector3( source.x, 0, source.z ) ;
        sourceXZ      = new Vector3( source.x, 0f, source.z ) ;
        positionXZ    = new Vector3( position.x, 0f, position.z );

        // tiltDirection = sourceXZ - positionXZ;
        // tiltDirection = positionXZ - sourceXZ;
        // tiltDirection = source - position;
        tiltDirection = sourceXZ - position;

        localTiltDirection = transform.worldToLocalMatrix.MultiplyVector( tiltDirection );
        
        // targetRotation = Quaternion.FromToRotation( tiltPivot.up, tiltDirection );
        targetRotation = Quaternion.FromToRotation( tiltPivot.up, localTiltDirection );
        // tiltIntensity  = intensity;

        
        
        anim.SetTrigger(animGotHitHash);

        // tiltAmountAnimation.Play();

    }

    public void UpdateCameraTilt()
    {
        if (!anim.GetCurrentAnimatorStateInfo( 0 ).IsName( "Camera Tilt" )) return;
        
        // Debug.Log("Tilt Target Rotation: " + targetRotation);
        // Debug.Log("Total Tilt: " + tiltAmount * tiltIntensity);

        tiltPivot.localRotation = Quaternion.Lerp( Quaternion.identity, 
                                                    targetRotation, 
                                                    tiltAmount * tiltIntensity);
            
        // Debug.Log("Animation Pivot rotation: " + tiltPivot.localRotation);
        
        
    }


    
    
    
    

    #endregion
}
