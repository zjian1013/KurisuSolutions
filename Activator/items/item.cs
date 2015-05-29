using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Activator.Items
{
    public class item
    {
        internal virtual int Id { get; set; }
        internal virtual int Cooldown { get; set; }
        internal virtual string Name { get; set; }
        internal virtual string DisplayName { get; set; }
        internal virtual float Range { get; set; }
        internal virtual MenuType[] Category { get; set; }

        internal virtual int DefaultMP { get; set; }
        internal virtual int DefaultHP { get; set; }

        public Menu Menu { get; private set; }
        public Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        public Obj_AI_Base Target
        {
            get
            {
                return
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(h => h.IsValidTarget(Range) && !h.IsZombie)
                        .OrderBy(h => h.Distance(Game.CursorPos))
                        .FirstOrDefault();
            }
        }

        public void UseItem(bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                LeagueSharp.Common.Items.UseItem(Id);
            }
        }

        public void UseItem(Obj_AI_Base target, bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                LeagueSharp.Common.Items.UseItem(Id, target);
            }
        }

        public void UseItem(Vector3 pos, bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                LeagueSharp.Common.Items.UseItem(Id, pos);
            }
        }

        public item CreateMenu(Menu root)
        {
            var usefname = DisplayName ?? Name;

            Menu = new Menu(Name, "m" + Name);
            Menu.AddItem(new MenuItem("use" + Name, "Use " + usefname)).SetValue(true);

            if (Category.Any(t => t == MenuType.EnemyLowHP))
                Menu.AddItem(new MenuItem("EnemyLowHP" + Name + "Pct", "Use on Enemy HP % <="))
                    .SetValue(new Slider(DefaultHP));

            if (Category.Any(t => t == MenuType.SelfLowHP))
                Menu.AddItem(new MenuItem("SelfLowHP" + Name + "Pct", "Use on Hero HP % <="))
                    .SetValue(new Slider(Name == "Botrk" ? 70 : DefaultHP));

            if (Category.Any(t => t == MenuType.SelfMuchHP))
                Menu.AddItem(new MenuItem("SelfMuchHP" + Name + "Pct", "Use on Hero Dmg Dealt % >="))
                    .SetValue(new Slider(40));

            if (Category.Any(t => t == MenuType.SelfLowMP))
                Menu.AddItem(new MenuItem("SelfLowMP" + Name + "Pct", "Use on Hero Mana % <="))
                    .SetValue(new Slider(DefaultMP));

            if (Category.Any(t => t == MenuType.SelfCount))
                Menu.AddItem(new MenuItem("SelfCount" + Name, "Use On Enemy Count >="))
                    .SetValue(new Slider(3, 1, 5));

            if (Category.Any(t => t == MenuType.SelfMinMP))
                Menu.AddItem(new MenuItem("SelfMinMP" + Name + "Pct", "Minimum Mana %")).SetValue(new Slider(40));

            if (Category.Any(t => t == MenuType.SelfMinHP))
                Menu.AddItem(new MenuItem("SelfMinHP" + Name + "Pct", "Minimum HP %")).SetValue(new Slider(40));

            if (Category.Any(t => t == MenuType.Zhonyas))
            {
                Menu.AddItem(new MenuItem("use" + Name + "Norm", "Use on Dangerous (Spells)")).SetValue(false);
                Menu.AddItem(new MenuItem("use" + Name + "Ulti", "Use on Dangerous (Ultimates Only)")).SetValue(true);
            }

            if (Category.Any(t => t == MenuType.Cleanse))
            {
                Menu.AddItem(new MenuItem("use" + Name + "Number", "Minimum Spells to Use")).SetValue(new Slider(DefaultHP/5, 1, 5));
                Menu.AddItem(new MenuItem("use" + Name + "Time", "Minumum Durration to Use")).SetValue(new Slider(2, 1, 5)); ;
                Menu.AddItem(new MenuItem("use" + Name + "Od", "Use Only for Dangerous")).SetValue(false);
            }

            if (Category.Any(t => t == MenuType.ActiveCheck))
                Menu.AddItem(new MenuItem("mode" + Name, "Mode: "))
                    .SetValue(new StringList(new[] { "Always", "Combo" }, Id == 3222 ? 0 : 1));

            root.AddSubMenu(Menu);
            return this;
        }

        public virtual void OnTick()
        {

        }
    }
}
