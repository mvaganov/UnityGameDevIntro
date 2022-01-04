# MrV's Game Development Class

## Game0 - procedural basics

* basic game code
	- ``Console.Write``
	- ``Console.SetCursorPosition``
	- ``Console.ReadKey`` which returns ``ConsoleKeyInfo``

_ASSIGN: embelish with if statements for boundary conditions_

_ASSIGN: win condition, when the player reaches some specific tile_

## Game1 - OOP

* Coord class
	- overloaded operator
	- static array
	- readonly vs const

* Entity class
	- Coord as a direction instead of a position
	- ConsoleColor

* C#'s alternate 2d array notation ``[,]``

* C#'s List class, which uses generics. analagous to C++'s vector class.

* the concept of a display list

* show how to detect what character the player is standing on

_ASSIGN: implement win condition of player meeting the NPC_

_ASSIGN: implement collision detection against walls._

_ASSIGN: implement another NPC with a lose condition if the player meets them._

_ASSIGN: implement collision detection against NPCs, so they can't share the same space._

## Game1_1 - realtime

* making the game realtime by checking if input exists

* show ``System.Threading.Thread.Sleep``, compare against a loop watching the time ``System.Environment.TickCount``

* implementing a timer in general
	- show a message after a duration
	- prevent player from moving till a time stamp is reached

_ASSIGN: give the NPC a delay between moves_

_ASSIGN: reimplement the win condition, this time with a timer._

## Game2 - engine

* walk through structure basics
	- explain the double-buffering mechanism for draw performance

* message box on a timer

_ASSIGN: change the map size, or load another map of your choice_

_ASSIGN: add limit to magic missle_

_ASSIGN: a bomb item that has an effect: touch it and after 5 seconds, all of the nearby walls are destroyed._

_ASSIGN: more moving entities_

_ASSIGN: make some entities move intentionally toward or away from the player_

_ASSIGN: add bombs that can be picked up and added to a player's inventory. the player can then place these bombs on the map_

_ASSIGN: create a special item that lets the player shoot fireballs, which fly like magic missiles, and explode like bombs_

_ASSIGN: create a boss monster, who shoots fireballs_

## getting started with Unity

* intro
	* UI basics
	* MrV's character controller

_ASSIGN: make a parkour level_

## UnityGame - Game2 in Unity

* walk through structure, show parallels

* drawing lines in 3d

_ASSIGN: do the same thing from Game2, except in Unity_
