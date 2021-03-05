using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using TileGame;

namespace Tests
{
    public class BoardTests
    {
        // BOARD SETUP TESTS

        [Test]
        public void TestBoardCreation()
        {
            Board testBoard = new Board(1, 1);
            Assert.True(testBoard.board[0, 0].ID == TileID.EmptyTile);
        }

        // EMPTY TILE TESTS

        [Test]
        public void TestExitedEmptyTileBecomesFalse()
        {
            Board testBoard = new Board(1, 1);
            testBoard.board[0, 0] = new EmptyTile(SimpleVector.Right(), true);
            testBoard.Update();
            Assert.False(testBoard.board[0, 0].HasSignal());
        }

        [Test]
        public void TestSignalMovesAcrossEmptyTiles()
        {
            Board testBoard = new Board(3,1);
            testBoard.board[0, 0] = new EmptyTile(SimpleVector.Right(), true);
            testBoard.Update();
            testBoard.Update();
            Assert.True(testBoard.board[2, 0].HasSignal());
        }

        [Test]
        public void TestSignalCollidesWithOtherSignal()
        {
            Board testBoard = new Board(3, 1);
            testBoard.board[0, 0] = new EmptyTile(SimpleVector.Right(), true);
            testBoard.board[2, 0] = new EmptyTile(SimpleVector.Left(), true);
            testBoard.Update();
            Assert.False(testBoard.board[1, 0].HasSignal());
        }

        [Test]
        public void TestSignalCollidesWithOtherSignals()
        {
            Board testBoard = new Board(3, 3);
            testBoard.board[0, 1] = new EmptyTile(SimpleVector.Right(), true);
            testBoard.board[2, 1] = new EmptyTile(SimpleVector.Left(), true);
            testBoard.board[1, 0] = new EmptyTile(SimpleVector.Down(), true);
            testBoard.Update();
            Assert.False(testBoard.board[1, 1].HasSignal());
        }

        // SPLITTER TESTS

        [Test]
        public void TestSplitterSplits()
        {
            Board testBoard = new Board(3, 3);
            testBoard.board[0, 1] = new EmptyTile(SimpleVector.Right(), true);
            testBoard.board[1, 1] = new Splitter(false);
            testBoard.Update();
            testBoard.Update();
            Assert.True(testBoard.board[0, 1].HasSignal() && testBoard.board[2, 1].HasSignal() && testBoard.board[1, 0].HasSignal() && testBoard.board[1, 2].HasSignal());
        }

        [Test]
        public void TestSplitterGenerator()
        {
            Board testBoard = new Board(3, 2);
            testBoard.board[0, 0] = new Splitter(true);
            testBoard.board[1, 0] = new Splitter(true);
            testBoard.board[2, 0] = new Splitter(true);

            for(int i=0; i<20; i++)
            {
                testBoard.Update();
            }
            
            Assert.True(testBoard.board[1,1].HasSignal());
        }

        [Test]
        public void TestSplitterCollisionsDisabled()
        {
            Board testBoard = new Board(3, 3);
            testBoard.board[0, 1] = new EmptyTile(SimpleVector.Right(), true);
            testBoard.board[2, 1] = new EmptyTile(SimpleVector.Left(), true);
            testBoard.board[1, 0] = new EmptyTile(SimpleVector.Down(), true);
            testBoard.board[1, 1] = new Splitter(false);
            testBoard.Update();
            Assert.True(testBoard.board[1, 1].HasSignal());
        }

        // REDIRECTOR TESTS

        public void TestRedirectorRedirects()
        {
            Board testBoard = new Board(2, 2);
            testBoard.board[0, 0] = new EmptyTile(SimpleVector.Right(), true);
            testBoard.board[1, 0] = new Redirector(SimpleVector.Down(), false);
            testBoard.Update();
            testBoard.Update();
            Assert.True(testBoard.board[0, 1].HasSignal());
        }

        [Test]
        public void TestRedirectorCollisions()
        {
            Board testBoard = new Board(3, 3);
            testBoard.board[0, 1] = new EmptyTile(SimpleVector.Right(), true);
            testBoard.board[2, 1] = new EmptyTile(SimpleVector.Left(), true);
            testBoard.board[1, 0] = new EmptyTile(SimpleVector.Down(), true);
            testBoard.board[1, 1] = new Redirector(SimpleVector.Down(), false);
            testBoard.Update();
            Assert.False(testBoard.board[1, 1].HasSignal());
        }

        [Test]
        public void TestRedirectorChains()
        {
            Board testBoard = new Board(2, 1);
            testBoard.board[0, 0] = new Redirector(SimpleVector.Right(), true);
            testBoard.board[1, 0] = new Redirector(SimpleVector.Left(), false);
            testBoard.Update();
            testBoard.Update();
            testBoard.Update();
            testBoard.Update();
            Assert.True(testBoard.board[0, 0].HasSignal());
        }

        // WALL TESTS
        
        [Test]
        public void TestWallBlocksSignals()
        {
            Board testBoard = new Board(3, 1);
            testBoard.board[0, 0] = new EmptyTile(SimpleVector.Right(), true);
            testBoard.board[1, 0] = new Wall();
            testBoard.Update();
            testBoard.Update();
            Assert.False(testBoard.board[2, 0].HasSignal());
        }

        [Test]
        public void TestWallCannotBeSetToHigh()
        {
            Board testBoard = new Board(1, 1);
            testBoard.board[0, 0] = new Wall();
            testBoard.board[0, 0].TrySetHighState();
            Assert.False(testBoard.board[0, 0].HasSignal());
        }

        // JUMPER TESTS

        [Test]
        public void TestJumperAllowsPerpendicularSignalsToOverlap()
        {
            Board testBoard = new Board(3, 3);
            testBoard.board[1, 0] = new EmptyTile(SimpleVector.Down(), true);
            testBoard.board[0, 1] = new EmptyTile(SimpleVector.Right(), true);
            testBoard.board[1, 1] = new Jumper(false, false);
            testBoard.Update();
            testBoard.Update();
            Assert.True(testBoard.board[1, 2].HasSignal() && testBoard.board[2, 1].HasSignal());
        }

        [Test]
        public void TestJumperCollidesParallelSignals()
        {
            Board testBoard = new Board(3, 3);
            testBoard.board[0, 1] = new EmptyTile(SimpleVector.Right(), true);
            testBoard.board[2, 1] = new EmptyTile(SimpleVector.Left(), true);
            testBoard.board[1, 0] = new EmptyTile(SimpleVector.Down(), true);
            testBoard.board[1, 2] = new EmptyTile(SimpleVector.Up(), true);
            testBoard.board[1, 1] = new Jumper(false, false);
            testBoard.Update();
            Assert.False(testBoard.board[1, 1].HasSignal());
        }
    }
}
