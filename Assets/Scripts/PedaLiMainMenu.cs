using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Drop-in procedural PedaLi main menu. It builds its own portrait UI and illustrated
/// background, so no sprites, fonts, prefabs, or manual Canvas setup are required.
/// It auto-shows on scenes that do not contain a PedaLiController.
/// </summary>
public sealed class PedaLiMainMenu : MonoBehaviour
{
    private static PedaLiMainMenu instance;
    private Canvas canvas;
    private Font font;
    private Image background;
    private Text biomeText;
    private Text statusText;
    private GameObject modal;
    private int biomeIndex;
    private readonly string[] biomeNames = { "JUNGLE", "MOUNTAINS", "SNOW PEAKS" };
    private readonly Color[] skyTop =
    {
        new Color(0.035f, 0.22f, 0.22f), new Color(0.08f, 0.19f, 0.36f), new Color(0.25f, 0.48f, 0.70f)
    };
    private readonly Color[] skyBottom =
    {
        new Color(0.34f, 0.67f, 0.49f), new Color(0.72f, 0.69f, 0.49f), new Color(0.83f, 0.91f, 0.98f)
    };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void TryAutoCreate()
    {
        if (instance != null) return;
        // A scene with the player is a gameplay scene; never cover it with the menu.
        if (FindObjectOfType<PedaLiController>() != null) return;
        var go = new GameObject("PedaLi Main Menu (Auto)");
        instance = go.AddComponent<PedaLiMainMenu>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        biomeIndex = Mathf.Clamp(PlayerPrefs.GetInt("PedaLi.MenuBiome", 0), 0, biomeNames.Length - 1);
        BuildUI();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (instance == this) instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (FindObjectOfType<PedaLiController>() != null) Destroy(gameObject);
    }

