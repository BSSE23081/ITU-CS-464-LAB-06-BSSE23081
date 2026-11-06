using System;
using System.Collections;
using UnityEngine;

// Builds all five levels at runtime from data, owns score, lives, timer, win/lose and a simple OnGUI HUD.
// Attach this to ONE empty GameObject in an empty scene. Nothing else is needed.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ---------- level data (units are the 960x540 layout; divided by U when built) ----------
    class Gd { public float[][] p; public PatrolMode m; public float v, w, ch; }
    class Cr { public float x, y, w, h, rest, warn; }
    class Sw { public float x, y, w, h, dx, dy, d, v; }
    class Pu { public PowerUpType k; public float x, y; }
    class Level
    {
        public string name, obj, kind; public float time; public float[] start, exit;
        public float[][] walls = new float[0][], coins = new float[0][], spikes = new float[0][];
        public Gd[] guards = new Gd[0]; public Cr[] crushers = new Cr[0]; public Sw[] sweepers = new Sw[0]; public Pu[] pickups = new Pu[0];
    }
    static float[] F(params float[] a) => a;
    static float[][] A(params float[][] a) => a;

    static readonly Level[] Levels =
    {
        new Level { name = "Warm-up", kind = "coins", start = F(70, 270),
            obj = "Collect all 6 coins. One slow guard, a few spikes.",
            walls = A(F(440, 150, 80, 240)),
            coins = A(F(200, 80), F(480, 80), F(820, 80), F(820, 460), F(480, 460), F(200, 460)),
            guards = new[] { new Gd { p = A(F(370, 100), F(590, 100), F(590, 440), F(370, 440)), m = PatrolMode.Loop, v = 80 } },
            spikes = A(F(240, 230, 50, 80)),
            pickups = new[] { new Pu { k = PowerUpType.Shield, x = 150, y = 450 } } },

        new Level { name = "Beat the clock", kind = "exit", time = 35, start = F(70, 270), exit = F(890, 450),
            obj = "Reach the exit within 35 seconds. Weave through the gaps.",
            walls = A(F(200, 0, 30, 360), F(420, 180, 30, 360), F(640, 0, 30, 360)),
            guards = new[] { new Gd { p = A(F(330, 90), F(540, 90)), m = PatrolMode.PingPong, v = 90 } },
            crushers = new[] { new Cr { x = 150, y = 400, w = 130, h = 100, rest = 2.4f, warn = 0.7f } },
            spikes = A(F(600, 430, 90, 70)),
            pickups = new[] { new Pu { k = PowerUpType.Speed, x = 330, y = 450 } } },

        new Level { name = "Coin run", kind = "coins", start = F(70, 270),
            obj = "Collect all 9 coins. Two guards, and a crusher guards the middle coin.",
            walls = A(F(300, 120, 360, 24), F(300, 396, 360, 24)),
            coins = A(F(90, 80), F(90, 460), F(250, 270), F(480, 60), F(480, 480), F(480, 270), F(710, 270), F(880, 80), F(880, 460)),
            guards = new[] {
                new Gd { p = A(F(250, 200), F(710, 200), F(710, 340), F(250, 340)), m = PatrolMode.Loop, v = 100 },
                new Gd { p = A(F(130, 60), F(130, 480)), m = PatrolMode.PingPong, v = 110 } },
            crushers = new[] { new Cr { x = 440, y = 230, w = 80, h = 80, rest = 2.4f, warn = 0.6f } },
            spikes = A(F(780, 230, 60, 80)),
            pickups = new[] { new Pu { k = PowerUpType.Shield, x = 600, y = 300 }, new Pu { k = PowerUpType.Speed, x = 380, y = 450 } } },

        new Level { name = "Slip past", kind = "exit", start = F(60, 270), exit = F(915, 50),
            obj = "Reach the exit. Guards chase when they see you; outrun them. A crusher blocks the last door.",
            walls = A(F(240, 0, 24, 200), F(240, 300, 24, 240), F(480, 0, 24, 120), F(480, 220, 24, 320), F(720, 0, 24, 300), F(720, 400, 24, 140)),
            guards = new[] {
                new Gd { p = A(F(340, 90), F(340, 450)), m = PatrolMode.PingPong, v = 90, ch = 120 },
                new Gd { p = A(F(570, 80), F(670, 80), F(670, 470), F(570, 470)), m = PatrolMode.Loop, v = 95, ch = 125 },
                new Gd { p = A(F(800, 120), F(900, 210), F(830, 350), F(900, 430)), m = PatrolMode.Random, v = 90, w = 0.8f, ch = 130 } },
            crushers = new[] { new Cr { x = 700, y = 305, w = 64, h = 90, rest = 2.2f, warn = 0.6f } },
            pickups = new[] { new Pu { k = PowerUpType.Shield, x = 380, y = 480 } } },

        new Level { name = "Gauntlet", kind = "both", time = 80, start = F(60, 270), exit = F(900, 270),
            obj = "Collect all 8 coins, then reach the exit within 80 seconds. Everything is out to get you.",
            walls = A(F(200, 90, 26, 130), F(200, 320, 26, 130), F(420, 0, 26, 150), F(420, 390, 26, 150), F(640, 90, 26, 130), F(640, 320, 26, 130)),
            coins = A(F(110, 60), F(110, 480), F(370, 270), F(540, 270), F(540, 60), F(540, 480), F(860, 100), F(860, 440)),
            guards = new[] {
                new Gd { p = A(F(480, 160), F(600, 160), F(600, 380), F(480, 380)), m = PatrolMode.Loop, v = 105, ch = 95 },
                new Gd { p = A(F(800, 60), F(800, 480)), m = PatrolMode.PingPong, v = 110, ch = 95 },
                new Gd { p = A(F(290, 60), F(380, 200), F(380, 400), F(290, 480)), m = PatrolMode.Random, v = 100, w = 0.5f, ch = 90 } },
            crushers = new[] { new Cr { x = 500, y = 235, w = 80, h = 70, rest = 2.2f, warn = 0.6f } },
            spikes = A(F(250, 235, 40, 70), F(880, 380, 40, 60)),
            sweepers = new[] { new Sw { x = 720, y = 200, w = 26, h = 90, dx = 0, dy = 1, d = 150, v = 90 } },
            pickups = new[] {
                new Pu { k = PowerUpType.Speed, x = 160, y = 270 }, new Pu { k = PowerUpType.Shield, x = 350, y = 330 },
                new Pu { k = PowerUpType.Magnet, x = 700, y = 500 } } },
    };

    // ---------- state ----------
    const float U = 30f, ScreenH = 540f;
    enum State { Menu, Intro, Play, Win, Lose }
    State state; int lv, score, need, lives; float elapsed, total;
    string title, body, button; Action act;
    Transform root; Health playerHealth; Vector3 spawnPos; Renderer exitRend;
    public Transform PlayerT { get; private set; }
    public bool PlayerAlive => PlayerT != null && PlayerT.gameObject.activeInHierarchy;
    public bool ExitUnlocked { get; private set; }

    static readonly Color Teal = new Color(.33f, .84f, .76f), Pink = new Color(1f, .31f, .64f), Orange = new Color(1f, .54f, .24f),
        Gold = new Color(1f, .82f, .29f), Wall = new Color(.35f, .43f, .55f), Floor = new Color(.12f, .14f, .17f), Grey = new Color(.35f, .38f, .43f);

    void Awake()
    {
        Instance = this;
        Camera cam = Camera.main;
        if (cam == null) { var go = new GameObject("Main Camera"); go.tag = "MainCamera"; cam = go.AddComponent<Camera>(); go.AddComponent<AudioListener>(); }
        cam.transform.position = new Vector3(16f, 21f, 9f);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color(.08f, .09f, .11f);
        if (FindFirstObjectByType<Light>() == null)
        {
            var l = new GameObject("Light").AddComponent<Light>(); l.type = LightType.Directional;
            l.transform.rotation = Quaternion.Euler(70f, 20f, 0f);
        }
        LoadLevel(0);
        Panel("Survive the Arena", "Teal ball vs the arena: guards patrol, spikes hurt, crushers warn then strike, power-ups help for a while.\n" +
              "Five levels, each with a different goal.\n\nWASD / arrows move  |  R restart level  |  1-5 jump to a level", "Play", () => LoadLevel(0));
        state = State.Menu;
    }

    // ---------- building ----------
    static Vector3 Pos(float x, float y, float h) => new Vector3(x / U, h, (ScreenH - y) / U);

    GameObject Box(string n, float x, float y, float w, float h, float height, Color c, bool trigger)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Cube); g.name = n; g.transform.SetParent(root);
        g.transform.position = Pos(x + w / 2f, y + h / 2f, height / 2f);
        g.transform.localScale = new Vector3(w / U, height, h / U);
        g.GetComponent<Renderer>().material.color = c;
        if (trigger) g.GetComponent<Collider>().isTrigger = true;
        return g;
    }

    GameObject Ball(string n, PrimitiveType t, Vector3 pos, float size, Color c, bool trigger)
    {
        var g = GameObject.CreatePrimitive(t); g.name = n; g.transform.SetParent(root);
        g.transform.position = pos; g.transform.localScale = Vector3.one * size;
        g.GetComponent<Renderer>().material.color = c;
        if (trigger) g.GetComponent<Collider>().isTrigger = true;
        return g;
    }

    void LoadLevel(int i)
    {
        StopAllCoroutines();
        lv = i; Level d = Levels[i];
        if (root != null) Destroy(root.gameObject);
        root = new GameObject("Level").transform;
        Coin.All.Clear();
        score = 0; lives = 3; elapsed = 0f; need = d.coins.Length; ExitUnlocked = d.kind == "exit"; exitRend = null;

        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube); floor.name = "Floor"; floor.transform.SetParent(root);
        floor.transform.position = new Vector3(16f, -.05f, 9f); floor.transform.localScale = new Vector3(32f, .1f, 18f);
        floor.GetComponent<Renderer>().material.color = Floor;
        Box("Border", -30, -30, 1020, 30, 1, Wall, false); Box("Border", -30, 540, 1020, 30, 1, Wall, false);
        Box("Border", -30, -30, 30, 600, 1, Wall, false);  Box("Border", 960, -30, 30, 600, 1, Wall, false);
        foreach (var r in d.walls) Box("Wall", r[0], r[1], r[2], r[3], 1f, Wall, false);

        foreach (var r in d.spikes) Box("Spikes", r[0], r[1], r[2], r[3], .25f, Pink, true).AddComponent<Hazard>();
        foreach (var c in d.crushers)
        {
            var h = Box("Crusher", c.x, c.y, c.w, c.h, .6f, Orange, true).AddComponent<Hazard>();
            h.damage = 2; h.restTime = c.rest; h.warnTime = c.warn; h.strikeTime = .4f;
        }
        foreach (var s in d.sweepers)
        {
            var o = Box("Sweeper", s.x, s.y, s.w, s.h, 1f, Pink, true);
            o.AddComponent<Rigidbody>().isKinematic = true;
            o.AddComponent<Hazard>();
            var m = o.AddComponent<MovingHazard>(); m.direction = new Vector3(s.dx, 0f, -s.dy); m.distance = s.d / U; m.speed = s.v / U;
        }
        foreach (var c in d.coins) Ball("Coin", PrimitiveType.Sphere, Pos(c[0], c[1], .35f), .5f, Gold, true).AddComponent<Coin>();
        foreach (var u in d.pickups)
        {
            Color col = u.k == PowerUpType.Speed ? Teal : u.k == PowerUpType.Shield ? new Color(.56f, .64f, 1f) : new Color(.77f, .55f, 1f);
            Ball("PowerUp " + u.k, PrimitiveType.Sphere, Pos(u.x, u.y, .4f), .6f, col, true).AddComponent<PowerUp>().type = u.k;
        }
        if (d.exit != null)
        {
            var e = Ball("Exit", PrimitiveType.Cylinder, Pos(d.exit[0], d.exit[1], .25f), 1.6f, Grey, true);
            e.transform.localScale = new Vector3(1.6f, .25f, 1.6f);
            e.AddComponent<ExitZone>(); exitRend = e.GetComponent<Renderer>();
        }

        var p = Ball("Player", PrimitiveType.Sphere, Pos(d.start[0], d.start[1], .4f), .8f, Teal, false);
        p.tag = "Player";
        var rb = p.AddComponent<Rigidbody>(); rb.linearDamping = 4f; rb.constraints = RigidbodyConstraints.FreezePositionY;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        p.AddComponent<PlayerController>(); playerHealth = p.AddComponent<Health>();
        PlayerT = p.transform; spawnPos = p.transform.position;

        foreach (var g in d.guards)   // after the player exists, so EnemyPatrol can find it
        {
            var pts = new Vector3[g.p.Length];
            for (int k = 0; k < pts.Length; k++) pts[k] = Pos(g.p[k][0], g.p[k][1], .45f);
            var o = Ball("Guard", PrimitiveType.Sphere, pts[0], .9f, Orange, true);
            o.AddComponent<Rigidbody>().isKinematic = true;
            o.AddComponent<Hazard>();
            var e = o.AddComponent<EnemyPatrol>(); e.points = pts; e.mode = g.m; e.speed = g.v / U; e.waitTime = g.w; e.sight = g.ch / U;
        }

        Panel("Level " + (i + 1) + " of 5 - " + d.name, d.obj + "\n3 hearts, 3 lives", "Start", () => { state = State.Play; Time.timeScale = 1f; });
        state = State.Intro;
    }

    // ---------- game rules ----------
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) LoadLevel(lv);
        for (int i = 0; i < Levels.Length; i++) if (Input.GetKeyDown(KeyCode.Alpha1 + i)) LoadLevel(i);
        if (state != State.Play) { if (Input.GetKeyDown(KeyCode.Return) && act != null) act(); return; }
        elapsed += Time.deltaTime;
        if (Levels[lv].time > 0f && elapsed >= Levels[lv].time) Lose("Time's up");
        if (exitRend != null) exitRend.material.color = ExitUnlocked ? Teal : Grey;
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (score >= need) { ExitUnlocked = true; if (Levels[lv].kind == "coins") Win(); }
    }

    public void PlayerDied()
    {
        lives--;
        if (lives <= 0) Lose("Out of lives");
        else StartCoroutine(Respawn());
    }

    // Wait a beat so a queued knockback can't hit the respawned player, then zero the velocity.
    IEnumerator Respawn()
    {
        PlayerT.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.6f);
        PlayerT.position = spawnPos;
        PlayerT.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        PlayerT.gameObject.SetActive(true);
        playerHealth.Restore(1.5f);
    }

    public void Win()
    {
        if (state != State.Play) return;
        total += elapsed; state = State.Win; Time.timeScale = 0f;
        if (lv == Levels.Length - 1) Panel("All 5 levels cleared!", "Total time " + total.ToString("F1") + " s. You survived the arena.", "Play again", () => { total = 0f; LoadLevel(0); });
        else Panel("Level " + (lv + 1) + " cleared!", "Time " + elapsed.ToString("F1") + " s", "Next level", () => LoadLevel(lv + 1));
    }

    void Lose(string why)
    {
        if (state != State.Play) return;
        state = State.Lose; Time.timeScale = 0f;
        Panel("Game over", why + ".\nLevel " + (lv + 1) + ": " + Levels[lv].name, "Retry level", () => LoadLevel(lv));
    }

    void Panel(string t, string b, string btn, Action a) { title = t; body = b; button = btn; act = a; Time.timeScale = 0f; }

    // ---------- UI ----------
    void OnGUI()
    {
        float s = Screen.height / 540f;
        var lbl = new GUIStyle(GUI.skin.label) { fontSize = (int)(18 * s), wordWrap = true, alignment = TextAnchor.UpperCenter };
        if (state == State.Play)
        {
            lbl.alignment = TextAnchor.UpperLeft; lbl.normal.textColor = Color.white;
            string hud = "L" + (lv + 1) + " " + Levels[lv].name + "    HP " + Mathf.Max(playerHealth.Current, 0) + "/" + playerHealth.Max + "    Lives x" + lives;
            if (need > 0) hud += "    Coins " + score + "/" + need;
            hud += Levels[lv].time > 0f ? "    Time " + Mathf.Max(Levels[lv].time - elapsed, 0f).ToString("F1") + " s" : "    Time " + elapsed.ToString("F1") + " s";
            if (playerHealth.shielded) hud += "    SHIELD";
            var pc = PlayerT.GetComponent<PlayerController>();
            if (pc.Boosted) hud += "    SPEED";
            if (pc.Magnetic) hud += "    MAGNET";
            GUI.Label(new Rect(10 * s, 6 * s, Screen.width, 30 * s), hud, lbl);
            return;
        }
        float w = Screen.width * .6f, h = Screen.height * .55f;
        var r = new Rect((Screen.width - w) / 2f, (Screen.height - h) / 2f, w, h);
        GUI.Box(r, "");
        var big = new GUIStyle(lbl) { fontSize = (int)(30 * s), fontStyle = FontStyle.Bold };
        GUI.Label(new Rect(r.x + 10, r.y + 20 * s, w - 20, 50 * s), title, big);
        GUI.Label(new Rect(r.x + 20, r.y + 80 * s, w - 40, h - 160 * s), body, lbl);
        var bs = new GUIStyle(GUI.skin.button) { fontSize = (int)(20 * s) };
        if (GUI.Button(new Rect(r.center.x - 90 * s, r.yMax - 60 * s, 180 * s, 44 * s), button, bs) && act != null) act();
    }
}