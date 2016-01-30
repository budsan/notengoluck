using System;
using System.Text;
using UnityEngine;

public class FlatGenerator : MonoBehaviour
{
	public enum RoomType
	{
		Invalid,
		Unassigned,
		Corridor,
		Room1,
		Room2,
		Room3,
		Room4,
		LivingRoom,
		Bathroom,
		Kitchen,
	}

	struct Range
	{
		public int wbegin, wend;
		public int hbegin, hend;

		private static void Swap<T>(ref T lhs, ref T rhs)
		{
			T temp;
			temp = lhs;
			lhs = rhs;
			rhs = temp;
		}

		public Range(int wbegin, int wend, int hbegin, int hend)
		{
			this.wbegin = wbegin;
			this.wend = wend;
			if (this.wend < this.wbegin)
				Swap<int>(ref this.wbegin, ref this.wend);

			this.hbegin = hbegin;
			this.hend = hend;
			if (this.hend < this.hbegin)
				Swap<int>(ref this.hbegin, ref this.hend);
		}

		public int WidthWide
		{
			get { return wend - wbegin; }
		}

		public int HeightWide
		{
			get { return hend - hbegin; }
		}

		public int Wide
		{
			get
			{
				int wwide = WidthWide;
				int hwide = HeightWide;
				if (wwide < hwide)
					return wwide;
				else
					return hwide;
			}
		}

		public bool IsInRange(Position p)
		{
			return IsInRange(p.x, p.y);
		}

		public bool IsInRange(int x, int y)
		{
			return x >= wbegin && x < wend && y >= hbegin && y < hend; 
		}

		public int Sqr { get { return WidthWide * HeightWide; } }
	}

	class RoomRequirement
	{
		public readonly int minSqr;
		public readonly RoomType type;
		public bool satisfied;

		public RoomRequirement(int minSqr, RoomType type)
		{
			this.minSqr = minSqr;
			this.type = type;
			this.satisfied = false;
		}

		public bool Satisfies(int sqr)
		{
			return sqr >= minSqr;
		}
	}

	class GenerationContext
	{
		public RoomType[,] room;
		public RoomRequirement[] requeriments;

		public RoomType GetPos(int x, int y)
		{
			if (x < 0 || y < 0 || x >= room.GetLength(0) || y >= room.GetLength(1))
				return RoomType.Invalid;

			return room[x, y];
		}

		public void SetPos(int x, int y, RoomType type)
		{
			if (x < 0 || y < 0 || x >= room.GetLength(0) || y >= room.GetLength(1))
				return;

			room[x, y] = type;
		}

		public RoomType GetPos(Position p)
		{
			return GetPos(p.x, p.y);
		}
	}

