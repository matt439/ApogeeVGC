using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Items;

public partial record Items
{
    /// <summary>
    /// Shared OnTakeItem handler for mega stones.
    /// Prevents removal from a Pokemon whose base species matches the mega stone's mapping.
    /// </summary>
    private static OnTakeItemEventInfo MegaStoneOnTakeItem() =>
        OnTakeItemEventInfo.Create((_, item, pokemon, _, _) =>
            item.MegaStone?.ContainsKey(pokemon.BaseSpecies.Id) == true
                ? BoolRelayVar.False
                : BoolRelayVar.True);

    private partial Dictionary<ItemId, Item> CreateItemsMegaStones()
    {
        var onTakeItem = MegaStoneOnTakeItem();

        return new Dictionary<ItemId, Item>
        {
            // ── Official Mega Stones (Gen 6-7) ──────────────────────────────────

            [ItemId.Abomasite] = new()
            {
                Id = ItemId.Abomasite,
                Name = "Abomasite",
                SpriteNum = 575,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Abomasnow] = SpecieId.AbomasnowMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 674,
                Gen = 6,
            },
            [ItemId.Absolite] = new()
            {
                Id = ItemId.Absolite,
                Name = "Absolite",
                SpriteNum = 576,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Absol] = SpecieId.AbsolMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 677,
                Gen = 6,
            },
            [ItemId.Aerodactylite] = new()
            {
                Id = ItemId.Aerodactylite,
                Name = "Aerodactylite",
                SpriteNum = 577,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Aerodactyl] = SpecieId.AerodactylMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 672,
                Gen = 6,
            },
            [ItemId.Aggronite] = new()
            {
                Id = ItemId.Aggronite,
                Name = "Aggronite",
                SpriteNum = 578,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Aggron] = SpecieId.AggronMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 667,
                Gen = 6,
            },
            [ItemId.Alakazite] = new()
            {
                Id = ItemId.Alakazite,
                Name = "Alakazite",
                SpriteNum = 579,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Alakazam] = SpecieId.AlakazamMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 679,
                Gen = 6,
            },
            [ItemId.Altarianite] = new()
            {
                Id = ItemId.Altarianite,
                Name = "Altarianite",
                SpriteNum = 615,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Altaria] = SpecieId.AltariaMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 755,
                Gen = 6,
            },
            [ItemId.Ampharosite] = new()
            {
                Id = ItemId.Ampharosite,
                Name = "Ampharosite",
                SpriteNum = 580,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Ampharos] = SpecieId.AmpharosMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 658,
                Gen = 6,
            },
            [ItemId.Audinite] = new()
            {
                Id = ItemId.Audinite,
                Name = "Audinite",
                SpriteNum = 617,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Audino] = SpecieId.AudinoMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 757,
                Gen = 6,
            },
            [ItemId.Banettite] = new()
            {
                Id = ItemId.Banettite,
                Name = "Banettite",
                SpriteNum = 582,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Banette] = SpecieId.BanetteMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 668,
                Gen = 6,
            },
            [ItemId.Beedrillite] = new()
            {
                Id = ItemId.Beedrillite,
                Name = "Beedrillite",
                SpriteNum = 628,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Beedrill] = SpecieId.BeedrillMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 770,
                Gen = 6,
            },
            [ItemId.Blastoisinite] = new()
            {
                Id = ItemId.Blastoisinite,
                Name = "Blastoisinite",
                SpriteNum = 583,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Blastoise] = SpecieId.BlastoiseMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 661,
                Gen = 6,
            },
            [ItemId.Blazikenite] = new()
            {
                Id = ItemId.Blazikenite,
                Name = "Blazikenite",
                SpriteNum = 584,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Blaziken] = SpecieId.BlazikenMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 664,
                Gen = 6,
            },
            [ItemId.Cameruptite] = new()
            {
                Id = ItemId.Cameruptite,
                Name = "Cameruptite",
                SpriteNum = 625,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Camerupt] = SpecieId.CameruptMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 767,
                Gen = 6,
            },
            [ItemId.CharizarditeX] = new()
            {
                Id = ItemId.CharizarditeX,
                Name = "Charizardite X",
                SpriteNum = 585,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Charizard] = SpecieId.CharizardMegaX,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 660,
                Gen = 6,
            },
            [ItemId.CharizarditeY] = new()
            {
                Id = ItemId.CharizarditeY,
                Name = "Charizardite Y",
                SpriteNum = 586,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Charizard] = SpecieId.CharizardMegaY,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 678,
                Gen = 6,
            },
            [ItemId.Diancite] = new()
            {
                Id = ItemId.Diancite,
                Name = "Diancite",
                SpriteNum = 624,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Diancie] = SpecieId.DiancieMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 764,
                Gen = 6,
            },
            [ItemId.Galladite] = new()
            {
                Id = ItemId.Galladite,
                Name = "Galladite",
                SpriteNum = 616,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Gallade] = SpecieId.GalladeMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 756,
                Gen = 6,
            },
            [ItemId.Garchompite] = new()
            {
                Id = ItemId.Garchompite,
                Name = "Garchompite",
                SpriteNum = 589,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Garchomp] = SpecieId.GarchompMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 683,
                Gen = 6,
            },
            [ItemId.Gardevoirite] = new()
            {
                Id = ItemId.Gardevoirite,
                Name = "Gardevoirite",
                SpriteNum = 587,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Gardevoir] = SpecieId.GardevoirMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 657,
                Gen = 6,
            },
            [ItemId.Gengarite] = new()
            {
                Id = ItemId.Gengarite,
                Name = "Gengarite",
                SpriteNum = 588,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Gengar] = SpecieId.GengarMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 656,
                Gen = 6,
            },
            [ItemId.Glalitite] = new()
            {
                Id = ItemId.Glalitite,
                Name = "Glalitite",
                SpriteNum = 623,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Glalie] = SpecieId.GlalieMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 763,
                Gen = 6,
            },
            [ItemId.Gyaradosite] = new()
            {
                Id = ItemId.Gyaradosite,
                Name = "Gyaradosite",
                SpriteNum = 589,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Gyarados] = SpecieId.GyaradosMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 676,
                Gen = 6,
            },
            [ItemId.Heracronite] = new()
            {
                Id = ItemId.Heracronite,
                Name = "Heracronite",
                SpriteNum = 590,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Heracross] = SpecieId.HeracrossMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 680,
                Gen = 6,
            },
            [ItemId.Houndoominite] = new()
            {
                Id = ItemId.Houndoominite,
                Name = "Houndoominite",
                SpriteNum = 591,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Houndoom] = SpecieId.HoundoomMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 666,
                Gen = 6,
            },
            [ItemId.Kangaskhanite] = new()
            {
                Id = ItemId.Kangaskhanite,
                Name = "Kangaskhanite",
                SpriteNum = 592,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Kangaskhan] = SpecieId.KangaskhanMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 675,
                Gen = 6,
            },
            [ItemId.Latiasite] = new()
            {
                Id = ItemId.Latiasite,
                Name = "Latiasite",
                SpriteNum = 593,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Latias] = SpecieId.LatiasMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 684,
                Gen = 6,
            },
            [ItemId.Latiosite] = new()
            {
                Id = ItemId.Latiosite,
                Name = "Latiosite",
                SpriteNum = 594,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Latios] = SpecieId.LatiosMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 685,
                Gen = 6,
            },
            [ItemId.Lopunnite] = new()
            {
                Id = ItemId.Lopunnite,
                Name = "Lopunnite",
                SpriteNum = 626,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Lopunny] = SpecieId.LopunnyMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 768,
                Gen = 6,
            },
            [ItemId.Lucarionite] = new()
            {
                Id = ItemId.Lucarionite,
                Name = "Lucarionite",
                SpriteNum = 595,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Lucario] = SpecieId.LucarioMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 673,
                Gen = 6,
            },
            [ItemId.Manectite] = new()
            {
                Id = ItemId.Manectite,
                Name = "Manectite",
                SpriteNum = 596,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Manectric] = SpecieId.ManectricMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 682,
                Gen = 6,
            },
            [ItemId.Mawilite] = new()
            {
                Id = ItemId.Mawilite,
                Name = "Mawilite",
                SpriteNum = 598,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Mawile] = SpecieId.MawileMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 681,
                Gen = 6,
            },
            [ItemId.Medichamite] = new()
            {
                Id = ItemId.Medichamite,
                Name = "Medichamite",
                SpriteNum = 599,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Medicham] = SpecieId.MedichamMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 665,
                Gen = 6,
            },
            [ItemId.Metagrossite] = new()
            {
                Id = ItemId.Metagrossite,
                Name = "Metagrossite",
                SpriteNum = 618,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Metagross] = SpecieId.MetagrossMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 758,
                Gen = 6,
            },
            [ItemId.MewtwoniteX] = new()
            {
                Id = ItemId.MewtwoniteX,
                Name = "Mewtwonite X",
                SpriteNum = 600,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Mewtwo] = SpecieId.MewtwoMegaX,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 662,
                Gen = 6,
            },
            [ItemId.MewtwoniteY] = new()
            {
                Id = ItemId.MewtwoniteY,
                Name = "Mewtwonite Y",
                SpriteNum = 601,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Mewtwo] = SpecieId.MewtwoMegaY,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 663,
                Gen = 6,
            },
            [ItemId.Pidgeotite] = new()
            {
                Id = ItemId.Pidgeotite,
                Name = "Pidgeotite",
                SpriteNum = 622,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Pidgeot] = SpecieId.PidgeotMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 762,
                Gen = 6,
            },
            [ItemId.Pinsirite] = new()
            {
                Id = ItemId.Pinsirite,
                Name = "Pinsirite",
                SpriteNum = 602,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Pinsir] = SpecieId.PinsirMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 671,
                Gen = 6,
            },
            [ItemId.Sablenite] = new()
            {
                Id = ItemId.Sablenite,
                Name = "Sablenite",
                SpriteNum = 614,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Sableye] = SpecieId.SableyeMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 754,
                Gen = 6,
            },
            [ItemId.Salamencite] = new()
            {
                Id = ItemId.Salamencite,
                Name = "Salamencite",
                SpriteNum = 627,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Salamence] = SpecieId.SalamenceMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 769,
                Gen = 6,
            },
            [ItemId.Sceptilite] = new()
            {
                Id = ItemId.Sceptilite,
                Name = "Sceptilite",
                SpriteNum = 613,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Sceptile] = SpecieId.SceptileMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 753,
                Gen = 6,
            },
            [ItemId.Scizorite] = new()
            {
                Id = ItemId.Scizorite,
                Name = "Scizorite",
                SpriteNum = 605,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Scizor] = SpecieId.ScizorMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 670,
                Gen = 6,
            },
            [ItemId.Sharpedonite] = new()
            {
                Id = ItemId.Sharpedonite,
                Name = "Sharpedonite",
                SpriteNum = 619,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Sharpedo] = SpecieId.SharpedomMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 759,
                Gen = 6,
            },
            [ItemId.Slowbronite] = new()
            {
                Id = ItemId.Slowbronite,
                Name = "Slowbronite",
                SpriteNum = 620,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Slowbro] = SpecieId.SlowbroMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 760,
                Gen = 6,
            },
            [ItemId.Steelixite] = new()
            {
                Id = ItemId.Steelixite,
                Name = "Steelixite",
                SpriteNum = 621,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Steelix] = SpecieId.SteelixMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 761,
                Gen = 6,
            },
            [ItemId.Swampertite] = new()
            {
                Id = ItemId.Swampertite,
                Name = "Swampertite",
                SpriteNum = 612,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Swampert] = SpecieId.SwampertMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 752,
                Gen = 6,
            },
            [ItemId.Tyranitarite] = new()
            {
                Id = ItemId.Tyranitarite,
                Name = "Tyranitarite",
                SpriteNum = 607,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Tyranitar] = SpecieId.TyranitarMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 669,
                Gen = 6,
            },
            [ItemId.Venusaurite] = new()
            {
                Id = ItemId.Venusaurite,
                Name = "Venusaurite",
                SpriteNum = 608,
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Venusaur] = SpecieId.VenusaurMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 659,
                Gen = 6,
            },

            // ── Custom Mega Stones (Homebrew) ───────────────────────────────────

            [ItemId.Barbaraclite] = new()
            {
                Id = ItemId.Barbaraclite,
                Name = "Barbaraclite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Barbaracle] = SpecieId.BarbaracleMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2001,
                Gen = 6,
            },
            [ItemId.Chandelurite] = new()
            {
                Id = ItemId.Chandelurite,
                Name = "Chandelurite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Chandelure] = SpecieId.ChandelureMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2002,
                Gen = 6,
            },
            [ItemId.Chesnaughtite] = new()
            {
                Id = ItemId.Chesnaughtite,
                Name = "Chesnaughtite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Chesnaught] = SpecieId.ChesnaughtMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2003,
                Gen = 6,
            },
            [ItemId.Clefablite] = new()
            {
                Id = ItemId.Clefablite,
                Name = "Clefablite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Clefable] = SpecieId.ClefableMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2004,
                Gen = 6,
            },
            [ItemId.Delphoxite] = new()
            {
                Id = ItemId.Delphoxite,
                Name = "Delphoxite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Delphox] = SpecieId.DelphoxMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2005,
                Gen = 6,
            },
            [ItemId.Dragalgite] = new()
            {
                Id = ItemId.Dragalgite,
                Name = "Dragalgite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Dragalge] = SpecieId.DragalgeMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2006,
                Gen = 6,
            },
            [ItemId.Dragonitite] = new()
            {
                Id = ItemId.Dragonitite,
                Name = "Dragonitite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Dragonite] = SpecieId.DragoniteMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2007,
                Gen = 6,
            },
            [ItemId.Drampite] = new()
            {
                Id = ItemId.Drampite,
                Name = "Drampite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Drampa] = SpecieId.DrampaMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2008,
                Gen = 6,
            },
            [ItemId.Eelektrossite] = new()
            {
                Id = ItemId.Eelektrossite,
                Name = "Eelektrossite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Eelektross] = SpecieId.EelektrossMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2009,
                Gen = 6,
            },
            [ItemId.Emboarite] = new()
            {
                Id = ItemId.Emboarite,
                Name = "Emboarite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Emboar] = SpecieId.EmboarMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2010,
                Gen = 6,
            },
            [ItemId.Excadrillite] = new()
            {
                Id = ItemId.Excadrillite,
                Name = "Excadrillite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Excadrill] = SpecieId.ExcadrillMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2011,
                Gen = 6,
            },
            [ItemId.Falinksite] = new()
            {
                Id = ItemId.Falinksite,
                Name = "Falinksite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Falinks] = SpecieId.FalinksMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2012,
                Gen = 6,
            },
            [ItemId.Feraligatrite] = new()
            {
                Id = ItemId.Feraligatrite,
                Name = "Feraligatrite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Feraligatr] = SpecieId.FeraligatrMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2013,
                Gen = 6,
            },
            [ItemId.Floettite] = new()
            {
                Id = ItemId.Floettite,
                Name = "Floettite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Floette] = SpecieId.FloetteMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2014,
                Gen = 6,
            },
            [ItemId.Froslassite] = new()
            {
                Id = ItemId.Froslassite,
                Name = "Froslassite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Froslass] = SpecieId.FroslassMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2015,
                Gen = 6,
            },
            [ItemId.Greninjite] = new()
            {
                Id = ItemId.Greninjite,
                Name = "Greninjite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Greninja] = SpecieId.GreninjaMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2016,
                Gen = 6,
            },
            [ItemId.Hawluchite] = new()
            {
                Id = ItemId.Hawluchite,
                Name = "Hawluchite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Hawlucha] = SpecieId.HawluchaMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2017,
                Gen = 6,
            },
            [ItemId.Malamarite] = new()
            {
                Id = ItemId.Malamarite,
                Name = "Malamarite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Malamar] = SpecieId.MalamarMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2018,
                Gen = 6,
            },
            [ItemId.Meganiummite] = new()
            {
                Id = ItemId.Meganiummite,
                Name = "Meganiummite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Meganium] = SpecieId.MeganiummMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2019,
                Gen = 6,
            },
            [ItemId.Pyroarite] = new()
            {
                Id = ItemId.Pyroarite,
                Name = "Pyroarite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Pyroar] = SpecieId.PyroarMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2020,
                Gen = 6,
            },
            [ItemId.Scolipedite] = new()
            {
                Id = ItemId.Scolipedite,
                Name = "Scolipedite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Scolipede] = SpecieId.ScolipedeMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2021,
                Gen = 6,
            },
            [ItemId.Scraftite] = new()
            {
                Id = ItemId.Scraftite,
                Name = "Scraftite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Scrafty] = SpecieId.ScraftyMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2022,
                Gen = 6,
            },
            [ItemId.Skarmoryite] = new()
            {
                Id = ItemId.Skarmoryite,
                Name = "Skarmoryite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Skarmory] = SpecieId.SkarmoryMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2023,
                Gen = 6,
            },
            [ItemId.Starmite] = new()
            {
                Id = ItemId.Starmite,
                Name = "Starmite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Starmie] = SpecieId.StarmieMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2024,
                Gen = 6,
            },
            [ItemId.Victrebelite] = new()
            {
                Id = ItemId.Victrebelite,
                Name = "Victrebelite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Victreebel] = SpecieId.VictreebelMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2025,
                Gen = 6,
            },
            [ItemId.Zygardite] = new()
            {
                Id = ItemId.Zygardite,
                Name = "Zygardite",
                MegaStone = new Dictionary<SpecieId, SpecieId>
                {
                    [SpecieId.Zygarde] = SpecieId.ZygardeMega,
                }.AsReadOnly(),
                OnTakeItem = onTakeItem,
                Num = 2026,
                Gen = 6,
            },
        };
    }
}
