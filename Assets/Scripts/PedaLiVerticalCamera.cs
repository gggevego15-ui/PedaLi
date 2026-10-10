using UnityEngine;

/// <summary>
/// Smooth camera follow for PedaLi's vertical climb, with optional depth-based
/// parallax on scenery layers. Attach to the Main Camera.
/// </summary>
[DisallowMultipleComponent]
public class PedaLiVerticalCamera : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        [Tooltip("Transform for a background/foreground scenery layer.")]
        public Transform target;

        [Range(0f, 1f)]
        [Tooltip("How much the layer follows camera movement. 0 = fixed world layer; higher values create stronger parallax.")]
        public float followFactor = 0.2f;

        [Tooltip("If enabled, the layer follows camera X/Y only and keeps its starting Z.")]
        public bool ignoreDepth = true;

        [HideInInspector] public Vector3 initialPosition;
    }

    [Header("Follow target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 2f, -10f);
    [SerializeField, Min(0f)] private float smoothTime = 0.18f;
    [SerializeField] private bool followHorizontal = true;
    [SerializeField] private bool followVertical = true;
    [SerializeField] private bool lookAheadWhileClimbing = true;
    [SerializeField, Min(0f)] private float verticalLookAhead = 1.5f;
    [SerializeField, Min(0f)] private float lookAheadSmoothTime = 0.25f;
    [SerializeField] private Vector2 verticalLimits = new Vector2(-10000f, 10000f);

    [Header("Parallax layers")]
    [SerializeField] private ParallaxLayer[] parallaxLayers;

    private Vector3 velocity;
    private Vector3 startingCameraPosition;
    private float currentLookAhead;
    private float lookAheadVelocity;
    private Vector3[] layerStartPositions;

    private void Awake()
    {
        startingCameraPosition = transform.position;
        CacheLayerPositions();
    }

    private void OnEnable()
    {
        CacheLayerPositions();
    }

    private void CacheLayerPositions()
    {
        if (parallaxLayers == null) return;

        layerStartPositions = new Vector3[parallaxLayers.Length];
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            ParallaxLayer layer = parallaxLayers[i];
            if (layer == null || layer.target == null) continue;

            layer.initialPosition = layer.target.position;
            layerStartPositions[i] = layer.target.position;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position + targetOffset;
        if (!followHorizontal) desired.x = transform.position.x;
        if (!followVertical) desired.y = transform.position.y;

        if (lookAheadWhileClimbing && followVertical)
        {
            float verticalDirection = Mathf.Clamp(target.position.y - transform.position.y - targetOffset.y, -1f, 1f);
            float desiredLookAhead = verticalDirection > 0.15f ? verticalLookAhead : 0f;
            currentLookAhead = Mathf.SmoothDamp(currentLookAhead, desiredLookAhead,
                ref lookAheadVelocity, Mathf.Max(0.001f, lookAheadSmoothTime));
            desired.y += currentLookAhead;
        }

        desired.y = Mathf.Clamp(desired.y, verticalLimits.x, verticalLimits.y);
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity,
            Mathf.Max(0.001f, smoothTime));

        ApplyParallax();
    }

    private void ApplyParallax()
    {
        if (parallaxLayers == null || layerStartPositions == null) return;

        Vector3 cameraDelta = transform.position - startingCameraPosition;
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            ParallaxLayer layer = parallaxLayers[i];
            if (layer == null || layer.target == null) continue;

            Vector3 delta = cameraDelta * Mathf.Clamp01(layer.followFactor);
            if (layer.ignoreDepth) delta.z = 0f;
            layer.target.position = layerStartPositions[i] + delta;
        }
    }

    /// <summary>Set or replace the follow target at runtime.</summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    /// <summary>Reset the parallax reference positions after repositioning the scene.</summary>
    public void RecalibrateParallax()
    {
        startingCameraPosition = transform.position;
        CacheLayerPositions();
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(target.position + targetOffset, 0.2f);
    }
}
