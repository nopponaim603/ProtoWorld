using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ProtoWorldPlayer : NetworkBehaviour {

    public enum PlayerRole
    {
        Residence,
        LogisticsProvider,
        Preple
    }

    public GameObject _mainUI;
    public GameObject _parentUI;
    public GameObject[] _ClickDropComponentLists;
    
    [SyncVar]
    public int IndexPlayer;

    public bool isFirstSetup;
    float countDown = 1f;

    public bool isFirstSetingUI =false;
    [SyncVar]
    public bool isReadyCreateUI = false;

    [SyncVar]
    public bool isReadyCommitPhase = false;

    [SyncVar]
    public int _CurrentGamePhase = 0;

    [SyncVar]
    public int _Quiz1 = 0;
    [SyncVar]
    public int _Quiz2 = 0;
    [SyncVar]
    public int _Quiz3 = 0;
    //public SyncListInt m_ints = new SyncListInt();
    //public int[] _CurrentResult = { 0, 0, 0 };
    //public Text _GamePhaseText;
    //public ProtoWorldGamePhasePanel _GamePhasePanel;
    public override void OnStartLocalPlayer()
    {
        //isFirstSetup = false;
        //_mainUI.SetActive(true);
        BuildClick();
    }

    public void BuildClick()
    {
        CmdBuildClick(this.gameObject, IndexPlayer);
    }

    [Command]
    void CmdBuildClick(GameObject self, int IndexPlayer)
    {
        if (IndexPlayer != 3)
        {

            //print("Name : " + _parentUI.name + " : activeSelf = " + _parentUI.activeSelf);

            GameObject ui = Instantiate(_ClickDropComponentLists[IndexPlayer]);
            ui.GetComponent<ClickAndSpawnNet>().owner = self;
            //ui.GetComponent<ClickAndSpawn>().RpcSetupParent();
            //ui.transform.SetParent(_parentUI.transform);
            NetworkServer.Spawn(ui);

            isReadyCreateUI = true;
        }
        //print("Try to Create.");
    }

    // Use this for initialization
    void Start () {
        //m_ints.
        //_GamePhaseText = GameObject.FindGameObjectWithTag("GamePhase").GetComponentInChildren<Text>();
        //_GamePhasePanel = GameObject.FindGameObjectWithTag("GamePhase").GetComponent<ProtoWorldGamePhasePanel>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;

        if(isReadyCreateUI && !isLocalPlayer && !isFirstSetingUI) {
            isFirstSetingUI = true;
            _mainUI.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            //m_ints.Clear();
            IntChanged();
        }

        if(Input.GetKeyDown(KeyCode.C) && _CurrentGamePhase < 6 && !isReadyCommitPhase)
        {
            print("Commint.. on Client.");
            isReadyCommitPhase = true;

            IntChanged();

            CmdCommitPhase(isReadyCommitPhase, _Quiz1 , _Quiz2, _Quiz3);
        }
    }


    [Command]
    public void CmdCommitPhase(bool inputReadyCommitPhase,int _q1 , int _q2 , int _q3)
    {
        this.isReadyCommitPhase = inputReadyCommitPhase;

        print("Server Print...");

        print("Quiz1 : " + _q1 + " : Quiz2 : " + _q2 + " : Quiz3 : " + _q3);
        /*
        for(int i = 0; i < _resultlist.Count; i++)
        {
            print("Index Result : " + i + " | Result is : " + _resultlist[i]);
        }
        */
    }

    private void OnIntChanged(SyncListInt.Operation op, int index)
    {
        Debug.Log("list changed " + op);
    }

    void IntChanged()
    {
        if (_CurrentGamePhase == 3)
        {
            List<int> temp = ProtoWorldNetGM.instance._GamePhasePanel.GetToggleGroups();
            _Quiz1 = temp[0];
            _Quiz2 = temp[1];
            _Quiz3 = temp[2];
        }
    }

    [ClientRpc]
    public void RpcUpdateCurrentGamePhase(int inputGamePhase)
    {
        print("Client Update Game Phase.");
        this._CurrentGamePhase = inputGamePhase;
        
        ProtoWorldNetGM.instance._GamePhasePanel.UpdatePanel(this._CurrentGamePhase);
        isReadyCommitPhase = false;
    }
}
