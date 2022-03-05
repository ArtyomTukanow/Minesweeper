using System;
using Exceptions;
using UserData.TileMap;
using UserData.TileMap.Generators;
using NUnit.Framework;
using UnityEngine;

namespace Tests.UserData.CommandSystem
{
    public class OpenCommandTest
    {
        [Test]
        public void OpenAreaTiles()
        {
            /*
             Открываем поле:
             
             01##
             13##
             ####
             ####
        
             */
            
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            userTileMap.CommandSystem.Open(Vector2Int.zero);
            
            Assert.IsTrue(userTileMap[0, 0].Opened);
            Assert.IsTrue(userTileMap[1, 0].Opened);
            Assert.IsTrue(userTileMap[1, 1].Opened);
            Assert.IsTrue(userTileMap[0, 1].Opened);
            
            Assert.IsFalse(userTileMap[2, 0].Opened);
            Assert.IsFalse(userTileMap[2, 1].Opened);
            Assert.IsFalse(userTileMap[2, 2].Opened);
            Assert.IsFalse(userTileMap[1, 2].Opened);
            Assert.IsFalse(userTileMap[0, 2].Opened);
        }
        
        [Test]
        public void OpenAreaTiles2()
        {
            /*
             Открываем поле:
             
             011#
             13##
             ####
             ####
        
             */
            
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            userTileMap.CommandSystem.Open(new Vector2Int(0, 0), new Vector2Int(2, 0));
            
            Assert.IsTrue(userTileMap[0, 0].Opened);
            Assert.IsTrue(userTileMap[1, 0].Opened);
            Assert.IsTrue(userTileMap[1, 1].Opened);
            Assert.IsTrue(userTileMap[0, 1].Opened);
            
            Assert.IsTrue(userTileMap[2, 0].Opened);
            
            Assert.IsFalse(userTileMap[2, 1].Opened);
            Assert.IsFalse(userTileMap[2, 2].Opened);
            Assert.IsFalse(userTileMap[1, 2].Opened);
            Assert.IsFalse(userTileMap[0, 2].Opened);
        }
        
        [Test]
        public void OpenOnlyOneTile()
        {
            /*
             Открываем поле:
             
             #1##
             ####
             ####
             ####
        
             */
            
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            userTileMap.CommandSystem.Open(new Vector2Int(1, 0));
            
            Assert.IsFalse(userTileMap[0, 0].Opened);
            Assert.IsTrue(userTileMap[1, 0].Opened);
            Assert.IsFalse(userTileMap[2, 0].Opened);
            
            Assert.IsFalse(userTileMap[0, 1].Opened);
            Assert.IsFalse(userTileMap[1, 1].Opened);
            Assert.IsFalse(userTileMap[2, 1].Opened);
        }
        
        [Test]
        public void OpenOnlyOneTile2()
        {
            /*
             Открываем поле:
             
             ##1#
             ####
             ####
             ####
        
             */
            
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            userTileMap.CommandSystem.Open(new Vector2Int(2, 0));
            
            Assert.IsFalse(userTileMap[0, 0].Opened);
            Assert.IsFalse(userTileMap[1, 0].Opened);
            Assert.IsTrue(userTileMap[2, 0].Opened);
            
            Assert.IsFalse(userTileMap[0, 1].Opened);
            Assert.IsFalse(userTileMap[1, 1].Opened);
            Assert.IsFalse(userTileMap[2, 1].Opened);
        }
        
        [Test]
        public void OpenOnlyOneTile3()
        {
            /*
             Открываем поле:
             
             #11#
             ####
             ####
             ####
        
             */
            
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            userTileMap.CommandSystem.Open(new Vector2Int(1, 0), new Vector2Int(2, 0));
            
            Assert.IsFalse(userTileMap[0, 0].Opened);
            Assert.IsTrue(userTileMap[1, 0].Opened);
            Assert.IsTrue(userTileMap[2, 0].Opened);
            
            Assert.IsFalse(userTileMap[0, 1].Opened);
            Assert.IsFalse(userTileMap[1, 1].Opened);
            Assert.IsFalse(userTileMap[2, 1].Opened);
        }
        
        [Test]
        public void WaitThrowExceptionOnTheBombPoint()
        {
            /*
             Получаем ошибку при открытии поля:
             
             ####
             ##*#
             ####
             ####
        
             */
            
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            try
            {
                userTileMap.CommandSystem.Open(new Vector2Int(2, 1));
                Assert.Fail("Command must be created with an exception!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(InvalidCommandException));
            }
        }
        
        [Test]
        public void WaitThrowExceptionOnTheInvalidPoint()
        {
            /*
             Получаем ошибку при открытии поля:
            
            *####
             ####
             ####
             ####
        
             */
            
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            try
            {
                userTileMap.CommandSystem.Open(new Vector2Int(-1, 0));
                Assert.Fail("Command must be created with an exception!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(InvalidCommandException));
            }
        }
        
        [Test]
        public void WaitThrowExceptionOnTheDoubleOpenPoint()
        {
            /*
             Получаем ошибку при двойном открытии поля:
             
             01##
             13##
             ####
             ####
        
             */
            
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            
            try
            {
                userTileMap.CommandSystem.Open(Vector2Int.zero);
                userTileMap.CommandSystem.Open(Vector2Int.zero);
                Assert.Fail("Command must be created with an exception!");
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.GetType(), typeof(InvalidCommandException));
            }
        }
    }
}