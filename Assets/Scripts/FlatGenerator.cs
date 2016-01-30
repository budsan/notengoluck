using System;
using System.Collections.Generic;
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

	string[] RoomTypeName = new[] {
		"X", "_", "C", "1", "2", "3", "4", "L", "B", "K"
	};

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
		List<Position> found = new List<Position>();
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
						found.Add(new Position(x, y));
					}
				}
			}

		if (found.Count > 0)
		{
			System.Random random = new System.Random();
			int i = random.Next(found.Count);
			result = found[i];
			return true;
		}

		result = new Position(0, 0);
		return false;
	}

	private bool AddRoom(GenerationContext context, RoomRequirement req)
	{
		Position corner;
		if (!FindCorner(context, out corner))
			return false;

		System.Random random = new System.Random();
		Range range = new Range(corner.x, corner.x + 1, corner.y, corner.y + 1);
		int sqr, currSqr;
		bool success = ValidRange(context, range, out sqr);
		currSqr = sqr;
		while(!success)
		{
			Range tmpRange = range;

			int[] p = new[] { 0, 1, 2, 3 };
			for (int i = 0; i < (p.Length - 1); i++) {
				int swap = i + (random.Next() % (p.Length - i));
				int t = p[i]; p[i] = p[swap]; p[swap] = t;
			}

			for (int i = 0; i < p.Length && currSqr == sqr; i++)
			{
				tmpRange = range;
				int op = p[i];
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

			if (currSqr != sqr)
			{
				sqr = currSqr;
				if (req.Satisfies(sqr))
				{
					PrintRange(context, req, range);
					return true;
				}
			}
			else
				return false;
		}

		return false;
	}

	void Start ()
	{
		GenerationContext context = new GenerationContext();
		bool generated = false;
		int tries = 1000;
		while (!generated && tries-- > 0)
		{
			generated = true;
			System.Random random = new System.Random();

			int Width = random.Next(15, 24);
			int Height = random.Next(15, 24);

			int SqrTotal = Width * Height;

			context.room = new RoomType[Width, Height];
			for (int x = 0; x < context.room.GetLength(0); x++)
				for (int y = 0; y < context.room.GetLength(1); y++)
					context.room[x, y] = RoomType.Unassigned;

			context.requeriments = new[] {
				new RoomRequirement((int)(SqrTotal * .1f), RoomType.Room1),
				new RoomRequirement((int)(SqrTotal * .1f), RoomType.Room2),
				new RoomRequirement((int)(SqrTotal * .1f), RoomType.Room3),
				new RoomRequirement((int)(SqrTotal * .1f), RoomType.Room4),
				new RoomRequirement((int)(SqrTotal * .2f), RoomType.Kitchen),
				new RoomRequirement((int)(SqrTotal * .25f), RoomType.LivingRoom),
				new RoomRequirement((int)(SqrTotal * .15f), RoomType.Bathroom),
			};

			//Shuffle
			for (int i = 0; i < (context.requeriments.Length - 1); i++)
			{
				int swapWith = i + (random.Next() % (context.requeriments.Length - i));
				RoomRequirement foo = context.requeriments[i];
				context.requeriments[i] = context.requeriments[swapWith];
				context.requeriments[swapWith] = foo;
			}

			foreach (RoomRequirement req in context.requeriments)
			{
				if (!AddRoom(context, req))
				{
					generated = false;
					break;
				}
			}
		}

		StringBuilder b = new StringBuilder();
		for (int x = 0; x < context.room.GetLength(0); x++)
		{
			for (int y = 0; y < context.room.GetLength(1); y++)
				b.Append(RoomTypeName[(int)context.room[x, y]]);

			b.Append("\n");
		}

		GenerateFloor(context.room);
	}

	void GenerateFloor(RoomType[,] room)
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangle = new List<int>();

		Position sz = new Position(room.GetLength(0), room.GetLength(1));
		bool[,] used = new bool[sz.x, sz.y];
		for (int x = 0; x < sz.x; x++)
			for (int y = 0; y < sz.y; y++)
				used[x, y] = false;

		for (;;)
		{
			bool found = false;
			Position init = new Position();
			for (int x = 0; x < sz.x && !found; x++)
				for (int y = 0; y < sz.y && !found; y++)
					if (!used[x, y]) {
						init = new Position(x, y);
						found = true;
					}
			
			if (!found)
				return;

			int wend = init.x + 1;
			RoomType initType = room[init.x, init.y];
			for (int x = init.x + 1; x < sz.x; x++) {
				RoomType type = room[x, init.y];
				if (type != initType || used[x, init.y])
					break;

				wend = x + 1;
			}

			bool stop = false;
			int hend = init.y + 1;
			for (int y = init.y + 1; y < sz.y && !stop; y++) {
				for (int x = init.x; x < wend && !stop; x++) {
					RoomType type = room[x, y];
					if (type != initType || used[x, y])
						stop = true;
				}

				if (!stop)
					hend = y + 1;
			}

			Range quad = new Range(init.x, wend, init.y, hend);
			for (int x = quad.wbegin; x < quad.wend; x++)
				for (int y = quad.hbegin; y < quad.hend; y++)
					used[x, y] = true;
		}
	}
}
