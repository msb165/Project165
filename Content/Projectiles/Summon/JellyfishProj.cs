using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Buffs;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Summon;

public class JellyfishProj : ModProjectile
{
    public override string Texture => $"Terraria/Images/NPC_{NPCID.BlueJellyfish}";

    public enum AIState : int
    {
        Idling = 0,
        Returning = 1,
        Attacking = 2
    }

    public AIState CurrentState
    {
        get => (AIState)Projectile.ai[0];
        set => Projectile.ai[0] = (float)value;
    }

    public float AttackTargetTimer
    {
        get => Projectile.ai[1];
        set => Projectile.ai[1] = value;
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.MinionSacrificable[Type] = true;
        ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        Main.projFrames[Type] = 7;
    }

    public override void SetDefaults() 
    {
        Projectile.netImportant = true;
        Projectile.alpha = 100;
        Projectile.width = 26;
        Projectile.height = 26;
        Projectile.aiStyle = -1;
        Projectile.penetrate = -1;
        Projectile.timeLeft *= 5;
        Projectile.minion = true;
        Projectile.minionSlots = 1f;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.friendly = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 40;
    }

    Player Player => Main.player[Projectile.owner];
    float JellyFishColor => Projectile.ai[2];

    public bool CheckActive(Player player)
    {
        if (player.dead || !player.active)
        {
            player.ClearBuff(ModContent.BuffType<JellyfishBuff>());
            return false;
        }

        if (player.HasBuff(ModContent.BuffType<JellyfishBuff>()))
        {
            Projectile.timeLeft = 2;
        }

        return true;
    }

    public override void AI()
    {
        if (!CheckActive(Player))
        {
            return;
        }

        switch (CurrentState)
        {
            case AIState.Idling:
                Idle();
                break;
            case AIState.Returning:
                Return();
                break;
            case AIState.Attacking:
                Attack();
                break;
        }

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        Projectile.spriteDirection = Projectile.direction;
    }

    public void Idle()
    {
        Vector2 newSpeed = Player.Center - Projectile.Center - Vector2.UnitY * 60f;
        float distance = newSpeed.Length();
        float multiplier = 12f;
        newSpeed.Normalize();

        if (distance > 60f)
        {
            newSpeed *= multiplier;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, newSpeed, 0.0125f);
        }
        if (distance > 2000f)
        {
            Projectile.position = Player.Center - Vector2.UnitY * 60f;
            Projectile.netUpdate = true;
        }

        if (Projectile.velocity.Length() > multiplier)
        {
            Projectile.velocity *= 0.95f;
        }

        int attackRange = 500;
        int attackTarget = -1;
        Projectile.Minion_FindTargetInRange(attackRange, ref attackTarget, skipIfCannotHitWithOwnBody: false);
        if (attackTarget != -1)
        {
            Projectile.netUpdate = true;
            CurrentState = AIState.Attacking;
            AttackTargetTimer = attackTarget;
        }
    }

    public void Return()
    {
        if (AttackTargetTimer == 0f) 
        {
            Projectile.velocity += Main.rand.NextVector2CircularEdge(2f, 8f);
        }
        AttackTargetTimer++;
        Projectile.velocity *= 0.92f;
        if (AttackTargetTimer >= 18f)
        {
            CurrentState = AIState.Idling;
            AttackTargetTimer = 0f;
        }
    }

    public void Attack()
    {
        NPC target = null;
        int targetIndex = (int)AttackTargetTimer;
        if (Main.npc.IndexInRange(targetIndex) && Main.npc[targetIndex].CanBeChasedBy(this))
        {
            target = Main.npc[targetIndex];
        }
        if (target == null)
        {
            CurrentState = AIState.Returning;
            AttackTargetTimer = 0f;
            Projectile.netUpdate = true;
        }      
        else if (Player.Distance(target.Center) >= 1000f)
        {
            CurrentState = AIState.Idling;
            AttackTargetTimer = 0f;
            Projectile.netUpdate = true;
        }
        else
        {
            Vector2 targetSpeed = Vector2.Normalize(target.Center - Projectile.Center) * 16f;
            Projectile.velocity = targetSpeed;
            Projectile.rotation = Projectile.velocity.SafeNormalize(Vector2.UnitY).ToRotation() + MathHelper.PiOver2;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Electrified, 300);
        Projectile.localNPCImmunity[target.whoAmI] = 10;
        target.immune[Projectile.owner] = 0;
        if (CurrentState != AIState.Idling) 
        {
            CurrentState = AIState.Returning;
            AttackTargetTimer = 0f;
            Projectile.netUpdate = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Color projColor = Projectile.GetAlpha(lightColor);
        Color trailColor = projColor;

        Main.instance.LoadNPC(NPCID.BlueJellyfish);
        Main.instance.LoadNPC(NPCID.GreenJellyfish);
        Main.instance.LoadNPC(NPCID.PinkJellyfish);
        int npcID = NPCID.BlueJellyfish;
        switch (JellyFishColor)
        {
            case 0:
                npcID = NPCID.BlueJellyfish;
                break;
            case 1:
                npcID = NPCID.GreenJellyfish;
                break;
            case 2:
                npcID = NPCID.PinkJellyfish;
                break;
        }

        Texture2D texture = TextureAssets.Npc[npcID].Value;
        SpriteBatch spriteBatch = Main.spriteBatch;

        int frameHeight = texture.Height / Main.projFrames[Type];
        int startY = frameHeight * Projectile.frame;
        Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);

        // Show afterimages only when it's attacking
        if (CurrentState is AIState.Attacking or AIState.Returning)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldDraw = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                trailColor *= 0.8f;
                spriteBatch.Draw(texture, oldDraw, sourceRectangle, trailColor, Projectile.rotation, sourceRectangle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            }
        }

        spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, projColor, Projectile.rotation, sourceRectangle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }
}
