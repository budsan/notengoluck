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
		Count
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
		public int minWidth;

		public RoomRequirement(int minSqr, RoomType type, int minWidth)
		{
			this.minSqr = minSqr;
			this.type = type;
			this.satisfied = false;
			this.minWidth = minWidth;
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
		public System.Random random = new System.Random();

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

	RoomType getPosMasked(GenerationContext context, Range newRoom, RoomType t, int x, int y)
	{
		RoomType original = context.GetPos(x, y);
		if (newRoom.IsInRange(x, y) && original == RoomType.Unassigned)
			return t;
		return original;
	}

	bool isWidthOk(GenerationContext context, Range newRoom, RoomRequirement req)
	{
		int emptyWidth = 1;
		foreach(RoomRequirement r in context.requeriments)
			if (!r.satisfied)
				emptyWidth = Math.Max(emptyWidth, r.minWidth);
		for (int x = 0; x < context.room.GetLength(0); x++)
			for (int y = 0; y < context.room.GetLength(1); y++)
			{
				RoomType t = context.GetPos(x, y);
				if (t != RoomType.Unassigned)
					continue;
				int width = emptyWidth;
				if (newRoom.IsInRange(x, y))
				{
					t = req.type;
					width = req.minWidth;
				}
				int leftright, topdown;
				leftright = topdown = 1;
				for (int x2 = x - 1; getPosMasked(context, newRoom, req.type, x2, y) == t && leftright <= width; --x2) ++leftright;
				for (int x2 = x + 1; getPosMasked(context, newRoom, req.type, x2, y) == t && leftright <= width; ++x2) ++leftright;
				for (int y2 = y - 1; getPosMasked(context, newRoom, req.type, x, y2) == t && topdown   <= width; --y2) ++topdown;
				for (int y2 = y + 1; getPosMasked(context, newRoom, req.type, x, y2) == t && topdown   <= width; ++y2) ++topdown;
				if (leftright < width || topdown < width)
				{
					return false;
				}
			}
		return true;
	}

	void PrintRange(GenerationContext context, RoomRequirement req, Range range)
	{
		for (int x = range.wbegin; x < range.wend; x++)
			for (int y = range.hbegin; y < range.hend; y++)
				if (context.GetPos(x, y) == RoomType.Unassigned)
					context.SetPos(x, y, req.type);
	}

	bool ValidRange(GenerationContext context, Range range, RoomRequirement req, out int sqr)
	{
		int count = 0;
		for (int x = range.wbegin; x < range.wend; x++)
			for (int y = range.hbegin; y < range.hend; y++)
				if (context.GetPos(x, y) == RoomType.Unassigned)
					count++;
		sqr = count;
		return isWidthOk(context, range, req);
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
						found.Add(new Position(x, y));
				}
			}

		if (found.Count > 0)
		{
			int i = context.random.Next(found.Count);
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

		int currSqr, tmpSqr;
		currSqr = tmpSqr = 1;
		Range currRange = new Range(corner.x, corner.x + 1, corner.y, corner.y + 1);
		Range tmpRange = currRange;
		int numFails = 0;
		int tries = 100;
		while(tries-- > 0)
		{
			int expandDir = context.random.Next() % 4;
			switch (expandDir)
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
			bool valid = ValidRange(context, tmpRange, req, out tmpSqr);
			if (valid && tmpSqr > currSqr)
			{
				currSqr = tmpSqr;
				currRange = tmpRange;
			}
			else if (tmpSqr > currSqr)
			{
				numFails++;
			}
			if (req.Satisfies(currSqr))
			{
				PrintRange(context, req, currRange);
				return true;
			}
			if (numFails > req.minWidth + 5)
			{
				numFails = 0;
				tmpRange = currRange;
				tmpSqr = currSqr;
			}
		}
		return false;
	}

	void Start ()
	{
		GenerationContext context = new GenerationContext();
		int seed = context.random.Next() % (1 << 16);
		context.random = new System.Random(seed);
		Debug.Log("Seed: " + seed);

		bool generated = false;
		int tries = 100;
		while (!generated && tries-- > 0)
		{
			generated = true;
			int Width = context.random.Next(13, 25);
			int Height = context.random.Next(13, 25);

			int SqrTotal = Width * Height;

			context.room = new RoomType[Width, Height];
			for (int x = 0; x < context.room.GetLength(0); x++)
				for (int y = 0; y < context.room.GetLength(1); y++)
					context.room[x, y] = RoomType.Unassigned;

			context.requeriments = new[] {
				new RoomRequirement((int)(SqrTotal * .1f), RoomType.Room1, 3),
				new RoomRequirement((int)(SqrTotal * .2f), RoomType.LivingRoom, 3),
				new RoomRequirement((int)(SqrTotal * .1f), RoomType.Room2, 2),
				new RoomRequirement((int)(SqrTotal * .1f), RoomType.Room3, 2),
				new RoomRequirement((int)(SqrTotal * .1f), RoomType.Room4, 2),
				new RoomRequirement((int)(SqrTotal * .2f), RoomType.Kitchen, 3),
				new RoomRequirement((int)(SqrTotal * .1f), RoomType.Bathroom, 2),
			};

			////Shuffle
			//for (int i = 0; i < (context.requeriments.Length - 1); i++)
			//{
			//	int swapWith = i + (context.random.Next() % (context.requeriments.Length - i));
			//	RoomRequirement foo = context.requeriments[i];
			//	context.requeriments[i] = context.requeriments[swapWith];
			//	context.requeriments[swapWith] = foo;
			//}

			foreach (RoomRequirement req in context.requeriments)
			{
				bool added = false;
				int tries2 = 1;
				while (!added && tries2-- > 0) {
					added = AddRoom(context, req);
				}
				if (!added)
				{
					generated = false;
					break;
				}
				req.satisfied = true;
			}
		}
		if (!generated)
			Debug.Log("Unable to generate! :c");

		GenerateFloor(context.room);
	}

	MeshRenderer GetMeshRenderer()
	{
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		if (meshRenderer == null)
			meshRenderer = gameObject.AddComponent<MeshRenderer>();

		return meshRenderer;
	}

	MeshFilter GetMeshFilter()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter == null)
			meshFilter = gameObject.AddComponent<MeshFilter>();

		return meshFilter;
	}

	class MeshContext
	{
		public RoomType[,] room;
		public bool[,] doors;
		public List<Vector2> uv = new List<Vector2>();
		public List<Vector3> vertices = new List<Vector3>();
		public List<int>[] triangle = new List<int>[(int)RoomType.Count];

		public RoomType GetPos(int x, int y)
		{
			if (x < 0 || y < 0 || x >= room.GetLength(0) || y >= room.GetLength(1))
				return RoomType.Invalid;

			return room[x, y];
		}

		public RoomType GetPos(Position p)
		{
			return GetPos(p.x, p.y);
		}
	}

	struct DoorOption
	{
		public Position p1;
		public Position p2;
		public DoorOption(Position p1, Position p2)
		{
			this.p1 = p1;
			this.p2 = p2;
		}
	}

	void GenerateDoors(MeshContext mc)
	{
		List<DoorOption>[,] doorOptions = new List<DoorOption>[(int)RoomType.Count, (int)RoomType.Count];
		for (int i = 0; i < doorOptions.GetLength(0); ++i)
			for (int j = 0; j < doorOptions.GetLength(1); ++j)
				doorOptions[i, j] = new List<DoorOption>();
		for (int i = 0; i < mc.room.GetLength(0); ++i)
			for (int j = 0; j < mc.room.GetLength(1); ++j)
			{
				RoomType t1 = mc.GetPos(i, j);
				if (t1 == RoomType.Invalid || t1 == RoomType.Unassigned)
					continue;
				Position[] neighbors = {
					new Position(i, j-1),
					new Position(i, j+1),
					new Position(i-1, j),
					new Position(i+1, j),
				};
				for (int n = 0; n < 4; ++n)
				{
					Position p = neighbors[n];
					RoomType t2 = mc.GetPos(p);
					if (t2 == RoomType.Invalid || t2 == RoomType.Unassigned)
						continue;
					int dx = n < 2 ? 1 : 0;
					int dy = 1 - dx;
					Position n11 = new Position(i + dx, j + dy);
					Position n12 = new Position(i - dx, j - dy);
					if (mc.GetPos(n11) != t1 || mc.GetPos(n12) != t1)
						continue;
					Position n21 = new Position(p.x + dx, p.y + dy);
					Position n22 = new Position(p.x - dx, p.y - dy);
					if (mc.GetPos(n21) != t2 || mc.GetPos(n22) != t2)
						continue;
					doorOptions[(int)t1, (int)t2].Add(new DoorOption(new Position(i, j), p));
				}
			}
		for (int i = 0; i < (int)RoomType.Count; ++i)
			for (int j = i + 1; j < (int)RoomType.Count; ++j)
			{
				int numOptions = doorOptions[i, j].Count;
				if (numOptions == 0)
					continue;
				int nDoors = numOptions > 5 ? 2 : 1;
				for (int k = 0; k < nDoors; ++k)
				{
					int index = UnityEngine.Random.Range(0, doorOptions[i, j].Count);
					DoorOption d = doorOptions[i, j][index];
					mc.doors[d.p1.x, d.p1.y] = true;
					mc.doors[d.p2.x, d.p2.y] = true;
				}
			}
	}

	void GenerateFloor(RoomType[,] room)
	{
		MeshFilter meshFilter = GetMeshFilter();
		Position sz = new Position(room.GetLength(0), room.GetLength(1));
		MeshContext mc = new MeshContext();
		mc.room = room;
		mc.doors = new bool[sz.x, sz.y];
		for (int x = 0; x < sz.x; x++)
			for (int y = 0; y < sz.y; y++)
				mc.doors[x, y] = false;

		GameObject floor = new GameObject("Floor");
		floor.transform.SetParent(transform, false);
		floor.transform.localPosition = new Vector3(0, 0, 0);
		BoxCollider coll = floor.AddComponent<BoxCollider>();
		coll.center = new Vector3(0, -0.5f, 0);
		coll.size = new Vector3(sz.x, 1, sz.y);

		Vector3 offset = new Vector3(room.GetLength(0)/-2.0f, 0, room.GetLength(1) / -2.0f);
		bool[,] used = new bool[sz.x, sz.y];
		for (int x = 0; x < sz.x; x++)
			for (int y = 0; y < sz.y; y++)
				used[x, y] = false;

		GenerateDoors(mc);

		for (;;)
		{
			bool found = false;
			Position init = new Position();
			for (int x = 0; x < sz.x && !found; x++)
				for (int y = 0; y < sz.y && !found; y++)
					if (!used[x, y] && room[x,y] != RoomType.Unassigned) {
						init = new Position(x, y);
						found = true;
					}

			if (!found)
				break;

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

			int vid = mc.vertices.Count;
			mc.vertices.Add(new Vector3(quad.wbegin, 0.0f, quad.hbegin) + offset);
			mc.vertices.Add(new Vector3(quad.wend  , 0.0f, quad.hbegin) + offset);
			mc.vertices.Add(new Vector3(quad.wbegin, 0.0f, quad.hend) + offset);
			mc.vertices.Add(new Vector3(quad.wend  , 0.0f, quad.hend) + offset);

			mc.uv.Add(new Vector2(quad.wbegin, quad.hbegin));
			mc.uv.Add(new Vector2(quad.wend, quad.hbegin));
			mc.uv.Add(new Vector2(quad.wbegin, quad.hend));
			mc.uv.Add(new Vector2(quad.wend, quad.hend));

			if (mc.triangle[(int)initType] == null)
				mc.triangle[(int)initType] = new List<int>();

			List<int> submesh = mc.triangle[(int)initType];
			submesh.Add(vid + 0); submesh.Add(vid + 2); submesh.Add(vid + 1);
			submesh.Add(vid + 2); submesh.Add(vid + 3); submesh.Add(vid + 1);

			//Walls
			if (mc.triangle[(int)RoomType.Invalid] == null)
				mc.triangle[(int)RoomType.Invalid] = new List<int>();

			//UP WALL
			{
				int length = 0;
				RoomType neigh = RoomType.Invalid;
				bool offlimit = quad.hend >= sz.y;
				for (int x = quad.wbegin; x < quad.wend; x++)
				{
					bool door = !offlimit && mc.doors[x, quad.hend];
					RoomType cneigh = offlimit ? RoomType.Invalid : room[x, quad.hend];
					bool valid = (door) || (neigh != cneigh);
					if (valid && (length > 0)) {
						UpWall(mc, quad, offset, x, length, cneigh);
						length = 0;
					}

					length = (cneigh != initType) && !door ? length + 1 : 0;
					neigh = cneigh;
				}

				if (length > 0)
					UpWall(mc, quad, offset, quad.wend, length, neigh);
			}

			////DOWN WALL
			{
				int length = 0;
				RoomType neigh = RoomType.Invalid;
				bool offlimit = (quad.hbegin - 1) < 0;
				for (int x = quad.wbegin; x < quad.wend; x++)
				{
					bool door = !offlimit && mc.doors[x, quad.hbegin - 1];
					RoomType cneigh = offlimit ? RoomType.Invalid : room[x, quad.hbegin - 1];
					bool valid = (door) || (neigh != cneigh);
					if (valid && (length > 0)) {
						DownWall(mc, quad, offset, x, length, cneigh);
						length = 0;
					}

					length = (cneigh != initType) && !door ? length + 1 : 0;
					neigh = cneigh;
				}

				if (length > 0)
					DownWall(mc, quad, offset, quad.wend, length, neigh);
			}

			////LEFT WALL
			{
				int length = 0;
				RoomType neigh = RoomType.Invalid;
				bool offlimit = (quad.wbegin - 1) < 0;
				for (int y = quad.hbegin; y < quad.hend; y++)
				{
					bool door = !offlimit && mc.doors[quad.wbegin - 1, y];
					RoomType cneigh = offlimit ? RoomType.Invalid : room[quad.wbegin - 1, y];
					bool valid = (door) || (neigh != cneigh);
					if (valid && (length > 0))
					{
						LeftWall(mc, quad, offset, y, length, cneigh);
						length = 0;
					}

					length = (cneigh != initType) && !door ? length + 1 : 0;
					neigh = cneigh;
				}

				if (length > 0)
					LeftWall(mc, quad, offset, quad.hend, length, neigh);
			}

			////RIGHT WALL
			{
				int length = 0;
				RoomType neigh = RoomType.Invalid;
				bool offlimit = quad.wend >= sz.x;
				for (int y = quad.hbegin; y < quad.hend; y++)
				{
					bool door = !offlimit && mc.doors[quad.wend, y];
					RoomType cneigh = offlimit ? RoomType.Invalid : room[quad.wend, y];
					bool valid = (door) || (neigh != cneigh);
					if (valid && (length > 0))
					{
						RightWall(mc, quad, offset, y, length, cneigh);
						length = 0;
					}

					length = (cneigh != initType) && !door ? length + 1 : 0;
					neigh = cneigh;
				}

				if (length > 0)
					RightWall(mc, quad, offset, quad.hend, length, neigh);
			}
		}

		if (meshFilter.mesh == null)
			meshFilter.mesh = new Mesh();

		Mesh mesh = meshFilter.mesh;
		mesh.vertices = mc.vertices.ToArray();
		mesh.uv = mc.uv.ToArray();
		mesh.subMeshCount = mc.triangle.Length;
		for (int i = 0; i < mc.triangle.Length; i++)
		{
			if (mc.triangle[i] != null)
				mesh.SetTriangles(mc.triangle[i], i);
		}
	}

	const float wHigh = 0.5f;
	const float wOff = 0.0f;
	const float wColHigh = 3.0f;
	const float wColOff = 0.1f;

	void UpWall(MeshContext mc, Range quad, Vector3 offset, int x, int length, RoomType neighType)
	{
		Range wrange = quad;
		wrange.wend = x;
		wrange.wbegin = x - length;
		UpWall(mc, wrange, offset);
	}

	void DownWall(MeshContext mc, Range quad, Vector3 offset, int x, int length, RoomType neighType)
	{
		Range wrange = quad;
		wrange.wend = x;
		wrange.wbegin = x - length;
		DownWall(mc, wrange, offset);
	}

	void LeftWall(MeshContext mc, Range quad, Vector3 offset, int y, int length, RoomType neighType)
	{
		Range wrange = quad;
		wrange.hend = y;
		wrange.hbegin = y - length;
		LeftWall(mc, wrange, offset);
	}

	void RightWall(MeshContext mc, Range quad, Vector3 offset, int y, int length, RoomType neighType)
	{
		Range wrange = quad;
		wrange.hend = y;
		wrange.hbegin = y - length;
		RightWall(mc, wrange, offset);
	}

	void UpWall(MeshContext mc, Range wrange, Vector3 offset)
	{
		int vid = mc.vertices.Count;
		mc.vertices.Add(new Vector3(wrange.wend - wOff, wHigh, wrange.hend - wOff) + offset);
		mc.vertices.Add(new Vector3(wrange.wbegin + wOff, wHigh, wrange.hend - wOff) + offset);
		mc.vertices.Add(new Vector3(wrange.wend - wOff, 0.0f, wrange.hend - wOff) + offset);
		mc.vertices.Add(new Vector3(wrange.wbegin + wOff, 0.0f, wrange.hend - wOff) + offset);

		mc.uv.Add(new Vector2(wrange.wbegin, 0));
		mc.uv.Add(new Vector2(wrange.wend, 0));
		mc.uv.Add(new Vector2(wrange.wbegin, wHigh));
		mc.uv.Add(new Vector2(wrange.wend, wHigh));

		List<int> wallMesh = mc.triangle[(int)RoomType.Invalid];
		wallMesh.Add(vid + 0); wallMesh.Add(vid + 2); wallMesh.Add(vid + 1);
		wallMesh.Add(vid + 2); wallMesh.Add(vid + 3); wallMesh.Add(vid + 1);

		BoxCollider coll = gameObject.AddComponent<BoxCollider>();
		coll.center = new Vector3(
			(wrange.wend + wrange.wbegin) * 0.5f,
			wColHigh * 0.5f,
			wrange.hend - (wColOff * 0.5f)
		) + offset;

		coll.size = new Vector3(
			(wrange.wend - wrange.wbegin),
			wColHigh,
			wColOff
		);
	}

	void DownWall(MeshContext mc, Range wrange, Vector3 offset)
	{
		int vid = mc.vertices.Count;
		mc.vertices.Add(new Vector3(wrange.wbegin + wOff, wHigh, wrange.hbegin + wOff) + offset);
		mc.vertices.Add(new Vector3(wrange.wend - wOff, wHigh, wrange.hbegin + wOff) + offset);
		mc.vertices.Add(new Vector3(wrange.wbegin + wOff, 0.0f, wrange.hbegin + wOff) + offset);
		mc.vertices.Add(new Vector3(wrange.wend - wOff, 0.0f, wrange.hbegin + wOff) + offset);

		mc.uv.Add(new Vector2(wrange.wbegin, 0));
		mc.uv.Add(new Vector2(wrange.wend, 0));
		mc.uv.Add(new Vector2(wrange.wbegin, wHigh));
		mc.uv.Add(new Vector2(wrange.wend, wHigh));

		List<int> wallMesh = mc.triangle[(int)RoomType.Invalid];
		wallMesh.Add(vid + 0); wallMesh.Add(vid + 2); wallMesh.Add(vid + 1);
		wallMesh.Add(vid + 2); wallMesh.Add(vid + 3); wallMesh.Add(vid + 1);

		BoxCollider coll = gameObject.AddComponent<BoxCollider>();
		coll.center = new Vector3(
			(wrange.wend + wrange.wbegin) * 0.5f,
			wColHigh * 0.5f,
			wrange.hbegin + (wColOff * 0.5f)
		) + offset;

		coll.size = new Vector3(
			(wrange.wend - wrange.wbegin),
			wColHigh,
			wColOff
		);
	}

	void LeftWall(MeshContext mc, Range wrange, Vector3 offset)
	{
		int vid = mc.vertices.Count;
		mc.vertices.Add(new Vector3(wrange.wbegin + wOff, wHigh, wrange.hend - wOff) + offset);
		mc.vertices.Add(new Vector3(wrange.wbegin + wOff, wHigh, wrange.hbegin + wOff) + offset);
		mc.vertices.Add(new Vector3(wrange.wbegin + wOff, 0.0f, wrange.hend - wOff) + offset);
		mc.vertices.Add(new Vector3(wrange.wbegin + wOff, 0.0f, wrange.hbegin + wOff) + offset);

		mc.uv.Add(new Vector2(wrange.hbegin, 0));
		mc.uv.Add(new Vector2(wrange.hend, 0));
		mc.uv.Add(new Vector2(wrange.hbegin, wHigh));
		mc.uv.Add(new Vector2(wrange.hend, wHigh));

		List<int> wallMesh = mc.triangle[(int)RoomType.Invalid];
		wallMesh.Add(vid + 0); wallMesh.Add(vid + 2); wallMesh.Add(vid + 1);
		wallMesh.Add(vid + 2); wallMesh.Add(vid + 3); wallMesh.Add(vid + 1);

		BoxCollider coll = gameObject.AddComponent<BoxCollider>();
		coll.center = new Vector3(
			wrange.wbegin + (wColOff * 0.5f),
			wColHigh * 0.5f,
			(wrange.hend + wrange.hbegin) * 0.5f
		) + offset;

		coll.size = new Vector3(
			wColOff, 
			wColHigh,
			(wrange.hend - wrange.hbegin)
		);
	}

	void RightWall(MeshContext mc, Range wrange, Vector3 offset)
	{
		int vid = mc.vertices.Count;
		mc.vertices.Add(new Vector3(wrange.wend - wOff, wHigh, wrange.hbegin + wOff) + offset);
		mc.vertices.Add(new Vector3(wrange.wend - wOff, wHigh, wrange.hend - wOff) + offset);
		mc.vertices.Add(new Vector3(wrange.wend - wOff, 0.0f, wrange.hbegin + wOff) + offset);
		mc.vertices.Add(new Vector3(wrange.wend - wOff, 0.0f, wrange.hend - wOff) + offset);

		mc.uv.Add(new Vector2(wrange.hbegin, 0));
		mc.uv.Add(new Vector2(wrange.hend, 0));
		mc.uv.Add(new Vector2(wrange.hbegin, wHigh));
		mc.uv.Add(new Vector2(wrange.hend, wHigh));

		List<int> wallMesh = mc.triangle[(int)RoomType.Invalid];
		wallMesh.Add(vid + 0); wallMesh.Add(vid + 2); wallMesh.Add(vid + 1);
		wallMesh.Add(vid + 2); wallMesh.Add(vid + 3); wallMesh.Add(vid + 1);

		BoxCollider coll = gameObject.AddComponent<BoxCollider>();
		coll.center = new Vector3(
			wrange.wend - (wColOff * 0.5f),
			wColHigh * 0.5f,
			(wrange.hend + wrange.hbegin) * 0.5f
		) + offset;

		coll.size = new Vector3(
			wColOff,
			wColHigh,
			(wrange.hend - wrange.hbegin)
		);
	}
}
