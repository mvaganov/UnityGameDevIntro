using System;

namespace MrV {
	public class Game : CommandLineGameEngine {
		EntityMobileObject player, mrv;
		EntityBasic goal;
        protected override void InitScreen(out Coord size, out char defaultCharacter) {
			size = new Coord(60, 20);
			defaultCharacter = '\0';
		}
		protected override void InitInput(AppInput appInput) {
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
			int controlSchemeIndex = 0;
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
		void PlayerMove(Coord dir) {
			player.SetVelocity(dir);
			KeepPointOnScreen(player.position + dir);
		}
		void MapScroll(Coord dir) { drawOffset -= dir; }
		protected override void InitData() {
			InitMaze();
			InitEntities();
		}
		void InitMaze() {
			string mazeFile = "maze.txt";
			MrV.MazeGen.WriteMaze(99, 51, 1, 1, 123, mazeFile);
			map = Map2d.LoadFromFile(mazeFile);
			drawList.Add(map);
		}
		void InitEntities() {
			player = new EntityMobileObject("player", new ConsoleTile('@', ConsoleColor.Green), new Coord(1, 1));
			bool wizardGranted = false;
			Coord lastPlayerMove = Coord.Zero;
			player.onUpdate = () => {
				if (player.velocity != Coord.Zero) {
					lastPlayerMove = player.velocity;
				}
				CollisionUpdate(player);
				if (map[player.position] == ' ') {
					map[player.position] = '.';
				}
			};
			mrv = new EntityMobileObject("Mr.V", new ConsoleTile('V', ConsoleColor.Cyan), new Coord(3, 3));
			int nextMove = 0;
			mrv.onUpdate = () => {
				CollisionUpdate(mrv);
				if (mrv.position == player.position && !wizardGranted) {
					SimpleMessageBox("You're a Wizard!\n\nPress space to shoot magic missles!", ConsoleColor.Cyan);
					wizardGranted = true;
					player.icon = new ConsoleTile('W', ConsoleColor.Green, ConsoleColor.DarkGreen);
					appInput.EnableInputMap(new InputMap(new KBind(ConsoleKey.Spacebar, () => {
						ShootMagicMissle(new ConsoleTile('*', ConsoleColor.Red), player.position, lastPlayerMove);
					}, "shoot magic missile")));
				}
				if (Environment.TickCount > nextMove) {
					Coord dir = Coord.CardinalDirections[Environment.TickCount % Coord.CardinalDirections.Length];
					mrv.SetVelocity(dir);
					nextMove = Environment.TickCount + 100;
				}
			};
			goal = new EntityBasic("goal", new ConsoleTile('g', ConsoleColor.Yellow), map.GetSize() - Coord.Two);
			goal.onUpdate = () => {
				if (goal.position == player.position) {
					SimpleMessageBox("You Won!", ConsoleColor.Yellow);
					status = GameStatus.Ended;
				}
			};
			drawList.Add(player);
			drawList.Add(mrv);
			drawList.Add(goal);
			updateList.Add(player);
			updateList.Add(mrv);
			updateList.Add(goal);
		}
		void ShootMagicMissle(ConsoleTile tile, Coord position, Coord direction) {
			EntityMobileObject fireball = new EntityMobileObject("fireball", tile, position);
			int nextMove = 0;
			fireball.onUpdate = () => {
				bool oob = !fireball.position.IsWithin(map.GetSize());
				bool hitWall = !oob && map[fireball.position].letter == '#';
				if (hitWall) {
					map[fireball.position] = ',';
				}
				if (oob || hitWall) {
					drawList.Remove(fireball);
					updateList.Remove(fireball);
				}
				if (Environment.TickCount > nextMove) {
					nextMove = Environment.TickCount + 50;
					fireball.SetVelocity(direction);
				} else {
					fireball.SetVelocity(Coord.Zero);
				}
			};
			drawList.Add(fireball);
			updateList.Add(fireball);
		}
		void CollisionUpdate(EntityMobileObject mob) {
			if (!mob.position.IsWithin(map.GetSize()) || map[mob.position].letter == '#') {
				mob.position = mob.lastValidPosition;
			} else {
				mob.lastValidPosition = mob.position;
			}
			mob.SetVelocity(Coord.Zero);
		}
		void PrintKeyBindings() {
			SimpleMessageBox(GetKeyText());
		}
		void SimpleMessageBox(string message, ConsoleColor textColor = ConsoleColor.White) {
			Coord screenSize = screen.GetSize();
			const int insetSize = 2, width = 40;
			Rect area = new Rect(insetSize, insetSize, width, screenSize.row - insetSize * 2);
			Coord inset = new Coord(insetSize, insetSize / 2);
			ConsoleTile messageBack = new ConsoleTile('.', ConsoleColor.Black, ConsoleColor.DarkGray);
			MessageBox(message, textColor, area, messageBack, inset);
		}
	}
}
