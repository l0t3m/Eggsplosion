using Fusion;
using UnityEngine;

public class BulletLogic : NetworkBehaviour
{
    [Networked]
    [HideInInspector] public Vector3 direction { get; set; }
    [SerializeField] private Rigidbody rb;

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (direction != Vector3.zero)
            rb.MovePosition(transform.position + direction);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            RPC_DestroyMe();
        }
    }

    [Rpc]
    public void RPC_DestroyMe()
    {
        FindFirstObjectByType<NetworkRunner>().Despawn(GetComponent<NetworkObject>());
    }
}
