#pragma once

struct MoveFlags
{
    // The move plays its animation when used on an ally.
    bool* allyanim = nullptr; // optional
    // Ignores a target's substitute.
    bool* bypasssub = nullptr; // optional
    // Power is multiplied by 1.5 when used by a Pokemon with the Ability Strong Jaw.
    bool* bite = nullptr; // optional
    // Has no effect on Pokemon with the Ability Bulletproof.
    bool* bullet = nullptr; // optional
    // The user cannot select this move after a previous successful use.
    bool* cantusetwice = nullptr; // optional
    // The user is unable to make a move between turns.
    bool* charge = nullptr; // optional
    // Makes contact.
    bool* contact = nullptr; // optional
    // When used by a Pokemon, other Pokemon with the Ability Dancer can attempt to execute the same move.
    bool* dance = nullptr; // optional
    // Thaws the user if executed successfully while the user is frozen.
    bool* defrost = nullptr; // optional
    // Can target a Pokemon positioned anywhere in a Triple Battle.
    bool* distance = nullptr; // optional
    // Cannot be selected by Copycat.
    bool* failcopycat = nullptr; // optional
    // Encore fails if target used this move.
    bool* failencore = nullptr; // optional
    // Cannot be repeated by Instruct.
    bool* failinstruct = nullptr; // optional
    // Cannot be selected by Me First.
    bool* failmefirst = nullptr; // optional
    // Cannot be copied by Mimic.
    bool* failmimic = nullptr; // optional
    // Targets a slot, and in 2 turns damages that slot.
    bool* futuremove = nullptr; // optional
    // Prevented from being executed or selected during Gravity's effect.
    bool* gravity = nullptr; // optional
    // Prevented from being executed or selected during Heal Block's effect.
    bool* heal = nullptr; // optional
    // Can be selected by Metronome.
    bool* metronome = nullptr; // optional
    // Can be copied by Mirror Move.
    bool* mirror = nullptr; // optional
    // Additional PP is deducted due to Pressure when it ordinarily would not.
    bool* mustpressure = nullptr; // optional
    // Cannot be selected by Assist.
    bool* noassist = nullptr; // optional
    // Prevented from being executed or selected in a Sky Battle.
    bool* nonsky = nullptr; // optional
    // Cannot be made to hit twice via Parental Bond.
    bool* noparentalbond = nullptr; // optional
    // Cannot be copied by Sketch.
    bool* nosketch = nullptr; // optional
    // Cannot be selected by Sleep Talk.
    bool* nosleeptalk = nullptr; // optional
    // Gems will not activate. Cannot be redirected by Storm Drain / Lightning Rod.
    bool* pledgecombo = nullptr; // optional
    // Has no effect on Pokemon which are Grass-type, have the Ability Overcoat, or hold Safety Goggles.
    bool* powder = nullptr; // optional
    // Blocked by Detect, Protect, Spiky Shield, and if not a Status move, King's Shield.
    bool* protect = nullptr; // optional
    // Power is multiplied by 1.5 when used by a Pokemon with the Ability Mega Launcher.
    bool* pulse = nullptr; // optional
    // Power is multiplied by 1.2 when used by a Pokemon with the Ability Iron Fist.
    bool* punch = nullptr; // optional
    // If this move is successful, the user must recharge on the following turn and cannot make a move.
    bool* recharge = nullptr; // optional
    // Bounced back to the original user by Magic Coat or the Ability Magic Bounce.
    bool* reflectable = nullptr; // optional
    // Power is multiplied by 1.5 when used by a Pokemon with the Ability Sharpness.
    bool* slicing = nullptr; // optional
    // Can be stolen from the original user and instead used by another Pokemon using Snatch.
    bool* snatch = nullptr; // optional
    // Has no effect on Pokemon with the Ability Soundproof.
    bool* sound = nullptr; // optional
    // Activates the Wind Power and Wind Rider Abilities.
    bool* wind = nullptr; // optional

    MoveFlags() = default;
    MoveFlags(const MoveFlags&) = default;
};