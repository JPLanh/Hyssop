public interface IStructure
{
    public string getStructureType();
    public void connectAdjacentType(string in_index);
    public void connectAdjacentDirection(string in_dir, bool in_connection);
}