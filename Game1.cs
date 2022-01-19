using System;
using System.Collections.Generic;

namespace Game1 {
	// ASSIGN: implement collision detection against '#' walls in the 2d array.
	// ASSIGN: implement win condition of player meeting the NPC
	// ASSIGN: (bonus) implement another NPC with a lose condition if the player meets them.
	// ASSIGN: (bonus) implement an item that the player can defeat an NPC with if they get it first.
	// ASSIGN: (bonus) implement collision detection against multiple NPCs, so they can't share the same space.
	class Program {
		public struct Coord {
			public short x, y;
			public Coord(int x, int y) { this.x = (short)x; this.y = (short)y; }
			public void SetCursorPosition() { Console.SetCursorPosition(x, y); }
			public bool IsInside(Coord limit) { return x < limit.x && y < limit.y && x >= 0 && y >= 0; }
			public static Coord operator+(Coord a, Coord b) { return new Coord(a.x + b.x, a.y + b.y); }
			public static Coord 
				Zero  = new Coord(0, 0),
				Up    = new Coord(0,-1),
				Left  = new Coord(-1,0),
				Down  = new Coord(0, 1),
				Right = new Coord(1, 0);
			public static Coord[] Directions = new Coord[] { Up, Left, Down, Right };
		}
		public class Entity {
			public Coord position;
			public Coord direction;
			public char icon;
			public ConsoleColor color;
			public Entity(Coord p, char i, ConsoleColor c) { position = p; icon = i; color = c; }
			public void Draw() {
				position.SetCursorPosition();
				ConsoleColor c = Console.ForegroundColor;
				Console.ForegroundColor = color;
				Console.Write(icon);
				Console.ForegroundColor = c;
			}
			public void Move() {
				position += direction;
				direction = Coord.Zero;
			}
		}
		public static void Main(string[] args) {
			string mapStr =
			"..............................\n" +
			"..............................\n" +
			"..............................\n" +
			"..............................\n" +
			".................########.....\n" +
			"................#.............\n" +
			"................#..#####......\n" +
			"................#......###....\n" +
			"................#####....###..\n" +
			"................#..........#..\n" +
			"................#..........#..\n" +
			"................#..........#..\n" +
			"................#..........#..\n" +
			"................############..\n" +
			"..............................\n";
			int mapWidth = mapStr.IndexOf('\n');
			Coord mapSize = new Coord(mapWidth, mapStr.Length / mapWidth);
			char[,] map = new char[mapSize.y, mapSize.x];
			int strIndex = 0;
			for (int row = 0; row < map.GetLength(0); ++row) {
				for (int col = 0; col < map.GetLength(1); ++col) {
					map[row, col] = mapStr[strIndex++];
					if (mapStr[strIndex] == '\n') { strIndex++; }
				}
			}
			Entity player = new Entity(new Coord(3,4), '@', ConsoleColor.Green);
			Entity mrv = new Entity(new Coord(20, 10), 'V', ConsoleColor.Cyan);
			List<Entity> entities = new List<Entity>() { player, mrv };
			bool running = true;
			while (running) {
				for (int row = 0; row < map.GetLength(0); ++row) {
					Console.SetCursorPosition(0, row);
					for (int col = 0; col < map.GetLength(1); ++col) {
						Console.Write(map[row, col]);
					}
				}
				for (int i = 0; i < entities.Count; ++i) {
					entities[i].Draw();
				}
				Console.SetCursorPosition(0, mapSize.y);
				ConsoleKeyInfo userInput = Console.ReadKey();
				switch (userInput.KeyChar) {
					case 'w': player.direction = Coord.Up; break;
					case 'a': player.direction = Coord.Left; break;
					case 's': player.direction = Coord.Down; break;
					case 'd': player.direction = Coord.Right; break;
					case 'q': case (char)27: running = false; break;
				}
				mrv.direction = Coord.Directions[System.Environment.TickCount % Coord.Directions.Length];
				for(int i = 0; i < entities.Count; ++i) {
					Coord prev = entities[i].position;
					entities[i].Move();
					if (!entities[i].position.IsInside(mapSize)) {
						entities[i].position = prev;
					}
				}
			}
		}
	}
}
