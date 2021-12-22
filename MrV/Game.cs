using System;
using System.Collections.Generic;

namespace MrV {
	public class Game {
		public enum GameStatus { None, Running, Ended }
		public GameStatus status;
		AppInput appInput;
		Map2d screen, backbuffer;
		Map2d map;
		List<IDrawable> drawList = new List<IDrawable>();
		List<IUpdatable> updateList = new List<IUpdatable>();
		List<ConsoleKeyInfo> inputQueue = new List<ConsoleKeyInfo>();
		Coord mapOffset = Coord.Zero;
		EntityMobileObject player;

		public void Init() {
			InitScreen();
			InitInput();
			InitMaze();
			InitEntities();
			status = GameStatus.Running;
		}
		void InitScreen() {
			Coord screenSize = new Coord(60, 20);
			screen = new Map2d(screenSize, '\0');
			backbuffer = new Map2d(screenSize, '\0');
		}
		void InitInput() {
			appInput = new AppInput();
			void MapScroll(Coord dir) { mapOffset -= dir; }
			void PlayerMove(Coord dir) {
				const int scrollBorderBuffer = 2;
				player.SetVelocity(dir);
				Coord p = player.position + mapOffset;
				if (p.col < scrollBorderBuffer) { MapScroll(Coord.Left); }
				if (p.row < scrollBorderBuffer) { MapScroll(Coord.Up); }
				if (p.col >= screen.GetSize().col - scrollBorderBuffer) { MapScroll(Coord.Right); }
				if (p.row >= screen.GetSize().row - scrollBorderBuffer) { MapScroll(Coord.Down); }
			}
			int controlSchemeIndex = 0;
			InputMap[] controlSchema = new InputMap[] {
			new InputMap(
				new KBind(ConsoleKey.W, () => PlayerMove(Coord.Up), "move player up"),
				new KBind(ConsoleKey.A, () => PlayerMove(Coord.Left), "move player left"),
				new KBind(ConsoleKey.S, () => PlayerMove(Coord.Down), "move player down"),
				new KBind(ConsoleKey.D, () => PlayerMove(Coord.Right), "move player right"),
				new KBind(ConsoleKey.UpArrow, () => PlayerMove(Coord.Up), "move player up"),
				new KBind(ConsoleKey.LeftArrow, () => PlayerMove(Coord.Left), "move player left"),
				new KBind(ConsoleKey.DownArrow, () => PlayerMove(Coord.Down), "move player down"),
				new KBind(ConsoleKey.RightArrow, () => PlayerMove(Coord.Right), "move player right")),
			new InputMap(
				new KBind(ConsoleKey.UpArrow, () => MapScroll(Coord.Up), "pan map up"),
				new KBind(ConsoleKey.LeftArrow, () => MapScroll(Coord.Left), "pan map left"),
				new KBind(ConsoleKey.DownArrow, () => MapScroll(Coord.Down), "pan map down"),
				new KBind(ConsoleKey.RightArrow, () => MapScroll(Coord.Right), "pan map right"))
			};
			InputMap system = new InputMap(
				new KBind(ConsoleKey.H, PrintKeyBindings, "show key bindings"),
				new KBind(ConsoleKey.M, () => {
					appInput.DisableInputMap(controlSchema[controlSchemeIndex]);
					++controlSchemeIndex; controlSchemeIndex %= controlSchema.Length;
					appInput.EnableInputMap(controlSchema[controlSchemeIndex]);
				}, "switch input mode"));
			appInput.EnableInputMap(system);
			appInput.EnableInputMap(controlSchema[controlSchemeIndex]);
		}
		void InitMaze() {
			string mazeFile = "maze.txt";
			MazeGeneration.MazeGen.WriteMaze(100, 51, 1, 1, 123, mazeFile);
			map = Map2d.LoadFromFile(mazeFile);
			drawList.Add(map);
		}
		void InitEntities() {
			player = new EntityMobileObject("player", new ConsoleTile('@', ConsoleColor.Green), new Coord(1, 1));
			player.onUpdate += () => {
				UpdateMob(player);
				if (map[player.position] == ' ') {
					map[player.position] = '.';
				}
			};
			drawList.Add(player);
			updateList.Add(player);
		}

		void UpdateMob(EntityMobileObject mob) {
			if (!mob.position.IsWithin(map.GetSize()) || map[mob.position].letter == '#') {
				mob.position = mob.lastValidPosition;
			} else {
				mob.lastValidPosition = mob.position;
			}
			mob.SetVelocity(Coord.Zero);
		}
		void PrintKeyBindings() {
			const int infoWindowScreenBuffer = 2;
			const int infoWindowScreenWidth = 40;
			Coord screenSize = screen.GetSize();
			screen.Fill(new ConsoleTile('.', ConsoleColor.Black, ConsoleColor.DarkGray), 
				new Rect(infoWindowScreenBuffer, infoWindowScreenBuffer, infoWindowScreenWidth, screenSize.row - infoWindowScreenBuffer*2));
			screen.Render(Coord.Zero, backbuffer);
			var binds = appInput.currentKeyBinds.keyBinds;
			Console.BackgroundColor = ConsoleColor.DarkGray;
			int kbindIndex = 0;
			foreach (var kbindEntry in binds) {
				List<KBind> kbinds = kbindEntry.Value;
				for (int i = 0; i < kbinds.Count; ++i) {
					KBind kbind = kbinds[i];
					Console.SetCursorPosition(3, 3 + kbindIndex);
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write(kbind.key);
					Console.ForegroundColor = ConsoleColor.Gray;
					Console.Write(" ");
					Console.Write(kbind.description);
					++kbindIndex;
					if (kbindIndex >= screenSize.row - 5) break;
				}
				if (kbindIndex >= screenSize.row - 5) break;
			}
			ConsoleTile.DefaultTile.ApplyColor();
			Console.ReadKey();
			screen.Fill(ConsoleTile.DefaultTile);
			backbuffer.Fill(ConsoleTile.DefaultTile);
		}
		public void Draw() {
			screen.Fill(ConsoleTile.DefaultTile);
			ConsoleTile[,] drawBuffer = screen.GetRawMap();
			for (int i = 0; i < drawList.Count; ++i) {
				drawList[i].Draw(drawBuffer, mapOffset);
			}
			screen.Render(Coord.Zero, backbuffer);
			Map2d temp = screen; screen = backbuffer; backbuffer = temp;
			ConsoleTile.DefaultTile.ApplyColor();
			Console.SetCursorPosition(0, screen.Height);
		}
		public void Input() {
			ConsoleKeyInfo input;
			while (Console.KeyAvailable) {
				input = Console.ReadKey();
				Console.Write("\b "); // backspace and overwrite typed character
				inputQueue.Add(input);
				if (input.Key == ConsoleKey.Escape) { status = GameStatus.Ended; return; }
			}
		}
		public void Update() {
			ServiceInputQueue();
			for (int i = 0; i < updateList.Count; ++i) {
				updateList[i].Update();
			}
		}
		public void ServiceInputQueue() {
			do {
				if (inputQueue.Count > 0) {
					ConsoleKeyInfo input = inputQueue[0];
					inputQueue.RemoveAt(0);
					if (!appInput.DoKeyPress(input)) {
						// Console.WriteLine(input.Key); // uncomment to show unexpected key presses
					}
				}
			} while (inputQueue.Count > 0);
		}
		public void Release() {
		}
	}
}
