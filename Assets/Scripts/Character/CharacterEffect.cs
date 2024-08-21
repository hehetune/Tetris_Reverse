using UnityEngine;

namespace Character
{
    public class CharacterEffect : MonoBehaviour
    {
        // [SerializeField] GameObject groundDust;
        // [SerializeField] GameObject wallDust;
        private Vector3 groundDustOffset = new Vector3(0f, -0.1f, 0f);
        private bool groundDustCoroutineAllow = false;
        private float groundDustEmissionRate = 0.1f;
        private bool wallDustCoroutineAllow = false;
        private float wallDustEmissionRate = 0.25f;
        
        // private void HandleGroundEffect()
        // {
        //     if (grounded && dirX != 0 && groundDustCoroutineAllow)
        //     {
        //         StartCoroutine("SpawnGroundCloud");
        //         groundDustCoroutineAllow = false;
        //     }
        //
        //     if (dirX == 0 || !grounded)
        //     {
        //         if (SpawnGroundCloud() != null) StopCoroutine("SpawnGroundCloud");
        //         groundDustCoroutineAllow = true;
        //     }
        // }
        //
        // private void HandleWallEffect()
        // {
        //     if (isSliding && wallDustCoroutineAllow)
        //     {
        //         StartCoroutine("SpawnWallDust");
        //         wallDustCoroutineAllow = false;
        //     }
        //
        //     if (!isSliding)
        //     {
        //         if (SpawnWallDust() != null) StopCoroutine("SpawnWallDust");
        //         wallDustCoroutineAllow = true;
        //     }
        // }
        
        // IEnumerator SpawnGroundCloud()
        // {
        //     while (grounded)
        //     {
        //         // FXSpawner.Instance.Spawn(FXSpawner.GroundDust, groundCheck.position + groundDustOffset,
        //         //     Quaternion.identity);
        //         yield return new WaitForSeconds(groundDustEmissionRate);
        //     }
        // }
        //
        // IEnumerator SpawnWallDust()
        // {
        //     while (isSliding)
        //     {
        //         // FXSpawner.Instance.Spawn(FXSpawner.WallDust, m_WallCheck.position, Quaternion.identity);
        //         yield return new WaitForSeconds(wallDustEmissionRate);
        //     }
        // }
    }
}