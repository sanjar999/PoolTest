using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField] private Transform _parent;
    [SerializeField] private Tile[] _tilePrefabs;
    [SerializeField] private float _moveStep = 3;
    [SerializeField] private float _changeTime = 20;
    [SerializeField] private float _spawnTime = 1;
    [SerializeField] private float _returnTime = 3;
    [SerializeField] private int _eachLvlTilesCount = 5;
    [SerializeField] private int _tilesCountAtStart = 20;
    private readonly List<Queue<Tile>> _ques = new();
    private readonly Queue<Tile> _currTiles = new();
    private float _spawnCooldown = 2;
    private float _returnCooldown = 0;
    private float _changeCooldown = 0;
    private float _timeDelta;
    private int _currDiff = 0;
    private Tile _prevTile;

    void Start()
    {
        for (int i = 0; i < _tilePrefabs.Length; i++)
        {
            Queue<Tile> newQue = new();
            for (int k = 0; k < _eachLvlTilesCount; k++)
                newQue.Enqueue(Instantiate(_tilePrefabs[i], _parent));
            _ques.Add(newQue);
        }

        _timeDelta = Time.deltaTime;

        for (int i = 0; i < _tilesCountAtStart; i++)
            Spawn();
    }

    void FixedUpdate()
    {
        _changeCooldown += _timeDelta;
        _spawnCooldown += _timeDelta;
        _returnCooldown += _timeDelta;

        if (_changeCooldown > _changeTime)
            IncreaseDifficulity();

        if (_spawnCooldown > _spawnTime)
            Spawn();

        if (_returnCooldown > _returnTime)
            Return();
    }

    private void IncreaseDifficulity()
    {
        _changeCooldown = 0;
        _currDiff = Mathf.Clamp(_currDiff + 1, 0, _tilePrefabs.Length - 1);
    }

    private void Return()
    {
        _returnCooldown = 0;
        var tempTile = _currTiles.Dequeue();
        _ques[tempTile.DiffType].Enqueue(tempTile);
    }

    private void Return(Tile tile)
    {
        _ques[tile.DiffType].Enqueue(tile);
    }

    private void Spawn()
    {
        _spawnCooldown = 0;

        Queue<Tile> tilesToSpawn = _ques[_currDiff];

        if (tilesToSpawn.Count == 0)
            foreach (var tile in _currTiles)
                Return(tile);

        var tempTile = tilesToSpawn.Dequeue();

        if (_prevTile)
            tempTile.transform.position = _prevTile.transform.position + new Vector3(0, 0, _moveStep);

        tempTile.gameObject.SetActive(true);
        _currTiles.Enqueue(tempTile);
        _prevTile = tempTile;
    }
}