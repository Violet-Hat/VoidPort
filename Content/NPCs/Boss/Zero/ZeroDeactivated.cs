using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoidPort.Content.NPCs.Boss.Zero
{
	public class ZeroDeactivated : ModNPC
	{
		private Asset<Texture2D> NPCTex;
		private Asset<Texture2D> ShieldTex;
		private Asset<Texture2D> RingTex;
		
		private float rotCounter = 0;
		
		public override void SetDefaults()
		{
			NPC.lifeMax = 1010;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 114;
			NPC.height = 136;
			NPC.knockBackResist = 0f;
			NPC.npcSlots = 0f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.immortal = true;
			NPC.hide = true;
			NPC.dontTakeDamage = true;
			NPC.behindTiles = true;
			NPC.dontCountMe = true;
			NPC.aiStyle = -1;
		}
		
		public override bool CheckActive()
		{
			return false;
		}

        public override bool CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

		public override void DrawBehind(int index)
		{
			Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
		}
		
		public override void AI()
		{
			rotCounter += 0.1f;
			
			if(rotCounter >= 360)
			{
				rotCounter = 0;
			}
		}
		
		public override bool PreDraw(SpriteBatch spritebatch, Vector2 screenPos, Color drawColor)
		{
			NPCTex = ModContent.Request<Texture2D>(Texture);
			ShieldTex ??= ModContent.Request<Texture2D>(Texture + "_Shield");
			RingTex ??= ModContent.Request<Texture2D>(Texture + "_Ring");
			
			float num = MathHelper.ToRadians(rotCounter);
			Vector2 offSetShield = new(ShieldTex.Width() / 2, ShieldTex.Height() / 2);
			Vector2 offSetRing = new(RingTex.Width() / 2, RingTex.Height() / 2);
			
			//Draw the NPC
			Main.EntitySpriteDraw(
				NPCTex.Value,
				NPC.Center - screenPos,
				NPC.frame,
				NPC.GetAlpha(new Color(0.85f, 0.85f, 0.85f)),
				NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			//Draw the shield
			Main.EntitySpriteDraw(
				ShieldTex.Value,
				NPC.Center - screenPos,
				null,
				NPC.GetAlpha(Color.DarkRed) * 0.5f,
				NPC.rotation, offSetShield, NPC.scale * 0.5f, SpriteEffects.None, 0);

			//Draw the ring
			Main.EntitySpriteDraw(
				RingTex.Value,
				NPC.Center - screenPos,
				null,
				NPC.GetAlpha(new Color(0.85f, 0.85f, 0.85f)),
				NPC.rotation + num, offSetRing, NPC.scale, SpriteEffects.None, 0);
			
			return false;
		}
	}
}