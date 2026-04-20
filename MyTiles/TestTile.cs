using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyTiles
{
    public class TestTile : ModTile
    {
        // 物块相关设置
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = false;
            Main.tileSolid[Type] = true;
            Main.tileNoAttach[Type] = false;
            Main.tileSolidTop[Type] = false;
            Main.tileCut[Type] = false;
            Main.tileMergeDirt[Type] = false;
            Main.tileWaterDeath[Type] = false;
            Main.tileLavaDeath[Type] = false;
            Main.tileBlockLight[Type] = true;

            HitSound = SoundID.Dig;
            DustType = DustID.Dirt;
            MineResist = 1;     // 被挖掘的时候所受的伤害系数(越大越难挖)
            MinPick = 20;       // 稿力要求
        }
    }
}