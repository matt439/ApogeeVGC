"""
Tests for the Pokemon Showdown replay parser.

Tests cover:
  - Helper functions (HP parsing, slot parsing, species detail parsing)
  - Full replay parsing (metadata, team preview, turns, bench, revealed info)
  - Edge cases (form changes, tera, weather, terrain, screens, forfeits)

Run:  python -m unittest test_parser -v
"""

import unittest
from parser import (
    parse_hp,
    parse_status_from_hp,
    parse_slot,
    parse_species_detail,
    player_side,
    parse_replay,
)


# ── Helper function tests ────────────────────────────────────────────────────


class TestParseHp(unittest.TestCase):
    def test_full_hp(self):
        self.assertEqual(parse_hp("100/100"), (100, 100))

    def test_partial_hp(self):
        self.assertEqual(parse_hp("62/100"), (62, 100))

    def test_zero_hp(self):
        self.assertEqual(parse_hp("0/100"), (0, 100))

    def test_fainted(self):
        self.assertEqual(parse_hp("0 fnt"), (0, 100))

    def test_hp_with_status(self):
        self.assertEqual(parse_hp("80/100 par"), (80, 100))

    def test_hp_with_burn(self):
        self.assertEqual(parse_hp("94/100 brn"), (94, 100))

    def test_non_percentage_hp(self):
        """Some formats use actual HP values like '267/341'."""
        self.assertEqual(parse_hp("267/341"), (267, 341))


class TestParseStatusFromHp(unittest.TestCase):
    def test_no_status(self):
        self.assertIsNone(parse_status_from_hp("100/100"))

    def test_paralysis(self):
        self.assertEqual(parse_status_from_hp("80/100 par"), "par")

    def test_burn(self):
        self.assertEqual(parse_status_from_hp("94/100 brn"), "brn")

    def test_sleep(self):
        self.assertEqual(parse_status_from_hp("100/100 slp"), "slp")

    def test_poison(self):
        self.assertEqual(parse_status_from_hp("50/100 psn"), "psn")

    def test_toxic(self):
        self.assertEqual(parse_status_from_hp("50/100 tox"), "tox")

    def test_freeze(self):
        self.assertEqual(parse_status_from_hp("50/100 frz"), "frz")

    def test_fainted_no_status(self):
        self.assertIsNone(parse_status_from_hp("0 fnt"))


class TestParseSlot(unittest.TestCase):
    def test_basic(self):
        self.assertEqual(parse_slot("p1a: Dragapult"), ("p1a", "Dragapult"))

    def test_p2(self):
        self.assertEqual(parse_slot("p2b: Tinkaton"), ("p2b", "Tinkaton"))

    def test_nickname_with_spaces(self):
        self.assertEqual(parse_slot("p1a: My Cool Mon"), ("p1a", "My Cool Mon"))

    def test_no_nickname(self):
        slot, nick = parse_slot("p1a")
        self.assertEqual(slot, "p1a")
        self.assertEqual(nick, "")


class TestParseSpeciesDetail(unittest.TestCase):
    def test_basic(self):
        self.assertEqual(
            parse_species_detail("Dragapult, L50, M"),
            ("Dragapult", 50, "M"),
        )

    def test_female(self):
        self.assertEqual(
            parse_species_detail("Tinkaton, L50, F"),
            ("Tinkaton", 50, "F"),
        )

    def test_genderless(self):
        self.assertEqual(
            parse_species_detail("Eternatus, L50"),
            ("Eternatus", 50, None),
        )

    def test_form(self):
        species, level, gender = parse_species_detail("Calyrex-Ice, L50")
        self.assertEqual(species, "Calyrex-Ice")
        self.assertEqual(level, 50)

    def test_zamazenta_crowned(self):
        species, level, gender = parse_species_detail("Zamazenta-Crowned, L50")
        self.assertEqual(species, "Zamazenta-Crowned")

    def test_shiny(self):
        species, level, gender = parse_species_detail("Dragapult, L50, M, shiny")
        self.assertEqual(species, "Dragapult")
        self.assertEqual(gender, "M")

    def test_different_level(self):
        _, level, _ = parse_species_detail("Pikachu, L100")
        self.assertEqual(level, 100)

    def test_zamazenta_star(self):
        """Team preview shows Zamazenta-* before form is revealed."""
        species, _, _ = parse_species_detail("Zamazenta-*, L50")
        self.assertEqual(species, "Zamazenta-*")


