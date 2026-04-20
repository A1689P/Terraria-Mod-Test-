using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyNPCs
{
    // 写一个普通的史莱姆
    public class TestSilme : ModNPC
    {
        private enum ActionState
        {
            idle,
            jump
        }

        private enum Frame
        {
            frame1,
            frame2
        }

        private ActionState AI_State;
        private Frame AI_Frame;
        private float timer{
            get{ return NPC.ai[0]; } 
            set{ NPC.ai[0] = value; }
        }
        private float frameTimer
        {
            get{ return NPC.ai[1]; }
            set{ NPC.ai[1] = value; }
        }

        // 下面会对这两个变量随机赋值
        private float hor_speed = 4;
        private float vel_speed = 6;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;       // 设置动画帧数
        }
        public override void SetDefaults()
        {
            NPC.width = 20;                     // NPC的宽度
            NPC.height = 40;                   // NPC的高度
            NPC.damage = 30;                    // NPC的接触伤害
            NPC.defense = 20;                   // NPC的防御力
            NPC.lifeMax = 300;                // NPC的最大生命值
            NPC.HitSound = SoundID.NPCHit2;     // NPC被击中的声音 官方ID可见此处 https://terraria.wiki.gg/wiki/Sound_IDs
            NPC.DeathSound = SoundID.NPCDeath2; // NPC死亡时的声音
            NPC.value = 200f;                 // NPC的价值(死亡后掉落的钱)
            NPC.knockBackResist = 0.3f;           // NPC的击退抗性
            NPC.boss = false;                    // 是否为BOSS
            NPC.townNPC = false;                // 是否为城镇NPC
            NPC.noGravity = false;               // NPC是否受重力影响
            NPC.noTileCollide = false;           // NPC是否与方块碰撞
            NPC.netAlways = true;               // NPC是否总是同步到客户端
            NPC.aiStyle = -1;                   // 自定义AI样式自定义为-1 官方ID可见此处 
            
            // 还有额外的旗子设置
        }

        // 自定义AI
        public override void AI()
        {
            switch(AI_State)
            {
                case ActionState.idle :
                    Idle();
                    break;
                case ActionState.jump :
                    Jump();
                    break;
            }

            // 如果实体现在正在水中，那么要令其可以漂浮在水面上
            if(NPC.wet)
            {
                NPC.noGravity = true;
                // 如果此时垂直速度向下，那就提供一个向上的速度
                if(NPC.velocity.Y >= -2)
                    NPC.velocity.Y -= 0.6f;
                // 在水面上没法跳，但是仍然要追踪玩家
                NPC.TargetClosest(true);
                NPC.velocity.X = NPC.direction * hor_speed / 3;
            }
            else
                NPC.noGravity = false;
        }

        // 用来实现动画效果
        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            frameTimer++;
            // 每30帧换一次
            if(frameTimer >= 10)
            {
                frameTimer = 0;
                if(AI_Frame == Frame.frame1)
                {
                    NPC.frame.Y = (int)Frame.frame2 * frameHeight;
                    AI_Frame = Frame.frame2;
                }
                else
                {
                    NPC.frame.Y = (int)Frame.frame1 * frameHeight;
                    AI_Frame = Frame.frame1;
                }
            }
        }

        // 掉落物函数
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // 有1/2的概率掉落蜜蜂枪
            npcLoot.Add(ItemDropRule.Common(ItemID.BeeGun,2));
            // 从几个物品中掉落一个
            // npcLoot.Add(ItemDropRule.OneFromOptions(1,ItemID.MagicDagger,ItemID.PhilosophersStone,ItemID.StarCloak));
            // 在专家模式有着更高掉率(普通模式1/2,专家1/1)
            // npcLoot.Add(ItemDropRule.NormalvsExpert(ItemID.BlessedApple,2,1));
            // 有条件掉落(如果在猩红世界并且不是专家,掉落猩红矿30-100个)
            // npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsCrimsonAndNotExpert(),ItemID.CrimtaneOre,1,30,100));
        }

        // 和生成有关的函数
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            // 白天并且在世界中心1/3范围内(Main.spawnTileX似乎是世界中心点)
            if(Main.dayTime && Math.Abs(spawnInfo.SpawnTileX - Main.spawnTileX) < Main.maxTilesX / 6)
                return 0.1f;
            return 0;
        }

        private void Idle()
        {
            // 通过垂直速度判断是否落地
            float SpeedY = NPC.velocity.Y;
            if(SpeedY != 0)
            {
                // 在还没有落地的时候需要保持水平速度，不然会被很低的墙卡住，显得很呆
                NPC.velocity.X = NPC.direction * hor_speed;
                timer = 0;
            }
            else
            {
                // 落地后开始计时，并且水平速度归零
                timer++;
                NPC.velocity.X = 0;
            }
                
            // 如果落地了，并且过了60帧(1s)
            if(timer >= 60)
            {
                AI_State = ActionState.jump;
            }
        }

        private void Jump()
        {
            NPC.TargetClosest(true);
            hor_speed = Main.rand.NextFloat(3f,4f);
            vel_speed = Main.rand.NextFloat(7f,8f);
            NPC.velocity = new Vector2(NPC.direction * hor_speed,-vel_speed);
            AI_State = ActionState.idle;
        }
    }
}