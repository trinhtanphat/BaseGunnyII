using System.Collections.Generic;
using Game.Logic.Actions;

namespace Game.Logic.Phy.Object;

public class PhysicalObj : Physics
{
	private string m_model;

	private string m_currentAction;

	private int m_scale;

	private int m_rotation;

	private BaseGame m_game;

	private bool m_canPenetrate;

	private int m_type;

	private string m_name;

	private int m_phyBringToFront;

	private int m_typeEffect;

	private Dictionary<string, string> m_actionMapping;

	public virtual int phyBringToFront => m_phyBringToFront;

	public virtual int Type => m_type;

	public int typeEffect => m_typeEffect;

	public string Model => m_model;

	public Dictionary<string, string> ActionMapping => m_actionMapping;

	public string CurrentAction
	{
		get
		{
			return m_currentAction;
		}
		set
		{
			m_currentAction = value;
		}
	}

	public int Scale => m_scale;

	public int Rotation => m_rotation;

	public bool CanPenetrate
	{
		get
		{
			return m_canPenetrate;
		}
		set
		{
			m_canPenetrate = value;
		}
	}

	public string Name => m_name;

	public PhysicalObj(int id, string name, string model, string defaultAction, int scale, int rotation)
		: base(id)
	{
		m_name = name;
		m_model = model;
		m_currentAction = defaultAction;
		m_scale = scale;
		m_rotation = rotation;
		m_canPenetrate = false;
		m_typeEffect = 0;
		switch (name)
		{
		case "hide":
			m_phyBringToFront = 6;
			break;
		case "top":
			m_phyBringToFront = 1;
			break;
		default:
			m_phyBringToFront = -1;
			break;
		}
		m_actionMapping = new Dictionary<string, string>();
		if (model == "asset.game.transmitted")
		{
			m_type = 3;
		}
		else if (model == "asset.game.six.ball")
		{
			if (!m_actionMapping.ContainsKey(defaultAction))
			{
				m_actionMapping.Add(defaultAction, getActionMap(defaultAction));
			}
		}
		else
		{
			m_type = 0;
		}
	}

	private string getActionMap(string act)
	{
		return act switch
		{
			"s1" => "shield1",
			"s2" => "shield2",
			"s3" => "shield3",
			"s4" => "shield4",
			"s5" => "shield5",
			"s6" => "shield6",
			"s-1" => "shield-1",
			"s-2" => "shield-2",
			"s-3" => "shield-3",
			"s-4" => "shield-4",
			"s-5" => "shield-5",
			"s-6" => "shield-6",
			"double" => "shield-double",
			_ => act,
		};
	}

	public void SetGame(BaseGame game)
	{
		m_game = game;
	}

	public void PlayMovie(string action, int delay, int movieTime)
	{
		if (m_game != null)
		{
			m_game.AddAction(new PhysicalObjDoAction(this, action, delay, movieTime));
		}
	}

	public override void CollidedByObject(Physics phy)
	{
		if (!m_canPenetrate && phy is SimpleBomb)
		{
			((SimpleBomb)phy).Bomb();
		}
	}
}