class TestPlayerSide(unittest.TestCase):
    def test_p1a(self):
        self.assertEqual(player_side("p1a"), "p1")

    def test_p2b(self):
        self.assertEqual(player_side("p2b"), "p2")


# ── Full replay parsing tests ────────────────────────────────────────────────


def _make_replay(log: str, replay_id: str = "test-1", rating: int = 1200) -> dict:
    """Helper to create a replay data dict from a log string."""
    return {
        "id": replay_id,
        "formatid": "gen9vgc2025regi",
        "log": log,
        "rating": rating,
    }


# A realistic VGC doubles log (simplified from real replay gen9vgc2025regi-2383265550)
DOUBLES_LOG = """|j|Player1
|j|Player2
|gametype|doubles
|player|p1|Player1|avatar1|1125
|player|p2|Player2|avatar2|1163
|teamsize|p1|6
|teamsize|p2|6
|gen|9
|tier|[Gen 9] VGC 2025 Reg I
|poke|p1|Glimmora, L50, M|
|poke|p1|Cosmoem, L50|
|poke|p1|Dragapult, L50, M|
|poke|p1|Calyrex-Ice, L50|
|poke|p1|Flamigo, L50, F|
|poke|p1|Clefable, L50, F|
|poke|p2|Eternatus, L50|
|poke|p2|Tornadus, L50, M|
|poke|p2|Tinkaton, L50, F|
|poke|p2|Chi-Yu, L50|
|poke|p2|Flutter Mane, L50|
|poke|p2|Zamazenta-*, L50|
|teampreview|4
|start
|switch|p1a: Dragapult|Dragapult, L50, M|100/100
|switch|p1b: Cosmoem|Cosmoem, L50|100/100
|switch|p2a: Eternatus|Eternatus, L50|100/100
|switch|p2b: Tinkaton|Tinkaton, L50, F|100/100
|-ability|p2a: Eternatus|Pressure
|-ability|p2b: Tinkaton|Mold Breaker
|turn|1
|move|p2b: Tinkaton|Fake Out|p1a: Dragapult
|-immune|p1a: Dragapult
|move|p1a: Dragapult|Will-O-Wisp|p2b: Tinkaton
|-status|p2b: Tinkaton|brn
|move|p2a: Eternatus|Dynamax Cannon|p1a: Dragapult
|-damage|p1a: Dragapult|0 fnt
|faint|p1a: Dragapult
|-damage|p2a: Eternatus|91/100|[from] item: Life Orb
|move|p1b: Cosmoem|Cosmic Power|p1b: Cosmoem
|-boost|p1b: Cosmoem|def|1
|-boost|p1b: Cosmoem|spd|1
|-damage|p2b: Tinkaton|94/100 brn|[from] brn
|upkeep
|switch|p1a: Calyrex|Calyrex-Ice, L50|100/100
|-ability|p1a: Calyrex|As One
|turn|2
|move|p2a: Eternatus|Dynamax Cannon|p1a: Calyrex
|-damage|p1a: Calyrex|62/100
|-damage|p2a: Eternatus|81/100|[from] item: Life Orb
|move|p1a: Calyrex|Leech Seed|p2a: Eternatus
|move|p1b: Cosmoem|Cosmic Power|p1b: Cosmoem
|-boost|p1b: Cosmoem|def|1
|-boost|p1b: Cosmoem|spd|1
|-heal|p1a: Calyrex|68/100|[from] item: Leftovers
|-damage|p2b: Tinkaton|88/100 brn|[from] brn
|upkeep
|turn|3
|move|p2a: Eternatus|Dynamax Cannon|p1a: Calyrex
|-damage|p1a: Calyrex|0 fnt
|faint|p1a: Calyrex
|-damage|p2a: Eternatus|59/100|[from] item: Life Orb
|move|p2b: Tinkaton|Knock Off|p1b: Cosmoem
|-damage|p1b: Cosmoem|83/100
|-enditem|p1b: Cosmoem|Eviolite|[from] move: Knock Off|[of] p2b: Tinkaton
|move|p1b: Cosmoem|Cosmic Power|p1b: Cosmoem
|-boost|p1b: Cosmoem|def|1
|-boost|p1b: Cosmoem|spd|1
|-damage|p2b: Tinkaton|82/100 brn|[from] brn
|upkeep
|switch|p1a: Clefable|Clefable, L50, F|100/100
|turn|4
|-terastallize|p2a: Eternatus|Fairy
|move|p2a: Eternatus|Flamethrower|p1b: Cosmoem
|-damage|p1b: Cosmoem|66/100
|-damage|p2a: Eternatus|49/100|[from] item: Life Orb
|move|p1a: Clefable|Night Shade|p2a: Eternatus
|-damage|p2a: Eternatus|26/100
|-damage|p2b: Tinkaton|75/100 brn|[from] brn
|upkeep
|turn|5
|move|p1a: Clefable|Night Shade|p2b: Tinkaton
|-damage|p2b: Tinkaton|49/100 brn
|-damage|p2a: Eternatus|0 fnt|[from] Leech Seed|[of] p1a: Clefable
|faint|p2a: Eternatus
|-damage|p2b: Tinkaton|43/100 brn|[from] brn
|upkeep
|switch|p2a: Flutter Mane|Flutter Mane, L50|100/100
|-enditem|p2a: Flutter Mane|Booster Energy
|turn|6
|win|Player2
|raw|Player1's rating: 1125 &rarr; <strong>1103</strong>
|raw|Player2's rating: 1163 &rarr; <strong>1185</strong>"""


