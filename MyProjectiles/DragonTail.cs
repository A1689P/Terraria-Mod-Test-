using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyProjectiles
{
    public class DragonTail : ModProjectile
    {
        Vector2 lastMove = Vector2.Zero;
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
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon; 
            Projectile.minionSlots = 0f;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false; // 无视方块碰撞
        }

        public override bool MinionContactDamage() => true;

        public override void AI()
        {
            Projectile pre = FindPrev();
            if(pre == null) return;

            Player player = Main.player[Projectile.owner];
            if (player.ownedProjectileCounts[ModContent.ProjectileType<DragonHead>()] > 0)
                Projectile.timeLeft = 2;

            // 沿龙头轨迹取点：本节 = 轨迹上「前一节的距离 + 间距」处
            const float spacing = 16f;
            Projectile.localAI[1] = pre.localAI[1] + spacing;

            if (DragonTrail.Sample(Projectile.owner, Projectile.localAI[1], out Vector2 point))
            {
                Projectile.Center = point;
            }
            else
            {
                // 轨迹还不够长（刚生成）就先按直线摆开
                Vector2 dir = (Projectile.Center - pre.Center).SafeNormalize(Vector2.UnitX);
                Projectile.Center = pre.Center + dir * spacing;
            }

            Vector2 look = pre.Center - Projectile.Center;
             if (look.LengthSquared() > 0.01f)
            {
                Projectile.rotation = look.ToRotation() + MathHelper.PiOver2;
                Projectile.spriteDirection = look.X >= 0 ? 1 : -1;
            }

            Projectile.velocity = Vector2.Zero;
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

        public override bool? CanHitNPC(NPC target)
        {
            Projectile head = FindHead();
            if(head == null) return false;
            return head.localNPCImmunity[target.whoAmI] <= 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile head = FindHead();
            if (head != null)
                head.localNPCImmunity[target.whoAmI] = 8;        // 打上冷却，后面几节自动跳过
        }

        private Projectile FindHead(){
            int headType = ModContent.ProjectileType<DragonHead>();
            Projectile head = null;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == Projectile.owner && p.type == headType)
                {
                    head = p;
                    break;
                }
            }
            return head;
        }
    }
}