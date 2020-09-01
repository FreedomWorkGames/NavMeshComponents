using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AI;
using UnityEngine;
using UnityEngine.AI;

[CustomEditor(typeof(NavMeshSourceTagUseAgentAndArea))]
public class NavMeshSourceTagUseAgentAndAreaEditor : Editor
{
    SerializedProperty m_DefaultArea;
    private SerializedProperty IncludeChildren;
    private NavMeshSourceTagUseAgentAndArea targetComponent;
    private bool m_includeChildrenTmp;
    private List<NavMeshSourceTagUseAgentAndArea> m_childrenTmp = new List<NavMeshSourceTagUseAgentAndArea>();
    private void OnEnable()
    {
        targetComponent = (target as NavMeshSourceTagUseAgentAndArea);
        IncludeChildren = serializedObject.FindProperty("IncludeChildren");
        var parentTag = targetComponent.transform.parent? targetComponent.transform.parent.GetComponentInParent<NavMeshSourceTagUseAgentAndArea>() : null;
        if (parentTag != null && parentTag.IncludeChildren == true)//父节点有，并且勾选includeChildren，删除自己
        {
            DestroyImmediate(targetComponent);
            EditorUtility.DisplayDialog("", "父节点中已经有一个NavMeshSourceTagUseAgentAndArea，并且勾选了IncludeChildren，此处不需要添加", "ok");
            return;
        }

        m_childrenTmp.Clear();
        targetComponent.GetComponentsInChildren<NavMeshSourceTagUseAgentAndArea>(m_childrenTmp); 
        if (m_childrenTmp.Count>1)//子节点有，includeChildren为false
        {
            IncludeChildren.boolValue = false;
            serializedObject.ApplyModifiedProperties();
            //EditorUtility.DisplayDialog("", "子节点中有NavMeshSourceTagUseAgentAndArea，此处IncludeChildren只能为false", "ok");
        } 
        
        m_DefaultArea = serializedObject.FindProperty("m_DefaultArea");

        m_includeChildrenTmp = IncludeChildren.boolValue;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        //draw area
        NavMeshComponentsGUIUtility.AreaPopup("Default Area", m_DefaultArea);

        if (IncludeChildren.boolValue != m_includeChildrenTmp)
        {
            if (IncludeChildren.boolValue)
            {
                m_childrenTmp.Clear();
                targetComponent.GetComponentsInChildren<NavMeshSourceTagUseAgentAndArea>(m_childrenTmp); 
                if (m_childrenTmp.Count>1)//子节点有，includeChildren为false
                {
                    IncludeChildren.boolValue = false;
                    //立即应用修改
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.DisplayDialog("", "子节点中有NavMeshSourceTagUseAgentAndArea，此处无法设置IncludeChildren为true", "ok");
                }   
            }
        }

        m_includeChildrenTmp = IncludeChildren.boolValue;
        
        serializedObject.ApplyModifiedProperties();
    }
}
