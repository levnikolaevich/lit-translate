namespace SDT.Bl.IHelpers
{
    public interface IGlobalCacheService<T>
    {
        T GetPriceList();
        void SetPriceList(T response);
    }
}
