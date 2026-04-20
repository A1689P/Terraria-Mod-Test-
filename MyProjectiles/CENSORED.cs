using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyMod.MyBuffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace MyMod.MyProjectiles
{
    // 灵感来源于脑叶公司
    public class CENSORED : ModProjectile
    {
        private bool IsHited
        {
            get {return Projectile.ai[0] != 0;}
            set {Projectile.ai[0] = value ? 1 : 0;}
        }

        private int targetWho
        {
            get {return (int)Projectile.ai[1];}
            set {Projectile.ai[1] = value;}
        }

        private const float minScale = 0.75f;
        private const float maxScale = 2f;

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;

            Projectile.friendly = true;
            Projectile.penetrate = 3;  
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true;    // 用来检查玩家和弹幕之间是否存在墙,true时候表示隔着墙弹幕无伤害
            Projectile.usesLocalNPCImmunity = true; // 启用独立无敌帧
            Projectile.localNPCHitCooldown = 10;    // 10帧一次伤害，要和上面搭配
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale *= Main.rand.NextFloat(minScale,maxScale);
        }
        
        public override void AI()
        {
            if(IsHited)
            {
                NPC target = Main.npc[targetWho];
                if(!target.active)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.velocity = target.velocity;      // 和击中的目标一起运动
                Projectile.ai[2]++;
                if(Projectile.ai[2] >= 10)
                {
                    target.SimpleStrikeNPC(1,0);
                    Projectile.ai[2] = 0;
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft = 120;
            IsHited = true;
            targetWho = target.whoAmI;  // 获取ID
            target.AddBuff(ModContent.BuffType<Panic>(),180);
        }

        public override bool? CanDamage()
        {
            return !IsHited;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            // 这里null表示绘制整个纹理
            // tex.size() / 2 获取纹理图片中心点
            Main.spriteBatch.Draw(tex,Projectile.Center - Main.screenLastPosition,null,lightColor * Projectile.Opacity,
            Projectile.rotation,tex.Size() / 2,Projectile.scale,SpriteEffects.None,0);
            return false;
        }
    }
}