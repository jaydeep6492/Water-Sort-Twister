using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace dotmob
{

    public partial class GameManager : Singleton<GameManager>
    {
        public Holder holder;
        
        public static UnityEvent OnChangeCurrency = new UnityEvent();
        public static int TOTAL_GAME_COUNT
        {
            get => PrefManager.GetInt(nameof(TOTAL_GAME_COUNT));
            set => PrefManager.SetInt(nameof(TOTAL_GAME_COUNT),value);
        }
        
        public static bool GAME_VIBRATION
        {
            get => PrefManager.GetBool(nameof(GAME_VIBRATION));
            set => PrefManager.SetBool(nameof(GAME_VIBRATION),value);
        }

        public static int HINT_COUNT
        {
            get => PrefManager.GetInt(nameof(HINT_COUNT)) > 3 ? PrefManager.GetInt(nameof(HINT_COUNT)) : 3;
            set => PrefManager.SetInt(nameof(HINT_COUNT), value);
        }
        public static int UNDO_COUNT
        {
            get => PrefManager.GetInt(nameof(UNDO_COUNT)) > 8 ? PrefManager.GetInt(nameof(UNDO_COUNT)) : 8;
            set => PrefManager.SetInt(nameof(UNDO_COUNT), value);
        }
        public static int SWAP_COUNT
        {
            get => PrefManager.GetInt(nameof(SWAP_COUNT)) > 1 ? PrefManager.GetInt(nameof(SWAP_COUNT)) : 1;
            set => PrefManager.SetInt(nameof(SWAP_COUNT), value);
        }
        
        public static int ADD_BOTTLE
        {
            get => PrefManager.GetInt(nameof(ADD_BOTTLE)) > 1 ? PrefManager.GetInt(nameof(ADD_BOTTLE)) : 1;
            set => PrefManager.SetInt(nameof(ADD_BOTTLE), value);
        }

        private LeaderBoardConfig _leaderBoard = new LeaderBoardConfig()
        {
            playerNames = new List<string>(),
            ranks = new List<string>(),
            scores = new List<string>()
        };

        public LeaderBoardConfig leaderBoard
        {
            get
            {
                _leaderBoard =
                    JsonConvert.DeserializeObject<LeaderBoardConfig>(PrefManager.GetString(nameof(leaderBoard)));
                return _leaderBoard;
            }
            set
            {
                _leaderBoard = value;
                PrefManager.SetString(nameof(leaderBoard), JsonConvert.SerializeObject(_leaderBoard));
            }
        }

        public TubeData tubeData;
        public InventoryData inventoryData;
        public int bottleIndex
        {
            get
            {
                tubeData.currentTubeId = PrefManager.GetInt(nameof(bottleIndex));
                return tubeData.currentTubeId;
            }
            set
            {
                tubeData.currentTubeId = value;
                PrefManager.SetInt(nameof(bottleIndex),  value);
            }
        }

        public static string PLAYER_NAME
        {
            get => PrefManager.GetString(nameof(PLAYER_NAME));
            set => PrefManager.SetString(nameof(PLAYER_NAME), value);
        }
        public static int GOLD_COIN
        {
            get => PrefManager.GetInt(nameof(GOLD_COIN));
            set
            {
                PrefManager.SetInt(nameof(GOLD_COIN), value); 
                OnChangeCurrency?.Invoke();
            }
        }

        public static int GEM_COIN
        {
            get => PrefManager.GetInt(nameof(GEM_COIN));
            set
            {
                PrefManager.SetInt(nameof(GEM_COIN), value);
                OnChangeCurrency?.Invoke();
            }
        }
        
        public static LoadGameData LoadGameData { get; set; }


        protected override void OnInit()
        {
            base.OnInit();
            Application.targetFrameRate = 60;
            GOLD_COIN = !PrefManager.HasKey(nameof(GOLD_COIN)) ? 1000 : GOLD_COIN;
            GEM_COIN = !PrefManager.HasKey(nameof(GEM_COIN)) ? 10 : GEM_COIN;
            holder =  tubeData.tubes[bottleIndex].tubePrefab;
            if (!PrefManager.HasKey(nameof(PLAYER_NAME)))
            {
                PLAYER_NAME = "Player" + DateTime.Now.Day + DateTime.Now.Second + Random.Range('a', 'z');
            }
            if (!PrefManager.HasKey(nameof(GAME_VIBRATION)))
            {
                GAME_VIBRATION = false;
            }
        }

        private void Start()
        {
            if (!PrefManager.HasKey(nameof(leaderBoard)) || leaderBoard.date !=DateTime.Now.Day + "-" + DateTime.Now.Month)
            {
                _leaderBoard.firstScore = Random.Range(750, 1000);
                _leaderBoard.secondScore = Random.Range(450, 700);
                _leaderBoard.thirdScore = Random.Range(250, 449);
                _leaderBoard.date = DateTime.Now.Day + "-" + DateTime.Now.Month;
                var rankRange = 0;
                var scoreRange = 0;
                for (var i = 0; i < 3; i++)
                {
                    switch (i)
                    {
                        case 0:
                            rankRange = 90;
                            scoreRange = 249;
                            break;
                        case 1:
                            rankRange = 120;
                            scoreRange = 140;
                            break;
                        case 2:
                            rankRange = 180;
                            scoreRange = 80;
                            break;
                    }
                    _leaderBoard.scores.Add(Random.Range(scoreRange-80, scoreRange).ToString());
                    _leaderBoard.ranks.Add(Random.Range(rankRange - 21, rankRange).ToString());
                    _leaderBoard.playerNames.Add(RandomName());
                }
                _leaderBoard.playerNames[Random.Range(0, _leaderBoard.playerNames.Count-1)] = PLAYER_NAME;
            }

            leaderBoard = _leaderBoard;
        }
    }
    
    public partial class GameManager
    {
        // ReSharper disable once FlagArgument
        public static void LoadScene(string sceneName, bool showLoading = true, float loadingScreenSpeed = 5f)
        {
            var loadingPanel = SharedUIManager.LoadingPanel;
            if (showLoading && loadingPanel != null)
            {
                loadingPanel.Speed = loadingScreenSpeed;
                loadingPanel.Show(completed: () =>
                {
                    SceneManager.LoadScene(sceneName);
                    loadingPanel.Hide();
                });
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        public void OnVibrate()
        {
            if(GAME_VIBRATION)
                Handheld.Vibrate();
        }
        public static void LoadGame(LoadGameData data, bool showLoading = true, float loadingScreenSpeed = 1f)
        {
            LoadGameData = data;
            LoadScene("Main", showLoading, loadingScreenSpeed);
        }

        private string RandomName()
        {
            var name = new string[]
            {"Rajesh", "Dylan", "Luke", "Gabriel","Jay", "Anthony", "Isaac", "Grayson", "Riya", "Levi", "Joseph",
                "Charlie", "Priya", "Kishan", "Raju", "Aniket", "Eklavy", "Sudhir", "Kashika", "Chandra","Rushil"};

        var surename = new string[] { "Chada","Sharma","Shukla","Shah", "Classic", "Jazz","Acharya","Bedi","Deshpande","Joshi","Naidu","Agarwal","Ganguly","Chopra","Patel","Gupta" };
            return name[Random.Range(0,name.Length)] +" "+surename[Random.Range(0,surename.Length)];
        }

    }
}

public struct LoadGameData
{
    public GameMode GameMode { get; set; }
    public Level Level { get; set; }
}