using UnityEngine;

/*
 * 
 * Local generation of files for offline play
 * 
 * Obsolete for the time being, will revisit after finishing online.
 * 
 */
public class AreaGenerator
{
    //x == row y == column

    #region Basic Farm
    public static void generateBasicPlayerFarm(string in_mode, GridSystem in_grid, int in_length, int in_width, int in_height, string in_name)
    {
        in_grid.createNewGrid(in_length, in_width, in_height, in_name);
        in_grid.generateIndexes();
        in_grid.generateBorder();
        generateBasicFarmParameter(in_grid, "Wooden Fence", in_length, in_width, in_height);
        generateBasicHouse(in_grid);
        generateBasicPath(in_grid);
        generateBasicFarmLight(in_grid, new Vector3(13, 2, 0), new Vector3(11, 10, 5));
        createTutorial(in_grid);
        in_grid.area.buildable = true;
        GameObject shippingBin = in_grid.spawnNewStorage(in_grid.getGridAtLocation(3, 13, 0), Quaternion.Euler(0f, 90f, 0f), "Basic Shipping Bin", "Shipping Bin");
        GameObject mailbox = in_grid.spawnNewStorage(in_grid.getGridAtLocation(3, 8, 0), Quaternion.Euler(0f, 90f, 0f), "Basic Mailbox", "Mailbox");
        if (shippingBin.TryGetComponent<StorageEntity>(out StorageEntity out_storage))
        {
            if (mailbox.TryGetComponent<StorageEntity>(out StorageEntity out_mail))
            {
                out_storage.deliverPoint = out_mail.storage;

            }
        }
        in_grid.saveArea(in_mode);

    }


    private static void createTutorial(GridSystem in_grid)
    {
        in_grid.generateObject(in_grid.currentGrid[12, 11, 1], "Area Connector", false, false);
        in_grid.currentGrid[12, 11, 0].state = "Tutorial Exit House";
        in_grid.generateObject(in_grid.currentGrid[12, 10, 1], "Area Connector", false, false);
        in_grid.currentGrid[12, 10, 0].state = "Tutorial Exit House";


        in_grid.generateObject(in_grid.currentGrid[2, 11, 1], "Area Connector", false, false);
        in_grid.currentGrid[2, 11, 0].state = "Tutorial Exit Farm";
        in_grid.generateObject(in_grid.currentGrid[2, 10, 1], "Area Connector", false, false);
        in_grid.currentGrid[2, 10, 0].state = "Tutorial Exit Farm";

    }

    private static void generateBasicHouse(GridSystem in_grid)
    {
        for (int x = 13; x <= 24; x++)
        {
            for (int y = 2; y <= 13; y++)
            {
                for (int z = 0; z < 5; z++)
                {
                    //Door
                    if ((x == 13 && y == 11 && z < 3) || (x == 13 && y == 10 && z < 3))
                    {
                        if (x == 13 && y == 11 && z == 1)
                        {
                            in_grid.spawnItem("Wooden Door", in_grid.getGridAtLocation(x, y, z), Quaternion.identity);
                            in_grid.currentGrid[x, y, z].state = "Open";
                        }

                    }
                    else
                    {
                        //Wall
                        if (x == 13 || x == 24 || y == 2 || y == 13)
                        {
                            in_grid.generateObject(in_grid.currentGrid[x, y, z], "Wooden Wall", false, false);

                        }
                        else
                        {
                            //floor
                            if (z == 0)
                                in_grid.generateObject(in_grid.currentGrid[x, y, 0], "Wooden Floor", false, false);
                            //Ceiling
                            if (z == 4)
                                in_grid.generateObject(in_grid.currentGrid[x, y, z], "Wooden Wall", false, false);

                        }
                    }
                }
            }
        }
    }

    public static void generateBasicPath(GridSystem in_grid)
    {
        for (int x = 2; x < 14; x++)
        {
            in_grid.generateObject(in_grid.currentGrid[x, 10, 0], "Stone Path", false, false);
            in_grid.generateObject(in_grid.currentGrid[x, 11, 0], "Stone Path", false, false);

        }
    }

    public static void generateBasicFarmLight(GridSystem in_grid, Vector3 in_position, Vector3 in_size)
    {
        in_grid.spawnItem("Torch", new Vector3(in_position.x, in_size.z, in_position.y) + new Vector3(in_size.x / 2, .1f, in_size.z / 2), Quaternion.Euler(90f, -90f, 90));

        Item tmp_item = in_grid.spawnItem("Basic Well", in_grid.getGridAtLocation(7, 6, 0), Quaternion.identity);
        tmp_item.quantity = 2;
        tmp_item.capacity = 50;
        tmp_item.maxCapacity = 100;

        in_grid.spawnItem("Basic Bed", in_grid.getGridAtLocation(22, 5, 0), Quaternion.identity);

    }


