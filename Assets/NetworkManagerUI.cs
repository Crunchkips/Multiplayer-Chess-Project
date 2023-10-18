using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    [SerializeField] private Button whiteBtn;
    [SerializeField] private Button blackBtn;

    public TurnManager turnManager;


    private void Awake()
    {
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });

        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
        });

        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });


        whiteBtn.onClick.AddListener(turnManager.AssignWhite);
        blackBtn.onClick.AddListener(turnManager.AssignBlack);

    }



    

}