	struct Position
	{
		public int x, y;
		public Position(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	void PrintRange(GenerationContext context, RoomRequirement req, Range range)
	{
		for (int x = range.wbegin; x < range.wend; x++)
		{
			for (int y = range.hbegin; y < range.hend; y++)
			{
				if (context.GetPos(x, y) == RoomType.Unassigned)
					context.SetPos(x, y, req.type);
			}
		}
	}

	bool ValidRange(GenerationContext context, Range range, out int sqr)
	{
		bool valid = true;
		int count = 0;
		for (int x = range.wbegin; x < range.wend; x++)
		{
			for (int y = range.hbegin; y < range.hend; y++)
			{
				if (context.GetPos(x, y) == RoomType.Unassigned)
				{
					Position p1 = new Position(x + 1, y + 1);
					Position p2 = new Position(x - 1, y + 1);
					Position p3 = new Position(x + 1, y - 1);
					Position p4 = new Position(x - 1, y - 1);

					int b1 = context.GetPos(p1) == RoomType.Unassigned && range.IsInRange(p1) ? 1 : 0;
					int b2 = context.GetPos(p2) == RoomType.Unassigned && range.IsInRange(p2) ? 1 : 0;
					int b3 = context.GetPos(p3) == RoomType.Unassigned && range.IsInRange(p3) ? 1 : 0;
					int b4 = context.GetPos(p4) == RoomType.Unassigned && range.IsInRange(p4) ? 1 : 0;
					int c = b1 + b2 + b3 + b4;

					if (c < 2)
						valid = false;
					else if (c == 2 && ((b1 == 1 && b2 == 1) || (b3 == 1 && b4 == 1)))
						valid = false;
						
					count++;
				}
			}
		}

		for (int x = 0; x < context.room.GetLength(0); x++)
		{
			for (int y = 0; y < context.room.GetLength(1); y++)
			{
				if (context.GetPos(x, y) == RoomType.Unassigned)
				{
					Position p1 = new Position(x + 1, y + 0);
					Position p2 = new Position(x - 1, y + 0);
					Position p3 = new Position(x + 0, y + 1);
					Position p4 = new Position(x + 0, y - 1);

					int b1 = context.GetPos(p1) == RoomType.Unassigned && !range.IsInRange(p1) ? 1 : 0;
					int b2 = context.GetPos(p2) == RoomType.Unassigned && !range.IsInRange(p2) ? 1 : 0;
					int b3 = context.GetPos(p3) == RoomType.Unassigned && !range.IsInRange(p3) ? 1 : 0;
					int b4 = context.GetPos(p4) == RoomType.Unassigned && !range.IsInRange(p4) ? 1 : 0;
					int c = b1 + b2 + b3 + b4;

					if (c < 2)
						valid = false;
					else if (c == 2 && ((b1 == 1 && b2 == 1) || (b3 == 1 && b4 == 1)))
						valid = false;
				}
			}
		}

		sqr = count;
		return valid;
	}

	bool FindCorner(GenerationContext context, out Position result, int centinel = 2)
	{
		for (int x = 0; x < context.room.GetLength(0); x++)
			for (int y = 0; y < context.room.GetLength(1); y++)
			{
				if (context.room[x,y] == RoomType.Unassigned)
				{
					int c = 0;
					c += context.GetPos(x + 1, y + 0) == RoomType.Unassigned ? 1 : 0;
					c += context.GetPos(x - 1, y + 0) == RoomType.Unassigned ? 1 : 0;
					c += context.GetPos(x + 0, y + 1) == RoomType.Unassigned ? 1 : 0;
					c += context.GetPos(x + 0, y - 1) == RoomType.Unassigned ? 1 : 0;

					if (c == centinel)
					{
						result = new Position(x, y);
						return true;
					}
				}
			}

		result = new Position(0, 0);
		return false;
	}

	private bool AddRoom(GenerationContext context, RoomRequirement req)
	{
		Position corner;
		//TODO: Que esta corner sea random, no la primera
		if (!FindCorner(context, out corner))
			return false;

		System.Random random = new System.Random();
		Range range = new Range(corner.x, corner.x + 1, corner.y, corner.y + 1);
		int sqr, currSqr;
		bool success = ValidRange(context, range, out sqr);
		currSqr = sqr;
		int tries = 100;
		while(!success && tries-- > 0)
		{
			Range tmpRange = range;
			int tries2 = 100;
			while (currSqr == sqr && tries2-- > 0)
			{
				tmpRange = range;
				int op = random.Next() % 4;
				switch (op)
				{
					case 0:
						tmpRange.wbegin--;
						break;
					case 1:
						tmpRange.wend++;
						break;
					case 2:
						tmpRange.hbegin--;
						break;
					case 3:
						tmpRange.hend++;
						break;
				}

				int tmpSqr;
				ValidRange(context, tmpRange, out tmpSqr);
				if (tmpSqr > currSqr)
				{
					currSqr = tmpSqr;
					range = tmpRange;
				}
			}

			sqr = currSqr;
			if (req.Satisfies(sqr))
				success = true;
		}

		if (success)
			PrintRange(context, req, range);
		else
			Debug.Log("Cannot place req :c");
		return success;
	}

	void Start ()
	{
		System.Random random = new System.Random();

		int Width = random.Next(15, 24);
		int Height = random.Next(15, 24);

		GenerationContext context = new GenerationContext();
		context.room = new RoomType[Width, Height];
		for (int x = 0; x < context.room.GetLength(0); x++)
			for (int y = 0; y < context.room.GetLength(1); y++)
				context.room[x, y] = RoomType.Unassigned;

		context.requeriments = new[] {
			new RoomRequirement(40, RoomType.Room1),
			new RoomRequirement(40, RoomType.Room2),
			new RoomRequirement(40, RoomType.Room3),
			new RoomRequirement(40, RoomType.Room4),
			new RoomRequirement(50, RoomType.Kitchen),
			new RoomRequirement(70, RoomType.LivingRoom),
			new RoomRequirement(20, RoomType.Bathroom),
		};

		//Shuffle
		for (int i = 0; i < (context.requeriments.Length - 1); i++)
		{
			int swapWith = i + (random.Next() % (context.requeriments.Length - i));
			RoomRequirement foo = context.requeriments[i];
			context.requeriments[i] = context.requeriments[swapWith];
			context.requeriments[swapWith] = foo;
		}

		//TODO: Si uno de los AddRooms peta, tira el algo desde 0 again.
		foreach(RoomRequirement req in context.requeriments)
			AddRoom(context, req);

		StringBuilder b = new StringBuilder();
		for (int x = 0; x < context.room.GetLength(0); x++)
		{
			for (int y = 0; y < context.room.GetLength(1); y++)
			{
				switch(context.room[x,y])
				{
					case RoomType.Room1:
						b.Append("1");
						break;
					case RoomType.Room2:
						b.Append("2");
						break;
					case RoomType.Room3:
						b.Append("3");
						break;
					case RoomType.Room4:
						b.Append("4");
						break;
					case RoomType.Kitchen:
						b.Append("K");
						break;
					case RoomType.LivingRoom:
						b.Append("L");
						break;
					case RoomType.Bathroom:
						b.Append("B");
						break;
					case RoomType.Corridor:
						b.Append("C");
						break;
					case RoomType.Unassigned:
						b.Append("_");
						break;
				}
			}

			b.Append("\n");
		}

		Debug.Log(b.ToString());
	}
}
