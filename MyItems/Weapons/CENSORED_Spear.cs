using MyMod.MyProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyItems.Weapons
{
    public class CENSORED_Spear : ModItem
    {
        public override void SetDefaults()
        {
            Item.shoot = ModContent.ProjectileType<CENSORED>();
            Item.shootSpeed = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 30;
        }
    }
}
