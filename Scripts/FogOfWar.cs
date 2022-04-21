using UnityEngine;
using UnityEngine.UI;

public class FogOfWar : MonoBehaviour
{
    public RawImage fogRawImage;
    public MeshCollider mapMeshCollider;
    public Transform player;



    public Vector2Int fogDensity = new Vector2Int(100, 100);

    // change this to modify the size of the vision
    public Vector2Int beEliminatedShapeSize = new Vector2Int(8, 6);

    private Texture2D fogTexture;

    private Vector2Int[] shapeLocalPosition;

    private Vector2 fogOriginPoint;
    private Vector2 worldSize;

    //public GameObject FOWCanvas;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        mapMeshCollider = GameObject.Find("Map").GetComponent<MeshCollider>();

        //FOWCanvas.GetComponent<Canvas>().worldCamera = player.GetComponentInChildren<Camera>();

        fogTexture = new Texture2D(fogDensity.x, fogDensity.y);
        fogRawImage.texture = fogTexture;

        worldSize = new Vector2(mapMeshCollider.bounds.size.x, mapMeshCollider.bounds.size.y);

        print("xsize" + mapMeshCollider.bounds.size.x);

        print("ysize" + mapMeshCollider.bounds.size.y);

        //The original position of the fog of war
        fogOriginPoint = new Vector2(mapMeshCollider.transform.position.x - worldSize.x * 0.5f, mapMeshCollider.transform.position.y - worldSize.y * 0.5f);

        print(fogOriginPoint);
        InitializeTheShape();
        InitializeTheFog();

        EliminateFog();
    }

    private void Update()
    {


        if (Input.anyKey)
        {


            EliminateFog();
        }
    }

    void InitializeTheShape()
    {
        int pixelCount = beEliminatedShapeSize.x * beEliminatedShapeSize.y;
        shapeLocalPosition = new Vector2Int[pixelCount];

        int halfX = Mathf.FloorToInt(beEliminatedShapeSize.x * 0.5f);
        int remainingX = beEliminatedShapeSize.x - halfX;
        int halfY = Mathf.FloorToInt(beEliminatedShapeSize.y * 0.5f);
        int remainingY = beEliminatedShapeSize.y - halfY;

        int index = 0;
        for (int y = -halfY; y < remainingY; y++)
        {
            for (int x = -halfX; x < remainingX; x++)
            {
                shapeLocalPosition[index] = new Vector2Int(x, y);
                index++;
            }
        }
    }

    void InitializeTheFog()
    {
        int pixelCount = fogDensity.x * fogDensity.y;

        //Deafult color of Fog of war
        Color[] blackColors = new Color[pixelCount];
        for (int i = 0; i < pixelCount; i++)
        {
            blackColors[i] = Color.black;
        }
        fogTexture.SetPixels(blackColors);

        fogTexture.Apply();
    }

    void EliminateFog()
    {
        Vector2 cubePos = new Vector2(player.position.x, player.position.y);
        
        Vector2 originDistanceRatio = (cubePos - fogOriginPoint) / worldSize;
        originDistanceRatio.Set(Mathf.Abs(originDistanceRatio.x), Mathf.Abs(originDistanceRatio.y));
     
        Vector2Int fogCenter = new Vector2Int(Mathf.RoundToInt(originDistanceRatio.x * fogDensity.x), Mathf.RoundToInt(originDistanceRatio.y * fogDensity.y));
        for (int i = 0; i < shapeLocalPosition.Length; i++)
        {
            int x = shapeLocalPosition[i].x + fogCenter.x;
            int y = shapeLocalPosition[i].y + fogCenter.y;
        
            if (x < 0 || x >= fogDensity.x || y < 0 || y >= fogDensity.y)
                continue;

            fogTexture.SetPixel(x, y, Color.clear);
        }

        fogTexture.Apply();
    }
}