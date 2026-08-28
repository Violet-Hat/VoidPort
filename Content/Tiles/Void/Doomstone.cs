using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace VoidPort.Content.Tiles.Void
{
    public class Doomstone : ModTile
    {
        public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(51, 48, 61));
			HitSound = SoundID.Tink;
            DustType = DustID.Obsidian;
			MineResist = 1.8f;
		}
		
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
        
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }
}