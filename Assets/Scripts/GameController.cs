using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public SoundController SoundController;

    public SpriteRenderer ScoreSprite1;
    public SpriteRenderer ScoreSprite2;
    public SpriteRenderer ScoreSprite3;
    
    public Sprite sprite_0;
    public Sprite sprite_1;
    public Sprite sprite_2;
    public Sprite sprite_3;
    public Sprite sprite_4;
    public Sprite sprite_5;
    public Sprite sprite_6;
    public Sprite sprite_7;
    public Sprite sprite_8;
    public Sprite sprite_9;

    public int Score;
    public GameObject CoinPrefab;
    public GameObject Respawn;
    public CharacterController Player;
    public List<GameObject> CoinSpawns;
    public static GameController Instance { get; private set; }
    
    private void SpawnRandomCoin(){
        var randomValueFrom0to1 = Random.value;
        var numberOfCoins = CoinSpawns.Count;
        int randomCoin = (int)(randomValueFrom0to1 * numberOfCoins);
        var coinSpawn = CoinSpawns[randomCoin];
        var coin = Instantiate(CoinPrefab);
        coin.transform.position = coinSpawn.transform.position;
    }

    private void IntToScoreSprites(int score){ //134
        int n = (score % 100); //34
        int m = (n % 10); // 4
        int value1 = (score - n)/100; // (134 - 34)/100 = 100/100 = 1
        int value2 = (n-m)/10; // (34 - 4)/10 = 30 / 10 = 3
        int value3 = m; // 4
        AssignSpriteToScoreSprite(value1, ScoreSprite1);
        AssignSpriteToScoreSprite(value2, ScoreSprite2);
        AssignSpriteToScoreSprite(value3, ScoreSprite3);
    }

    private void AssignSpriteToScoreSprite(int number, SpriteRenderer spriteRenderer)
    {
        switch(number){
            case 0:
                spriteRenderer.sprite = sprite_0;
                break;  
            case 1:
                spriteRenderer.sprite = sprite_1;
                break;  
            case 2:
                spriteRenderer.sprite = sprite_2;
                break;  
            case 3:
                spriteRenderer.sprite = sprite_3;
                break;  
            case 4:
                spriteRenderer.sprite = sprite_4;
                break;  
            case 5:
                spriteRenderer.sprite = sprite_5;
                break;  
            case 6:
                spriteRenderer.sprite = sprite_6;
                break;  
            case 7:
                spriteRenderer.sprite = sprite_7;
                break;  
            case 8:
                spriteRenderer.sprite = sprite_8;
                break;  
            case 9:
                spriteRenderer.sprite = sprite_9;
                break; 
        }
    }
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        IntToScoreSprites(Score);
        SpawnRandomCoin();
    }
   
    public void GetCoin(){
        SoundController.PlayCoinPickupSFX();

        Score++;
        IntToScoreSprites(Score);
        SpawnRandomCoin();
    }

    public void TouchLava(){
        SoundController.PlayExplodeSFX();
        Debug.Log("TouchedLava");
        Score = 0;
        IntToScoreSprites(Score);
        
        Player.enabled = false;
        Player.transform.position = Respawn.transform.position;
        Player.enabled = true;

    }
}
