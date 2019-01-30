using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridGeneratorTest : MonoBehaviour {

    // Grid basic values
    [SerializeField]
    public int GridWidth = 10;
    [SerializeField]
    public int GridHeight = 10;
    [SerializeField]
    public uint PrngSeed = 0;

    // GameObjects Refs
    [SerializeField]
    public GameObject GOWall;
    [SerializeField]
    public GameObject GOGround;
    [SerializeField]
    public GameObject GOStartPoint;
    [SerializeField]
    public GameObject GOWayPoint;
    [SerializeField]
    public GameObject GOExitPoint;

    // RandomWalk Values
    [SerializeField]
    int GridMaxTunnels;
    [SerializeField]
    int GridMaxLength;
    [SerializeField]
    int GridMinLenght;

    [SerializeField]
    int GridRoomFrequency;
    int GridCurrentRoomFrequency;

    [SerializeField]
    int GridBiasPass;
    private int GridCurrentBiasPass;

    [SerializeField]
    Vector2 WayPoint1;


    private int PointX;
    private int PointY;
    private Edirection CurrentDirection = Edirection.Up;
    
    // Pseudo Random Number Generator
    Prng PseudoRandom;

    // 1D Array to handle grid Data
    private EEnvironmentBlock[] Grid;
    
    // Get and Set for Grid element
    // TODO : check size allocation
    public EEnvironmentBlock GetGridBlock(int x, int y) => Grid[(x + (y * GridWidth))];


    /*
     *  SetGridBlock:
     *      Set to EEnvironmentBlock a given block x,y
     *      and can move the pointer.
     */
    public void SetGridBlock(int x, int y, EEnvironmentBlock newValue, bool movePointer = false) {
      // TODO : check posision
      Grid[(x + (y * GridWidth))] = newValue;
        // do we move the point X and y ?
        if (movePointer) {
            PointX = x; PointY = y;
            //Debug.Log("SetGridBlock - pointer is now at " + x + "," + y);
        }
    }


    /*
     *  ToString :
     */
    public override string ToString()
    {
        string output = "Grid :";
        if (Grid.Length < 1) {
            for (int i = 0; i < Grid.Length; i++)
                output += " " + Grid[i];
        }
        return "Grid is Empty";
    }


    /*
     * InitGrid :
     *      Create the PRNG
     *      Initialize Grid
     */
    private void InitGrid()
    {
        // Grid init
        Grid = new EEnvironmentBlock[(GridWidth * GridHeight)];

        // Prng init with seed value  
        PseudoRandom = new Prng();
        PseudoRandom.Seed(PrngSeed);
        GridCurrentRoomFrequency = GridRoomFrequency;
    }

    /*
     *  PlaceBlock :
     *      Set block type in memory
     */
    private void PlaceBlock(EEnvironmentBlock BlockType, int x, int z, bool movePointer=false)
    {
        // correcting position 
        float oX = x + 0.5f;
        float oZ = z + 0.5f;

        // Set the block
        switch (BlockType)
        {
            default:
            case EEnvironmentBlock.Wall:
                Instantiate(GOWall, new Vector3(-oX, 0, -oZ), Quaternion.identity, transform);
                break;
            case EEnvironmentBlock.Ground:
                Instantiate(GOGround, new Vector3(-oX, 0, -oZ), Quaternion.identity, transform);
                break;
            case EEnvironmentBlock.StartPoint:
                Instantiate(GOStartPoint, new Vector3(-oX, 0, -oZ), Quaternion.identity, transform);
                break;
            case EEnvironmentBlock.WayPoint:
                Instantiate(GOWayPoint, new Vector3(-oX, 0, -oZ), Quaternion.identity, transform);
                break;
            case EEnvironmentBlock.ExitPoint:
                Instantiate(GOExitPoint, new Vector3(-oX, 0, -oZ), Quaternion.identity, transform);
                break;
        }

        // do we move the point X and y ?
        if (movePointer) {
            PointX = x; PointY = z;
        }
    }

    private void PlaceRoom(Edirection direction, int h, int l)
    {
        // hold temp pointer
        int tx = PointX;
        int tz = PointY;
         
        Debug.Log("RandomWalk - Making a ("+ h +"x"+ l +")room in direction " + direction);

        // swap orientation
        if (direction == Edirection.Up || direction == Edirection.Down)
        {
            int temp = l;
            l = h;
            h = temp;
        }

        // place Room
        while (h > 0)
        {
            PlaceTunnel(direction, l);
            PointY++;
            h--;
            PointX = tx -1;
        }

        // get back in good position
        PointX = tx;
        PointY = tz;
        PlaceTunnel(direction, l);
       
    }

    /*
     *  PlaceTunnel :
     *      Dig a tunnel
     */
    private void PlaceTunnel(Edirection direction, int lenght)
    {
        Debug.Log("RandomWalk - Digging tunnel " + direction +" size " + lenght);
        int dx = 0;
        int dz = 0;

        switch (direction)
        {
            case Edirection.Left:   dz = 1;  break;
            case Edirection.Right:  dz = -1; break;
            case Edirection.Up:     dx = 1;  break;
            case Edirection.Down:   dx = -1; break;
        }

        while (lenght > 0)
        {
            int vY = PointY + dz;
            int vX = PointX + dx;

            // within the grid range ?
            if ((vY < GridHeight-1) && (vY > 0)
              && (vX < GridWidth-1) && (vX > 0))
            {
                SetGridBlock(PointX + dx, PointY + dz, EEnvironmentBlock.Ground, true);
                lenght--;
            } else
            {
                Debug.Log("RandomWalk - Border reached, at " + PointX + ";" + PointY);
                return;
            }
        }

        Debug.Log("RandomWalk - Dig finished at "+ PointX +";" + PointY);
    }

    /*
     *  MakeGrid:
     *      By default Make a randomized Grid
     */
    private void MakeGrid()
    {
        for (int x= 0; x < GridWidth; x++) {
            for (int y = 0; y < GridHeight; y++)
                PlaceBlock(GetGridBlock(x, y), x, y);
        }
    }

    /*
     *  MakeRandomWalk:
     *      Place Tunnels
     */
    public void MakeRandomWalk()
    {
        // Set all titles to Wall
        for (int x = 0; x < GridWidth; x++)
            for (int y = 0; y < GridHeight; y++) SetGridBlock(x, y, EEnvironmentBlock.Wall);

        // Set the Start point
        int pos = PseudoRandom.NextRange(1, GridWidth - 1);
        SetGridBlock(0, pos, EEnvironmentBlock.StartPoint, true);
        Debug.Log("RandomWalk - Entry point set at 0;" + pos );

        // Set the Exit point
        //pos = PseudoRandom.NextRange(1, GridWidth - 1);
        SetGridBlock(GridHeight-1, GridWidth - pos, EEnvironmentBlock.ExitPoint);
        Debug.Log("RandomWalk - Exit point set at "+ GridWidth +";" + pos);

        // Put a Way point at the center
        SetGridBlock(GridHeight / 2, GridWidth / 2, EEnvironmentBlock.WayPoint);
        Debug.Log("RandomWalk - Way point set at " + GridWidth/2 + ";" + GridHeight/2);

        PlaceTunnel(Edirection.Up, 10);
        PlaceRoom(Edirection.Up, 4, 4);

        while (GridMaxTunnels > 0)
        {
            int x = PointX;
            int y = PointY;
            // place tunnels
            PlaceTunnel(CurrentDirection, PseudoRandom.NextRange(GridMinLenght, GridMaxLength));
            
            // and some rooms
            GridCurrentRoomFrequency--;
            if (GridCurrentRoomFrequency < 1)
            {
                GridCurrentRoomFrequency = GridRoomFrequency;
                PlaceRoom(CurrentDirection, 3, 3);
            }

            Turn();

            // Cost Too many time
            // if (x != PointX || y != PointY) GridMaxTunnels--;
            GridMaxTunnels--;
        }

    }

    /* 
     * Turn function for RandomWalk
     */
    private void Turn()
    {
        bool turnRight = PseudoRandom.NextBool();
        switch (CurrentDirection)
        {
            case Edirection.Up:
                CurrentDirection = (turnRight) ? Edirection.Right : Edirection.Left;
                break;
            case Edirection.Right:
                CurrentDirection = (turnRight) ? Edirection.Up : Edirection.Down;
                break;
            case Edirection.Down:
                CurrentDirection = (turnRight) ? Edirection.Left : Edirection.Right;
                break;
            case Edirection.Left:
                CurrentDirection = (turnRight) ? Edirection.Up : Edirection.Down;
                break;
        }
    }


    // Use this for initialization
    void Start () {
        InitGrid();
        MakeRandomWalk();

        MakeGrid();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
