using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;

namespace MyMod.MyProjectiles
{
    // 用来测试圆周运动(和椭圆运动(还不知到怎么实现))的弹幕代码
    public class CircleTest : ModProjectile
    {
        private Vector2 trackPos;
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 pos = (Projectile.Center - player.Center).SafeNormalize(Vector2.Zero).RotatedBy(0.1f) * 100;      // 需要追击的点
            if(pos != Vector2.Zero)
            {
                pos = player.Center + pos;      // 转成相对玩家的位置
                Vector2 dir = pos - Projectile.Center;
                dir = dir.SafeNormalize(Vector2.Zero);
                Projectile.velocity = dir * 15;
            }

            // 生成粒子效果
            Dust dust = Dust.NewDustDirect(Projectile.Center,1,1,DustID.FireworkFountain_Blue);
            dust.velocity = Vector2.Zero;
            dust.noGravity = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            trackPos = player.Center + Vector2.UnitY * 100;
        }
    }
}