namespace Trinhky.Blazor.Server.Editors
{
    public class TestModelService
    {
        private static List<TestModel> CreateDataSource()
        {
            List<TestModel> temp = new();
            for(int i = 0; i < 40; i++)
            {
                temp.Add(new TestModel() { ID = i, Name = "", TepName = "", TepUrl="" });
            }
            return temp;
        }

        private List<TestModel> DataSource { get; set; }
        public TestModelService()
        {
            DataSource = CreateDataSource();
        }
        public Task<IEnumerable<TestModel>> GetDataSourceAsync(CancellationToken ct = default)
        {
            return Task.FromResult(DataSource.AsEnumerable());
        }
        List<TestModel> InsertInternal(TestModel itemModel)
        {
            var dataItem = new TestModel();
            Update(dataItem, itemModel);
            dataItem.ID = DataSource.OrderBy(m => m.ID).LastOrDefault().ID + 1;
            DataSource.Add(dataItem);
            return DataSource;
        }
        public Task<List<TestModel>> Insert(TestModel itemModel)
        {
            return Task.FromResult(InsertInternal(itemModel));
        }
        List<TestModel> RemoveInternal(TestModel dataItem)
        {
            DataSource.Remove(dataItem);
            return DataSource;
        }
        public Task<List<TestModel>> Remove(TestModel dataItem)
        {
            return Task.FromResult(RemoveInternal(dataItem));
        }
        List<TestModel> UpdateInternal(TestModel dataItem, TestModel itemModel)
        {
            dataItem.Name = itemModel.Name;
            dataItem.TepUrl = itemModel.TepUrl;
            dataItem.TepName = itemModel.TepName;
            return DataSource;
        }
        public Task<List<TestModel>> Update(TestModel dataItem, TestModel itemModel)
        {
            return Task.FromResult(UpdateInternal(dataItem, itemModel));
        }
    }
}
