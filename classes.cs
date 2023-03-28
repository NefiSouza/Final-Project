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
