using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLogic : NetworkBehaviour, IStateAuthorityChanged
{
    [Networked]
    [HideInInspector] public int PlayerID { get; set; }
    [SerializeField] private MeshRenderer m_renderer;
    private NetworkRunner runner;

    [SerializeField] private NetworkObject bullet;
    [SerializeField] private Rigidbody rb;

    private float maxShootTime = 1.5f;
    private float actualShootTime = 0;

    public override void Spawned()
    {
        base.Spawned();
        runner = FindFirstObjectByType<NetworkRunner>();
        
        StartCoroutine(WaitForID());
    }

    private IEnumerator WaitForID()
    {
        if (PlayerID == 0)
            yield return new WaitForSeconds(1);
        else
        {          
            if (PlayerID == runner.LocalPlayer.AsIndex)
            {
                GetComponent<NetworkObject>().RequestStateAuthority();
                runner.ProvideInput = true;
            }
            yield return null;
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        actualShootTime -= Time.fixedDeltaTime;
        if (runner != null)
        {
            if (!Object.HasStateAuthority)
                return;

            Vector3 move = Vector3.zero;

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
                RPC_Shoot(runner.LocalPlayer.PlayerId, Object.Id, origin, direction, transform.rotation);
            }

            if (move != Vector3.zero)
                rb.AddForce(move, ForceMode.VelocityChange);
        }
    }

    [Rpc]
    private async void RPC_Shoot(int shooter, NetworkId objID, Vector3 origin, Vector3 direction, Quaternion rotation)
    {
        if (runner.IsSharedModeMasterClient)
        {
            var obj = await runner.SpawnAsync(bullet, origin+direction*1.5f, rotation);
            BulletLogic bl = obj.GetComponent<BulletLogic>();
            bl.Direction = direction;
            bl.ShooterID = shooter;
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

    public void StateAuthorityChanged()
    {
        runner = FindFirstObjectByType<NetworkRunner>();
    }
}
