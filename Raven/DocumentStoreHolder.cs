namespace OrderManager.Raven;

public static class DocumentStoreHolder
{
    private static readonly Lazy<IDocumentStore> LazyStore = new Lazy<IDocumentStore>(() =>
    {
        IDocumentStore store = new DocumentStore
        {
            Urls = new[] { "http://localhost:8080/" },
            Database = "Northwind"
        };

        store.Initialize();

        return store;
    });

    public static IDocumentStore Store => LazyStore.Value;
}