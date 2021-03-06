﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PurpleStyrofoam.AiController;
using PurpleStyrofoam.Helpers;
using PurpleStyrofoam.Items.Weapons.Melee.Swords;
using PurpleStyrofoam.Managers;
using PurpleStyrofoam.Managers.Classes;
using PurpleStyrofoam.Maps;
using PurpleStyrofoam.Maps.Dungeon_Areas;
using PurpleStyrofoam.Rendering.Menus;
using PurpleStyrofoam.Rendering.Menus.PopUpMenu;
using PurpleStyrofoam.Rendering.Sprites;
using PurpleStyrofoam.Rendering.Text;

namespace PurpleStyrofoam.Rendering
{
    /// <summary>
    /// The main engine of the game. Handles all update method calls, all draw method calls, and holds all object lists.
    /// </summary>
    static class RenderHandler
    {
        public static List<AnimatedSprite> allCharacterSprites { get; private set; }
        public static List<Projectile> allProjectiles { get; private set; }

        /// <summary>
        /// The projectiles to be deleted. Needed in order to circumvent editing lists mid-iteration.
        /// </summary>
        public static List<Projectile> purgeProjectiles { get; private set; }
        /// <summary>
        /// The sprites to be deleted. Needed in order to circumvent editing lists mid-iteration.
        /// </summary>
        public static List<AnimatedSprite> purgeSprites { get; private set; }
        public static BaseMap selectedMap;
        /// <summary>
        /// Active GameState. Used to differentiate pausing and active gameplay
        /// </summary>
        public static GAMESTATE CurrentGameState { get; set; }

        /// <summary>
        /// Initializes RenderHandler at the beginning of the game.
        /// </summary>
        public static void Initialize()
        {
            allCharacterSprites = new List<AnimatedSprite>();
            allProjectiles = new List<Projectile>();
            purgeProjectiles = new List<Projectile>();
            purgeSprites = new List<AnimatedSprite>();
            MenuHandler.Initialize();
            CurrentGameState = GAMESTATE.MAINMENU;
        }

        /// <summary>
        /// Changes the game state. Primarily used to change the map.
        /// </summary>
        /// <param name="newMap">The map to be changed to</param>
        /// <param name="player">The player information to be handed in</param>
        /// <param name="newX">The new player position</param>
        /// <param name="newY">The new player position</param>
        public static void InitiateChange(BaseMap newMap, PlayerController player, Point pos, List<AnimatedSprite> newSprites = null)
        {
            Game.PlayerCharacter = player;
            Game.PlayerManager = (PlayerManager)Game.PlayerCharacter.Manager;

            allCharacterSprites.Clear();
            allProjectiles.Clear();

            selectedMap = newMap;
            ObjectMapper.MapObjects(selectedMap);
            if (newSprites != null) allCharacterSprites.AddRange(newSprites);
            else if (selectedMap.sprites != null) allCharacterSprites.AddRange(selectedMap.sprites);
            if (!allCharacterSprites.Contains(player)) allCharacterSprites.Add(player);


            LoadGameTextures();
            PlayerInfoUI.Initialize();
            if (Game.PlayerManager.Inventory == null) Game.PlayerManager.Inventory = new InventoryManager();
            if (Game.PlayerManager.EquippedWeapon != null) Game.PlayerManager.Inventory.Items[0] = Game.PlayerManager.EquippedWeapon;
            Game.PlayerManager.Inventory.LoadItems();
            MenuHandler.AllPopUps.RemoveAll((x) => x is InventoryManager);
            MenuHandler.AllPopUps.Add(Game.PlayerManager.Inventory);

            Game.PlayerCharacter.SpriteRectangle.Location = pos;
        }

        private static void LoadGameTextures()
        {
            foreach (AnimatedSprite i in allCharacterSprites) i.Load();
            foreach (MapObject i in selectedMap.BackgroundLayer) i.Load();
            foreach (MapObject i in selectedMap.ActiveLayer) i.Load();
            foreach (MapObject i in selectedMap.ForegroundLayer) i.Load();
            if (Game.PlayerManager.EquippedWeapon != null) Game.PlayerManager.EquippedWeapon.Sprite.Load();
        }



        /// <summary>
        /// Excess MapObjects being created that weren't included in original map definition
        /// </summary>
        public static List<MapObject> extras = new List<MapObject>();

        /// <summary>
        /// Primary update method. Runs update method calls when needed.
        /// </summary>
        /// <seealso cref="CurrentGameState"/>
        public static void Update()
        {
            switch (CurrentGameState)
            {
                case GAMESTATE.ACTIVE:
                    MenuHandler.CheckKeys();
                    foreach (AnimatedSprite sprite in allCharacterSprites)
                    {
                        sprite.Update();
                        sprite.Buffs.Update();
                        ObjectMapper.DeleteSpriteObject(sprite);
                        ObjectMapper.AddSpriteObject(sprite);
                    }
                    foreach (Projectile item in allProjectiles) item.Update();
                    foreach (MapInteractable i in selectedMap.InteractableLayer) i.MapAction?.Invoke();
                    foreach (TimerHelper t in TimerHelper.Timers) t.Update();
                    if (Game.PlayerManager.EquippedWeapon != null) Game.PlayerManager.EquippedWeapon.Sprite.Update();
                    if (purgeProjectiles.Count != 0) DeleteProjectiles();
                    if (purgeSprites.Count != 0) DeleteSprites();
                    PlayerInfoUI.Update();
                    break;
                case GAMESTATE.MAINMENU:
                    break;
                case GAMESTATE.PAUSED:
                    MenuHandler.CheckKeys();
                    if (DialogueHandler.ActiveDialogue != null) DialogueHandler.Update();
                    else MenuHandler.Update();
                    PlayerInfoUI.Update();
                    break;
                default:
                    throw new NotSupportedException("Game has entered an invalid gamestate: " + CurrentGameState);
            }
        }