class TestParseReplayMetadata(unittest.TestCase):
    def setUp(self):
        self.result = parse_replay(_make_replay(DOUBLES_LOG))

    def test_not_none(self):
        self.assertIsNotNone(self.result)

    def test_players(self):
        self.assertEqual(self.result["players"]["p1"]["name"], "Player1")
        self.assertEqual(self.result["players"]["p2"]["name"], "Player2")

    def test_ratings_from_raw(self):
        self.assertEqual(self.result["players"]["p1"]["rating_before"], 1125)
        self.assertEqual(self.result["players"]["p1"]["rating_after"], 1103)
        self.assertEqual(self.result["players"]["p2"]["rating_before"], 1163)
        self.assertEqual(self.result["players"]["p2"]["rating_after"], 1185)

    def test_winner(self):
        self.assertEqual(self.result["winner"], "p2")

    def test_format(self):
        self.assertEqual(self.result["format"], "gen9vgc2025regi")


class TestParseReplayTeams(unittest.TestCase):
    def setUp(self):
        self.result = parse_replay(_make_replay(DOUBLES_LOG))

    def test_team_preview_count(self):
        self.assertEqual(len(self.result["team_preview"]["p1"]), 6)
        self.assertEqual(len(self.result["team_preview"]["p2"]), 6)

    def test_team_preview_species(self):
        p1_species = [p["species"] for p in self.result["team_preview"]["p1"]]
        self.assertIn("Dragapult", p1_species)
        self.assertIn("Calyrex-Ice", p1_species)
        self.assertIn("Cosmoem", p1_species)
        self.assertIn("Clefable", p1_species)

    def test_team_preview_gender(self):
        p1_preview = {p["species"]: p for p in self.result["team_preview"]["p1"]}
        self.assertEqual(p1_preview["Dragapult"]["gender"], "M")
        self.assertEqual(p1_preview["Clefable"]["gender"], "F")
        self.assertIsNone(p1_preview["Cosmoem"]["gender"])

    def test_team_brought(self):
        """Only pokemon that actually appeared in battle."""
        p1_brought = self.result["team_brought"]["p1"]
        self.assertIn("Dragapult", p1_brought)
        self.assertIn("Cosmoem", p1_brought)
        self.assertIn("Calyrex-Ice", p1_brought)
        self.assertIn("Clefable", p1_brought)
        # Glimmora and Flamigo were never switched in
        self.assertNotIn("Glimmora", p1_brought)
        self.assertNotIn("Flamigo", p1_brought)


