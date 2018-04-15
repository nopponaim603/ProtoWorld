using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ProtoWorldNetGM : NetworkBehaviour {

    public enum GamePhase
    {
        None,
        Introduction,
        Policy,
        MasterPlanning,
        Zoning,
        Design,
        Permits
    }
    
    [SyncVar]
    public int _CurrentGamePhase = 2;

    public static ProtoWorldNetGM instance;

    public List<ProtoWorldPlayer> _listPlayer;

    [SyncVar]
    public bool isFirstTimeSetting = false;

    public ProtoWorldGamePhasePanel _GamePhasePanel;

    public Transform _parentPanel;
    public GameObject _prefabText;
    public List<Text> _listState;
    private void Awake()
    {
        instance = this;
        _listPlayer = new List<ProtoWorldPlayer>();
        _listState = new List<Text>();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
    [Server]
	void Update () {

        if (_listPlayer.Count > 0 && !isFirstTimeSetting)
        {
            isFirstTimeSetting = true;
            foreach (ProtoWorldPlayer player in _listPlayer)
            {
                player.RpcUpdateCurrentGamePhase(_CurrentGamePhase);
                print("Update");
            }
            
        }


        if(_listState.Count < _listPlayer.Count)
        {
            _listState.Add(Instantiate(_prefabText, _parentPanel).GetComponent<Text>());
        }else
        {
            for (int i = 0; i < _listPlayer.Count; i++)
            {
                _listState[i].text = _listPlayer[i].name + " : commit = " + _listPlayer[i].isReadyCommitPhase;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_listPlayer.Count > 0)
            {
                bool allreadyCommit = true;

                for(int i = 0; i< _listPlayer.Count; i++)
                {
                    _listState[i].text = _listPlayer[i].name + " : commit = " + _listPlayer[i].isReadyCommitPhase;
                    allreadyCommit &= _listPlayer[i].isReadyCommitPhase;
                }

                /*
                foreach (ProtoWorldPlayer player in _listPlayer)
                {

                    allreadyCommit &= player.isReadyCommitPhase;
                }*/

                if (allreadyCommit)
                {
                    print("All True...");
                    _CurrentGamePhase++;
                    foreach (ProtoWorldPlayer player in _listPlayer)
                    {
                        player.RpcUpdateCurrentGamePhase(_CurrentGamePhase);
                    }
                    //RpcUpdateCurrentGamePhase
                }
            }
        }
        
        
    }

    public override void OnNetworkDestroy()
    {
        base.OnNetworkDestroy();
        _listPlayer.Clear();
    }
}
