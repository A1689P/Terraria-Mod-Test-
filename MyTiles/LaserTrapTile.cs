using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MyMod.MyProjectiles;

namespace MyMod.MyTiles
{
    public class LaserTrapTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;     
            Main.tileCut[Type] = false; 
            Main.tileBlockLight[Type] = true;  
            Main.tileLavaDeath[Type] = false;  
            Main.tileWaterDeath[Type] = false; 

            HitSound = SoundID.Dig;
            DustType = DustID.Dirt;
            MineResist = 1;     // 被挖掘的时候所受的伤害系数(越大越难挖)
            MinPick = 20;       // 稿力要求
        }

        public override void HitWire(int i, int j)
        {
            Vector2 pos = new Vector2(i * 16 + 8,j * 16 + 8);
            Vector2 dir = new Vector2(1,0);
            int speed = 10;
            Projectile.NewProjectile(null,pos,dir * speed,ProjectileID.BeeArrow,10,0);
        }
    }
}