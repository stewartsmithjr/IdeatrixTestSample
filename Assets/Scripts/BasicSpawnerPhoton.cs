using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BasicSpawnerPhoton : SimulationBehaviour, INetworkRunnerCallbacks 
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    [SerializeField] Camera _camera;
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    {
        if (player == Runner.LocalPlayer)
        {
            _camera.gameObject.SetActive(false);

            // Create a unique position for the player
            Vector3 spawnPosition = PlayerPos((ulong)player.PlayerId);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

   
    public void OnInput(NetworkRunner runner, NetworkInput input) 
    {
       
    }


    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    private NetworkRunner _runner;
    private void OnEnable()
    {
       
        royaleSpawns.Add(spawns[0].position);

        for (int i = 0; i < 50; i++)
        {
            bool val = SpawnPosList(index);
            if (val == false)
            {
                SpawnPosList(index);
            }
        }


    }
  
   
    

  


    #region SpawnManager
    public List<Transform> spawns;

    
    private void Update()
    {

    }

    [SerializeField] private List<Vector3> royaleSpawns = new List<Vector3>();
    [SerializeField] private Transform spawnXmin;
    [SerializeField] private Transform spawnYmin;
    [SerializeField] private Transform spawnZmin;
    [SerializeField] private Transform spawnXmax;
    [SerializeField] private Transform spawnYmax;
    [SerializeField] private Transform spawnZmax;

    [SerializeField] int index = 0;

    public Vector3 PlayerPos(ulong clientId)
    {
        Vector3 pos = royaleSpawns[(int)clientId];
        royaleSpawns.RemoveAt((int)clientId);

        return pos;
    }
    [SerializeField] int aiCount;
   
    bool SpawnPosList(int dex)
    {
        float xDif = Mathf.Abs(royaleSpawns[dex].x - playerSpawn().x);

        float zDif = Mathf.Abs(royaleSpawns[dex].z - playerSpawn().z);

        if ((xDif > 10 && zDif > 10))
        {
            royaleSpawns.Add(playerSpawn());

            index++;

            return true;
        }

        else
        {
            return false;
        }
    }
    public Vector3 playerSpawn()
    {
        float xmin = spawnXmin.position.x;
        float xmax = spawnXmax.position.x;
        float ymin = spawnYmin.position.y;
        float ymax = spawnYmax.position.y;
        float zmin = spawnZmin.position.z;
        float zmax = spawnZmax.position.z;
        Vector3 spawnPos = new Vector3(XPos(xmin, xmax), YPos(ymin, ymax), ZPos(zmin, zmax));
        return spawnPos;
    }
    private float XPos(float min, float max)
    {

        return UnityEngine.Random.Range(min, max);

    }

    private float YPos(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    private float ZPos(float min, float max) { return UnityEngine.Random.Range(min, max); }
    #endregion
}