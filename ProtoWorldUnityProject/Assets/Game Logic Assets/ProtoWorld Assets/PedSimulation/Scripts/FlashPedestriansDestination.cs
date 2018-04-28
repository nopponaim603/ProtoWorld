/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
* 
* FLASH PEDESTRIAN SIMULATOR
* FlashPedestriansDestination.cs
* Miguel Ramos Carretero
* 
*/

using UnityEngine;
using System.Collections;

/// <summary>
/// Class that defines a destination entry for Flash Pedestrians (composed of a transform, 
/// a float priority and a array of colliders representing the stations nearby). 
/// </summary>
public class FlashPedestriansDestination : MonoBehaviour
{
    public string destinationName;

    public bool hideInUI = false;

    public bool isDestinationForWork = false;

    public bool isDestinationForLeisure = false;

    [HideInInspector]
    public Transform[] destinationTransform;

    [Range(0, 10)]
    public float destinationPriority;

    [HideInInspector]
    public Collider[][] stationsNearThisDestination;

    [Range(0, 10000)]
    public float radiousToCheckStations = 500;

    /// <summary>
    /// Number of destination points around this destination.
    /// </summary>
    [Range(1, 10000)]
    public int numberOfdestinations = 1;

    /// <summary>
    /// Radious to spread the destination points.
    /// </summary>
    [Range(1, 100000)]
    public float radiousToSpreadDestinations = 1;

    /// <summary>
    /// True to visualize in the scene the destination radious.
    /// </summary>
    public bool showDestinationArea = false;

    /// <summary>
    /// Material of the destination points.
    /// </summary>
    public Material destinationMaterial;

    /// <summary>
    /// True if destinations should be visualize in the game. 
    /// </summary>
    public bool visualizeDestinations = false;

    /// <summary>
    /// Script awakening.
    /// </summary>
    void Awake()
    {
        destinationTransform = new Transform[numberOfdestinations];
        stationsNearThisDestination = new Collider[numberOfdestinations][];

        //destinationTransform = this.transform;

        //FlashPedestriansGlobalParameters pedGlobalParameters = GetComponent<FlashPedestriansGlobalParameters>();

        for (int i = 0; i < numberOfdestinations; i++)
        {
            GameObject dest = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            dest.transform.parent = this.transform;
            dest.transform.localScale = new Vector3(2f, 0.1f, 2f);

            dest.name = destinationName + i;

            if (destinationMaterial != null)
                dest.GetComponent<MeshRenderer>().material = destinationMaterial;

            if (!visualizeDestinations)
                dest.GetComponent<MeshRenderer>().enabled = false;

            Vector3 position = new Vector3(
                      this.transform.position.x + Random.Range(-radiousToSpreadDestinations, radiousToSpreadDestinations),
                      0f,
                      this.transform.position.z + Random.Range(-radiousToSpreadDestinations, radiousToSpreadDestinations));

            //Move the destination point to the closest point in the walkable navmesh
            UnityEngine.AI.NavMeshHit hit;
            UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 1000.0f,
                  1 << UnityEngine.AI.NavMesh.GetAreaFromName("footway") | 1 << UnityEngine.AI.NavMesh.GetAreaFromName("residential")
                  | 1 << UnityEngine.AI.NavMesh.GetAreaFromName("cycleway") | 1 << UnityEngine.AI.NavMesh.GetAreaFromName("Pedestrian")
                  | 1 << UnityEngine.AI.NavMesh.GetAreaFromName("step") | 1 << UnityEngine.AI.NavMesh.GetAreaFromName("TrafficRoads")
                  | 1 << UnityEngine.AI.NavMesh.GetAreaFromName("Walkable"));

            position = hit.position;

            dest.transform.position = position;

            destinationTransform[i] = dest.transform;

            // Get the stations near this destination point
            stationsNearThisDestination[i] = Physics.OverlapSphere(position, radiousToCheckStations, 1 << LayerMask.NameToLayer("Stations"));

            Debug.Log("Destination " + dest.name + " has found " + stationsNearThisDestination[i].Length + " stations nearby");
        }
    }

    /// <summary>
    /// Draw gizmos on scene.
    /// </summary>
    void OnDrawGizmos()
    {
        if (showDestinationArea)
        {
            Gizmos.color = new Color32(0, 0, 255, 64);
            Gizmos.DrawSphere(transform.position, radiousToSpreadDestinations);
        }
    }
}
