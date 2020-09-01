using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AI;
using UnityEngine;
using UnityEngine.AI;

[CustomEditor(typeof(LocalNavMeshBuilderUseAgent))]
public class LocalNavMeshBuilderUseAgentEditor : Editor
{
    private SerializedProperty m_AgentTypeID;

    private void OnEnable()
    {
        m_AgentTypeID = serializedObject.FindProperty("m_AgentTypeID");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var bs = NavMesh.GetSettingsByID(m_AgentTypeID.intValue);
        serializedObject.Update();
        if (bs.agentTypeID != -1)
        {
            // Draw image
            const float diagramHeight = 80.0f;
            Rect agentDiagramRect = EditorGUILayout.GetControlRect(false, diagramHeight);
            NavMeshEditorHelpers.DrawAgentDiagram(agentDiagramRect, bs.agentRadius, bs.agentHeight, bs.agentClimb, bs.agentSlope);
        }
        NavMeshComponentsGUIUtility.AgentTypePopup("Agent Type", m_AgentTypeID);
        serializedObject.ApplyModifiedProperties();
    }
}
