using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

// Tagging component for use with the LocalNavMeshBuilder
// Supports mesh-filter and terrain - can be extended to physics and/or primitives
[DefaultExecutionOrder(-200)]
public class NavMeshSourceTagImproved : MonoBehaviour
{
    // Global containers for all active mesh/terrain tags
    public static List<NavMeshSourceTagImproved> m_Meshes = new List<NavMeshSourceTagImproved>();
    public static List<NavMeshSourceTagImproved> m_Terrains = new List<NavMeshSourceTagImproved>();
    
    
    [HideInInspector]
    [SerializeField]
    int m_DefaultArea;
    public int defaultArea { get { return m_DefaultArea; } set { m_DefaultArea = value; } }
    
    public List<MeshFilter> selfMeshFilters { get; private set; }
    public List<Terrain> selfTerrains { get; private set; }
    public bool IncludeChildren = true;

    void OnEnable()
    {
        if(selfMeshFilters == null)
            selfMeshFilters = new List<MeshFilter>(IncludeChildren?64 :1);
        if(selfTerrains == null)
            selfTerrains = new List<Terrain>(IncludeChildren?64:1);
        if (IncludeChildren)
        {
            GetComponentsInChildren<MeshFilter>(selfMeshFilters);
            GetComponentsInChildren<Terrain>(selfTerrains);
        }
        else
        {
            var m  = GetComponent<MeshFilter>();
            if (m!= null)
            {
                selfMeshFilters.Add(m);
            }

            var t = GetComponent<Terrain>();
            if (t!= null)
            {
                selfTerrains.Add(t);
            }
        }

        if (selfMeshFilters.Count > 0)
        {
            m_Meshes.Add(this);
        }

        if (selfTerrains.Count > 0)
        {
            m_Terrains.Add(this);
        }
       
    }

    void OnDisable()
    {
        if (selfMeshFilters.Count>0)
        {
            m_Meshes.Remove(this);
            selfMeshFilters.Clear();
        }

        if (selfTerrains.Count>0)
        {
            m_Terrains.Remove(this);
            selfTerrains.Clear();
        }
    }

    // Collect all the navmesh build sources for enabled objects tagged by this component
    public static void Collect(ref List<NavMeshBuildSource> sources)
    {
        sources.Clear();

        for (var i = 0; i < m_Meshes.Count; ++i)
        {
            var mfTag = m_Meshes[i];
            if (mfTag == null) continue;
            foreach (var meshFilter in mfTag.selfMeshFilters)
            {
                var m = meshFilter.sharedMesh;
                if (m == null) continue;

                var s = new NavMeshBuildSource();
                s.shape = NavMeshBuildSourceShape.Mesh;
                s.sourceObject = m;
                s.transform = meshFilter.transform.localToWorldMatrix;
                s.area = mfTag.defaultArea;
                sources.Add(s);
            }
        }

        for (var i = 0; i < m_Terrains.Count; ++i)
        {
            var terrainTag = m_Terrains[i];
            if (terrainTag == null) continue;
            foreach (var terrain in terrainTag.selfTerrains)
            {
                var s = new NavMeshBuildSource();
                s.shape = NavMeshBuildSourceShape.Terrain;
                s.sourceObject = terrain.terrainData;
                // Terrain system only supports translation - so we pass translation only to back-end
                s.transform = Matrix4x4.TRS(terrain.transform.position, Quaternion.identity, Vector3.one);
                s.area = terrainTag.defaultArea;
                sources.Add(s);
            }
        }
    }
}
