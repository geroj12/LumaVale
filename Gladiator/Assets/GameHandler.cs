using Unity.Cinemachine;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private CombatDirectionHandler visualFollower;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private GameObject player;

    [SerializeField] private Animator playerAnim;
    
    

    public void RespawnPlayer(Vector3 location)
    {

    }
}
