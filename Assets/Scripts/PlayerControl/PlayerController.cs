using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject cameraHolder;

    [Header("Player stats")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float mouseSensitivity;        // чутливість мишки
    [SerializeField] private float jumpForce;
    [SerializeField] private float smoothTime;              // коефіціент

    private float verticalLookRotation;
    private Vector3 smoothMove;
    private Vector3 moveAmount;
    private Rigidbody rb;
    private PhotonView pnView;

    public bool isGround;

    [Header("Player weapon")]
    [SerializeField] Item [] items;

    private int itemIndex;
    private int prevItemIdex = -1;

    [Header("Player UI")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    private PlayerManager playerManager;

    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject ui;

    [SerializeField] private Animator visualDamageAnimator;

    private void Awake()
    {
        currentHealth = maxHealth;
        //
        pnView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        //
        playerManager = PhotonView.Find((int) pnView.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(!pnView.IsMine)
        {
            Destroy(playerCamera);
            Destroy(rb);
            //
            Destroy(ui);
        }
        else
        {
            EquipItem(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!pnView.IsMine)
        {
            return;
        }

        Look();
        Movement();
        //
        Jump();
        SelectWeapon();
        UseItem();
    }

    private void SelectWeapon()
    {
        for(int i = 0; i < items.Length; i++)
        {
            if(Input.GetKeyDown((i+1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }
    }

    private void EquipItem(int index)
    {
        if(index == prevItemIdex)
        {
            return;
        }

        itemIndex = index;
        items[itemIndex].itemGameObject.SetActive(true);

        if(prevItemIdex != -1)
        {
            items[prevItemIdex].itemGameObject.SetActive(false);
        }

        prevItemIdex = itemIndex;

        if(pnView.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("index", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!pnView.IsMine && targetPlayer == pnView.Owner)
        {
            EquipItem((int)changedProps["index"]);
        }
    }

    private void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -80f, 90f);

        // playerCamera.transform.localEulerAngles = Vector3.left * verticalLookRotation;
        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    private void Movement()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount,
                                        moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed),
                                        ref smoothMove,
                                        smoothTime);
    }

    private void FixedUpdate()
    {
        if(!pnView.IsMine)
        {
            return;
        }
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void GroundState(bool isGround)
    {
        this.isGround = isGround;
    }

    public void TakeDamage(float damage)
    {
        pnView.RPC("RPC_Damage", RpcTarget.All, damage);
    }

    private void UseItem()
    {
        if(Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }
    }

    [PunRPC]
    void RPC_Damage(float damage)
    {
        if(!pnView.IsMine)
        {
            return;
        }
        //
        currentHealth -= damage;
        healthBar.fillAmount = currentHealth / maxHealth;
        visualDamageAnimator.Play("InAction");
        //
        if(currentHealth <= 0)
        {
            playerManager.Die(transform.position);
        }
    }
}
