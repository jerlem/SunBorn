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

    // RandomWalk Values
    [SerializeField]
    int GridMaxTunnels;
    [SerializeField]
    int GridMaxLength;
    [SerializeField]
    int GridMinLenght;

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
        }

        // do we move the point X and y ?
        if (movePointer) {
            PointX = x; PointY = z;
        }
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
                Debug.Log("RandomWalk - Border reached, at " + PointX + "/" + PointY);
                return;
            }
        }

        Debug.Log("RandomWalk - Dig finished at "+ PointX +"/" + PointY);
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
        Debug.Log("RandomWalk - Entry point set at 0/ " + pos );
        SetGridBlock(0, pos, EEnvironmentBlock.StartPoint, true);

        PlaceTunnel(Edirection.Up, 10);

        while (GridMaxTunnels > 0)
        {
            int x = PointX;
            int y = PointY;

            PlaceTunnel(CurrentDirection, PseudoRandom.NextRange(GridMinLenght, GridMaxLength));
            Turn();

            if (x != PointX || y != PointY) GridMaxTunnels--;
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
