using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Collections.Unicode;
using static UnityEngine.UI.Image;

public struct InputStruct : INetworkInput
{
    public bool DidShoot;
    public Vector3 MoveDirection;
    public Vector3 ShootDirection;
    public int ShooterID;
}

public class PlayerLogic : NetworkBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private SkinnedMeshRenderer m_renderer;
    [SerializeField] private NetworkObject bullet;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;

    private float maxShootTime = 1.5f;
    private float actualShootTime = 0;

    public override void Spawned()
    {
        base.Spawned();
        Runner.AddCallbacks(this);
        Runner.ProvideInput = true;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        actualShootTime -= Time.fixedDeltaTime;
        if (GetInput(out InputStruct data))
        {
            animator.SetFloat("Speed", rb.linearVelocity.magnitude);
            if (Runner.IsServer)
            {
                rb.AddForce(data.MoveDirection, ForceMode.VelocityChange);
                if (data.DidShoot)
                    RPC_Shoot(Object.Id, rb.transform.position, data.ShootDirection, transform.rotation);
            }
        }
    }

    private IEnumerator StopAnim(string anim)
    {
        yield return new WaitForSeconds(2);
        animator.SetBool(anim, false);
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    private async void RPC_Shoot(NetworkId objID, Vector3 origin, Vector3 direction, Quaternion rotation)
    {
        if (Object.Runner.IsServer)
        {
            var obj = await Object.Runner.SpawnAsync(bullet, origin+(direction*5f), rotation);
            BulletLogic bl = obj.GetComponent<BulletLogic>();
            bl.Direction = direction;
            bl.ShooterObjectId = objID;
        }
    }

    [Rpc]
    public void RPC_ColorPlayer(Color color)
    {
        Material mat = m_renderer.material;
        mat.color = color;
        m_renderer.material = mat;
    }

    #region callbacks
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
       
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
       
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
  
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
     
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
       
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!Object.HasInputAuthority)
            return;

        Vector3 move = Vector3.zero;
        InputStruct data = new InputStruct();

        if (Keyboard.current.wKey.isPressed)
            move += Vector3.forward;
        if (Keyboard.current.sKey.isPressed)
            move -= Vector3.forward;
        if (Keyboard.current.dKey.isPressed)
            move += Vector3.right;
        if (Keyboard.current.aKey.isPressed)
            move -= Vector3.right;
        if (Mouse.current.leftButton.isPressed && actualShootTime <= 0)
        {
            Vector3 origin = transform.position;
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
            mouseScreenPos.z = 12.75f;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector3 direction = (worldPos - origin).normalized;
            actualShootTime = maxShootTime;
            data.DidShoot = true;
            data.ShootDirection = direction;
            data.ShooterID = Runner.LocalPlayer.PlayerId;
            animator.SetBool("Throwing", true);
            StartCoroutine(StopAnim("Throwing"));
        }
        else
            data.DidShoot = false;
        data.MoveDirection = move;
        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
  
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
      
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
       
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
       
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }
    #endregion
}
