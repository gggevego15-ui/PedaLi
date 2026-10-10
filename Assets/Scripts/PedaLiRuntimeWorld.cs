using UnityEngine;

/// <summary>Displays the current climb height in the runtime-generated gameplay.</summary>
public sealed class PedaLiRuntimeWorld : MonoBehaviour
{
    private void OnGUI()
    {
        PedaLiController player = FindObjectOfType<PedaLiController>();
        if (player == null) return;
        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(10f, 18f, Screen.width - 20f, 34f),
            "HEIGHT  " + Mathf.Max(0, Mathf.RoundToInt(player.transform.position.y)) + " m", style);
    }
}
