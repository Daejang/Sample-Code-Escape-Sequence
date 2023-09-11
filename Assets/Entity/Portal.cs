// Authors: Robert Seiver, Kalby Jang
// Copyright © 2020 DigiPen - All Rights Reserved

using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using GG.Player;
using TMPro;
using UnityEngine.Events;

namespace GG.Level
{
    public class Portal : MonoBehaviour
    {
        public Material OnMaterial;
        public MeshRenderer PortalMesh;
        public LevelGenerator LevelGenerator;
        public List<GameObject> Terminals = new List<GameObject>();

        public int _requiredTerminals;
        public int RequiredTerminals
        {
            get => _requiredTerminals;
            set
            {
                Debug.Log("Set to " + value  );
                _requiredTerminals = value;
            }
        }
        
        private int activeTerminals;
        public int ActiveTerminals
        {
            get => activeTerminals;
            set => activeTerminals = value;
        }

        // Countdown timer
        [SerializeField] private GameObject PortalCountdownPanel;
        [SerializeField] private GameObject TerminalPanel;
        public TMP_Text CountdownText;
        [SerializeField] private PopupPanel CountdownPopupPanel;
        public float CountdownTimer;
        private float timer;
        private bool countdownStarted;

        // Start timer to show goal
        [SerializeField] private float TextStartTime = 6f;
        
        public UnityAction OnTerminalActivated;
        public UnityAction<float> OnCountDownUpdate;
        public UnityAction onPortalActivated;

        private PlayerCharacterController PlayerCC;
        
        [SerializeField] private AK.Wwise.Event WinSoundEvent;
        [SerializeField] private AK.Wwise.Event[] TerminalEvents;
        
        private void Awake()
        {
            ActiveTerminals = 0;
            timer = CountdownTimer;
            
            // Testing purposes only
            if (!LevelGenerator)
            {
                Terminal[] terminals = GameObject.FindObjectsOfType<Terminal>();

                foreach (Terminal terminal in terminals)
                    // Terminals.Add( terminal.gameObject );
                    AddTerminal(terminal.gameObject);
            }

            CountdownText = PortalCountdownPanel.GetComponentInChildren<TMP_Text>();
        }

        private void Start()
        {
            //Store a reference to the player controller to use transition functions later
            PlayerCC = GameObject.FindObjectOfType<PlayerCharacterController>();
            
            CountdownPopupPanel.RequestPopup("Get to the portal to escape!", TextStartTime);
        }


        private void Update()
        {
            if (countdownStarted)
            {
                // Update timer
                timer -= Time.deltaTime;
                if (timer <= 0f)
                    timer = 0f;
                CountdownText.text = timer.ToString("F");
                
                OnCountDownUpdate.Invoke(timer);
                
                // Compute x^2 easing interpolant
                float interpolant = 1.0f - (timer / CountdownTimer);
                interpolant *= interpolant;
                float speed = Mathf.Lerp(5.0f, 12.5f, interpolant);
                
                // Update color
                Color c = Color.Lerp(Color.white, Color.red, interpolant);
                c.a = 0.25f * (Mathf.Sin(Time.time * speed) + 3.0f);
                CountdownText.color = c;
                
                // Game over if the timer reaches 0
                if (timer <= 0.1f)
                {
                    // Activate transition effects, will likely need a portal SFX here since there's a delay now
                    PlayerCC.GameOverTransition();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.name == "Player" && activeTerminals >= RequiredTerminals)
            {
                // Activate transtion effects and the Win Wwise Event
                PlayerCC.WinGameTransition();
                WinSoundEvent.Post(gameObject);
            }
        }

        public void AddTerminal(GameObject newTerminal)
        {
            Terminals.Add(newTerminal);

            Terminal terminal = newTerminal.GetComponent<Terminal>();

            OnTerminalActivated += terminal.OnTerminalActivatedCB;
            OnCountDownUpdate += terminal.OnCountDownUpdate;

            TerminalUI.max++;
        }

        public void TerminalActivated(GameObject objMarker)
        {
            if (activeTerminals < RequiredTerminals)
            {
                // Set the next terminal sound event
                TerminalEvents[activeTerminals].Post(GameObject.FindGameObjectWithTag("PersistentAudio"));
                activeTerminals++;
                OnTerminalActivated.Invoke();
                
                // Activate turn sequence
                if (!countdownStarted && activeTerminals >= RequiredTerminals)
                {
                    // Fade out UI
                    CanvasGroup[] groupArr = TerminalPanel.GetComponentsInChildren<CanvasGroup>();
                    foreach (var group in groupArr)
                    {
                        group.DOFade(0f, 1f);
                    }
                    
                    PortalMesh.material = OnMaterial;
                    StartCoroutine(nameof(StartCountdown));
                    
                    // Set an objective marker
                    if (objMarker != null)
                    {
                        objMarker.SetActive(true);
                        var marker = objMarker.GetComponent<ObjectiveMarker>();
                        Vector3 portalPos = transform.position;
                        portalPos.y += transform.localScale.y / 2;
                        marker.MarkNewPosition(portalPos);
                        marker.SetMarkerColor(Color.white);
                    }

                    if (onPortalActivated != null)
                        onPortalActivated.Invoke();
                }
            }
        }
        
        private IEnumerator StartCountdown()
        {
            yield return new WaitForSeconds(2);

            PortalCountdownPanel.SetActive(true);
            CountdownText.transform.parent.GetComponent<CanvasGroup>().DOFade(1f, 1f);

            yield return new WaitForSeconds(1);
            countdownStarted = true;
        }
    }
}