class TestParseReplayTurns(unittest.TestCase):
    def setUp(self):
        self.result = parse_replay(_make_replay(DOUBLES_LOG))
        self.turns = self.result["turns"]

    def test_turn_count(self):
        self.assertEqual(self.result["turn_count"], 6)
        # Last turn (6) has win immediately, so turns list may have 5 or 6 entries
        self.assertGreaterEqual(len(self.turns), 5)

    def test_turn1_active_state(self):
        """Turn 1 snapshot should show the initial leads."""
        t1 = self.turns[0]
        self.assertEqual(t1["turn"], 1)
        self.assertEqual(t1["active"]["p1a"]["species"], "Dragapult")
        self.assertEqual(t1["active"]["p1a"]["hp"], 100)
        self.assertEqual(t1["active"]["p1b"]["species"], "Cosmoem")
        self.assertEqual(t1["active"]["p2a"]["species"], "Eternatus")
        self.assertEqual(t1["active"]["p2b"]["species"], "Tinkaton")

    def test_turn1_actions(self):
        t1 = self.turns[0]
        actions = t1["actions"]
        move_details = [(a["slot"], a["type"], a["detail"]) for a in actions]
        self.assertIn(("p2b", "move", "Fake Out"), move_details)
        self.assertIn(("p1a", "move", "Will-O-Wisp"), move_details)
        self.assertIn(("p2a", "move", "Dynamax Cannon"), move_details)
        self.assertIn(("p1b", "move", "Cosmic Power"), move_details)

    def test_turn1_faint(self):
        """Dragapult faints in turn 1."""
        t1 = self.turns[0]
        self.assertIn("p1a", t1["faints"])

    def test_turn2_calyrex_switched_in(self):
        """At start of turn 2, Calyrex-Ice should be in p1a slot."""
        t2 = self.turns[1]
        self.assertEqual(t2["active"]["p1a"]["species"], "Calyrex-Ice")
        self.assertEqual(t2["active"]["p1a"]["hp"], 100)

    def test_turn2_cosmoem_boosts_carried(self):
        """Cosmoem got +1 def +1 spd in turn 1, should show at turn 2 start."""
        t2 = self.turns[1]
        boosts = t2["active"]["p1b"]["boosts"]
        self.assertEqual(boosts.get("def"), 1)
        self.assertEqual(boosts.get("spd"), 1)

    def test_turn2_tinkaton_status(self):
        """Tinkaton was burned in turn 1."""
        t2 = self.turns[1]
        self.assertEqual(t2["active"]["p2b"]["status"], "brn")

    def test_turn3_calyrex_hp_after_damage_and_heal(self):
        """At start of turn 3, Calyrex took 38 damage then healed 6 from Leftovers = 68 HP."""
        t3 = self.turns[2]
        self.assertEqual(t3["active"]["p1a"]["hp"], 68)

    def test_turn4_tera(self):
        """Turn 4: Eternatus terastallizes to Fairy."""
        t4 = self.turns[3]
        actions = t4["actions"]
        tera_actions = [a for a in actions if a.get("tera")]
        # Eternatus should have tera: Fairy on its move action
        eternatus_actions = [a for a in actions if a["slot"] == "p2a"]
        self.assertTrue(
            any(a.get("tera") == "Fairy" for a in eternatus_actions),
            f"Expected Eternatus tera=Fairy, got: {eternatus_actions}",
        )

    def test_turn5_eternatus_faint(self):
        """Eternatus faints from Leech Seed in turn 5."""
        t5 = self.turns[4]
        self.assertIn("p2a", t5["faints"])


