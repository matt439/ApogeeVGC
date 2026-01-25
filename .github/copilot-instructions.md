# Copilot Instructions

## General Guidelines
- First general instruction
- Second general instruction

## Code Style
- Use specific formatting rules
- Follow naming conventions

## Project-Specific Rules
- In the ApogeeVGC codebase:
  - `VoidReturn` in C# is equivalent to a TypeScript method that does not return anything (implicit void).
  - `Undefined` in C# corresponds to the explicit undefined sentinel value in TypeScript, meaning "this action should be ignored".
  - These have different meanings:
    - `VoidReturn` = no return needed
    - `Undefined` = explicit "ignore this action" signal.
  - `Empty` = `NOT_FAIL ("")` = "move worked but produced no notable effect".