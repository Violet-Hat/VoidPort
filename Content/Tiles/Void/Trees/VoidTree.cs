using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

using VoidPort.Content.Gores.Void;

namespace VoidPort.Content.Tiles.Void.Trees
{
	public class VoidTree : ModTree
	{
		public override TreePaintingSettings TreeShaderSettings => new()
        {
			UseSpecialGroups = true,
			SpecialGroupMinimalHueValue = 11f / 72f,
			SpecialGroupMaximumHueValue = 0.25f,
			SpecialGroupMinimumSaturationValue = 0.88f,
			SpecialGroupMaximumSaturationValue = 1f
		};

        //the tiles this tree can grow on
		public override void SetStaticDefaults()
        {
			GrowsOnTileId = [ModContent.TileType<SoftDoomstone>()];
		}

        //the sapling that grows into this tree
		public override int SaplingGrowthType(ref int style)
        {
			style = 0;
			return ModContent.TileType<VoidTreeSappling>();
		}

        //drop wood
		public override int DropWood()
		{
			return ModContent.ItemType<VoidWoodItem>();
		}

		//get the tree trunk texture
		public override Asset<Texture2D> GetTexture() 
		{
			return ModContent.Request<Texture2D>("VoidPort/Content/Tiles/Void/Trees/VoidTree");
		}

		//branch Textures
		public override Asset<Texture2D> GetBranchTextures() 
		{
			return ModContent.Request<Texture2D>("VoidPort/Content/Tiles/Void/Trees/VoidTree_Branches");
		}

		//top Textures
		public override Asset<Texture2D> GetTopTextures() 
		{
			return ModContent.Request<Texture2D>("VoidPort/Content/Tiles/Void/Trees/VoidTree_Tops");
		}

        public override int TreeLeaf() {
			return ModContent.GoreType<TreeGore>();
		}

        public override int CreateDust()
        {
            return DustID.Shadewood;
        }

		public override bool Shake(int x, int y, ref bool createLeaves)
        {
			Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), new Vector2(x, y) * 16, ModContent.ItemType<VoidWoodItem>());

            createLeaves = true;

			return false;
		}
	}
}