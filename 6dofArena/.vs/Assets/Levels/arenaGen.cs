using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arenaGen : MonoBehaviour {
    public GameObject cube;
    public GameObject player;
    public GameObject turret;
    public float arenaWidth = 20.0f;
    public int iArenaWidth = 3;
    public int minRooms = 1;
    public int minWidth = 4;
    public int maxWidth = 4;

    void Start()
    {
        //boxGen();
        VoxelMap myMap = new VoxelMap(iArenaWidth, cube);
        levelData myLevel = new levelData(iArenaWidth, minRooms, minWidth, maxWidth);
        levelData smallRooms = new levelData(iArenaWidth, 0, 4, 8);
        myMap.buildLevel(myLevel, smallRooms);
        myMap.cullHidden();
        myMap.instantiate();
        player = Instantiate(player);
        player.transform.position = myMap.startPosition();
        //turret.transform.position = myMap.startPosition();
    }

    class Path
    {
        //path is a series of grids arrainged in a list
        //each grid may link to another path
        List<Grid3> gridList;

    }

    void Update()
    {

    }

    struct room
    {
        public int x, y, z, xWidth, yWidth, zWidth, val;
        public room(int myX, int myY, int myZ, int myXWidth, int myYWidth, int myZWidth, int myVal) { x = myX; y = myY;z = myZ;xWidth = myXWidth;yWidth = myYWidth;zWidth = myZWidth;val = myVal; }
    }

    struct levelData
    {
        public int width, minRooms, minWidth, maxWidth;
        public levelData(int myWidth, int myMinRooms, int myMinWidth, int myMaxWidth)
        {
            width = myWidth;
            minRooms = myMinRooms;
            minWidth = myMinWidth;
            maxWidth = myMaxWidth;
        }
    }

    struct cell
    {
        public int x, y, z, val;
        public cell(int myX = 0, int myY = 0, int myZ = 0, int myVal = 0)
        {
            x = myX;
            y = myY;
            z = myZ;
            val = myVal;
        }
    }
    
    struct voxelPlus
    {
        public cell myCell;
        public GameObject vox;
        public voxelPlus(GameObject myVox,cell pos)
        {
            myCell = pos;
            vox = myVox;
        }
    }

    class VoxelMap
    {
        Grid3 myGrid;
        Grid3 gridCopy;
        int mapWidth;
        GameObject voxel;
        List<room> connectorList;
        List<voxelPlus> voxelList;
        void conway()
        {
            int i, f, g, index;
            index = 0;
            myGrid.data.CopyTo(gridCopy.data, 0);//updates the copy of the grid for double buffering
            for (i = 0; i < mapWidth; i++)
            {
                for (f = 0; f < mapWidth; f++)
                {
                    for (g = 0; g < mapWidth; g++)
                    {
                        if (gridCopy.data[index] == 0)
                        {
                            if (gridCopy.adj(i, f, g, 1) > 1 && gridCopy.adjEdges(i,f,g)==0) { myGrid.data[index] = 1; }
                        }
                        index++;
                    }
                }
            }
        }
        public void DeleteCell(int x, int y, int z)
        {
            //only removes cell, does not yet add neibors
            myGrid.setVal(x, y, z, 1);
            for (int i = 0; i < voxelList.Count; i++)
            {
                voxelPlus vox = voxelList[i];
                if (x == vox.myCell.x && y == vox.myCell.y && z == vox.myCell.z)
                {
                    Destroy(vox.vox);
                    voxelList.RemoveAt(i);
                    break;
                }
            }
        }
        public Vector3 startPosition()
        {
            cell myCell = new cell();
            myGrid.cellValueExistsReplace(ref myCell, 1, 2);
            Vector3 myPos = new Vector3(myCell.x * voxel.transform.localScale.x, myCell.y * voxel.transform.localScale.y, myCell.z * voxel.transform.localScale.z);
            return myPos;
        }
        public cell startCell()
        {
            cell myCell = new cell();
            myGrid.cellValueExistsReplace(ref myCell, 2, 4);
            return myCell;
        }
        public void buildLevel(levelData myData, levelData smallRooms)
        {
            int i, f, g;
            int boxWidth = 3;
            int roomWidth = 8;
            bool place = false;
            /*myGrid.setRegion(1, 1, 1, boxWidth * roomWidth, boxWidth * roomWidth, boxWidth * roomWidth, 1);
            for (i = 0; i < roomWidth; i++)
            {
                for (f = 0; f < roomWidth; f++)
                {
                    for (g = 0; g < roomWidth; g++)
                    {
                        if (i!=0 && i!=roomWidth-1 && f!=0 && f!=roomWidth-1 && g!=0 && g != roomWidth-1)
                        {
                            //if (!(i % 2 == 0) || !(f % 2 == 0))
                            place = i%2==0;
                            if (f % 2 == 0) { place = !place; }
                            if (g % 2 == 0 && g % 4 == 2) { place = !place; }
                            if (place)
                            {
                                myGrid.setRegion(1 + i * boxWidth, 1 + f * boxWidth, 1 + g * boxWidth, boxWidth, boxWidth, boxWidth, 0);
                            }
                        }
                    }
                }
            }/**/

            /*myGrid.setRegion(1, 1, 1, 15, 15, 15, 1);

            myGrid.setRegion(6, 6, 6, 3, 3, 3, 0);

            myGrid.setRegion(3, 3, 3, 3, 3, 3, 0);
            myGrid.setRegion(3, 9, 3, 3, 3, 3, 0);
            myGrid.setRegion(9, 3, 3, 3, 3, 3, 0);
            myGrid.setRegion(9, 9, 3, 3, 3, 3, 0);

            myGrid.setRegion(3, 3, 9, 3, 3, 3, 0);
            myGrid.setRegion(3, 9, 9, 3, 3, 3, 0);
            myGrid.setRegion(9, 3, 9, 3, 3, 3, 0);
            myGrid.setRegion(9, 9, 9, 3, 3, 3, 0);/**/
            for (i = 0; i < myData.minRooms; i++)
            {
                placeRoom(myData);
            }

            //isConnected();
            //while (isConnected() == false) { placeRoom(myData); }
            while (isConnected() == false) { connectorList.Add(placeRoom(smallRooms)); }
            //conway();
            roomCull(1.0f);/**/
        }
        public void roomCull(float reduction = 1.0f)
        {
            int testI;
            room tempRoom;
            int endSize = (int)(connectorList.Count * reduction);
            while (connectorList.Count > endSize)
            {
                testI = irandom(0, connectorList.Count);
                tempRoom = connectorList[testI];
                tempRoom.val = 0;
                connectorList.RemoveAt(testI);
                //myGrid.data.CopyTo(gridCopy.data, 0);//updates the copy of the grid for double buffering
                myGrid.setRegion(tempRoom);
                tempRoom.val = 1;
                if (isConnected() == false) { myGrid.setRegion(tempRoom); }
                //if (isConnected() == false) { gridCopy.data.CopyTo(myGrid.data, 0); }
            }
        }
        public bool isConnected()
        {
            bool output = true;
            int val = 1;
            int fillVal = 2;
            List<cell> openList = new List<cell>();
            cell tempCell = new cell();
            myGrid.data.CopyTo(gridCopy.data, 0);//updates the copy of the grid for double buffering
            if (gridCopy.cellValueExists(ref tempCell, val))
            {
                openList.Add(tempCell);
                gridCopy.setVal(tempCell.x, tempCell.y, tempCell.z, fillVal);
            }
            while (openList.Count > 0)
            {
                tempCell = openList[0];
                addCell(ref openList, tempCell.x + 1, tempCell.y, tempCell.z, val, fillVal);
                addCell(ref openList, tempCell.x - 1, tempCell.y, tempCell.z, val, fillVal);

                addCell(ref openList, tempCell.x, tempCell.y - 1, tempCell.z, val, fillVal);
                addCell(ref openList, tempCell.x, tempCell.y + 1, tempCell.z, val, fillVal);

                addCell(ref openList, tempCell.x, tempCell.y, tempCell.z - 1, val, fillVal);
                addCell(ref openList, tempCell.x, tempCell.y, tempCell.z + 1, val, fillVal);
                openList.RemoveAt(0);
            }
            output= !gridCopy.inGrid(val);
            //if (output) { gridCopy.data.CopyTo(myGrid.data, 0); }
            return output;
        }
        public void addCell(ref List<cell> myList, int x, int y, int z, int val, int fillVal)
        {
            cell nextCell = new cell();
            if (gridCopy.queryCell(x, y, z, val, ref nextCell))
            {
                myList.Add(nextCell);
                gridCopy.data[gridCopy.getIndex(x, y, z)] = fillVal;
            }
        }
        public int irandom(int min, int max)
        {
            float fMin = (float)min;
            float fMax = (float)max;
            float fOut = Random.Range(fMin, fMax);
            int iOut = (int)fOut;
            return iOut;
        }
        public room placeRoom(levelData myData)
        {
            int maxLeng = myData.maxWidth;
            int minLeng = myData.minWidth;
            int xLeng = irandom(minLeng, maxLeng);
            int yLeng = irandom(minLeng, maxLeng);
            int zLeng = irandom(minLeng, maxLeng);
            //assumes mapWidth is greater than maxLeng
            int x = irandom(1, mapWidth - xLeng - 1);
            int y = irandom(1, mapWidth - yLeng - 1);
            int z = irandom(1, mapWidth - zLeng - 1);
            myGrid.setRegion(x, y, z, xLeng, yLeng, zLeng, 1);
            room tempRoom = new room(x, y, z, xLeng, yLeng, zLeng, 1);
            return tempRoom;
        }
        public void cullHidden()
        {
            myGrid.data.CopyTo(gridCopy.data, 0);
            int index = 0;
            for (int i = 0; i < mapWidth; i++)
            {
                for (int f = 0; f < mapWidth; f++)
                {
                    for (int g = 0; g < mapWidth; g++)
                    {
                        if (myGrid.data[index] == 0)
                        {
                            if (gridCopy.adj(i, f, g, 0, true) == 6) { myGrid.data[index] = 3; }
                        }
                        index++;
                    }
                }
            }
        }
        public void instantiate()
        {
            int index = 0;
            float xScale = voxel.transform.localScale.x;
            float yScale = voxel.transform.localScale.y;
            float zScale = voxel.transform.localScale.z;
            for (int i = 0; i < mapWidth; i++)
            {
                for (int f = 0; f < mapWidth; f++)
                {
                    for (int g = 0; g < mapWidth; g++)
                    {                        
                        if (myGrid.data[index] == 0)
                        {
                            GameObject tempObj = Instantiate(voxel);
                            //voxelList.Add(new voxelPlus(tempObj, new cell(i, f, g)));
                            tempObj.transform.position = new Vector3(i * xScale, f * yScale, g * zScale);
                        }
                        index++;
                    }
                }
            }
        }
        public void setVal(int x, int y, int z, int val) { myGrid.setVal(x, y, z, val); }//manual set val for debuggling only
        public VoxelMap(int width, GameObject myVoxel)
        {
            int seed;
            //seed = (int)System.DateTime.Now.Ticks;
            seed = 255;
            Random.InitState(seed);
            Debug.Log(seed);
            myGrid = new Grid3(width);
            gridCopy = new Grid3(width);
            mapWidth = width;
            voxel = myVoxel;
            connectorList = new List<room>();
        }
    }
	
    class Grid3
    {
        public int []data;
        public int dataSize;
        int gridWidth;
        /*public void conway()
        {
            Grid3 tempGrid = new Grid3(gridWidth);
            data.CopyTo(tempGrid.data,0);

        }*/
        public bool cellValueExists(ref cell myCell, int val)
        {
            bool output = true;
            for (int i = 0; i < dataSize; i++)
            {
                if (data[i] == val)
                {
                    myCell = getCell(i);
                    break;
                }
            }
            return output;
        }
        public bool cellValueExistsReplace(ref cell myCell, int val, int rep)
        {
            bool output = true;
            for (int i = 0; i < dataSize; i++)
            {
                if (data[i] == val)
                {
                    myCell = getCell(i);
                    data[i] = rep;
                    break;
                }
            }
            return output;
        }
        public cell getCell(int index)
        {
            int x = index / (gridWidth * gridWidth);
            index -= x * gridWidth * gridWidth;
            int y = index / gridWidth;
            index -= y * gridWidth;
            int z = index;
            cell outCell = new cell(x, y, z, data[index]);
            return outCell;
        }
        public bool inGrid(int val)
        {
            for (int i = 0; i < dataSize; i++)
            {
                if (data[i] == val) { return true; }
            }
            return false;
        }
        public void setRegion(room myRoom) { setRegion(myRoom.x, myRoom.y, myRoom.z, myRoom.xWidth, myRoom.yWidth, myRoom.zWidth, myRoom.val); }
        public void setRegion(int x, int y, int z, int xLeng, int yLeng, int zLeng, int val)
        {
            for (int i = 0; i < xLeng; i++)
            {
                for (int f = 0; f < yLeng; f++)
                {
                    for (int g = 0; g < zLeng; g++)
                    {
                        if (onGrid(i + x, f + y, g + z)) { setVal(i + x, f + y, g + z, val); }
                    }
                }
            }
        }
        public int adj(int x, int y, int z, int val, bool includeEdge = false)
        {
            int count = 0;
            if (query(x - 1, y, z, val)) { count++; }
            if (query(x + 1, y, z, val)) { count++; }

            if (query(x, y - 1, z, val)) { count++; }
            if (query(x, y + 1, z, val)) { count++; }

            if (query(x, y, z - 1, val)) { count++; }
            if (query(x, y, z + 1, val)) { count++; }
            if (includeEdge) { count += adjEdges(x, y, z); }
            return count;
        }
        public bool queryCell(int x, int y, int z, int val, ref cell myCell)
        {
            bool output = onGrid(x, y, z);
            int testVal, index;
            if (output)
            {
                index = getIndex(x, y, z);
                testVal = data[index];
                output = testVal == val;
                if (output) { myCell = getCell(index); }
            }
            return output;
        }
        public bool query(int x, int y, int z, int val)
        {
            bool output = onGrid(x, y, z);
            if (output)
            {
                output = (getVal(x, y, z) == val);
            }
            return output;
        }
        public int adjEdges(int x, int y, int z)
        {
            int edges = 0;
            if (x == 0 || x == gridWidth - 1) { edges++; }
            if (y == 0 || y == gridWidth - 1) { edges++; }
            if (z == 0 || z == gridWidth - 1) { edges++; }
            return edges;
        }
        public bool onGrid(int x, int y, int z)
        {
            if (x<0 || x > gridWidth - 1) { return false; }
            else if (y<0 || y > gridWidth - 1) { return false; }
            else if (z<0 || z> gridWidth - 1) { return false; }
            else { return true; }
        }
        public int getWidth() { return gridWidth; }
        public void setVal(int x, int y, int z, int val)
        {
            data[getIndex(x, y, z)] = val;
        }
        public int getVal(int x, int y, int z)
        {
            return data[getIndex(x, y, z)];
        }
        public int getIndex(int x, int y, int z)
        {
            int index = x * gridWidth * gridWidth;
            index += y * gridWidth;
            index += z;
            return index;
        }
        public void clear(int val)
        {
            for (int i = 0; i < dataSize; i++)
            {
                data[i] = val;
            }
        }
        public Grid3(int width, int clearVal = 0)
        {
            dataSize = width * width * width;
            data = new int [dataSize];
            clear(clearVal);
            gridWidth = width;
        }
    }

    void boxGen()
    {
        GameObject cubex = Instantiate(cube);
        GameObject cubey = Instantiate(cube);
        GameObject cubez = Instantiate(cube);

        cubex.GetComponent<Transform>().localScale = new Vector3(1.0f, arenaWidth, arenaWidth);
        cubex.GetComponent<Transform>().localPosition = new Vector3(-arenaWidth * 0.5f, 0.0f, 0.0f);
        GameObject cubexneg = Instantiate(cubex);
        cubexneg.GetComponent<Transform>().localPosition = new Vector3(arenaWidth * 0.5f, 0.0f, 0.0f);

        cubey.GetComponent<Transform>().localScale = new Vector3(arenaWidth, 1.0f, arenaWidth);
        cubey.GetComponent<Transform>().localPosition = new Vector3(0.0f, -arenaWidth * 0.5f, 0.0f);
        GameObject cubeyneg = Instantiate(cubey);
        cubeyneg.GetComponent<Transform>().localPosition = new Vector3(0.0f, arenaWidth * 0.5f, 0.0f);

        cubez.GetComponent<Transform>().localScale = new Vector3(arenaWidth, arenaWidth, 1.0f);
        cubez.GetComponent<Transform>().localPosition = new Vector3(0.0f, 0.0f, -arenaWidth * 0.5f);
        GameObject cubezneg = Instantiate(cubez);
        cubezneg.GetComponent<Transform>().localPosition = new Vector3(0.0f, 0.0f, arenaWidth * 0.5f);
    } 
}
