using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace MyMod.MyProjectiles
{
    // 制作一个测试激光，顺便测试自定义碰撞
    public class TestLaserProj : ModProjectile
    {
        private const int maxLength = 1000;
        private int Length = maxLength;
        private float scaleX = 1;       // 当前激光的x轴的比例
        private int existTime = 30;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;

            Projectile.width = 20;
            Projectile.height = 32;

            Projectile.friendly = true;
            Projectile.penetrate = -1;  
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = existTime;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ownerHitCheck = true;    // 用来检查玩家和弹幕之间是否存在墙,true时候表示隔着墙弹幕无伤害
            Projectile.usesLocalNPCImmunity = true; // 启用独立无敌帧
            Projectile.localNPCHitCooldown = 10;    // 10帧一次伤害，要和上面搭配
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.velocity = Vector2.Zero;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return base.GetAlpha(lightColor);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.position = player.position;

            // 修改scaleX用来碰撞检测，和绘制
            int existTimeOver2 = existTime / 2;
            if(Projectile.timeLeft >= existTimeOver2 && Projectile.timeLeft <= existTime) 
                scaleX = 2 - ((float)Projectile.timeLeft / existTimeOver2);
            else if(Projectile.timeLeft < existTimeOver2)  
                scaleX = (float)Projectile.timeLeft / existTimeOver2;
            
            scaleX *= 1.5f;
        }

        // 自定义碰撞    (Projectile.ownerHitCheck = true; 这个也会影响自定义碰撞的效果)
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center;
            Vector2 end = start + Vector2.UnitY.RotatedBy(Projectile.rotation) * Length;
            float width = Projectile.width * scaleX;
            float collisionPoint = 0;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(),targetHitbox.Size(),start,end,width,ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];

            float rot = Projectile.rotation;
            Vector2 ori = new Vector2(texture.Width / 2, 0);

            Vector2 upscale = new Vector2(scaleX, 1);
            Vector2 midSclae = new Vector2(scaleX, Length / frameHeight);
            Vector2 downScale = new Vector2(scaleX, 1);

            Vector2 dir = Vector2.UnitY.RotatedBy(rot);
            Vector2 upPos = Projectile.position - Main.screenPosition;        // 一定记得要减去Main.screenPostion
            Vector2 midPos = upPos + dir * frameHeight;
            Vector2 downPos = midPos + dir * frameHeight * midSclae.Y;

            Rectangle up = new Rectangle(0,0,texture.Width,frameHeight);
            Rectangle mid = new Rectangle(0,frameHeight,texture.Width,frameHeight);
            Rectangle down = new Rectangle(0,2 * frameHeight,texture.Width,frameHeight);

            Main.EntitySpriteDraw(texture,upPos,up,Color.White,rot,ori,upscale,SpriteEffects.None);       // up   
            Main.EntitySpriteDraw(texture,midPos,mid,Color.White,rot,ori,midSclae,SpriteEffects.None);         // mid
            Main.EntitySpriteDraw(texture,downPos,down,Color.White,rot,ori,downScale,SpriteEffects.None);     // down
            return false;
        }
    }
}