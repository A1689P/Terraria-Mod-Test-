using MyMod.MyProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyItems.Weapons
{
    public class TestLaser : ModItem
    {
        public override void SetDefaults()
        {
            Item.shoot = ModContent.ProjectileType<TestLaserProj>();
            Item.shootSpeed = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;

            Item.damage = 60;
            Item.crit = 30;
            Item.mana = 10;
            Item.knockBack = 5;
            Item.channel = true;
        }
    }
}