class TestParseReplayBench(unittest.TestCase):
    def setUp(self):
        self.result = parse_replay(_make_replay(DOUBLES_LOG))
        self.turns = self.result["turns"]

    def test_turn1_no_bench(self):
        """At turn 1, no pokemon have been switched out yet — bench may be empty."""
        t1 = self.turns[0]
        bench = t1.get("bench", {})
        # Neither side has benched anything yet (initial leads)
        p1_bench_species = [b["species"] for b in bench.get("p1", [])]
        p2_bench_species = [b["species"] for b in bench.get("p2", [])]
        self.assertNotIn("Dragapult", p1_bench_species)
        self.assertNotIn("Cosmoem", p1_bench_species)

    def test_turn2_dragapult_on_bench_fainted(self):
        """After turn 1, Dragapult fainted and Calyrex switched in.
        At turn 2 start, Dragapult should be on bench as fainted."""
        t2 = self.turns[1]
        bench = t2.get("bench", {})
        p1_bench = bench.get("p1", [])
        dragapult = next((b for b in p1_bench if b["species"] == "Dragapult"), None)
        self.assertIsNotNone(dragapult, f"Dragapult should be on p1 bench, got: {p1_bench}")
        self.assertTrue(dragapult["fainted"])
        self.assertEqual(dragapult["hp"], 0)

    def test_turn4_calyrex_on_bench_fainted(self):
        """After turn 3, Calyrex-Ice also fainted. At turn 4, both should be on bench."""
        t4 = self.turns[3]
        bench = t4.get("bench", {})
        p1_bench = bench.get("p1", [])
        p1_bench_species = [b["species"] for b in p1_bench]
        self.assertIn("Dragapult", p1_bench_species)
        self.assertIn("Calyrex-Ice", p1_bench_species)
        calyrex = next(b for b in p1_bench if b["species"] == "Calyrex-Ice")
        self.assertTrue(calyrex["fainted"])

    def test_turn6_eternatus_on_bench_fainted(self):
        """After turn 5, Eternatus fainted. At turn 6, it should be on p2 bench."""
        t6 = self.turns[5]
        bench = t6.get("bench", {})
        p2_bench = bench.get("p2", [])
        eternatus = next((b for b in p2_bench if b["species"] == "Eternatus"), None)
        self.assertIsNotNone(eternatus, f"Eternatus should be on p2 bench, got: {p2_bench}")
        self.assertTrue(eternatus["fainted"])


class TestParseReplayRevealedInfo(unittest.TestCase):
    def setUp(self):
        self.result = parse_replay(_make_replay(DOUBLES_LOG))
        self.revealed = self.result["revealed"]

    def test_dragapult_moves(self):
        self.assertIn("Will-O-Wisp", self.revealed["p1"]["Dragapult"]["moves"])

    def test_eternatus_moves(self):
        moves = self.revealed["p2"]["Eternatus"]["moves"]
        self.assertIn("Dynamax Cannon", moves)
        self.assertIn("Flamethrower", moves)

    def test_tinkaton_moves(self):
        moves = self.revealed["p2"]["Tinkaton"]["moves"]
        self.assertIn("Fake Out", moves)
        self.assertIn("Knock Off", moves)

    def test_cosmoem_moves(self):
        self.assertIn("Cosmic Power", self.revealed["p1"]["Cosmoem"]["moves"])

    def test_eternatus_ability(self):
        self.assertEqual(self.revealed["p2"]["Eternatus"]["ability"], "Pressure")

    def test_tinkaton_ability(self):
        self.assertEqual(self.revealed["p2"]["Tinkaton"]["ability"], "Mold Breaker")

    def test_calyrex_ability(self):
        self.assertEqual(self.revealed["p1"]["Calyrex-Ice"]["ability"], "As One")

    def test_eternatus_item(self):
        self.assertEqual(self.revealed["p2"]["Eternatus"]["item"], "Life Orb")

    def test_cosmoem_item(self):
        self.assertEqual(self.revealed["p1"]["Cosmoem"]["item"], "Eviolite")

    def test_calyrex_item(self):
        self.assertEqual(self.revealed["p1"]["Calyrex-Ice"]["item"], "Leftovers")

    def test_eternatus_tera(self):
        self.assertEqual(self.revealed["p2"]["Eternatus"]["tera_type"], "Fairy")

    def test_flutter_mane_item(self):
        self.assertEqual(self.revealed["p2"]["Flutter Mane"]["item"], "Booster Energy")


# ── Edge case tests with synthetic logs ───────────────────────────────────────


