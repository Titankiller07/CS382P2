using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameMode{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S; 

    [Header("Inscribed")]
    public TextMeshProUGUI uitLevel;
    public TextMeshProUGUI uitShots;
    public TextMeshProUGUI uitHighScore;
    public Vector3 castlePos;
    public GameObject[] castles;

    [Header("Dynamic")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";

    private Dictionary<int, int> levelHighScores;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        S = this;

        level = 0;
        shotsTaken = 0;
        levelMax = castles.Length;
        levelHighScores = new Dictionary<int, int>();
        LoadHighScores();
        StartLevel();
    }

    void StartLevel(){
        if(castle != null){
            Destroy(castle);
        }

        Projectile.DESTROY_PROJECTILES();

        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;

        Goal.goalMet = false;

        shotsTaken = 0;

        UpdateGUI();

        mode = GameMode.playing;
        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
    }

    void UpdateGUI(){
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken;

        if(levelHighScores.ContainsKey(level)){
            uitHighScore.text = "High Score: " + levelHighScores[level];
        }
        else{
            uitHighScore.text = "High Score: ";
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGUI();

        if((mode == GameMode.playing) && Goal.goalMet){
            mode = GameMode.levelEnd;
            FollowCam.SWITCH_VIEW(FollowCam.eView.both);
            UpdateHighScore();
            Invoke("NextLevel", 2f);
        }   
    }
    void UpdateHighScore()
    {
        // Check if the current level has a high score
        if (levelHighScores.ContainsKey(level))
        {
            // Update the high score if the current shots are less than the stored high score
            if (shotsTaken < levelHighScores[level])
            {
                levelHighScores[level] = shotsTaken;
            }
        }
        else
        {
            // If no high score exists for this level, add the current shots as the high score
            levelHighScores.Add(level, shotsTaken);
        }
        SaveHighScores();
    }
    void NextLevel(){
        level++;
        if(level == levelMax){
            SceneManager.LoadScene(1);
        }
        StartLevel();
    }

    static public void SHOT_FIRED(){
        S.shotsTaken++;
    }

    static public GameObject GET_CASTLE(){
        return S.castle;
    } 

    void SaveHighScores(){
        foreach (var entry in levelHighScores)
        {
            PlayerPrefs.SetInt("HighScore" + entry.Key, entry.Value);
        }
        PlayerPrefs.Save();
    }

    void LoadHighScores(){
        for (int i = 0; i < levelMax; i++)
        {
            if (PlayerPrefs.HasKey("HighScore" + i))
            {
                 levelHighScores[i] = PlayerPrefs.GetInt("HighScore" + i);
            }
        }
    }
}
