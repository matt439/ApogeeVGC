# Singles Battle Team Preview Clarification

## Question

> Isn't this a singles battle? And `FormatId.CustomSingles`.

## Answer

**Yes, it IS a singles battle**, and your implementation is **CORRECT**!

## Team Preview in Competitive Singles

In competitive Pokémon singles (like Pokemon Showdown), team preview works differently than in the games:

### **How It Works**

1. **You bring 6 Pokémon** to the battle
2. **During team preview**, you see your opponent's 6 Pokémon
3. **You reorder your entire team** (positions 1-6)
4. **The first Pokémon** in your reordered team becomes your **lead**
5. During battle, you can switch to any of your other 5 Pokémon

### **Why Reorder All 6?**

- **Strategic positioning**: Put your lead first, backup plans second, etc.
- **Switch flexibility**: Pre-plan your switch order based on opponent's team
- **Mind games**: Your opponent sees your team but not the order you chose

## Current Format Configuration

```csharp
[FormatId.CustomSingles] = new Format
{
    Name = "Custom Singles",
    Ruleset = [],
    Banlist = [],
    RuleTable = new RuleTable
 {
        PickedTeamSize = 6, // ? CORRECT - Reorder all 6 Pokémon
    },
}
```

**This is the standard competitive singles format.**

## Added: Blind Singles Format

For a simplified singles format **without team preview** (like in the main games):

```csharp
[FormatId.CustomSinglesBlind] = new Format
{
    Name = "Custom Singles (Blind)",
    Ruleset = [],
    Banlist = [],
    RuleTable = new RuleTable
    {
        PickedTeamSize = 0, // No team preview - team order is fixed
    },
}
```

### Using the Blind Format

```csharp
// In Driver.cs
BattleOptions battleOptions = new()
{
    Id = FormatId.CustomSinglesBlind, // ? Use blind format
    Player1Options = player1Options,
    Player2Options = player2Options,
    Debug = true,
};
```

With `PickedTeamSize = 0`:
- No team preview phase
- Your team order is exactly as you submitted it
- First Pokémon in the list is your lead
- Battle starts immediately

## Comparison

| Format | Team Preview | Picked Team Size | Use Case |
|---|---|---|---|
| **CustomSingles** | ? Yes | 6 | Competitive singles (like Showdown) |
| **CustomSinglesBlind** | ? No | 0 | Casual/Story mode style |
| **CustomDoubles** | ? Yes | 4 | Competitive doubles (pick 4 from 6) |

## Keyboard Input Range

Your GUI correctly allows **1-6 input for team preview** because:
- Singles team preview = Pick order of all 6 Pokémon
- Doubles team preview = Pick 4 from 6 Pokémon

The implementation is correct for the format you're using!

## Summary

? **`CustomSingles` with `PickedTeamSize = 6` is correct**  
? **Your GUI accepting 1-6 input is correct**  
? **This is standard competitive singles format**  

If you want a simpler format without team preview, use `CustomSinglesBlind` instead.
