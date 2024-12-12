namespace Silence.Infrastructure.Helpers;

public class IndexedListItem
{
    public int Index { get; set; }

    public string? Name { get; set; }

    public bool IsEven => Index % 2 == 0;
}

