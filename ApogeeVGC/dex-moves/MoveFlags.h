#pragma once

#include <optional>

struct MoveFlags
{
    // The move plays its animation when used on an ally.
    std::optional<bool> allyanim = false;
    // Ignores a target's substitute.
    std::optional<bool> bypasssub = false;
    // Power is multiplied by 1.5 when used by a Pokemon with the Ability Strong Jaw.
    std::optional<bool> bite = false;
    // Has no effect on Pokemon with the Ability Bulletproof.
    std::optional<bool> bullet = false;
    // The user cannot select this move after a previous successful use.
    std::optional<bool> cantusetwice = false;
    // The user is unable to make a move between turns.
    std::optional<bool> charge = false;
    // Makes contact.
    std::optional<bool> contact = false;
    // When used by a Pokemon, other Pokemon with the Ability Dancer can attempt to execute the same move.
    std::optional<bool> dance = false;
    // Thaws the user if executed successfully while the user is frozen.
    std::optional<bool> defrost = false;
    // Can target a Pokemon positioned anywhere in a Triple Battle.
    std::optional<bool> distance = false;
    // Cannot be selected by Copycat.
    std::optional<bool> failcopycat = false;
    // Encore fails if target used this move.
    std::optional<bool> failencore = false;
    // Cannot be repeated by Instruct.
    std::optional<bool> failinstruct = false;
    // Cannot be selected by Me First.
    std::optional<bool> failmefirst = false;
    // Cannot be copied by Mimic.
    std::optional<bool> failmimic = false;
    // Targets a slot, and in 2 turns damages that slot.
    std::optional<bool> futuremove = false;
    // Prevented from being executed or selected during Gravity's effect.
    std::optional<bool> gravity = false;
    // Prevented from being executed or selected during Heal Block's effect.
    std::optional<bool> heal = false;
    // Can be selected by Metronome.
    std::optional<bool> metronome = false;
    // Can be copied by Mirror Move.
    std::optional<bool> mirror = false;
    // Additional PP is deducted due to Pressure when it ordinarily would not.
    std::optional<bool> mustpressure = false;
    // Cannot be selected by Assist.
    std::optional<bool> noassist = false;
    // Prevented from being executed or selected in a Sky Battle.
    std::optional<bool> nonsky = false;
    // Cannot be made to hit twice via Parental Bond.
    std::optional<bool> noparentalbond = false;
    // Cannot be copied by Sketch.
    std::optional<bool> nosketch = false;
    // Cannot be selected by Sleep Talk.
    std::optional<bool> nosleeptalk = false;
    // Gems will not activate. Cannot be redirected by Storm Drain / Lightning Rod.
    std::optional<bool> pledgecombo = false;
    // Has no effect on Pokemon which are Grass-type, have the Ability Overcoat, or hold Safety Goggles.
    std::optional<bool> powder = false;
    // Blocked by Detect, Protect, Spiky Shield, and if not a Status move, King's Shield.
    std::optional<bool> protect = false;
    // Power is multiplied by 1.5 when used by a Pokemon with the Ability Mega Launcher.
    std::optional<bool> pulse = false;
    // Power is multiplied by 1.2 when used by a Pokemon with the Ability Iron Fist.
    std::optional<bool> punch = false;
    // If this move is successful, the user must recharge on the following turn and cannot make a move.
    std::optional<bool> recharge = false;
    // Bounced back to the original user by Magic Coat or the Ability Magic Bounce.
    std::optional<bool> reflectable = false;
    // Power is multiplied by 1.5 when used by a Pokemon with the Ability Sharpness.
    std::optional<bool> slicing = false;
    // Can be stolen from the original user and instead used by another Pokemon using Snatch.
    std::optional<bool> snatch = false;
    // Has no effect on Pokemon with the Ability Soundproof.
    std::optional<bool> sound = false;
    // Activates the Wind Power and Wind Rider Abilities.
    std::optional<bool> wind = false;

    MoveFlags() = default;
    MoveFlags(const MoveFlags&) = default;
};