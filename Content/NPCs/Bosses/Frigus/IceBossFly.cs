using Terraria;
using System.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Project165.Content.Dusts;
using Project165.Common.Systems;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.ItemDropRules;
using Project165.Content.Items.TreasureBags;
using Project165.Content.Projectiles.Hostile;
using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.Weapons.Magic;
using Project165.Content.Items.Weapons.Ranged;
using Terraria.Audio;
using Project165.Content.Items.Weapons.Summon;

namespace Project165.Content.NPCs.Bosses.Frigus;

[AutoloadBossHead]
public class IceBossFly : ModNPC
{
    public enum AIState : int
    {
        Spawning = -1,
        PhaseOne_Sideways = 0,
        PhaseOne_Static = 1,
        PhaseOne_Bottom = 2,
        DeathAnimation = 3,
    }

    public AIState CurrentAIState
    {
        get => (AIState)NPC.ai[0];
        set => NPC.ai[0] = (float)value;
    }

    public ref float Timer => ref NPC.ai[1];
    public ref float ShootCounter => ref NPC.localAI[0];
    public bool PhaseTwo
    {
        get => NPC.ai[2] == 1f;
        set => NPC.ai[2] = value ? 1f : 0f;
    }

    public float acceleration = 0.2f;

    public override void SetStaticDefaults()
    {
        NPCID.Sets.TrailingMode[Type] = 1;
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
        NPCID.Sets.NPCBestiaryDrawModifiers npcBestiaryDrawModifiers = new()
        {
            PortraitScale = 1.25f,
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, npcBestiaryDrawModifiers);
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        Main.npcFrameCount[Type] = 2;
    }

    public override void SetDefaults()
    {
        NPC.width = 110;
        NPC.height = 98;
        NPC.defense = 15;
        NPC.damage = 1;
        NPC.lifeMax = 25000;
        NPC.HitSound = SoundID.NPCHit5;
        NPC.DeathSound = SoundID.NPCDeath14;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.boss = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.netAlways = true;
        NPC.scale = 1.25f;
        NPC.value = 120000f;
        NPC.npcSlots = 5f;
        NPC.aiStyle = -1;
        Music = MusicID.FrostMoon;
    }

