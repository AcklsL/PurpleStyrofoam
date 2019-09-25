﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PurpleStyrofoam.Rendering;

namespace PurpleStyrofoam.Items.Weapons.Melee.Swords
{
    class Flight : Weapon
    {
        public override int Damage => 100;

        public override ATTACKSPEED AttackSpeed { get; set; }
        public override string Name { get; set; }

        public override int ID => 003;

        public override string Description => "Flight, a legendary sword given to humans by the god of wind himself. It is able to be swung without fatigue, and can control the very wind itself.";

        public override RARITY Rarity => RARITY.LEGENDARY;

        public override ImageHandler image { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Flight()
        {
            Name = "Flight";
            AttackSpeed = ATTACKSPEED.FAST;
        }

        public override void OnInventoryUse()
        {
            throw new NotImplementedException();
        }

        public override void OnLeftClick()
        {
            throw new NotImplementedException();
        }

        public override void OnQAbility()
        {
            // Increase attack speed and attacks during this period throw an extra slash of wind that pierce infinitely and travels until it hits a wall/floor.
            throw new NotImplementedException();
        }

        public override void OnRightClick()
        {
            throw new NotImplementedException();
        }
    }
}