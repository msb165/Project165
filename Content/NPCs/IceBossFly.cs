using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.ItemDropRules;
using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.TreasureBags;

namespace Project165.Content.NPCs
{
    public class IceBossFly : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 110;
            NPC.height = 132;            
            NPC.defense = 10;
            NPC.damage = 45;
            NPC.lifeMax = 20000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.value = 120000f;
            NPC.npcSlots = 5f;
            NPC.aiStyle = -1;
        }

        private Player Player => Main.player[NPC.target];
        private readonly bool debugMode = false;


        private void DoDebugStuff()
        {
            Main.NewText($"Distance player -> boss: {Vector2.Distance(Player.Center, NPC.Center)}");
            Main.NewText($"ai[0]: {NPC.ai[0]} ai[1]: {NPC.ai[1]} ai[2]: {NPC.ai[2]} ai[3]: {NPC.ai[3]}");
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<IceBossBag>()));

            LeadingConditionRule notExpertConditionRule = new(new Conditions.NotExpert());
            notExpertConditionRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<IceChakram>(), 3));
            npcLoot.Add(notExpertConditionRule);
        }
        public override void AI()
        {
            bool flag2 = false;
            bool flag3 = false;
            float num4 = 20f;

            if (debugMode)
            {
                DoDebugStuff();
            }

            if (NPC.life < NPC.lifeMax * 0.12)
            {
                flag2 = true;
            }
            
            if (NPC.life < NPC.lifeMax * 0.04)
            {
                flag3 = true;
            }
            
            if (flag3)
            {
                num4 = 10f;
            }
            if (NPC.target < 0 || NPC.target == 255 || Player.dead || !Player.active)
            {
                NPC.TargetClosest();
            }            
            
            Vector2 npcPlayerDistance = new(NPC.Center.X - Player.Center.X, NPC.Center.Y - 60f - Player.Center.Y);            
            float npcPlayerDistanceRotated = npcPlayerDistance.ToRotation() + MathHelper.PiOver2;

            if (npcPlayerDistanceRotated < 0f)
            {
                npcPlayerDistanceRotated += MathHelper.TwoPi;
            }
            else if ((double)npcPlayerDistanceRotated > MathHelper.TwoPi)
            {
                npcPlayerDistanceRotated -= MathHelper.TwoPi;
            }
            float rotationSpeed = 0.05f;
            if (NPC.ai[0] == 3f && NPC.ai[1] == 2f && NPC.ai[2] > 40f)
            {
                rotationSpeed = 0.08f;
            }
            if (NPC.ai[0] == 3f && NPC.ai[1] == 4f && NPC.ai[2] > num4)
            {
                rotationSpeed = 0.15f;
            }
            if (Main.expertMode)
            {
                rotationSpeed *= 1.5f;
            }
            if (flag3)
            {
                rotationSpeed = 0f;
            }
            if (NPC.rotation < npcPlayerDistanceRotated)
            {
                if ((double)(npcPlayerDistanceRotated - NPC.rotation) > MathHelper.Pi)
                {
                    NPC.rotation -= rotationSpeed;
                }
                else
                {
                    NPC.rotation += rotationSpeed;
                }
            }
            else if (NPC.rotation > npcPlayerDistanceRotated)
            {
                if ((double)(NPC.rotation - npcPlayerDistanceRotated) > MathHelper.Pi)
                {
                    NPC.rotation += rotationSpeed;
                }
                else
                {
                    NPC.rotation -= rotationSpeed;
                }
            }
            if (NPC.rotation > npcPlayerDistanceRotated - rotationSpeed && NPC.rotation < npcPlayerDistanceRotated + rotationSpeed)
            {
                NPC.rotation = npcPlayerDistanceRotated;
            }
            if (NPC.rotation < 0f)
            {
                NPC.rotation += MathHelper.TwoPi;
            }
            else if (NPC.rotation > MathHelper.TwoPi)
            {
                NPC.rotation -= MathHelper.TwoPi;
            }
            if (NPC.rotation > npcPlayerDistanceRotated - rotationSpeed && NPC.rotation < npcPlayerDistanceRotated + rotationSpeed)
            {
                NPC.rotation = npcPlayerDistanceRotated;
            }

            if (Main.IsItDay() || Player.dead)
            {
                NPC.velocity.Y += 0.08f;
                NPC.EncourageDespawn(10);
                return;
            }
            if (NPC.ai[0] == 0f)
            {
                if (NPC.ai[1] == 0f)
                {
                    float dashSpeed = 7f;
                    float num11 = 0.15f;

                    float num12 = Player.Center.X + 250f - NPC.Center.X;
                    float num13 = Player.Center.Y - 250f - NPC.Center.Y;
                    float num14 = (float)Math.Sqrt(num12 * num12 + num13 * num13);                    
                    num14 = dashSpeed / num14;
                    num12 *= num14;
                    num13 *= num14;

                    if (NPC.velocity.X < num12)
                    {
                        NPC.velocity.X += num11;
                        if (NPC.velocity.X < 0f && num12 > 0f)
                        {
                            NPC.velocity.X += num11;
                        }
                    }
                    else if (NPC.velocity.X > num12)
                    {
                        NPC.velocity.X -= num11;
                        if (NPC.velocity.X > 0f && num12 < 0f)
                        {
                            NPC.velocity.X -= num11;
                        }
                    }
                    if (NPC.velocity.Y < num13)
                    {
                        NPC.velocity.Y += num11;
                        if (NPC.velocity.Y < 0f && num13 > 0f)
                        {
                            NPC.velocity.Y += num11;
                        }
                    }
                    else if (NPC.velocity.Y > num13)
                    {
                        NPC.velocity.Y -= num11;
                        if (NPC.velocity.Y > 0f && num13 < 0f)
                        {
                            NPC.velocity.Y -= num11;
                        }
                    }
                    NPC.ai[2] += 1f;
                    float waitingTime = 400f;
                    if (NPC.ai[2] >= waitingTime)
                    {
                        NPC.ai[1] = 1f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.target = 255;
                        NPC.netUpdate = true;
                    }
                    else if (Vector2.Distance(Player.position, NPC.position) <= 1000f) 
                    {
                        if (!Player.dead)
                        {
                            NPC.ai[3] += 1f;
                        }
                        if (NPC.ai[3] >= 40f)
                        {
                            NPC.ai[3] = 0f;
                            Vector2 npcCenter = NPC.Center;    

                            float num422 = Player.Center.X - NPC.Center.X;
                            float num423 = Player.Center.Y - NPC.Center.Y;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {  
                                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Player.position, Player.width, Player.height))
                                {
                                    float num424 = (float)Math.Sqrt(num422 * num422 + num423 * num423);
                                    num424 = 10f / num424;
                                    num422 *= num424;
                                    num423 *= num424;
                                    npcCenter.X += num422;
                                    npcCenter.Y += num423;
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), npcCenter.X, npcCenter.Y, num422, num423, ProjectileID.IcewaterSpit, 25, 0f, Main.myPlayer);
                                }                
                            }
                        }
                    }
                }
                else if (NPC.ai[1] == 1f)
                {
                    NPC.rotation = npcPlayerDistanceRotated;
                    float dashSpeed = 12f;
                    Vector2 vector4 = NPC.Center;
                    float num24 = Player.Center.X - vector4.X;
                    float num25 = Player.Center.Y - vector4.Y;
                    float num26 = (float)Math.Sqrt(num24 * num24 + num25 * num25);
                    num26 = dashSpeed / num26;
                    NPC.velocity.X = num24 * num26;
                    NPC.velocity.Y = num25 * num26;
                    NPC.ai[1] = 2f;
                    NPC.netUpdate = true;
                    if (NPC.netSpam > 10)
                    {
                        NPC.netSpam = 10;
                    }
                }
                else if (NPC.ai[1] == 2f)
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= 40f)
                    {
                        NPC.velocity *= 0.99f;
                        if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                        {
                            NPC.velocity.X = 0f;
                        }
                        if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                        {
                            NPC.velocity.Y = 0f;
                        }
                    }
                    else
                    {
                        NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                    }                    
                    if (NPC.ai[2] >= 75f)
                    {
                        NPC.ai[3] += 1f;
                        NPC.ai[2] = 0f;
                        NPC.target = 255;
                        NPC.rotation = npcPlayerDistanceRotated;
                        if (NPC.ai[3] >= 3f)
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[3] = 0f;
                        }
                        else
                        {
                            NPC.ai[1] = 1f;
                        }
                    }
                }
                if (NPC.life < NPC.lifeMax * 0.65f)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                    if (NPC.netSpam > 10)
                    {
                        NPC.netSpam = 10;
                    }
                }
                return;
            }
            if (NPC.ai[0] == 1f || NPC.ai[0] == 2f)
            {
                NPC.rotation += 0.2f;
                NPC.ai[1] += 1f;

                if (NPC.ai[1] % 8f == 0f)
                {                    
                    Vector2 vector5 = new(NPC.Center.X, NPC.Center.Y);
                    float num31 = Main.rand.Next(-800, 801);
                    float num32 = Main.rand.Next(-800, 801);                    
                    float num33 = (float)Math.Sqrt(num31 * num31 + num32 * num32);

                    num33 = 15f / num33;
                    num31 *= num33;
                    num32 *= num33;               
                    
                    vector5.X += num31;
                    vector5.Y += num32;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Player.position, Player.width, Player.height))
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), vector5.X, vector5.Y, num31, num32, ProjectileID.IcewaterSpit, 25, 0f, Main.myPlayer);
                        }                        
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        Dust.NewDust(vector5, 20, 20, DustID.Ice, vector5.X * 0.4f, vector5.Y * 0.4f);
                    }
                }
                if (NPC.ai[1] >= 100f)
                {
                    if (NPC.ai[3] == 1f)
                    {
                        NPC.ai[3] = 0f;
                        NPC.ai[1] = 0f;
                    }
                    else
                    {
                        NPC.ai[0] += 1f;
                        NPC.ai[1] = 0f;
                        if (NPC.ai[0] == 3f)
                        {
                            NPC.ai[2] = 0f;
                        }
                        else
                        {
                            SoundEngine.PlaySound(SoundID.Roar, NPC.position);
                        }
                    }
                }
                                
                NPC.velocity *= 0.99f;                

                if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                {
                    NPC.velocity.X = 0f;
                }
                if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                {
                    NPC.velocity.Y = 0f;
                }
                return;                
            }

            if (NPC.ai[1] == 0f && flag2)
            {
                NPC.ai[1] = 5f;
            }
            if (NPC.ai[1] == 0f)
            {
                float num39 = 6f;
                float num40 = 0.07f;
                Vector2 vector8 = NPC.Center;
                float num41 = Player.Center.X - vector8.X;
                float num42 = Player.Center.Y - 120f - vector8.Y;
                float num43 = (float)Math.Sqrt(num41 * num41 + num42 * num42);
                if (num43 > 400f)
                {
                    num39 += 1f;
                    num40 += 0.05f;
                    if (num43 > 600f)
                    {
                        num39 += 1f;
                        num40 += 0.05f;
                        if (num43 > 800f)
                        {
                            num39 += 1f;
                            num40 += 0.05f;
                        }
                    }
                }
                num43 = num39 / num43;
                num41 *= num43;
                num42 *= num43;
                if (NPC.velocity.X < num41)
                {
                    NPC.velocity.X += num40;
                    if (NPC.velocity.X < 0f && num41 > 0f)
                    {
                        NPC.velocity.X += num40;
                    }
                }
                else if (NPC.velocity.X > num41)
                {
                    NPC.velocity.X -= num40;
                    if (NPC.velocity.X > 0f && num41 < 0f)
                    {
                        NPC.velocity.X -= num40;
                    }
                }
                if (NPC.velocity.Y < num42)
                {
                    NPC.velocity.Y += num40;
                    if (NPC.velocity.Y < 0f && num42 > 0f)
                    {
                        NPC.velocity.Y += num40;
                    }
                }
                else if (NPC.velocity.Y > num42)
                {
                    NPC.velocity.Y -= num40;
                    if (NPC.velocity.Y > 0f && num42 < 0f)
                    {
                        NPC.velocity.Y -= num40;
                    }
                }
                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= 200f)
                {
                    NPC.ai[1] = 1f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    if (NPC.life < NPC.lifeMax * 0.35)
                    {
                        NPC.ai[1] = 3f;
                    }
                    NPC.target = 255;
                    NPC.netUpdate = true;
                }
                if (flag3)
                {
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                    NPC.ai[1] = 3f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] -= 1000f;
                }
            }
            else if (NPC.ai[1] == 1f)
            {
                SoundEngine.PlaySound(SoundID.ForceRoar, NPC.position);
                NPC.rotation = npcPlayerDistanceRotated;
                float dashSpeed = 11f;
                if (NPC.ai[3] == 1f)
                {
                    dashSpeed *= 1.15f;
                }
                if (NPC.ai[3] == 2f)
                {
                    dashSpeed *= 1.4f;
                }
                Vector2 vector9 = NPC.Center;
                float num45 = Player.Center.X - vector9.X;
                float num46 = Player.Center.Y - vector9.Y;
                float num47 = (float)Math.Sqrt(num45 * num45 + num46 * num46);
                num47 = dashSpeed / num47;
                NPC.velocity.X = num45 * num47;
                NPC.velocity.Y = num46 * num47;

                NPC.ai[1] = 2f;

                NPC.netUpdate = true;
                if (NPC.netSpam > 10)
                {
                    NPC.netSpam = 10;
                }
            }
            else if (NPC.ai[1] == 2f)
            {
                float num48 = 60f;
                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= num48)
                {
                    NPC.velocity *= 0.99f;
                    if (NPC.velocity.X > -0.1f && NPC.velocity.X < 0.1f)
                    {
                        NPC.velocity.X = 0f;
                    }
                    if (NPC.velocity.Y > -0.1f && NPC.velocity.Y < 0.1f)
                    {
                        NPC.velocity.Y = 0f;
                    }
                }
                else
                {
                    NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                }
                if (NPC.ai[2] >= 90f)
                {
                    NPC.ai[3] += 1f;
                    NPC.ai[2] = 0f;
                    NPC.target = 255;
                    NPC.rotation = npcPlayerDistanceRotated;
                    if (NPC.ai[3] >= 3f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[3] = 0f;
                        if (Main.netMode != NetmodeID.MultiplayerClient && NPC.life < NPC.lifeMax * 0.5)
                        {
                            NPC.ai[1] = 3f;
                            NPC.ai[3] += Main.rand.Next(1, 4);
                        }
                        NPC.netUpdate = true;
                        if (NPC.netSpam > 10)
                        {
                            NPC.netSpam = 10;
                        }
                    }
                    else
                    {
                        NPC.ai[1] = 1f;
                    }
                }
            }
            else if (NPC.ai[1] == 3f)
            {
                if (NPC.ai[3] == 4f && flag2 && NPC.Center.Y > Player.Center.Y)
                {
                    NPC.TargetClosest();
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                    if (NPC.netSpam > 10)
                    {
                        NPC.netSpam = 10;
                    }
                }
                else if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.TargetClosest();
                    float num50 = 20f;
                    Vector2 vector10 = NPC.Center;
                    float num51 = Player.Center.X - vector10.X;
                    float num52 = Player.Center.Y - vector10.Y;
                    float num53 = Math.Abs(Player.velocity.X) + Math.Abs(Player.velocity.Y) / 4f;
                    num53 += 10f - num53;
                    if (num53 < 5f)
                    {
                        num53 = 5f;
                    }
                    if (num53 > 15f)
                    {
                        num53 = 15f;
                    }
                    if (NPC.ai[2] == -1f && !flag3)
                    {
                        num53 *= 4f;
                        num50 *= 1.3f;
                    }
                    if (flag3)
                    {
                        num53 *= 2f;
                    }
                    num51 -= Player.velocity.X * num53;
                    num52 -= Player.velocity.Y * num53 / 4f;
                    num51 *= 1f + Main.rand.Next(-10, 11) * 0.01f;
                    num52 *= 1f + Main.rand.Next(-10, 11) * 0.01f;
                    if (flag3)
                    {
                        num51 *= 1f + Main.rand.Next(-10, 11) * 0.01f;
                        num52 *= 1f + Main.rand.Next(-10, 11) * 0.01f;
                    }
                    float num54 = (float)Math.Sqrt(num51 * num51 + num52 * num52);
                    float num55 = num54;
                    num54 = num50 / num54;
                    NPC.velocity.X = num51 * num54;
                    NPC.velocity.Y = num52 * num54;
                    NPC.velocity.X += Main.rand.Next(-20, 21) * 0.1f;
                    NPC.velocity.Y += Main.rand.Next(-20, 21) * 0.1f;
                    if (flag3)
                    {
                        NPC.velocity.X += Main.rand.Next(-50, 51) * 0.1f;
                        NPC.velocity.Y += Main.rand.Next(-50, 51) * 0.1f;
                        float num56 = Math.Abs(NPC.velocity.X);
                        float num57 = Math.Abs(NPC.velocity.Y);
                        if (NPC.Center.X > Player.Center.X)
                        {
                            num57 *= -1f;
                        }
                        if (NPC.Center.Y > Player.Center.Y)
                        {
                            num56 *= -1f;
                        }
                        NPC.velocity.X = num57 + NPC.velocity.X;
                        NPC.velocity.Y = num56 + NPC.velocity.Y;
                        NPC.velocity.Normalize();
                        NPC.velocity *= num50;
                        NPC.velocity.X += Main.rand.Next(-20, 21) * 0.1f;
                        NPC.velocity.Y += Main.rand.Next(-20, 21) * 0.1f;
                    }
                    else if (num55 < 100f)
                    {
                        if (Math.Abs(NPC.velocity.X) > Math.Abs(NPC.velocity.Y))
                        {
                            float num58 = Math.Abs(NPC.velocity.X);
                            float num59 = Math.Abs(NPC.velocity.Y);
                            if (NPC.Center.X > Player.Center.X)
                            {
                                num59 *= -1f;
                            }
                            if (NPC.Center.Y > Player.Center.Y)
                            {
                                num58 *= -1f;
                            }
                            NPC.velocity.X = num59;
                            NPC.velocity.Y = num58;
                        }
                    }
                    else if (Math.Abs(NPC.velocity.X) > Math.Abs(NPC.velocity.Y))
                    {
                        float num60 = (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) / 2f;
                        float num61 = num60;
                        if (NPC.Center.X > Player.Center.X)
                        {
                            num61 *= -1f;
                        }
                        if (NPC.Center.Y > Player.Center.Y)
                        {
                            num60 *= -1f;
                        }
                        NPC.velocity.X = num61;
                        NPC.velocity.Y = num60;
                    }
                    NPC.ai[1] = 4f;
                    NPC.netUpdate = true;
                    if (NPC.netSpam > 10)
                    {
                        NPC.netSpam = 10;
                    }
                }
            }
            else if (NPC.ai[1] == 4f)
            {
                if (NPC.ai[2] == 0f)
                {
                    SoundEngine.PlaySound(SoundID.ForceRoar, NPC.position);
                }
                float num62 = num4;
                NPC.ai[2] += 1f;
                if (NPC.ai[2] == num62 && Vector2.Distance(NPC.position, Player.position) < 200f)
                {
                    NPC.ai[2] -= 1f;
                }
                if (NPC.ai[2] >= num62)
                {
                    NPC.velocity *= 0.95f;
                    if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                    {
                        NPC.velocity.X = 0f;
                    }
                    if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                    {
                        NPC.velocity.Y = 0f;
                    }
                }
                else
                {
                    NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                }
                float num63 = num62 + 13f;
                if (NPC.ai[2] >= num63)
                {
                    NPC.netUpdate = true;
                    if (NPC.netSpam > 10)
                    {
                        NPC.netSpam = 10;
                    }
                    NPC.ai[3] += 1f;
                    NPC.ai[2] = 0f;
                    if (NPC.ai[3] >= 5f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[3] = 0f;
                        if (NPC.target >= 0 && Collision.CanHit(NPC.position, NPC.width, NPC.height, Player.position, Player.width, Player.height))
                        {
                            SoundEngine.PlaySound(SoundID.Roar, NPC.position);
                            NPC.ai[0] = 2f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 1f;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        NPC.ai[1] = 3f;
                    }
                }
            }
            else if (NPC.ai[1] == 5f)
            {                
                float num65 = 11f;
                float num66 = 0.4f;
                Vector2 npcCenter = NPC.Center;
                float num67 = Player.Center.X - npcCenter.X;
                float num68 = Player.Center.Y + 500f - npcCenter.Y;
                float playerrNPCCenterLength = (float)Math.Sqrt(num67 * num67 + num68 * num68);
                playerrNPCCenterLength = num65 / playerrNPCCenterLength;
                num67 *= playerrNPCCenterLength;
                num68 *= playerrNPCCenterLength;
                if (NPC.velocity.X < num67)
                {
                    NPC.velocity.X += num66;
                    if (NPC.velocity.X < 0f && num67 > 0f)
                    {
                        NPC.velocity.X += num66;
                    }
                }
                else if (NPC.velocity.X > num67)
                {
                    NPC.velocity.X -= num66;
                    if (NPC.velocity.X > 0f && num67 < 0f)
                    {
                        NPC.velocity.X -= num66;
                    }
                }
                if (NPC.velocity.Y < num68)
                {
                    NPC.velocity.Y += num66;
                    if (NPC.velocity.Y < 0f && num68 > 0f)
                    {
                        NPC.velocity.Y += num66;
                    }
                }
                else if (NPC.velocity.Y > num68)
                {
                    NPC.velocity.Y -= num66;
                    if (NPC.velocity.Y > 0f && num68 < 0f)
                    {
                        NPC.velocity.Y -= num66;
                    }
                }
                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= 70f)
                {
                    NPC.TargetClosest();
                    NPC.ai[1] = 3f;
                    NPC.ai[2] = -1f;
                    NPC.ai[3] = Main.rand.Next(-3, 1);
                    NPC.netUpdate = true;
                }
            }
            if (flag3 && NPC.ai[1] == 5f)
            {
                NPC.ai[1] = 3f;
            }            
        }
    }
}
