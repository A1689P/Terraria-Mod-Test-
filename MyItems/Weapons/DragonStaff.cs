using Microsoft.Xna.Framework;
using MyMod.MyBuffs;
using MyMod.MyProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.MyItems.Weapons
{
    public class DragonStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 1f;   // 一次一个召唤栏位
        }
        public override void SetDefaults()
        {
            Item.damage = 40; 
            Item.width = 32;
			Item.height = 32;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item44;

            Item.DamageType = DamageClass.Summon;
            Item.noMelee = true;
            Item.buffType = ModContent.BuffType<DragonBuff>();
            Item.shoot = ModContent.ProjectileType<DragonHead>();
        }

        // 自己求和：player.slotsMinions 在 Player.Update 里、ItemCheck 之前每帧被清零，
        // 读它做上限判断永远是 0，等于没写。这里直接扫弹幕数组。
        private static float SlotsInUse(Player player)
        {
            float used = 0f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == player.whoAmI && p.minion)
                    used += p.minionSlots;
            }
            return used;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // 先续 buff：任何 return 之前，避免上限时 buff 断档
            player.AddBuff(Item.buffType, 3600);

            int headType = ModContent.ProjectileType<DragonHead>();
            int bodyType = ModContent.ProjectileType<DragonBody>();
            int tailType = ModContent.ProjectileType<DragonTail>();

            // 没有头：生成整条龙（head 的 OnSpawn 负责生成 2 个 body 和 tail）
            if (player.ownedProjectileCounts[headType] <= 0)
            {
                // if (SlotsInUse(player) + 1f > player.maxMinions + 0.001f)
                //     return false;
                Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
                return false;
            }

            // 有头：在 tail 之前插 2 个 body
            Projectile tail = null;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == player.whoAmI && p.type == tailType)
                {
                    tail = p;
                    break;
                }
            }

            if (tail == null) return false;      // 有头没有尾，出错了

            // 上限：使用物品之前 FreeUpPetsAndMinions 已经腾出 1 格，这里用腾完之后
            // 真实的和来判断，才能和它的加减保持平衡，不会越召唤越短
            // if (SlotsInUse(player) + 1f > player.maxMinions + 0.001f)
            //     return false;
            int body1 = Projectile.NewProjectile(source, tail.Center, Vector2.Zero,
                bodyType, 0, 0f, player.whoAmI, ai0: tail.ai[0], ai1: 0);

            int body2 = Projectile.NewProjectile(source, tail.Center, Vector2.Zero,
                bodyType, 0, 0f, player.whoAmI, ai0: Main.projectile[body1].identity, ai1: 1);

            tail.ai[0] = Main.projectile[body2].identity;
            tail.netUpdate = true;      // ← 必须
            tail.localAI[0] = -1f;      // ← 缓存失效
            return false;
        }
    }
}