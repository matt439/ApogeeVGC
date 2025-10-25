//PS C:\VSProjects\pokemon-showdown> node dist/sim/examples/battle-stream-example
//p1 is RandomPlayerAI
//p2 is RandomPlayerAI
//|t:| 1761201157
//| gametype | singles
//| player | p1 | Bot 1 ||
//| player | p2 | Bot 2 ||
//| gen | 7
//| tier | [Gen 7] Custom Game
//|clearpoke
//|poke|p1|Trevenant, L91, F|item
//|poke|p1|Lugia, L71|item
//|poke|p1|Blissey, L85, F|item
//|poke|p1|Mightyena, L94, F|item
//|poke|p1|Registeel, L83|item
//|poke|p1|Skuntank, L87, M|item
//|poke|p2|Jellicent, L88, M|item
//|poke|p2|Gumshoos, L94, F|item
//|poke|p2|Vikavolt, L86, F|item
//|poke|p2|Chandelure, L84, F|item
//|poke|p2|Entei, L78|item
//|poke|p2|Shiftry, L90, M|item
//|teampreview
//|
//|t:| 1761201158
//| teamsize | p1 | 6
//| teamsize | p2 | 6
//| start
//|switch| p1a: Trevenant | Trevenant, L91, F|303/303
//|switch|p2a: Jellicent | Jellicent, L88, M|319/319
//|turn|1
//|
//|t:| 1761201158
//| move | p2a: Jellicent | Shadow Ball | p1a: Trevenant
//| -supereffective | p1a: Trevenant
//| -damage | p1a: Trevenant | 145 / 303
//| -enditem | p1a: Trevenant | Sitrus Berry | [eat]
//| -heal | p1a: Trevenant | 220 / 303 | [from] item: Sitrus Berry
//| move | p1a: Trevenant | Earthquake | p2a: Jellicent
//| -damage | p2a: Jellicent | 221 / 319
//|
//| -heal | p2a: Jellicent | 240 / 319 | [from] item: Leftovers
//| upkeep
//| turn | 2
//|
//| t:| 1761201158
//| move | p2a: Jellicent | Taunt | p1a: Trevenant
//| -start | p1a: Trevenant | move: Taunt
//| move | p1a: Trevenant | Earthquake | p2a: Jellicent
//| -damage | p2a: Jellicent | 137 / 319
//|
//| -heal | p2a: Jellicent | 156 / 319 | [from] item: Leftovers
//| -item | p1a: Trevenant | Sitrus Berry | [from] ability: Harvest
//| upkeep
//| turn | 3
//|
//| t:| 1761201158
//| move | p2a: Jellicent | Recover | p2a: Jellicent
//| -heal | p2a: Jellicent | 316 / 319
//| move | p1a: Trevenant | Horn Leech | p2a: Jellicent
//| -supereffective | p2a: Jellicent
//| -damage | p2a: Jellicent | 80 / 319
//| -heal | p1a: Trevenant | 303 / 303 | [from] drain | [of] p2a: Jellicent
//|
//| -heal | p2a: Jellicent | 99 / 319 | [from] item: Leftovers
//| upkeep
//| turn | 4
//|
//| t:| 1761201158
//| move | p2a: Jellicent | Taunt || [still]
//| -fail | p2a: Jellicent
//| debug | move failed because it did nothing
//|move|p1a: Trevenant | Earthquake | p2a: Jellicent
//| -damage | p2a: Jellicent | 0 fnt
//| faint | p2a: Jellicent
//|
//| -end | p1a: Trevenant | move: Taunt
//| upkeep
//|
//| t:| 1761201158
//|switch| p2a: Chandelure | Chandelure, L84, F|238/238
//|turn|5
//|
//|t:| 1761201158
//| move | p2a: Chandelure | Calm Mind | p2a: Chandelure
//| -boost | p2a: Chandelure | spa | 1
//| -boost | p2a: Chandelure | spd | 1
//| move | p1a: Trevenant | Earthquake | p2a: Chandelure
//| -supereffective | p2a: Chandelure
//| -damage | p2a: Chandelure | 46 / 238
//|
//| -heal | p2a: Chandelure | 60 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 6
//|
//| t:| 1761201158
//| move | p1a: Trevenant | Protect | p1a: Trevenant
//| -singleturn | p1a: Trevenant | Protect
//| move | p2a: Chandelure | Shadow Ball | p1a: Trevenant
//| -activate | p1a: Trevenant | move: Protect
//|
//| -heal | p2a: Chandelure | 74 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 7
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Fire Blast | p1a: Trevenant
//| -supereffective | p1a: Trevenant
//| -damage | p1a: Trevenant | 0 fnt
//| faint | p1a: Trevenant
//|
//| -heal | p2a: Chandelure | 88 / 238 | [from] item: Leftovers
//| upkeep
//|
//| t:| 1761201158
//|switch| p1a: Registeel | Registeel, L83 | 268 / 268
//| turn | 8
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Calm Mind | p2a: Chandelure
//| -boost | p2a: Chandelure | spa | 1
//| -boost | p2a: Chandelure | spd | 1
//| move | p1a: Registeel | Sleep Talk || [still]
//| -fail | p1a: Registeel
//|
//| -heal | p2a: Chandelure | 102 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 9
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Substitute | p2a: Chandelure
//| -start | p2a: Chandelure | Substitute
//| -damage | p2a: Chandelure | 43 / 238
//| move | p1a: Registeel | Rest | p1a: Registeel
//| -fail | p1a: Registeel | heal
//|
//| -heal | p2a: Chandelure | 57 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 10
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Substitute | p2a: Chandelure
//| -fail | p2a: Chandelure | move: Substitute
//| move | p1a: Registeel | Seismic Toss | p2a: Chandelure
//| -immune | p2a: Chandelure
//|
//| -heal | p2a: Chandelure | 71 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 11
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Shadow Ball | p1a: Registeel
//| -damage | p1a: Registeel | 100 / 268
//| move | p1a: Registeel | Toxic || [still]
//| -fail | p1a: Registeel
//| debug | move failed because it did nothing
//|
//|-heal|p2a: Chandelure | 85 / 238 | [from] item: Leftovers
//| -heal | p1a: Registeel | 116 / 268 | [from] item: Leftovers
//| upkeep
//| turn | 12
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Substitute | p2a: Chandelure
//| -fail | p2a: Chandelure | move: Substitute
//| move | p1a: Registeel | Sleep Talk || [still]
//| -fail | p1a: Registeel
//|
//| -heal | p2a: Chandelure | 99 / 238 | [from] item: Leftovers
//| -heal | p1a: Registeel | 132 / 268 | [from] item: Leftovers
//| upkeep
//| turn | 13
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Shadow Ball | p1a: Registeel
//| -damage | p1a: Registeel | 0 fnt
//| faint | p1a: Registeel
//|
//| -heal | p2a: Chandelure | 113 / 238 | [from] item: Leftovers
//| upkeep
//|
//| t:| 1761201158
//|switch| p1a: Blissey | Blissey, L85, F|572/572
//|turn|14
//|
//|t:| 1761201158
//| move | p2a: Chandelure | Calm Mind | p2a: Chandelure
//| -boost | p2a: Chandelure | spa | 1
//| -boost | p2a: Chandelure | spd | 1
//| move | p1a: Blissey | Wish | p1a: Blissey
//|
//| -heal | p2a: Chandelure | 127 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 15
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Calm Mind | p2a: Chandelure
//| -boost | p2a: Chandelure | spa | 1
//| -boost | p2a: Chandelure | spd | 1
//| move | p1a: Blissey | Wish || [still]
//| -fail | p1a: Blissey
//| debug | move failed because it did nothing
//|
//|-heal|p2a: Chandelure | 141 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 16
//|
//| t:| 1761201158
//| move | p1a: Blissey | Protect | p1a: Blissey
//| -singleturn | p1a: Blissey | Protect
//| move | p2a: Chandelure | Fire Blast | p1a: Blissey
//| -activate | p1a: Blissey | move: Protect
//|
//| -heal | p2a: Chandelure | 155 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 17
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Fire Blast | p1a: Blissey
//| -damage | p1a: Blissey | 262 / 572
//| move | p1a: Blissey | Wish | p1a: Blissey
//|
//| -heal | p2a: Chandelure | 169 / 238 | [from] item: Leftovers
//| -heal | p1a: Blissey | 297 / 572 | [from] item: Leftovers
//| upkeep
//| turn | 18
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Substitute | p2a: Chandelure
//| -fail | p2a: Chandelure | move: Substitute
//| move | p1a: Blissey | Wish || [still]
//| -fail | p1a: Blissey
//| debug | move failed because it did nothing
//|
//|-heal|p1a: Blissey | 572 / 572 | [from] move: Wish | [wisher] Blissey
//| -heal | p2a: Chandelure | 183 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 19
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Substitute | p2a: Chandelure
//| -fail | p2a: Chandelure | move: Substitute
//| move | p1a: Blissey | Seismic Toss | p2a: Chandelure
//| -immune | p2a: Chandelure
//|
//| -heal | p2a: Chandelure | 197 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 20
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Substitute | p2a: Chandelure
//| -fail | p2a: Chandelure | move: Substitute
//| move | p1a: Blissey | Wish | p1a: Blissey
//|
//| -heal | p2a: Chandelure | 211 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 21
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Shadow Ball | p1a: Blissey
//| -immune | p1a: Blissey
//| move | p1a: Blissey | Toxic || [still]
//| -fail | p1a: Blissey
//| debug | move failed because it did nothing
//|
//|-heal|p2a: Chandelure | 225 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 22
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Calm Mind | p2a: Chandelure
//| -boost | p2a: Chandelure | spa | 1
//| -boost | p2a: Chandelure | spd | 1
//| move | p1a: Blissey | Wish | p1a: Blissey
//|
//| -heal | p2a: Chandelure | 238 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 23
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Shadow Ball | p1a: Blissey
//| -immune | p1a: Blissey
//| move | p1a: Blissey | Seismic Toss | p2a: Chandelure
//| -immune | p2a: Chandelure
//|
//| upkeep
//| turn | 24
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Substitute | p2a: Chandelure
//| -fail | p2a: Chandelure | move: Substitute
//| move | p1a: Blissey | Toxic || [still]
//| -fail | p1a: Blissey
//| debug | move failed because it did nothing
//|
//|upkeep
//|turn|25
//|
//|t:| 1761201158
//| move | p2a: Chandelure | Fire Blast | p1a: Blissey
//| -damage | p1a: Blissey | 209 / 572
//| move | p1a: Blissey | Seismic Toss | p2a: Chandelure
//| -immune | p2a: Chandelure
//|
//| -heal | p1a: Blissey | 244 / 572 | [from] item: Leftovers
//| upkeep
//| turn | 26
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Shadow Ball | p1a: Blissey
//| -immune | p1a: Blissey
//| move | p1a: Blissey | Seismic Toss | p2a: Chandelure
//| -immune | p2a: Chandelure
//|
//| -heal | p1a: Blissey | 279 / 572 | [from] item: Leftovers
//| upkeep
//| turn | 27
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Calm Mind | p2a: Chandelure
//| -boost | p2a: Chandelure | spa | 1
//| -boost | p2a: Chandelure | spd | 1
//| move | p1a: Blissey | Seismic Toss | p2a: Chandelure
//| -immune | p2a: Chandelure
//|
//| -heal | p1a: Blissey | 314 / 572 | [from] item: Leftovers
//| upkeep
//| turn | 28
//|
//| t:| 1761201158
//| move | p2a: Chandelure | Fire Blast | p1a: Blissey
//| -damage | p1a: Blissey | 0 fnt
//| debug | move failed because it did nothing
//|faint|p1a: Blissey
//|
//| upkeep
//|
//| t:| 1761201158
//|switch| p1a: Lugia | Lugia, L71 | 268 / 268
//| turn | 29
//|
//| t:| 1761201158
//| move | p1a: Lugia | Aeroblast | p2a: Chandelure
//| -activate | p2a: Chandelure | move: Substitute | [damage]
//| move | p2a: Chandelure | Substitute | p2a: Chandelure
//| -fail | p2a: Chandelure | move: Substitute
//|
//| upkeep
//| turn | 30
//|
//| t:| 1761201158
//| move | p1a: Lugia | Toxic || [still]
//| -fail | p1a: Lugia
//| debug | move failed because it did nothing
//|move|p2a: Chandelure | Fire Blast | p1a: Lugia | [miss]
//| -miss | p2a: Chandelure | p1a: Lugia
//|
//| upkeep
//| turn | 31
//|
//| t:| 1761201158
//| move | p1a: Lugia | Roost || [still]
//| -fail | p1a: Lugia | heal
//| move | p2a: Chandelure | Calm Mind | p2a: Chandelure
//| -boost | p2a: Chandelure | spa | 0
//| -boost | p2a: Chandelure | spd | 0
//| debug | move failed because it did nothing
//|
//|upkeep
//|turn|32
//|
//|t:| 1761201158
//| move | p1a: Lugia | Toxic || [still]
//| -fail | p1a: Lugia
//| debug | move failed because it did nothing
//|move|p2a: Chandelure | Fire Blast | p1a: Lugia
//| debug | Multiscale weaken
//| -damage | p1a: Lugia | 13 / 268
//| -status | p1a: Lugia | brn
//|
//| -heal | p1a: Lugia | 29 / 268 brn | [from] item: Leftovers
//| -damage | p1a: Lugia | 13 / 268 brn | [from] brn
//| upkeep
//| turn | 33
//|
//| t:| 1761201158
//| move | p1a: Lugia | Roost | p1a: Lugia
//| -heal | p1a: Lugia | 147 / 268 brn
//| -singleturn | p1a: Lugia | move: Roost
//| move | p2a: Chandelure | Fire Blast | p1a: Lugia
//| -damage | p1a: Lugia | 0 fnt
//| faint | p1a: Lugia
//|
//| upkeep
//|
//| t:| 1761201158
//|switch| p1a: Mightyena | Mightyena, L94, F|284/284
//|-ability|p1a: Mightyena | Intimidate | boost
//| -immune | p2a: Chandelure
//| turn | 34
//|
//| t:| 1761201158
//| move | p1a: Mightyena | Play Rough | p2a: Chandelure
//| -resisted | p2a: Chandelure
//| -end | p2a: Chandelure | Substitute
//| move | p2a: Chandelure | Calm Mind | p2a: Chandelure
//| -boost | p2a: Chandelure | spa | 0
//| -boost | p2a: Chandelure | spd | 0
//| debug | move failed because it did nothing
//|
//|upkeep
//|turn|35
//|
//|t:| 1761201158
//| move | p1a: Mightyena | Play Rough | p2a: Chandelure
//| -resisted | p2a: Chandelure
//| -damage | p2a: Chandelure | 186 / 238
//| move | p2a: Chandelure | Substitute | p2a: Chandelure
//| -start | p2a: Chandelure | Substitute
//| -damage | p2a: Chandelure | 127 / 238
//|
//| -heal | p2a: Chandelure | 141 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 36
//|
//| t:| 1761201158
//| move | p1a: Mightyena | Play Rough | p2a: Chandelure
//| -resisted | p2a: Chandelure
//| -activate | p2a: Chandelure | move: Substitute | [damage]
//| move | p2a: Chandelure | Calm Mind | p2a: Chandelure
//| -boost | p2a: Chandelure | spa | 0
//| -boost | p2a: Chandelure | spd | 0
//| debug | move failed because it did nothing
//|
//|-heal|p2a: Chandelure | 155 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 37
//|
//| t:| 1761201158
//| move | p1a: Mightyena | Play Rough | p2a: Chandelure
//| -resisted | p2a: Chandelure
//| -end | p2a: Chandelure | Substitute
//| move | p2a: Chandelure | Substitute | p2a: Chandelure
//| -start | p2a: Chandelure | Substitute
//| -damage | p2a: Chandelure | 96 / 238
//|
//| -heal | p2a: Chandelure | 110 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 38
//|
//| t:| 1761201158
//| move | p1a: Mightyena | Play Rough | p2a: Chandelure
//| -resisted | p2a: Chandelure
//| -crit | p2a: Chandelure
//| -end | p2a: Chandelure | Substitute
//| move | p2a: Chandelure | Substitute | p2a: Chandelure
//| -start | p2a: Chandelure | Substitute
//| -damage | p2a: Chandelure | 51 / 238
//|
//| -heal | p2a: Chandelure | 65 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 39
//|
//| t:| 1761201158
//| move | p1a: Mightyena | Play Rough | p2a: Chandelure
//| -resisted | p2a: Chandelure
//| -crit | p2a: Chandelure
//| -end | p2a: Chandelure | Substitute
//| move | p2a: Chandelure | Substitute | p2a: Chandelure
//| -start | p2a: Chandelure | Substitute
//| -damage | p2a: Chandelure | 6 / 238
//|
//| -heal | p2a: Chandelure | 20 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 40
//|
//| t:| 1761201158
//| move | p1a: Mightyena | Play Rough | p2a: Chandelure
//| -resisted | p2a: Chandelure
//| -end | p2a: Chandelure | Substitute
//| move | p2a: Chandelure | Calm Mind | p2a: Chandelure
//| -boost | p2a: Chandelure | spa | 0
//| -boost | p2a: Chandelure | spd | 0
//| debug | move failed because it did nothing
//|
//|-heal|p2a: Chandelure | 34 / 238 | [from] item: Leftovers
//| upkeep
//| turn | 41
//|
//| t:| 1761201158
//| move | p1a: Mightyena | Play Rough | p2a: Chandelure
//| -resisted | p2a: Chandelure
//| -damage | p2a: Chandelure | 0 fnt
//| faint | p2a: Chandelure
//|
//| upkeep
//|
//| t:| 1761201158
//|switch| p2a: Gumshoos | Gumshoos, L94, F|318/318
//|turn|42
//|
//|t:| 1761201158
//| move | p1a: Mightyena | Play Rough | p2a: Gumshoos
//| -damage | p2a: Gumshoos | 175 / 318
//| move | p2a: Gumshoos | Crunch | p1a: Mightyena
//| -resisted | p1a: Mightyena
//| -damage | p1a: Mightyena | 227 / 284
//|
//| upkeep
//| turn | 43
//|
//| t:| 1761201158
//| move | p1a: Mightyena | Play Rough | p2a: Gumshoos
//| -damage | p2a: Gumshoos | 51 / 318
//| move | p2a: Gumshoos | Crunch | p1a: Mightyena
//| -resisted | p1a: Mightyena
//| -damage | p1a: Mightyena | 168 / 284
//|
//| upkeep
//| turn | 44
//|
//| t:| 1761201158
//| move | p1a: Mightyena | Play Rough | p2a: Gumshoos
//| -damage | p2a: Gumshoos | 0 fnt
//| faint | p2a: Gumshoos
//|
//| upkeep
//|
//| t:| 1761201158
//|switch| p2a: Entei | Entei, L78 | 307 / 307
//| turn | 45
//|
//| t:| 1761201158
//| move | p2a: Entei | Sacred Fire | p1a: Mightyena
//| -damage | p1a: Mightyena | 12 / 284
//| move | p1a: Mightyena | Play Rough | p2a: Entei
//| -resisted | p2a: Entei
//| -damage | p2a: Entei | 249 / 307
//|
//| upkeep
//| turn | 46
//|
//| t:| 1761201158
//| move | p2a: Entei | Sacred Fire | p1a: Mightyena | [miss]
//| -miss | p2a: Entei | p1a: Mightyena
//| move | p1a: Mightyena | Play Rough | p2a: Entei
//| -resisted | p2a: Entei
//| -damage | p2a: Entei | 190 / 307
//|
//| upkeep
//| turn | 47
//|
//| t:| 1761201158
//| move | p2a: Entei | Sacred Fire | p1a: Mightyena
//| -damage | p1a: Mightyena | 0 fnt
//| debug | move failed because it did nothing
//|faint|p1a: Mightyena
//|
//| upkeep
//|
//| t:| 1761201158
//|switch| p1a: Skuntank | Skuntank, L87, M|321/321
//|turn|48
//|
//|t:| 1761201158
//| move | p2a: Entei | Sacred Fire | p1a: Skuntank
//| -damage | p1a: Skuntank | 122 / 321
//| move | p1a: Skuntank | Crunch | p2a: Entei
//| -damage | p2a: Entei | 79 / 307
//|
//| upkeep
//| turn | 49
//|
//| t:| 1761201158
//| move | p1a: Skuntank | Sucker Punch | p2a: Entei
//| -damage | p2a: Entei | 0 fnt
//| faint | p2a: Entei
//|
//| upkeep
//|
//| t:| 1761201158
//|switch| p2a: Vikavolt | Vikavolt, L86, F|273/273
//|turn|50
//|
//|t:| 1761201158
//| move | p1a: Skuntank | Poison Jab | p2a: Vikavolt
//| -damage | p2a: Vikavolt | 195 / 273
//| move | p2a: Vikavolt | Energy Ball | p1a: Skuntank
//| -resisted | p1a: Skuntank
//| -damage | p1a: Skuntank | 69 / 321
//|
//| -heal | p2a: Vikavolt | 212 / 273 | [from] item: Leftovers
//| upkeep
//| turn | 51
//|
//| t:| 1761201158
//| debug | before turn callback: pursuit
//| move | p1a: Skuntank | Pursuit | p2a: Vikavolt
//| -damage | p2a: Vikavolt | 166 / 273
//| move | p2a: Vikavolt | Bug Buzz | p1a: Skuntank
//| -damage | p1a: Skuntank | 0 fnt
//| faint | p1a: Skuntank
//|
//| win | Bot 2