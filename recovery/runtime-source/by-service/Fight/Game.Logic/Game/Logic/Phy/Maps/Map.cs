using System;
using System.Collections.Generic;
using System.Drawing;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Maps;

public class Map
{
	private MapInfo _info;

	private float _wind;

	private HashSet<Physics> _objects;

	protected Tile _layer1;

	protected Tile _layer2;

	protected Rectangle _bound;

	public float wind
	{
		get
		{
			return _wind;
		}
		set
		{
			_wind = value;
		}
	}

	public float gravity => _info.Weight;

	public float airResistance => _info.DragIndex;

	public Tile Ground => _layer1;

	public MapInfo Info => _info;

	public Rectangle Bound => _bound;

	public Map(MapInfo info, Tile layer1, Tile layer2)
	{
		_info = info;
		_objects = new HashSet<Physics>();
		_layer1 = layer1;
		_layer2 = layer2;
		if (_layer1 != null)
		{
			_bound = new Rectangle(0, 0, _layer1.Width, _layer1.Height);
		}
		else
		{
			_bound = new Rectangle(0, 0, _layer2.Width, _layer2.Height);
		}
	}

	public void Dig(int cx, int cy, Tile surface, Tile border)
	{
		if (_layer1 != null)
		{
			_layer1.Dig(cx, cy, surface, border);
		}
		if (_layer2 != null)
		{
			_layer2.Dig(cx, cy, surface, border);
		}
	}

	public bool IsEmpty(int x, int y)
	{
		return (_layer1 == null || _layer1.IsEmpty(x, y)) && (_layer2 == null || _layer2.IsEmpty(x, y));
	}

	public bool IsSpecialMap()
	{
		int iD = Info.ID;
		return iD == 1303;
	}

	public bool IsRectangleEmpty(Rectangle rect)
	{
		return (_layer1 == null || _layer1.IsRectangleEmptyQuick(rect)) && (_layer2 == null || _layer2.IsRectangleEmptyQuick(rect));
	}

	public Point FindYLineNotEmptyPoint(int x, int y, int h)
	{
		x = ((x >= 0) ? ((x >= _bound.Width) ? (_bound.Width - 1) : x) : 0);
		y = ((y >= 0) ? y : 0);
		h = ((y + h >= _bound.Height) ? (_bound.Height - y - 1) : h);
		for (int i = 0; i < h; i++)
		{
			if (!IsEmpty(x - 1, y) || !IsEmpty(x + 1, y))
			{
				return new Point(x, y);
			}
			y++;
		}
		return Point.Empty;
	}

	public Point FindYLineNotEmptyPoint(int x, int y)
	{
		return FindYLineNotEmptyPoint(x, y, _bound.Height);
	}

	public Point FindNextWalkPoint(int x, int y, int direction, int stepX, int stepY)
	{
		if (direction != 1 && direction != -1)
		{
			return Point.Empty;
		}
		int num = x + direction * stepX;
		if (num < 0 || num > _bound.Width)
		{
			return Point.Empty;
		}
		Point point = FindYLineNotEmptyPoint(num, y - stepY - 1, _bound.Height);
		if (point != Point.Empty && Math.Abs(point.Y - y) > stepY)
		{
			point = Point.Empty;
		}
		return point;
	}

	public bool canMove(int x, int y)
	{
		return IsEmpty(x, y) && !IsOutMap(x, y);
	}

	public bool IsOutMap(int x, int y)
	{
		return x < 0 || x >= _bound.Width || y >= _bound.Height;
	}

	public void AddPhysical(Physics phy)
	{
		phy.SetMap(this);
		lock (_objects)
		{
			_objects.Add(phy);
		}
	}

	public void RemovePhysical(Physics phy)
	{
		phy.SetMap(null);
		lock (_objects)
		{
			_objects.Remove(phy);
		}
	}

	public List<Physics> GetAllPhysicalSafe()
	{
		List<Physics> list = new List<Physics>();
		lock (_objects)
		{
			foreach (Physics @object in _objects)
			{
				list.Add(@object);
			}
		}
		return list;
	}

	public List<PhysicalObj> GetAllPhysicalObjSafe()
	{
		List<PhysicalObj> list = new List<PhysicalObj>();
		lock (_objects)
		{
			foreach (Physics @object in _objects)
			{
				if (@object is PhysicalObj)
				{
					list.Add(@object as PhysicalObj);
				}
			}
		}
		return list;
	}

