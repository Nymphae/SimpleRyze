using System;
using System.Linq;
using System.Threading;
using LeagueSharp;
using SharpDX;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using Color = System.Drawing.Color;


namespace ConsoleApplication6
{
    internal class Program
    {

        private static Obj_AI_Hero Player = ObjectManager.Player;
        private static String championName = "Ryze";
        private static Menu _Menu;
        private static Orbwalking.Orbwalker _orbwalker;
        private static Menu TSMenu;
        private static Spell Q, W, E, R;
        private static Items.Item _seraphItem;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            {
                if (Player.ChampionName != championName)

                    return;
            }
            Q = new Spell(SpellSlot.Q, 625);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R);

            _seraphItem = new Items.Item(3040);

            _Menu = new Menu("Simple Ryze", "ryze.mainmenu", true);
            TSMenu = new Menu("Target Selecotr", "target.selector");
            TargetSelector.AddToMenu(TSMenu);
            _Menu.AddSubMenu(TSMenu);
            _orbwalker = new Orbwalking.Orbwalker(_Menu.SubMenu("Orbwalking"));
            var comboMenu = new Menu("Combo", "kek.ryze.combo");
            {
                comboMenu.AddItem(new MenuItem("kek.ryze.combo.useq", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("kek.ryze.combo.usew", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("kek.ryze.combo.usee", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("kek.ryze.combo.user", "Use R").SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("kek.ryze.combo.ultset", "Use Ult Below % HP").SetValue(new Slider(60, 1, 100)));


            }

            var drawMenu = new Menu("Draw Settings", "kek.ryze.draw");
            {
                drawMenu.AddItem(new MenuItem("kek.ryze.drawQrange", "Q Draw").SetValue(true));
                drawMenu.AddItem(new MenuItem("kek.ryze.drawWrange", "W Draw").SetValue(true));
            }

            var itemMenu = new Menu("Items", "kek.ryze.items");
            {
                itemMenu.AddItem(
                    new MenuItem("kek.ryze.item.useSeraph", "Hp to use Seraphs").SetValue(new Slider(30, 1, 100)));
                itemMenu.AddItem(new MenuItem("kek.ryze.items.useseraphs", "Use Seraphs Embrace").SetValue(true));
            }
            _Menu.AddSubMenu(itemMenu);
            _Menu.AddSubMenu(drawMenu);
            _Menu.AddSubMenu(comboMenu);
            _Menu.AddToMainMenu();
            Game.OnGameUpdate += Game_OnGameUpdate;
            LeagueSharp.Drawing.OnDraw += Drawing;
            Game.PrintChat("Loading Nymphae's Simple Ryze");
        }


        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                DespPower();
                RunePrison();
                Overload();
                SpellFlux();
                Seraphs();

            }
        }

        private static void Drawing(EventArgs args)
        {
            if (_Menu.Item("kek.ryze.drawQrange").GetValue<bool>())
            {
                if (Q.IsReady())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.DarkCyan);

                }

            }
            if (_Menu.Item("kek.ryze.drawWrange").GetValue<bool>())
            {
                if (W.IsReady())
                {
                    Render.Circle.DrawCircle(Player.Position, W.Range, Color.LightYellow);

                }

            }

        }

        private static void DespPower()
        {
            if (!_Menu.Item("kek.ryze.combo.user").GetValue<bool>())
                return;
            if (R.IsReady())
            {
                int sliderValue = _Menu.Item("kek.ryze.combo.ultset").GetValue<Slider>().Value;
                float healthPercent = Player.Health / Player.MaxHealth * 100;
                if (healthPercent < sliderValue)
                    R.CastOnUnit(Player);
            }
        }

        private static void Overload()
        {
            if (!_Menu.Item("kek.ryze.combo.useq").GetValue<bool>())
                return;
            Obj_AI_Hero target = TargetSelector.GetTarget(625, TargetSelector.DamageType.Magical);
            if (Q.IsReady())
            {
                if (target.IsValidTarget(E.Range))
                {
                    Q.CastOnUnit(target);
                }
            }
        }

        private static void RunePrison()
        {
            if (!_Menu.Item("kek.ryze.combo.usew").GetValue<bool>())
                return;
            Obj_AI_Hero target = TargetSelector.GetTarget(600, TargetSelector.DamageType.Magical);
            if (W.IsReady())
            {
                if (target.IsValidTarget())
                {
                    W.CastOnUnit(target);
                }
            }
        }

        private static void SpellFlux()
        {
            if (!_Menu.Item("kek.ryze.combo.usee").GetValue<bool>())
                return;
            Obj_AI_Hero target = TargetSelector.GetTarget(600, TargetSelector.DamageType.Magical);
            if (E.IsReady())
            {
                if (target.IsValidTarget())
                {
                    E.CastOnUnit(target);
                }
            }
        }

        private static void Seraphs()
        {
            var seraphuse = _Menu.Item("kek.ryze.item.useseraphs").GetValue<bool>();

            if (!_Menu.Item("kek.ryze.item.useSeraphs").GetValue<bool>())

            return;


            if (seraphuse)
            {
                int sliderValue = _Menu.Item("kek.ryze.item.useSeraph").GetValue<Slider>().Value;
                float healthPercent = Player.Health / Player.MaxHealth * 100;
                if (healthPercent < sliderValue)
                    Items.UseItem(3040);
            }
        }
    }

}


