using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ProtoWorldLobbyHook : LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        base.OnLobbyServerSceneLoadedForPlayer(manager, lobbyPlayer, gamePlayer);
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();

        ProtoWorldPlayer player = gamePlayer.GetComponent<ProtoWorldPlayer>();
        player.name = lobby.nameInput.text;
        player.IndexPlayer = lobby.IndexPlayer;
        //player.RpcUpdateCurrentGamePhase(ProtoWroldNetGM.instance._CurrentGamePhase);
        //player.BuildClick();

        print("Player Index : " + lobby.IndexPlayer);

        ProtoWorldNetGM.instance._listPlayer.Add(player);
    }

}
