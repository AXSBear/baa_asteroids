﻿using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;

namespace Asteroids.OGL.GameEngine.Managers
{
    /// <summary>
    ///     Input Manager for <see cref="M:OpenTK.GameWindow" />
    /// </summary>
    public static class InputManager
    {
        private static List<Key> KeysDown;
        private static List<Key> KeysDownLast;
        private static List<Key> KeysDownPrevious;
        
        /// <summary>
        ///     Initialize Input Manager
        /// </summary>
        /// <param name="game">Game window - source of input events</param>
        public static void Initialize(GameWindow game)
        {
            Console.WriteLine("Input Manager initialize...");
            KeysDown = new List<Key>();
            KeysDownLast = new List<Key>();
            
            game.KeyDown += GameOnKeyDown;
            game.KeyUp += GameOnKeyUp;
            
            Console.WriteLine("Input Manager initialize complete");
        }

        private static void GameOnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            while (KeysDown.Contains(e.Key))
            {
                KeysDown.Remove(e.Key);
            }
        }

        private static void GameOnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            KeysDown.Add(e.Key);
        }

        public static void Update()
        {
            KeysDownPrevious = new List<Key>(KeysDownLast);
            KeysDownLast = new List<Key>(KeysDown);
        }

        public static bool KeyPress(Key key)
        {
           return (KeysDown.Contains(key) && (!KeysDownLast.Contains(key) || !KeysDownPrevious.Contains(key)));
        }

        public static bool KeyRelease(Key key)
        {
            return (!KeysDown.Contains(key) && (KeysDownLast.Contains(key) || KeysDownPrevious.Contains(key)));
        }

        public static bool KeyDown(Key key)
        {
            return KeysDown.Contains(key);
        }
    }
}