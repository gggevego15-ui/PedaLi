using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds a lightweight stylized nature backdrop from simple meshes at runtime.
/// Attach to an empty GameObject behind the player. Assign the generated layer
/// transforms to PedaLiVerticalCamera's Parallax Layers array.
/// </summary>
[DisallowMultipleComponent]
public class PedaLiNatureParallaxBuilder : MonoBehaviour
{
    [Header("Backdrop dimensions")]
    [SerializeField, Min(10f)] private float worldWidth = 32f;
    [SerializeField, Min(20f)] private float worldHeight = 100f;
    [SerializeField] private float centerY;
    [SerializeField] private int randomSeed = 15;

    [Header("Depth (camera looks along +Z; player is nearer the camera)")]
    [SerializeField] private float skyZ = 12f;
    [SerializeField] private float farMountainZ = 9f;
    [SerializeField] private float nearMountainZ = 7f;
    [SerializeField] private float forestZ = 5f;
    [SerializeField] private float cloudZ = 10f;

    [Header("Parallax strengths — assign these layer transforms to the camera")]
    [SerializeField, Range(0f, 1f)] private float farMountainFollow = 0.10f;
    [SerializeField, Range(0f, 1f)] private float nearMountainFollow = 0.22f;
    [SerializeField, Range(0f, 1f)] private float forestFollow = 0.42f;
    [SerializeField, Range(0f, 1f)] private float cloudFollow = 0.14f;

    [Header("Build")]
    [SerializeField] private bool buildOnStart = true;
    [SerializeField] private bool rebuildOnPlay = true;

    public Transform FarMountains { get; private set; }
    public Transform NearMountains { get; private set; }
    public Transform Forest { get; private set; }
    public Transform Clouds { get; private set; }
    public Transform Sky { get; private set; }

    private readonly List<Mesh> generatedMeshes = new List<Mesh>();
    private readonly List<Material> generatedMaterials = new List<Material>();

    private static readonly Color SkyColor = new Color(0.38f, 0.72f, 0.91f);
    private static readonly Color FarMountainColor = new Color(0.43f, 0.66f, 0.65f);
    private static readonly Color NearMountainColor = new Color(0.23f, 0.49f, 0.43f);
    private static readonly Color ForestColor = new Color(0.10f, 0.31f, 0.25f);
    private static readonly Color CloudColor = new Color(0.88f, 0.95f, 0.92f);

    private void Start()
    {
        if (buildOnStart) BuildBackdrop();
    }

    [ContextMenu("Build Nature Backdrop")]
    public void BuildBackdrop()
    {
        if (rebuildOnPlay) ClearGeneratedChildren();

        generatedMeshes.Clear();
        generatedMaterials.Clear();

        Sky = CreateLayer("Parallax_Sky", skyZ, 0f);
        AddQuad(Sky, "Sky", new Vector2(-worldWidth * 0.5f, centerY - worldHeight * 0.5f),
            new Vector2(worldWidth * 0.5f, centerY + worldHeight * 0.5f), SkyColor);

        FarMountains = CreateLayer("Parallax_FarMountains", farMountainZ, farMountainFollow);
        AddMountainRange(FarMountains, 17, worldHeight * 0.10f, worldHeight * 0.38f, FarMountainColor, 1);

        NearMountains = CreateLayer("Parallax_NearMountains", nearMountainZ, nearMountainFollow);
        AddMountainRange(NearMountains, 13, worldHeight * 0.06f, worldHeight * 0.28f, NearMountainColor, 2);

        Forest = CreateLayer("Parallax_Forest", forestZ, forestFollow);
        AddForest(Forest, 38);

        Clouds = CreateLayer("Parallax_Clouds", cloudZ, cloudFollow);
        AddClouds(Clouds, 12);

        Debug.Log("PedaLi nature backdrop generated. Assign Parallax_FarMountains, Parallax_NearMountains, Parallax_Forest and Parallax_Clouds to PedaLiVerticalCamera's Parallax Layers.");
    }

    private Transform CreateLayer(string layerName, float z, float follow)
    {
        GameObject layer = new GameObject(layerName);
        layer.transform.SetParent(transform, false);
        layer.transform.position = new Vector3(0f, 0f, z);
        layer.transform.localScale = Vector3.one;

        // Store the intended follow factor as an easy-to-read hint in the object name.
        layer.name = layerName + "_Follow" + Mathf.RoundToInt(follow * 100f);
        return layer.transform;
    }

    private void AddMountainRange(Transform parent, int peaks, float baseOffset, float peakHeight, Color color, int seedOffset)
    {
        Random.State oldState = Random.state;
        Random.InitState(randomSeed + seedOffset);

        List<Vector2> points = new List<Vector2>();
        float left = -worldWidth * 0.65f;
        float right = worldWidth * 0.65f;
        float step = (right - left) / peaks;
        points.Add(new Vector2(left, centerY - worldHeight * 0.5f));

        for (int i = 0; i <= peaks; i++)
        {
            float x = left + i * step;
            float y = centerY - worldHeight * 0.5f + baseOffset + Random.Range(peakHeight * 0.35f, peakHeight);
            points.Add(new Vector2(x, y));
            points.Add(new Vector2(x + step * 0.5f, centerY - worldHeight * 0.5f + baseOffset + Random.Range(0f, peakHeight * 0.55f)));
        }

        points.Add(new Vector2(right, centerY - worldHeight * 0.5f));
        AddPolygon(parent, "MountainMesh", points, color);
        Random.state = oldState;
    }