	public Physics[] FindPhysicalObjects(Rectangle rect, Physics except)
	{
		List<Physics> list = new List<Physics>();
		lock (_objects)
		{
			foreach (Physics @object in _objects)
			{
				if (@object.IsLiving && @object != except)
				{
					Rectangle bound = @object.Bound;
					Rectangle bound2 = @object.Bound1;
					bound.Offset(@object.X, @object.Y);
					bound2.Offset(@object.X, @object.Y);
					if (bound.IntersectsWith(rect) || bound2.IntersectsWith(rect))
					{
						list.Add(@object);
					}
				}
			}
		}
		return list.ToArray();
	}

	public bool FindPlayers(Point p, int radius)
	{
		int num = 0;
		lock (_objects)
		{
			foreach (Physics @object in _objects)
			{
				if (@object is Player && @object.IsLiving && (@object as Player).BoundDistance(p) < (double)radius)
				{
					num++;
				}
				if (num >= 2)
				{
					return true;
				}
			}
		}
		return false;
	}

	public List<Player> FindPlayers(int x, int y, int radius)
	{
		List<Player> list = new List<Player>();
		lock (_objects)
		{
			foreach (Physics @object in _objects)
			{
				if (@object is Player && @object.IsLiving && @object.Distance(x, y) < (double)radius)
				{
					list.Add(@object as Player);
				}
			}
		}
		return list;
	}

	public List<Living> FindLivings(int x, int y, int radius)
	{
		List<Living> list = new List<Living>();
		lock (_objects)
		{
			foreach (Physics @object in _objects)
			{
				if (@object is Living && @object.IsLiving && @object.Distance(x, y) < (double)radius)
				{
					list.Add(@object as Living);
				}
			}
		}
		return list;
	}

	public List<Living> FindPlayers(int fx, int tx, List<Player> exceptPlayers)
	{
		List<Living> list = new List<Living>();
		lock (_objects)
		{
			foreach (Physics @object in _objects)
			{
				if (!(@object is Player) || !@object.IsLiving || @object.X <= fx || @object.X >= tx)
				{
					continue;
				}
				if (exceptPlayers != null)
				{
					foreach (Player exceptPlayer in exceptPlayers)
					{
						if (((Player)@object).PlayerDetail != exceptPlayer.PlayerDetail)
						{
							list.Add(@object as Living);
						}
					}
				}
				else
				{
					list.Add(@object as Living);
				}
			}
		}
		return list;
	}

	public List<Living> FindHitByHitPiont()
	{
		List<Living> list = new List<Living>();
		lock (_objects)
		{
			foreach (Physics @object in _objects)
			{
				if (@object is Living && @object.IsLiving)
				{
					list.Add(@object as Living);
				}
			}
		}
		return list;
	}

	public List<Living> FindHitByHitPiont(Point p, int radius)
	{
		List<Living> list = new List<Living>();
		lock (_objects)
		{
			foreach (Physics @object in _objects)
			{
				if (@object is Living && @object.IsLiving && (@object as Living).BoundDistance(p) < (double)radius)
				{
					list.Add(@object as Living);
				}
			}
		}
		return list;
	}

	public Living FindNearestEnemy(int x, int y, double maxdistance, Living except)
	{
		Living result = null;
		lock (_objects)
		{
			foreach (Physics @object in _objects)
			{
				if (@object is Living && @object != except && @object.IsLiving && ((Living)@object).Team != except.Team)
				{
					double num = @object.Distance(x, y);
					if (num < maxdistance)
					{
						result = @object as Living;
						maxdistance = num;
					}
				}
			}
		}
		return result;
	}

	public List<Living> FindAllNearestEnemy(int x, int y, double maxdistance, Living except)
	{
		List<Living> list = new List<Living>();
		lock (_objects)
		{
			foreach (Physics @object in _objects)
			{
				if (@object is Living && @object != except && @object.IsLiving && ((Living)@object).Team != except.Team)
				{
					double num = @object.Distance(x, y);
					if (num < maxdistance)
					{
						list.Add(@object as Living);
						maxdistance = num;
					}
				}
			}
		}
		return list;
	}

	public void Dispose()
	{
		foreach (Physics @object in _objects)
		{
			@object.Dispose();
		}
	}

	public Map Clone()
	{
		Tile layer = ((_layer1 != null) ? _layer1.Clone() : null);
		Tile layer2 = ((_layer2 != null) ? _layer2.Clone() : null);
		return new Map(_info, layer, layer2);
	}
}
