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

    public List<int> GetToggleGroups()
    {
        List<int> result = new List<int>();
        ToggleGroup[] _listToggleGroup = MasterPlanningPanel.GetComponentsInChildren<ToggleGroup>();
        foreach(ToggleGroup tempToggleGroup in _listToggleGroup)
        {
            IEnumerable<Toggle> _listActiveToggles = tempToggleGroup.ActiveToggles();
            foreach (Toggle tempToggle in _listActiveToggles)
            {
                print("Group : " + tempToggleGroup.name + " | Name Active : " + tempToggle.name + " : " + tempToggle.transform.GetSiblingIndex());

                result.Add(tempToggle.transform.GetSiblingIndex() - 1);
            }
        }

        return result;
    }

    public void ShowVote()
    {
        string TempMessage = "";

        TempMessage += "Quiz 1 ";
        for (int x = 0; x < ProtoWorldNetGM.instance._SumVoteQuiz1.Length; x++)
        {
            TempMessage += "\nVote Choice " + x + " = " + ProtoWorldNetGM.instance._SumVoteQuiz1[x];
        }

        TempMessage += "\nQuiz 2 ";
        for (int x = 0; x < ProtoWorldNetGM.instance._SumVoteQuiz2.Length; x++)
        {
            TempMessage += "\nVote Choice " + x + " = " + ProtoWorldNetGM.instance._SumVoteQuiz2[x];
        }

        TempMessage += "\nQuiz 3 ";
        for (int x = 0; x < ProtoWorldNetGM.instance._SumVoteQuiz3.Length; x++)
        {
            TempMessage += "\nVote Choice " + x + " = " + ProtoWorldNetGM.instance._SumVoteQuiz3[x];
        }

        MasterPlanningPanel.GetComponentInChildren<Text>().text = TempMessage;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
