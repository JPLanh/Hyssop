public interface IConnectable
{
    public bool connectionCheck(AreaIndex in_from, AreaIndex in_to, string in_dir);
    public bool connectionCut(AreaIndex in_from, AreaIndex in_to, string in_dir);
}