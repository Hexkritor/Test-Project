using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TestProject
{
    public class UIGamePanel : MonoBehaviour
    {

        public PlayerController player;

        public Image hpBar;
        public TextMeshProUGUI hpLabel;
        public TextMeshProUGUI attackLabel;

        void OnGUI()
        {
            if (player)
            {
                hpBar.fillAmount = player.hpPercent;
                hpLabel.text = player.hpPercentText;
                attackLabel.text = string.Format("Attack? {0}", (player.canAttack) ? "Yes" : "No");
            }
        }
    }
}
