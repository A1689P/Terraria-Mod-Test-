using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace MyMod.MyBuffs
{
    // 测试药水的buff,大概是用于增加临时生命(200)
    class TestPotionBuff : ModBuff
    {
        int addLife = 200;
        public override LocalizedText Description => base.Description.WithFormatArgs(10);
        public override void Update(Player player, ref int buffIndex)
        {
            player.statLifeMax2 += addLife;
        }
    }
}