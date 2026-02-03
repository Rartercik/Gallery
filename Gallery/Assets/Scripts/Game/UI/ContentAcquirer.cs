using Data;
using Game.UI;
using System;

public class ContentAcquirer
{
    public enum Filter
    {
        All,
        Odd,
        Even
    }

    private Content _content;
    private Filter _filter;

    public ContentAcquirer(Content content, Filter filter)
    {
        _content = content;
        _filter = filter;
    }

    public void SetContent(ContentItem item, int index)
    {
        var free = (index + 1) % 4 != 0;
        index = GetFilteredIndex(index);
        if (_content.CheckAvailability(index))
        {
            item.SetVisible(true);
            item.SetImage(_content.GetSprite(index), free);
            return;
        }

        item.SetVisible(false);
    }

    public void SetFilter(Filter filter)
    {
        _filter = filter;
    }

    public int GetFilteredContentLength()
    {
        var lengthAddition = _content.ContentLength % 2 == 0 ? 0 : 1;
        switch (_filter)
        {
            case Filter.All:
                return _content.ContentLength;
            case Filter.Odd:
            case Filter.Even:
                return _content.ContentLength / 2 + lengthAddition;
            default:
                throw new InvalidOperationException("Unrecognized filter");
        }
    }

    private int GetFilteredIndex(int index)
    {
        switch (_filter)
        {
            case Filter.All:
                return index;
            case Filter.Odd:
                return index * 2;
            case Filter.Even:
                return index * 2 + 1;
            default:
                throw new InvalidOperationException("Unrecognized filter");
        }
    }
}