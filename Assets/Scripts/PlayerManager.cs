using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private PhotonView pnView;

    private GameObject controller;

    private GameObject destroyFX;

    private void Awake()
    {
        pnView = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(pnView.IsMine)
        {
            CreateController();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void CreateController()
    {
        Transform point = SpawnManager.Instance.GetSpawnPoint();
        //
        controller = PhotonNetwork.Instantiate(Path.Combine("PlayerController"), point.position, point.rotation, 0, new object[] {pnView.ViewID});
    }

    public void Die(Vector3 position)
    {
        destroyFX = PhotonNetwork.Instantiate(Path.Combine("Destroy_FX"), position, Quaternion.identity, 0, new object[] {pnView.ViewID});
        //
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}
