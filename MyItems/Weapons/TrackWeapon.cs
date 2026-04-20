using MyMod.MyProjectiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyItems.Weapons
{
    public class TrackWeapon : ModItem
    {
        public override void SetDefaults()
        {
            Item.shoot = ModContent.ProjectileType<TrackBullet>();
            Item.shootSpeed = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 30;
        }
    }
}