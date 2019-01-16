using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class MapGenerator : MonoBehaviour
    {
        public GameObject MapParentGameObject;
        public GameObject TilePrefab;
        public bool GenerateAtStart = true;

        public int Width = 10;
        public int Height = 10;

        public void Start()
        {
            if (GenerateAtStart)
            {
                GenerateMap();
            }
        }

        public void GenerateMap()
        {
            foreach (Transform child in MapParentGameObject.transform.GetEnumerator().ToEnumerable().ToList()) // kill all children
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(child.gameObject);
                }
                else
                {
                    GameObject.Destroy(child.gameObject);
                }
            }

            var positionToTiles = new List<TileModel>();

            for (int y = 0; y < Height; y++)
            {
                for (int x = y%2; x < Width; x++)
                {
                    var tile = Instantiate(TilePrefab);
                    var hexPosition = new MyHexPosition(x+y/2, y);
                    tile.GetComponent<TileModel>().Position = hexPosition;
                    if (x == y%2 )
                    { 
                        tile.GetComponent<TileModel>().Role = TileRole.StartTeam1;
                    }else if (x == Width - 1)
                    {
                        tile.GetComponent<TileModel>().Role = TileRole.StartTeam2;
                    }
                    else
                    {
                        tile.GetComponent<TileModel>().Role = TileRole.Normal;
                    }
                    positionToTiles.Add(tile.GetComponent<TileModel>());
                    tile.transform.SetParent(MapParentGameObject.transform);

                    if (Application.isEditor)
                    {
                        tile.GetComponent<TileView>().MyStart();
                    }
                }
            }

            MapParentGameObject.GetComponent<MapModel>().Tiles = positionToTiles;

            var mouseColliderObject = new GameObject("MapMouseCollider");
            var newCollider = mouseColliderObject.AddComponent<BoxCollider>();
            mouseColliderObject.transform.SetParent(MapParentGameObject.transform);
            mouseColliderObject.transform.localPosition = Vector3.zero;

            newCollider.center = new Vector3(Width * 1.5f / 2 - 1, -0.5f, Height * 1.5f / 2 ); // todo, calculate it using mathematics, this is approximation
            newCollider.size = new Vector3(Width * 1.5f + 2,  1, Height*1.5f + 4);

            mouseColliderObject.layer = UserLayer.TileCollisionLayer.LayerIndex;
        }
    }
}
