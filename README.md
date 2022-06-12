# Bitty's Sigils

This mod adds 45 new sigils for mod creators to use. None of these sigils will appear in game except for cards that they are added to. (Will not appear on totems and most other sources.)

Made by Bitty45 (discord: bitty45#5291)

## The following sigils are added:

<details>
<summary>45 New Sigils:
</summary>

|Name|Description|
|:-|:-|
|**Restriction Sigils**|---|
|Repulsion|A card bearing this sigil may not attack.|
|Nonexistent Soul|A card bearing this sigil may not have sigils transferred to, or from this card.|
|Weak Soul|A card bearing this sigil may not have its sigils transferred to another card.|
|Strong Soul|A card bearing this sigil may not recieve sigils from another card.|
|Pyrophobia|A card bearing this sigil may not be buffed at campsites.|
|Pyrophobia (Power)|A card bearing this sigil may not recieve Power buffs from campsites.|
|Pyrophobia (Health)|A card bearing this sigil may not recieve Health buffs from campsites.|
|Foolhardy|A card bearing this sigil may not recieve modifications from any source.|
|Mox Dependant|If a card bearing this sigil's owner controls no Mox cards, a card bearing this sigil perishes.|
|Mox Phobic|If a card bearing this sigil's owner controls a Mox card, a card bearing this sigil perishes.|
|**Strafe Sigils**|---|
|Sticky|At the end of the owner's turn, a card bearing this sigil will move the opposing card and itself in the direction inscribed in the sigil.|
|Hauler|At the end of the owner's turn, a card bearing this sigil will move in the direction inscribed in the sigil. Adjacent friendly creatures will be pulled in the same direction.|
|Jumper|At the end of the owner's turn, a card bearing this sigil will move itself to the first empty space in the direction inscribed in the sigil.|
|Super Sprinter|At the end of the owner's turn, a card bearing this sigil will move itself as far as possible in the direction inscribed in the sigil.|
|Board Shifter|At the end of the owner's turn, a card bearing this sigil will move in the direction inscribed in the sigil. All cards will shift in the same direction, looping to the other edge of the board.|
|Board Shifter (Opponent)|At the end of the owner's turn, a card bearing this sigil will move in the direction inscribed in the sigil. The opponent's cards will shift in the same direction, looping to the other edge of the board.|
|Board Shifter (Player)|At the end of the owner's turn, a card bearing this sigil will move in the direction inscribed in the sigil. Friendly cards will shift in the same direction, looping to the other edge of the board.|
|**Swap Sigils**|---|
|Heart Swap|When a card bearing this sigil is played, all cards on the board swap their power and health.|
|Stubborn|A card bearing this sigil may not have its stats swapped.|
|Flipper|At the end of the owner's turn, a card bearing this sigil swaps its stats.|
|**Hand/Deck Sigils**|---|
|Replenish|When a card bearing this sigil is drawn, draw a card.|
|Fleeting|A card bearing this sigil will be discarded at the end of the turn.|
|Fleeting Draw|When a card bearing this sigil is played, draw 1 card with Fleeting.|
|Outcast|At the end of the turn, a card bearing this sigil is shuffled into the deck.|
|Explosive Squirrel Reproduction|When a card bearing this sigil is played, fill your side deck with squirrels.|
|**Misc Sigils**|---|
|Dusty Quill|Whenever a creature dies while a card bearing this sigil is on the board, a corpse is raised in it's place. Corpses are defined as: 0/1.|
|Wildlife Camera|When a card bearing this sigil kills another card, a copy of the killed card is created in your hand.|
|Bleached Brush|When a card bearing this sigil is played, the opposing card will lose all its sigils.|
|Soul Link|When a card bearing this sigil perishes, all other allied cards bearing this sigil perish as well.|
|Hollow Draw|When a card bearing this sigil is played, discard the oldest card in your hand, draw a card.|
|Hollow Barrage|When a card bearing this sigil is played, all cards take 1 damage.|
|Hollow Repeater|When a card bearing this sigil perishes, all On Play sigils on that card are activated.|
|Champion|A card bearing this sigil will not take damage from other cards except from combat.|
|Clockwise|When a card bearing this sigil is played, all cards are moved clockwise.|
|Twister|At the end of each turn a card bearing this sigil is on the board, move all cards on the board clockwise.|
|Counter Attack|Once a card bearing this sigil is struck, the striker is then dealt damage equal to this card's attack.|
|Mirror Counter|Once a card bearing this sigil is struck, the striker is then dealt damage equal to the striker's attack.|
|Deathbell|At the start of each turn, a card bearing this sigil perishes.|
|Mutual Hire|When a card bearing this sigil is played, a copy of it is created in the opposing space.|
|Mysterious Mushrooms|At the end of the turn, if there is a card on either side of a card bearing this sigil, they are fused together.|
|Eggist|When a card bearing this sigil is played, an Egg is created on each empty adjacent space.|
|Fir Caller|When a card bearing this sigil is played, a Fir is created in each of the player's spaces.|
|**Activated Sigils**|---|
|Charged Barrage|Activate: Pay 4 life to increase the power of all cards on your side of the board by 1.|
|Sigil Roll|Activate: Pay 4 Life to add a random sigil to a card bearing this sigil.|
|Health Roll|Activate: Pay 1 Energy to set the health of a card bearing this sigil randomly between 1 and 3.|
|||
</details>

<details>
<summary>5 New Triggers:
</summary>

- IOnStatSwap
- IOnOtherStatSwap
- IOnGemChange
- IOnGemLoss
- IOnGemGain
- IOnThisDiscard
- IOnOtherDiscard
</details>

## Credits

<details>
<summary>Thanks to:
</summary>

- apotheoticDivinity [aD] for art: **Flipper**, **Fleeting Draw**, **Outcast**, **Replenish**
- Magolorgaming for art: **No Campfire (all 3), No Stones, No Modifications**
- Amy for art: **No/Only Transfers** and **Act 2 Sigil Art**
- The Saxby Family for the concepts and art for: **Heart Swap, Squirrel Gift, Mutual Hire, Charged Barrage, Health Rolll, Sigil Roll, Camera, Quill, Brush**
- 284.webp for the **Sticky** sigil art
- Scented Shadow for **Jumper** art and act 2 art
- HeroYoYo for **Eggist** art and act 2 art
</details>

## Requirements

As with most mods, you need [BepInEx](https://inscryption.thunderstore.io/package/BepInEx/BepInExPack_Inscryption/) installed. You also need the [API](https://inscryption.thunderstore.io/package/API_dev/API/).

## Changelog 1.4.0
### New sigils:
- Outcast
- Fleeting
- Fleeting Draw
- Hollow Repeater
- Flipper
- Explosive Squirrel Reproduction
- Replenish
### New Triggers:
- IOnDiscard
- IOnOtherDiscard

<details>
<summary>Changelog</summary>

- 1.4.0: 7 new sigils, discard triggers
- 1.3.0: 7 new sigils
- 1.2.1: Act 2 Art, new gem triggers
- 1.2.0: 9 new sigils, swap triggers
- 1.1.0: 7 new sigils
- 1.0.0: 15 new sigils

</details>