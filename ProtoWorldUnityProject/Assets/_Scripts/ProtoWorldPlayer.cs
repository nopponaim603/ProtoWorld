using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ProtoWorldPlayer : NetworkBehaviour {
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
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(isReadyCreateUI && !isLocalPlayer && !isFirstSetingUI)
        {
            isFirstSetingUI = true;
            _mainUI.SetActive(false);
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
        print("Try to Create.");
    }
}