    public static void generateBasicFarmParameter(GridSystem in_grid, string in_input, int length, int width, int height)
    {

        for (int row = 1; row <= length; row++)
        {
            if (row == 1 || row == length)
            {
                for (int column = 1; column < width; column++)
                {
                    if (row == 1 && (column == 10 || column == 11))
                    {
                        in_grid.generateObject(in_grid.currentGrid[row, column, 0], "Area Connector", false, false);
                        in_grid.currentGrid[row, column, 0].state = "Teleport 49 " + (column - 5) + " 0 Central Hub";
                    }
                    else
                    {
                        in_grid.generateObject(in_grid.currentGrid[row, column, 0], in_input, false, false);
                        in_grid.generateObject(in_grid.currentGrid[row, column, 1], in_input, false, false);
                    }
                }
            }
            else
            {
                in_grid.generateObject(in_grid.currentGrid[row, 1, 0], in_input, false, false);
                in_grid.generateObject(in_grid.currentGrid[row, 1, 1], in_input, false, false);

                in_grid.generateObject(in_grid.currentGrid[row, width, 0], in_input, false, false);
                in_grid.generateObject(in_grid.currentGrid[row, width, 1], in_input, false, false);
            }
        }
    }
    #endregion

    #region Central Hub
    public static void generateCentralHub(string in_mode, GridSystem in_grid, int in_length, int in_width, int in_height, string in_name)
    {
        in_grid.createNewGrid(in_length, in_width, in_height, in_name);
        in_grid.generateIndexes();
        in_grid.generateBorder();
        in_grid.area.buildable = false;
        generateCentralHubParameter(in_grid, "Wooden Fence", in_length, in_width, in_height);
        generateCentralHubPath(in_grid);
        generateBasicShop(in_grid, new Vector3(2, 2, 0), new Vector3(20, 22, 4), "North");
        generateBasicShop(in_grid, new Vector3(33, 9, 0), new Vector3(16, 15, 4), "East");
        generateBasicShopLight(in_grid, new Vector3(2, 2, 0), new Vector3(20, 22, 4));
        generateBasicShopLight(in_grid, new Vector3(33, 9, 0), new Vector3(16, 15, 4));
        generateBasicShopInterior(in_grid);
        //        generateBasicShopNPC(in_grid);
        //        generateBasicHouse(in_grid);
        //        generateCentralHubPath(in_grid);
        in_grid.saveArea(in_mode);
    }

    public static void generateBasicShopNPC(GridSystem in_grid)
    {
        in_grid.spawnNPC(in_grid.getGridAtLocation(5, 14, 0), "Alex", "Shop");
    }

    private static void generateCentralHubParameter(GridSystem in_grid, string in_input, int length, int width, int height)
    {

        for (int row = 1; row <= length; row++)
        {
            if (row == 1 || row == length)
            {
                for (int column = 1; column <= width; column++)
                {
                    if (row == 50 && (column == 5 || column == 6))
                    {
                        in_grid.generateObject(in_grid.currentGrid[row, column, 0], "Area Connector", false, false);
                        in_grid.currentGrid[row, column, 0].state = "Teleport 2 " + (column + 5) + " 0 (Player Farm)";
                    }
                    else
                    {
                        in_grid.generateObject(in_grid.currentGrid[row, column, 0], in_input, false, false);
                        in_grid.generateObject(in_grid.currentGrid[row, column, 1], in_input, false, false);
                    }
                }
            }
            else
            {


                //if (row == 28 || row == 29)
                //{
                //    Debug.Log("Alt exit");
                //    in_grid.generateObject(in_grid.currentGrid[row, width, 0], "Area Connector", false, false);
                //    //                        in_grid.currentGrid[row, column, 0].state = "Teleport 2 " + column + " 0 (Player Farm)";
                //} else
                {
                    in_grid.generateObject(in_grid.currentGrid[row, 1, 0], in_input, false, false);
                    in_grid.generateObject(in_grid.currentGrid[row, 1, 1], in_input, false, false);

                    in_grid.generateObject(in_grid.currentGrid[row, width, 0], in_input, false, false);
                    in_grid.generateObject(in_grid.currentGrid[row, width, 1], in_input, false, false);

                }
            }
        }
    }

