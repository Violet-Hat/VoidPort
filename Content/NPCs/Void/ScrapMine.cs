using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using ReLogic.Content;

using VoidPort.Common;
using VoidPort.Enums;
using VoidPort.Content.Tiles.Void;

namespace VoidPort.Content.NPCs.Void
{
    public class ScrapMine : ModNPC
    {
        //Textures
        private static Asset<Texture2D> NPCTex;
        private static Asset<Texture2D> GlowTex;

        //AI values
        readonly float EnragedMultiplier = Main.expertMode ? 0.4f : 0.3f;
        readonly float DetectionRadius = 300f;
        readonly float RotationSpeed = 0.1f;
        readonly float RotationSpeedEnraged = 0.25f;
        readonly float TooClose = 50f;
        readonly float Amount = 1f / 30;
        readonly float ActiveToPassiveTimer = Timer.SecondsToFrames(3);
        readonly float ExplosionTimer = Timer.SecondsToFrames(5);

        readonly int speed = 3;
        readonly int speedEnraged = 6;

        private ref float AIState => ref NPC.ai[0];
        private ref float AITimer => ref NPC.ai[1];

        //Functions
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "VoidPort/Assets/BestiaryTextures/ScrapMine",
            };
        }

        public override void SetDefaults()
        {
            NPC.width = 66;
            NPC.height = 62;
            NPC.lifeMax = 220;
            NPC.damage = 0;
            NPC.defense = 100;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.25f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 2, 50);
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            SpawnModBiomes = [ModContent.GetInstance<Biomes.Void.VoidBiome>().Type];
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(
            [
                new FlavorTextBestiaryInfoElement("Mods.VoidPort.Bestiary.ScrapMine"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.Void.VoidBiome>().ModBiomeBestiaryInfoElement)
            ]);
		}

        public override void AI()
        {
            NPC.TargetClosest(true);

            //Enemy AI
            switch(AIState)
            {
                case (float)ScrapMineState.Inactive:
                    Inactive();
                    break;

                case (float)ScrapMineState.Active:
                    Active();
                    break;

                case (float)ScrapMineState.Enraged:
                    Enraged();
                    break;
            }

            //If the player is too close, explode
            if (Main.player[NPC.target].Distance(NPC.Center) < TooClose)
            {
                Main.player[NPC.target].ApplyDamageToNPC(NPC, NPC.lifeMax * 2, 0, 0, false);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            switch (AIState)
            {
                case (float) ScrapMineState.Inactive:
                    NPC.frame.Y = (int)ScrapMineFrames.Inactive * frameHeight;
                    break;
                
                case (float) ScrapMineState.Active:
                case (float) ScrapMineState.Enraged:
                    NPC.frame.Y = (int)ScrapMineFrames.ActiveEnraged * frameHeight;
                    break;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            //If hit, activate
            if (AIState == (float)ScrapMineState.Inactive)
            {
                AIState = (float)ScrapMineState.Active;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScrapItem>(), 1, 2, 3));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTex ??= ModContent.Request<Texture2D>(Texture);
            GlowTex ??= ModContent.Request<Texture2D>(Texture + "_Glow");

            //Draw the NPC
            bool isEnraged = AIState == (float)ScrapMineState.Enraged;
            Color NPCDrawColor = isEnraged ? NPC.GetAlpha(Color.Red) : NPC.GetAlpha(drawColor);

			Main.EntitySpriteDraw(
				NPCTex.Value,
				NPC.Center + new Vector2(0, NPC.gfxOffY + 4) - Main.screenPosition,
				NPC.frame,
				NPCDrawColor,
				NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0
            );

            //Draw the glowmask
            bool isActive = AIState == (float)ScrapMineState.Active || isEnraged;
            Color GlowMaskDrawColor = isActive ? NPC.GetAlpha(Color.White) : NPC.GetAlpha(Color.White) * 0.5f;

            //Draw the glowmask
			Main.EntitySpriteDraw(
				GlowTex.Value,
				NPC.Center + new Vector2(0, NPC.gfxOffY + 4) - Main.screenPosition,
				NPC.frame,
				GlowMaskDrawColor,
				NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0
            );

            return false;
        }

        private void Inactive()
        {
            //Slow down the enemy smoothly if the velocity is higher than zero
            if (NPC.velocity != Vector2.Zero)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, Amount);
            }

            if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < DetectionRadius)
            {
                NPC.defense = 20;
                AIState = (float)ScrapMineState.Active;
            }
        }

        private void Active()
        {
            //Enraged state check
            if (NPC.life <= NPC.lifeMax * EnragedMultiplier)
            {
                AIState = (float)ScrapMineState.Enraged;
                AITimer = 0;
            }

            //The player must be out of range for some seconds to avoid being chased
            if (Main.player[NPC.target].Distance(NPC.Center) < DetectionRadius)
            {
                AITimer = 0;
            }
            else
            {
                AITimer++;

                if (AITimer > ActiveToPassiveTimer)
                {
                    NPC.defense = 100;
                    AIState = (float)ScrapMineState.Inactive;
                    AITimer = 0;
                }
            }

            NPC.rotation += RotationSpeed;
            
            Vector2 desiredVelocity = NPC.DirectionTo(Main.player[NPC.target].Center) * speed;
            NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, Amount);
        }

        private void Enraged()
        {
            AITimer++;

            Vector2 desiredVelocity = NPC.DirectionTo(Main.player[NPC.target].Center) * speedEnraged;
            NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, Amount);

            //Explode after few seconds
            if (AITimer > ExplosionTimer)
            {
                Main.player[NPC.target].ApplyDamageToNPC(NPC, NPC.lifeMax * 2, 0, 0, false);
            }

            NPC.rotation += RotationSpeedEnraged;
        }
    }
}