namespace Indexer
{
    public interface IUserInterface
    {
        void IndexFolder(string folderPath);
        void LoadIndex(string indexPath);
        void Search(string query, int k);
    }

}