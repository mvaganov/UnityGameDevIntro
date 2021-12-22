using System;
namespace Game0 {
	class Program {
		public static void Main(string[] args) {
			int width = 30, height = 15, x = 3, y = 4;
			char player = '@', world = '.';
			bool running = true;
			while (running) {
				Console.SetCursorPosition(0, 0);
				for (int row = 0; row < height; ++row) {
					for (int col = 0; col < width; ++col) {
						Console.Write(world);
					}
					Console.Write('\n');
				}
				Console.SetCursorPosition(x, y);
				Console.Write(player);
				Console.SetCursorPosition(0, height);
				ConsoleKeyInfo userInput = Console.ReadKey();
				switch (userInput.KeyChar) {
					case 'w': --y; break;
					case 'a': --x; break;
					case 's': ++y; break;
					case 'd': ++x; break;
					case 'q': case (char)27: running = false; break;
				}
			}
		}
	}
}
