using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ProtoWroldNetGM : NetworkBehaviour {

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

    public static ProtoWroldNetGM instance;

    public List<ProtoWorldPlayer> _listPlayer;

    [SyncVar]
    public bool isFirstTimeSetting = false;

    private void Awake()
    {
        instance = this;
        _listPlayer = new List<ProtoWorldPlayer>();
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


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_listPlayer.Count > 0)
            {
                bool allreadyCommit = true;

                foreach (ProtoWorldPlayer player in _listPlayer)
                {
                    allreadyCommit &= player.isReadyCommitPhase;
                }

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
