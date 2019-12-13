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
    class Flight : Weapon
    {
        public override int ID => 003;

        public override string Description => "Flight, a legendary sword given to humans by the god of wind himself. It is able to be swung without fatigue, and can control the very wind itself.";
        public Flight(ContentManager content) : base("Flight", 100, ATTACKSPEED.FAST, RARITY.LEGENDARY, new ItemSprite(Game.GameContent.Load<Texture2D>("playerSprite"), new Vector2(0,0))) { }
        public Flight() : base("Flight", 100, ATTACKSPEED.FAST, RARITY.LEGENDARY, new ItemSprite(Game.GameContent.Load<Texture2D>("playerSprite"), new Vector2(0,0)))
        {

        }
        public override void OnInventoryUse()
        {
            throw new NotImplementedException();
        }

        public override void OnLeftClick()
        {
            AnimatedSprite temp = RenderHandler.allCharacterSprites.Find(x => x.GetType().Name.Equals("PlayerController"));

        }

        public override void OnQAbility()
        {
            // Increase attack speed and attacks during this period throw an extra slash of wind that pierce infinitely and travels until it hits a wall/floor.
            throw new NotImplementedException();
        }

    }
}