    public static void generateBasicShopInterior(GridSystem in_grid)
    {
        for (int x = 5; x <= 19; x++)
        {
            in_grid.generateObject(in_grid.currentGrid[x, 3, 0], "Wooden Wall", false, false);
            in_grid.generateObject(in_grid.currentGrid[x, 23, 0], "Wooden Wall", false, false);
        }

        for (int y = 5; y <= 10; y++)
        {
            in_grid.generateObject(in_grid.currentGrid[21, y, 0], "Wooden Wall", false, false);
            in_grid.generateObject(in_grid.currentGrid[3, y, 0], "Wooden Wall", false, false);
        }

        for (int y = 17; y <= 21; y++)
        {
            in_grid.generateObject(in_grid.currentGrid[21, y, 0], "Wooden Wall", false, false);
            in_grid.generateObject(in_grid.currentGrid[3, y, 0], "Wooden Wall", false, false);
        }

        for (int y = 7; y <= 11; y++)
        {
            in_grid.generateObject(in_grid.currentGrid[16, y, 0], "Wooden Wall", false, false);
            in_grid.generateObject(in_grid.currentGrid[11, y, 0], "Wooden Wall", false, false);
        }

        for (int y = 16; y <= 19; y++)
        {
            in_grid.generateObject(in_grid.currentGrid[16, y, 0], "Wooden Wall", false, false);
            in_grid.generateObject(in_grid.currentGrid[11, y, 0], "Wooden Wall", false, false);
        }

        for (int x = 3; x <= 6; x++)
        {
            in_grid.generateObject(in_grid.currentGrid[x, 11, 0], "Wooden Wall", false, false);
            in_grid.generateObject(in_grid.currentGrid[x, 16, 0], "Wooden Wall", false, false);
        }

        for (int y = 12; y <= 15; y++)
        {
            in_grid.generateObject(in_grid.currentGrid[6, y, 0], "Wooden Wall", false, false);
        }


        //in_grid.spawnNewNPC(in_grid.getGridAtLocation(5, 14, 0) + new Vector3(0f, .50f, 0f), Quaternion.Euler(0f, 90f, 0f), "Alex", "General Shop");
        //in_grid.spawnNewNPC(in_grid.getGridAtLocation(42, 17, 0) + new Vector3(0f, .50f, 0f), Quaternion.Euler(0f, 180f, 0f), "Magmus", "Smithery");
    }

    public static void generateCentralHubPath(GridSystem in_grid)
    {
        for (int x = 49; x >= 28; x--)
        {
            in_grid.generateObject(in_grid.currentGrid[x, 5, 0], "Stone Path", false, false);
            in_grid.generateObject(in_grid.currentGrid[x, 6, 0], "Stone Path", false, false);
        }
        for (int y = 24; y >= 11; y--)
        {
            in_grid.generateObject(in_grid.currentGrid[28, y, 0], "Stone Path", false, false);
            in_grid.generateObject(in_grid.currentGrid[29, y, 0], "Stone Path", false, false);
        }
        for (int y = 10; y >= 2; y--)
        {
            in_grid.generateObject(in_grid.currentGrid[28, y, 0], "Stone Path", false, false);
            in_grid.generateObject(in_grid.currentGrid[29, y, 0], "Stone Path", false, false);
        }
        for (int x = 22; x <= 27; x++)
        {
            in_grid.generateObject(in_grid.currentGrid[x, 13, 0], "Stone Path", false, false);
            in_grid.generateObject(in_grid.currentGrid[x, 14, 0], "Stone Path", false, false);

        }
    }

    private static void generateBasicHouse(GridSystem in_grid, Vector3 in_position, Vector3 in_size, string in_direction)
    {

        for (float x = in_position.x; x <= in_position.x + in_size.x; x++)
        {
            for (float y = in_position.y; y <= in_position.y + in_size.y; y++)
            {
                for (float z = in_position.z; z <= in_position.z + in_size.z; z++)
                {
                    bool isDoor = false;
                    if (x == in_position.x || x == in_position.x + in_size.x || y == in_position.y || y == in_position.y + in_size.y)
                    {
                        if (
                                in_direction.Equals("North") &&
                                (
                                    (x == in_position.x + in_size.x && y == in_position.y + ((int)(in_size.y / 2)) && z < in_position.z + 4) ||
                                    (x == in_position.x + in_size.x && y == in_position.y + ((int)(in_size.y / 2)) + 1 && z < in_position.z + 4)
                                )
                            )
                        {
                            isDoor = true;
                            if (x == in_position.x + in_size.x && y == in_position.y + ((int)(in_size.y / 2)) + 1 && z == in_position.z + 1)
                            {
                                in_grid.spawnItem("Wooden Door", in_grid.getGridAtLocation((int)x, (int)y, (int)z), Quaternion.identity);
                            }


                        }
                        if (!isDoor)
                            in_grid.generateObject(in_grid.currentGrid[(int)x, (int)y, (int)z], "Wooden Wall", false, false);
                        //Door

                    }
                    else
                    {
                        //floor
                        if (z == in_position.z)
                            in_grid.generateObject(in_grid.currentGrid[(int)x, (int)y, 0], "Wooden Floor", false, false);
                        if (z == in_position.z + in_size.z)
                            in_grid.generateObject(in_grid.currentGrid[(int)x, (int)y, (int)z], "Wooden Wall", false, false);

                    }
                }
            }
        }
    }


