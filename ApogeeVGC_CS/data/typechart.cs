﻿using ApogeeVGC_CS.sim;

namespace ApogeeVGC_CS.data
{
    public static class TypeChart
    {
        public static TypeDataTable TypeData { get; } = new()
        {
            [new IdEntry("bug")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["Bug"] = 0,
                    ["Dark"] = 0,
                    ["Dragon"] = 0,
                    ["Electric"] = 0,
                    ["Fairy"] = 0,
                    ["Fighting"] = 2,
                    ["Fire"] = 1,
                    ["Flying"] = 1,
                    ["Ghost"] = 0,
                    ["Grass"] = 2,
                    ["Ground"] = 2,
                    ["Ice"] = 0,
                    ["Normal"] = 0,
                    ["Poison"] = 0,
                    ["Psychic"] = 0,
                    ["Rock"] = 1,
                    ["Steel"] = 0,
                    ["Stellar"] = 0,
                    ["Water"] = 0,
                },
                HpIvs = new SparseStatsTable
                {
                    [StatId.Atk] = 30,
                    [StatId.Def] = 30,
                    [StatId.Spd] = 30,
                },
                HpDvs = new SparseStatsTable
                {
                    [StatId.Atk] = 13,
                    [StatId.Def] = 13,
                },
            },
            [new IdEntry("dark")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["prankster"] = 3,
                    ["Bug"] = 1,
                    ["Dark"] = 2,
                    ["Dragon"] = 0,
                    ["Electric"] = 0,
                    ["Fairy"] = 1,
                    ["Fighting"] = 1,
                    ["Fire"] = 0,
                    ["Flying"] = 0,
                    ["Ghost"] = 2,
                    ["Grass"] = 0,
                    ["Ground"] = 0,
                    ["Ice"] = 0,
                    ["Normal"] = 0,
                    ["Poison"] = 0,
                    ["Psychic"] = 3,
                    ["Rock"] = 0,
                    ["Steel"] = 0,
                    ["Stellar"] = 0,
                    ["Water"] = 0,
                },
                HpIvs = new SparseStatsTable(),
            },
            [new IdEntry("dragon")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["Bug"] = 0,
                    ["Dark"] = 0,
                    ["Dragon"] = 1,
                    ["Electric"] = 2,
                    ["Fairy"] = 1,
                    ["Fighting"] = 0,
                    ["Fire"] = 2,
                    ["Flying"] = 0,
                    ["Ghost"] = 0,
                    ["Grass"] = 2,
                    ["Ground"] = 0,
                    ["Ice"] = 1,
                    ["Normal"] = 0,
                    ["Poison"] = 0,
                    ["Psychic"] = 0,
                    ["Rock"] = 0,
                    ["Steel"] = 0,
                    ["Stellar"] = 0,
                    ["Water"] = 2,
                },
                HpIvs = new SparseStatsTable
                {
                    [StatId.Atk] = 30
                },
                HpDvs = new SparseStatsTable
                {
                    [StatId.Def] = 14
                },
            },
            [new IdEntry("electric")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["par"] = 3,
                    ["Bug"] = 0,
                    ["Dark"] = 0,
                    ["Dragon"] = 0,
                    ["Electric"] = 2,
                    ["Fairy"] = 0,
                    ["Fighting"] = 0,
                    ["Fire"] = 0,
                    ["Flying"] = 2,
                    ["Ghost"] = 0,
                    ["Grass"] = 0,
                    ["Ground"] = 1,
                    ["Ice"] = 0,
                    ["Normal"] = 0,
                    ["Poison"] = 0,
                    ["Psychic"] = 0,
                    ["Rock"] = 0,
                    ["Steel"] = 2,
                    ["Stellar"] = 0,
                    ["Water"] = 0,
                },
                HpIvs = new SparseStatsTable
                {
                    [StatId.Spa] = 30
                },
                HpDvs = new SparseStatsTable
                {
                    [StatId.Atk] = 14
                },
            },
            [new IdEntry("fairy")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["Bug"] = 2,
                    ["Dark"] = 2,
                    ["Dragon"] = 3,
                    ["Electric"] = 0,
                    ["Fairy"] = 0,
                    ["Fighting"] = 2,
                    ["Fire"] = 0,
                    ["Flying"] = 0,
                    ["Ghost"] = 0,
                    ["Grass"] = 0,
                    ["Ground"] = 0,
                    ["Ice"] = 0,
                    ["Normal"] = 0,
                    ["Poison"] = 1,
                    ["Psychic"] = 0,
                    ["Rock"] = 0,
                    ["Steel"] = 1,
                    ["Stellar"] = 0,
                    ["Water"] = 0,
                },
            },
            [new IdEntry("fighting")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["Bug"] = 2,
                    ["Dark"] = 2,
                    ["Dragon"] = 0,
                    ["Electric"] = 0,
                    ["Fairy"] = 1,
                    ["Fighting"] = 0,
                    ["Fire"] = 0,
                    ["Flying"] = 1,
                    ["Ghost"] = 0,
                    ["Grass"] = 0,
                    ["Ground"] = 0,
                    ["Ice"] = 0,
                    ["Normal"] = 0,
                    ["Poison"] = 0,
                    ["Psychic"] = 1,
                    ["Rock"] = 2,
                    ["Steel"] = 0,
                    ["Stellar"] = 0,
                    ["Water"] = 0,
                },
                HpIvs = new SparseStatsTable
                {
                    [StatId.Def] = 30,
                    [StatId.Spa] = 30,
                    [StatId.Spd] = 30,
                    [StatId.Spe] = 30
                },
                HpDvs = new SparseStatsTable
                {
                    [StatId.Atk] = 12,
                    [StatId.Def] = 12
                },
            },
            [new IdEntry("fire")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["brn"] = 3,
                    ["Bug"] = 2,
                    ["Dark"] = 0,
                    ["Dragon"] = 0,
                    ["Electric"] = 0,
                    ["Fairy"] = 2,
                    ["Fighting"] = 0,
                    ["Fire"] = 2,
                    ["Flying"] = 0,
                    ["Ghost"] = 0,
                    ["Grass"] = 2,
                    ["Ground"] = 1,
                    ["Ice"] = 2,
                    ["Normal"] = 0,
                    ["Poison"] = 0,
                    ["Psychic"] = 0,
                    ["Rock"] = 1,
                    ["Steel"] = 2,
                    ["Stellar"] = 0,
                    ["Water"] = 1,
                },
                HpIvs = new SparseStatsTable
                {
                    [StatId.Atk] = 30,
                    [StatId.Spa] = 30,
                    [StatId.Spe] = 30
                },
                HpDvs = new SparseStatsTable
                {
                    [StatId.Atk] = 14,
                    [StatId.Def] = 12
                },
            },
            [new IdEntry("flying")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["Bug"] = 2,
                    ["Dark"] = 0,
                    ["Dragon"] = 0,
                    ["Electric"] = 1,
                    ["Fairy"] = 0,
                    ["Fighting"] = 2,
                    ["Fire"] = 0,
                    ["Flying"] = 0,
                    ["Ghost"] = 0,
                    ["Grass"] = 2,
                    ["Ground"] = 3,
                    ["Ice"] = 1,
                    ["Normal"] = 0,
                    ["Poison"] = 0,
                    ["Psychic"] = 0,
                    ["Rock"] = 1,
                    ["Steel"] = 0,
                    ["Stellar"] = 0,
                    ["Water"] = 0,
                },
                HpIvs = new SparseStatsTable
                {
                    [StatId.Hp] = 30,
                    [StatId.Atk] = 30,
                    [StatId.Def] = 30,
                    [StatId.Spa] = 30,
                    [StatId.Spd] = 30
                },
                HpDvs = new SparseStatsTable
                {
                    [StatId.Atk] = 12,
                    [StatId.Def] = 13
                },
            },
            [new IdEntry("ghost")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["trapped"] = 3,
                    ["Bug"] = 2,
                    ["Dark"] = 1,
                    ["Dragon"] = 0,
                    ["Electric"] = 0,
                    ["Fairy"] = 0,
                    ["Fighting"] = 3,
                    ["Fire"] = 0,
                    ["Flying"] = 0,
                    ["Ghost"] = 1,
                    ["Grass"] = 0,
                    ["Ground"] = 0,
                    ["Ice"] = 0,
                    ["Normal"] = 3,
                    ["Poison"] = 2,
                    ["Psychic"] = 0,
                    ["Rock"] = 0,
                    ["Steel"] = 0,
                    ["Stellar"] = 0,
                    ["Water"] = 0,
                },
                HpIvs = new SparseStatsTable
                {
                    [StatId.Def] = 30,
                    [StatId.Spd] = 30
                },
                HpDvs = new SparseStatsTable
                {
                    [StatId.Atk] = 13,
                    [StatId.Def] = 14
                },
            },
            [new IdEntry("grass")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["powder"] = 3,
                    ["Bug"] = 1,
                    ["Dark"] = 0,
                    ["Dragon"] = 0,
                    ["Electric"] = 2,
                    ["Fairy"] = 0,
                    ["Fighting"] = 0,
                    ["Fire"] = 1,
                    ["Flying"] = 1,
                    ["Ghost"] = 0,
                    ["Grass"] = 2,
                    ["Ground"] = 2,
                    ["Ice"] = 1,
                    ["Normal"] = 0,
                    ["Poison"] = 1,
                    ["Psychic"] = 0,
                    ["Rock"] = 0,
                    ["Steel"] = 0,
                    ["Stellar"] = 0,
                    ["Water"] = 2,
                },
                HpIvs = new SparseStatsTable
                {
                    [StatId.Atk] = 30,
                    [StatId.Spa] = 30
                },
                HpDvs = new SparseStatsTable
                {
                    [StatId.Atk] = 14,
                    [StatId.Def] = 14
                },
            },
            [new IdEntry("ground")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["sandstorm"] = 3,
                    ["Bug"] = 0,
                    ["Dark"] = 0,
                    ["Dragon"] = 0,
                    ["Electric"] = 3,
                    ["Fairy"] = 0,
                    ["Fighting"] = 0,
                    ["Fire"] = 0,
                    ["Flying"] = 0,
                    ["Ghost"] = 0,
                    ["Grass"] = 1,
                    ["Ground"] = 0,
                    ["Ice"] = 1,
                    ["Normal"] = 0,
                    ["Poison"] = 2,
                    ["Psychic"] = 0,
                    ["Rock"] = 2,
                    ["Steel"] = 0,
                    ["Stellar"] = 0,
                    ["Water"] = 1,
                },
                HpIvs = new SparseStatsTable
                {
                    [StatId.Spa] = 30,
                    [StatId.Spd] = 30
                },
                HpDvs = new SparseStatsTable
                {
                    [StatId.Atk] = 12
                },
            },
            [new IdEntry("ice")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["hail"] = 3,
                    ["frz"] = 3,
                    ["Bug"] = 0,
                    ["Dark"] = 0,
                    ["Dragon"] = 0,
                    ["Electric"] = 0,
                    ["Fairy"] = 0,
                    ["Fighting"] = 1,
                    ["Fire"] = 1,
                    ["Flying"] = 0,
                    ["Ghost"] = 0,
                    ["Grass"] = 0,
                    ["Ground"] = 0,
                    ["Ice"] = 2,
                    ["Normal"] = 0,
                    ["Poison"] = 0,
                    ["Psychic"] = 0,
                    ["Rock"] = 1,
                    ["Steel"] = 1,
                    ["Stellar"] = 0,
                    ["Water"] = 0,
                },
                HpIvs = new SparseStatsTable
                {
                    [StatId.Atk] = 30,
                    [StatId.Def] = 30
                },
                HpDvs = new SparseStatsTable
                {
                    [StatId.Def] = 13
                },
            },
            [new IdEntry("normal")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["Bug"] = 0,
                    ["Dark"] = 0,
                    ["Dragon"] = 0,
                    ["Electric"] = 0,
                    ["Fairy"] = 0,
                    ["Fighting"] = 1,
                    ["Fire"] = 0,
                    ["Flying"] = 0,
                    ["Ghost"] = 3,
                    ["Grass"] = 0,
                    ["Ground"] = 0,
                    ["Ice"] = 0,
                    ["Normal"] = 0,
                    ["Poison"] = 0,
                    ["Psychic"] = 0,
                    ["Rock"] = 0,
                    ["Steel"] = 0,
                    ["Stellar"] = 0,
                    ["Water"] = 0,
                },
            },
            [new IdEntry("poison")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["psn"] = 3,
                    ["tox"] = 3,
                    ["Bug"] = 2,
                    ["Dark"] = 0,
                    ["Dragon"] = 0,
                    ["Electric"] = 0,
                    ["Fairy"] = 2,
                    ["Fighting"] = 2,
                    ["Fire"] = 0,
                    ["Flying"] = 0,
                    ["Ghost"] = 0,
                    ["Grass"] = 2,
                    ["Ground"] = 1,
                    ["Ice"] = 0,
                    ["Normal"] = 0,
                    ["Poison"] = 2,
                    ["Psychic"] = 1,
                    ["Rock"] = 0,
                    ["Steel"] = 0,
                    ["Stellar"] = 0,
                    ["Water"] = 0,
                },
                HpIvs = new SparseStatsTable
                {
                    [StatId.Def] = 30,
                    [StatId.Spa] = 30,
                    [StatId.Spd] = 30
                },
                HpDvs = new SparseStatsTable
                {
                    [StatId.Atk] = 12,
                    [StatId.Def] = 14
                },
            },
            [new IdEntry("psychic")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["Bug"] = 1,
                    ["Dark"] = 1,
                    ["Dragon"] = 0,
                    ["Electric"] = 0,
                    ["Fairy"] = 0,
                    ["Fighting"] = 2,
                    ["Fire"] = 0,
                    ["Flying"] = 0,
                    ["Ghost"] = 1,
                    ["Grass"] = 0,
                    ["Ground"] = 0,
                    ["Ice"] = 0,
                    ["Normal"] = 0,
                    ["Poison"] = 0,
                    ["Psychic"] = 2,
                    ["Rock"] = 0,
                    ["Steel"] = 0,
                    ["Stellar"] = 0,
                    ["Water"] = 0,
                },
                HpIvs = new SparseStatsTable
                {
                    [StatId.Atk] = 30,
                    [StatId.Spe] = 30
                },
                HpDvs = new SparseStatsTable
                {
                    [StatId.Def] = 12
                },
            },
            [new IdEntry("rock")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["sandstorm"] = 3,
                    ["Bug"] = 0,
                    ["Dark"] = 0,
                    ["Dragon"] = 0,
                    ["Electric"] = 0,
                    ["Fairy"] = 0,
                    ["Fighting"] = 1,
                    ["Fire"] = 2,
                    ["Flying"] = 2,
                    ["Ghost"] = 0,
                    ["Grass"] = 1,
                    ["Ground"] = 1,
                    ["Ice"] = 0,
                    ["Normal"] = 2,
                    ["Poison"] = 2,
                    ["Psychic"] = 0,
                    ["Rock"] = 0,
                    ["Steel"] = 1,
                    ["Stellar"] = 0,
                    ["Water"] = 1,
                },
                HpIvs = new SparseStatsTable
                {
                    [StatId.Def] = 30,
                    [StatId.Spd] = 30,
                    [StatId.Spe] = 30
                },
                HpDvs = new SparseStatsTable
                {
                    [StatId.Atk] = 13,
                    [StatId.Def] = 12
                },
            },
            [new IdEntry("steel")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["psn"] = 3,
                    ["tox"] = 3,
                    ["sandstorm"] = 3,
                    ["Bug"] = 2,
                    ["Dark"] = 0,
                    ["Dragon"] = 2,
                    ["Electric"] = 0,
                    ["Fairy"] = 2,
                    ["Fighting"] = 1,
                    ["Fire"] = 1,
                    ["Flying"] = 2,
                    ["Ghost"] = 0,
                    ["Grass"] = 2,
                    ["Ground"] = 1,
                    ["Ice"] = 2,
                    ["Normal"] = 2,
                    ["Poison"] = 3,
                    ["Psychic"] = 2,
                    ["Rock"] = 2,
                    ["Steel"] = 2,
                    ["Stellar"] = 0,
                    ["Water"] = 0,
                },
                HpIvs = new SparseStatsTable
                {
                    [StatId.Spd] = 30
                },
                HpDvs = new SparseStatsTable
                {
                    [StatId.Atk] = 13
                },
            },
            [new IdEntry("stellar")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["Bug"] = 0,
                    ["Dark"] = 0,
                    ["Dragon"] = 0,
                    ["Electric"] = 0,
                    ["Fairy"] = 0,
                    ["Fighting"] = 0,
                    ["Fire"] = 0,
                    ["Flying"] = 0,
                    ["Ghost"] = 0,
                    ["Grass"] = 0,
                    ["Ground"] = 0,
                    ["Ice"] = 0,
                    ["Normal"] = 0,
                    ["Poison"] = 0,
                    ["Psychic"] = 0,
                    ["Rock"] = 0,
                    ["Steel"] = 0,
                    ["Stellar"] = 0,
                    ["Water"] = 0,
                },
            },
            [new IdEntry("water")] = new TypeData
            {
                DamageTaken = new Dictionary<string, int>
                {
                    ["Bug"] = 0,
                    ["Dark"] = 0,
                    ["Dragon"] = 0,
                    ["Electric"] = 1,
                    ["Fairy"] = 0,
                    ["Fighting"] = 0,
                    ["Fire"] = 2,
                    ["Flying"] = 0,
                    ["Ghost"] = 0,
                    ["Grass"] = 1,
                    ["Ground"] = 0,
                    ["Ice"] = 2,
                    ["Normal"] = 0,
                    ["Poison"] = 0,
                    ["Psychic"] = 0,
                    ["Rock"] = 0,
                    ["Steel"] = 2,
                    ["Stellar"] = 0,
                    ["Water"] = 2,
                },
                HpIvs = new SparseStatsTable
                {
                    [StatId.Atk] = 30,
                    [StatId.Def] = 30,
                    [StatId.Spa] = 30
                },
                HpDvs = new SparseStatsTable
                {
                    [StatId.Atk] = 14,
                    [StatId.Def] = 13
                },
            },
        };
    }
}
