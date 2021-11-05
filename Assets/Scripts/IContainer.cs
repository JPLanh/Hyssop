using System.Collections.Generic;
public interface IContainer
{
    public void init();
    public void setStorage(List<ItemExistanceDTOWrapper> in_item_list);
    public void modifyStorage(ItemExistanceDTOWrapper in_item);
}