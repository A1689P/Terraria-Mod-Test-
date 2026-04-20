using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace MyMod.MyProjectiles
{
    // 实现追踪效果的弹幕
    public class TrackBullet : ModProjectile
    {
        private float trackDistance = 200;
        private NPC target = null;
        private float angelVel = 0.2f;
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if(target != null && target.active)
            {
                Vector2 dir = target.Center - Projectile.Center;
                float angle1 = dir.ToRotation();
                float angle2 = Projectile.velocity.ToRotation();
                float angle = angle1 - angle2;
                if(angle < -MathHelper.Pi) angle = MathHelper.TwoPi + angle;    // 转成正的角度
                else if(angle > MathHelper.Pi) angle = -MathHelper.TwoPi + angle;   // 变成负的角度

                float flag;
                if(angle < 0) flag = -1;
                else flag = 1;
                Projectile.velocity = Projectile.velocity.RotatedBy(flag * angelVel);
            }
            else SearchTarget();

            CreateDust();
        }

        private void SearchTarget()
        {
            foreach(NPC npc in Main.npc)
            {
                if(npc.active && !npc.friendly && !npc.townNPC)
                {
                    Vector2 dis = npc.Center - Projectile.Center;
                    if(dis.Length() < trackDistance)
                    {
                        target = npc;
                        break;
                    }
                }
            }
        }

        private void CreateDust()
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center,1,1,DustID.FireworkFountain_Blue);
            dust.velocity = Vector2.Zero;
            dust.noGravity = true;
        }
    }
}
