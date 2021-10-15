using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TestProject
{
    public abstract class Enemy : MonoBehaviour
    {

        public enum State
        {
            Normal,
            AttackCooldown,
            Flee
        }

        public delegate void OnDeathDelegate();
        public OnDeathDelegate onDeathAction;

        protected Rigidbody m_rigid;
        protected NavMeshAgent m_navAgent;

        [Header("Parameters")]
        [SerializeField]
        protected int m_hp;
        [SerializeField]
        protected int m_damage;
        [SerializeField]
        protected float m_speed;
        [SerializeField]
        protected float m_attackRange;
        [SerializeField]
        protected float m_attackCooldown;

        protected NavMeshPath m_navPath;
        [SerializeField]
        protected State m_state = State.Normal;
        protected float m_attackCooldownLeft;


        protected virtual void Start()
        {
            m_navAgent = gameObject.GetComponent<NavMeshAgent>();
            m_rigid = gameObject.GetComponent<Rigidbody>();

            m_navPath = new NavMeshPath();

            //component & data check
            if (!m_navAgent)
            {
                throw new System.Exception(string.Format("No NavMeshAgent attached to {0}", gameObject.name));
            }
            if (!m_rigid)
            {
                throw new System.Exception(string.Format("No Rigidbody attached to {0}", gameObject.name));
            }

            m_navAgent.speed = m_speed;
        }

        protected float CalculateDistance(Vector3 targetPoint)
        {

            Vector3[] wayPoints = new Vector3[m_navPath.corners.Length];

            for (int i = 0; i < m_navPath.corners.Length; i++)
            {
                wayPoints[i] = m_navPath.corners[i];
            }

            float distance = 0;

            for (int i = 0; i < wayPoints.Length - 1; i++)
            {
                distance += Vector3.Distance(wayPoints[i], wayPoints[i + 1]);
            }
            return distance;
        }

        public virtual void UpdateEnemy(Vector3 playerCoords, bool canPlayerAttack)
        {
            //update state
            m_state = (canPlayerAttack) ? State.Flee :
                (m_attackCooldownLeft > 0) ? State.AttackCooldown: State.Normal;
            //set action based on state 
            switch (m_state)
            {
                case State.Normal:
                    if (m_navAgent.isOnNavMesh)
                    {
                        m_navAgent.CalculatePath(playerCoords, m_navPath);
                        if (CalculateDistance(playerCoords) <= m_attackRange)
                        {
                            m_navAgent.isStopped = false;
                            m_navAgent.destination = playerCoords;
                        }
                        else
                        {
                            m_navAgent.isStopped = true;
                        }
                    }
                    break;
                case State.AttackCooldown:
                case State.Flee:
                    if (m_navAgent.isOnNavMesh)
                    {
                        m_navAgent.CalculatePath(playerCoords, m_navPath);
                        if (CalculateDistance(playerCoords) <= m_attackRange)
                        {
                            m_navAgent.isStopped = false;
                            m_navAgent.destination = gameObject.transform.position +
                                (gameObject.transform.position - playerCoords).normalized * m_speed;
                        }
                        else
                        {
                            m_navAgent.isStopped = true;
                        }

                    }
                    break;
            }
            m_rigid.velocity = Vector3.zero;
            //update timers
            m_attackCooldownLeft = (m_attackCooldownLeft > 0) ? m_attackCooldownLeft - Time.deltaTime : 0;
        }

        protected void OnCollisionEnter(Collision collision)
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController && m_state == State.Normal)
            {
                playerController.GetDamage(m_damage);
                m_attackCooldownLeft = m_attackCooldown;
            }
        }

        public void GetDamage(int value)
        {
            m_hp -= value;
            if (m_hp <= 0)
            {
                Death();
            }
        }

        protected void Death()
        {
            onDeathAction?.Invoke();
            gameObject.SetActive(false);
        }

    }
}