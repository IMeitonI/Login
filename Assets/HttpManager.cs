using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HttpManager : MonoBehaviour {

    [SerializeField]
    private string URL;
    
    static public string Token;
    public string Username;
   
    public static  UserData actualUser;

    // Start is called before the first frame update
    void Start() {
       
        Token = PlayerPrefs.GetString("token");
        Username = PlayerPrefs.GetString("username");
       
        Debug.Log("TOKEN:" + Token);
        StartCoroutine(GetPerfil());
    }

   
   

    public void ClickSignUp() {
        string postData = FindButtons();
        StartCoroutine(SignUp(postData));
    }
    public void ClickLogIn() {
        string postData = FindButtons();
        StartCoroutine(LogIn(postData));
    }

    private string FindButtons() {
        AuthData data = new AuthData();

        data.username = GameObject.Find("InputFieldUsername").GetComponent<InputField>().text;
        data.password = GameObject.Find("InputFieldPassword").GetComponent<InputField>().text;

        string postData = JsonUtility.ToJson(data);
        return postData;
    }
    IEnumerator SignUp(string postData) {
        Debug.Log(postData);


        string url = URL + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError) {
            Debug.Log("NETWORK ERROR " + www.error);
        } else if (www.responseCode == 200) {
            //Debug.Log(www.downloadHandler.text);
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

            Debug.Log("Bienvenido " + resData.usuario.username + ", id:" + resData.usuario._id);
            StartCoroutine(LogIn(postData));

        } else {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }
    
    IEnumerator GetPerfil() {
        
        string url = URL + "/api/usuarios/" + Username;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("x-token", Token);
        yield return www.SendWebRequest();
        if (www.isNetworkError) {
            Debug.Log("NETWORK ERROR " + www.error);
        } else if (www.responseCode == 200) {
            Debug.Log(www.downloadHandler.text);
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
            actualUser = resData.usuario;
            
            Debug.Log("Token valido " + resData.usuario.username + ", id:" + resData.usuario._id + " y su score es: " + resData.usuario.score);
           SceneManager.LoadScene(1);
        } else {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }
    
    IEnumerator LogIn(string postData) {
        Debug.Log("LOG IN :" + postData);
        string url = URL + "/api/auth/login";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");
        yield return www.SendWebRequest();
        if (www.isNetworkError) {
            Debug.Log("NETWORK ERROR " + www.error);
        } else if (www.responseCode == 200) {
                   
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
            actualUser = resData.usuario;

            Debug.Log("Autenticado " + resData.usuario.username + ", id:" + resData.usuario._id);
            Debug.Log("TOKEN: " + resData.token);
            PlayerPrefs.SetString("token", resData.token);
            PlayerPrefs.SetString("username", resData.usuario.username);
            
            SceneManager.LoadScene("Game");
        } else {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }

   
}


[System.Serializable]
public class ScoreData {
    public int userId;
    public int value;
    public string name;

}

[System.Serializable]
public class Scores {
    public ScoreData[] scores;
}

[System.Serializable]
public class AuthData {
    public string username;
    public string password;
    public UserData usuario;
    public string token;
    
}
[System.Serializable]
public class Alldata {
    public UserData[] usuarios;
}

[System.Serializable]
public class UserData {
    public string _id;
    public string username;
    public bool estado;
    public int score;
}

