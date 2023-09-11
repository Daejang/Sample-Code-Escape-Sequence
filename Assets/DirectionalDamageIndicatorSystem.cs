// Authors: Kalby Jang
// Copyright © 2021 DigiPen - All Rights Reserved

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public enum IndicatorMode
{
    TwoD,
    ThreeD
}

public enum IndicatorStyle
{
    TwoD,
    ThreeD
}



public class DirectionalDamageIndicatorSystem : MonoBehaviour, IDamageable
{
  
    
    
    #region Class Members
    
    [Header("Arrows")]
    public IndicatorMode  indicatorMode  = IndicatorMode.TwoD;
    public IndicatorStyle indicatorStyle = IndicatorStyle.TwoD;

    [FormerlySerializedAs( "twoDArrow" )]   public GameObject arrow2D; 
    [FormerlySerializedAs( "threeDArrow" )] public GameObject arrow3D;
    
    [SerializeField] private RectTransform arrow2DCloneParent;
    [FormerlySerializedAs( "arrowCloneParent" )] [SerializeField] private Transform arrow3DCloneParent;

    [SerializeField] private List<DamageIndicatorArrow> indicatorArrows;
    // private RectTransform              rectTransform;
    private List<DamageIndicatorArrow> removedArrows;

    [Header( "Aesthetic" )] 
    [FormerlySerializedAs( "arrow2DDistFromOrigin" )] 
    public float arrow2DDistanceFromOrigin = 20f;
    [FormerlySerializedAs( "arrow3DDistFromOrigin" )] [FormerlySerializedAs( "arrowDistFromOrigin" )] public float        arrow3DDistanceFromOrigin = 1f;
    public  MeshRenderer arrowsCenter;
    
    [Header( "Testing" )] 
    public Transform testAgavator;


    #endregion

    #region Unity Methods
    
    void Awake()
    {
        // rectTransform   = GetComponent<RectTransform>();
        indicatorArrows = new List<DamageIndicatorArrow>();
        removedArrows = new List<DamageIndicatorArrow>();
    }

    void Update()
    {
        // Testing
        if (Input.GetKeyDown( KeyCode.K ) && testAgavator)
            SpawnIndicator(testAgavator);
        
        
        UpdateSpawnedArrows();
    }

    #endregion

    #region Class Methods

    //-------------------------------------------------------------------------
    // Inherited Methods
    //-------------------------------------------------------------------------
    public void Damage( DamageInfo damageInfo )
    {
        SpawnIndicator( damageInfo.owner.transform );
    }
    
    //-------------------------------------------------------------------------
    // Class Methods
    //-------------------------------------------------------------------------

    public void SpawnIndicator( Transform target )
    {
        switch (indicatorMode)
        {
            case IndicatorMode.TwoD:
                Spawn2DArrow( target );
                break;

            case IndicatorMode.ThreeD:
                Spawn3DArrow( target );
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Spawn2DArrow( Transform target )
    {
        SetupSpawnedArrow( Instantiate( arrow2D, Vector3.zero, Quaternion.identity, arrow2DCloneParent ), target );
    }
    
    private void Spawn3DArrow( Transform target )
    {
        SetupSpawnedArrow( Instantiate( arrow3D, transform.position, Quaternion.identity, arrow3DCloneParent ), target );
        
        arrowsCenter.enabled = true;
    }

    private void SetupSpawnedArrow( GameObject newArrow, Transform target )
    {
        // DamageIndicatorArrow arrowInst = new DamageIndicatorArrow(newArrow, target, 3f );
        DamageIndicatorArrow arrowInst = newArrow.GetComponent<DamageIndicatorArrow>();
        
        arrowInst.target        = target;
        arrowInst.remainingTime = 3f;
        
        indicatorArrows.Add( arrowInst );

       

        // Debug.Break();
    }
    
    private void UpdateSpawnedArrows()
    {
        
        
        // Exit if indicator arrow list is empty
        if (indicatorArrows.Count == 0)
        {
            arrowsCenter.enabled = false;

            return;
        }
        
        
        
        // foreach (DamageIndicatorArrow indicatorArrow in indicatorArrows)
        // for (int a = 0; a < indicatorArrows.Count; a++)
        for (int a = indicatorArrows.Count - 1; a >= 0; a--)
        {
            // Countdown
            // indicatorArrow.remainingTime -= Time.deltaTime;
            indicatorArrows[a].remainingTime -= Time.deltaTime;

            // Remove when lifetime is finished
            if (indicatorArrows[a].remainingTime <= 0f)
            {
                RemoveArrow( indicatorArrows[a] );
                continue;
            }
            
            // -- Point at opponent --
            // Setting position
            // Vector3 arrowDirNorm = (indicatorArrow.target.position - canvasCamera.ScreenToWorldPoint( rectTransform.position )).normalized;
            Vector3 arrowDirNorm = (indicatorArrows[a].target.position - transform.position).normalized;

            // Setting Rotation
            switch (indicatorMode)
            {
                case IndicatorMode.TwoD:
                    Update2DArrowTransform(indicatorArrows[a], arrowDirNorm);
                    break;
            
                case IndicatorMode.ThreeD:
                    Update3DArrowTransform(indicatorArrows[a], arrowDirNorm);
                    break;
            
            }
        }
        
        
        
        
    }

    private void Update2DArrowTransform( DamageIndicatorArrow arrow, Vector3 pointDir )
    {
        RectTransform rectTrans = arrow.GetComponent<RectTransform>();

        if (!rectTrans) return;
        
        Quaternion tempQuat = Quaternion.LookRotation( pointDir );
        tempQuat.z = -tempQuat.y;
        tempQuat.x = 0;
        tempQuat.y = 0;
        
        Vector3 northDir = new Vector3(0, 0, transform.eulerAngles.y);
        rectTrans.localRotation = tempQuat * Quaternion.Euler( northDir );

        rectTrans.localPosition = arrow2DCloneParent.localPosition +  rectTrans.up * arrow2DDistanceFromOrigin;
    }
    
    private void Update3DArrowTransform( DamageIndicatorArrow arrow, Vector3 pointDir)
    {
        if (!transform) return;

        // Positions the arrow in little away from the center
        arrow.transform.position = transform.position + pointDir * arrow3DDistanceFromOrigin;
        
        // Rotate towards target
        arrow.transform.LookAt(arrow.target.position); 
    }

    private void RemoveArrow( DamageIndicatorArrow oldArrow )
    {
        if (indicatorArrows.Contains( oldArrow ))
            indicatorArrows.Remove( oldArrow );

        // Can check how the arrow is disabled here
        Destroy( oldArrow.gameObject );
    }


    #endregion
}

  
