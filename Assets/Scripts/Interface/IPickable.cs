public interface IPickable
{
    public void pickupCheck(AreaIndex in_index, GridSystem in_grid, out string out_item, out string state);
}