    private void AddForest(Transform parent, int treeCount)
    {
        Random.State oldState = Random.state;
        Random.InitState(randomSeed + 3);

        float ground = centerY - worldHeight * 0.5f;
        for (int i = 0; i < treeCount; i++)
        {
            float x = Mathf.Lerp(-worldWidth * 0.62f, worldWidth * 0.62f, i / (float)(treeCount - 1));
            float width = Random.Range(0.45f, 1.1f);
            float height = Random.Range(worldHeight * 0.07f, worldHeight * 0.17f);
            float y = ground + Random.Range(-0.5f, 1.0f);
            List<Vector2> tree = new List<Vector2>
            {
                new Vector2(x - width * 0.5f, y),
                new Vector2(x - width * 0.18f, y + height * 0.52f),
                new Vector2(x - width * 0.34f, y + height * 0.52f),
                new Vector2(x, y + height),
                new Vector2(x + width * 0.34f, y + height * 0.48f),
                new Vector2(x + width * 0.16f, y + height * 0.48f),
                new Vector2(x + width * 0.5f, y),
            };
            AddPolygon(parent, "Tree_" + i, tree, Color.Lerp(ForestColor, NearMountainColor, Random.Range(0f, 0.18f)));
        }

        // A continuous ground strip hides small gaps at the bottom.
        AddQuad(parent, "ForestGround", new Vector2(-worldWidth, ground - 1f),
            new Vector2(worldWidth, ground + 0.8f), ForestColor);
        Random.state = oldState;
    }

    private void AddClouds(Transform parent, int count)
    {
        Random.State oldState = Random.state;
        Random.InitState(randomSeed + 4);

        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(-worldWidth * 0.52f, worldWidth * 0.52f);
            float y = Random.Range(centerY - worldHeight * 0.30f, centerY + worldHeight * 0.48f);
            float width = Random.Range(1.2f, 3.6f);
            float height = Random.Range(0.35f, 0.8f);
            AddCloudShape(parent, "Cloud_" + i, x, y, width, height, CloudColor);
        }

        Random.state = oldState;
    }

    private void AddCloudShape(Transform parent, string objectName, float x, float y, float width, float height, Color color)
    {
        // A stepped silhouette gives the cloud a soft, low-poly/pixel-inspired edge.
        float w = width * 0.5f;
        List<Vector2> points = new List<Vector2>
        {
            new Vector2(x - w, y), new Vector2(x - w * 0.78f, y + height * 0.55f),
            new Vector2(x - w * 0.48f, y + height * 0.55f), new Vector2(x - w * 0.30f, y + height),
            new Vector2(x + w * 0.12f, y + height * 0.92f), new Vector2(x + w * 0.30f, y + height * 0.55f),
            new Vector2(x + w * 0.68f, y + height * 0.55f), new Vector2(x + w, y + height * 0.25f),
            new Vector2(x + w * 0.86f, y), new Vector2(x - w, y)
        };
        AddPolygon(parent, objectName, points, color);
    }

    private void AddQuad(Transform parent, string objectName, Vector2 min, Vector2 max, Color color)
    {
        List<Vector2> points = new List<Vector2>
        {
            new Vector2(min.x, min.y), new Vector2(min.x, max.y),
            new Vector2(max.x, max.y), new Vector2(max.x, min.y)
        };
        AddPolygon(parent, objectName, points, color);
    }

    private void AddPolygon(Transform parent, string objectName, List<Vector2> points, Color color)
    {
        if (points == null || points.Count < 3) return;

        GameObject go = new GameObject(objectName);
        go.transform.SetParent(parent, false);
        Mesh mesh = new Mesh { name = objectName + "_Mesh" };
        Vector3[] vertices = new Vector3[points.Count];
        Color[] colors = new Color[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            vertices[i] = new Vector3(points[i].x, points[i].y, 0f);
            colors[i] = color;
        }

        // Triangle fan works for the simple, convex silhouettes used here.
        int[] triangles = new int[(points.Count - 2) * 3];
        for (int i = 0; i < points.Count - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        generatedMeshes.Add(mesh);

        MeshFilter filter = go.AddComponent<MeshFilter>();
        filter.sharedMesh = mesh;
        MeshRenderer renderer = go.AddComponent<MeshRenderer>();
        Material material = CreateUnlitMaterial();
        if (material != null)
        {
            material.color = Color.white;
            renderer.sharedMaterial = material;
            generatedMaterials.Add(material);
        }
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    private Material CreateUnlitMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Unlit/Color");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null)
        {
            Debug.LogWarning("PedaLi backdrop could not find an unlit shader. Assign a compatible material to generated MeshRenderers.");
            return null;
        }

        return new Material(shader);
    }

    private void ClearGeneratedChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (Application.isPlaying) Destroy(child);
            else DestroyImmediate(child);
        }

        foreach (Mesh mesh in generatedMeshes)
        {
            if (mesh == null) continue;
            if (Application.isPlaying) Destroy(mesh);
            else DestroyImmediate(mesh);
        }
        foreach (Material material in generatedMaterials)
        {
            if (material == null) continue;
            if (Application.isPlaying) Destroy(material);
            else DestroyImmediate(material);
        }
    }

    private void OnDestroy()
    {
        foreach (Mesh mesh in generatedMeshes)
            if (mesh != null) Destroy(mesh);
        foreach (Material material in generatedMaterials)
            if (material != null) Destroy(material);
    }
}
