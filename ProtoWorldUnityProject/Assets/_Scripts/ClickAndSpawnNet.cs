﻿/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
* 
* DRAG AND DROP SYSTEM 
* Furkan Sonmez
* 
*/

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;

public class ClickAndSpawnNet : NetworkBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    //this is the gameobject that will be instantiated, assign this object in the inspector of the ClickAndDrop object
    public GameObject objectToInstantiate;

    public string objectName;


    public UnityEvent m_OnSpawn;

    private int counter = 0;

    [SyncVar]
    public GameObject owner;

    public delegate void EventOnClickSpawn(Vector3 hitLocation);

    [SyncEvent]
    public event EventOnClickSpawn EventClickSpawn;
    /*
    [ClientRpc]
    public void RpcSetupParent()
    {
       
    }
    */

    public void Start()
    {
        this.transform.SetParent(owner.GetComponent<ProtoWorldPlayer>()._parentUI.transform);
        //EventClickSpawn += CmdCreatePoint;
    }

    // this happens as soon as you click on the image and start dragging. So Immediately after this the object that has been instantiated will be dragged.
    public void OnBeginDrag(PointerEventData eventData)
    {
        //instantiate the gameobject to the hitlocation
        print("Create. form Client.");
        CmdCreatePoint(rayHitPositionClass.hitLocation);
        //EventClickSpawn(rayHitPositionClass.hitLocation);

    }

    //[Command]
    public void CmdCreatePoint(Vector3 hitLocation)
    {
        print("Created. on Server");

        GameObject obj = Instantiate(objectToInstantiate, hitLocation, Quaternion.identity) as GameObject;

        if (objectName != "")
            obj.name = objectName + counter++;
        else
            obj.name = objectToInstantiate.name + counter++;

        NetworkServer.Spawn(obj.gameObject);

        //m_OnSpawn.Invoke();

        //print("Created. on Server");

    }


    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
