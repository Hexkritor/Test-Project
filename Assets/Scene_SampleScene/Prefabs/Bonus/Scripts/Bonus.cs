using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TestProject
{
    public class Bonus : MonoBehaviour
    {

        [SerializeField]
        private float m_addAttackTime;

        private void OnTriggerEnter(Collider other)
        {
            print("TRIGGERED");
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            if (player)
            {
                player.SetAttackTime(m_addAttackTime);
                gameObject.SetActive(false);
            }
        }
    }
}