    private static void generateBasicShop(GridSystem in_grid, Vector3 in_position, Vector3 in_size, string in_direction)
    {

        for (float x = in_position.x; x <= in_position.x + in_size.x; x++)
        {
            for (float y = in_position.y; y <= in_position.y + in_size.y; y++)
            {
                for (float z = in_position.z; z <= in_position.z + in_size.z; z++)
                {
                    bool notAWall = false;
                    if (x == in_position.x || x == in_position.x + in_size.x || y == in_position.y || y == in_position.y + in_size.y)
                    {
                        if (in_direction.Equals("North"))
                        {
                            //Door
                            if (
                                    (x == in_position.x + in_size.x && y == in_position.y + ((int)(in_size.y / 2)) && z < in_position.z + 3) ||
                                    (x == in_position.x + in_size.x && y == in_position.y + ((int)(in_size.y / 2)) + 1 && z < in_position.z + 3)
                                )
                            {
                                notAWall = true;
                                if (x == in_position.x + in_size.x && y == in_position.y + ((int)(in_size.y / 2)) + 1 && z == in_position.z + 1)
                                {
                                    in_grid.spawnItem("Wooden Door", in_grid.getGridAtLocation((int)x, (int)y, (int)z), Quaternion.identity);
                                    in_grid.currentGrid[(int)x, (int)y, (int)z].state = "Open";
                                }
                            }

                            //Glass
                            if (
                                    (x == in_position.x + in_size.x && (y > in_position.y + 2 && y < in_position.y + 9) && (z == in_position.z + 1 || z == in_position.z + 2)) ||
                                    (x == in_position.x + in_size.x && (y < in_position.y + in_size.y - 2 && y > in_position.y + in_size.y - 8) && (z == in_position.z + 1 || z == in_position.z + 2))
                                )
                            {
                                notAWall = true;
                                in_grid.generateObject(in_grid.currentGrid[(int)x, (int)y, (int)z], "Glass", false, false);
                            }
                        }

                        if (in_direction.Equals("East"))
                        {
                            //Door
                            if (
                                    (x == in_position.x + ((int)(in_size.x / 2)) && y == in_position.y && z < in_position.z + 3) ||
                                    (x == in_position.x + ((int)(in_size.x / 2)) + 1 && y == in_position.y && z < in_position.z + 3)
                                )
                            {
                                notAWall = true;
                                if (x == in_position.x + ((int)(in_size.x / 2)) && y == in_position.y && z == in_position.z + 1)
                                {
                                    in_grid.spawnItem("Wooden Door", in_grid.getGridAtLocation((int)x, (int)y, (int)z), Quaternion.Euler(new Vector3(0f, -90f, 0f)));
                                    in_grid.currentGrid[(int)x, (int)y, (int)z].state = "Open";
                                }
                            }

                            //Glass
                            if (
                                    (x == in_position.x && (y > in_position.y + 2 && y < in_position.y + 9) && (z == in_position.z + 1 || z == in_position.z + 2)) ||
                                    (x == in_position.x && (y < in_position.y + in_size.y - 2 && y > in_position.y + in_size.y - 8) && (z == in_position.z + 1 || z == in_position.z + 2))
                                )
                            {
                                notAWall = true;
                                in_grid.generateObject(in_grid.currentGrid[(int)x, (int)y, (int)z], "Glass", false, false);
                            }
                        }
                        if (!notAWall)
                            in_grid.generateObject(in_grid.currentGrid[(int)x, (int)y, (int)z], "Wooden Wall", false, false);
                        //Door

                    }
                    else
                    {
                        //floor
                        if (z == in_position.z)
                            in_grid.generateObject(in_grid.currentGrid[(int)x, (int)y, 0], "Wooden Floor", false, false);
                        if (z == in_position.z + in_size.z)
                            in_grid.generateObject(in_grid.currentGrid[(int)x, (int)y, (int)z], "Wooden Wall", false, false);

                    }
                }
            }
        }
    }

    public static void generateBasicShopLight(GridSystem in_grid, Vector3 in_position, Vector3 in_size)
    {
        int widthSegment = (int)(in_size.x / 2);
        int heightSegment = (int)(in_size.y / 2);


        in_grid.spawnItem("Torch", new Vector3(in_position.x, in_size.z, in_position.y) + new Vector3(widthSegment, .1f, heightSegment), Quaternion.Euler(90f, -90f, 90f));
    }
}
#endregion