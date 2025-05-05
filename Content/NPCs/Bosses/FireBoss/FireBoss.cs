using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Common.Systems;
using Project165.Content.Items.TreasureBags;
using Project165.Content.Items.Weapons.Magic;
using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.Weapons.Ranged;
using Project165.Content.Projectiles.Hostile;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.NPCs.Bosses.FireBoss;

[AutoloadBossHead]
public class FireBoss : ModNPC
{
    public enum AIState : int
    {
        Spawning = 0,
        Flying = 1,
        SpamProjectiles = 2,
        ShootSpheres = 3,
        Death = 4
    }

    public AIState CurrentAIState
    {
        get => (AIState)NPC.ai[0];
        set => NPC.ai[0] = (float)value;
    }

    public ref float AITimer => ref NPC.ai[1];
    public ref float Direction => ref NPC.ai[2];

    public ref float ShootTimer => ref NPC.ai[3];

    public override void SetStaticDefaults()
    {
        NPCID.Sets.TrailingMode[Type] = 1;
        NPCID.Sets.TrailCacheLength[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = true;
        NPCID.Sets.NPCBestiaryDrawModifiers npcBestiaryDrawModifiers = new()
        {
            PortraitScale = 1.25f,
            Scale = 2.5f
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, npcBestiaryDrawModifiers);
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        Main.npcFrameCount[Type] = 4;
    }

    public override void SetDefaults()
    {
        NPC.boss = true;
        NPC.lifeMax = 70000;
        NPC.defense = 150;
        NPC.Size = new(60);
        NPC.scale = 2.5f;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.lavaImmune = true;
        NPC.noTileCollide = true;
        NPC.netAlways = true;
        NPC.HitSound = SoundID.Item105 with { Pitch = 1f };
        NPC.DeathSound = SoundID.Item105;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.aiStyle = -1;
        NPC.npcSlots = 10f;
        Music = MusicID.OtherworldlyLunarBoss;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(
        [
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld
        ]);
    }

    public override void BossLoot(ref string name, ref int potionType)
    {
        potionType = ItemID.SuperHealingPotion;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance);
        NPC.damage = (int)(NPC.damage * 0.6f);
    }

