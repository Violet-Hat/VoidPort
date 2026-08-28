using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;

using VoidPort.Common;
using VoidPort.Enums;
using VoidPort.Content.Tiles.Void;

namespace VoidPort.Content.NPCs.Void
{
    public class StoneSearcher : ModNPC
    {
        //Textures
        private static Asset<Texture2D> GlowTex;
        private static Asset<Texture2D> DrillTex;

        //AI values
        private float MinSpeed = 0f;
        private float MaxSpeed = 0f;
        readonly float MagicFloat = 0.01f;
        readonly float Amount = 1f / 20;
        readonly float SwitchTimer = Main.expertMode ? Timer.SecondsToFrames(8) : Timer.SecondsToFrames(10);
        readonly float ResetTimer = Timer.SecondsToFrames(15);

        readonly int EnemyOffSetY = 125;

        private bool IsSpeedSet = false;

        public ref float AIState => ref NPC.ai[0];
        public ref float AITimer => ref NPC.ai[1];

        //Functions
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(MinSpeed);
            writer.Write(MaxSpeed);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            MinSpeed = reader.Read();
            MaxSpeed = reader.Read();
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 30;
            NPC.lifeMax = 80;
            NPC.damage = 25;
            NPC.defense = 10;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 1, 10);
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            SpawnModBiomes = [ModContent.GetInstance<Biomes.Void.VoidBiome>().Type];
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(
            [
                new FlavorTextBestiaryInfoElement("Mods.VoidPort.Bestiary.StoneSearcher"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.Void.VoidBiome>().ModBiomeBestiaryInfoElement)
            ]);
		}

        public override void AI()
        {
            //Set the starting minimum / maximum speed to a random value
            if (!IsSpeedSet) SetEnemySpeed();

            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            //Face the player
            FacePlayer(player);

            //AI function calls
            switch (AIState)
            {
                case (float)StoneSearcherState.Hovering:
                    HoverChaser(true);
                    break;

                case (float)StoneSearcherState.Chasing:
                    HoverChaser(false);
                    break;

                case (float)StoneSearcherState.Drilling:
                    Drill();
                    break;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScrapItem>(), 2, 1, 3));
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTex ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            DrillTex ??= ModContent.Request<Texture2D>(Texture + "_Drill");

            //Draw the glowmask
            Main.spriteBatch.Draw(
                GlowTex.Value,
                NPC.Center + new Vector2(0, NPC.gfxOffY + 4) - Main.screenPosition,
                NPC.frame,
                Color.White * 0.5f,
                NPC.rotation, NPC.frame.Size() / 2, NPC.scale, 0, 0
            );

            //Draw the drill
            if (AIState == (float)StoneSearcherState.Drilling)
            {
                Vector2 offSetDrill = new(DrillTex.Width() / 2, DrillTex.Height() / 2);
                Main.spriteBatch.Draw(
                    DrillTex.Value,
                    NPC.Center + new Vector2(0, NPC.gfxOffY + 4) - Main.screenPosition,
                    new Rectangle(0, 0, 66, 66),
                    Color.White,
                    NPC.rotation, offSetDrill, NPC.scale, SpriteEffects.None, 0
                );
            }
        }

        private void SetEnemySpeed()
        {
            MinSpeed = WorldGen.genRand.NextFloat(1.5f, 2.51f);
            MaxSpeed = WorldGen.genRand.NextFloat(5.5f, 6.51f);
            NPC.netUpdate = true;

            IsSpeedSet = true;
        }

        private void FacePlayer(Player player)
        {
            Vector2 vector = new(NPC.Center.X, NPC.Center.Y);
            float rotationX = player.Center.X - vector.X;
            float rotationY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)rotationY, (double)rotationX);
        }

        private void HoverChaser(bool isHovering)
        {
            AITimer++;

            //Calculate position
            Vector2 pos;
            if (isHovering)
            {
                pos = new(Main.player[NPC.target].Center.X, Main.player[NPC.target].Center.Y - EnemyOffSetY);
            }
            else
            {
                pos = Main.player[NPC.target].Center;
            }

            //Calculate velocity depending on distance and set NPC velocity
            float vel = MathHelper.Clamp(NPC.Distance(pos) * MagicFloat, MinSpeed, MaxSpeed);
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(pos) * vel, Amount);

            //Change state to drilling
            if (AITimer > SwitchTimer)
            {
                AIState = (float)StoneSearcherState.Drilling;
            }
        }

        private void Drill()
        {
            AITimer++;

            //Play a drilling sound
            if (AITimer % 15 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item23, NPC.Center);
            }

            //Get the desired velocity and set the NPC velocity
            Vector2 desiredVelocity = NPC.DirectionTo(Main.player[NPC.target].Center) * MaxSpeed;
            NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, Amount);

            //Reset
            if (AITimer > ResetTimer && Main.netMode != NetmodeID.MultiplayerClient)
            {
                //Randomized Minimum / Maximum speed and AIState, AITimer returns to zero
                MinSpeed = WorldGen.genRand.NextFloat(1.5f, 2.51f);
                MaxSpeed = WorldGen.genRand.NextFloat(5.5f, 6.51f);
                AIState = WorldGen.genRand.Next(2);
                AITimer = 0;

                //Sync the state
                NPC.netUpdate = true;
            }
        }
    }
}