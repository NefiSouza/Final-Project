using System;
using System.Diagnostics; // gives me acess to the stopwatch. 
using System.Runtime.InteropServices;

class Program
{

    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);

    static void Main(string[] args)
    {
        // ! Too much time waisted
        // Console.Out.Flush();
        // Console.SetOut(TextWriter.Synchronized(Console.Out));
        // Console.OutputEncoding = System.Text.Encoding.Unicode;
        // System.Reflection.PropertyInfo fontProp = typeof(Console).GetProperty("Font", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        // if (fontProp != null)
        // {
        //     Console.Font = new System.Drawing.Font("Arial Unicode MS", Console.Font.Size);
        // }

        // * revised character 
        // * Player: } 
        // * Player Projectiles: - / \ 
        // * Enemy: { ~ ]
        // * Enemy Projectiles: o 0 x +
        // * Sheild: ] 
        // * Towers: (C O) >
        // * Health bar: [][][][][][][][][][] 
        // Initialize classes 
        Screen windowSize = new Screen();
        Stopwatch stopwatch = new Stopwatch();


        Animation startAnimation = new Animation();
        startAnimation.SetFrames(30);

        Animation spawnClock = new Animation(); 
        spawnClock.SetFrames(600);

        Animation shootClock = new Animation(); 
        shootClock.SetFrames(180);

        Animation gameOverLength = new Animation();
        gameOverLength.SetFrames(600);

        Animation playerShoot = new Animation();
        playerShoot.SetFrames(30);

        // Initialize variables
        List<string> lineProjectile = new List<string>
        {
            "-"
        };

        List<string> circleProjectile = new List<string>
        {
            "o"
        };

        List<string> basicEnemy = new List<string>
        {
            "{"
        };

        

        HashSet<ConsoleKey> keysPressed = new HashSet<ConsoleKey>();
        List<Background> backgrounds = new List<Background>();
        List<Projectile> enemyProjectiles = new List<Projectile>();
        List<Projectile> playerProjectiles = new List<Projectile>(); 
        // ! Ill have to setup the player class individually. 
        List<Enemy> enemies = new List<Enemy>();
        List<Structures> structures = new List<Structures>();

        Animation baseBulletSpeed = new Animation(); 

        Projectile baseProjectile = new Projectile(0, 0, lineProjectile, true);

        Projectile baseEnemyProjectile = new Projectile(0, 0, circleProjectile, false);

        string scene = "start";
        int fps = 60;

        double frameDuration = 1000.0 / fps;

        int lastWidth = 0;
        int lastHeight = 0;

        int frameCounter = 0;

        bool screenSizeChanged = false;

        bool sceneChange = true; 

        int level = 0;
        int spawnRate = 600;
        int bulletRate = 120;
        int enemyNumber = 1; 
        int enemiesKilled = 0;
        int currentScore = 0;
        bool skipSpawn = false; 
        int healthDisplayNumber = 55;
        string healthDisplayString = string.Concat(Enumerable.Repeat("[]", healthDisplayNumber));
        int highscore = 0; 
        bool canShoot = false;

        // Initilize other

        stopwatch.Start();

        Random random = new Random(); 

        LoadScreen game = new LoadScreen();
        LoadScreen start = null;

        Player player = null;

        // ! pregame test


        // ! End of test area 

