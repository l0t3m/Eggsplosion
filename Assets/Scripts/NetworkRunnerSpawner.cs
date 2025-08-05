using Fusion;
using Fusion.Addons.Physics;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkRunnerSpawner
{
    public static NetworkRunner SpawnNetworkRunner()
    {
        NetworkRunner runner = Object.Instantiate(new GameObject()).AddComponent<NetworkRunner>();
        runner.AddComponent<RunnerSimulatePhysics3D>();
        return runner;
    }
}
