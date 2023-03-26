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