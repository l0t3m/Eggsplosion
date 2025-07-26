using Fusion;
using Fusion.Addons.Physics;
using System.Collections;
using UnityEngine;

public class BulletLogic : NetworkBehaviour
{
    [Networked]
    [HideInInspector] public Vector3 Direction { get; set; }
    [Networked]
    [HideInInspector] public int ShooterID { get; set; } = -1;
    [Networked]
    [HideInInspector] public NetworkId ShooterObjectId { get; set; }

    [SerializeField] private Rigidbody rb;

    private float timeToDespawn;

    public override void Spawned()
    {
        base.Spawned();
        
        StartCoroutine(ClaimBullet());
        timeToDespawn = 5;
    }

    private IEnumerator ClaimBullet()
    {
        if (ShooterID != -1)
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

    [Rpc]
    public void RPC_DestroyMe(NetworkId pushMe)
    {
        NetworkRunner runner = FindFirstObjectByType<NetworkRunner>();
        if (runner.TryFindObject(pushMe, out NetworkObject objectToPush))
        {
            if (Vector3.Distance(objectToPush.transform.position, Object.transform.position) < 5)
                objectToPush.GetComponent<Rigidbody>().AddForce(Direction * 15, ForceMode.Impulse);
        }
        if (runner.IsSharedModeMasterClient)
            runner.Despawn(Object);
    }

    [Rpc]
    public void RPC_DestroyMe()
    {
        NetworkRunner runner = FindFirstObjectByType<NetworkRunner>();
        if (runner.IsSharedModeMasterClient)
            runner.Despawn(Object);
    }
}