class TestWeatherAndTerrain(unittest.TestCase):
    def test_weather_set_and_clear(self):
        log = """|gametype|doubles
|player|p1|A||1000
|player|p2|B||1000
|poke|p1|Kyogre, L50|
|poke|p2|Groudon, L50|
|switch|p1a: Kyogre|Kyogre, L50|100/100
|switch|p2a: Groudon|Groudon, L50|100/100
|-weather|RainDance|[from] ability: Drizzle|[of] p1a: Kyogre
|turn|1
|-weather|RainDance|[upkeep]
|move|p1a: Kyogre|Water Spout|p2a: Groudon
|turn|2
|-weather|none
|move|p1a: Kyogre|Water Spout|p2a: Groudon
|turn|3
|win|A"""
        result = parse_replay(_make_replay(log))
        # Turn 1: rain should be active
        self.assertEqual(result["turns"][0]["field"]["weather"], "RainDance")
        # Turn 2: rain still up (upkeep doesn't change it)
        self.assertEqual(result["turns"][1]["field"]["weather"], "RainDance")
        # Turn 3: weather cleared
        self.assertNotIn("weather", result["turns"][2]["field"])

    def test_terrain(self):
        log = """|gametype|doubles
|player|p1|A||1000
|player|p2|B||1000
|poke|p1|Rillaboom, L50, M|
|poke|p2|Incineroar, L50, M|
|switch|p1a: Rillaboom|Rillaboom, L50, M|100/100
|switch|p2a: Incineroar|Incineroar, L50, M|100/100
|-fieldstart|move: Grassy Terrain|[from] ability: Grassy Surge|[of] p1a: Rillaboom
|turn|1
|move|p1a: Rillaboom|Fake Out|p2a: Incineroar
|turn|2
|-fieldend|move: Grassy Terrain
|move|p1a: Rillaboom|Fake Out|p2a: Incineroar
|turn|3
|win|A"""
        result = parse_replay(_make_replay(log))
        self.assertIn("terrain", result["turns"][0]["field"])
        self.assertNotIn("terrain", result["turns"][2]["field"])


class TestTrickRoom(unittest.TestCase):
    def test_trick_room_set_and_expire(self):
        log = """|gametype|doubles
|player|p1|A||1000
|player|p2|B||1000
|poke|p1|Dusclops, L50, M|
|poke|p2|Incineroar, L50, M|
|switch|p1a: Dusclops|Dusclops, L50, M|100/100
|switch|p2a: Incineroar|Incineroar, L50, M|100/100
|turn|1
|move|p1a: Dusclops|Trick Room|p1a: Dusclops
|-fieldstart|move: Trick Room|[of] p1a: Dusclops
|turn|2
|move|p1a: Dusclops|Night Shade|p2a: Incineroar
|-damage|p2a: Incineroar|75/100
|turn|3
|-fieldend|move: Trick Room
|move|p1a: Dusclops|Night Shade|p2a: Incineroar
|turn|4
|win|A"""
        result = parse_replay(_make_replay(log))
        # Turn 1: no trick room yet (set happens during turn 1)
        self.assertNotIn("trick_room", result["turns"][0]["field"])
        # Turn 2: trick room active
        self.assertTrue(result["turns"][1]["field"].get("trick_room"))
        # Turn 3: still active (ends during turn 3)
        self.assertTrue(result["turns"][2]["field"].get("trick_room"))
        # Turn 4: expired
        self.assertNotIn("trick_room", result["turns"][3]["field"])


class TestScreensAndTailwind(unittest.TestCase):
    def test_tailwind(self):
        log = """|gametype|doubles
|player|p1|A||1000
|player|p2|B||1000
|poke|p1|Tornadus, L50, M|
|poke|p2|Incineroar, L50, M|
|switch|p1a: Tornadus|Tornadus, L50, M|100/100
|switch|p2a: Incineroar|Incineroar, L50, M|100/100
|turn|1
|move|p1a: Tornadus|Tailwind|p1a: Tornadus
|-sidestart|p1: A|move: Tailwind
|turn|2
|move|p1a: Tornadus|Air Slash|p2a: Incineroar
|turn|3
|-sideend|p1: A|move: Tailwind
|move|p1a: Tornadus|Air Slash|p2a: Incineroar
|turn|4
|win|A"""
        result = parse_replay(_make_replay(log))
        self.assertNotIn("p1_tailwind", result["turns"][0]["field"])
        self.assertTrue(result["turns"][1]["field"].get("p1_tailwind"))
        self.assertTrue(result["turns"][2]["field"].get("p1_tailwind"))
        self.assertNotIn("p1_tailwind", result["turns"][3]["field"])

    def test_reflect(self):
        log = """|gametype|doubles
|player|p1|A||1000
|player|p2|B||1000
|poke|p1|Grimmsnarl, L50, M|
|poke|p2|Incineroar, L50, M|
|switch|p1a: Grimmsnarl|Grimmsnarl, L50, M|100/100
|switch|p2a: Incineroar|Incineroar, L50, M|100/100
|turn|1
|move|p1a: Grimmsnarl|Reflect|p1a: Grimmsnarl
|-sidestart|p1: A|Reflect
|turn|2
|win|A"""
        result = parse_replay(_make_replay(log))
        self.assertTrue(result["turns"][1]["field"].get("p1_reflect"))


