using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Base.Events;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using log4net;

namespace Game.Logic.Spells;

public class SpellMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, ISpellHandler> handles = new Dictionary<int, ISpellHandler>();

	public static ISpellHandler LoadSpellHandler(int code)
	{
		return handles[code];
	}

	[ScriptLoadedEvent]
	public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
	{
		handles.Clear();
		int num = SearchSpellHandlers(Assembly.GetAssembly(typeof(BaseGame)));
		if (log.IsInfoEnabled)
		{
			log.Info("SpellMgr: Loaded " + num + " spell handlers from GameServer Assembly!");
		}
	}

	protected static int SearchSpellHandlers(Assembly assembly)
	{
		int num = 0;
		Type[] types = assembly.GetTypes();
		foreach (Type type in types)
		{
			if (type.IsClass && type.GetInterface("Game.Logic.Spells.ISpellHandler") != null)
			{
				SpellAttibute[] array = (SpellAttibute[])type.GetCustomAttributes(typeof(SpellAttibute), inherit: true);
				if (array.Length > 0)
				{
					num++;
					RegisterSpellHandler(array[0].Type, Activator.CreateInstance(type) as ISpellHandler);
				}
			}
		}
		return num;
	}

	protected static void RegisterSpellHandler(int type, ISpellHandler handle)
	{
		handles.Add(type, handle);
	}

	public static void ExecuteSpell(BaseGame game, Player player, ItemTemplateInfo item)
	{
		try
		{
			ISpellHandler spellHandler = LoadSpellHandler(item.Property1);
			spellHandler.Execute(game, player, item);
		}
		catch (Exception exception)
		{
			log.Error("Execute Spell Error:", exception);
		}
	}
}
