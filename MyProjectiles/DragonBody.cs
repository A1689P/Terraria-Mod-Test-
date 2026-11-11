using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyProjectiles
{
    public class DragonBody : ModProjectile
    {
        public override string Texture => "MyMod/MyProjectiles/DragonBody1";
        public static Asset<Texture2D> BodyTex1;
        public static Asset<Texture2D> BodyTex2;
        public override void SetStaticDefaults()
        {
            BodyTex1 = ModContent.Request<Texture2D>("MyMod/MyProjectiles/DragonBody1");
            BodyTex2 = ModContent.Request<Texture2D>("MyMod/MyProjectiles/DragonBody2");
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 18;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon; 
            Projectile.minionSlots = 0.5f;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false; // 无视方块碰撞
        }

        public override void AI()
        {
            Projectile pre = FindPrev();
            if(pre == null) return;

            Player player = Main.player[Projectile.owner];
            if (player.ownedProjectileCounts[ModContent.ProjectileType<DragonHead>()] > 0)
                Projectile.timeLeft = 2;

            Vector2 tarPos = pre.Center;
            Vector2 dir = tarPos - Projectile.Center;
            dir = dir.SafeNormalize(Vector2.Zero);
            int interval = 15;

            Projectile.Center = tarPos - dir * interval;    
            Projectile.velocity = Vector2.Zero; 
            Projectile.rotation = dir.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = dir.X >= 0 ? 1 : -1;
        }

        public override void OnSpawn(IEntitySource source) => Projectile.localAI[0] = -1;

        // 查找前一个体节
        private Projectile FindPrev()
        {
            int id = (int) Projectile.ai[0];
            int hint = (int)Projectile.localAI[0];

            if(hint >= 0 && hint < Main.maxProjectiles)
            {
                Projectile p = Main.projectile[hint];
                if(p.active && p.owner == Projectile.owner && p.identity == id)
                    return p;
            }

            for(int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if(p.active && p.owner == Projectile.owner && p.identity == id)
                {
                    Projectile.localAI[0] = i;
                    return p;
                }
            }
            return null;
        }

        // 被清除的时候要令后面的接上前面
        public override void OnKill(int timeLeft)
        {
            int myid = Projectile.identity;
            float mypre = Projectile.ai[0];
            for(int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if(p.active && p.owner == Projectile.owner && p.ai[0] == myid)
                {
                    p.ai[0] = mypre;
                    // p.ai[1] = 1f;
                    p.netUpdate = true;
                    p.localAI[0] = -1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int id = (int)Projectile.ai[1] % 2;
            Texture2D tex = id == 0 ? BodyTex1.Value : BodyTex2.Value;

            // 翻转
            SpriteEffects effects = Projectile.spriteDirection == -1 
                ? SpriteEffects.FlipHorizontally 
                : SpriteEffects.None;

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White,
                Projectile.rotation, tex.Size() * 0.5f, 1, effects);
            return false;
        }
    }
}