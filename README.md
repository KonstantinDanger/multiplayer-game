A multiplayer game as a university project. 

Tech Stack: Unity, C#, .NET, Steam (for launching and testing), FizzySteamworks (library for steam and unity), Mirror (library for multiplayer).
Genre: Competitive 1v1 first person fighting with rouge-lite elements

Game cycle: 
Two players arrive at the same map. They both start with few abilities, predefined by their own character class. Players gain xp by eliminating AI foes to raise the level. Each new unlocked level proposes 1 out of 3 upgrades for specific class abilities OR generic stats (hp, cooldowns, movement speed etc). After a while the zone starts shrinking, causing players to move close to each other. When the zone’s fully (or almost) minimized the deathmatch starts. Dying while deathmatch is active causes defeat to the player who fell down. Winning in the whole match gives more currency to the winner. Currency is intended for purchasing or upgrading class abilities (or another things in the future). Progress gained while playing a match session is nullified after it is over except for currency and class abilities bought before the match. If both players have fallen during a deathmatch it counts as a draw. If one of the players disconnects while the session is going on:
  * A match is considered as a canceled if session time < half of session time
  * A match is considered as a defeat to that one who leaves, and the victory to a remaining one if session time > half of session time.
Destroying an opponent before the deathmatch leads to respawn of the fallen one after some time.
