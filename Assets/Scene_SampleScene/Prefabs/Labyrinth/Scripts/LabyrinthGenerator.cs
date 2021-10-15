using System.Linq;
using System.Collections.Generic;
using UnityEditor.AI;
using UnityEngine;
using TheHexCore.Basic;

namespace TestProject
{
    public class LabyrinthGenerator : MonoBehaviour
    {

        private const float planeBaseSize = 10;

        [Header("Primitives")]
        [SerializeField]
        private GameObject m_labyrinthPlane;
        [SerializeField]
        private GameObject m_labyrinthWall;
        [Header("Parameters")]
        [SerializeField]
        private Vector2Int m_size;
        [SerializeField, Tooltip("Labyrinth cell in units"), Min(1)]
        private float m_scale;
        [SerializeField, Min(0.1f)]
        private float m_wallThickness;

        private Vector2Int[] directions =
        {
                Vector2Int.up,
                Vector2Int.right,
                Vector2Int.down,
                Vector2Int.left
        };

        public Vector2Int size { get { return m_size; } }
        public float scale { get { return m_scale; } }

        private void CheckVarriables()
        {
            if (!m_labyrinthPlane)
            {
                throw new System.Exception("The labyrinth plane is missing");
            }
            if (!m_labyrinthWall)
            {
                throw new System.Exception("The labyrinth wall is missing");
            }
            if (m_size.x < 1 || m_size.y < 1)
            {
                throw new System.Exception("The labyrinth size axis cannot be less than 1");
            }
        }

        private void ClearLabyrinth()
        {
            Transform[] childs = gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < childs.Length; ++i)
            {
                if (childs[i] != gameObject.transform)
                {
                    DestroyImmediate(childs[i].gameObject);
                }
            }
        }

        private void BuildPlane()
        {
            GameObject plane = Instantiate(m_labyrinthPlane, gameObject.transform);
            plane.transform.localScale = new Vector3(m_size.x / planeBaseSize * m_scale, 1, m_size.y / planeBaseSize * m_scale);
        }

        private void BuildOuterWalls()
        {
            for (int i = 0; i < directions.Length; ++i)
            {
                GameObject bigWall = Instantiate(m_labyrinthWall, gameObject.transform);
                bigWall.transform.localPosition = new Vector3(directions[i].x * m_size.x * m_scale, 1, directions[i].y * m_size.y * m_scale) * 0.5f;
                bigWall.transform.localScale = new Vector3
                (
                    directions[i].x != 0 ? m_wallThickness : m_size.x * m_scale,
                    1,
                    directions[i].y != 0 ? m_wallThickness : m_size.y * m_scale
                );
            }
        }

        public void BuildInnerWalls()
        {
            //set starting flags
            int[,] cellFlags = new int[m_size.x, m_size.y];
            for (int i = 0; i < cellFlags.GetLength(0); ++i)
            {
                for (int j = 0; j < cellFlags.GetLength(1); ++j)
                {
                    cellFlags[i, j] = 0xF;
                }
            }

            Stack<Vector2Int> stack = new Stack<Vector2Int>();
            List<Vector2Int> toSelect = new List<Vector2Int>();
            
            //set starting point
            switch(Random.Range(0,4))
            {
                case 0: stack.Push(new Vector2Int(Random.Range(0, m_size.x), 0));
                    break;
                case 1:
                    stack.Push(new Vector2Int(0, Random.Range(0, m_size.y)));
                    break;
                case 2:
                    stack.Push(new Vector2Int(Random.Range(0, m_size.x), m_size.y - 1));
                    break;
                case 3:
                    stack.Push(new Vector2Int(m_size.x - 1, Random.Range(0, m_size.y)));
                    break;
            }

            //generate wall positions
            while (stack.Count > 0)
            {
                Vector2Int cell = stack.Peek();

                toSelect.Clear();

                for (int i = 0; i < directions.Length; ++i)
                {
                    if (BasicExtention.InRange(cell.x + directions[i].x, 0, m_size.x - 1) &&
                        BasicExtention.InRange(cell.y + directions[i].y, 0, m_size.y - 1))
                    {
                        if (cellFlags[cell.x + directions[i].x, cell.y + directions[i].y] == 0xF)
                        {
                            toSelect.Add(cell + directions[i]);
                        }
                    }
                }

                if (toSelect.Count > 0)
                {
                    Vector2Int next = toSelect[Random.Range(0, toSelect.Count)];
                    cellFlags[cell.x, cell.y] ^= 1 << System.Array.FindIndex(directions, x => x == next - cell);
                    cellFlags[next.x, next.y] ^= 1 << System.Array.FindIndex(directions, x => x == cell - next);
                    stack.Push(next);
                }
                else
                {
                    stack.Pop();
                }
            }

            //instantiate walls
            for (int i = 0; i < cellFlags.GetLength(0); ++i)
            {
                for (int j = 0; j < cellFlags.GetLength(1); ++j)
                {
                    //check only up and right walls
                    int upIndex = System.Array.IndexOf(directions, Vector2Int.up);
                    int rightIndex = System.Array.IndexOf(directions, Vector2Int.right);
                    if (BasicExtention.InRange(i, 0, m_size.x - 1) &&
                        BasicExtention.InRange(j + 1, 0, m_size.y - 1) &&
                        (cellFlags[i, j] & (1 << upIndex)) > 0)
                    {
                        GameObject upWall = Instantiate(m_labyrinthWall, gameObject.transform);
                        upWall.transform.localPosition = new Vector3
                        (
                            -(m_size.x - 1) * 0.5f + i,
                            1,
                            -(m_size.y - 1) * 0.5f + j + 0.5f
                        );
                        upWall.transform.localPosition = Vector3.Scale(upWall.transform.localPosition, new Vector3(m_scale, 0.5f, m_scale));
                        upWall.transform.localScale = new Vector3
                        (
                            Vector2.up.x != 0 ? m_wallThickness : m_scale,
                            1,
                            Vector2.up.y != 0 ? m_wallThickness : m_scale
                        );
                    }
                    if (BasicExtention.InRange(i + 1, 0, m_size.x - 1) &&
                        BasicExtention.InRange(j, 0, m_size.y - 1) &&
                        (cellFlags[i, j] & (1 << rightIndex)) > 0)
                    {
                        GameObject rightWall = Instantiate(m_labyrinthWall, gameObject.transform);
                        rightWall.transform.localPosition = new Vector3
                        (
                            -(m_size.x - 1) * 0.5f + i + 0.5f, 
                            1,
                            -(m_size.y - 1) * 0.5f + j
                        );
                        rightWall.transform.localPosition = Vector3.Scale(rightWall.transform.localPosition, new Vector3(m_scale, 0.5f, m_scale));
                        rightWall.transform.localScale = new Vector3
                        (
                            Vector2.right.x != 0 ? m_wallThickness : m_scale,
                            1,
                            Vector2.right.y != 0 ? m_wallThickness : m_scale
                        );
                    }
                }
            }
        }    


        public void Generate()
        {
            CheckVarriables();
            ClearLabyrinth();
            BuildPlane();
            BuildOuterWalls();
            BuildInnerWalls();

#if UNITY_EDITOR
            NavMeshBuilder.ClearAllNavMeshes();
            NavMeshBuilder.BuildNavMesh();
#endif
        }
    }
}
