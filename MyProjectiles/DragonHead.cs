using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyMod.MyBuffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyProjectiles
{
    public class DragonHead : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionCannotBeFreed[Type] = true;   // 原版 625/628 同款：永远不被牺牲
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
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

        public override void OnSpawn(IEntitySource source)
        {
            int body1 = Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero,
            ModContent.ProjectileType<DragonBody>(), 0, 0f, Projectile.owner,
            ai0: Projectile.identity, ai1 : 0);       // 前一节的 identity 直接进 ai[0]

            int body2 = Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero,
            ModContent.ProjectileType<DragonBody>(), 0, 0f, Projectile.owner,
            ai0: Main.projectile[body1].identity, ai1 : 1);

            int tail = Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero,
            ModContent.ProjectileType<DragonTail>(), 0, 0f, Projectile.owner,
            ai0: Main.projectile[body2].identity);
        }

        // 临时诊断：区分「timeLeft 到期」和「被别人 Kill」
        public override void OnKill(int timeLeft)
        {
            Player p = Main.player[Projectile.owner];
            Mod.Logger.Info($"[Dragon] head died: timeLeft={timeLeft} slots={p.slotsMinions:F1}/{p.maxMinions} numMinions={p.numMinions} buff={p.HasBuff(ModContent.BuffType<DragonBuff>())} ownedHead={p.ownedProjectileCounts[Type]} active={Projectile.active}");
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if(player.dead || !player.active)
                player.ClearBuff(ModContent.BuffType<DragonBuff>());
            if(player.HasBuff(ModContent.BuffType<DragonBuff>()) 
                && player.ownedProjectileCounts[ModContent.ProjectileType<DragonBody>()] > 0)       // 如果body没了那头就没有必要了,为了确保右键buff可以清理这里保留对buff判断
                Projectile.timeLeft = 2;

            Vector2 tarPos = player.Center;
            Vector2 dir = tarPos - Projectile.Center;
            dir = dir.SafeNormalize(Vector2.Zero);
            int interval = 75;

            Projectile.Center = tarPos - dir * interval;    
            Projectile.velocity = Vector2.Zero; 
            Projectile.rotation = dir.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = dir.X >= 0 ? 1 : -1;       // 镜像翻转
        }
    }
}