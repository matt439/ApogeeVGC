using ApogeeVGC_CS.sim;

namespace ApogeeVGC_CS.data
{
    public static class Items
    {
        public static ItemDataTable ItemData { get; } = new()
        {
            [new IdEntry("leftovers")] = new ItemData
            {
                // Required by Item
                Fullname = string.Empty,
                EffectType = EffectType.Item,
                IsBerry = false,
                IgnoreKlutz = false,
                IsGem = false,
                IsPokeball = false,
                IsPrimalOrb = false,
                // Required by BasicEffect
                Exists = true,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
                // Existing properties
                Name = "Leftovers",
                SpriteNum = 242,
                Fling = new FlingData { BasePower = 10, },
                OnResidualOrder = 5,
                OnResidualSubOrder = 4,
                OnResidual = (battle, target, source, effect) =>
                {
                    battle.Heal(target.BaseMaxHp / 16);
                },
                Num = 234,
                Gen = 2,
            },
            [new IdEntry("choicespecs")] = new ItemData
            {
                Name = "Choice Specs",
                SpriteNum = 70,
                Fling = new FlingData { BasePower = 10 },
                OnStart = (battle, pokemon, sourcePokemon, effect) =>
                {
                    if (pokemon.Volatiles.ContainsKey("choicelock"))
                    {
                        battle.Debug("removing choicelock");
                    }
                    pokemon.RemoveVolatile("choicelock");
                    return true;
                },
                OnModifyMove = (battle, move, pokemon, target) =>
                {
                    pokemon.AddVolatile("choicelock");
                },
                OnModifySpAPriority = 1,
                OnModifySpA = (battle, spa, pokemon, target, move) =>
                {
                    if (pokemon.Volatiles.ContainsKey("dynamax")) return spa;
                    return (int)(spa * 1.5);
                },
                IsChoice = true,
                Num = 297,
                Gen = 4,
                Fullname = string.Empty,
                EffectType = EffectType.Item,
                Exists = true,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
                // Required by Item
                IsBerry = false,
                IgnoreKlutz = false,
                IsGem = false,
                IsPokeball = false,
                IsPrimalOrb = false,
            },
            [new IdEntry("flameorb")] = new ItemData
            {
                Name = "Flame Orb",
                SpriteNum = 145,
                Fling = new FlingData { BasePower = 30, Status = "brn" },
                OnResidualOrder = 28,
                OnResidualSubOrder = 3,
                OnResidual = (battle, target, source, effect) =>
                {
                    target.TrySetStatus("brn", target);
                },
                Num = 273,
                Gen = 4,
                Fullname = string.Empty,
                EffectType = EffectType.Item,
                Exists = true,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
                IsBerry = false,
                IgnoreKlutz = false,
                IsGem = false,
                IsPokeball = false,
                IsPrimalOrb = false,
            },
            [new IdEntry("rockyhelmet")] = new ItemData
            {
                Name = "Rocky Helmet",
                SpriteNum = 417,
                Fling = new FlingData { BasePower = 60 },
                OnDamagingHitOrder = 2,
                OnDamagingHit = (battle, damage, target, source, move) =>
                {
                    if (battle.CheckMoveMakesContact(move, source, target))
                    {
                        battle.Damage(source.BaseMaxHp / 6, source, target);
                    }
                },
                Num = 540,
                Gen = 5,
                Fullname = string.Empty,
                EffectType = EffectType.Item,
                Exists = true,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
                IsBerry = false,
                IgnoreKlutz = false,
                IsGem = false,
                IsPokeball = false,
                IsPrimalOrb = false,
            },
            [new IdEntry("lightclay")] = new ItemData
            {
                Name = "Light Clay",
                SpriteNum = 252,
                Fling = new FlingData { BasePower = 30 },
                Num = 269,
                Gen = 4,
                Fullname = string.Empty,
                EffectType = EffectType.Item,
                Exists = true,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
                IsBerry = false,
                IgnoreKlutz = false,
                IsGem = false,
                IsPokeball = false,
                IsPrimalOrb = false,
            },
            [new IdEntry("assaultvest")] = new ItemData
            {
                Name = "Assault Vest",
                SpriteNum = 581,
                Fling = new FlingData { BasePower = 80 },
                OnModifySpDPriority = 1,
                OnModifySpD = (battle, spd, pokemon, target, move) =>
                {
                    battle.ChainModify(3, 2);
                    return null;
                },
                OnDisableMove = (battle, pokemon) =>
                {
                    foreach (MoveSlot? moveSlot in pokemon.MoveSlots)
                    {
                        Move? move = battle.Dex.Moves.Get(moveSlot.Id);
                        if (move.Category == MoveCategory.Status && move.Id != new Id("mefirst"))
                        {
                            pokemon.DisableMove(moveSlot.Id, false);
                        }
                    }
                },
                Num = 640,
                Gen = 6,
                Fullname = string.Empty,
                EffectType = EffectType.Item,
                Exists = true,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
                IsBerry = false,
                IgnoreKlutz = false,
                IsGem = false,
                IsPokeball = false,
                IsPrimalOrb = false,
            },
        };
    }
}