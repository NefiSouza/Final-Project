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

 Console.CursorVisible = false;
        // Game loop
        while (true)
        {
            double elapsedTime = stopwatch.ElapsedMilliseconds;
            // * I hope this will auto refresh every frame. 
            if (frameCounter < 6000) // The framerate resets every 100 seconds. 
            {
                frameCounter += 1;
            }
            else
            {
                frameCounter = 0;
                stopwatch.Reset();
                stopwatch.Restart();
            }

            // * Check window size 
            windowSize.CheckSize();

            int screenWidth = windowSize.GetWidth();
            int screenHeight = windowSize.GetHeight();

            List<int> screenRect = windowSize.GetScreenRect();

            // Todo: Handle keyboard events
            keysPressed.Clear();

            for (int i = 0; i <= 255; i++)
            {
                short keyState = GetAsyncKeyState(i);
                if (keyState == -32767)
                {
                    ConsoleKey consoleKey = (ConsoleKey)i;
                    keysPressed.Add(consoleKey);
                }
            }

            // Todo: Allow entitys to update

            // Todo: Draw the scene 

            // ! checking if the screen size has changed. 
            if (lastWidth != screenWidth || lastHeight != screenHeight)
            {
                lastWidth = screenWidth;
                lastHeight = screenHeight;
                screenSizeChanged = true;
            }
            else
            {
                screenSizeChanged = false;
            }

            if (screenWidth <= 80 || screenHeight <= 30)
            {   
                Console.Clear();
                scene = "small screen";
                sceneChange = true; 
            }

            if (scene == "start")
            {
                if (sceneChange)
                {
                    Console.Clear();

                    start = new LoadScreen();

                    // ! Draws the title 
                    List<string> title = new List<string>
                    {
                    "          Terminal WAR",
                    "'`'`'`'`'`            '`'`'`'`'`"}; // TODO: It would be cool to add an animation where the word dropped in, one letter at a time. 

                    List<string> startText = new List<string>
                    {"Press Enter to Start: "};

                    List<string> highScore = new List<string>
                    {$"High Score: {highscore}"};

                    Background titleObject = new Background(0, 0, title);

                    start.AddBackground(titleObject);

                    Background startObject = new Background(0, 0, startText);

                    start.AddBackground(startObject);

                    Background highScoreObject = new Background(0, 0, highScore);

                    start.AddBackground(highScoreObject);

                    backgrounds = start.GetBackground();

                    start.Update(keysPressed, screenRect, frameCounter);

                    // Set the starting positin of the titleObject. 
                    int width = backgrounds[0].GetWidth();
                    backgrounds[0].SetLocation((screenWidth - width) / 2, 3);

                    // Set the starting position of the startObject. 
                    width = backgrounds[1].GetWidth();
                    backgrounds[1].SetLocation((screenWidth - width) / 2, 12);

                    // Set the starting positoin of the highScoreObject. 
                    width = backgrounds[2].GetWidth();
                    backgrounds[2].SetLocation((screenWidth - width) / 2, 18);

                    // Draws the starting screen. 
                    start.Redraw(); 

                    sceneChange = false;
                }


