using System.Collections.Generic;
using System.Linq;

public class Screen
{
    private int _screenWidth;
    private int _screenHeight;

    public int GetWidth()
    {
        return _screenWidth;
    }

    public int GetHeight()
    {
        return _screenHeight;
    }

    public void CheckSize()
    {
        _screenWidth = Console.BufferWidth - 1;
        _screenHeight = Console.BufferHeight - 1;
    }

    public List<int> GetScreenRect() 
    {
        int screenWidth = _screenWidth;
        int screenHeight = _screenHeight;
        List<int> screenRect = new List<int> {0, screenWidth, 0, screenHeight};
        return screenRect;
    }
}

public class Object
{
    protected int _x;
    protected int _y;
    protected int _width;
    protected int _height;
    protected List<int> _rect;
    protected List<string> _drawing;
    protected bool _destroyed;



    public Object(int x, int y, List<string> drawing)
    {
        _x = x;
        _y = y;
        _drawing = drawing;
    }



    public void SetImage(List<string> drawing)
    {
        _drawing = drawing;
    }

    public void SetLocation(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public void SetDimensions()
    {
        int width = 0;
        int height = 0;
        foreach (string line in _drawing)
        {
            height += 1;
            if (line.Length >= width)
            {
                width = line.Length;
            }
        }

        _width = width;
        _height = height;

        int left = _x;
        int right = _x + _width - 1;
        int top = _y;
        int bottom = _y + _height - 1;
        List<int> rect = new List<int>
        {left, right, top, bottom};

        _rect = rect;
    }

    public int GetX()
    {
        return _x;
    }

    public int GetY()
    {
        return _y;
    }

    public int GetWidth()
    {
        return _width;
    }

    public int GetHeight()
    {
        return _height;
    }

    public void Draw()
    {
        int counter = 1;

        Console.SetCursorPosition(_x, _y);
         
        foreach (string line in _drawing)
        {
            for (int i = 0; i < line.Length; i++)
            {
                Console.Write(line[i]);
            }

            Console.SetCursorPosition(_x, (_y + counter));
            counter++;
        }
    }

    public void Clear()
    {
        int counter = 1;
        Console.SetCursorPosition(_x, _y);
        foreach (string line in _drawing)
        {
            for (int i = 0; i < line.Length; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(_x, (_y + counter));
            counter++;
        }
    }

    public bool GetDestroyed()
    {
        return _destroyed;
    }


    public void Destroy()
    {
        _destroyed = true;
    }

    public List<int> GetRect()
    {
        return _rect;
    }

    public bool DetectCollision(List<int> otherRect)
    {
        // Convert the object's rectangle to a polygon
        List<Vector2> vertices = new List<Vector2>();
        vertices.Add(new Vector2(_rect[0]+1, _rect[2]+1));
        vertices.Add(new Vector2(_rect[1]-1, _rect[2]+1));
        vertices.Add(new Vector2(_rect[1]-1, _rect[3]-1));
        vertices.Add(new Vector2(_rect[0]+1, _rect[3]-1));
        Polygon polygon1 = new Polygon(vertices);

        // Convert the other rectangle to a polygon
        vertices = new List<Vector2>();
        vertices.Add(new Vector2(otherRect[0]+1, otherRect[2]+1));
        vertices.Add(new Vector2(otherRect[1]-1, otherRect[2]+1));
        vertices.Add(new Vector2(otherRect[1]-1, otherRect[3]-1));
        vertices.Add(new Vector2(otherRect[0]+1, otherRect[3]-1));
        Polygon polygon2 = new Polygon(vertices);

        // Check for overlap on each axis
        List<Vector2> axes = new List<Vector2>();
        axes.AddRange(polygon1.GetEdges().Select(edge => edge.Normal()));
        axes.AddRange(polygon2.GetEdges().Select(edge => edge.Normal()));
        foreach (Vector2 axis in axes)
        {
            float min1 = polygon1.GetMinProjection(axis);
            float max1 = polygon1.GetMaxProjection(axis);
            float min2 = polygon2.GetMinProjection(axis);
            float max2 = polygon2.GetMaxProjection(axis);
            if (max1 < min2 || max2 < min1)
            {
                // There is a gap on this axis, so the polygons do not overlap
                return false;
            }
        }

        // There is no gap on any axis, so the polygons overlap
        return true;
    }
}

public class Projectile : Object
{
    // private int _damage; // Not needed for simpler game. 
    // private int _speed; Not needed for simpler game. 
    private bool _direction; // true is right, false is left. Meaning that true is player and false is enemy. 
    // private Animation _animation; Not needed for simpler game. 

    public Projectile(int x, int y, List<string> drawing, bool direction) : base(x, y, drawing)
    {
        // _speed = speed;
        // _damage = damage;
        _direction = direction;
        // _animation = animation;
    }

    public void Move(List<int> backgroundRect, int frameCounter)
    { 

        int currentX = this.GetX();
        int currentY = this.GetY(); 

        this.Clear();

        List<int> leftRectangle = new List<int>(backgroundRect);
        leftRectangle[0] += 2;

        List<int> rightRectangle = new List<int>(backgroundRect);
        rightRectangle[1] -= 2;

        bool insideLeftWall = this.DetectCollision(leftRectangle);

        bool insideRightWall = this.DetectCollision(rightRectangle);


        // ! This made it so that only one bullet would move at a time. 
        // _animation.SetFrames(_speed);

        // bool animate = _animation.Animate(frameCounter); 

        
        if (_direction)
        {
            if (insideRightWall)
            {
                this.SetLocation(currentX + 1, currentY);
            } else
            {
                this.Destroy();
            }
        } else if (!_direction)
        {
            if (insideLeftWall)
            {
                    this.SetLocation(currentX - 1, currentY);
            } else
            {
                this.Destroy();
            }
            
        }

    }

    // public int GetDamage()
    // {
    //     return _damage;
    // }

    // public int GetSpeed()
    // {
    //     return _speed;
    // }

    public bool GetDirection()
    {
        return _direction;
    }

}

public class Enemy : Object
{
    // private int _damage; Not required for our simplified program. 

    private Projectile _projectile;

    private int _health;

    // private int _speed; Not needed for simpler game

    public Enemy(int x, int y, List<string> drawing, int health) : base(x, y, drawing)
    {
        // _speed = speed;
        _health = health;
        // _damage = damage;
    }

    public void Move()
    {
        // Todo: Get random numbers for the direction that the enemy will be moving in. 0 = left, 1 = up, 2 = right, 3 = down. 
        // Todo: Check if that random direction will put the ship in a place outside the background. If it does don't move the ship. 
        // Todo: Otherwise draw the ship in the new location, and clear the old location. 
    }

    public int GetHealth()
    {
        return _health;
    }

    // public int GetDamage()
    // {
    //     return _damage;
    // }

    public void SetProjectile(Projectile projectile) // ! The reson we don't set this in the constructor is because some ships won't have projectiles.
    {
        _projectile = projectile;
    }

    public void TakeDamage()
    {
        _health -= 1;
        if (_health == 0)
        {
            this.Destroy();
        }
    }

}

public class Player : Object
{

    private int _health;
    private Projectile _projectile;
    private List<ConsoleKey> _pressed;

    public Player(int x, int y, List<string> drawing, int health, Projectile projectile) : base(x, y, drawing)
    {
        _health = health;
        _projectile = projectile;
    }

    public void Move(HashSet<ConsoleKey> keysPressed, List<int> backgroundRect)
    {

        // 0 left, 1 right, 2 top, 3 bottom. 

        int currentX = this.GetX();
        int currentY = this.GetY(); 

        List<int> leftRectangle = new List<int>(backgroundRect);
        leftRectangle[0] += 2;

        List<int> rightRectangle = new List<int>(backgroundRect);
        rightRectangle[1] -= 2;

        List<int> topRectangle = new List<int>(backgroundRect);
        topRectangle[2] += 2;

        List<int> bottomRectangle = new List<int>(backgroundRect);
        bottomRectangle[3] -= 2;

        bool insideLeftWall = this.DetectCollision(leftRectangle);

        bool insideRightWall = this.DetectCollision(rightRectangle);

        bool insideTopWall = this.DetectCollision(topRectangle);

        bool insideBottomWall = this.DetectCollision(bottomRectangle);

        if (keysPressed.Contains(ConsoleKey.A) && insideLeftWall || keysPressed.Contains(ConsoleKey.LeftArrow) && insideLeftWall)
        {
            this.Clear(); 
            this.SetLocation(currentX - 1, currentY);
        }
        else if (keysPressed.Contains(ConsoleKey.D) && insideRightWall || keysPressed.Contains(ConsoleKey.RightArrow) && insideRightWall)
        {
            this.Clear(); 
            this.SetLocation(currentX + 1, currentY);
        }
        else if (keysPressed.Contains(ConsoleKey.W) && insideTopWall || keysPressed.Contains(ConsoleKey.UpArrow) && insideTopWall)
        {
            this.Clear();
            this.SetLocation(currentX, currentY - 1);
        }
        else if (keysPressed.Contains(ConsoleKey.S) && insideBottomWall || keysPressed.Contains(ConsoleKey.DownArrow) && insideBottomWall)
        {
            this.Clear();
            this.SetLocation(currentX, currentY + 1);
        }
    }


    public int GetHealth()
    {
        return _health;
    }

    public void SetProjectile(Projectile projectile)
    {
        _projectile = projectile;
    }

    public void TakeDamage()
    {
        _health -= 1;
        if (_health == 0)
        {
            this.Destroy();
        }
    }


}

public class Structures : Object
{
    private int _health;

    public Structures(int x, int y, List<string> drawing, int health) : base(x, y, drawing)
    {
        health = _health;
    }
}

public class Background : Object
{
    public bool NeedsRedraw = false;

    public Background(int x, int y, List<string> drawing) : base(x, y, drawing)
    {
    }

    public new void SetImage(List<string> drawing)
    {
        _drawing = drawing;
        NeedsRedraw = true;
    }

    public new void SetLocation(int x, int y)
    {
        _x = x;
        _y = y;
        NeedsRedraw = true;
    } // ! Possible issues to troubleshoot are that perhaps the SetLocation, or SetImage methods are being used repeatedly. 
}

// ! change the functionality for this class at some point. 
public class LoadScreen
{
    private List<Background> _backgrounds;
    private List<Projectile> _playerProjectiles;
    private List<Projectile> _enemyProjectiles;
    private Player _player;
    private List<Enemy> _enemies;
    private List<Structures> _structures;

    public LoadScreen()
    {
        _backgrounds = new List<Background>();
        _playerProjectiles = new List<Projectile>();
        _enemyProjectiles = new List<Projectile>();
        _enemies = new List<Enemy>();
        _structures = new List<Structures>();
    }


    public void AddBackground(Background background)
    {
        _backgrounds.Add(background);
    }

    public void AddPlayerProjectile(Projectile projectile)
    {
        _playerProjectiles.Add(projectile);
    }

    public void AddEnemyProjectile(Projectile projectile)
    {
        _enemyProjectiles.Add(projectile);
    }

    public void AddPlayer(Player player)
    {
        this._player = player;
    }

    public void AddEnemy(Enemy enemy)
    {
        _enemies.Add(enemy);
    }

    public void AddStructure(Structures structure)
    {
        _structures.Add(structure);
    }

    public List<Background> GetBackground()
    {
        return _backgrounds;
    }

    public List<Projectile> GetPlayerProjectiles()
    {
        return _playerProjectiles;
    }

    public List<Projectile> GetEnemyProjectiles()
    {
        return _enemyProjectiles;
    }

    public Player GetPlayers()
    {
        return _player;
    }

    public List<Enemy> GetEnemies()
    {
        return _enemies;
    }

    public List<Structures> GetStructures()
    {
        return _structures;
    }

    public void SetBackground(List<Background> updatedBackgrounds)
    {
        _backgrounds = updatedBackgrounds;
    }

    public void SetPlayerProjectile(List<Projectile> updatedProjectiles)
    {
        _playerProjectiles = updatedProjectiles;
    }

    public void SetEnemyProjectile(List<Projectile> updatedProjectiles)
    {
        _enemyProjectiles = updatedProjectiles;
    }

    public void SetPlayer(ref Player updatedPlayer)
    {
        _player = updatedPlayer; 
    }

    public void SetEnemy(List<Enemy> updatedEnemy)
    {
        _enemies = updatedEnemy;
    }

    public void SetStructure(List<Structures> updatedStructures)
    {
        _structures = updatedStructures;
    }


    // Todo: Add an update function to all the object classes. 
    public void Update(HashSet<ConsoleKey> keysPressed, List<int> backgroundRect, int frameCounter)
    {
        int newX = 0;
        int newY = 0;

        foreach (var background in _backgrounds)
        {
            newX = background.GetX();
            newY = background.GetY();

            bool needsRedraw = background.NeedsRedraw; 

            background.SetDimensions();
            if (needsRedraw == true)
            {
                background.SetLocation(newX, newY);
            }
        }

            if (_playerProjectiles is List<Projectile>) 
            {
                for (int i = _playerProjectiles.Count - 1; i >= 0; i--)
                {
                    var projectile = _playerProjectiles[i];
                    projectile.SetDimensions(); 
                    projectile.Move(backgroundRect, frameCounter);
                    bool isDestroyed = projectile.GetDestroyed(); 
                    if (isDestroyed)
                    {   
                        projectile.Clear();
                        _playerProjectiles.RemoveAt(i);
                    }
                }
            }

            if (_enemyProjectiles is List<Projectile>)
            {
                for (int i = _enemyProjectiles.Count - 1; i >= 0; i--)
                {
                    var projectile = _enemyProjectiles[i];
                    projectile.SetDimensions(); 
                    projectile.Move(backgroundRect, frameCounter);
                    bool isDestroyed = projectile.GetDestroyed(); 
                    if (isDestroyed)
                    {   
                        projectile.Clear();
                        _enemyProjectiles.RemoveAt(i);
                    }
                }
            }
            
            if (_player is Player)
            {
                _player.Move(keysPressed, backgroundRect);
                newX = _player.GetX();
                newY = _player.GetY();
                _player.SetLocation(newX, newY);
            }


        // Todo: check for collisions between all objects. 
        // Todo: Only allow the enemies to move if they are within the background. 
        // Todo: Anything that has collided with a damage dealing entitiy will lose health. 
        // Todo: Anything that has zero health is destroyed.
        // Todo: If the player is destroyed, change the scene to game over. 

    }

    public void Redraw()
    {
        foreach (var background in _backgrounds) // ! The blinking issue is a problem with the background, not the player.
        {
            if (background.NeedsRedraw)
            {
                background.Draw();
                background.NeedsRedraw = false;
            }
        }

        foreach (var projectile in _playerProjectiles)
        {
            projectile.Draw(); 
        }

        foreach (var projectile in _enemyProjectiles)
        {
            projectile.Draw();
        }
        
        if (_player is Player)
        {
            _player.Draw();
        }

        foreach (var enemy in _enemies)
        {
            enemy.Draw();
        }

    }
}


public class Animation
{
    private int _counter = 0;
    private int _animationFrames;
    private int _timesAnimated = 0; // ! This is for things like bullets, where youll need to know the amount of times the animation has happened.

    public void SetFrames(int frames)
    {
        _animationFrames = frames;
    }

    public bool Animate(int frameCounter)
    {
        if (frameCounter == 0)
        {
            _counter = 0;
            _timesAnimated = 0;
        }
        if (frameCounter >= (_counter * _animationFrames))
        {
            _counter += 1;
            _timesAnimated++;
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetTimes()
    {
        return _timesAnimated;
    }
}

public class Vector2
{
    public float X { get; set; }
    public float Y { get; set; }

    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public static Vector2 operator +(Vector2 a, Vector2 b)
    {
        return new Vector2(a.X + b.X, a.Y + b.Y);
    }

    public static Vector2 operator -(Vector2 a, Vector2 b)
    {
        return new Vector2(a.X - b.X, a.Y - b.Y);
    }

    public static Vector2 operator *(Vector2 a, float scalar)
    {
        return new Vector2(a.X * scalar, a.Y * scalar);
    }

    public static Vector2 operator /(Vector2 a, float scalar)
    {
        return new Vector2(a.X / scalar, a.Y / scalar);
    }

    public float Magnitude()
    {
        return (float)Math.Sqrt(X * X + Y * Y);
    }

    public Vector2 Normalize()
    {
        float magnitude = Magnitude();
        return new Vector2(X / magnitude, Y / magnitude);
    }

    public static float Dot(Vector2 a, Vector2 b)
    {
        return a.X * b.X + a.Y * b.Y;
    }

    public static float AngleBetween(Vector2 a, Vector2 b)
    {
        float dotProduct = Dot(a, b);
        float magA = a.Magnitude();
        float magB = b.Magnitude();
        return (float)Math.Acos(dotProduct / (magA * magB));
    }
}

public class Polygon
{
    private readonly List<Vector2> _vertices;
    private readonly List<Edge> _edges;

    public Polygon(List<Vector2> vertices)
    {
        _vertices = vertices;
        _edges = new List<Edge>();
        for (int i = 0; i < _vertices.Count; i++)
        {
            int j = (i + 1) % _vertices.Count;
            _edges.Add(new Edge(_vertices[i], _vertices[j]));
        }
    }

    public List<Edge> GetEdges()
    {
        return _edges;
    }

    public float GetMinProjection(Vector2 axis)
    {
        float min = float.MaxValue;
        foreach (Vector2 vertex in _vertices)
        {
            float projection = Vector2.Dot(vertex, axis);
            if (projection < min)
            {
                min = projection;
            }
        }
        return min;
    }

    public float GetMaxProjection(Vector2 axis)
    {
        float max = float.MinValue;
        foreach (Vector2 vertex in _vertices)
        {
            float projection = Vector2.Dot(vertex, axis);
            if (projection > max)
            {
                max = projection;
            }
        }
        return max;
    }
}

public class Edge
{
    public Vector2 Start { get; private set; }
    public Vector2 End { get; private set; }

    public Edge(Vector2 start, Vector2 end)
    {
        Start = start;
        End = end;
    }

    public Vector2 Normal()
    {
        Vector2 edge = End - Start;
        return new Vector2(-edge.Y, edge.X);
    }
}