class TestFormChange(unittest.TestCase):
    def test_detailschange(self):
        log = """|gametype|doubles
|player|p1|A||1000
|player|p2|B||1000
|poke|p1|Meloetta, L50|
|poke|p2|Incineroar, L50, M|
|switch|p1a: Meloetta|Meloetta, L50|100/100
|switch|p2a: Incineroar|Incineroar, L50, M|100/100
|turn|1
|move|p1a: Meloetta|Relic Song|p2a: Incineroar
|-damage|p2a: Incineroar|80/100
|detailschange|p1a: Meloetta|Meloetta-Pirouette, L50
|turn|2
|move|p1a: Meloetta|Close Combat|p2a: Incineroar
|turn|3
|win|A"""
        result = parse_replay(_make_replay(log))
        # Turn 1: still base form
        self.assertEqual(result["turns"][0]["active"]["p1a"]["species"], "Meloetta")
        # Turn 2: form changed
        self.assertEqual(result["turns"][1]["active"]["p1a"]["species"], "Meloetta-Pirouette")


class TestSwitchAction(unittest.TestCase):
    def test_switch_recorded_as_action(self):
        log = """|gametype|doubles
|player|p1|A||1000
|player|p2|B||1000
|poke|p1|Incineroar, L50, M|
|poke|p1|Rillaboom, L50, M|
|poke|p2|Flutter Mane, L50|
|switch|p1a: Incineroar|Incineroar, L50, M|100/100
|switch|p2a: Flutter Mane|Flutter Mane, L50|100/100
|turn|1
|switch|p1a: Rillaboom|Rillaboom, L50, M|100/100
|move|p2a: Flutter Mane|Shadow Ball|p1a: Rillaboom
|-damage|p1a: Rillaboom|60/100
|turn|2
|win|A"""
        result = parse_replay(_make_replay(log))
        t1 = result["turns"][0]
        switch_actions = [a for a in t1["actions"] if a["type"] == "switch"]
        self.assertEqual(len(switch_actions), 1)
        self.assertEqual(switch_actions[0]["slot"], "p1a")
        self.assertEqual(switch_actions[0]["detail"], "Rillaboom")

    def test_bench_updated_on_switch(self):
        log = """|gametype|doubles
|player|p1|A||1000
|player|p2|B||1000
|poke|p1|Incineroar, L50, M|
|poke|p1|Rillaboom, L50, M|
|poke|p2|Flutter Mane, L50|
|switch|p1a: Incineroar|Incineroar, L50, M|100/100
|switch|p2a: Flutter Mane|Flutter Mane, L50|100/100
|turn|1
|move|p2a: Flutter Mane|Shadow Ball|p1a: Incineroar
|-damage|p1a: Incineroar|60/100
|turn|2
|switch|p1a: Rillaboom|Rillaboom, L50, M|100/100
|move|p2a: Flutter Mane|Shadow Ball|p1a: Rillaboom
|-damage|p1a: Rillaboom|70/100
|turn|3
|win|A"""
        result = parse_replay(_make_replay(log))
        # Turn 3: Incineroar was switched out with 60 HP, should be on bench
        t3 = result["turns"][2]
        bench = t3.get("bench", {})
        p1_bench = bench.get("p1", [])
        incineroar = next((b for b in p1_bench if b["species"] == "Incineroar"), None)
        self.assertIsNotNone(incineroar, f"Incineroar should be on bench, got: {p1_bench}")
        self.assertEqual(incineroar["hp"], 60)
        self.assertFalse(incineroar["fainted"])


class TestDragForcedSwitch(unittest.TestCase):
    def test_drag_not_recorded_as_action(self):
        """Drag (forced switch) should NOT be recorded as a player action."""
        log = """|gametype|doubles
|player|p1|A||1000
|player|p2|B||1000
|poke|p1|Incineroar, L50, M|
|poke|p1|Rillaboom, L50, M|
|poke|p2|Whirlwind User, L50|
|switch|p1a: Incineroar|Incineroar, L50, M|100/100
|switch|p2a: Whirlwind User|Whirlwind User, L50|100/100
|turn|1
|move|p2a: Whirlwind User|Whirlwind|p1a: Incineroar
|drag|p1a: Rillaboom|Rillaboom, L50, M|100/100
|turn|2
|win|A"""
        result = parse_replay(_make_replay(log))
        t1 = result["turns"][0]
        # Drag should not appear as a switch action
        switch_actions = [a for a in t1["actions"] if a["type"] == "switch"]
        self.assertEqual(len(switch_actions), 0)


