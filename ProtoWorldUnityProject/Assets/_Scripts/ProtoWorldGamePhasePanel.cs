using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProtoWorldGamePhasePanel : MonoBehaviour {

    public GameObject PolicyPanel;
    public GameObject MasterPlanningPanel;
    public GameObject ZoningPanel;
    public Text HeaderPanel;

    public void UpdatePanel(int _currentGamePhase)
    {
        switch((ProtoWorldNetGM.GamePhase)_currentGamePhase)
        {
            case ProtoWorldNetGM.GamePhase.Policy:
                PolicyPanel.SetActive(true);
                MasterPlanningPanel.SetActive(false);
                ZoningPanel.SetActive(false);
                break;
            case ProtoWorldNetGM.GamePhase.MasterPlanning:
                PolicyPanel.SetActive(false);
                MasterPlanningPanel.SetActive(true);
                ZoningPanel.SetActive(false);
                break;
            case ProtoWorldNetGM.GamePhase.Zoning:
                PolicyPanel.SetActive(false);
                MasterPlanningPanel.SetActive(false);
                ZoningPanel.SetActive(true);
                break;

            default:
                PolicyPanel.SetActive(false);
                MasterPlanningPanel.SetActive(false);
                ZoningPanel.SetActive(false);
                break;
        }

        HeaderPanel.text = ((ProtoWorldNetGM.GamePhase)_currentGamePhase).ToString();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
