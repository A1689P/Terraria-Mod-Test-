using MyMod.MyTiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyItems
{
    public class TestTileItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.autoReuse = true;

            Item.createTile = ModContent.TileType<TestTile>();
            Item.placeStyle = 0;

            Item.consumable = true;
            Item.maxStack = 9999;
        }
    }
}