    private Player Player => Main.player[NPC.target];

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(
        [
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow
        ]);
    }

    public override void BossLoot(ref string name, ref int potionType)
    {
        potionType = ItemID.GreaterHealingPotion;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
        NPC.damage = (int)(NPC.damage * 0.6f);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        Conditions.NotExpert notExpertCondition = new();
        npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<IceBossBag>()));

        npcLoot.Add(ItemDropRule.ByCondition(notExpertCondition, ModContent.ItemType<Avalanche>(), 5));
        npcLoot.Add(ItemDropRule.ByCondition(notExpertCondition, ModContent.ItemType<IceShotgun>(), 5));
        npcLoot.Add(ItemDropRule.ByCondition(notExpertCondition, ModContent.ItemType<IceChakram>(), 5));
        npcLoot.Add(ItemDropRule.ByCondition(notExpertCondition, ModContent.ItemType<IceBat>(), 5));
        npcLoot.Add(ItemDropRule.ByCondition(notExpertCondition, ModContent.ItemType<IceGuardianStaff>(), 5));
    }

    #region AI

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(NPC.dontTakeDamage);
        writer.Write(NPC.localAI[0]);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        NPC.dontTakeDamage = reader.ReadBoolean();
        NPC.localAI[0] = reader.ReadSingle();
    }

    public override void AI()
    {
        if (NPC.target < 0 || NPC.target == 255 || Player.dead || !Player.active)
        {
            NPC.TargetClosest();
        }

        if (Player.dead)
        {
            NPC.velocity.Y += 0.15f;
            NPC.EncourageDespawn(10);
            return;
        }

        HandleRotation();

        if (NPC.localAI[0] == 0f)
        {
            NPC.localAI[0] = 1f;
            NPC.alpha = 255;
            NPC.velocity.Y = -5f;
            CurrentAIState = AIState.Spawning;
        }

        PhaseTwo = NPC.life < NPC.lifeMax * 0.65f;

        switch (CurrentAIState)
        {
            case AIState.Spawning:
                SpawnAnimation();
                break;
            case AIState.PhaseOne_Sideways:
                StayOnSideways();
                break;
            case AIState.PhaseOne_Static:
                StayOnPlace();
                break;
            case AIState.PhaseOne_Bottom:
                StayOnBottom();
                break;
            case AIState.DeathAnimation:
                DeathAnimation();
                break;
        }

        if (NPC.life <= 1)
        {
            NPC.life = 1;
            CurrentAIState = AIState.DeathAnimation;
            if (NPC.netSpam > 10)
            {
                NPC.netSpam = 10;
            }
        }
    }
    #endregion

    #region Attacks

    private void StayOnSideways()
    {
        float timeToShoot = 40f;

        if (PhaseTwo)
        {
            timeToShoot = 30f;
        }

        if (NPC.alpha >= 0)
        {
            NPC.alpha -= 16;
        }

        if (NPC.Distance(Player.Center) > 1000f)
        {
            NPC.Teleport(new Vector2(Player.Center.X + (175f * Player.direction), Player.Center.Y), 1);
        }

        Vector2 targetPosition = Vector2.Normalize(Player.Center + Vector2.UnitX * 175f - NPC.Center) * 15f;
        NPC.SimpleFlyMovement(targetPosition, acceleration);

        Timer++;
        if (Timer % timeToShoot == 0 && Timer != 40f)
        {
            SummonProjectiles(velocity: 10f);
        }

        if (Timer > 300f)
        {
            Timer = 0f;
            CurrentAIState = AIState.PhaseOne_Static;
            NPC.netUpdate = true;
        }
    }

    private void StayOnPlace()
    {
        float timeToShoot = 12f;

        if (PhaseTwo)
        {
            timeToShoot = 10f;
        }

        NPC.velocity *= 0.65f;

        // Teleport in front of the player if they get too far.
        if (NPC.Distance(Player.Center) > 1300f)
        {
            NPC.Teleport(new Vector2(Player.Center.X + (400f * Player.direction), Player.Center.Y), 1);
        }

        if (Timer % timeToShoot == 0)
        {
            SummonProjectiles(velocity: 14f);
        }

        Timer++;
        if (Timer > 500f)
        {
            Timer = 0f;
            CurrentAIState = AIState.PhaseOne_Bottom;
            NPC.netUpdate = true;
        }
    }

    private void StayOnBottom()
    {
        if (NPC.alpha < 200)
        {
            NPC.alpha += 16;
        }

        float timeToShoot = 100f;

        if (NPC.Distance(Player.Center) > 800f)
        {
            NPC.Teleport(Player.Center + Vector2.UnitY * 200f, 1);
        }

        if (PhaseTwo)
        {
            timeToShoot = 75f;
            if (Main.expertMode)
            {
                timeToShoot = 70f;
            }
        }

        Vector2 targetPosition = Vector2.Normalize(Player.Center + Vector2.UnitY * 170f - NPC.Center) * 15f;
        NPC.SimpleFlyMovement(targetPosition, acceleration);

        Timer++;
        if (Timer % timeToShoot == 0)
        {
            for (int i = 0; i < 10; i++)
            {
                NPC.position += NPC.netOffset;
                Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowDust>(), newColor: Color.SkyBlue, Scale: 1.25f);
                NPC.position -= NPC.netOffset;
            }

            for (int i = 0; i < 16; i++)
            {
                Vector2 spawnPos = new(Player.Center.X + Main.screenWidth / 2 - Main.screenWidth / 16 * i, Main.screenPosition.Y + Main.screenHeight + 500f);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, Vector2.UnitY * -4f, ModContent.ProjectileType<IceBossProjectile>(), NPC.GetAttackDamage_ForProjectiles(30f, 23f), 0f, Main.myPlayer, 1f, 0f);
            }
        }

        if (Timer > 400f)
        {
            Timer = 0f;
            CurrentAIState = AIState.PhaseOne_Sideways;
            NPC.netUpdate = true;
        }
    }
    #endregion

    #region Misc
    public override void FindFrame(int frameHeight)
    {
        NPC.frame.Y = 0 * frameHeight;
        if (PhaseTwo)
        {
            NPC.frame.Y = 1 * frameHeight;
        }
    }

    private void DeathAnimation()
    {
        NPC.dontTakeDamage = true;
        NPC.chaseable = false;

        Timer++;
        NPC.velocity *= 0.8f;
        NPC.rotation += 0.4f * NPC.direction;

        if (NPC.scale < 2f)
        {
            NPC.scale += 0.005f;
        }
        if (NPC.alpha < 255)
        {
            NPC.alpha += 4;
        }
        if (NPC.alpha > 255)
        {
            NPC.alpha = 255;
        }

        for (int i = 0; i < 5; i++)
        {
            NPC.position += NPC.netOffset;
            Dust deathDust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<CloudDust>());
            deathDust.velocity *= 4f;
            NPC.position -= NPC.netOffset;
        }

        if (Timer >= 64f)
        {
            NPC.active = false;
            NPC.HitEffect();
            NPC.NPCLoot();
            NPC.netUpdate = true;

            SoundEngine.PlaySound(SoundID.Roar with { Pitch = -0.5f }, NPC.position);
        }
    }

    private void SpawnAnimation()
    {
        NPC.dontTakeDamage = true;
        NPC.chaseable = false;
        NPC.rotation = -NPC.velocity.ToRotation() + MathHelper.PiOver2;
        NPC.velocity *= 0.96f;
        if (NPC.alpha > 0)
        {
            NPC.alpha -= 2;
            for (int i = 0; i < 4; i++)
            {
                NPC.position += NPC.netOffset;
                Vector2 speed = Main.rand.NextVector2CircularEdge(500f, 500f).SafeNormalize(Vector2.UnitY) * 6f;
                Dust.NewDustPerfect(NPC.Center - speed * 20f, ModContent.DustType<CloudDust>(), speed, 128, default, 0.75f);
                NPC.position -= NPC.netOffset;
            }
        }
        else
        {
            NPC.alpha = 0;
            CurrentAIState = AIState.PhaseOne_Sideways;
            NPC.chaseable = true;
            NPC.dontTakeDamage = false;
            NPC.netUpdate = true;
        }
    }

    private void SummonProjectiles(float velocity)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            return;
        }

        Vector2 spawnVelocity = Vector2.Normalize(Player.Center - NPC.Center) * velocity;
        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, spawnVelocity, ModContent.ProjectileType<IceBossProjectile>(), NPC.GetAttackDamage_ForProjectiles(28f, 23f), 0f, Main.myPlayer, 0f, 0f);
    }

    private void HandleRotation()
    {
        if (CurrentAIState is AIState.DeathAnimation or AIState.Spawning)
        {
            return;
        }

        Vector2 npcPlayerPos = Vector2.Normalize(NPC.Center - Player.Center);
        float newRotation = npcPlayerPos.ToRotation() + MathHelper.PiOver2;

        NPC.rotation = NPC.rotation.AngleTowards(newRotation, 0.4f);
    }

    #endregion

    public override void OnKill()
    {
        NPC.SetEventFlagCleared(ref DownedBossSystem.downedFrigus, -1);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            Vector2 spawnVel = Vector2.UnitY * 8f;
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, spawnVel, Mod.Find<ModGore>("IceBossFly_Back").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, spawnVel, Mod.Find<ModGore>("IceBossFly_Back").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, spawnVel, Mod.Find<ModGore>("IceBossFly_Front").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, spawnVel, Mod.Find<ModGore>("IceBossFly_Front").Type);
        }
    }

    public override bool CheckDead()
    {
        NPC.life = 1;
        Timer = 0f;
        NPC.active = true;
        NPC.netUpdate = true;

        if (NPC.netSpam > 10)
        {
            NPC.netSpam = 10;
        }

        return false;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = TextureAssets.Npc[Type].Value;
        SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Vector2 drawOrigin = new(texture.Width / 2, texture.Height / Main.npcFrameCount[Type] / 2);
        Color npcDrawColor = Color.White * NPC.Opacity;
        Color npcDrawColorTrail = npcDrawColor with { A = 0 };


        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            npcDrawColorTrail *= 0.6f;
            spriteBatch.Draw(texture, NPC.oldPos[i] + NPC.Size / 2f - screenPos, NPC.frame, npcDrawColorTrail, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
        }

        spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, npcDrawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
        return false;
    }
}
