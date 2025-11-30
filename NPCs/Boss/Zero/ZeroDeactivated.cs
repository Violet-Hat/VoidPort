using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoidPort.NPCs.Boss.Zero
{
	public class ZeroDeactivated : ModNPC
	{
		private Asset<Texture2D> NPCTexture;
		private Asset<Texture2D> ShieldTexture;
		private Asset<Texture2D> RingTexture;
		
		private float rotationCounter;
		
		public override void SetDefaults()
		{
			NPC.lifeMax = 10;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 114;
			NPC.height = 136;
			NPC.knockBackResist = 0f;
			NPC.npcSlots = 0f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.behindTiles = true;
			NPC.dontCountMe = true;
			NPC.aiStyle = -1;
		}
		
		public override bool CheckActive()
		{
			return false;
		}
		
		public override void AI()
		{
			rotationCounter += 0.1f;
			
			if(rotationCounter >= 360)
			{
				rotationCounter = 0;
			}
		}
		
		public override bool PreDraw(SpriteBatch spritebatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture = ModContent.Request<Texture2D>(Texture);
			ShieldTexture ??= ModContent.Request<Texture2D>("VoidPort/NPCs/Boss/Zero/ZeroShield");
			RingTexture ??= ModContent.Request<Texture2D>("VoidPort/NPCs/Boss/Zero/ZeroShieldRing");
			
			float num = MathHelper.ToRadians(rotationCounter);
			Vector2 offSetRing = new Vector2(RingTexture.Width() / 2, RingTexture.Width() / 2);
			Vector2 offSetShield = new Vector2(ShieldTexture.Width() / 2, ShieldTexture.Width() / 2);
			
			Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(ShieldTexture.Value, NPC.Center - screenPos, null, NPC.GetAlpha(Color.DarkRed) * 0.5f, NPC.rotation, offSetShield, NPC.scale * 0.5f, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(RingTexture.Value, NPC.Center - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation + num, offSetRing, NPC.scale, SpriteEffects.None, 0);
			
			return false;
		}
	}
}