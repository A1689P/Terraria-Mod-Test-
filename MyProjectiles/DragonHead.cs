using Microsoft.Xna.Framework;
using MyMod.MyBuffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyProjectiles
{
    // 问题: 2空战攻击状态下会一直追不上(比如史莱姆女皇二阶段,后面改吧，应该要有个冲刺过程)
    //       4运动仍然不是非常丝滑 5还需要调整索敌范围，距离玩家范围的参数
    // 扩展: 光效，粒子特效
    public class DragonHead : ModProjectile
    {
        enum State{IDLE, CHASE, ATTACK, RETURN}
        State state = State.IDLE;
        Vector2 randomPoint = Vector2.Zero;
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
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon; 
            Projectile.minionSlots = 0f;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false; // 无视方块碰撞

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override bool MinionContactDamage() => true;
        
        public override void OnSpawn(IEntitySource source)
        {
            DragonTrail.Reset(Projectile.owner);

            int body1 = Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero,
            ModContent.ProjectileType<DragonBody>(), 20, 0f, Projectile.owner,
            ai0: Projectile.identity, ai1 : 0);       // 前一节的 identity 直接进 ai[0]

            int body2 = Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero,
            ModContent.ProjectileType<DragonBody>(), 20, 0f, Projectile.owner,
            ai0: Main.projectile[body1].identity, ai1 : 1);

            int tail = Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero,
            ModContent.ProjectileType<DragonTail>(), 20, 0f, Projectile.owner,
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

            NPC target = FindTarget(1000);       // 已经是范围内的目标
            if(Projectile.Distance(player.Center) > 2000)
                state = State.RETURN;
            switch (state)
            {
                case State.IDLE : Idle(player, target); break;
                case State.CHASE : Chase(player, target); break;
                case State.ATTACK : Attack(player, target); break;
                case State.RETURN : Return(player); break;
            }

            // 记录轨迹，供体节按弧长取点（本节在链首，弧长为 0）
            Projectile.localAI[1] = 0f;
            DragonTrail.Push(Projectile.owner, Projectile.Center);
        }

        private void Idle(Player player, NPC target)
        {
            if(target != null && Projectile.Distance(target.Center) < 800 && Projectile.Distance(player.Center) < 500)
            {
                state = State.CHASE;
                return;
            }

            Vector2 dir = Vector2.Zero;
            if(randomPoint == Vector2.Zero || randomPoint.Distance(player.Center) > 300 ||Projectile.Center.Distance(randomPoint) < 3f)     // 还未生成随机点或是已经靠近随机点
            {
                randomPoint = player.Center + Main.rand.NextVector2Circular(180, 140);
            }
            
            Vector2 preDir = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
            dir = randomPoint - Projectile.Center;

            float curAng = preDir.ToRotation();
            float wantAng = dir.ToRotation();
            float maxturn = MathHelper.ToRadians(12f);
            float diff = MathHelper.WrapAngle(wantAng - curAng);
            dir = (curAng + MathHelper.Clamp(diff, -maxturn, maxturn)).ToRotationVector2();

            float step = Projectile.Center.Distance(randomPoint) * 0.05f;
            if(step > 16) step = 16;
            
            Projectile.Center += dir * step;
            
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = dir.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = dir.X >= 0 ? 1 : -1;
        }

        private void Chase(Player player, NPC target)
        {
            if(target == null || Projectile.Distance(player.Center) > 1200f)
            {
                state = State.IDLE;
                return;
            }

            float dist = (target.Center - Projectile.Center).Length();
            if(dist < 50)
            {
                state = State.ATTACK;
                return;
            }
            else if(dist > 1000)
            {
                state = State.IDLE;
                return;
            }

            Vector2 tarPos = target.Center;
            Vector2 dir = tarPos - Projectile.Center;
            dir = dir.SafeNormalize(Vector2.Zero);
            dist *= 0.1f;       // 追逐敌人的时候跟随的参数提高

            float speed = dist > 16 ? 16 : dist;
            Projectile.Center = Projectile.Center + dir * speed;    
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = dir.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = dir.X >= 0 ? 1 : -1;       // 镜像翻转
        }

        private void Attack(Player player, NPC target)
        {
            if(target == null || Projectile.Distance(player.Center) > 1200f)
            {
                state = State.IDLE;
                return;
            }

            float dist = (target.Center - Projectile.Center).Length();
            if(dist > 75)
            {
                state = State.CHASE;
                return;
            }

            Vector2 predir = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
            Vector2 tardir = target.Center - Projectile.Center;
            Vector2 dir = dist > 50f ? tardir : predir;

            float curAng = predir.ToRotation();
            float wantAng = dir.ToRotation();
            float maxturn = MathHelper.ToRadians(30f);
            float diff = MathHelper.WrapAngle(wantAng - curAng);
            dir = (curAng + MathHelper.Clamp(diff, -maxturn, maxturn)).ToRotationVector2();

            int speed = 5;                     // 攻击状态下距离比较近此时用dist * arg会因为dist太小导致运动幅度很小，所以设置speed代替
            Projectile.Center += dir * speed;   // 设置位置
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = dir.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = dir.X >= 0 ? 1 : -1;       // 镜像翻转
        }   

        private void Return(Player player)
        {
            Projectile.Center = player.Center - Vector2.UnitY * 50;     // 回来的时候放在头上
            state = State.IDLE;
        }

        private NPC FindTarget(int range)
        {
            int attackTarget = -1;
            Projectile.Minion_FindTargetInRange(range,ref attackTarget,false);
            if(attackTarget != -1) return Main.npc[attackTarget];
            return null;
        }
    }
}