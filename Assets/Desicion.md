DECISIONS:  Coin Adventure
Assignment 10:  Three Enemy Types
AI tool used: ChatGPT
Project: Unity 6, 2D mobile platformer
AI Log and My Decisions (Week 10)
1. I asked ChatGPT for help with three different enemies: Goblin (Patroller), Ghost (Shooter), and Orc (Charger). I used separate finite state machines (FSMs) and kept the existing scripts instead of adding unnecessary AI scripts.
2. Goblin: It entered the Chase state but did not attack. I used Console messages and found that IsNearPlayer() was false. I tested the attack range in the Inspector and adjusted the enemy setup so it could reach the player.
3. Orc: It fell below the platform, and the attack animation appeared before the dash. I checked its Rigidbody2D and Collider2D setup and changed the sprite order so the warning comes before the dash attack.
4. Sprites and player health: I used AI assistance to prepare enemy sprite images and connect Idle, Walk, Attack, and Hurt sprites. I also connected the existing PlayerHealth script to three hearts and checked damage and Game Over.
5. Facing and level finish: ChatGPT helped me make enemies face their movement or attack direction and set up the end of Level 1 and Level 2. I applied and tested these changes in Unity.
What AI Got Wrong and What I Changed
Some suggestions used different names for similar scripts and confused the project structure. I decided to keep my original script names (GoblinPatrol, GhostEnemy, OrcEnemy, and PlayerHealth) and avoid duplicate scripts. I also tested attack distances and sprite assignments in the Inspector instead of relying only on suggested code.
Phone Playtest and Tuning — Complete After Testing

AI Use Declaration
I used ChatGPT for code suggestions, debugging, sprite preparation, and explanations. I manually made the changes and tested the game in Unity. I did not use an automated coding agent to edit the Unity project.