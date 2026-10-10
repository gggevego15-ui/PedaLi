using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Builds a playable PedaLi climbing run at runtime. No hand-authored scene or external art is required.
/// Launch from the main menu; Unity creates the player, platforms, backdrop, camera and touch controls.
/// </summary>
public static class PedaLiGameplayBootstrap
{
    private static bool launched;
    private static readonly Color Ink = new Color(0.075f, 0.12f, 0.15f);
    private static readonly Color Gold = new Color(1f, 0.72f, 0.24f);

    public static void Launch()
    {
        if (launched && UnityEngine.Object.FindObjectOfType<PedaLiController>() != null) return;
        launched = true;
        Time.timeScale = 1f;

        GameObject world = new GameObject("PedaLi Runtime Gameplay");
        world.AddComponent<PedaLiRuntimeWorld>();

        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject cameraGO = new GameObject("Main Camera");
            cameraGO.tag = "MainCamera";
            cam = cameraGO.AddComponent<Camera>();
            cameraGO.AddComponent<AudioListener>();
        }
        cam.orthographic = true;
        cam.orthographicSize = 8.5f;
        cam.transform.position = new Vector3(0f, 2f, -12f);
        cam.transform.rotation = Quaternion.identity;
        cam.backgroundColor = new Color(0.36f, 0.68f, 0.75f);
        cam.clearFlags = CameraClearFlags.SolidColor;

        GameObject player = new GameObject("PedaLi Climber");
        player.transform.position = new Vector3(0f, 1.2f, 0f);
        player.transform.localScale = Vector3.one;
        var character = player.AddComponent<CharacterController>();
        character.height = 1.45f;
        character.radius = 0.34f;
        character.center = new Vector3(0f, 0.73f, 0f);
        character.stepOffset = 0.25f;
        character.slopeLimit = 50f;
        var sprite = player.AddComponent<SpriteRenderer>();
        sprite.sprite = MakeClimberSprite(0, 0);
        sprite.sortingOrder = 20;
        sprite.material = new Material(Shader.Find("Sprites/Default"));
        var animator = player.AddComponent<PedaLiSpriteAnimator>();
        ConfigurePixelAnimations(animator);
        player.AddComponent<PedaLiController>();

        BuildPlatforms(world.transform);
        var cameraFollow = cam.gameObject.GetComponent<PedaLiVerticalCamera>();
        if (cameraFollow == null) cameraFollow = cam.gameObject.AddComponent<PedaLiVerticalCamera>();
        cameraFollow.SetTarget(player.transform);

        var backdropGO = new GameObject("PedaLi Nature Backdrop");
        backdropGO.transform.SetParent(world.transform, false);
        var backdrop = backdropGO.AddComponent<PedaLiNatureParallaxBuilder>();
        backdrop.Configure(player.transform, cameraFollow, 34f, 76f);

