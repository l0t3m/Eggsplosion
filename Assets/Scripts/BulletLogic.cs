using Fusion;
using Fusion.Addons.Physics;
using System.Collections;
using UnityEngine;

public class BulletLogic : NetworkBehaviour
{
    [Networked]
    [HideInInspector] public Vector3 Direction { get; set; }
    [Networked]
    [HideInInspector] public NetworkId ShooterObjectId { get; set; }

    [SerializeField] private Rigidbody rb;

    private float timeToDespawn;

    public override void Spawned()
    {
        base.Spawned();
        
        StartCoroutine(DoBulletLogic());
        timeToDespawn = 5;
    }

    private IEnumerator DoBulletLogic()
    {
        if (Direction != Vector3.zero)
        {
            transform.LookAt(Direction);
            rb.AddForce(Direction * 19f, ForceMode.Impulse);
            yield return null;
        }
        else
            yield return new WaitForSeconds(0.01f);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();


        if (Direction != null)
        {
            if (Direction != Vector3.zero)
                transform.position += Direction*1.5f;
        }

        timeToDespawn -= Time.fixedDeltaTime;
        if (timeToDespawn <= 0)
            RPC_DestroyMe();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            RPC_DestroyMe(collision.gameObject.GetComponent<NetworkObject>().Id);
        }
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    public void RPC_DestroyMe(NetworkId pushMe)
    {
        if (Runner.TryFindObject(pushMe, out NetworkObject objectToPush))
        {
            if (Vector3.Distance(objectToPush.transform.position, transform.position) < 5)
                objectToPush.GetComponent<Rigidbody>().AddForce(Direction * 15, ForceMode.Impulse);
        }
        if (Runner.IsServer)
            Runner.Despawn(Object);
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    public void RPC_DestroyMe()
    {
        if (Runner.IsServer)
            Runner.Despawn(Object);
    }
}
