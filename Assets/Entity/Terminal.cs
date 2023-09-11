// Authors: Robert Seiver, Kalby Jang
// Copyright © 2020 DigiPen - All Rights Reserved

using GG;
using GG.Level;
using UnityEngine;
using UnityEngine.Events;

public class Terminal : MonoBehaviour, IPrompt
{
    public Material OnMaterial;
    public MeshRenderer ScreenMesh;
    private static Portal portal;
    public TerminalUI terminalUI;
    public UnityAction OnActivation;
    
    private GameObject marker;
    [HideInInspector]
    public LevelGenerator.Quadrant QuadrantLocation;
    private TerminalChecklist TerminalChecklistComponent;
    
    private TriggerVolume.PromptTriggered callback;

    public bool Activated { get; private set; } = false;
    
    // Start is called before the first frame update
    private void Start()
    {
        if (portal == null)
            portal = GameObject.Find("Portal").GetComponent<Portal>();

        if (terminalUI == null)
            terminalUI = GetComponentInChildren<TerminalUI>();

        TerminalChecklistComponent = FindObjectOfType<TerminalChecklist>();
    }

    public void SetMarkerObject(GameObject obj)
    {
        marker = obj;
    }

    public void Activate()
    {
        //Tell the TerminalChecklist which terminal was just activated
        if (TerminalChecklistComponent != null)
            TerminalChecklistComponent.targetQuadrant = QuadrantLocation;

        if (callback != null)
            callback();
        
        Activated = true;
        ScreenMesh.material = OnMaterial;
        portal.Terminals.Remove(gameObject);
        if (marker != null)
            marker.SetActive(false);
        portal.TerminalActivated(marker);
        terminalUI?.SetToActivatedState();
    }
    
    // Event Callback Functions ----------------------------------------------- 

    // OnTerminalActivatedCB
    // param: count
    // Updates all terminal UI text
    public void OnTerminalActivatedCB()
    {
        TerminalUI.current = portal.ActiveTerminals;
        TerminalUI.max = portal._requiredTerminals;
        
        if (Activated)
            terminalUI.SetMonitorText( portal.ActiveTerminals + " / " + portal._requiredTerminals + "\n" + "Activated" );
    }

    public void OnCountDownUpdate(float time)
    {
        terminalUI.SetMonitorText(time.ToString("F3"));
    }
    
    public void SetEventCallback(TriggerVolume.PromptTriggered callback)
    {
        this.callback = callback;
    }

}