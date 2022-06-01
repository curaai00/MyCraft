using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class World : MonoBehaviour
{
    public static readonly int WidthByChunk = 100;
    public static readonly int SizeByVoxels = WidthByChunk * Chunk.Width;

    [SerializeField]
    public Transform player;
    public BlockTable BlockTable;

    public Chunk[,] chunks = new Chunk[WidthByChunk, WidthByChunk];

    private void Start()
    {
        BlockTable = new();

        Cursor.lockState = CursorLockMode.Locked;

        int ctr = WidthByChunk / 2;
        player.position = new Vector3(ctr * Chunk.Width, Chunk.Height - 20, ctr * Chunk.Width);
    }

    public Block GenerateBlock(Vector3Int pos)
    {
        Block TerrarianBlock()
        {
            int yPos = Mathf.FloorToInt(pos.y);
            float noise = NoiseHelper.Perlin(new Vector2(pos.x, pos.z), 0f, 0.1f);
            var terrianHeight = Mathf.FloorToInt(noise * Chunk.Height);

            Block block;
            if (yPos <= terrianHeight)
                block = new Block(2);// grass
            else
                block = new Blocks.Air();
            return block;
        }

        return TerrarianBlock();
    }

    public void EditBlock(Vector3Int pos, Block block)
    {
        var coord = ToChunkCoord(pos);
        GetChunk(coord.Item1).EditBlock(block, coord.Item2);
    }

    public Block GetBlock(Vector3 worldPos)
    {
        var pair = ToChunkCoord(worldPos);
        return GetChunk(pair.Item1).GetBlock(pair.Item2);
    }

    public (ChunkCoord, Vector3Int) ToChunkCoord(in Vector3Int worldPos)
    {
        int x = worldPos.x;
        int y = worldPos.y;
        int z = worldPos.z;

        int chunkX = x / Chunk.Width;
        int chunkZ = z / Chunk.Width;

        x -= chunkX * Chunk.Width;
        z -= chunkZ * Chunk.Width;

        var a = new ChunkCoord(chunkX, chunkZ);
        var b = new Vector3Int(x, y, z);
        return (a, b);
    }
    public (ChunkCoord, Vector3Int) ToChunkCoord(in Vector3 worldPos) => ToChunkCoord(Vector3Int.FloorToInt(worldPos));
    public Chunk GetChunk(Vector3Int worldPos) => GetChunk(ToChunkCoord(worldPos).Item1);
    public Chunk GetChunk(ChunkCoord coord) => chunks[coord.x, coord.z];
    public bool IsSolidBlock(in Vector3 worldPos)
    {
        var pos = ToChunkCoord(worldPos);
        if (pos.Item2.y < 0 || Chunk.Height <= pos.Item2.y)
            return false;

        if (GetChunk(pos.Item1) != null && GetBlock(worldPos) != null)
            return GetBlock(worldPos).isSolid;
        return false;
    }
}
