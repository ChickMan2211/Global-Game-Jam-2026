using System;
using System.Collections.Generic;
using UnityEngine;

public class LeavesSpawn : MonoBehaviour
{
   public List<GameObject> leaves;
   private  List<Transform> currentPoints;
   public Transform endDistandX;
   public Transform spawnPoint;
   

   private void Update()
   {
      foreach (GameObject l in leaves)
      {
         if (l.transform.position.x < endDistandX.transform.position.x)
         {
            l.transform.position = spawnPoint.transform.position;
         }
      }
  
   }
}
