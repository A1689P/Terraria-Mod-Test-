using MyMod.MyProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyItems.Weapons
{
    public class CircleTest_Weapon : ModItem
    {
        public override void SetDefaults()
        {
            Item.shoot = ModContent.ProjectileType<CircleTest>();
            Item.shootSpeed = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 30;
        }
    }
}
