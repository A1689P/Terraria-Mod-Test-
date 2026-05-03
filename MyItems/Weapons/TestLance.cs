using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyMod.MyProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyItems.Weapons
{
    public class TestLance : ModItem
    {
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 30;
            Item.knockBack = 100;
            Item.channel = true;

            Item.shoot = ModContent.ProjectileType<TestLanceProj>();
            Item.shootSpeed = 10;
            Item.noMelee = true;
        }

        public override bool CanShoot(Player player)
        {
            // 确保只有一个弹幕射出
            return !(player.ownedProjectileCounts[ModContent.ProjectileType<TestLanceProj>()] > 0);
        }
    }
}
