using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace PesGrudadosLore.Content.Items.Weapons
{
    public class ExampleShootingSword : ModItem
    {
        public enum Mode
        {
            Dig,
            Place,
            Melee,
            Magic
        }

        private Mode currentMode = Mode.Dig;

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.scale = 4;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 50;
            Item.knockBack = 15;
            Item.crit = 10;

            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.Purple;

            Item.UseSound = SoundID.Item15;

            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 12f;
        }

        public override bool AltFunctionUse(Player player)
        {
            // Toggle between modes
            currentMode = (Mode)(((int)currentMode + 1) % 4);
            NotifyModeSwitch();
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            switch (currentMode)
            {
                case Mode.Dig:
                    ExecuteDigMode(player);
                    break;
                case Mode.Place:
                    ExecutePlaceMode(player);
                    break;
                case Mode.Melee:
                    ExecuteMeleeMode(player);
                    break;
                case Mode.Magic:
                    ExecuteMagicMode(player, source, position, damage, knockback);
                    break;
            }
            return false;
        }

        #region Mode Handlers

        private void ExecuteDigMode(Player player)
        {
            Vector2 mouseWorldPosition = Main.MouseWorld;
            int targetX = (int)(mouseWorldPosition.X / 16f);
            int targetY = (int)(mouseWorldPosition.Y / 16f);

            int radius = Main.rand.Next(3, 7); // Random dig radius
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    if (Vector2.Distance(new Vector2(i, j), Vector2.Zero) <= radius)
                    {
                        WorldGen.KillTile(targetX + i, targetY + j, fail: false, effectOnly: false, noItem: false);
                        SpawnDigParticles(targetX + i, targetY + j);
                    }
                }
            }

            SoundEngine.PlaySound(SoundID.Item14, player.position); // Explosion sound
        }

        private void ExecutePlaceMode(Player player)
        {
            Vector2 mouseWorldPosition = Main.MouseWorld;
            int targetX = (int)(mouseWorldPosition.X / 16f);
            int targetY = (int)(mouseWorldPosition.Y / 16f);

            int[] randomBlocks = { ItemID.DirtBlock, ItemID.Wood, ItemID.StoneBlock, ItemID.MudBlock, ItemID.SandBlock };

            int radius = Main.rand.Next(3, 7); // Random dig radius
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    int blockToPlace = randomBlocks[Main.rand.Next(randomBlocks.Length)];
                    WorldGen.PlaceTile(targetX + i, targetY + j, blockToPlace);
                    SpawnPlaceParticles(targetX + i, targetY + j);
                }
            }

            SoundEngine.PlaySound(SoundID.Item1, player.position); // Placement sound
        }

        private void ExecuteMeleeMode(Player player)
        {
            Item.scale = 10;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.damage = 100;

            SoundEngine.PlaySound(SoundID.Item1, player.position); // Melee swing sound
        }

        private void ExecuteMagicMode(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, int damage, float knockback)
        {
            int numberOfProjectiles = Main.rand.Next(5, 20); // Random number of projectiles
            int[] randomProjectiles = { ProjectileID.BulletHighVelocity, ProjectileID.BabyDino, ProjectileID.GoldenShowerFriendly, ProjectileID.QueenSlimeMinionBlueSpike,
            ProjectileID.Parrot };

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                Vector2 randomVelocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                randomVelocity.Normalize();
                randomVelocity *= Main.rand.NextFloat(6f, 16f);

                int randomProjectile = randomProjectiles[Main.rand.Next(randomProjectiles.Length)];
                Projectile.NewProjectile(source, player.Center, randomVelocity, randomProjectile, damage, knockback, player.whoAmI);
            }

            SoundEngine.PlaySound(SoundID.Item20, player.position); // Magic sound
        }

        #endregion

        #region Helpers

        private void NotifyModeSwitch()
        {
            string modeMessage = currentMode switch
            {
                Mode.Dig => "Dig Mode",
                Mode.Place => "Place Mode",
                Mode.Melee => "Melee Mode",
                Mode.Magic => "Magic Mode",
                _ => "Unknown Mode"
            };
            Main.NewText($"Switched to {modeMessage}", Color.Yellow);
        }

        private void SpawnDigParticles(int x, int y)
        {
            // Spawns blue glowing particles for Dig mode
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(new Vector2(x * 16, y * 16), DustID.BlueTorch, new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)), 150, Color.Blue, 1.5f);
            }
        }

        private void SpawnPlaceParticles(int x, int y)
        {
            // Spawns green glowing particles for Place mode
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(new Vector2(x * 16, y * 16), DustID.RedTorch, new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)), 150, Color.Green, 1.5f);
            }
        }

        #endregion

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DirtBlock, 10)
                .Register();
        }
    }
}