    Player Player => Main.player[NPC.target];

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(NPC.localAI[0]);
        writer.Write(NPC.localAI[1]);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        NPC.localAI[0] = reader.ReadSingle();
        NPC.localAI[1] = reader.ReadSingle();
    }

    public override void AI()
    {
        if (NPC.target < 0 || NPC.target == 255 || Player.dead || !Player.active)
        {
            NPC.TargetClosest();
        }

        if (Player.dead)
        {
            NPC.velocity.Y += 0.2f;
            NPC.EncourageDespawn(despawnTime: 10);
        }

        if (NPC.localAI[1] == 0f)
        {
            NPC.localAI[1] = 1f;
            NPC.alpha = 255;
            CurrentAIState = AIState.Spawning;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 2; i++)
                {
                    int direction = i == 0 ? 1 : -1;
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitX * 800f * -direction, Vector2.Zero, ModContent.ProjectileType<FireTornado>(), NPC.GetAttackDamage_ForProjectiles(10f, 13f), 8f, Main.myPlayer, NPC.whoAmI, direction);
                }
            }
        }

        Lighting.AddLight(NPC.Center, 2f, 2f, 2f);
        HandleRotation();

        switch (CurrentAIState)
        {
            case AIState.Spawning:
                SpawnAnimation();
                break;
            case AIState.Flying:
                FlyAbove();
                break;
            case AIState.SpamProjectiles:
                SpamProjectiles();
                break;
            case AIState.ShootSpheres:
                ShootSpheres();
                break;
        }

        NPC.dontTakeDamage = CurrentAIState is AIState.Spawning or AIState.Death;
        NPC.chaseable = CurrentAIState is not AIState.Spawning or AIState.Death;
    }

    #region Attacks
    public void SpawnAnimation()
    {
        NPC.TargetClosest();
        NPC.localAI[0]++;
        if (NPC.alpha > 0)
        {
            NPC.alpha -= 4;
        }

        for (int i = 0; i < 3; i++)
        {
            Vector2 newVelocity = Main.rand.NextVector2CircularEdge(200f, 200f).SafeNormalize(Vector2.UnitY) * 5f;
            NPC.position += NPC.netOffset;
            Dust dust = Dust.NewDustPerfect(NPC.Center - newVelocity * 20f, DustID.Torch, newVelocity, Scale: 3f);
            dust.noGravity = true;
            NPC.position -= NPC.netOffset;
        }

        if (NPC.alpha < 0)
        {
            NPC.alpha = 0;
        }

        if (NPC.localAI[0] > 180f)
        {
            NPC.localAI[0] = 180f;
        }

        if (NPC.localAI[0] >= 180f)
        {
            CurrentAIState = AIState.Flying;
            NPC.netUpdate = true;
        }
    }

    public void FlyAbove()
    {
        if (Direction == 0f)
        {
            NPC.TargetClosest();
            Direction = (NPC.Center.X < Player.Center.X).ToDirectionInt();
        }

        NPC.TargetClosest();
        float moveAcceleration = 0.4f;
        float maxSpeed = 11f;
        float maxDistance = 400f;
        float distance = MathF.Abs(NPC.Center.X - Player.Center.X);
        float verticalDistance = Player.Center.Y - 200f - NPC.Center.Y;
        verticalDistance = MathHelper.Clamp(verticalDistance, -32f, 32f);

        if ((NPC.Center.X < Player.Center.X && Direction < 0f || NPC.Center.X > Player.Center.X && Direction > 0f) && distance > maxDistance)
        {
            Direction = 0f;
        }

        NPC.velocity.X += Direction * moveAcceleration;
        NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -maxSpeed, maxSpeed);

        NPC.velocity.Y = (NPC.velocity.Y * (maxDistance - 1f) + verticalDistance) / maxDistance;
        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.025f, 0.05f);
        if (distance < 800f && NPC.position.Y < Player.position.Y && AITimer > 35f)
        {
            ShootTimer++;
            int timeToShoot = 60;
            if (NPC.life < NPC.lifeMax * 0.5)
            {
                timeToShoot -= 4;
            }
            if (NPC.life < NPC.lifeMax * 0.25)
            {
                timeToShoot -= 8;
            }
            if (ShootTimer >= timeToShoot && Main.netMode != NetmodeID.MultiplayerClient)
            {
                ShootTimer = 0f;
                Vector2 newVelocity = Vector2.Normalize(Player.Center - NPC.Center) * 12f;
                for (int i = 0; i < 3; i++)
                {
                    newVelocity = newVelocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.85f, 1.125f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, newVelocity, ModContent.ProjectileType<FireBossProj>(), NPC.GetAttackDamage_ForProjectiles(28f, 30f), 0f, Main.myPlayer);
                }
            }
        }

        AITimer++;
        if (AITimer > 800f && distance < 600f)
        {
            CurrentAIState = AIState.ShootSpheres;
            AITimer = 0f;
            Direction = 0f;
            ShootTimer = 0f;
            NPC.rotation = 0f;
            NPC.netUpdate = true;
        }
    }

    public void SpamProjectiles()
    {
        NPC.TargetClosest();
        NPC.velocity *= 0.7f;

        if (Main.netMode != NetmodeID.MultiplayerClient && AITimer > 30f)
        {
            ShootTimer++;
            int timeToShoot = 14;
            if (NPC.life < NPC.lifeMax * 0.75)
            {
                timeToShoot = 12;
            }
            if (NPC.life < NPC.lifeMax * 0.5)
            {
                timeToShoot = 10;
            }
            if (ShootTimer % timeToShoot == 0f)
            {
                ShootTimer = 0f;
                Vector2 newVelocity = Vector2.Normalize(Player.Center - NPC.Center) * 8f;
                for (int i = 0; i < 5; i++)
                {
                    newVelocity *= Main.rand.NextFloat(0.85f, 1.125f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + newVelocity, newVelocity.RotatedByRandom(MathHelper.ToRadians(90f)), ModContent.ProjectileType<FireBossProj>(), NPC.GetAttackDamage_ForProjectiles(15f, 17f), 0f, Main.myPlayer);
                }
            }
        }

        AITimer++;
        if (AITimer > 650f)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type == ModContent.ProjectileType<FireBossProj>())
                {
                    Main.projectile[i].Kill();
                }
            }

            ShootTimer = 0f;
            CurrentAIState = AIState.Flying;
            AITimer = 0f;
            NPC.netUpdate = true;
        }
    }

    public void ShootSpheres()
    {
        int timeToShoot = 40;
        double difficultyValue = Main.expertMode ? 0.5 : 0.3;

        NPC.TargetClosest();

        Vector2 newVelocity = Vector2.Normalize(Player.Center + Player.Size * 4f - NPC.Center) * 4.5f;
        NPC.SimpleFlyMovement(newVelocity, 0.5f);

        if (NPC.life < NPC.lifeMax * difficultyValue)
        {
            timeToShoot -= 8;
        }
        if (NPC.life < NPC.lifeMax * 0.2)
        {
            timeToShoot -= 8;
        }

        float projAmount = 6;
        ShootTimer++;
        if (ShootTimer >= timeToShoot && ShootTimer > 0f)
        {
            ShootTimer = 0f;
            Vector2 center = Vector2.Normalize(NPC.Center) * 40f;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < projAmount; i++)
                {
                    float rotate = i - (projAmount - 1f) / 2f;
                    Vector2 offset = center.RotatedBy(MathHelper.TwoPi / projAmount * rotate);
                    Vector2 newVel = Vector2.Normalize(Player.Center - NPC.Center) * 12f;

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset.RotatedBy(newVel.ToRotation()), newVel, ModContent.ProjectileType<FireBossProj>(), NPC.GetAttackDamage_ForProjectiles(29f, 30f), 0f, Main.myPlayer);
                }
            }
        }
        AITimer++;
        if (AITimer > 800f)
        {
            CurrentAIState = AIState.SpamProjectiles;
            ShootTimer = 0f;
            AITimer = 0f;
            NPC.netUpdate = true;
        }
    }
    #endregion

    #region Misc
    private void HandleRotation()
    {
        if (CurrentAIState is AIState.Death or AIState.Spawning or AIState.Flying)
        {
            return;
        }

        Vector2 npcPlayerPos = NPC.Center - Player.Center;
        float newRotation = npcPlayerPos.ToRotation() + MathHelper.PiOver2;

        NPC.rotation = NPC.rotation.AngleTowards(newRotation, 0.1f);
    }

    #endregion

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        Conditions.NotExpert notExpert = new();
        npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<FireBossBag>()));

        npcLoot.Add(ItemDropRule.ByCondition(notExpert, ModContent.ItemType<UltraFireStaff>(), 3));
        npcLoot.Add(ItemDropRule.ByCondition(notExpert, ModContent.ItemType<InfernalBow>(), 3));
        npcLoot.Add(ItemDropRule.ByCondition(notExpert, ModContent.ItemType<SuperFireSword>(), 3));
    }

    public override void OnKill()
    {
        NPC.SetEventFlagCleared(ref DownedBossSystem.downedFireBoss, -1);
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (projectile.type == ProjectileID.FinalFractal)
        {
            projectile.usesLocalNPCImmunity = false;
        }
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter += 0.25;
        if (NPC.frameCounter >= Main.npcFrameCount[Type])
        {
            NPC.frameCounter = 0;
        }
        NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Vector2 drawOrigin = new(texture.Width / 2, texture.Height / Main.npcFrameCount[Type] / 2 + 16);
        Color npcDrawColor = Color.White * NPC.Opacity;
        Color secondPhaseColor = Color.Red * NPC.Opacity;
        Color npcDrawColorTrail = npcDrawColor with { A = 0 };
        float lerpValue = NPC.life < NPC.lifeMax / 2 ? 0.5f : 0f;
        //Color drawColorLerp = Color.Lerp(npcDrawColor, secondPhaseColor, lerpValue);
        //Main.NewText(NPC.life < NPC.lifeMax / 2);

        float offset = (float)Main.timeForVisualEffects / 240f + Main.GlobalTimeWrappedHourly * 0.5f;

        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            Vector2 drawPos = NPC.oldPos[i] + NPC.Size * 0.5f - screenPos;
            npcDrawColorTrail *= 0.5f;
            spriteBatch.Draw(texture, drawPos, NPC.frame, npcDrawColorTrail, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
        }

        for (float i = 0f; i < 1f; i += 0.25f)
        {
            spriteBatch.Draw(texture, NPC.Center - screenPos + (Vector2.UnitY * 8f).RotatedBy((i + offset) * MathHelper.TwoPi) * 3f, NPC.frame, Color.Yellow with { A = 0 } * NPC.Opacity * 0.2f, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
        }

        spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, npcDrawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);

        return false;
    }
}
