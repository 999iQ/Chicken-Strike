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
    [Tooltip("Здоровье игрока")] // подсказка
    public float Maxhp = 100;
    public float hp = 100;
    [SerializeField] private Image _HpBar;
    [SerializeField] private GameObject _Ui; // интерфейс игрока для удаления клона


    [Header("Оружие игрока")]
    [Tooltip("Массив оружий (предметов игрока)")]
    [SerializeField] private Item[] items;
    private int itemIndex, previousItemIndex = -1;
    public List<Transform> weaponChildLayer = new List<Transform>(); // выключаем слой рендера у оружий для 2 камеры


    [Header("Характеристики движения игрока")]
    public float speed;
    public float smoothTime;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    public float jumpForce;
    private bool _isGround;
    [SerializeField] private IsGroundCheck _isGroundCheck; // ссылка на скрипт проверки земли под ногами 
    private Rigidbody rigBody;

    
    [Header("Чувствительность мыши")]
    public float sensivityMouse;

    //[Header("Статистика игрока")]

    [Header("Настройки фотона и камеры")]
    public GameObject playerBody; // тело игрока для off рендера in camera

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
        if(stream.IsWriting) //отправляем переменные для синхрона
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
            photonView.Owner.NickName = PlayerPrefs.GetString("PlayerName"); // присваеваем его ник--
        }
        
    }
    private void Start()
    {
        transform.name = photonView.Owner.NickName;
        hp = Maxhp;
        rigBody = GetComponent<Rigidbody>();
        EquipItem(0); // индекс оружия = 0

        if (photonView.IsMine)
        {
            spawnerManager = FindObjectOfType<SpawnManager>();
            Cursor.lockState = CursorLockMode.Locked;
            playerBody.SetActive(false); // отключаем тело игрока в камере
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); // У ЛОКАЛЬНОГО ИГРОКА ИНГОРИМ ЛУЧИ, ФИКС СУИЦИДА 
        }
        

        if (!photonView.IsMine)
        {
            // фикс бага, чужое оружие видно через стены
            // var go in gameObject.GetAllChilds()
            foreach (Transform trans in weaponChildLayer)
                trans.gameObject.layer = 0; // default layer = 0
            

            Destroy(_Ui); // фикс двойного интерфейса 

            // если это не мы, то отключаем камеру и оружие
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(cameraRenderWeapon);
            scriptPlayerContr.enabled = false;

            // fix jump отключаем одну из двух физик чтобы прыжок был плавным
            Destroy(rigBody);
        }
    }

    private void Update()
    {
        // проверка на локального игрока
        if (!photonView.IsMine)
            return;


        if (!pause) //смотрим и бегаем пока нет паузы
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

    [PunRPC] // надо для передачи данных 
    public void RPC_TakeDamage(float damage, string Killer)
    {

        hp -= damage;
        _HpBar.fillAmount = hp / Maxhp; // хп бар

        if (hp <= 0) // Респавн
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
        // получаем список игроков с именем который нас продомажил и убил но не мы
        var ListLastDamager = PhotonNetwork.PlayerList.ToList().Find(x => x.NickName == Killer && x.NickName != photonView.Owner.NickName);
        Debug.Log(ListLastDamager + " xaxaxaxaxa");

        // ЕСЛИ ТАКОЙ ИГРОК ЕСТЬ (МЫ НЕ САМИ СДОХЛИ)
        if (ListLastDamager != null)
        {
            Hashtable PlayerCustomProps = new Hashtable();

            // ТО ДОБАВЛЯЕМ +1 КИЛЛ ++ НАШЕМУ ЛАСТ ДАМАГЕРУ
            PlayerCustomProps.Add("Kills", ((int)ListLastDamager.CustomProperties["Kills"]) + 1);

            // СОХРАНЯЕМ И СООБЩАЕМ ОБ ИЗМЕНЕНИЯХ ВСЕМ ИГРОКАМ
            ListLastDamager.SetCustomProperties(PlayerCustomProps);
        }
        // смерти ++ 
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
            // переключение оружий на кнопки цифры
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                //EquipItem(i);
                photonView.RPC("EquipItem", RpcTarget.All, i);
                break;
            }
        }

        // ПЕРЕКЛЮЧЕНИЕ ОРУЖИЯ КОЛЁСИКОМ МЫШКИ
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            // fix выхода за пределы массива оружий
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
            // fix выхода за пределы массива оружий
            if (itemIndex <= 0)
            {
                photonView.RPC(nameof(EquipItem), RpcTarget.All, items.Length - 1);
            }
            else
            {
                photonView.RPC(nameof(EquipItem), RpcTarget.All, itemIndex - 1);
            }
        }

        // выстрел
        if(Input.GetMouseButton(0))
        {
            items[itemIndex].Use();
        }
    }

    [PunRPC]
    private void EquipItem(int _index) // ОРУЖИЕ 
    {
        // fix бага двойного нажатия и убирания оружия
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;

        // активируем оружие с этим индексом в массиве оружий
        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            // после переключения оружия, прошлое оружие стан. не активным
            items[previousItemIndex].itemGameObject.SetActive(false);
        }
        // индекс прошлого оружия приравнивается к текущему
        previousItemIndex = itemIndex;

    }

    private void Look()
    {
        // игрок поворачивается
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * sensivityMouse);

        // камера, с ограничениями внизу и вверху
        _mouseY += Input.GetAxisRaw("Mouse Y") * sensivityMouse;
        _mouseY = Mathf.Clamp(_mouseY, -90f, 90f);
        cameraHolder.transform.localEulerAngles = Vector3.left * _mouseY;
    }

    private void FixedUpdate()
    {
        // фикс движения от фпс
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
            // пауза
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
