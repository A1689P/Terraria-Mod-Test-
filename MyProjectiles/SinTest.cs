using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyProjectiles
{
    public class SinTest : ModProjectile
    {
        private Vector2 StartAngle;
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void OnSpawn(IEntitySource source)
        {
            StartAngle = Projectile.velocity;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            float angle = MathF.Cos(Projectile.ai[0]);
            angle = MathF.Atan(angle);
            Projectile.velocity = StartAngle.RotatedBy(angle);
            // 使用弧度制控制旋转
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 生成粒子效果
            Dust dust = Dust.NewDustDirect(Projectile.Center,1,1,DustID.FireworkFountain_Blue);
            dust.velocity = Vector2.Zero;
            dust.noGravity = true;
        }
    }
}
