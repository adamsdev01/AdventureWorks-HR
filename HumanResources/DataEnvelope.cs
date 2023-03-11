namespace HumanResources
{
    public class DataEnvelope<T>
    {
        public List<T>? CurrentPageData { get; set; }
        public int TotalItemCount { get; set; }
    }
}
