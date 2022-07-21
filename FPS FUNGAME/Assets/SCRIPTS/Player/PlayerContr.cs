using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class PlayerContr : MonoBehaviourPunCallbacks, IDamageable
{
    [Tooltip("�������� ������")] // ���������
    public float Maxhp = 100;
    public float hp = 100;
    [SerializeField] private Image _HpBar;
    [SerializeField] private GameObject _Ui; // ��������� ������ ��� �������� �����


    [Header("������ ������")]
    [Tooltip("������ ������ (��������� ������)")]
    [SerializeField] private Item[] items;
    private int itemIndex, previousItemIndex = -1;
    public List<Transform> weaponChildLayer = new List<Transform>(); // ��������� ���� ������� � ������ ��� 2 ������


    [Header("�������������� �������� ������")]
    public float speed;
    public float smoothTime;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    public float jumpForce;
    private bool _isGround;
    [SerializeField] private IsGroundCheck _isGroundCheck; // ������ �� ������ �������� ����� ��� ������ 
    private Rigidbody rigBody;

    
    [Header("���������������� ����")]
    public float sensivityMouse;

    //[Header("���������� ������")]

    [Header("��������� ������ � ������")]
    public GameObject playerBody; // ���� ������ ��� off ������� in camera

    public Transform itemHolder;

    public GameObject cameraHolder;
    public GameObject cameraRenderWeapon;
    private float _mouseY;
    public PlayerContr scriptPlayerContr;
    public GameObject pauseCanvas;
    private bool pause = false;

    private SpawnManager spawnerManager;


    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting) //���������� ���������� ��� ��������
        {
            stream.SendNext(kills);
            stream.SendNext(deaths);
        }
        else
        {
            kills = (int)stream.ReceiveNext();
            deaths = (int)stream.ReceiveNext();
        }
    }*/


    private void Awake()
    {
        if (photonView.IsMine)
        {
            photonView.Owner.NickName = PlayerPrefs.GetString("PlayerName"); // ����������� ��� ���--
        }
        
    }
    private void Start()
    {
        transform.name = photonView.Owner.NickName;
        hp = Maxhp;
        rigBody = GetComponent<Rigidbody>();
        EquipItem(0); // ������ ������ = 0

        if (photonView.IsMine)
        {
            spawnerManager = FindObjectOfType<SpawnManager>();
            Cursor.lockState = CursorLockMode.Locked;
            playerBody.SetActive(false); // ��������� ���� ������ � ������
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); // � ���������� ������ ������� ����, ���� ������� 
        }
        

        if (!photonView.IsMine)
        {
            // ���� ����, ����� ������ ����� ����� �����
            // var go in gameObject.GetAllChilds()
            foreach (Transform trans in weaponChildLayer)
                trans.gameObject.layer = 0; // default layer = 0
            

            Destroy(_Ui); // ���� �������� ���������� 

            // ���� ��� �� ��, �� ��������� ������ � ������
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(cameraRenderWeapon);
            scriptPlayerContr.enabled = false;

            // fix jump ��������� ���� �� ���� ����� ����� ������ ��� �������
            Destroy(rigBody);
        }
    }

    private void Update()
    {
        // �������� �� ���������� ������
        if (!photonView.IsMine)
            return;


        if (!pause) //������� � ������ ���� ��� �����
        {
            GetInput();
            Look();
            ScrollItems();
            _isGround = _isGroundCheck.IsGround;
        }
        else
        {
            moveAmount = new Vector3(0, 0, 0);
            smoothMoveVelocity = new Vector3(0, 0, 0);
        }
        
        PauseManager();
        
    }

    [PunRPC] // ���� ��� �������� ������ 
    public void RPC_TakeDamage(float damage, string Killer)
    {

        hp -= damage;
        _HpBar.fillAmount = hp / Maxhp; // �� ���

        if (hp <= 0) // �������
        {
            Died(Killer);
        }
        
    }

    public void TakeDamage(float damage, string lastDamager)
    {
        photonView.RPC(nameof(RPC_TakeDamage), photonView.Owner, damage, lastDamager);
    }

    public void Died(string Killer)
    {
        // �������� ������ ������� � ������ ������� ��� ���������� � ���� �� �� ��
        var ListLastDamager = PhotonNetwork.PlayerList.ToList().Find(x => x.NickName == Killer && x.NickName != photonView.Owner.NickName);
        Debug.Log(ListLastDamager + " xaxaxaxaxa");

        // ���� ����� ����� ���� (�� �� ���� ������)
        if (ListLastDamager != null)
        {
            Hashtable PlayerCustomProps = new Hashtable();

            // �� ��������� +1 ���� ++ ������ ���� ��������
            PlayerCustomProps.Add("Kills", ((int)ListLastDamager.CustomProperties["Kills"]) + 1);

            // ��������� � �������� �� ���������� ���� �������
            ListLastDamager.SetCustomProperties(PlayerCustomProps);
        }
        // ������ ++ 
        Hashtable PlayerCustomProps_2 = new Hashtable();
        PlayerCustomProps_2.Add("Deaths", ((int)PhotonNetwork.LocalPlayer.CustomProperties["Deaths"]) + 1);
        PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerCustomProps_2);

        PhotonNetwork.Destroy(gameObject);
        spawnerManager.SpawnPlayers();
    }

    void ScrollItems()
    {
        for (int i = 0; i < items.Length; ++i)
        {
            // ������������ ������ �� ������ �����
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                //EquipItem(i);
                photonView.RPC("EquipItem", RpcTarget.All, i);
                break;
            }
        }

        // ������������ ������ ��˨����� �����
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            // fix ������ �� ������� ������� ������
            if (itemIndex >= items.Length - 1)
            {
                photonView.RPC(nameof(EquipItem), RpcTarget.All, 0);
            }
            else
            {
                photonView.RPC(nameof(EquipItem), RpcTarget.All, itemIndex + 1);
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            // fix ������ �� ������� ������� ������
            if (itemIndex <= 0)
            {
                photonView.RPC(nameof(EquipItem), RpcTarget.All, items.Length - 1);
            }
            else
            {
                photonView.RPC(nameof(EquipItem), RpcTarget.All, itemIndex - 1);
            }
        }

        // �������
        if(Input.GetMouseButton(0))
        {
            items[itemIndex].Use();
        }
    }

    [PunRPC]
    private void EquipItem(int _index) // ������ 
    {
        // fix ���� �������� ������� � �������� ������
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;

        // ���������� ������ � ���� �������� � ������� ������
        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            // ����� ������������ ������, ������� ������ ����. �� ��������
            items[previousItemIndex].itemGameObject.SetActive(false);
        }
        // ������ �������� ������ �������������� � ��������
        previousItemIndex = itemIndex;

    }

    private void Look()
    {
        // ����� ��������������
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * sensivityMouse);

        // ������, � ������������� ����� � ������
        _mouseY += Input.GetAxisRaw("Mouse Y") * sensivityMouse;
        _mouseY = Mathf.Clamp(_mouseY, -90f, 90f);
        cameraHolder.transform.localEulerAngles = Vector3.left * _mouseY;
    }

    private void FixedUpdate()
    {
        // ���� �������� �� ���
        rigBody.MovePosition(rigBody.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
    private void GetInput()
    {
        // movement
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * speed, ref smoothMoveVelocity, smoothTime);

        // jump
        if (Input.GetKeyDown(KeyCode.Space) && _isGround)
        {
            rigBody.AddForce(transform.up * jumpForce);
        }

    }

    private void PauseManager()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // �����
            if (pauseCanvas.activeSelf == false)
            {
                pauseCanvas.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                pause = true;
            }
            else
            {
                pauseCanvas.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                pause = false;
            }
        }
    }




}
