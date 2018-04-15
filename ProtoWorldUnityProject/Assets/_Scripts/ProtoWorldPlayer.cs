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
    //public Text _GamePhaseText;
    //public ProtoWorldGamePhasePanel _GamePhasePanel;

	// Use this for initialization
	void Start () {
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

        if(Input.GetKeyDown(KeyCode.C) && _CurrentGamePhase < 6)
        {
            print("Commint.. on Client.");
            isReadyCommitPhase = true;
            CmdCommitPhase(isReadyCommitPhase);
        }
    }

    public override void OnStartLocalPlayer()
    {
        //isFirstSetup = false;
        //_mainUI.SetActive(true);
        BuildClick();
    }

    public void BuildClick()
    {
        CmdBuildClick(this.gameObject,IndexPlayer);
    }

    [Command]
    void CmdBuildClick(GameObject self,int IndexPlayer)
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

    [Command]
    public void CmdCommitPhase(bool inputReadyCommitPhase)
    {
        this.isReadyCommitPhase = inputReadyCommitPhase;

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
