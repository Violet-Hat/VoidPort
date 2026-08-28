using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using VoidPort.Content.Projectiles.Void;

namespace VoidPort.Content.Items.Void
{
    public class VoidSaber : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 48;
			Item.height = 48;
            Item.damage = 10;
			Item.knockBack = 4.5f;
			Item.useAnimation = 12;
			Item.useTime = 12;
			Item.UseSound = SoundID.Item1;
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.buyPrice(gold: 1);
            Item.useStyle = ItemUseStyleID.Swing;
			Item.shoot = ModContent.ProjectileType<VoidSaberProjectile>();
            Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.shootsEveryUse = true;
			Item.autoReuse = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			float adjustedItemScale = player.GetAdjustedItemScale(Item);
			Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
			NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);

			return base.Shoot(player, source, position, velocity, type, damage, knockback);
		}
    }
}