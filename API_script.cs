//Script used in one of the programs
//I removed unecessary/unimportant information to focus on the core function
//At the moment this is simply the script I created to connect tiktok livestreams to unity programs and spawn gameobjects depending on likes/comments and gifts in this instance

using TikTokLiveSharp.Client;
using System;
using TikTokLiveSharp;
using TikTokLiveUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TikTokLiveSharp.Events;
using TikTokLiveSharp.Models.Protobuf.Messages;
using TikTokLiveSharp.Events.Objects;
using Text = UnityEngine.UI.Text;
using TikTokLiveSharp.Models.HTTP;

namespace TikTokLiveUnity
{
    public class TikTokManager : MonoBehaviour
    {
        private static TikTokManager instance;

        public static TikTokManager Instance
        {
            get { return instance; }
        }

        public InputField inputfield; // inputfield where user enters their TikTok username. The connector will connect to the livestream that is running on the corresponding account

        // The gameobjects (objects in game) to be spawned. In this game they are spawned when the stream viewers interact with the stream
        // The gameobject to be spawned (1 or 2) is dependant on the interaction the viewers sends
        public GameObject gameObject1;
        public GameObject gameObject2;

        public string username; // used to store username of user

        // 'Click' method is activated by the 'Start Connector' button in the program
        // It gets the username inputted by the user and connects to that account
        public void Click()
        {
            username = inputfield.text;
            DontDestroyOnLoad(gameObject);

            //conections for comments, likes and gifts
           TikTokLiveManager.Instance.OnChatMessage += OnComment;
           TikTokLiveManager.Instance.OnLike += OnLike;
           TikTokLiveManager.Instance.OnGiftMessage += OnGiftMessage;

            // connect method
            Connect();
        }

        private void Connect()
        {
            bool connected = TikTokLiveManager.Instance.Connected;
            bool connecting = TikTokLiveManager.Instance.Connecting;
            if (connected || connecting)
                Debug.Log("Already connected or connecting");
            else TikTokLiveManager.Instance.ConnectToStreamAsync(username, Debug.LogException);
            Debug.Log("Connecting");
        }

        // On comment received (a viewer comments on the livestream) a gameobject spawns
        private void OnComment(TikTokLiveClient sender, Chat e)
        {
            SpawnPrefab(gameObject1);
        }

        // Gift handling / Donations
        // On gift received, depending on the gift recieved a specific amount of gameobjects will spawn
        void OnGiftMessage(TikTokLiveClient sender, TikTokLiveSharp.Events.GiftMessage e)
        {
            {
                var gift = e.Gift;
                string giftName = gift.Name;
                long amount = e.Amount;

                if (giftName == "Rose") // Received a gift (TikTok donations)
                {
                    // spawn 10 gameobjects
                    for (int i = 0; i < 10; i++)
                    {
                        SpawnPrefab(gameObject1);
                    }
                   
                    Debug.Log("Received a rose gift");
                }
               
                else if (giftName == "TikTok")
                {
                    // spawn 10 gameobjects
                    for (int i = 0; i < 10; i++)
                    {
                        SpawnPrefab(gameObject2);
                    }
                    Debug.Log("Received a TIKTOK gift");
                }
               
                else if (giftName == "Cap")
                {
                    // spawn 35 gameobjects
                    for (int i = 0; i < 35; i++)
                    {
                        SpawnPrefab(gameObject1);
                    }
                    Debug.Log("Received a cap gift");
                }

                else if (giftName == "Paper Crane")
                {
                    // spawn 35 gameobjects
                    for (int i = 0; i < 35; i++)
                    {
                        SpawnPrefab(gameObject2);
                    }
                    Debug.Log($"Received a paper crane gift");
                }
            }
        }

        // On likes received spawn gameobject
        private void OnLike(TikTokLiveClient sender, Like like)
        {
            SpawnPrefab(gameObject2);
        }

        //method to spawn corresponding gameobjects
        private void SpawnPrefab(GameObject prefab) // object to spawn passed through SpawnPrefab method
        {
            GameObject obj = Instantiate(prefab); // instantiate the gameobject
        }

        // if the tiktoklivemanager no longer exists end all connections
        private void OnDestroy()
        {
            if (!TikTokLiveManager.Exists)
                return;
               TikTokLiveManager.Instance.OnChatMessage -= OnComment;
            TikTokLiveManager.Instance.OnLike -= OnLike;
            TikTokLiveManager.Instance.OnGiftMessage -= OnGiftMessage;
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }  
    }
}
