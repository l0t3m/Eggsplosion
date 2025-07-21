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
            if (Mouse.current.leftButton.isPressed)
            {
                Vector3 origin = transform.position;
                Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
                mouseScreenPos.z = 12.75f; // Distance from camera to world point

                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
                Vector3 direction = (worldPos - origin).normalized;
                RPC_Shoot(origin, direction, transform.rotation);
            }

            if (move != Vector3.zero)
                transform.position += move;
        }
    }

    [Rpc]
    private async void RPC_Shoot(Vector3 origin, Vector3 direction, Quaternion rotation)
    {
        var obj = await runner.SpawnAsync(bullet, origin, rotation);
        obj.GetComponent<BulletLogic>().direction = direction;
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