        BuildTouchUI(player.GetComponent<PedaLiController>());
        Debug.Log("PedaLi runtime gameplay launched. Move with A/D or arrows, jump with Space, grab with E/Shift.");
    }

    private static void BuildPlatforms(Transform parent)
    {
        CreatePlatform(parent, "Starting Ledge", new Vector3(0f, 0f, 0.35f), new Vector3(8.5f, 0.65f, 1.2f), new Color(0.18f, 0.40f, 0.29f), true);
        float[] xs = { -1.4f, 1.0f, -0.6f, 1.4f, 0f, -1.3f, 1.2f, -0.3f, 1.4f, -1.1f, 0.6f, -1.3f, 1.1f, -0.1f, 1.3f, -0.9f, 0.4f, -1.2f, 0.9f, -0.2f, 1.2f, -1.0f, 0.4f, -1.2f, 1.0f, 0f };
        for (int i = 0; i < 65; i++)
        {
            float x = xs[i % xs.Length];
            float y = 1.35f + i * 1.25f;
            float width = (i % 4 == 0) ? 2.7f : 2.25f;
            Color c = i < 20 ? new Color(0.27f, 0.52f, 0.31f) : i < 45 ? new Color(0.48f, 0.48f, 0.39f) : new Color(0.72f, 0.82f, 0.88f);
            CreatePlatform(parent, "Climb Platform " + (i + 1), new Vector3(x, y, 0.55f), new Vector3(width, 0.32f, 1.0f), c, false);
            if (i % 3 == 1)
                CreatePlatform(parent, "Rock Ledge " + i, new Vector3(x + (x > 0 ? -0.75f : 0.75f), y + 0.46f, 0.65f), new Vector3(0.55f, 0.18f, 0.8f), new Color(0.40f, 0.45f, 0.40f), false);
        }
    }

    private static void CreatePlatform(Transform parent, string name, Vector3 position, Vector3 scale, Color color, bool start)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.position = position;
        go.transform.localScale = scale;
        var renderer = go.GetComponent<Renderer>();
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        renderer.material = new Material(shader);
        renderer.material.color = color;
        if (start)
        {
            GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cube);
            top.name = "Start Ledge Highlight";
            top.transform.SetParent(parent, false);
            top.transform.position = position + new Vector3(0f, scale.y * 0.5f + 0.055f, -0.12f);
            top.transform.localScale = new Vector3(scale.x * 0.96f, 0.08f, scale.z * 1.05f);
            top.GetComponent<Renderer>().material = new Material(shader);
            top.GetComponent<Renderer>().material.color = Gold;
        }
    }

    private static void BuildTouchUI(PedaLiController controller)
    {
        var es = UnityEngine.Object.FindObjectOfType<EventSystem>();
        if (es == null)
        {
            var eventGO = new GameObject("PedaLi EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        var canvasGO = new GameObject("PedaLi Touch Controls", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 30;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;

        AddHudText(canvasGO.transform, "HEIGHT", "CLIMB HIGHER", new Vector2(0.5f, 0.94f), new Vector2(0.5f, 0.94f), Vector2.zero, new Vector2(560f, 70f), 27, Color.white);
        AddTouchButton(canvasGO.transform, controller, "LEFT", "◀", PedaLiMobileButton.ButtonAction.MoveLeft, new Vector2(0.12f, 0.115f), new Vector2(145f, 145f), new Color(0.04f, 0.15f, 0.19f, 0.82f));
        AddTouchButton(canvasGO.transform, controller, "RIGHT", "▶", PedaLiMobileButton.ButtonAction.MoveRight, new Vector2(0.29f, 0.115f), new Vector2(145f, 145f), new Color(0.04f, 0.15f, 0.19f, 0.82f));
        AddTouchButton(canvasGO.transform, controller, "GRAB", "HOLD", PedaLiMobileButton.ButtonAction.Grab, new Vector2(0.72f, 0.115f), new Vector2(160f, 145f), new Color(0.10f, 0.30f, 0.29f, 0.90f));
        AddTouchButton(canvasGO.transform, controller, "JUMP", "JUMP", PedaLiMobileButton.ButtonAction.Jump, new Vector2(0.90f, 0.115f), new Vector2(160f, 160f), Gold);
    }

    private static void AddTouchButton(Transform parent, PedaLiController controller, string name, string label, PedaLiMobileButton.ButtonAction action, Vector2 anchor, Vector2 size, Color color)
    {
        var go = new GameObject("Touch " + name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(PedaLiMobileButton));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;
        var image = go.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = true;
        var button = go.GetComponent<Button>();
        button.targetGraphic = image;
        var colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.2f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.2f);
        button.colors = colors;
        var mobile = go.GetComponent<PedaLiMobileButton>();
        typeof(PedaLiMobileButton).GetField("controller", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(mobile, controller);
        typeof(PedaLiMobileButton).GetField("action", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(mobile, action);
        AddHudText(go.transform, name + " Label", label, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 29, action == PedaLiMobileButton.ButtonAction.Jump ? Ink : Color.white);
    }

    private static Text AddHudText(Transform parent, string name, string value, Vector2 amin, Vector2 amax, Vector2 pos, Vector2 size, int fontSize, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = amin; rt.anchorMax = amax; rt.anchoredPosition = pos; rt.sizeDelta = size;
        var text = go.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.text = value; text.fontSize = fontSize; text.fontStyle = FontStyle.Bold;
        text.color = color; text.alignment = TextAnchor.MiddleCenter;
        text.resizeTextForBestFit = true; text.resizeTextMinSize = 14; text.resizeTextMaxSize = fontSize;
        text.raycastTarget = false;
        return text;
    }

    private static void ConfigurePixelAnimations(PedaLiSpriteAnimator animator)
    {
        SetClip(animator, "idle", MakeFrames(2, 0), 4f);
        SetClip(animator, "run", MakeFrames(4, 1), 10f);
        SetClip(animator, "jump", MakeFrames(2, 2), 7f);
        SetClip(animator, "grab", MakeFrames(2, 3), 4f);
        SetClip(animator, "climb", MakeFrames(4, 4), 8f);
        SetClip(animator, "fall", MakeFrames(2, 5), 5f);
    }

    private static void SetClip(PedaLiSpriteAnimator animator, string fieldName, Sprite[] frames, float fps)
    {
        FieldInfo field = typeof(PedaLiSpriteAnimator).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null) return;
        var clip = field.GetValue(animator) as PedaLiSpriteAnimator.SpriteAnimation;
        if (clip == null) clip = new PedaLiSpriteAnimator.SpriteAnimation();
        clip.frames = frames;
        clip.framesPerSecond = fps;
        clip.loop = fieldName != "jump";
        field.SetValue(animator, clip);
    }

    private static Sprite[] MakeFrames(int count, int state)
    {
        var frames = new Sprite[count];
        for (int i = 0; i < count; i++)
        {
            Texture2D texture = MakeClimberTexture(state, i);
            frames[i] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.05f), 24f);
            frames[i].name = "PedaLi_" + state + "_" + i;
        }
        return frames;
    }

    private static Sprite MakeClimberSprite(int state, int frame)
    {
        Texture2D texture = MakeClimberTexture(state, frame);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.05f), 24f);
    }

    private static Texture2D MakeClimberTexture(int state, int frame)
    {
        const int w = 24, h = 32;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        Color clear = new Color(0f, 0f, 0f, 0f);
        Color outline = new Color(0.08f, 0.10f, 0.13f);
        Color skin = new Color(1f, 0.72f, 0.48f);
        Color shirt = new Color(1f, 0.64f, 0.16f);
        Color pants = new Color(0.12f, 0.22f, 0.28f);
        Color boot = new Color(0.10f, 0.11f, 0.12f);
        Color[] pixels = new Color[w * h];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = clear;
        Action<int, int, int, int, Color> rect = (x0, y0, rw, rh, c) =>
        {
            for (int y = y0; y < y0 + rh; y++)
            for (int x = x0; x < x0 + rw; x++)
                if (x >= 0 && x < w && y >= 0 && y < h) pixels[y * w + x] = c;
        };
        int bob = state == 0 && frame == 1 ? 1 : 0;
        int legA = (state == 1 || state == 4) && frame % 2 == 0 ? 1 : 0;
        int legB = (state == 1 || state == 4) && frame % 2 != 0 ? 1 : 0;
        int armRaise = state == 2 || state == 3 || state == 4 ? 2 : 0;
        rect(8, 25 + bob, 9, 5, outline); rect(9, 25 + bob, 7, 5, skin);
        rect(7, 18 + bob, 11, 8, outline); rect(8, 18 + bob, 9, 7, shirt);
        rect(6, 19 + armRaise, 3, 7 - armRaise, outline); rect(6, 20 + armRaise, 2, 5 - armRaise, skin);
        rect(17, 19 + (armRaise > 0 ? -2 : 0), 3, 7, outline); rect(18, 20 + (armRaise > 0 ? -2 : 0), 2, 5, skin);
        rect(8, 10 + legA, 4, 9 - legA, outline); rect(9, 10 + legA, 2, 8 - legA, pants);
        rect(14, 10 + legB, 4, 9 - legB, outline); rect(15, 10 + legB, 2, 8 - legB, pants);
        rect(7 - legA, 7 + legA, 6, 3, boot); rect(13 + legB, 7 + legB, 6, 3, boot);
        rect(8, 29 + bob, 9, 3, outline); rect(9, 29 + bob, 7, 2, skin);
        rect(9, 30 + bob, 2, 1, outline); rect(15, 30 + bob, 2, 1, outline);
        tex.SetPixels(pixels); tex.Apply(false, true); return tex;
    }
}
