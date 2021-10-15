using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TestProject
{
    [CustomEditor(typeof(LabyrinthGenerator))]
    public class LabyrinthGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            LabyrinthGenerator labyrinthGenerator = (LabyrinthGenerator)target;

            if (GUILayout.Button("Generate"))
            {
                labyrinthGenerator.Generate();
            }
        }
    }
}
