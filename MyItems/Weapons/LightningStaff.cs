using Microsoft.Xna.Framework;
using MyMod.MyBuffs;
using MyMod.MyProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyItems.Weapons
{
    public class LightningStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 40; 
            Item.width = 32;
			Item.height = 32;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item44;

            Item.DamageType = DamageClass.Summon;
            Item.noMelee = true;
            Item.buffType = ModContent.BuffType<LightningBallBuff>();
            Item.shoot = ModContent.ProjectileType<LightningBall>();
            
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType,2);
            var projectile = Projectile.NewProjectileDirect(source,position,velocity,type,damage,knockback,Main.myPlayer);
            return false;
        }
    }
}