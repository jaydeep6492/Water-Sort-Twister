using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using dotmob;
using Game;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region Public Fields
    public static LevelManager Instance { get; private set; }
    public static event Action LevelCompleted;
    
    public int hint;
    public int undoCount;
    public int swapCount;
    public int addBottleCount;
    
    public GameMode GameMode { get; private set; } = GameMode.Easy;
    public Level Level { get; private set; }
    
    public State CurrentState { get; private set; } = State.None;

    public bool HaveUndo => _undoStack.Count > 0;
    public bool IsTransfer { get; set; }
    
    public GameObject swapBackground;
    #endregion
    
    #region Private Fields
    [SerializeField] private float _minXDistanceBetweenHolders;
    [SerializeField] private Camera _camera;
    private Holder _holderPrefab;

    [SerializeField] private AudioClip _winClip;
   
    

    private readonly List<Holder> _holders = new List<Holder>();

    private readonly Stack<MoveData> _undoStack = new Stack<MoveData>();

   
    private Dictionary<int, int> hintStore = new Dictionary<int, int>();
    
    private bool _swapAvail =false;
    
    #endregion
    
    #region Unity Callbacks
    private void Awake()
    {
        Instance = this;
        var loadGameData = GameManager.LoadGameData;
        _holderPrefab = GameManager.Instance.holder;
        GameMode = loadGameData.GameMode;
        Level = loadGameData.Level;
        LoadLevel();
        CurrentState = State.Playing;
        hint = GameManager.HINT_COUNT;
        undoCount = GameManager.UNDO_COUNT;
        swapCount = GameManager.SWAP_COUNT;
        addBottleCount = GameManager.ADD_BOTTLE;
    }
    
    private void Update()
    {
        if (CurrentState != State.Playing)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            var collider = Physics2D.OverlapPoint(_camera.ScreenToWorldPoint(Input.mousePosition));
            if (collider != null)
            {
                var holder = collider.GetComponent<Holder>();

                if (holder == null) return;
                if (_swapAvail)
                {
                    _swapAvail = false;
                    swapBackground.SetActive(false);
                    holder.SwapLiquid();
                    swapCount--;
                    GameManager.SWAP_COUNT = swapCount;
                }
                else
                {
                    OnClickHolder(holder);
                }
            }
        }
    }
    #endregion
    private void LoadLevel()
    {
        var list = PositionsForHolders(Level.map.Count, out var width).ToList();
        _camera.orthographicSize = 0.5f * width * Screen.height / Screen.width;

        var levelMap = Level.LiquidDataMap;
        for (var i = _holders.Count; i < levelMap.Count; i++)
        {
            var levelColumn = levelMap[i];
            var holder = Instantiate(_holderPrefab, list[i], Quaternion.identity);
            holder.Init(levelColumn);
            _holders.Add(holder);
        }

        if(_holders.Count<=0) return;
        for (var i = 0; i < _holders.Count; i++)
        {
            _holders[i].transform.position = list[i];
            _holders[i].OnUpdatePosition();
        }
    }

    public void OnAddBottle()
    {
        if (addBottleCount > 0)
        {
            Level.map.Add(new LevelColumn());
            LoadLevel();
            addBottleCount--;
        }
        else
        {
            if (!AdsManager.IsVideoAvailable())
            {
                SharedUIManager.PopUpPanel.ShowAsInfo("Notice", "Sorry no video ads available.Check your internet connection!");
                return;
            }

            SharedUIManager.PopUpPanel.ShowAsConfirmation("Add Bottle","Do You Want To Watch Video Ads To Add More Bottles", success =>
            {
               
                if (!success)
                {
                    return;
                }
                
                AdsManager.ShowVideoAds(true, s =>
                {
                    if(!s)
                        return;
                    Debug.Log("Call Back");
                    GameManager.ADD_BOTTLE += 1;
                    addBottleCount = GameManager.ADD_BOTTLE;
                    MainMenu.UIManager.Instance.OnShowMessage("Successfully Added");
                });
          
            });
        }
    }

    public void OnSwapLiquid()
    {
        if (swapCount > 0)
        {
            _swapAvail = !_swapAvail;
            swapBackground.SetActive(_swapAvail);
        }
        else
        {
            if (!AdsManager.IsVideoAvailable())
            {
                SharedUIManager.PopUpPanel.ShowAsInfo("Notice", "Sorry no video ads available.Check your internet connection!");
                return;
            }

            SharedUIManager.PopUpPanel.ShowAsConfirmation("Add Swap","Do You Want To Watch Video Ads To Add More Swap Color", success =>
            {
               
                if (!success)
                {
                    return;
                }
                
                AdsManager.ShowVideoAds(true, s =>
                {
                    if(!s)
                        return;
                    Debug.Log("Call Back");
                    GameManager.SWAP_COUNT += 2;
                    swapCount = GameManager.SWAP_COUNT;
                    MainMenu.UIManager.Instance.OnShowMessage("Successfully Added");
                });
          
            });
        }
    }
    public void OnHint(bool skipIndex)
    {
        if (hint <= 0)
        {
            if (!AdsManager.IsVideoAvailable())
            {
                SharedUIManager.PopUpPanel.ShowAsInfo("Notice", "Sorry no video ads available.Check your internet connection!");
                return;
            }

            SharedUIManager.PopUpPanel.ShowAsConfirmation("Add Hint","Do You Want To Watch Video Ads To Add More Hints", success =>
            {
               
                if (!success)
                {
                    return;
                }
                
                AdsManager.ShowVideoAds(true, s =>
                {
                    if(!s)
                        return;
                    Debug.Log("Call Back");
                    GameManager.HINT_COUNT += 2;
                    hint = GameManager.HINT_COUNT;
                    MainMenu.UIManager.Instance.OnShowMessage("Successfully Added");
                });
          
            });
            return;
        }
        Debug.Log($"Remaining Hint :: {hint-1}");
        while (true)
        {
            if (IsTransfer) return;
            Holder toHolder = null;
            Holder fromHolder = null;
            int I = -1, J = -1;
            for (var i = 0; i < _holders.Count; i++)
            {
                for (var j = i + 1; j < _holders.Count; j++)
                {
                    if (_holders[j].TopLiquid != null && _holders[i].TopLiquid.GroupId == _holders[j].TopLiquid.GroupId)
                    {
                        fromHolder = _holders[j];
                    }
                    else if (fromHolder != null && _holders[j].TopLiquid == null)
                    {
                        toHolder = _holders[j];
                        goto ShowResult;
                    }
                }
            }
            for (var i = 0; i < _holders.Count; i++)
            {
                for (var j = i + 1; j < _holders.Count; j++)
                {
                    if (skipIndex && hintStore.ContainsKey(i) && hintStore[i] == j)
                    {
                        continue;
                    }

                    if (_holders[j].TopLiquid == null)
                    {
                        fromHolder = _holders[i];
                        toHolder = _holders[j];
                        I = i;
                        J = j;
                        goto ShowResult;
                    }

                    if (_holders[i].TopLiquid.GroupId == _holders[j].TopLiquid.GroupId)
                    {
                        if (_holders[i].TopLiquid.Value > _holders[j].TopLiquid.Value)
                        {
                            if (_holders[i].CurrentTotal + _holders[j].TopLiquid.Value <= _holders[i].MAXValue)
                            {
                                fromHolder = _holders[j];
                                toHolder = _holders[i];
                                I = i;
                                J = j;
                                goto ShowResult;
                            }
                            else if (_holders[j].CurrentTotal < _holders[j].MAXValue)
                            {
                                fromHolder = _holders[i];
                                toHolder = _holders[j];
                                I = i;
                                J = j;
                                goto ShowResult;
                            }
                        }
                        else
                        {
                            if (_holders[j].CurrentTotal + _holders[i].TopLiquid.Value <= _holders[j].MAXValue)
                            {
                                fromHolder = _holders[i];
                                toHolder = _holders[j];
                                I = i;
                                J = j;
                                goto ShowResult;
                            }
                            else if (_holders[i].CurrentTotal < _holders[i].MAXValue)
                            {
                                fromHolder = _holders[j];
                                toHolder = _holders[i];
                                I = i;
                                J = j;
                                goto ShowResult;
                            }
                        }
                    }
                }
            }

            ShowResult :
            if (fromHolder == null || toHolder == null)
            {
                OnClickUndo();
                skipIndex = true;
                continue;
            }

            hintStore.Clear();
            hintStore.Add(I, J);
            OnClickHolder(fromHolder);
            OnClickHolder(toHolder, b =>
            {
                if (!b) return;
                hint--;
                GameManager.HINT_COUNT = hint;
                UIManager.Instance.gamePlayPanel.SetHintText();
            } );
            break;
        }
    }

    public void OnClickUndo()
    {
        if (CurrentState != State.Playing || !HaveUndo)
        {
            MainMenu.UIManager.Instance.OnShowMessage("Not Any Movement In Gameplay");
            return;
        }
        if (undoCount <= 0)
        {
            if (!AdsManager.IsVideoAvailable())
            {
                SharedUIManager.PopUpPanel.ShowAsInfo("Notice", "Sorry no video ads available.Check your internet connection!");
                return;
            }

            SharedUIManager.PopUpPanel.ShowAsConfirmation("Add Undo","Do You Want To Watch Video Ads To Add More Undo", success =>
            {
               
                if (!success)
                {
                    return;
                }
                
                AdsManager.ShowVideoAds(true, s =>
                {
                    if(!s)
                        return;
                    Debug.Log("Call Back");
                    GameManager.UNDO_COUNT += 2;
                    undoCount = GameManager.UNDO_COUNT;
                    MainMenu.UIManager.Instance.OnShowMessage("Successfully Added");
                });
          
            });
            return;
        }
        Debug.Log($"Remaining Undo :: {undoCount-1}");
        var moveData = _undoStack.Pop();
        moveData.FromHolder.AddLiquid(moveData.ToHolder.TopLiquid.GroupId,moveData.LiquidValue);
        moveData.ToHolder.RemoveAtLast(moveData.LiquidValue);
        moveData.ToHolder.endOfHolder.SetActive(moveData.ToHolder.TopLiquid != null && moveData.ToHolder.TopLiquid.Value == moveData.ToHolder.MAXValue);
        undoCount--;
        GameManager.UNDO_COUNT = undoCount;
    }

   

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnClickHolder(Holder holder,Action<bool> action = null)
    {
        /*if(IsTransfer)
            return;*/

        
            
        var pendingHolder = _holders.FirstOrDefault(h => h.IsPending);

        if (pendingHolder != null && pendingHolder != holder)
        {
            if(holder.TopLiquid != null && holder.TopLiquid.Value == holder.MAXValue) return;
            if (holder.TopLiquid == null ||
                (pendingHolder.TopLiquid.GroupId == holder.TopLiquid.GroupId && !holder.IsFull ))
            {
                var liquidValue = pendingHolder.TopLiquid.Value;
                //var liquidBool = pendingHolder.TopLiquid.IsBottomLiquid;
                IsTransfer = true;
                StartCoroutine(SimpleCoroutine.CoroutineEnumerator(pendingHolder.MoveAndTransferLiquid(holder, CheckAndGameOver),()=>
                {
                    
                    IsTransfer = false;
                    var moveData = new MoveData()
                    {
                        FromHolder = pendingHolder,
                        ToHolder = holder,
                        LiquidValue = liquidValue
                    };
                    _undoStack.Push(moveData);
                    if (holder.TopLiquid.Value == holder.MAXValue)
                    {
                        GameManager.Instance.OnVibrate();
                    }
                    holder.endOfHolder.SetActive(holder.TopLiquid.Value == holder.MAXValue);
                    action?.Invoke(true);
                }));
            }
            else
            {
                pendingHolder.ClearPending();
                holder.StartPending();
            }
        }
        else if (holder.Liquids.Any())
        {
            if (!holder.IsPending)
            {
                if(holder.TopLiquid.Value != holder.MAXValue)
                    holder.StartPending();
            }
            else
            {
                holder.ClearPending();
            }
        }
    }



    private void CheckAndGameOver()
    {
        

        if (
            _holders.All(holder =>
        {
            var liquids = holder.Liquids.ToList();
            Debug.Log(_holders.Count);
            return liquids.Count == 0 || liquids.Count == 1;
        }) &&
        _holders.Where(holder => holder.Liquids.Any()).GroupBy(holder => holder.Liquids.First().GroupId)
            .All(holders => holders.Count() == 1))
        {
            //Debug.Log("Xong 1 chai");
            OverTheGame();
        }
    }

    private void OverTheGame()
    {
        if (CurrentState != State.Playing)
            return;

        PlayClipIfCan(_winClip);
        CurrentState = State.Over;
        
        LevelCompleted?.Invoke();
        ResourceManager.CompleteLevel(GameMode, Level.no);
    }

    private void PlayClipIfCan(AudioClip clip, float volume = 0.35f)
    {
        if (!AudioManager.IsSoundEnable || clip == null)
            return;
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
    }

    public IEnumerable<Vector2> PositionsForHolders(int count, out float expectWidth)
    {
        expectWidth = 4 * _minXDistanceBetweenHolders;
        if (count <= 6)
        {
            var minPoint = transform.position - ((count - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right -
                           Vector3.up * 1f;

            expectWidth = Mathf.Max(count * _minXDistanceBetweenHolders, expectWidth);

            return Enumerable.Range(0, count)
                .Select(i => (Vector2) minPoint + i * _minXDistanceBetweenHolders * Vector2.right);
        }

        var aspect = (float) Screen.width / Screen.height;

        var maxCountInRow = Mathf.CeilToInt(count / 2f);

        if ((maxCountInRow + 1) * _minXDistanceBetweenHolders > expectWidth)
        {
            expectWidth = (maxCountInRow + 1) * _minXDistanceBetweenHolders;
        }

        var height = expectWidth / aspect;


        var list = new List<Vector2>();
        var topRowMinPoint = transform.position + Vector3.up * height / 6f -
                             ((maxCountInRow - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right - Vector3.up * 1f;
        list.AddRange(Enumerable.Range(0, maxCountInRow)
            .Select(i => (Vector2) topRowMinPoint + i * _minXDistanceBetweenHolders * Vector2.right));

        var lowRowMinPoint = transform.position - Vector3.up * height / 6f -
                             (((count - maxCountInRow) - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right -
                             Vector3.up * 1f;
        list.AddRange(Enumerable.Range(0, count - maxCountInRow)
            .Select(i => (Vector2) lowRowMinPoint + i * _minXDistanceBetweenHolders * Vector2.right));

        return list;
    }


    public enum State
    {
        None,
        Playing,
        Over
    }

    public struct MoveData
    {
        public Holder FromHolder { get; set; }
        public Holder ToHolder { get; set; }
        public float LiquidValue { get; set; }
    }
}

[Serializable]
public struct LevelGroup : IEnumerable<Level>
{
    public List<Level> levels;

    public IEnumerator<Level> GetEnumerator()
    {
        return levels?.GetEnumerator() ?? Enumerable.Empty<Level>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

[Serializable]
public struct Level
{
    public int no;
    public List<LevelColumn> map;

    public List<IEnumerable<LiquidData>> LiquidDataMap => map.Select(GetLiquidDatas).ToList();

    public static IEnumerable<LiquidData> GetLiquidDatas(LevelColumn column)
    {
        var list = column.ToList();

        for (var j = 0; j < list.Count; j++)
        {
            var currentGroup = list[j];
            var count = 0;
            for (; j < list.Count; j++)
            {
                if (currentGroup == list[j])
                {
                    count++;
                }
                else
                {
                    j--;
                    break;
                }
            }

            yield return new LiquidData
            {
                groupId = currentGroup,
                value = count
            };
        }
    }
}

public struct LiquidData
{
    public int groupId;
    public float value;
}

[Serializable]
public struct LevelColumn : IEnumerable<int>
{
    public List<int> values;

    public IEnumerator<int> GetEnumerator()
    {
        return values?.GetEnumerator() ?? Enumerable.Empty<int>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}