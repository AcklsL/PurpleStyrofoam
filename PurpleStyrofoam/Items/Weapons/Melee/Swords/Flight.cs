﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PurpleStyrofoam.Rendering;

namespace PurpleStyrofoam.Items.Weapons.Melee.Swords
{
    class Flight : Sword
    {
        public override int ID => 003;

        public override string Description => "Flight, a legendary sword given to humans by the god of wind himself. It is able to be swung without fatigue, and can control the very wind itself.";
        public Flight() : base("Flight", 100, RARITY.SECRET, new ItemSprite("Swords/Flight", 60, 60))
        {
            KnockBack = 1f;
        }

        public override void OnQAbility()
        {
            // Increase attack speed and attacks during this period throw an extra slash of wind that pierce infinitely and travels until it hits a wall/floor.
            throw new NotImplementedException();
        }
    }
}
