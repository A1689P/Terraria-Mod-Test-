using MyMod.MyProjectiles;
using Terraria;
using Terraria.ModLoader;

namespace MyMod.MyBuffs
{
    public class LightningBallBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if(player.ownedProjectileCounts[ModContent.ProjectileType<LightningBall>()] > 0)
            {
                player.buffTime[buffIndex] = 300;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;    // 具体作用似乎是让后面的buff可以在这个buff删除后向前移动一位(from deepseek)
            }
        }
    }
}
