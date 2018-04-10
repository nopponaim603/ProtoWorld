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
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        
    }

    public override void OnStartLocalPlayer()
    {
        //isFirstSetup = false;
        BuildClick();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

    }

    public void BuildClick()
    {
    
        _mainUI.SetActive(true);
        CmdBuildClick(_parentUI, IndexPlayer);
        
    }

    [Command]
    void CmdBuildClick(GameObject _parentUI,int IndexPlayer)
    {
        if (IndexPlayer != 3)
        {
            print("Name : " + _parentUI.name + " : " + _parentUI.transform.ToString());
            GameObject ui = Instantiate(_ClickDropComponentLists[IndexPlayer]);
            ui.GetComponent<RectTransform>().SetParent(_parentUI.GetComponent<RectTransform>());
            NetworkServer.Spawn(ui);
        }
        print("Try to Create.");
    }
}
