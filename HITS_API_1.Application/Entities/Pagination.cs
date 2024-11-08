namespace HITS_API_1.Application.Entities;

public class Pagination
{
    private int _size = 5;
    private Int64 _count;
    private int _current = 1;

    public int Size => _size;

    public Int64 Count => _count;

    public int Current => _current;

    public Pagination(int? size, Int64 objectsQuantity, int? current)
    {
        if (size != null)
        {
            _size = size.Value;
        }
        
        _count = Convert.ToInt64(Math.Ceiling(objectsQuantity / (double)_size));

        if (current != null)
        {
            _current = current.Value;
        }
    }
}