namespace HITS_API_1.Application.Entities;

public class Pagination
{
    private int _size;
    private int _count;
    private int _current;

    public int Size
    {
        get => _size;
        set => _size = value;
    }

    public int Count
    {
        get => _count;
        set => _count = value;
    }

    public int Current
    {
        get => _current;
        set => _current = value;
    }

    public Pagination(int size, int objectsQuantity, int current)
    {
        _size = size;
        _count = Convert.ToInt32(Math.Ceiling(objectsQuantity / (double)current));
        _current = current;
    }
}