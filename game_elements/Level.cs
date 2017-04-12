using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Drawing;
using System.IO;

namespace Politechnikon.game_elements
{
    struct Level
    {
        private Block[,] grid;
        private string filename;
        public Point playerStartPos;

        public Block this[int x, int y]
        {
            get
            {
                return grid[x, y];
            }
            set
            {
                grid[x, y] = value;
            }
        }
        public string Filename
        {
            get { return filename; }
        }
        public int Width
        {
            get
            {
                return grid.GetLength(0);
            }
        }
        public int Height
        {
            get
            {
                return grid.GetLength(1);
            }
        }

        public Level(int width, int height)
        {
            grid = new Block[width, height];
            filename = "none";
            playerStartPos = new Point(1, 1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    {
                        grid[x, y] = new Block(BlockType.Solid, x, y);
                    }
                    else
                    {
                        grid[x, y] = new Block(BlockType.Empty, x, y);
                    }
                }
            }
        }

    }

    public enum BlockType
    {
        Solid,
        Empty,
        Platform,
        Ladder,
        LadderPlatform
    }
    struct Block
    {
        private BlockType type;
        private int posX, posY;
        private bool solid, platform, ladder;

        public BlockType Type
        {
            get { return type; }
        }
        public int X
        {
            get { return posX; }
        }
        public int Y
        {
            get { return posY; }
        }
        public bool IsSolid
        {
            get { return solid; }
        }
        public bool IsPlatform
        {
            get { return platform; }
        }
        public bool IsLadder
        {
            get { return ladder; }
        }

        public Block(BlockType type, int x, int y)
        {
            this.posX = x;
            this.posY = y;
            this.type = type;
            this.ladder = false;
            this.solid = false;
            this.platform = false;

            switch (type)
            {
                case BlockType.Ladder:
                    ladder = true;
                    break;
                case BlockType.LadderPlatform:
                    ladder = true;
                    platform = true;
                    break;
                case BlockType.Solid:
                    solid = true;
                    break;
                case BlockType.Platform:
                    platform = true;
                    break;
                default:

                    break;
            }
        }
    }
}