    private void BuildUI()
    {
        font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        var canvasGO = new GameObject("PedaLi Menu Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasGO.transform.SetParent(transform, false);
        canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
        {
            var es = new GameObject("PedaLi UI EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
            es.transform.SetParent(transform, false);
        }

        background = MakeImage("Illustrated Biome Background", canvasGO.transform, Full, CreateLandscapeTexture(biomeIndex), Color.white);
        AddSoftGlow(canvasGO.transform);
        AddText(canvasGO.transform, "PedaLi Wordmark", "PedaLi", new Vector2(0.5f, 0.79f), new Vector2(0.5f, 0.79f), new Vector2(0, 0), new Vector2(850, 170), 112, new Color(1f, 0.97f, 0.83f), FontStyle.Bold, TextAnchor.MiddleCenter);
        AddText(canvasGO.transform, "Tagline", "ONE CLIMB. THREE WORLDS.", new Vector2(0.5f, 0.745f), new Vector2(0.5f, 0.745f), Vector2.zero, new Vector2(800, 56), 27, new Color(0.92f, 1f, 0.93f), FontStyle.Bold, TextAnchor.MiddleCenter);

        var badge = MakePanel("Biome Badge", canvasGO.transform, new Vector2(0.5f, 0.635f), new Vector2(0.5f, 0.635f), new Vector2(0, 0), new Vector2(460, 78), new Color(0.02f, 0.10f, 0.14f, 0.78f));
        biomeText = AddText(badge.transform, "Biome Name", biomeNames[biomeIndex], Full, Full, Vector2.zero, Vector2.zero, 25, Color.white, FontStyle.Bold, TextAnchor.MiddleCenter);

        MakeButton(canvasGO.transform, "Climb Button", "START CLIMBING  ↗", new Vector2(0.5f, 0.49f), new Vector2(0.5f, 0.49f), new Vector2(0, 0), new Vector2(650, 112), new Color(1f, 0.72f, 0.25f), new Color(0.16f, 0.12f, 0.07f), StartGame);
        MakeButton(canvasGO.transform, "Biome Button", "CHOOSE YOUR WORLD", new Vector2(0.5f, 0.405f), new Vector2(0.5f, 0.405f), Vector2.zero, new Vector2(650, 92), new Color(0.05f, 0.18f, 0.21f, 0.88f), Color.white, CycleBiome);
        MakeButton(canvasGO.transform, "Settings Button", "SETTINGS  ⚙", new Vector2(0.5f, 0.33f), new Vector2(0.5f, 0.33f), Vector2.zero, new Vector2(650, 92), new Color(0.05f, 0.18f, 0.21f, 0.76f), Color.white, ShowSettings);

        AddText(canvasGO.transform, "World Route", "JUNGLE     •     MOUNTAINS     •     SNOW PEAKS", new Vector2(0.5f, 0.135f), new Vector2(0.5f, 0.135f), Vector2.zero, new Vector2(940, 60), 22, new Color(1f, 1f, 1f, 0.93f), FontStyle.Bold, TextAnchor.MiddleCenter);
        AddText(canvasGO.transform, "Footer", "CLIMB HIGHER. DISCOVER WHAT'S ABOVE.", new Vector2(0.5f, 0.095f), new Vector2(0.5f, 0.095f), Vector2.zero, new Vector2(900, 46), 19, new Color(0.9f, 0.97f, 1f, 0.9f), FontStyle.Normal, TextAnchor.MiddleCenter);
        statusText = AddText(canvasGO.transform, "Status", "", new Vector2(0.5f, 0.235f), new Vector2(0.5f, 0.235f), Vector2.zero, new Vector2(900, 90), 23, Color.white, FontStyle.Bold, TextAnchor.MiddleCenter);
    }

    private static readonly Vector2 FullMin = Vector2.zero;
    private static readonly Vector2 FullMax = Vector2.one;
    private static readonly RectTransform Full = null;

    private void AddSoftGlow(Transform parent)
    {
        var glow = MakeImage("Sun Glow", parent, null, CreateCircleTexture(256), new Color(1f, 0.87f, 0.56f, 0.22f));
        var rt = glow.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.78f, 0.70f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(520, 520);
        glow.transform.SetAsLastSibling();
        // Keep the glow behind menu copy and controls.
        glow.transform.SetSiblingIndex(1);
    }

    private void CycleBiome()
    {
        biomeIndex = (biomeIndex + 1) % biomeNames.Length;
        PlayerPrefs.SetInt("PedaLi.MenuBiome", biomeIndex);
        biomeText.text = biomeNames[biomeIndex];
        background.sprite = Sprite.Create(CreateLandscapeTexture(biomeIndex), new Rect(0, 0, 512, 1024), new Vector2(0.5f, 0.5f), 100);
    }

    private void StartGame()
    {
        string current = SceneManager.GetActiveScene().path;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (!string.IsNullOrEmpty(path) && path != current &&
                !name.ToLowerInvariant().Contains("menu") && !name.ToLowerInvariant().Contains("title"))
            {
                SceneManager.LoadScene(i);
                return;
            }
        }
        ShowMessage("YOUR CLIMB IS ALMOST READY", "Add your gameplay scene to File → Build Settings / Build Profiles. The menu is ready; the game scene will open here once included.");
    }

    private void ShowSettings()
    {
        ShowMessage("SETTINGS", "Choose your preferences below. Changes are saved automatically.");
        var message = modal.transform.Find("Modal Message");
        if (message != null)
        {
            var rt = message.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.68f);
            rt.sizeDelta = new Vector2(700, 90);
        }
        AddButtonToModal("Toggle Music: " + (PlayerPrefs.GetInt("PedaLi.Music", 1) == 1 ? "ON" : "OFF"), new Vector2(0, -45), () => { PlayerPrefs.SetInt("PedaLi.Music", 1 - PlayerPrefs.GetInt("PedaLi.Music", 1)); ShowSettings(); });
        AddButtonToModal("Toggle Vibration: " + (PlayerPrefs.GetInt("PedaLi.Vibration", 1) == 1 ? "ON" : "OFF"), new Vector2(0, -125), () => { PlayerPrefs.SetInt("PedaLi.Vibration", 1 - PlayerPrefs.GetInt("PedaLi.Vibration", 1)); ShowSettings(); });
    }

    private void ShowMessage(string title, string message)
    {
        if (modal != null) Destroy(modal);
        modal = MakePanel("Menu Modal", canvas.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(850, 610), new Color(0.025f, 0.09f, 0.14f, 0.97f)).gameObject;
        AddText(modal.transform, "Modal Title", title, new Vector2(0.5f, 0.78f), new Vector2(0.5f, 0.78f), Vector2.zero, new Vector2(760, 90), 34, new Color(1f, 0.78f, 0.35f), FontStyle.Bold, TextAnchor.MiddleCenter);
        AddText(modal.transform, "Modal Message", message, new Vector2(0.5f, 0.48f), new Vector2(0.5f, 0.48f), Vector2.zero, new Vector2(740, 250), 24, Color.white, FontStyle.Normal, TextAnchor.MiddleCenter);
        MakeButton(modal.transform, "Modal Close", "OK / CLOSE", new Vector2(0.5f, 0.13f), new Vector2(0.5f, 0.13f), Vector2.zero, new Vector2(400, 78), new Color(1f, 0.72f, 0.25f), new Color(0.12f, 0.1f, 0.05f), () => { if (modal != null) Destroy(modal); modal = null; });
        if (statusText != null) statusText.text = "";
    }

