// Authors: Kalby Jang
// Copyright © 2021 DigiPen - All Rights Reserved

using System;
using System.Collections;
using System.Collections.Generic;
using GG;
using GG.Entity;
using UnityEngine;
using Random = UnityEngine.Random;
using Ludiq;
using Bolt;
using UnityEngine.Events;
using UnityEngine.VFX;

public class ExplosiveBarrel : MonoBehaviour, IDamageable
{
    [Header("Explosion Stuff")]
    public AoeAttack aoeAttack;

    public AreaDamage     areaDamage;
    public SphereCollider blastRadius;
    
    public float          explosionTime = 1;
    

    [Header("The aesthetics")]
    public MeshRenderer barrelModel;

    public  Health            health { get; private set; }
    public  GameObject        PopupTextPrefab;
    public  Vector3           popupTextPositionOffest;
    [SerializeField] private PopupTextSettings popupTextSettings;

    public AK.Wwise.Event damagedSound;
    public AK.Wwise.Event leakingSound;
    

    public VisualEffect smokeEffects;
    public VisualEffect explosionEffects;
        
    
    public UnityEvent OnValatileState;
    public UnityEvent OnExplosionState;

    
    
    
    private bool isExploding = false; 
    private float curTime;
    
    
    
    void Awake()
    {
        // health = GetComponentInChildren<Health>();

        curTime = explosionTime;
    }
    
    private void OnEnable()
    {
        // health.onDie     += OnDeath;
        // health.onDamaged += OnDamaged;
        areaDamage.OnHit += OnHit;
    }

    private void OnDisable()
    {
        // health.onDamaged -= OnDamaged;
        areaDamage.OnHit -= OnHit;
        // health.onDie     -= OnDeath;


    }

    private void Update()
    {
        // ExplosionPhase();
    }
    
    //-------------------------------------------------------------------------
    //  Inherited Methods
    //-------------------------------------------------------------------------
    public void Damage( DamageInfo damageInfo )
    {
        if (damageInfo.owner.CompareTag("Enemy"))
            return;
        CustomEvent.Trigger( gameObject, "damaged" );

        smokeEffects.transform.position = damageInfo.collisionInfo.contacts[0].point;
        smokeEffects.transform.forward = damageInfo.collisionInfo.contacts[0].normal;
        
        damagedSound.Post( gameObject );

        OnValatileState?.Invoke();
    }
    
    //-------------------------------------------------------------------------
    //  Public Methods
    //-------------------------------------------------------------------------

    public void VolatileState()
    {
        leakingSound.Post( gameObject );
        
        
        OnValatileState?.Invoke();
    }
    
    public void ExplodeState()
    {
        
        leakingSound.Stop( gameObject );

        
        blastRadius.gameObject.SetActive( true );
        // knockback.gameObject.SetActive( false );
        
        aoeAttack.Attack();
        barrelModel.enabled = false;

        

        OnExplosionState?.Invoke();
    }

    public void SetSmokeEffects( Collision col )
    {
        smokeEffects.transform.position = col.contacts[0].point;
        smokeEffects.transform.forward  = col.contacts[0].normal;

    }

    

    //-------------------------------------------------------------------------
    //  Private Methods
    //-------------------------------------------------------------------------
    
    private void ExplosionPhase()
    {
        if (!isExploding) return;

        curTime -= Time.deltaTime;

        if (curTime <= 0f)
        {
            // Finishing things up
            // blastRadius.gameObject.SetActive( false );
            // knockback.gameObject.SetActive( false );
            
            Destroy(gameObject);
        }
    }

    private void DamagePopup( Vector3 position, float damage )
    {
        GameObject popupText = Instantiate(PopupTextPrefab);
        popupText.transform.position = position;
        
        popupTextSettings.text       = damage.ToString();

        popupText.GetComponent<PopupText>().InitializePopupText(popupTextSettings);
    }

    //-------------------------------------------------------------------------
    //  Event Callbacks
    //-------------------------------------------------------------------------


    private void OnHit(float amount, GameObject victim)
    {
        DamagePopup(victim.transform.position + popupTextPositionOffest, amount);
    }
    
    private void OnDeath()
    {
        isExploding = true;
        
        // Blast damage
        aoeAttack.Attack();

        //curTime = aoeAttack.attackRadiusSize / aoeAttack.attackGrowthRate;
        curTime = aoeAttack.GrowthTime;
        
        // Effects
        barrelModel.enabled = false;
    }
    
}
