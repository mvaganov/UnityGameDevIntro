# MrV's Game Development Class

## Game0 - procedural basics

* demonstrate: basic game code
	- ``Console.Write``
	- ``Console.SetCursorPosition``
	- ``Console.ReadKey`` which returns ``ConsoleKeyInfo``

* lecture: parts of a game engine
	- initialization
	- game loop
		- draw
		- user input
		- update

_ASSIGN: embelish with if statements for boundary conditions_

_ASSIGN: win condition, when the player reaches some specific tile_

## Game1 - OOP

* demonstrate: changing which project is used as Main by editing &lt;StartupObject&gt; in the csproj file.

* lecture: Coord class
	- overloaded operator
	- static array
	- readonly vs const

* lecture: Entity class
	- Coord as a direction instead of a position
	- ConsoleColor

* lecture: C#'s alternate 2d array notation ``[,]``

* demonstrate: display list using C#'s List class, which uses generics. analagous to C++'s vector class, or Java's ArrayList.

* demonstrate: show how to detect what character the player is standing on

_ASSIGN: implement collision detection against '#' walls in the 2d array._

_ASSIGN: implement win condition of player meeting the NPC_

_ASSIGN: (bonus) implement another NPC with a lose condition if the player meets them._

_ASSIGN: (bonus) implement an item that the player can defeat an NPC with if they get it first._

_ASSIGN: (bonus) implement collision detection against multiple NPCs, so they can't share the same space._

_ASSIGN: (EXTRA BONUS) load a map from a file instead of the string_

_ASSIGN: (SUPER BONUS) create 'doors' on the map that can be opened with collected keys_

_ASSIGN: (MEGA BONUS) create logic that allows the map to change if the player reaches a specific tile or Entity_

## Game1_1 - realtime

* demonstrate: ``System.Environment.TickCount``

* demonstrate: making the game realtime by checking if input exists
	- ``if (Console.KeyAvailable)``

* demonstrate: ``System.Threading.Thread.Sleep``, compare against a loop watching the time ``System.Environment.TickCount``

* demonstrate: implementing a timer in general
	- show a message after a duration
	- prevent player from moving till a time stamp is reached

_ASSIGN: make the game real-time and give the NPC a delay between moves_

_ASSIGN: reimplement the win condition, this time with a timer that gives success if the player reaches the goal in 1 minute, and a non-victory message if the goal is reached after 1 minute._

## Game2 - engine

* lecture: walk through structure basics
	- explain the double-buffering mechanism for draw performance

* demonstrate: message box on a timer

* demonstrate: changing the maze

_ASSIGN: change the map size, or load another map of your choice_

_ASSIGN: implement limited 'ammo' for the magic missle_

_ASSIGN: a bomb item that has an effect: walk onto it and after 5 seconds, all of the nearby walls are destroyed._

_ASSIGN: if the player is caught in a bomb explosion, the game should restart._

_ASSIGN: add more moving entities, with delays between moves_

_ASSIGN: make some entities move intentionally toward or away from the player_

_ASSIGN: add bombs that can be picked up and added to a player's inventory, then the player can then place these bombs on the map_

_ASSIGN: create a special item that lets the player shoot fireballs, which fly like magic missiles, and explode like bombs_

_ASSIGN: create a boss monster, who shoots fireballs, which can harm the player_

## getting started with Unity

* lecture: Unity intro
	* UI basics
	* MrV's character controller

* demonstrate: collision detection

_ASSIGN: make a parkour level_

## UnityGame - Game2 in Unity

* lecture: walk through structure, show parallels with command line game

* demonstrate: drawing lines in 3d

_ASSIGN: do the same thing from Game2, except in Unity_
