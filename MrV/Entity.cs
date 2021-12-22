using System;

namespace MrV {
	public abstract class EntityBase : IDrawable, IRect, IUpdatable {
		public string name;
		public Coord position;
		public Coord velocity;

		public Action onUpdate;
		public virtual void Update() {
			position += velocity;
			onUpdate?.Invoke();
		}
		public abstract void Draw(ConsoleTile[,] map, Coord offset);
		public virtual Coord GetPosition() => position;
		public abstract Coord GetSize();
		public virtual Rect GetRect() => new Rect(position, position + GetSize());
		public void SetVelocity(Coord value) { velocity = value; }
	}

	public class EntityBasic : EntityBase {
		public ConsoleTile icon;

		public override Coord GetSize() => Coord.One;

		public EntityBasic() { }

		public EntityBasic(string name, ConsoleTile icon, Coord position) {
			this.name = name;
			this.position = position;
			this.icon = icon;
		}

		public override void Draw(ConsoleTile[,] map, Coord offset) {
			if (position.IsWithin(-offset, Coord.SizeOf(map) - offset)) {
				map[position.row + offset.row, position.col + offset.col] = icon;
			}
		}
	}

	public class EntityMobileObject : EntityBasic {
		public char currentMove;
		public Coord lastValidPosition;
		public EntityMobileObject() { }
		public EntityMobileObject(string name, ConsoleTile icon, Coord position) : base(name, icon, position) { }
	}
}