using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SetGetScore : MonoBehaviour
{
    [SerializeField]
    private string URL;
    [SerializeField] Text text ;
    
    int actualScore=0;
    private void Start() {
        text.text = "0";
        
        Debug.Log("Holaaa " + HttpManager.actualUser.score);
    }
    
    public IEnumerator SetScores(string URL,int newScore, UserData resData, string Token) {
        Debug.Log("PAcht" + resData.username);


        resData.score = newScore;     
        string postData = JsonUtility.ToJson(resData);
        Debug.Log("akakakjakjak    " + postData);
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
    IEnumerator GetScores(string Token) {
        string url = URL + "/api/usuarios" + "?limit=5&sort=true";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.method = "GET";
        www.SetRequestHeader("content-type", "application/json");
        www.SetRequestHeader("x-token", Token);

        yield return www.SendWebRequest();

        if (www.isNetworkError) {
            Debug.Log("NETWORK ERROR " + www.error);
        } else if (www.responseCode == 200) {
            //Debug.Log(www.downloadHandler.text);
            Scores resData = JsonUtility.FromJson<Scores>(www.downloadHandler.text);
            print(resData.scores);
            //for (int i = 0; i < resData.allScores.Length; i++) {
            //    print("nombre: " + resData.allScores[i].username + " Puntaje: " + resData.allScores[i].score);

            //}
            //foreach (UserData score in resData.allScores) {
            //    Debug.Log(score.username + " | " + score.score);
            //}
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
            StartCoroutine(SetScores(URL, actualScore, HttpManager.actualUser, HttpManager.Token));
        }
    }

    public void Puntajes() {
        StartCoroutine(GetScores(HttpManager.Token));
    }
    
}
