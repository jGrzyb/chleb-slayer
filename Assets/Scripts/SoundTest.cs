using UnityEngine;

/// <summary>
/// Attach to any GameObject in a test scene.
/// Requires SoundManager to be present (with its AudioSources / AudioClips assigned).
/// </summary>
public class SoundTest : MonoBehaviour
{
    // ── Layout constants ────────────────────────────────────────────────
    private const int BtnW  = 220;
    private const int BtnH  = 40;
    private const int PadX  = 20;
    private const int PadY  = 10;
    private const int ColW  = BtnW + PadX;

    // ── GUI ─────────────────────────────────────────────────────────────
    private void OnGUI()
    {
        var sm = SoundManager.I;
        if (sm == null)
        {
            GUI.Label(new Rect(20, 20, 400, 30), "⚠  SoundManager not found in scene!");
            return;
        }

        int x = PadX;
        int y = PadY;

        // ── Column 1 : Music ────────────────────────────────────────────
        SectionLabel(ref x, ref y, "── MUSIC ──");

        if (Button(ref x, ref y, "Play Menu Music"))
            sm.PlayMusic(sm.MenuMusic);

        if (Button(ref x, ref y, "Play Level Music"))
            sm.PlayMusic(sm.LevelMusic);

        if (Button(ref x, ref y, "Play News Music"))
            sm.PlayMusic(sm.NewsMusic);

        if (Button(ref x, ref y, "Stop All Music"))
            sm.StopAllMusic();

        // ── Column 2 : Exclusive SFX ────────────────────────────────────
        x = PadX + ColW;
        y = PadY;

        SectionLabel(ref x, ref y, "── EXCLUSIVE SFX ──");
        GUI.Label(new Rect(x, y, BtnW, BtnH - 10),
            "<size=10><color=grey>Only one plays at a time (stops previous)</color></size>",
            new GUIStyle(GUI.skin.label) { richText = true });
        y += BtnH - 10;

        if (Button(ref x, ref y, "Player Hurt"))
            sm.PlayExclusive(sm.PlayerHurtSFX);

        if (Button(ref x, ref y, "Player Attack"))
            sm.PlayExclusive(sm.PlayerAttackSFX);

        // ── Column 3 : Overlapping / Pooled SFX ─────────────────────────
        x = PadX + ColW * 2;
        y = PadY;

        SectionLabel(ref x, ref y, "── OVERLAPPING SFX ──");
        GUI.Label(new Rect(x, y, BtnW, BtnH - 10),
            "<size=10><color=grey>Multiple instances can play simultaneously</color></size>",
            new GUIStyle(GUI.skin.label) { richText = true });
        y += BtnH - 10;

        if (Button(ref x, ref y, "Shoot"))
            sm.PlayShoot(sm.ShootClip);

        if (Button(ref x, ref y, "Shoot × 3 Rapid"))
        {
            sm.PlayShoot(sm.ShootClip);
            sm.PlayShoot(sm.ShootClip, 0.8f);
            sm.PlayShoot(sm.ShootClip, 0.6f);
        }

        if (Button(ref x, ref y, "Enemy Hurt"))
            sm.PlayEnemyHurt(sm.EnemyHurtClip);

        if (Button(ref x, ref y, "Enemy Hurt × 3 Rapid"))
        {
            sm.PlayEnemyHurt(sm.EnemyHurtClip);
            sm.PlayEnemyHurt(sm.EnemyHurtClip);
            sm.PlayEnemyHurt(sm.EnemyHurtClip);
        }

        if (Button(ref x, ref y, "Enemy Death"))
            sm.PlayEnemyDeath(sm.EnemyDeathClip);

        if (Button(ref x, ref y, "Enemy Death × 3 Rapid"))
        {
            sm.PlayEnemyDeath(sm.EnemyDeathClip);
            sm.PlayEnemyDeath(sm.EnemyDeathClip);
            sm.PlayEnemyDeath(sm.EnemyDeathClip);
        }

        if (Button(ref x, ref y, "Item Pickup"))
            sm.PlayOverlap(sm.ItemPickupClip);

        if (Button(ref x, ref y, "Item Pickup × 3 Rapid"))
        {
            sm.PlayOverlap(sm.ItemPickupClip);
            sm.PlayOverlap(sm.ItemPickupClip);
            sm.PlayOverlap(sm.ItemPickupClip);
        }

        if (Button(ref x, ref y, "Place Tower"))
            sm.PlayOverlap(sm.PlaceTowerClip);
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    private static bool Button(ref int x, ref int y, string label)
    {
        bool clicked = GUI.Button(new Rect(x, y, BtnW, BtnH), label);
        y += BtnH + PadY;
        return clicked;
    }

    private static void SectionLabel(ref int x, ref int y, string text)
    {
        var style = new GUIStyle(GUI.skin.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize  = 13
        };
        GUI.Label(new Rect(x, y, BtnW, BtnH), text, style);
        y += BtnH;
    }
}