using Microsoft.Xna.Framework;
using MyMod.MyBuffs;
using Terraria.ID;
using Terraria.ModLoader;

// 测试药水
namespace MyMod.MyItems.Consumables
{
    public class TestPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.DrinkParticleColors[Type] = [
                new Color(200,200,200),
                new Color(100,100,100),
                new Color(80,90,100)
            ];
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item3;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = 100;       // 一个银币
			Item.buffType = ModContent.BuffType<TestPotionBuff>();
			Item.buffTime = 540;
        }
    }
}