        /// <summary>
        /// Removes sprites listed in the <c>purgeSprites</c> list
        /// </summary>
        public static void DeleteSprites()
        {
            foreach (AnimatedSprite sprite in purgeSprites)
            {
                allCharacterSprites.Remove(sprite);
                ObjectMapper.DeleteSpriteObject(sprite);
            }
            purgeSprites.Clear();
        }

        /// <summary>
        /// Removes projectiles listed in the <c>purgeProjectiles</c> list
        /// </summary>
        public static void DeleteProjectiles()
        {
            foreach (Projectile proj in purgeProjectiles)
            {
                allProjectiles.Remove(proj);
            }
            purgeProjectiles.Clear();
        }

        public static Vector2 ScreenOffset = new Vector2(0, 0);
        private static int XOffset = (int) Game.ScreenSize.X/2;
        private static int YOffset = (int) Game.ScreenSize.Y/2;

        /// <summary>
        /// Primary Draw method. Calls all draw methods required for game
        /// </summary>
        /// <param name="sp">The <c>SpriteBatch</c> given from the Game class.</param>
        public static void Draw(SpriteBatch sp)
        {
            int xMove = 0;
            int yMove = 0;
            switch (CurrentGameState)
            {
                case GAMESTATE.ACTIVE:
                    ScreenOffset.X = (Game.PlayerCharacter.SpriteRectangle.X) - XOffset;
                    ScreenOffset.Y = (Game.PlayerCharacter.SpriteRectangle.Y) - YOffset;
                    FindOffset(out xMove, out yMove);
                    sp.Begin(SpriteSortMode.Deferred, transformMatrix: Matrix.CreateTranslation(xMove, yMove, 0));
                    selectedMap.DrawMap(sp, (spr) => {
                        foreach (MapObject i in extras) i.Draw(spr);
                        foreach (AnimatedSprite item in allCharacterSprites) item.Draw(spr);
                        foreach (Projectile item in allProjectiles) item.Draw(spr);
                        if (Game.PlayerManager.EquippedWeapon != null) Game.PlayerManager.EquippedWeapon.Sprite.Draw(spr);
                    });
                    PlayerInfoUI.Draw(sp);
                    break;
                case GAMESTATE.MAINMENU:
                    sp.Begin(SpriteSortMode.Deferred);
                    if (MenuHandler.ActiveFullScreenMenu != null) MenuHandler.DrawFullScreenMenu(sp);
                    break;
                case GAMESTATE.PAUSED:
                    ScreenOffset.X = (Game.PlayerCharacter.SpriteRectangle.X) - XOffset;
                    ScreenOffset.Y = (Game.PlayerCharacter.SpriteRectangle.Y) - YOffset;
                    FindOffset(out xMove, out yMove);
                    sp.Begin(SpriteSortMode.Deferred, transformMatrix: Matrix.CreateTranslation(xMove, yMove, 0));
                    selectedMap.DrawMap(sp, (spr) => {
                        foreach (MapObject i in extras) i.Draw(spr);
                        foreach (AnimatedSprite item in allCharacterSprites) item.Draw(spr);
                        foreach (Projectile item in allProjectiles) item.Draw(spr);
                        if (Game.PlayerManager.EquippedWeapon != null) Game.PlayerManager.EquippedWeapon.Sprite.Draw(spr);
                    });
                    if (DialogueHandler.ActiveDialogue != null) DialogueHandler.Draw(sp);
                    if (MenuHandler.ActivePopUp != null) MenuHandler.DrawPopUpMenu(sp);
                    PlayerInfoUI.Draw(sp);
                    break;
                default:
                    throw new NotSupportedException("Game has entered an invalid gamestate: " + CurrentGameState);
            }
            sp.End();
        }

        private static void FindOffset(out int XMovement, out int YMovement)
        {
            if (ScreenOffset.X <= selectedMap.maxBounds.Left)
            {
                XMovement = selectedMap.maxBounds.Left;
                ScreenOffset.X = 0;
            } else if (ScreenOffset.X >= selectedMap.maxBounds.Right)
            {
                XMovement = selectedMap.maxBounds.Right;
                ScreenOffset.X = selectedMap.maxBounds.Right - Game.ScreenSize.X;
            } else XMovement = (-Game.PlayerCharacter.SpriteRectangle.X) + XOffset;


            if (ScreenOffset.Y <= selectedMap.maxBounds.Top)
            {
                YMovement = selectedMap.maxBounds.Top;
                ScreenOffset.Y = 0;
            } else if (ScreenOffset.Y >= selectedMap.maxBounds.Bottom)
            {
                YMovement = selectedMap.maxBounds.Bottom;
                ScreenOffset.Y = selectedMap.maxBounds.Bottom - Game.ScreenSize.Y;
            } else YMovement = (-Game.PlayerCharacter.SpriteRectangle.Y) + YOffset;
        }
    }

    /// <summary>
    /// Data type for game's current state
    /// </summary>
    enum GAMESTATE
    {
        MAINMENU, ACTIVE, PAUSED
    }
}
