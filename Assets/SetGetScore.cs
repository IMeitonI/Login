using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SetGetScore : MonoBehaviour
{
   
    [SerializeField] Text[] scores = new Text[5];
    [SerializeField]
    private string URL;
    [SerializeField] Text text ;
    public string Token;
    string actualToken;
    int actualScore=0;
    private void Start() {
        text.text = "0";
        Token = PlayerPrefs.GetString("token");
        actualToken = Token;
        Debug.Log("Holaaa " + HttpManager.actualUser.score+ "Token: "+ Token);
    }
    
    public IEnumerator SetScores(string URL,int newScore, UserData resData, string Token) {
        Debug.Log("PAcht" + resData.username);


        resData.score = newScore;     
        string postData = JsonUtility.ToJson(resData);
        //Debug.Log("akakakjakjak    " + postData);
        string url = URL + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "PATCH";
        www.SetRequestHeader("content-type", "application/json");
        www.SetRequestHeader("x-token", Token);
        yield return www.SendWebRequest();

        if (www.isNetworkError) {
            Debug.Log("NETWORK ERROR " + www.error);
        } else if (www.responseCode == 200) {
            //Debug.Log(www.downloadHandler.text);
            AuthData resData2 = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
            
            Debug.Log("Cambi'e el puntaje" + resData2.usuario.score);
           
        } else {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }
    IEnumerator GetScores( string Token) {
        string url = URL + "/api/usuarios?limit=5&sort=true" ;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.method = "GET";
        www.SetRequestHeader("content-type", "application/json");
        www.SetRequestHeader("x-token", Token);
        yield return www.SendWebRequest();

        if (www.isNetworkError) {
            Debug.Log("NETWORK ERROR " + www.error);
        } else if (www.responseCode == 200) {
           
            Alldata resData = JsonUtility.FromJson<Alldata>(www.downloadHandler.text);
            
            for (int i = 0; i < resData.usuarios.Length; i++) {
                scores[i].text= resData.usuarios[i].username + " - " + resData.usuarios[i].score;

            }
            
        } else {
            Debug.Log(www.error);
        }
    }

    public void AddScore() {
       
        actualScore +=1;
        text.text = actualScore.ToString();
    }

    public void Stop() {
        if (actualScore > HttpManager.actualUser.score) {
            StartCoroutine(SetScores(URL, actualScore, HttpManager.actualUser, Token));
        }
    }

    public void Puntajes() {
        Token = PlayerPrefs.GetString("token");
        
        
        StartCoroutine(GetScores(Token));
    }

    public void Exit() {
        PlayerPrefs.SetString("token", null);
        PlayerPrefs.SetString("username", null);
        SceneManager.LoadScene(0);
    }
    
}
