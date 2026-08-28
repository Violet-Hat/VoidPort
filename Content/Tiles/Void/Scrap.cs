using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;

using VoidPort.Common.Tiles;

namespace VoidPort.Content.Tiles.Void
{
    public class Scrap : ModTile
    {
		private static Asset<Texture2D> Glowmask;

        public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(94, 79, 96));
			HitSound = SoundID.Tink;
            DustType = DustID.Iron;
			MineResist = 1f;
		}
		
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

        public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            Tile tile = Framing.GetTileSafely(i, j);
			tile.TileFrameX += (short)(234 * (i % 2));
			tile.TileFrameY += (short)(90 * (j % 3));
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");

			//Get the tile in this position and offset
			Tile tile = Framing.GetTileSafely(i, j);
			Vector2 pos = TileGlobal.TileCustomPosition(i, j);

			//Draw the glowmask
			spriteBatch.Draw(
				Glowmask.Value,
				pos,
				new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
				Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}