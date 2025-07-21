using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class BulletLogic : NetworkBehaviour
{
    [Networked]
    [HideInInspector] public Vector3 Direction { get; set; }
    [Networked]
    [HideInInspector] public NetworkId ShooterObjectID { get; set; }
    [SerializeField] private NetworkRigidbody3D rb;

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (Direction != null)
        {
            if (Direction != Vector3.zero)
                rb.Rigidbody.AddForce(Direction * 100, ForceMode.Force);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (collision.transform.GetComponent<NetworkObject>().Id != ShooterObjectID)
                RPC_DestroyMe();
        }
    }

    [Rpc]
    public void RPC_DestroyMe()
    {
        var runner = FindFirstObjectByType<NetworkRunner>();
        if (runner.IsSharedModeMasterClient)
            runner.Despawn(GetComponent<NetworkObject>());
    }
}
