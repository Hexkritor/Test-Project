using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestProject
{
    

    public class PlayerController : MonoBehaviour
    {
        public delegate void OnDeathDelegate();
        public OnDeathDelegate onDeathAction;

        [Header("Components")]
        [SerializeField]
        private Rigidbody m_rigid;
        [Header("Parameters")]
        [SerializeField]
        private PlayerData m_data;


        private int m_hp;
        private int m_maxHp;

        private float m_attackBuffTime;

        private Vector3 m_movementDirection = Vector3.zero;

        public bool canAttack { get { return m_attackBuffTime > 0; } }
        public float hpPercent { get { return (float)m_hp/m_maxHp; } }
        public string hpPercentText { get { return string.Format("{0}/{1}", m_hp, m_maxHp); } }

        private void Start()
        {
            //component & data check
            if (!m_data)
            {
                throw new System.Exception("Player has no data! Cannot init the player");
            }
            if (!m_rigid)
            {
                m_rigid = GetComponent<Rigidbody>();
                if (!m_rigid)
                {
                    throw new System.Exception("No rigidbody attached to player");
                }
            }

            m_hp = m_data.hp;
            m_maxHp = m_hp;

            StartCoroutine(UpdateAttack());
        }

        private void Update()
        {
            //Move Player
            m_movementDirection.x = Input.GetAxis("Horizontal");
            m_movementDirection.z = Input.GetAxis("Vertical");

            if (m_movementDirection.sqrMagnitude > 1)
            {
                m_movementDirection.Normalize();
            }

            m_rigid.velocity = m_movementDirection * m_data.speed;
        }

        IEnumerator UpdateAttack()
        {
            while (true)
            {
                m_attackBuffTime = (m_attackBuffTime > 0) ? m_attackBuffTime - Time.deltaTime : 0;
                yield return new WaitForEndOfFrame();
            }
        }

        public void SetAttackTime(float attackTime)
        {
            m_attackBuffTime = attackTime;
        }

        public void OnCollisionEnter(Collision collision)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy && m_attackBuffTime > 0)
            {
                enemy.GetDamage(m_data.damage);
            }
        }

        public void GetDamage(int value)
        {
            m_hp -= value;
            print("Player HP :" + m_hp.ToString());
            if (m_hp <= 0)
            {
                Death();
            }
        }

        protected void Death()
        {
            Camera.main.transform.parent = null;
            onDeathAction?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
