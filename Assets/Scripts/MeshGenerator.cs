using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    [Range(1,200)]
    public int xSize = 20;
    [Range(1,200)]
    public int zSize = 20;

    public float scale;
    
    public int xPos;
    public int zPos;
    public int vertIndex;
    [Range(0,4)]
    public int morphRange;

    public float yRotation;

    public Transform player;
    public Gradient gradient;
    
    private int[] triangles;
    private Vector3[] vertices;
    private Color[] colors;
    
    private MeshCollider meshCollider;
    private Mesh mesh;
    [Range(0,3)]
    public int y = 0;
    public float heightMultiplier;

    private int buildOffset;

    public bool forward;
    public bool back;
    public bool left;
    public bool right;

    private float minTerrainHeight;
    private float maxTerrainHeight;

    private void Start()
    {
        mesh = new Mesh();
        mesh = GetComponent<MeshFilter>().mesh;

        CreateMesh();
        RedrawMesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
    }

    private void Update()
    {
        yRotation = player.transform.localEulerAngles.y;
        CalculatePlayerCoords();
        if (Input.GetKeyDown(KeyCode.E))
        {
            y = 1;
            UpdateMesh();
            RedrawMesh();
            UpdateMeshCollider();
        }
    }

    private void OnValidate()
    {
        if (y > 0)
        {
            UpdateMesh();
        }
        RedrawMesh();
        UpdateMeshCollider();
    }

    void CreateMesh()
    {
       
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.PerlinNoise(x * .3f, z * .3f) * heightMultiplier;
                vertices[i] = new Vector3(x, height, z);

                if (y > maxTerrainHeight)
                    maxTerrainHeight = y;
                if (y < minTerrainHeight)
                    minTerrainHeight = y;
                
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris] = vert + 0;
                triangles[tris+1] = vert + xSize + 1;
                triangles[tris+2] = vert + 1;
            
                triangles[tris+3] = vert + 1;
                triangles[tris+4] = vert + xSize + 1;
                triangles[tris+5] = vert + xSize + 2;
                
                vert++;
                tris += 6;
            }
            vert++;
        }
        colors = new Color[vertices.Length];
        
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight,maxTerrainHeight,vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }
    
    void RedrawMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        
        mesh.RecalculateNormals();
    }

    private void UpdateMesh()
    {
        if (forward || back)
        {
            for (int i = vertIndex + (xSize * buildOffset) - (Mathf.RoundToInt(morphRange / 2)) + buildOffset;
                i < vertIndex + (morphRange / 2) + (xSize * buildOffset) + buildOffset;
                i++)
            {
                //vertices[i] += new Vector3(0, y, 0);
                Vector3 riseToPoint = vertices[i] + new Vector3(0, y, 0);
                vertices[i] = Vector3.Slerp(vertices[i] , riseToPoint, 2);
            }
        }
        else
        {
            for (int i = vertIndex + buildOffset - (xSize + 1) * (Mathf.RoundToInt(morphRange / 2));
                i < vertIndex + buildOffset + (xSize + 1) * (Mathf.RoundToInt(morphRange / 2));
                i += (xSize + 1))
            {
                vertices[i] += new Vector3(0, y, 0);
                Vector3 riseToPoint = vertices[i] + new Vector3(0, y, 0);
                vertices[i] = Vector3.Slerp(vertices[i] , riseToPoint, 2);
            }
        }
    }

    void UpdateMeshCollider()
    {
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }
    
    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (i % 2 == 0)
                Gizmos.color = Color.blue;
            else 
                Gizmos.color = Color.red;
            Gizmos.DrawSphere(vertices[i],.1f);
        }
    }

    void CalculatePlayerCoords()
    {
        xPos = Mathf.RoundToInt(player.position.x);
        zPos = Mathf.RoundToInt(player.position.z);
        
        // Only works when mesh is a square :/
        vertIndex = (zPos * zSize) + xPos + zPos;

        if (vertices[vertIndex].y == 0)
            y = 0;

        if ((-45 < yRotation && yRotation <= 0) || (0 <= yRotation && yRotation <= 45))
        {
            // forward
            forward = true;
            back = false;
            right = false;
            left = false;
            buildOffset = 2;
        }
        else if (45 < yRotation && yRotation <= 135)
        {
            // right
            forward = false;
            back = false;
            right = true;
            left = false;
            buildOffset = 2;
        }
        else if ((135 < yRotation && yRotation <= 180) || (-180 < yRotation && yRotation <= -135))
        {
            // back
            forward = false;
            back = true;
            right = false;
            left = false;
            buildOffset = -2;
        }
        else if (-135 < yRotation && yRotation <= 0)
        {
            // left
            forward = false;
            back = false;
            right = false;
            left = true;
            buildOffset = -2;
        }
    }
}


