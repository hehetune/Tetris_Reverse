// using UnityEngine;
//
// namespace TetrisCore
// {
//     public class Ghost : MonoBehaviour
//     {
//         public Sprite sprite;
//         public Board board;
//         public Piece trackingPiece;
//
//         public Vector3Int[] cells { get; private set; }
//         public Vector3Int position { get; private set; }
//
//         private GameObject[] spriteObjects;
//
//         private void Awake()
//         {
//             this.cells = new Vector3Int[4];
//             this.spriteObjects = new GameObject[4];
//         }
//
//         private void LateUpdate()
//         {
//             if (!TetrisGameManager.Instance.isGameStarted) return;
//             Clear();
//             Copy();
//             Drop();
//             Set();
//         }
//
//         public void Clear()
//         {
//             for (int i = 0; i < this.spriteObjects.Length; i++)
//             {
//                 if (spriteObjects[i] != null)
//                 {
//                     Destroy(spriteObjects[i]);
//                 }
//             }
//         }
//
//         private void Copy()
//         {
//             for (int i = 0; i < this.cells.Length; i++)
//             {
//                 this.cells[i] = this.trackingPiece.cells[i];
//             }
//         }
//
//         private void Drop()
//         {
//             Vector3Int position = this.trackingPiece.position;
//
//             int current = position.y;
//             int bottom = -this.board.YMin / 2 - 1;
//
//             this.board.Clear(this.trackingPiece);
//
//             for (int row = current; row >= bottom; row--)
//             {
//                 position.y = row;
//                 if (this.board.IsValidPosition(this.trackingPiece, position))
//                 {
//                     this.position = position;
//                 }
//                 else
//                 {
//                     break;
//                 }
//             }
//
//             this.board.Set(this.trackingPiece);
//         }
//
//         private void Set()
//         {
//             for (int i = 0; i < this.cells.Length; i++)
//             {
//                 Vector3Int cellPosition = this.cells[i] + this.position;
//                 CreateSprite(cellPosition, i);
//             }
//         }
//
//         private void CreateSprite(Vector3Int position, int index)
//         {
//             GameObject obj = new GameObject($"GhostBlock_{index}");
//             obj.transform.position = position;
//             SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
//             renderer.sprite = sprite;
//             obj.transform.parent = transform;
//
//             spriteObjects[index] = obj;
//         }
//     }
// }
