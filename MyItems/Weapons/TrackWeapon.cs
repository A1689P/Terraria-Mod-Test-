using Microsoft.Xna.Framework;
using MyMod.MyProjectiles;
using Terraria;
using Terraria.DataStructures;
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
            Item.useTime = 30;
            Item.useAnimation = 50;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 30;
            Item.knockBack = 5;

            Item.channel = true;
        }
    }
}