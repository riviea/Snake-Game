using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace SnakeGame
{
    interface IDrawableShape
    {
        void Draws();
        List<Shape> Elements();
    }

    class Shape
    {
        protected int x { get; set; }
        protected int y { get; set; }
        protected char sym { get; set; }

        public int X { get { return x; } }
        public int Y { get { return y; } }
        public char Sym { get { return sym; } }

        public void Draw()
        {
            Console.SetCursorPosition(x * 2, y);
            Console.Write(sym);
        }

        public void Move(int _x, int _y)
        {
            x += _x;
            y += _y;
        }

        public bool IsHit(int _x, int _y)
        {
            if (x == _x && y == _y)
                return true;
            return false;
        }

        public bool IsHit(Shape obj)
        {
            if (x == obj.x && y == obj.y)
                return true;
            return false;
        }
    }

    class Tile : Shape
    {
        public enum ThroughType { None, Moveable, Immovable }
        private ThroughType through;

        public Tile(int _x, int _y, char _sym)
        {
            x = _x;
            y = _y;
            sym = _sym;
        }

        public void SetThrough(ThroughType _thr)
        {
            through = _thr;
        }

        public bool IsMove(Shape obj)
        {
            if (this.IsHit(obj) && (this.through == ThroughType.Immovable))
                return false;
            return true;
        }


    }

    class Cell : Shape
    {
        public Cell() { }

        public Cell(Cell clone)
        {
            x = clone.x;
            y = clone.y;
            sym = clone.sym;
        }
        public Cell(int _x, int _y, char _sym)
        {
            x = _x;
            y = _y;
            sym = _sym;
        }
    }

    class Food : Shape
    {
        public Food(int _x, int _y, char _sym)
        {
            x = _x;
            y = _y;
            sym = _sym;
        }
    }

    class Snake : IDrawableShape
    {
        public enum Direction { None, Left, Right, Up, Down }
        private Direction dir;

        public Direction Dir { get { return dir; } set { dir = value; } }

        private List<Cell> cells = new List<Cell>();
        private int size;
        private int moveX, moveY;

        public int MoveX { get { return moveX; } }
        public int MoveY { get { return moveY; } }

        public int X { get { return cells.First().X; } }
        public int Y { get { return cells.First().Y; } }

        public Snake(int _x, int _y, char _sym)
        {
            cells.Add(new Cell(_x, _y, _sym));
            size = 0;
            dir = Direction.None;
        }

        public bool UpdateDir()
        {
            int prevMoveX = moveX, prevMoveY = moveY;

            switch (dir)
            {
                case Direction.Left:
                    moveX = -1;
                    moveY = 0;
                    break;
                case Direction.Right:
                    moveX = 1;
                    moveY = 0;
                    break;
                case Direction.Up:
                    moveX = 0;
                    moveY = -1;
                    break;
                case Direction.Down:
                    moveX = 0;
                    moveY = 1;
                    break;
                default:
                    moveX = 0;
                    moveY = 0;
                    break;
            }

            // 이동한 방향이 몸통과 충돌할 경우
            foreach (var cell in cells)
            {

                if (cell.IsHit(NextMove(moveX, moveY)))
                {
                    return true;
                //    moveX = prevMoveX;
                //    moveY = prevMoveY;
                }
            }
            return false;
        }

        public void MakeTail(int _size)
        {
            size += _size;
            
            int y = (dir==Direction.None ? 1 : 0);
            for (int i = 0; i < _size; ++i)
            {
                Cell tempCell = cells.Last();
                Cell cell = new Cell(tempCell.X, tempCell.Y+y, tempCell.Sym);
                cells.Add(cell);
            }
        }

        public void Draws()
        {
            foreach (var i in cells)
                i.Draw();
        }

        public List<Shape> Elements()
        {
            List<Shape> objs = new List<Shape>();
            foreach (var cell in cells)
                objs.Add(cell);

            return objs;
        }

        public void Move()
        {
            Cell head = this.NextMove();

            for (var i = cells.Count - 1; i > 0; --i)
            {
                //tail = tail-1
                cells[i] = new Cell(cells[i - 1]);
            }
            cells[0] = head;
        }

        public void Eat()
        {
            this.MakeTail(1);
        }

        public Cell NextMove()
        {
            Cell temp = new Cell(cells.First());
            temp.Move(moveX, moveY);

            return temp;
        }

        public Cell NextMove(int _x, int _y)
        {

            Cell temp = new Cell(cells.First());
            temp.Move(_x, _y);

            return temp;
        }

        public void Debug()
        {
            Console.SetCursorPosition(0, 15);
            Console.WriteLine($"{moveX}, {moveY}");
            Console.WriteLine(NextMove(moveX, moveY).IsHit(cells.First()));
            Console.WriteLine($"NextMove:x={NextMove().X}, y={NextMove().Y}");
            for (int i = 0; i < cells.Count; ++i)
                Console.WriteLine($"cells[{i}]:x={cells[i].X}, y={cells[i].Y}");
        }
    }

    class Map : IDrawableShape
    {
        private int width { get; }
        private int height { get; }
        private Tile[,] tiles;

        public int getW { get { return width; } }
        public int getH { get { return height; } }

        public Map(int _w, int _h)
        {
            width = _w;
            height = _h;

            tiles = new Tile[_h, _w];
            for (int i = 0; i < _w; i++)
            {
                for (int j = 0; j < _h; j++)
                {
                    tiles[j, i] = new Tile(i, j, 'ㆍ');
                }
            }
        }

        public Tile GetTile(int _x, int _y)
        {
            return tiles[_y, _x];
        }

        public void CreateOutline()
        {
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    if ((j == 0 || (j == height - 1)) || ((i == 0 || (i == width - 1))))
                    {
                        tiles[j, i] = new Tile(i, j, '■');
                        tiles[j, i].SetThrough(Tile.ThroughType.Immovable);
                    }
                }
            }
        }

        public void Draws()
        {
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    tiles[j, i].Draw();
                }
            }
        }

        public List<Shape> Elements()
        {
            List<Shape> objs = new List<Shape>();
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    objs.Add(tiles[j, i]);
                }
            }
            return objs;
        }
    }

    class FoodCreator : IDrawableShape
    {
        private List<Food> foods = new List<Food>();
        public List<Food> Foods { get { return foods; } }

        private int createRangeX, createRangeY;

        public FoodCreator(int width, int height)
        {
            createRangeX = width;
            createRangeY = height;
        }

        public void MakeFood(int amount)
        {
            Random rand = new Random();
            for (int i = 0; i < amount; ++i)
            {
                int spawnX = rand.Next(1, createRangeX - 1);
                int spawnY = rand.Next(1, createRangeY - 1);

                Food n = new Food(spawnX, spawnY, '◎');

                if ((foods.Contains(n)))
                    continue;
                else
                    foods.Add(n);
            }
        }

        public void RemoveFood(Food _food)
        {
            foods.Remove(_food);
        }

        public void Draws()
        {
            foreach (var food in foods)
                food.Draw();
        }

        public List<Shape> Elements()
        {
            List<Shape> objs = new List<Shape>();
            foreach (var food in foods)
                objs.Add(food);

            return objs;
        }
    }

    class Game
    {
        public enum SpawnType { Default, Random }

        public bool gameover;

        public Map map;
        public Snake snake;
        public FoodCreator creator;
        public List<IDrawableShape> objs;
        public Shape[,] buffer;
        public int speed;

        public Game(int w, int h, SpawnType type)
        {
            Initialize(w, h, type);
        }

        public void Initialize(int w, int h, SpawnType type)
        {
            speed = 1000;

            //map, snake, creatror init
            map = new Map(w, h);
            creator = new FoodCreator(w, h);
            switch (type)
            {
                default:
                case SpawnType.Default:
                    snake = new Snake(map.getW / 2, map.getH / 2, '◆');
                    break;

                case SpawnType.Random:
                    Random rand = new Random();
                    int spawnX = rand.Next(1, map.getW - 1);
                    int spawnY = rand.Next(1, map.getH - 1);
                    snake = new Snake(spawnX, spawnY, '◇');
                    break;
            }

            snake.MakeTail(2);
            creator.MakeFood(3);
            map.CreateOutline();

            //buffer init
            buffer = new Shape[map.getH, map.getW];
            for (int i = 0; i < map.getW; ++i)
            {
                for (int j = 0; j < map.getH; ++j)
                {
                    buffer[j, i] = new Shape();
                }
            }

            //Add drawble objects
            objs = new List<IDrawableShape>()
            {
                map,
                creator,
                snake,
            };
        }

        public void Update()
        {
            //Input Update
            AwaitInput();

            if (snake.UpdateDir() && (snake.Dir != Snake.Direction.None))
                GameOver(); 

            // 뱀이 이동불가능한 타일과 충돌했는지 체크
            Cell tempCell = snake.NextMove();
            if (map.GetTile(tempCell.X, tempCell.Y).IsMove(tempCell) && (snake.Dir != Snake.Direction.None))
            {
                for (int i = 0; i < creator.Foods.Count; ++i)
                {
                    if (tempCell.IsHit(creator.Foods[i]))
                    {
                        creator.RemoveFood(creator.Foods[i]);
                        snake.Eat();
                        speed -= 50;
                        creator.MakeFood(1);
                    }
                }
                snake.Move();
            }
            else if (map.GetTile(tempCell.X, tempCell.Y).IsMove(tempCell) == false)
                GameOver();

            UpdateBuffers();
        }

        public void GameOver()
        {
            gameover = true;
            string abc = "--- G A M E O V E R ---";
            Console.SetCursorPosition(map.getW - abc.Length / 2, map.getH);
            Console.WriteLine(abc);
        }

        public void Render()
        {
            DrawBuffers(); // is Backbuffer…
        }

        public void UpdateBuffers()
        {
            //Update Buffer
            foreach (var obj in objs)
            {
                List<Shape> shapes = obj.Elements();
                foreach (var shape in shapes)
                {
                    buffer[shape.Y, shape.X] = shape;
                }
            }
        }

        public void DrawBuffers()
        {
            //Draw Buffer
            foreach (var obj in buffer)
            {
                obj.Draw();
            }
        }

        public void Draws()
        {
            foreach(var obj in objs)
            {
                obj.Draws();
            }
        }

        public void AwaitInput()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        snake.Dir = Snake.Direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                        snake.Dir = Snake.Direction.Right;
                        break;
                    case ConsoleKey.UpArrow:
                        snake.Dir = Snake.Direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                        snake.Dir = Snake.Direction.Down;
                        break;
                    default:
                        break;
                }
            }
        }


    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Game gameManager = new Game(25, 15, Game.SpawnType.Default);
            while (!gameManager.gameover)
            {
                gameManager.Update();
                gameManager.Render();

                while (Console.KeyAvailable)
                    Console.ReadKey(false);

                Console.CursorVisible = false;
                Console.SetCursorPosition(0, gameManager.map.getH);

                Thread.Sleep(gameManager.speed);
            }
        }
    }
}
