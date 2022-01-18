using System;
using System.Collections.Generic;

namespace Game1_1 {
	class Program {
		public struct Coord {
			public short x, y;
			public Coord(int x, int y) { this.x = (short)x; this.y = (short)y; }
			public void SetCursorPosition() { Console.SetCursorPosition(x, y); }
			public bool IsInside(Coord limit) { return x < limit.x && y < limit.y && x >= 0 && y >= 0; }
			public static Coord operator+(Coord a, Coord b) { return new Coord(a.x + b.x, a.y + b.y); }
			public readonly static Coord 
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
		class Game {
			char[,] map;
			Coord mapSize;
			Entity player, mrv;
			List<Entity> entities;
			public bool running;
			ConsoleKeyInfo userInput;
			public void Init() {
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
				mapSize = new Coord(mapWidth, mapStr.Length / mapWidth);
				map = new char[mapSize.y, mapSize.x];
				int strIndex = 0;
				for (int row = 0; row < map.GetLength(0); ++row) {
					for (int col = 0; col < map.GetLength(1); ++col) {
						map[row, col] = mapStr[strIndex++];
						if (mapStr[strIndex] == '\n') { strIndex++; }
					}
				}
				player = new Entity(new Coord(3, 4), '@', ConsoleColor.Green);
				mrv = new Entity(new Coord(20, 10), 'V', ConsoleColor.Cyan);
				entities = new List<Entity>() { player, mrv };
				running = true;
			}
			public void Draw() {
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
			}
			public void GetUserInput() {
				userInput = Console.ReadKey();
				
				switch (userInput.KeyChar) {
					case 'w': player.direction = Coord.Up; break;
					case 'a': player.direction = Coord.Left; break;
					case 's': player.direction = Coord.Down; break;
					case 'd': player.direction = Coord.Right; break;
					case 'q': case (char)27: running = false; break;
				}
			}
			public void Update() {
				mrv.direction = Coord.Directions[System.Environment.TickCount % Coord.Directions.Length];
				for (int i = 0; i < entities.Count; ++i) {
					Coord prev = entities[i].position;
					entities[i].Move();
					if (!entities[i].position.IsInside(mapSize)) {
						entities[i].position = prev;
					}
				}
			}
		}
		public static void Main(string[] args) {
			Game g = new Game();
			g.Init();
			while (g.running) {
				g.Draw();
				g.GetUserInput();
				g.Update();
			}
		}
	}
}
