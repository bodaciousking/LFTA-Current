using UnityEngine;
using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.CodeGenerating;

public class PlayerData : NetworkBehaviour
{
    [AllowMutableSyncType] public SyncVar<int> PlayerID;
    [AllowMutableSyncType] public SyncVar<int> TeamID;

    public override void OnStartClient()
    {
        if (IsOwner)
        {
            // enable camera/input, show "your turn" UI, etc.
        }
    }
}