using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PawsOfFire.Logic
{
    internal class LavaController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                if (PawsOfFireGameManager.allowInput) PawsOfFireGameManager.player.Death();
            }
        }
    }
}
