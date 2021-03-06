﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using PurpleStyrofoam.Rendering;
using PurpleStyrofoam.Items.Weapons;
using System.Timers;
using PurpleStyrofoam.AiController.AIs;
using PurpleStyrofoam.Managers.Classes;
using PurpleStyrofoam.Helpers;

namespace PurpleStyrofoam.AiController
{
    public class PlayerController : AnimatedSprite
    {
        public bool InAir { get; private set; }

        public PlayerController(PlayerManager manager) 
            : base(PlayerManager.basePlayerSpriteName, 1, 1, 100, 100, new PlayerControlledAI(), manager)
        {
        }

        public override void Update()
        {
            // Check if player is colliding with something. This will set the directional variables
            DetectCollision();

            // Update the velocity and position of the player
            UpdateVelocity();

            // Updates player position
            SpriteRectangle.X += (int)(velocity.X * (float)Game.GameTimeSeconds);
            SpriteRectangle.Y += (int)(velocity.Y * (float)Game.GameTimeSeconds);

            // Update velocity if there is keyboard input
            CheckKeys();

            // Update player animation
            animate.Update();
            if (animate.Finished() && animate.Texture.Name == TextureHelper.Sprites.SmileyWalk) animate.Switch(PlayerManager.jumpingSPlayerSprite, SpriteRectangle, 1, 1);

            // Update the item position relative to player
            if (Game.PlayerManager.EquippedWeapon != null)
                Game.PlayerManager.EquippedWeapon.Sprite.ItemRectangle.Y = SpriteRectangle.Y + (SpriteRectangle.Width / 2);

            damageIndicator.Update(SpriteRectangle);
        }
        public override void DetectCollision()
        {
            Rectangle rect = new Rectangle(
                new Point(
                            SpriteRectangle.X + (int)(velocity.X * Game.GameTimeSeconds),
                            SpriteRectangle.Y + (int)(velocity.Y * Game.GameTimeSeconds)),
                new Point(SpriteRectangle.Width, SpriteRectangle.Height)); // Creates the projected rectangle

            bool[] array = CollisionDetection.DetectCollisionArrayMap(rect);
            North = array[0];
            South = array[1];
            East = array[2];
            West = array[3];
        }
        private const int moveSpeed = 20;
        public void CheckKeys()
        {
            if (South) InAir = false;
            if (RenderHandler.CurrentGameState == GAMESTATE.ACTIVE)
            {
                if (KeyHelper.CheckHeld(Keys.A))
                {
                    velocity.X -= velocity.X > -terminalVelocity.X ? moveSpeed : 0;
                    if (!InAir) animate.Switch(PlayerManager.movingPlayerSprite, SpriteRectangle);
                    if (!Game.PlayerManager.EquippedWeapon.Sprite.Visible) animate.Flipped = true;
                }
                if (KeyHelper.CheckHeld(Keys.D))
                {
                    velocity.X += velocity.X < terminalVelocity.X ? moveSpeed : 0;
                    if (!InAir) animate.Switch(PlayerManager.movingPlayerSprite, SpriteRectangle);
                    if (!Game.PlayerManager.EquippedWeapon.Sprite.Visible) animate.Flipped = false;
                }
                if (KeyHelper.CheckTap(Keys.Space))
                {
                    if (!InAir)
                    {
                        InAir = true;
                        velocity.Y -= terminalVelocity.Y;
                        animate.Switch(TextureHelper.Sprites.SmileyWalk, SpriteRectangle, 4, 4);
                    }
                }
                //if (newState.IsKeyDown(Keys.S)) { }
                if (KeyHelper.CheckTap(Keys.Q)){
                    ((PlayerManager)Manager).EquippedWeapon?.OnQAbility();
                }
                if (KeyHelper.CheckTap(Keys.E))
                {
                    ((PlayerManager) Manager).Class.EAction();
                }
                
            }
            if ((velocity.X > -1 && velocity.X < 1) && !InAir) animate.Switch(PlayerManager.basePlayerSpriteName, SpriteRectangle, 1,1);
        }
        public override void UpdateVelocity()
        {
            // Decrease Velocity

            if (North && velocity.Y < 0) velocity.Y = 0;
            else if (South && velocity.Y > 0) velocity.Y = 0;
            else velocity.Y -= velocity.Y < terminalVelocity.Y ? gravity : 0;


            if (East && velocity.X > 0) velocity.X = 0;
            else if (West && velocity.X < 0) velocity.X = 0;
            else velocity.X -= velocity.X < 0 ? -5 : velocity.X > 0 ? 5 : 0;
        }
    }
}
