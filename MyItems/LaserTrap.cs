using MyMod.MyTiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyItems
{
    public class LaserTrap : ModItem
    {
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            
            Item.createTile = ModContent.TileType<LaserTrapTile>();
            Item.placeStyle = 0;

            Item.consumable = true;
            Item.maxStack = 9999;
            Item.damage = 30;
        }
    }
}