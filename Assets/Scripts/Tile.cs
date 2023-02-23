using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private int type;
    public int DiffType { get { return type; } }
}
