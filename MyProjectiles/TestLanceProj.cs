using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyProjectiles
{
    public class TestLanceProj : ModProjectile
    {
        private float Offset = 80;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true; // Sync this projectile if a player joins mid game.

			// The width and height do not affect the collision of the Jousting Lance because we calculate that separately (see Colliding() below)
			Projectile.width = 25;
			Projectile.height = 25;

			Projectile.aiStyle = -1;

			Projectile.friendly = true; // Player shot projectile. Does damage to enemies but not to friendly Town NPCs.
			Projectile.penetrate = -1; // Infinite penetration. The projectile can hit an infinite number of enemies.
			Projectile.tileCollide = false; // Don't kill the projectile if it hits a tile.
			Projectile.scale = 1f; // The scale of the projectile. This only effects the drawing and the width of the collision.
			Projectile.ownerHitCheck = true; // Make sure the owner of the projectile has line of sight to the target (aka can't hit things through tile).
			Projectile.DamageType = DamageClass.MeleeNoSpeed; // Set the damage to melee damage.
            Projectile.timeLeft = 2;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 * 3;
            // Projectile.velocity = Vector2.Zero;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 mousePos = Main.MouseWorld;
            Vector2 dir = mousePos - player.position;
            dir.Normalize();
            Projectile.position = player.position + dir * Offset + Vector2.UnitY * 40;
            Projectile.rotation = dir.ToRotation() + MathHelper.PiOver4 * 3;

            if (player.channel)
            {
                Projectile.ai[0]++; // 将ai[0]作为计时器
                
                // 让玩家保持使用物品
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
            else Projectile.ai[0] = 0;

            if(Projectile.ai[0] > 0)
            {
                Projectile.timeLeft = 2;
            }

            // 生成粒子效果，用来调试
            Dust dust = Dust.NewDustDirect(Projectile.position,1,1,DustID.FireworkFountain_Blue);
            dust.velocity = Vector2.Zero;
            dust.noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 pos = Projectile.position - Main.screenPosition;        // 一定记得要减去Main.screenPostion
            float rot = Projectile.rotation;
            Vector2 ori = new Vector2(texture.Width / 2,texture.Height / 2);
            Vector2 scale = new Vector2(1,1);

            Main.EntitySpriteDraw(texture,pos,null,Color.White,rot,ori,scale,SpriteEffects.None);
            return false;
        }
    }
}