    private void AddButtonToModal(string label, Vector2 position, Action action)
    {
        if (modal == null) return;
        MakeButton(modal.transform, label, label, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, new Vector2(330, 64), new Color(0.13f, 0.34f, 0.36f), Color.white, action);
    }

    private Button MakeButton(Transform parent, string name, string label, Vector2 amin, Vector2 amax, Vector2 pos, Vector2 size, Color color, Color textColor, Action action)
    {
        var panel = MakePanel(name, parent, amin, amax, pos, size, color);
        var button = panel.gameObject.AddComponent<Button>();
        button.targetGraphic = panel;
        var colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.14f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.14f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;
        button.onClick.AddListener(() => action());
        AddText(panel.transform, name + " Label", label, FullMin, FullMax, Vector2.zero, Vector2.zero, 27, textColor, FontStyle.Bold, TextAnchor.MiddleCenter);
        return button;
    }

    private Image MakePanel(string name, Transform parent, Vector2 amin, Vector2 amax, Vector2 pos, Vector2 size, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = amin; rt.anchorMax = amax; rt.anchoredPosition = pos; rt.sizeDelta = size;
        var image = go.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private Image MakeImage(string name, Transform parent, RectTransform unused, Texture2D texture, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = FullMin; rt.anchorMax = FullMax; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        var img = go.GetComponent<Image>();
        img.color = color;
        img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);
        img.type = Image.Type.Simple;
        return img;
    }

    private Text AddText(Transform parent, string name, string value, Vector2 amin, Vector2 amax, Vector2 pos, Vector2 size, int fontSize, Color color, FontStyle style, TextAnchor align)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = amin; rt.anchorMax = amax; rt.anchoredPosition = pos; rt.sizeDelta = size;
        var text = go.GetComponent<Text>();
        text.font = font;
        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.fontStyle = style;
        text.alignment = align;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = Mathf.Max(14, fontSize - 8);
        text.resizeTextMaxSize = fontSize;
        return text;
    }

    private Texture2D CreateLandscapeTexture(int index)
    {
        const int w = 512, h = 1024;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        Color top = skyTop[index], bottom = skyBottom[index];
        var pixels = new Color[w * h];
        float ridge1 = index == 0 ? 0.30f : (index == 1 ? 0.40f : 0.45f);
        float ridge2 = index == 0 ? 0.20f : (index == 1 ? 0.28f : 0.34f);
        Color far = index == 0 ? new Color(0.20f, 0.43f, 0.48f) : (index == 1 ? new Color(0.38f, 0.49f, 0.61f) : new Color(0.66f, 0.79f, 0.88f));
        Color mid = index == 0 ? new Color(0.09f, 0.30f, 0.27f) : (index == 1 ? new Color(0.31f, 0.35f, 0.36f) : new Color(0.49f, 0.62f, 0.72f));
        Color near = index == 0 ? new Color(0.035f, 0.16f, 0.15f) : (index == 1 ? new Color(0.16f, 0.23f, 0.27f) : new Color(0.30f, 0.43f, 0.54f));
        for (int y = 0; y < h; y++)
        {
            float t = y / (float)(h - 1);
            Color sky = Color.Lerp(bottom, top, t);
            for (int x = 0; x < w; x++)
            {
                float nx = x / (float)w;
                float waveA = Mathf.PerlinNoise(nx * 5.4f, 0.18f) * 0.07f;
                float waveB = Mathf.PerlinNoise(nx * 8.3f, 1.7f) * 0.08f;
                float ridgeFar = ridge1 + waveA + Mathf.Sin(nx * 15f) * 0.025f;
                float ridgeMid = ridge2 + waveB + Mathf.Sin(nx * 20f + 1f) * 0.02f;
                Color c = sky;
                if (t < ridgeFar) c = far;
                if (t < ridgeMid) c = mid;
                if (t < ridge2 * 0.58f + Mathf.PerlinNoise(nx * 12f, 3.1f) * 0.055f) c = near;
                if (index == 2 && t < ridgeFar && Mathf.Sin(nx * 28f + t * 6f) > 0.35f)
                    c = Color.Lerp(c, Color.white, 0.75f); // snow highlights
                pixels[y * w + x] = c;
            }
        }
        tex.SetPixels(pixels);
        tex.Apply(false, true);
        return tex;
    }

    private Texture2D CreateCircleTexture(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float d = Vector2.Distance(new Vector2(x, y), new Vector2(size * 0.5f, size * 0.5f)) / (size * 0.5f);
            float a = Mathf.Clamp01((1f - d) * 2.0f);
            pixels[y * size + x] = new Color(1f, 0.9f, 0.62f, a * a);
        }
        tex.SetPixels(pixels); tex.Apply(false, true); return tex;
    }
}