using UnityEngine;

namespace TetrisCore
{
    public class NextPiece : MonoBehaviour
    {
        public Sprite sprite;
        public Vector3Int[] cells { get; private set; }
        public Vector3Int position;

        private GameObject[] spriteObjects;

        private void Awake()
        {
            this.cells = new Vector3Int[4];
            this.spriteObjects = new GameObject[4];
        }

        public void Clear()
        {
            for (int i = 0; i < this.spriteObjects.Length; i++)
            {
                if (spriteObjects[i] != null)
                {
                    Destroy(spriteObjects[i]);
                }
            }
        }

        private void Copy(Vector2Int[] cells)
        {
            for (int i = 0; i < this.cells.Length; i++)
            {
                this.cells[i] = (Vector3Int)cells[i];
            }
        }

        private void Set()
        {
            for (int i = 0; i < this.cells.Length; i++)
            {
                Vector3Int cellPosition = this.cells[i] + this.position;
                CreateSprite(cellPosition, i);
            }
        }

        public void ChangePiece(Vector2Int[] cells, Sprite sprite)
        {
            this.sprite = sprite;
            Clear();
            Copy(cells);
            Set();
        }

        private void CreateSprite(Vector3Int position, int index)
        {
            GameObject obj = new GameObject($"NextPieceBlock_{index}");
            obj.transform.position = position;
            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            obj.transform.parent = transform;

            spriteObjects[index] = obj;
        }
    }
}