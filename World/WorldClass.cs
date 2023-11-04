using GameTemplate.World.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GameTemplate.World
{
    public class WorldClass
    {
        public static List<WorldObject> activeWorldObjects;
        public static WorldData activeWorldData;
        public static Tile[,] activeWorldChunk;
        public static WorldDetails worldDetails;

        public static int CurrentWorldWidth;
        public static int CurrentWorldHeight;

        public static void GenerateWorld(int width, int height, int baseCoordinate, int heightVariance, int heightThreshold)
        {
            Tile[,] worldTiles = new Tile[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    worldTiles[x, y] = Tile.GetTileInfo(Tile.TileType.None, new Vector2(x, y) * 16f);
                }
            }
            Main.camera.bounds = new Vector2(width, height) * 16;

            int baseYCoordinate = height - baseCoordinate + Main.random.Next(-heightVariance, heightVariance + 1);
            int currentYCoordinate = baseYCoordinate;
            for (int x = 0; x < width; x++)
            {
                int segmentLength = Main.random.Next(3, 18 + 1);
                if (x + segmentLength > width)
                    segmentLength = width - x;

                for (int x2 = 0; x2 < segmentLength; x2++)
                {
                    Point tileCoordinates = new Point(x + x2, currentYCoordinate);
                    worldTiles[tileCoordinates.X, tileCoordinates.Y] = Tile.GetTileInfo((Tile.TileType)Main.random.Next((int)Tile.TileType.Grass, (int)Tile.TileType.Grass + 1), tileCoordinates.ToVector2() * 16f);
                    for (int y = 1; y < height - currentYCoordinate; y++)
                    {
                        tileCoordinates.Y = height - y;
                        Tile.TileType tileType = Tile.TileType.Dirt;
                        if (height - (currentYCoordinate + y) == 3)
                            tileType = Tile.TileType.DirtToUndergroundDirt;
                        else if (height - (currentYCoordinate + y) >= 4)
                            tileType = Tile.TileType.UndergroundDirt;

                        worldTiles[tileCoordinates.X, tileCoordinates.Y] = Tile.GetTileInfo(tileType, tileCoordinates.ToVector2() * 16f);
                    }
                }

                x += segmentLength - 1;
                currentYCoordinate += Main.random.Next(-heightVariance, heightVariance + 1);
                while (Math.Abs(baseYCoordinate - currentYCoordinate) >= heightThreshold)
                {
                    currentYCoordinate += Main.random.Next(-heightVariance, heightVariance + 1);
                }
            }

            WorldData worldData = new WorldData(0, width, height);
            worldData.tiles = worldTiles;
            activeWorldData = worldData;
            activeWorldObjects = new List<WorldObject>();
            CurrentWorldWidth = width;
            CurrentWorldHeight = height;
            CleanupWorld();
            GenerateExtraWorldFeatures(baseYCoordinate);
            ChunkLoader.ResetChunkDimensions();
            activeWorldChunk = new Tile[ChunkLoader.ChunkSizeWidth, ChunkLoader.ChunkSizeHeight];
            if (Main.currentPlayer != null)
            {
                Main.currentPlayer.position = new Vector2(5 * 16, (baseYCoordinate - 4) * 16f);
                ChunkLoader.ForceUpdateActiveWorldChunk(Main.currentPlayer.position);
            }
            worldDetails = new WorldDetails();
            worldDetails.Initialize();
        }

        public static void GenerateExtraWorldFeatures(int baseYCoordinate)
        {

        }

        public static void CleanupWorld()
        {
            for (int x = 0; x < activeWorldData.dimensions.X; x++)
            {
                for (int y = 0; y < activeWorldData.dimensions.Y; y++)
                {
                    Tile currentTile = activeWorldData.tiles[x, y];
                    if (x > 1 && x < activeWorldData.dimensions.X - 1)
                    {
                        Tile leftTile = activeWorldData.tiles[x - 1, y];
                        Tile rightTile = activeWorldData.tiles[x + 1, y];
                        if (currentTile.tileType == Tile.TileType.Grass)
                        {
                            if (leftTile.tileType == Tile.TileType.None)
                                currentTile = Tile.GetTileInfo(Tile.TileType.LeftGrass, currentTile.tilePosition);
                            if (rightTile.tileType == Tile.TileType.None)
                                currentTile = Tile.GetTileInfo(Tile.TileType.RightGrass, currentTile.tilePosition);
                        }
                    }

                    activeWorldData.tiles[x, y] = currentTile;
                }
            }
        }

        public static void Update()
        {
            worldDetails.Update();
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (activeWorldChunk == null)
                return;

            foreach (Tile tile in activeWorldChunk)
            {
                tile.Draw(spriteBatch);
            }
            foreach (WorldObject obj in activeWorldObjects)
                obj.Draw(spriteBatch);
        }

        public static void DrawDetails(SpriteBatch spriteBatch)
        {
            worldDetails.Draw(spriteBatch);
        }
    }
}
