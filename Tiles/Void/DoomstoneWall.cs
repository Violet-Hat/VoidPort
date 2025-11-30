using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace VoidPort.Tiles.Void
{
    public class DoomstoneWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(7, 7, 17));
            DustType = DustID.Stone;
        }
    }
}