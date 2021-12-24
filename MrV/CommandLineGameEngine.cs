using System;
using System.Collections.Generic;
using System.Text;

namespace MrV {
	public abstract class CommandLineGameEngine {
		public enum GameStatus { None, Running, Ended }
		public GameStatus status;
		protected AppInput appInput;
		protected Map2d screen, backbuffer;
		protected Map2d map;
		protected Coord drawOffset = Coord.Zero;
		protected List<IDrawable> drawList = new List<IDrawable>();
		protected List<IUpdatable> updateList = new List<IUpdatable>();
		protected List<ConsoleKeyInfo> inputQueue = new List<ConsoleKeyInfo>();

		public virtual void Init() {
			InitScreen(out Coord size, out char defaultChar);
			InitScreen(size, defaultChar);
			appInput = new AppInput();
			InitInput(appInput);
			InitData();
			status = GameStatus.Running;
		}
		public virtual void Release() {
		}
		public virtual void Draw() {
			screen.Fill(ConsoleTile.DefaultTile);
			ConsoleTile[,] drawBuffer = screen.GetRawMap();
			Render(drawBuffer);
			screen.Render(Coord.Zero, backbuffer);
			Map2d temp = screen; screen = backbuffer; backbuffer = temp;
			ConsoleTile.DefaultTile.ApplyColor();
			Console.SetCursorPosition(0, screen.Height);
		}
		public void KeepPointOnScreen(Coord position) {
			Coord screenMargin = new Coord(3, 2);
			Rect viewArea = new Rect(screenMargin, screen.GetSize() - Coord.One - screenMargin);
			Coord distanceFromScreen = viewArea.GetOutOfBoundsDelta(position + drawOffset);
			drawOffset -= distanceFromScreen;
		}
		protected virtual void Render(ConsoleTile[,] drawBuffer) {
			for (int i = 0; i < drawList.Count; ++i) {
				drawList[i].Draw(drawBuffer, drawOffset);
			}
		}
		protected void InitScreen(Coord size, char initialCharacter) {
			screen = new Map2d(size, initialCharacter);
			backbuffer = new Map2d(size, initialCharacter);
		}
		public void Update() {
			ServiceInputQueue();
			for (int i = 0; i < updateList.Count; ++i) {
				updateList[i].Update();
			}
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
		public string GetKeyText() {
			var binds = appInput.currentKeyBinds.keyBinds;
			StringBuilder sb = new StringBuilder();
			foreach (var kbindEntry in binds) {
				List<KBind> kbinds = kbindEntry.Value;
				for (int i = 0; i < kbinds.Count; ++i) {
					KBind kbind = kbinds[i];
					sb.Append(kbind.key.ToString()).Append(" ").Append(kbind.description).Append("\n");
				}
			}
			return sb.ToString();
		}
		public void MessageBox(string message, ConsoleColor txtColor, Rect area, ConsoleTile back, Coord inset) {
			screen.Fill(back, area);
			screen.Render(Coord.Zero, backbuffer);
			Console.BackgroundColor = back.Back;
			Console.ForegroundColor = txtColor;
			Coord cursor = area.min + inset;
			bool spaceLeftToWrite = true;
			for (int i = 0; spaceLeftToWrite && i < message.Length; ++i) {
				if (cursor.X >= area.max.X - inset.X) {
					for (; i < message.Length && i != '\n'; ++i) ; // no wrap. skip all chars till new line
					if (i > message.Length) { break; }
				}
				char c = message[i];
				switch (c) {
					case '\n':
						cursor.X = area.min.X + inset.X;
						cursor.Y++;
						if (cursor.Y >= area.max.Y - inset.Y) {
							spaceLeftToWrite = false;
						}
						break;
					default:
						Console.SetCursorPosition(cursor.X, cursor.Y);
						Console.Write(c);
						cursor.X++;
						break;
				}
			}
			ConsoleTile.DefaultTile.ApplyColor();
			Console.ReadKey();
			screen.Fill(ConsoleTile.DefaultTile);
			backbuffer.Fill(ConsoleTile.DefaultTile);
		}
		protected abstract void InitInput(AppInput appInput);
		protected abstract void InitScreen(out Coord size, out char defaultCharacter);
		protected abstract void InitData();
	}
}