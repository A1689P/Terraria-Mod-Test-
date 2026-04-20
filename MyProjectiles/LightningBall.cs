using System;
using MyMod.MyBuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace MyMod.MyProjectiles
{
    public class LightningBall : ModProjectile
    {
        private int frameSpeed = 5;
        private int shootCD = 30;
        private float shootTimer
        {
            get {return Projectile.ai[1];}
            set {Projectile.ai[1] = value;}
        }

        public override void SetStaticDefaults()
        {
            // 设定弹幕所具有的动画帧数
            Main.projFrames[Projectile.type] = 4;

            // 似乎是右键锁定的必要代码
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            // 让邪教徒对这个弹幕有抗性，邪教徒对追踪弹幕都具有抗性
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;     // 如果是通过碰撞造成伤害，这个字段要和是否找到目标一致，不然没查找到目标也造成伤害
            // Only determines the damage type
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon; 
            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 1f;
            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false; // 无视方块碰撞
        }

        public override bool? CanCutTiles()
        {
            // 是否会破坏草，罐子之类的东西
            // 设置为true有可能意外破坏蜜蜂幼虫
            return false;
        }

        public override bool MinionContactDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            // 检查角色状态，buff状态
            if(player.dead || !player.active)
                player.ClearBuff(ModContent.BuffType<LightningBallBuff>());
            if(player.HasBuff(ModContent.BuffType<LightningBallBuff>()))
                Projectile.timeLeft = 2;

            AnimeControl();

            // 弹幕位置计算
            int totalnum = player.ownedProjectileCounts[Type];
            float angle = (Projectile.minionPos + 1) / ((float)totalnum + 1) * MathF.PI;
            Vector2 offset = -Vector2.UnitX * 100;
            offset = offset.RotatedBy(angle);
            Projectile.Center = player.Center + offset;

            // 攻击相关代码
            shootTimer++;
            NPC target = FindTarget2();
            if(target != null)
            {
                if(shootTimer > shootCD)
                {
                    Vector2 dir = target.Center - Projectile.Center;
                    dir.Normalize();
                    var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(),Projectile.Center,dir * 20,ProjectileID.BloodArrow,Projectile.damage,0,Main.myPlayer);
                    shootTimer = 0;
                }
            }
        }

        // 从wiki上搞下来的查找可锁定目标的代码
        private void FindTarget1()
        {
            for (int i = 0; i < Main.maxNPCs; i++) {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy()) {
                /* If we are here, that means we found an NPC that is:
                * active (alive)
                * chaseable (e.g. not a cultist archer)
                * max life bigger than 5 (e.g. not a critter)
                * can take damage (e.g. moonlord core after all it's parts are downed)
                * hostile
                * not immortal (e.g. not a target dummy)
                */
                }
            }
        }

        private NPC FindTarget2()
        {
            int attackRange = 600;
            int attackTarget = -1;
            Projectile.Minion_FindTargetInRange(attackRange,ref attackTarget,false);
            if(attackTarget != -1) return Main.npc[attackTarget];
            return null;
        }

        private void AnimeControl()
        {
            // 动画帧控制代码
            Projectile.frameCounter++;
            if(Projectile.frameCounter > frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                Projectile.frame %= Main.projFrames[Projectile.type];
            }
        }
    }
}