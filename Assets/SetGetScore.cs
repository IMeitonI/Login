using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetGetScore : MonoBehaviour
{
    [SerializeField]
    private string URL;
    int score;
    int actualScore;
    private void Start() {
        score = PlayerPrefs.GetInt("score");
    }
    private void Update() {
        if(actualScore> score) {
            StartCoroutine(SetScores(URL,actualScore,HttpManager.actualUser,HttpManager.Token));
        }
    }
    public IEnumerator SetScores(string URL,int newScore, UserData resData, string Token) {
        Debug.Log("PAcht" + resData.username);


        resData.score = newScore;     
        string postData = JsonUtility.ToJson(resData);
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

    void AddScore() {
        actualScore =+1;
    }
    
}