class TestCantMove(unittest.TestCase):
    def test_cant_recorded(self):
        log = """|gametype|doubles
|player|p1|A||1000
|player|p2|B||1000
|poke|p1|Snorlax, L50, M|
|poke|p2|Incineroar, L50, M|
|switch|p1a: Snorlax|Snorlax, L50, M|100/100
|switch|p2a: Incineroar|Incineroar, L50, M|100/100
|-status|p1a: Snorlax|slp
|turn|1
|cant|p1a: Snorlax|slp
|move|p2a: Incineroar|Flare Blitz|p1a: Snorlax
|-damage|p1a: Snorlax|70/100 slp
|turn|2
|win|A"""
        result = parse_replay(_make_replay(log))
        t1 = result["turns"][0]
        cant_actions = [a for a in t1["actions"] if a["type"] == "cant"]
        self.assertEqual(len(cant_actions), 1)
        self.assertEqual(cant_actions[0]["slot"], "p1a")
        self.assertEqual(cant_actions[0]["detail"], "slp")


class TestNoWinner(unittest.TestCase):
    def test_tie_returns_none_winner(self):
        log = """|gametype|doubles
|player|p1|A||1000
|player|p2|B||1000
|poke|p1|Snorlax, L50, M|
|poke|p2|Incineroar, L50, M|
|switch|p1a: Snorlax|Snorlax, L50, M|100/100
|switch|p2a: Incineroar|Incineroar, L50, M|100/100
|turn|1
|tie"""
        result = parse_replay(_make_replay(log))
        self.assertIsNone(result["winner"])


class TestEmptyLog(unittest.TestCase):
    def test_empty_log_returns_none(self):
        result = parse_replay({"id": "test", "formatid": "test", "log": ""})
        self.assertIsNone(result)

    def test_no_log_returns_none(self):
        result = parse_replay({"id": "test", "formatid": "test"})
        self.assertIsNone(result)

    def test_no_players_returns_none(self):
        result = parse_replay(_make_replay("|turn|1\n|win|Nobody"))
        self.assertIsNone(result)


class TestCureStatus(unittest.TestCase):
    def test_status_cured(self):
        log = """|gametype|doubles
|player|p1|A||1000
|player|p2|B||1000
|poke|p1|Snorlax, L50, M|
|poke|p2|Incineroar, L50, M|
|switch|p1a: Snorlax|Snorlax, L50, M|100/100
|switch|p2a: Incineroar|Incineroar, L50, M|100/100
|turn|1
|-status|p1a: Snorlax|par
|move|p2a: Incineroar|Fake Out|p1a: Snorlax
|turn|2
|-curestatus|p1a: Snorlax|par
|move|p1a: Snorlax|Body Slam|p2a: Incineroar
|turn|3
|win|A"""
        result = parse_replay(_make_replay(log))
        # Turn 2: paralyzed
        self.assertEqual(result["turns"][1]["active"]["p1a"]["status"], "par")
        # Turn 3: cured
        self.assertIsNone(result["turns"][2]["active"]["p1a"]["status"])


class TestMoveTargets(unittest.TestCase):
    def test_move_target_parsed(self):
        log = """|gametype|doubles
|player|p1|A||1000
|player|p2|B||1000
|poke|p1|Incineroar, L50, M|
|poke|p2|Flutter Mane, L50|
|poke|p2|Rillaboom, L50, M|
|switch|p1a: Incineroar|Incineroar, L50, M|100/100
|switch|p2a: Flutter Mane|Flutter Mane, L50|100/100
|switch|p2b: Rillaboom|Rillaboom, L50, M|100/100
|turn|1
|move|p1a: Incineroar|Flare Blitz|p2b: Rillaboom
|-damage|p2b: Rillaboom|50/100
|turn|2
|win|A"""
        result = parse_replay(_make_replay(log))
        t1 = result["turns"][0]
        incin_action = next(a for a in t1["actions"] if a["slot"] == "p1a")
        self.assertEqual(incin_action["target"], "p2b")


if __name__ == "__main__":
    unittest.main()
