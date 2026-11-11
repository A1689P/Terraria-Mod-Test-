using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyProjectiles
{
    public class DragonTail : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionCannotBeFreed[Type] = true;   // 原版 628 同款
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 46;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon; 
            Projectile.minionSlots = 0f;
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
    }
}