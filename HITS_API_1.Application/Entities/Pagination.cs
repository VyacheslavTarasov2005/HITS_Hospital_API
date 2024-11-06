namespace HITS_API_1.Application.Entities;

public class Pagination
{
    private int _size;
    private Int64 _count;
    private int _current;

    public int Size => _size;

    public Int64 Count => _count;

    public int Current => _current;

    public Pagination(int size, Int64 objectsQuantity, int current)
    {
        _size = size;
        _count = Convert.ToInt64(Math.Ceiling(objectsQuantity / (double)size));
        _current = current;
    }
}